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

        // if the directory doesn't exist, create it
        if (!Directory.Exists("exports"))
        {
            Directory.CreateDirectory("exports");
        }

        // clean all files to prevent cluttering
        DirectoryInfo di = new($"{Globals.Config.DefaultSaveLocation}/");
        if (di.GetFiles().Length >= 100) {
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete(); 
            }
        } 

        // create new zip
        DateTimeOffset dto = new(DateTime.Now);
        string newExportName = $"export_{dto.ToUnixTimeSeconds()}.zip";
        string newExportFilePath = Path.Combine(Globals.Config.DefaultSaveLocation, newExportName);

        using (FileStream newZipStream = new(newExportFilePath, FileMode.OpenOrCreate))
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
                        string newFilePath = Path.Combine(bookshelfGroupName, $"{bookshelfGroupName}-{bookshelf.ShelfNumber}.txt");
                        ZipArchiveEntry newBookshelfEntry = archive.CreateEntry(newFilePath);
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

        return File(await System.IO.File.ReadAllBytesAsync(newExportFilePath), "application/zip", newExportName);
    }
}