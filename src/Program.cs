
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
    public static Dictionary<ulong, Format> _format = new Dictionary<ulong, Format>();
    public static Config _config = new Config();
    public static InputHandler _inputHandler = new InputHandler();
    public static Commands _commands = new Commands();
    public static Dictionary<ulong, Stack<Tuple<int, List<string>>>> _normalUndoStack = new Dictionary<ulong, Stack<Tuple<int, List<string>>>>();
    public static Dictionary<ulong, List<string>> _buffer = new Dictionary<ulong, List<string>>();
    public static Dictionary<ulong, bool> _doubleChecked = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, string> _shelfName = new Dictionary<ulong, string>();
    public static List<string> _booklist = new List<string>();
    public static int[] _originalBooklistIndex = new int[_booklist.Count];
    public static Dictionary<string, Book> _detailBooklist = new Dictionary<string, Book>();

    public static ulong _currentShelvesGroupId = 1126023880927092766;
    public static ulong _archivedShelvesGroupId = 1126023908542386217;
    public static ulong _guildId = 1126023351069065248;
    public static ulong _generalChannelId = 1126023351811448884;
}

public class Program
{
    private DiscordSocketClient _client;
    private SlashCommandHandler _slashCommandHandler;
    private LoggingService _loggingService;
    private ChatReader _chatReader;
    private CommandService _commands;

	public static Task Main(string[] args) => new Program().MainAsync();

    public List<SlashCommandBuilder> RegisterCommand()
    {
        return new List<SlashCommandBuilder>
        {
            // Help command
            new SlashCommandBuilder()
                .WithName("help")
                .WithDescription("This is my help command!"),
            // Echo command
            new SlashCommandBuilder()
                .WithName("echo")
                .WithDescription("An echo chamber")
                .AddOption("message", ApplicationCommandOptionType.String, "The message you want to echo", isRequired: true),
            // Del command
            new SlashCommandBuilder()
                .WithName("del")
                .WithDescription("Delete previous entry"),
            // Undo command
            new SlashCommandBuilder()
                .WithName("undo")
                .WithDescription("Undo a delete operation"),
            // Count command
            new SlashCommandBuilder()
                .WithName("count")
                .WithDescription("Count the number of books entered in this channel"),
            // Version command
            new SlashCommandBuilder()
                .WithName("version")
                .WithDescription("Obtain the version of this software / bot"),
            // Search command
            new SlashCommandBuilder()
                .WithName("search")
                .WithDescription("Search a book")
                .AddOption("barcode", ApplicationCommandOptionType.String, "The barcode (ACNO) of the book", isRequired: true),
            // Start command
            new SlashCommandBuilder()
                .WithName("start")
                .WithDescription("Start a stock taking procedure")
                .AddOption("channel_name", ApplicationCommandOptionType.String, "The name of the bookshelf", isRequired: true),
            // Finish command
            new SlashCommandBuilder()
                .WithName("finish")
                .WithDescription("Finish a stock taking procedure"),
        };
    }

    public async Task Client_Ready()
    {
        // Let's build a guild command! We're going to need a guild so lets just put that in a variable.
        var guild = _client.GetGuild(Globals._guildId);

        // Next, lets create our slash command builder. This is like the embed builder but for slash commands.
        // var PingCommand = new SlashCommandBuilder().WithName("ping").WithDescription("Pong!");
        Globals._commands.Prerequisiting(_client);

        try
        {
            // Now that we have our builder, we can call the CreateApplicationCommandAsync method to make our slash command.
            List<ApplicationCommandProperties> applicationCommandProperties = new();
            await _client.BulkOverwriteGlobalApplicationCommandsAsync(applicationCommandProperties.ToArray());
            // await guild.CreateApplicationCommandAsync(PingCommand.Build());

            List<SlashCommandBuilder> CommandList = RegisterCommand();
            foreach (var CommandBuilder in CommandList)
            {
                await guild.CreateApplicationCommandAsync(CommandBuilder.Build());
            }

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
        // Get encoding
        // System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        // For debug usage
        // Console.OutputEncoding = System.Text.Encoding.UTF8;
        // Console.WriteLine(Console.OutputEncoding.ToString());


        // Library part
        // Load current configuration
        Globals._commands.ReloadConfig();
        SerilogCreator _serilogCreator = new SerilogCreator();

        // Load default booklist
        Globals._commands.ReloadBooklist(Globals._config.DefaultBooklistLocation);
        
        // TODO: Load books from current stocking taking files to buffer
        
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
        
        var token = File.ReadAllText("token.txt");

        Globals._commands.LoadSystemMessage();

        // var channel = _client.GetGuild(Globals._guildId).CreateTextChannelAsync("hello");
        // var channel = _client.GetChannel(1234) as ITextChannel;
        // await channel.ModifyAsync(x =>
        // {
        //     x.CategoryId = 1234;
        // });

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Globals._commands.Prerequisiting(_client);
        Serilog.Log.Information("All the data has been loaded successfully.");
        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
}