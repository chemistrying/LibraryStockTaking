using LibrarySystemApi.Models;
using LibrarySystemApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;

namespace LibrarySystemApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StocktakeController : ControllerBase
{
    private readonly SessionsService _sessionsService;
    private readonly BookshelvesService _bookshelvesService;
    private readonly BooksService _booksService;
    private readonly List<string> _possibleOperations = ["add", "delete", "start", "finish"];
    private readonly Regex _trimmedBarcodeRegexPattern = new(@"^(C?)(\d{1,4})$");

    public StocktakeController(SessionsService sessionsService, BookshelvesService booksehlvesService, BooksService booksService)
    {
        _sessionsService = sessionsService;
        _bookshelvesService = booksehlvesService;
        _booksService = booksService;
    }

    private async Task<ObjectResult> VerifySessionAndBookshelf(StocktakePayload stocktakePayload)
    {
        var session = await _sessionsService.GetAsync(stocktakePayload.SessionId);

        // check if the session exists and the session is active or not
        if (session is null)
        {
            return NotFound("");
        }
        else if (!session.IsActive)
        {
            return StatusCode(StatusCodes.Status403Forbidden, "The session is currently inactive");
        }

        // check if the bookshelf is in the session
        bool bookshelfInSession = false;
        foreach (var bookshelfGroup in session.AllBookshelfGroups)
        {
            if (bookshelfGroup.AllBookshelvesId.Find(x => x.Equals(stocktakePayload.BookshelfId)) is not null)
            {
                bookshelfInSession = true;
            }
        }

        // bookshelf id and session id not match (or one of them is wrong)
        if (!bookshelfInSession)
        {
            return NotFound("");
        }

        var bookshelf = await _bookshelvesService.GetAsync(stocktakePayload.BookshelfId);

        // ensure the bookshelf is really there
        if (bookshelf is null)
        {
            return NotFound("");
        }

        return Ok("");
    }

    private ObjectResult VerifyPayload(StocktakePayload stocktakePayload, Bookshelf bookshelf)
    {
        stocktakePayload.Operation = stocktakePayload.Operation.ToLower();

        if (_possibleOperations.Find(x => x == stocktakePayload.Operation) is null)
        {
            return BadRequest("Invalid operation (only methods 'add', 'delete', 'start' or 'finish' are allowed)");
        }
        else if (bookshelf.Status == StocktakeStatusCode.Finished)
        {
            return BadRequest("This bookshelf has been stock-taken");
        }
        else if ((stocktakePayload.Operation == "add" || stocktakePayload.Operation == "delete") && stocktakePayload.Barcode is null)
        {
            return BadRequest("Barcode is needed for 'add' or 'delete' operation");
        }
        else if (stocktakePayload.Operation == "start" && bookshelf.Status != StocktakeStatusCode.NotStarted)
        {
            return BadRequest("You cannot start an already-started bookshelf stock-taking subsession");
        }
        else if (stocktakePayload.Operation == "finish" && bookshelf.Status == StocktakeStatusCode.NotStarted)
        {
            return BadRequest("You cannot finish a bookshelf stock-taking subsession that hasn't even started");
        }

        return Ok("");
    }

    private string PrehandleBarcode(string barcode)
    {
        if (Globals.Config.AutoCapitalize)
        {
            barcode = barcode.ToUpper();
        }

        if (Globals.Config.AutoZero)
        {
            Match match = _trimmedBarcodeRegexPattern.Match(barcode);
            if (match.Success)
            {
                barcode = match.Groups[1].Value + match.Groups[2].Value.PadLeft(5, '0');
            }
        }

        // Serilog.Log.Information(barcode);
        return barcode;
    }

