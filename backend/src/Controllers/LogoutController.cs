using LibrarySystemApi.Models;
using LibrarySystemApi.Services;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace LibrarySystemApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LogoutController : ControllerBase
{
    private readonly AccountsService _accountsService;

    public LogoutController(AccountsService accountsService)
    {
        _accountsService = accountsService;
    }

    [HttpPost]
    public async Task<IActionResult> Post()
    {
        await HttpContext.SignOutAsync();

        return Ok();
    }
}