
using Discord;
using Discord.WebSocket;
using Discord.Net;
using Discord.Commands;
using Newtonsoft.Json;
using Serilog;

public static class Globals
{
    public static string _currentFileLocation = "foo";
    public static bool _running = true;
    public static Format _format = new Format();
    public static Config _config = new Config();
    public static InputHandler _inputHandler = new InputHandler();
    public static Commands _commands = new Commands();
    public static Stack<Tuple<int, List<string>>> _normalUndoStack = new Stack<Tuple<int, List<string>>>();
    public static List<string> _buffer = new List<string>();
    public static List<string> _booklist = new List<string>();
    public static int[] _originalBooklistIndex = new int[_booklist.Count];
    public static Dictionary<string, Book> _detailBooklist = new Dictionary<string, Book>();
}

public class Program
{
    private DiscordSocketClient _client;
    private SlashCommandHandler _slashCommandHandler;
    private LoggingService _loggingService;
    private ChatReader _chatReader;
    private CommandService _commands;
    private ulong guildId = 995823884744020079;

	public static Task Main(string[] args) => new Program().MainAsync();

    public async Task Client_Ready()
    {
        // Let's build a guild command! We're going to need a guild so lets just put that in a variable.
        var guild = _client.GetGuild(guildId);

        // Next, lets create our slash command builder. This is like the embed builder but for slash commands.
        // var PingCommand = new SlashCommandBuilder().WithName("ping").WithDescription("Pong!");
        var HelpCommand = new SlashCommandBuilder().WithName("help").WithDescription("This is my help command!");
        var EchoCommand = new SlashCommandBuilder()
            .WithName("echo")
            .WithDescription("An echo chamber")
            .AddOption("message", ApplicationCommandOptionType.String, "The message you want to echo", isRequired: true);

        try
        {
            // Now that we have our builder, we can call the CreateApplicationCommandAsync method to make our slash command.
            List<ApplicationCommandProperties> applicationCommandProperties = new();
            await _client.BulkOverwriteGlobalApplicationCommandsAsync(applicationCommandProperties.ToArray());
            // await guild.CreateApplicationCommandAsync(PingCommand.Build());
            await guild.CreateApplicationCommandAsync(HelpCommand.Build());
            await guild.CreateApplicationCommandAsync(EchoCommand.Build());

            // With global commands we don't need the guild.
            // await _client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
            // Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
            // For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
        }
        catch (HttpException exception)
        {
            // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

            // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
            Console.WriteLine(json);
        }
    }

    public async Task MainAsync()
    {
        // Library part
        // Load current configuration
        Globals._commands.ReloadConfig();
        SerilogCreator _serilogCreator = new SerilogCreator();

        // Load default booklist
        Globals._commands.ReloadBooklist(Globals._config.DefaultBooklistLocation);
        
        Serilog.Log.Information("All the resources has been loaded successfully.");

        var _config = new DiscordSocketConfig
        {
            AlwaysDownloadUsers = false,
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };
        
        _client = new DiscordSocketClient(_config);

        _slashCommandHandler = new SlashCommandHandler(_client);

        _loggingService = new LoggingService(_client);

        _commands = new CommandService();

        _chatReader = new ChatReader(_client, _commands);
        await _chatReader.InstallCommandsAsync();

        // Delete Slash Commands
        List<ApplicationCommandProperties> applicationCommandProperties = new();

        _client.Ready += Client_Ready;
        
        //  You can assign your bot token to a string, and pass that in to connect.
        //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
        var token = File.ReadAllText("token.txt");

        // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
        // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
        // var token = File.ReadAllText("token.txt");
        // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

        Globals._commands.LoadSystemMessage();

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
}