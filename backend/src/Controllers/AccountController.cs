using LibrarySystemApi.Models;
using LibrarySystemApi.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LibrarySystemApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly AccountsService _accountsService;
    private readonly TokensService _tokensService;

    public AccountController(AccountsService accountsService, TokensService tokensService)
    {
        _accountsService = accountsService;
        _tokensService = tokensService;
    }

    [HttpGet]
    public async Task<ActionResult<string?>> Get()
    {
        return HttpContext.User.FindFirstValue(ClaimTypes.Name);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<string?>> Post(LoginModel loginModel)
    {
        string name = loginModel.username, password = loginModel.password;
        var account = await _accountsService.GetAsync(name);

        // check if the session exists and the session is active or not
        if (account is not null)
        {
            return BadRequest("The username has already been used.");
        } 
        else if (name == "" || password == "")
        {
            return BadRequest("Username or password cannot be empty strings.");
        }

        Account newAccount = new()
        {
            Name = name,
            PasswordHash = PasswordHasher.HashPassword(password, PasswordHasher.GenerateSalt()),
            IsAdmin = false,
            CreationTime = DateTime.Now,
        };

        await _accountsService.CreateAsync(newAccount);

        return name;
    }

    [HttpPut]
    public async Task<ActionResult<string?>> Put(AccountPayload accountPayload)
    {
        var account = await _accountsService.GetAsync(accountPayload.OldUsername);

        // check if the session exists and the session is active or not
        if (account is null)
        {
            return BadRequest("The account doesn't exist.");
        }

        // check duplication
        if (accountPayload.NewUsername is not null)
        {
            var testAccount = await _accountsService.GetAsync(accountPayload.NewUsername);
            if (testAccount is not null)
            {
                return BadRequest("The new username already exists.");
            }
        }

        byte[] salt = PasswordHasher.GetSaltFromHash(account.PasswordHash);

        if (await _accountsService.LoginAsync(accountPayload.OldUsername, PasswordHasher.HashPassword(accountPayload.OldPassword, salt)))
        {
            // ok
            account.Name = accountPayload.NewUsername ?? accountPayload.OldUsername;
            account.PasswordHash = accountPayload.NewPassword == null ? account.PasswordHash : PasswordHasher.HashPassword(accountPayload.NewPassword, PasswordHasher.GenerateSalt());

            await _accountsService.UpdateAsync(account.Id!, account);

            return Ok();
        }

        return Unauthorized();
    }

    [HttpDelete]
    public async Task<ActionResult<string?>> Delete(string name, string password)
    {
        var account = await _accountsService.GetAsync(name);

        // check if the account exists
        if (account is null)
        {
            return BadRequest("The account doesn't exist.");
        }

        byte[] salt = PasswordHasher.GetSaltFromHash(account.PasswordHash);

        await _accountsService.RemoveAsync(name, PasswordHasher.HashPassword(password, salt));

        return name;
    }
}