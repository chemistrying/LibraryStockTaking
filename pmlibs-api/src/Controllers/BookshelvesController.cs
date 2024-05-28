using LibrarySystemApi.Models;
using LibrarySystemApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystemApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookshelvesController : ControllerBase
{
    private readonly SessionsService _sessionsService;
    private readonly BookshelvesService _bookshelvesService;

    public BookshelvesController(SessionsService sessionsService, BookshelvesService booksehlvesService)
    {
        _sessionsService = sessionsService;
        _bookshelvesService = booksehlvesService;
    }

    [HttpGet("{bookshelfId:length(24)}")]
    public async Task<ActionResult<Bookshelf>> Get(string bookshelfId)
    {
        var bookshelf = await _bookshelvesService.GetAsync(bookshelfId);

        if (bookshelf is null)
        {
            return NotFound();
        }

        var sessionId = bookshelf.SessionId;

        // fetch active session
        var activeSession = await _sessionsService.GetActiveAsync();

        if (activeSession is null)
        {
            return NotFound();
        }
        else if (activeSession.Id != sessionId)
        {
            return NotFound();
        }

        return bookshelf;
    }

    [HttpGet("{sessionId:length(24)}/{groupName}")]
    public async Task<ActionResult<List<Bookshelf>>> GetByGroup(string sessionId, string groupName) {
        var session = await _sessionsService.GetAsync(sessionId);

        // check if the session exists and the session is active or not
        if (session is null)
        {
            return NotFound();
        }
        else if (!session.IsActive)
        {
            return StatusCode(StatusCodes.Status403Forbidden, "The session is currently inactive");
        }
        else if (session.AllBookshelfGroups.Find(x => x.GroupName == groupName) is null)
        {
            return NotFound();
        }


        var bookshelves = await _bookshelvesService.GetGroupBookshelvesAsync(sessionId, groupName);
        return bookshelves;
    }

    [HttpPost]
    public async Task<IActionResult> Post(string sessionId, string groupName)
    {
        var session = await _sessionsService.GetAsync(sessionId);

        // check if the session exists and the session is active or not
        if (session is null)
        {
            return NotFound();
        }
        else if (!session.IsActive)
        {
            return StatusCode(StatusCodes.Status403Forbidden, "The session is currently inactive");
        } 
        
        var idx = session.AllBookshelfGroups.FindIndex(x => x.GroupName == groupName);
        // check if the group name exists or not
        if (idx == -1)
        {
            return NotFound();
        } 

        var shelfNumber = session.AllBookshelfGroups[idx].AllBookshelvesId.Count + 1;

        Bookshelf newBookshelf = new()
        {
            GroupName = groupName,
            ShelfNumber = shelfNumber,
            Description = $"{groupName}-{shelfNumber}",
            Status = 0,
            AllBooks = []
        };

        // create a bookshelf
        await _bookshelvesService.CreateAsync(newBookshelf);

        // update session
        session.AllBookshelfGroups[idx].AllBookshelvesId.Add(newBookshelf.Id!);

        // push changes to database
        await _sessionsService.UpdateAsync(sessionId, session);

        return CreatedAtAction(nameof(Get), new { sessionId = sessionId, bookshelfId = newBookshelf.Id }, newBookshelf);
    }

    [HttpPut("{sessionId:length(24)}/{bookshelfId:length(24)}")]
    public async Task<IActionResult> Update(string sessionId, string bookshelfId, string description)
    {
        var session = await _sessionsService.GetAsync(sessionId);

        // check if the session exists and the session is active or not
        if (session is null)
        {
            return NotFound();
        }
        else if (!session.IsActive)
        {
            return StatusCode(StatusCodes.Status403Forbidden, "The session is currently inactive");
        }

        // check if the bookshelf is in the session
        bool bookshelfInSession = false;
        foreach (var bookshelfGroup in session.AllBookshelfGroups)
        {
            if (bookshelfGroup.AllBookshelvesId.Find(x => x.Equals(bookshelfId)) is not null)
            {
                bookshelfInSession = true;
            }
        }

        // bookshelf id and session id not match (or one of them is wrong)
        if (!bookshelfInSession)
        {
            return NotFound();
        }

        var bookshelf = await _bookshelvesService.GetAsync(bookshelfId);

        // ensure the bookshelf is really there
        if (bookshelf is null)
        {
            return NotFound();
        }

        bookshelf.Description = description;

        await _bookshelvesService.UpdateAsync(bookshelfId, bookshelf);

        return Ok();
    }
}