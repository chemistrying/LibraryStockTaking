using LibrarySystemApi.Models;
using LibrarySystemApi.Services;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace LibrarySystemApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgressController : ControllerBase
{
    private readonly SessionsService _sessionsService;
    private readonly BookshelvesService _bookshelvesService;

    public ProgressController(SessionsService sessionsService, BookshelvesService booksehlvesService)
    {
        _sessionsService = sessionsService;
        _bookshelvesService = booksehlvesService;
    }

    [HttpGet("{sessionId:length(24)}")]
    public async Task<ActionResult<double>> Get(string sessionId) {
        var session = await _sessionsService.GetAsync(sessionId);

        if (session is null)
        {
            return NotFound();
        }

        List<Bookshelf> bookshelves = await _bookshelvesService.GetSessionBookshelvesAsync(sessionId);
        
        double progressPercentage = 100.0 * bookshelves.Count(x => x.Status == StocktakeStatusCode.Finished) / bookshelves.Count;

        return progressPercentage;
    }

    [HttpGet("{sessionId:length(24)}/{groupName}")]
    public async Task<ActionResult<double>> Get(string sessionId, string groupName) {
        var session = await _sessionsService.GetAsync(sessionId);

        if (session is null)
        {
            return NotFound();
        }
        else if (session.AllBookshelfGroups.Find(x => x.GroupName == groupName) is null)
        {
            return NotFound();
        }


        List<Bookshelf> bookshelves = await _bookshelvesService.GetGroupBookshelvesAsync(sessionId, groupName);

        double progressPercentage = 100.0 * bookshelves.Count(x => x.Status == StocktakeStatusCode.Finished) / bookshelves.Count;

        return progressPercentage;

    }
}