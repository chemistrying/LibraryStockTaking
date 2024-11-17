using Serilog;
using System.Security.Claims;
using LibrarySystemApi.Services;
using LibrarySystemApi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

LoggerConfiguration loggerConfiguration = new LoggerConfiguration().WriteTo.Console();

switch (Globals.Config.LoggingLevel)
{
    case "Verbose":
        loggerConfiguration.MinimumLevel.Verbose();
        break;
    case "Debug":
        loggerConfiguration.MinimumLevel.Debug();
        break;
    default:
        loggerConfiguration.MinimumLevel.Information();
        break;
}

Log.Logger = loggerConfiguration.CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(new SessionsService("sessions"));
builder.Services.AddSingleton(new BookshelvesService("bookshelves"));
builder.Services.AddSingleton(new BooksService("books"));
builder.Services.AddSingleton(new AccountsService("accounts"));
builder.Services.AddSingleton(new TokensService("tokens"));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddAuthentication(options => 
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/api/Login";
    options.LogoutPath = "api/Logout";
    options.AccessDeniedPath = "/api/Login";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, ["Administrator"]));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();