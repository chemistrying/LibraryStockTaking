using LibrarySystemApi.Models;
using LibrarySystemApi.Services;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace LibrarySystemApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReloadController : ControllerBase
{
    private readonly BooksService _booksService;

    public ReloadController(BooksService booksService)
    {
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
        
        return Ok();
    }
}