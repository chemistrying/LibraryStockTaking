using Serilog;

using LibrarySystemApi.Models;
using LibrarySystemApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<SessionsDatabaseSettings>(
    builder.Configuration.GetSection("SessionsDatabase"));
builder.Services.Configure<BookshelvesDatabaseSettings>(
    builder.Configuration.GetSection("BookshelvesDatabase"));
builder.Services.Configure<BooksDatabaseSettings>(
    builder.Configuration.GetSection("BooksDatabase"));

builder.Services.AddSingleton<SessionsService>();
builder.Services.AddSingleton<BookshelvesService>();
builder.Services.AddSingleton<BooksService>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

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