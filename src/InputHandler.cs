public class InputHandler
{
    public InputHandler()
    {
        
    }

    public void HandleInput(string Input)
    {
        if (Input.StartsWith(';'))
        {
            // format changing
            Globals._format.Change(Input.Length == 1 ? "" : Input.Substring(1));
        }
        else if (Input.StartsWith('/'))
        {
            int pos = Input.IndexOf(' ');
            string cmd = (pos == -1 ? Input.Substring(1) : Input.Substring(1, pos - 1)).ToLower();
            string args = pos == -1 ? "" : Input.Substring(pos + 1);
            switch (cmd)
            {
                case "del":
                    Globals._commands.Del(args == "" ? 0 : Convert.ToInt32(args));
                    break;
                case "save":
                    Globals._commands.Save(args);
                    break;
                case "undo":
                    Globals._commands.Undo();
                    break;
                case "help":
                    Globals._commands.Help(args);
                    break;
                case "count":
                    Globals._commands.Count(args);
                    break;
                case "check":
                    Globals._commands.Check(args);
                    break;
                case "reload":
                    Globals._commands.ReloadBooklist(args);
                    break;
                case "config":
                    break;
                case "quit":
                    Globals._commands.Quit();
                    break;
                default:
                    Console.WriteLine("Invalid command.");
                    break;
            }
        }
        else
        {
            Globals._buffer.Add(Globals._format.GetFormat(Input));
        }
    }
}