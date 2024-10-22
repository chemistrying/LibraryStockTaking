using Serilog;
using LibrarySystemApi.Services;
using LibrarySystemApi.Models;

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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(new SessionsService("sessions"));
builder.Services.AddSingleton(new BookshelvesService("bookshelves"));
builder.Services.AddSingleton(new BooksService("books"));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();