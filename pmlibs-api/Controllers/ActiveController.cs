using LibrarySystemApi.Models;
using LibrarySystemApi.Services;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace LibrarySystemApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActiveController : ControllerBase
{
    private readonly SessionsService _sessionsService;
    private readonly BookshelvesService _bookshelvesService;

    public ActiveController(SessionsService sessionsService, BookshelvesService booksehlvesService)
    {
        _sessionsService = sessionsService;
        _bookshelvesService = booksehlvesService;
    }

    [HttpGet]
    public async Task<ActionResult<StockTakingSession?>> Get() =>
        await _sessionsService.GetActiveAsync();

    [HttpPost("{id:length(24)}")]
    public async Task<IActionResult> Post(string id)
    {
        var session = await _sessionsService.GetAsync(id);

        // check if the session exists and the session is active or not
        if (session is null)
        {
            return NotFound();
        }
        else if (session.IsActive)
        {
            return Ok();
        }

        var oldSession = await _sessionsService.GetActiveAsync();

        if (oldSession is not null)
        {
            oldSession.IsActive = false;
            await _sessionsService.UpdateAsync(oldSession.Id!, oldSession);
        }

        session.IsActive = true;
        await _sessionsService.UpdateAsync(id, session);
        
        return Ok();
    }
}