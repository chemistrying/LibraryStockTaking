public static class Globals
{
    public static string _currentFileLocation = "foo";
    public static bool _running = true;
    public static Format _format = new Format();
    public static Config _config = new Config();
    public static InputHandler _inputHandler = new InputHandler();
    public static Commands _commands = new Commands();
    public static List<string> _buffer = new List<string>();
    public static List<string> _booklist = new List<string>();
}

public class Program
{
    static void Main(string[] args)
    {
        Globals._commands.ReloadConfig();
        do
        {
            string input = Console.ReadLine();
            Globals._inputHandler.HandleInput(input);
        } while (Globals._running);
    }
}