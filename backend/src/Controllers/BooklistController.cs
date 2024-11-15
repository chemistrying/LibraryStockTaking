using LibrarySystemApi.Models;
using LibrarySystemApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LibrarySystemApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BooklistController : ControllerBase
{
    private readonly BooksService _booksService;

    public BooklistController(BooksService booksService)
    {
        _booksService = booksService;
    }

    [Authorize(Policy = "AdminOnly")]
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