using LibrarySystemApi.Models;
using LibrarySystemApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Newtonsoft.Json;

namespace LibrarySystemApi.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly SessionsService _sessionsService;
    private readonly BookshelvesService _bookshelvesService;

    public SessionsController(SessionsService sessionsService, BookshelvesService booksehlvesService)
    {
        _sessionsService = sessionsService;
        _bookshelvesService = booksehlvesService;
    }
    
    [HttpGet]
    public async Task<List<StockTakingSession>> Get() =>
        await _sessionsService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<StockTakingSession>> Get(string id)
    {
        var session = await _sessionsService.GetAsync(id);

        if (session is null)
        {
            return NotFound();
        }

        return session;
    }

    [HttpPost]
    public async Task<IActionResult> Post(string sessionName, string description)
    {
        var pregenBookshelfGroups = new List<BookshelfGroup>();

        var bookshelfTreeProfile = JsonConvert.DeserializeObject<Dictionary<string, int>>(System.IO.File.ReadAllText(Path.Combine(Globals.Config.DefaultProgramFilesLocation, $"profile_v{Globals.Config.BookshelfTreeProfile}.json")))!;
            
        // create a blank session first
        StockTakingSession newSession = new();

        await _sessionsService.CreateAsync(newSession);

        foreach (KeyValuePair<string, int> entry in bookshelfTreeProfile)
        {
            // create bookshelf list
            List<Bookshelf> allBookshelves = [];
            List<string> allBookshelvesId = [];
            for (int i = 1; i <= entry.Value; i++)
            {
                allBookshelves.Add(new()
                {
                    SessionId = newSession.Id!,
                    GroupName = entry.Key,
                    ShelfNumber = i,
                    Description = $"{entry.Key}-{i}",
                    Status = 0,
                    AllBooks = []
                });
            }

            // add to database
            await _bookshelvesService.CreateManyAsync(allBookshelves);

            // update the IDs
            for (int i = 0; i < entry.Value; i++)
            {
                allBookshelvesId.Add(allBookshelves[i].Id!);
            }

            pregenBookshelfGroups.Add(new()
            {
                GroupName = entry.Key,
                AllBookshelvesId = allBookshelvesId
            });
        }

        newSession = new()
        {
            Id = newSession.Id,
            SessionName = sessionName,
            Description = description,
            StartDate = DateTime.Now,
            IsActive = false,
            AllBookshelfGroups = pregenBookshelfGroups
        };

        await _sessionsService.UpdateAsync(newSession.Id!, newSession);

        return CreatedAtAction(nameof(Get), new { id = newSession.Id }, newSession);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, string sessionName, string description)
    {
        var session = await _sessionsService.GetAsync(id);

        if (session is null)
        {
            return NotFound();
        }

        session.SessionName = sessionName;
        session.Description = description;

        await _sessionsService.UpdateAsync(id, session);

        return Ok();
    }
    
    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var session = await _sessionsService.GetAsync(id);

        if (session is null)
        {
            return NotFound();
        }

        // remove bookshelves as well
        foreach (BookshelfGroup bookshelfGroup in session.AllBookshelfGroups)
        {
            await _bookshelvesService.RemoveManyAsync(bookshelfGroup.GroupName);
        }

        await _sessionsService.RemoveAsync(id);

        return NoContent();
    }
}