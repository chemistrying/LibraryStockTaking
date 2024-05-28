using LibrarySystemApi.Models;
using LibrarySystemApi.Services;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace LibrarySystemApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReloadController : ControllerBase
{
    private readonly SessionsService _sessionsService;
    private readonly BookshelvesService _bookshelvesService;
    private readonly BooksService _booksService;

    public ReloadController(SessionsService sessionsService, BookshelvesService bookshelvesService, BooksService booksService)
    {
        _sessionsService = sessionsService;
        _bookshelvesService = bookshelvesService;
        _booksService = booksService;
    }

    [HttpPost]
    public async Task<IActionResult> Post()
    {
        // reload config
        Globals.Config = JsonConvert.DeserializeObject<Config>(System.IO.File.ReadAllText("config.json"))!;

        // reload booklist
        await _booksService.RemoveAllAsync();
        List<Book> newBooklist = [];
        using (StreamReader sr = new(Path.Combine(Globals.Config.DefaultProgramFilesLocation, "booklist.txt")))
        {
            sr.ReadLine();
            while (!sr.EndOfStream)
            {
                string[] blocks = sr.ReadLine().Split("| ");
                Book book = new(blocks.Select(s => s.Trim()).ToArray());
                newBooklist.Add(book);
            }
        }

        await _booksService.CreateAsync(newBooklist);

        // update duplication list
        Globals.DuplicationList = new Trie();

        var session = await _sessionsService.GetActiveAsync();

        if (session is not null)
        {
            List<Bookshelf> bookshelves = await _bookshelvesService.GetSessionBookshelvesAsync(session.Id!);
            foreach (var bookshelf in bookshelves)
            {
                foreach (var bookInput in bookshelf.AllBooks)
                {
                    // do not insert erroneous barcode
                    if (bookInput.ReturnedResponse.Verdict != StocktakeVerdict.Error)
                    {
                        Globals.DuplicationList.Insert(bookInput.Barcode);
                    }
                }
            }
        }
        
        return Ok();
    }
}