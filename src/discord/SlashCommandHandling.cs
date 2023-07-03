using Discord.Net;
using Discord.WebSocket;

public class SlashCommandHandler
{
    public SlashCommandHandler(DiscordSocketClient client)
    {
        client.SlashCommandExecuted += HandleSlashCommandAsync;
    }

    public async Task HandleSlashCommandAsync(SocketSlashCommand command)
    {
        string Result = Globals._inputHandler.HandleInput("/" + command.Data.Name);
        Console.WriteLine(Result);
        // Need to slice results
        await command.RespondAsync(Result);
        // await command.Channel.SendMessageAsync("Hi");
        // await command.RespondAsync($"You executed {command.Data.Name} with parameter {command.Data.Options.First().Value}!");
    }
}