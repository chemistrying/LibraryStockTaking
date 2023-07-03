using System.Reflection;
using Discord.WebSocket;
using Discord.Commands;
using Discord;
using Serilog;

public class ChatReader
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;

    // Retrieve client and CommandService instance via ctor
    public ChatReader(DiscordSocketClient client, CommandService commands)
    {
        _commands = commands;
        _client = client;
    }
    
    public async Task InstallCommandsAsync()
    {
        // Hook the MessageReceived event into our command handler
        _client.MessageReceived += HandleCommandAsync;

        // Here we discover all of the command modules in the entry 
        // assembly and load them. Starting from Discord.NET 2.0, a
        // service provider is required to be passed into the
        // module registration method to inject the 
        // required dependencies.
        //
        // If you do not use Dependency Injection, pass null.
        // See Dependency Injection guide for more information.
        await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), 
                                        services: null);
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        // Don't process the command if it was a system message
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        // Create a number to track where the prefix ends and the command begins
        int argPos = 0;

        // Create a WebSocket-based command context based on the message
        var context = new SocketCommandContext(_client, message);

        // Determine if the message is a command based on the prefix and make sure no bots trigger commands
        if (message.Author.IsBot)
        {
            return;
        }
        else
        {
            Console.WriteLine(messageParam.Content);
            // await context.Channel.SendMessageAsync($"Received message \"{messageParam.Content}\".");
            await context.Channel.SendMessageAsync(Globals._commands.ReadInput(messageParam.Content));
        }
    }
}