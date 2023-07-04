public class InputHandler
{
    public InputHandler()
    {
        
    }

    public string HandleInput(string Input, ulong Source, Discord.WebSocket.DiscordSocketClient _client, Discord.WebSocket.SocketUser User)
    {
        if (Input.StartsWith(';'))
        {
            // format changing
            Globals._format[Source].Change(Input.Length == 1 ? "" : Input.Substring(1));
            Serilog.Log.Debug($"Changed input format to {Input}.");
        }
        else if (Input.StartsWith(':'))
        {
            // Ignore format
            Globals._commands.ReadInput(Input.Substring(1), Source);
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
                    return Globals._commands.Del(0, Source);
                    break;
                // case "save":
                //     return Globals._commands.Save(args);
                //     break;
                case "undo":
                    return Globals._commands.Undo(Source);
                    break;
                case "help":
                    return Globals._commands.Help(args);
                    break;
                case "count":
                    return Globals._commands.Count(Source);
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
                case "start":
                    return Globals._commands.Start(args, _client);
                case "finish":
                    return Globals._commands.Finish(args, _client, User);
                default:
                    return "Invalid Command";
                    break;
            }
        }
        
        // TODO: Make formatting better, right now disable it first
        // return Globals._commands.ReadInput(Globals._format[Source].GetFormat(Input));
        return Globals._commands.ReadInput(Input, Source);
    }
}