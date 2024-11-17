using LibrarySystemApi.Models;
using LibrarySystemApi.Services;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost]
    public async Task<ActionResult<string?>> Post(LoginModel loginModel)
    {
        string name = loginModel.name, password = loginModel.password;
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
        var account = await _accountsService.GetAsync(accountPayload.OldName);

        // check if the session exists and the session is active or not
        if (account is null)
        {
            return BadRequest("The account doesn't exist.");
        }

        byte[] salt = PasswordHasher.GetSaltFromHash(account.PasswordHash);

        if (await _accountsService.LoginAsync(accountPayload.OldName, PasswordHasher.HashPassword(accountPayload.OldPassword, salt)))
        {
            // ok
            account.Name = accountPayload.NewName ?? accountPayload.OldName;
            account.PasswordHash = accountPayload.NewPassword == null ? account.PasswordHash : PasswordHasher.HashPassword(accountPayload.NewPassword, PasswordHasher.GenerateSalt());
            
            await _accountsService.UpdateAsync(account.Name, account);

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