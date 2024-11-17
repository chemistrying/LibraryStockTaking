using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Newtonsoft.Json;

namespace LibrarySystemApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class AdminController : ControllerBase
{

    public AdminController()
    {
        
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok("You're an administrator.");
    }
}