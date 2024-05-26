using LibrarySystemApi.Models;
using LibrarySystemApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystemApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooklistController : ControllerBase
{
    private readonly BooksService _booksService;

    public BooklistController(BooksService booksService)
    {
        _booksService = booksService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Book>>> Get() => 
        await _booksService.GetAsync();

    [HttpGet("{barcode}")]
    public async Task<ActionResult<Book>> Get(string barcode)
    {
        var book = await _booksService.GetAsync(barcode);

        if (book is null) {
            return NotFound();
        }

        return book;
    }
}