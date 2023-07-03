public class InputHandler
{
    public InputHandler()
    {
        
    }

    public string HandleInput(string Input)
    {
        if (Input.StartsWith(';'))
        {
            // format changing
            Globals._format.Change(Input.Length == 1 ? "" : Input.Substring(1));
            Serilog.Log.Debug($"Changed input format to {Input}.");
        }
        else if (Input.StartsWith(':'))
        {
            // Ignore format
            Globals._commands.ReadInput(Input.Substring(1));
        }
        else if (Input.StartsWith('/'))
        {
            int pos = Input.IndexOf(' ');
            string cmd = (pos == -1 ? Input.Substring(1) : Input.Substring(1, pos - 1)).ToLower();
            string args = pos == -1 ? "" : Input.Substring(pos + 1);
            switch (cmd)
            {
                case "del":
                    // return Globals._commands.Del(args == "" ? 0 : Convert.ToInt32(args));
                    // make it less complicated
                    return Globals._commands.Del(0);
                    break;
                case "save":
                    return Globals._commands.Save(args);
                    break;
                case "undo":
                    return Globals._commands.Undo();
                    break;
                case "help":
                    return Globals._commands.Help(args);
                    break;
                case "count":
                    return Globals._commands.Count(args);
                    break;
                // case "check":
                //     return Globals._commands.Check(args);
                //     break;
                case "reload":
                    return Globals._commands.ReloadBooklist(args);
                    break;
                case "config":
                    return Globals._commands.Config(args);
                    break;
                // case "quit":
                //     return Globals._commands.Quit();
                //     break;
                case "version":
                    return Globals._commands.Version();
                    break;
                // case "exist":
                //     return Globals._commands.Exist(args);
                //     break;
                case "search":
                    return Globals._commands.Search(args);
                    break;
                default:
                    return "Invalid Command";
                    break;
            }
        }
        
        return Globals._commands.ReadInput(Globals._format.GetFormat(Input));
    }
}