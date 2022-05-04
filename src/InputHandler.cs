public class InputHandler
{
    public InputHandler()
    {
        
    }

    void HandleInput(String Input)
    {
        if (Input.StartsWith(';'))
        {
            // format changing
            Globals._format.Change(Input.Length == 1 ? "" : Input.Substring(1));
        }
        else if (Input.StartsWith('/'))
        {
            int pos = Input.IndexOf(' ');
            String cmd = Input.Substring(1, pos - 1).ToLower();
            switch (cmd)
            {
                case "del":
                    break;
                case "save":
                    break;
                case "undo":
                    break;
                case "help":
                    break;
                case "count":
                    break;
                case "check":
                    break;
                case "reload":
                    break;
                case "config":
                    break;
                default:
                    Console.WriteLine("Invalid command.");
                    break;
            }
        }
        else
        {
            Globals.Buffer.Add(Globals._format.GetFormat(Input));
        }
    }
}