    private async Task<ActionResult<StocktakeResponse>> PerformAction(StocktakePayload stocktakePayload, Bookshelf bookshelf)
    {
        var operation = stocktakePayload.Operation!;
        var barcode = stocktakePayload.Barcode;
        Book? book;
        StocktakeResponse stocktakeResponse = new()
        {
            Verdict = StocktakeVerdict.Ok,
            Message = ""
        };
        
        switch (operation)
        {
            case "add":
                barcode = PrehandleBarcode(barcode!);

                var prevBook = bookshelf.AllBooks.Count == 0 ? null : await _booksService.GetAsync(bookshelf.AllBooks.Last().Barcode);
                
                BookInput newBookInput = new()
                {
                    Barcode = barcode,
                    InputTime = DateTime.Now
                };

                // TODO: Prehandle the barcode
                bookshelf.Status = StocktakeStatusCode.InProgress;

                // check barcode validity
                book = await _booksService.GetAsync(barcode);
                if (book is null)
                {
                    stocktakeResponse.Verdict = StocktakeVerdict.Error;
                    stocktakeResponse.Message = $"Book '{barcode}' is not a valid entry in the booklist. Are you sure that you have inputted the correct barcode?";
                }
                else if (book.Status != "A" && book.Status != "B")
                {
                    stocktakeResponse.Verdict = StocktakeVerdict.Warning;
                    stocktakeResponse.Message = $"Book '{barcode}' has an abnormal status code '{book.Status}', might be a sign of wrong barcode?";
                }
                else if (prevBook is not null && prevBook.Callno1 != book.Callno1)
                {
                    stocktakeResponse.Verdict = StocktakeVerdict.Warning;
                    stocktakeResponse.Message = $"Book '{barcode}' has a different category comparing to the previous book (current: {book.Callno1}, previous: {prevBook.Callno1}), might be a sign of wrong barcode?";
                }
                else if (bookshelf.AllBooks.Find(x => x.Barcode == barcode) is not null)
                {
                    stocktakeResponse.Verdict = StocktakeVerdict.Warning;
                    stocktakeResponse.Message = "You have already stock-taken this book. Please remove it to prevent duplicates.";
                }
                else if (Globals.DuplicationList.Query(barcode) >= 1)
                {
                    // obtain all bookshelves from that session
                    List<Bookshelf> allBookshelves = await _bookshelvesService.GetSessionBookshelvesAsync(bookshelf.SessionId);

                    var other = allBookshelves.Find(x => x.AllBooks.Find(y => y.Barcode == barcode) != null);

                    if (other is null)
                    {
                        Serilog.Log.Fatal("Trie does not match database.");
                        stocktakeResponse.Verdict = StocktakeVerdict.Error;
                        stocktakeResponse.Message = "The system has encountered an abnormal bug. Please report this issue to administrator / Github repo.";
                    }
                    else
                    {
                        stocktakeResponse.Verdict = StocktakeVerdict.Warning;
                        stocktakeResponse.Message = $"Book '{barcode} has been stock-taken in other bookshelves (aka {other.GroupName}-{other.ShelfNumber}), are you sure you have entered the correct barcode?";
                    }
                }
                else
                {
                    stocktakeResponse.Message = $"Book '{barcode}' has been successfully added.";
                }
                
                if (book is not null)
                {
                    stocktakeResponse.BookInformation = book.ToStandardFormat();

                    // add to duplication list
                    Globals.DuplicationList.Insert(barcode);
                }

                newBookInput.ReturnedResponse = stocktakeResponse;
                
                bookshelf.AllBooks.Add(newBookInput);
                break;
            case "delete":
                barcode = PrehandleBarcode(barcode!);

                var targetIndex = bookshelf.AllBooks.FindLastIndex(x => x.Barcode == barcode);
                if (targetIndex == -1)
                {
                    stocktakeResponse.Verdict = StocktakeVerdict.Error;
                    stocktakeResponse.Message = $"Barcode {barcode} cannot found in the bookshelf.";
                }
                else
                {
                    bookshelf.AllBooks.RemoveAt(targetIndex);

                    // remove from duplication list if and only if the barcode is valid
                    book = await _booksService.GetAsync(barcode);
                    if (book is not null)
                    {
                        Globals.DuplicationList.Delete(barcode);
                    }

                    stocktakeResponse.Message = $"Barcode {barcode} has been deleted successfully.";
                }

                bookshelf.Status = StocktakeStatusCode.InProgress;
                break;
            case "start":
                bookshelf.Status = StocktakeStatusCode.InProgress;
                bookshelf.StartTime = DateTime.Now;
                stocktakeResponse.Message = $"Bookshelf {bookshelf.GroupName}-{bookshelf.ShelfNumber} is now being stock-taken at {bookshelf.StartTime}.";
                break;
            case "finish":
                bookshelf.Status++;
                if (bookshelf.Status == StocktakeStatusCode.FinalChecking)
                {
                    stocktakeResponse.Verdict = StocktakeVerdict.Warning;
                    stocktakeResponse.Message = $"Please double check the number of books in the bookshelf. Current number of books: {bookshelf.AllBooks.Count}.";
                }
                else
                {
                    bookshelf.EndTime = DateTime.Now;
                    stocktakeResponse.Message = $"Bookshelf {bookshelf.GroupName}-{bookshelf.ShelfNumber} has been stock-taken at {bookshelf.EndTime}.";
                }
                break;
        }

        // update bookshelf
        await _bookshelvesService.UpdateAsync(bookshelf.Id!, bookshelf);

        return stocktakeResponse;
    }

    [HttpPatch]
    public async Task<ActionResult<StocktakeResponse>> Patch(StocktakePayload stocktakePayload)
    {
        var validSessionAndBookshelf = await VerifySessionAndBookshelf(stocktakePayload);

        if (validSessionAndBookshelf.StatusCode != StatusCodes.Status200OK)
        {
            return validSessionAndBookshelf;
        }


        var bookshelf = await _bookshelvesService.GetAsync(stocktakePayload.BookshelfId);

        // cross check bookshelf
        if (bookshelf is null)
        {
            return NotFound();
        }

        // verify operation paylaod
        var validPayload = VerifyPayload(stocktakePayload, bookshelf);
        
        if (validPayload.StatusCode != StatusCodes.Status200OK)
        {
            return validPayload;
        }

        // perform operations

        var stocktakeResponse = await PerformAction(stocktakePayload, bookshelf);

        return stocktakeResponse;
    }
}