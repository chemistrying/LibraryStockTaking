using LibrarySystemApi.Models;
using LibrarySystemApi.Services;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace LibrarySystemApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly AccountsService _accountsService;
    private readonly TokensService _tokensService;

    public LoginController(AccountsService accountsService, TokensService tokensService)
    {
        _accountsService = accountsService;
        _tokensService = tokensService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return StatusCode(StatusCodes.Status403Forbidden, "You need higher privilege to access the API endpoints.");
    }

    [HttpPost]
    public async Task<IActionResult> Post(LoginModel loginModel)
    {
        string name = loginModel.name, password = loginModel.password;

        var account = await _accountsService.GetAsync(name);

        if (account is null)
        {
            return Unauthorized();
        }

        // Get salt
        byte[] hashedPasswordWithSalt = Convert.FromBase64String(account.PasswordHash);
        byte[] salt = new byte[16];
        Buffer.BlockCopy(hashedPasswordWithSalt, 0, salt, 0, salt.Length);

        bool verdict = await _accountsService.LoginAsync(name, PasswordHasher.HashPassword(password, salt));
        if (verdict) 
        {
            // await _tokensService.RemoveAllAsync(name);

            // Token token = new()
            // {
            //     Owner = account.Name,
            //     ExpiryDate = DateTime.Now.AddDays(1)
            // };

            // await _tokensService.CreateAsync(token);

            var claims = new List<Claim>
            {
                // new Claim(ClaimTypes.NameIdentifier, token.Id!),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, account.IsAdmin ? "Administrator" : "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            
            return Ok();
        }

        return Unauthorized();
    }
}