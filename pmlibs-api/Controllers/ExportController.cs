using System.IO.Compression;
using LibrarySystemApi.Models;
using LibrarySystemApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystemApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExportController : ControllerBase
{
    private readonly SessionsService _sessionsService;
    private readonly BookshelvesService _bookshelvesService;

    public ExportController(SessionsService sessionsService, BookshelvesService booksehlvesService)
    {
        _sessionsService = sessionsService;
        _bookshelvesService = booksehlvesService;
    }

    [HttpGet]
    public async Task<ActionResult> Get() {
        var session = await _sessionsService.GetActiveAsync();

        // check if the session exists and the session is active or not
        if (session is null)
        {
            return NotFound();
        }

        DateTimeOffset dto = new(DateTime.Now);
        string newExportName = $"exports/export_{dto.ToUnixTimeSeconds()}.zip";

        using (FileStream newZipStream = new(newExportName, FileMode.OpenOrCreate))
        {
            using (ZipArchive archive = new(newZipStream, ZipArchiveMode.Update))
            {
                foreach (BookshelfGroup bookshelfGroup in session.AllBookshelfGroups)
                {
                    // create directory for this group
                    string bookshelfGroupName = bookshelfGroup.GroupName;
                    archive.CreateEntry($"{bookshelfGroupName}/");

                    // fetch bookshelves of this group
                    List<Bookshelf> bookshelves = await _bookshelvesService.GetGroupBookshelvesAsync(session.Id!, bookshelfGroupName);

                    // iterate the bookshelf and write all the stuff to a file
                    foreach (Bookshelf bookshelf in bookshelves)
                    {
                        // create entry
                        ZipArchiveEntry newBookshelfEntry = archive.CreateEntry($"{bookshelfGroupName}/{bookshelfGroupName}-{bookshelf.ShelfNumber}.txt");
                        using (StreamWriter writer = new(newBookshelfEntry.Open()))
                        {
                            foreach (BookInput bookInput in bookshelf.AllBooks)
                            {
                                await writer.WriteLineAsync(bookInput.Barcode);
                            }
                        }
                    }
                }
            }
        }

        return File(await System.IO.File.ReadAllBytesAsync(newExportName), "application/zip");
    }
}