public class Commands
{
    public Commands()
    {

    }

    public void LoadSystemMessage()
    {
        Console.WriteLine(File.ReadAllText(Globals._config.DefaultProgramFilesLocation + "systemMessage.txt"));
        Serilog.Log.Information("System message is loaded successfully.");
    }

    public void ReadInput(string Barcode)
    {
        // Console.WriteLine($"DEBUG: {Barcode}");
        int Pos = Globals._config.Autocheck ? Globals._commands.Position(Barcode) : -2;
        if (Globals._config.Autocheck)
        {
            if (Pos == -1)
            {
                Console.WriteLine("The previous barcode is invalid according to the booklist. Please fix it immediately.");
            }
        }
        if (Pos != -1 || !Globals._config.BlockInvalidInputs)
        {
            Globals._buffer.Add(Barcode);
        }
        else
        {
            Console.WriteLine("The previous barcode has been automatically deleted because it is invalid according to the booklist.");
        }
        Serilog.Log.Debug($"Barcode {Barcode} has been processed successfully.");
    }

    public void Del(int Args)
    {
        if (Args == 0)
        {
            // Remove last book
            if (Globals._buffer.Count == 0)
            {
                Console.Beep();
                Serilog.Log.Warning("Nothing to delete. Going to cancel this delete operation.");
                Console.WriteLine("There is nothing for you to delete.");
            }
            else
            {
                Globals._normalUndoStack.Push(Tuple.Create(0, new List<string>() { Globals._buffer[Globals._buffer.Count - 1] }));
                // Console.WriteLine(Globals._buffer[Globals._buffer.Count - 1]);
                // Console.WriteLine(Globals._normalUndoStack.First().Item2[0]);
                Globals._buffer.RemoveAt(Globals._buffer.Count - 1);
                Serilog.Log.Information("Successfully delete previous input.");
                Console.WriteLine("Deleted previous input.");
            }
        }
        else if (Args > 0)
        {
            // Remove last x books
            Globals._normalUndoStack.Push(Tuple.Create(Args, Globals._buffer.GetRange(Math.Min(0, Globals._buffer.Count - 1 - Args), Math.Max(Globals._buffer.Count, Args))));
            Globals._buffer.RemoveRange(Math.Min(0, Globals._buffer.Count - 1 - Args), Math.Max(Globals._buffer.Count, Args));
            Serilog.Log.Information($"Successfully delete last {Math.Min(Globals._buffer.Count, Args)} inputs.");
            Console.WriteLine($"Deleted last {Math.Min(Globals._buffer.Count, Args)} inputs.");
        }
        else
        {
            // Remove a specific position book
            if (Globals._buffer.Count + Args < 0)
            {
                Console.Beep();
                Serilog.Log.Warning($"Index is out of bound. Going to cancel this delete operation.");
                Console.WriteLine("Index out of bound.");
            }
            else
            {
                Globals._normalUndoStack.Push(Tuple.Create(Args, new List<string>() { Globals._buffer[Globals._buffer.Count + Args] }));
                Globals._buffer.RemoveAt(Globals._buffer.Count + Args);
                Serilog.Log.Information($"Successfully delete the {Globals._buffer.Count + Args} input");
                Console.WriteLine($"Deleted the {Globals._buffer.Count + Args} input.");
            }
        }
    }

    public void Save(string Args)
    {
        Serilog.Log.Debug("Trying to save files.");
        if (Globals._buffer.Count == 0)
        {
            Serilog.Log.Warning("Nothing to save. Going to cancel this save operation.");
            Console.WriteLine("There is nothing to save.");
            return;
        }
        string fileLocation = Args == "" ? Globals._currentFileLocation : Args.Substring(0, Args.IndexOf(' ') == -1 ? Args.Length : Args.IndexOf(' '));
        string furtherArgs = Args.IndexOf(' ') == -1 ? "" : Args.Substring(Args.IndexOf(' ') + 1);
        if (furtherArgs != "-y")
        {
            string Confirm = "";
            Serilog.Log.Debug("Asking for save confirmation.");
            do
            {
                Console.Write($"Confirm saving into \"{fileLocation}.txt\" this file? [Y|N] ");
                Confirm = Console.ReadLine();
            } while (Confirm != "Y" && Confirm != "N");
            if (Confirm == "N")
            {
                Serilog.Log.Information("Save operation has been cancelled safely.");
                Console.WriteLine("Save operation cancelled.");
                return;
            }
        }

        File.AppendAllText(fileLocation + ".txt", string.Join("\n", Globals._buffer) + "\n");
        Globals._buffer.Clear();
        Globals._normalUndoStack.Clear();
        Serilog.Log.Information($"Successfully save inputs to \"{fileLocation}.txt\".");
        Serilog.Log.Debug($"Inputs saved: {string.Join(", ", Globals._buffer)}");
        Console.WriteLine($"Successfully saved to \"{fileLocation}.txt\".");
    }

    public void Undo()
    {
        if (Globals._normalUndoStack.Count == 0)
        {
            Serilog.Log.Information("Nothing to undo. Going to cancel this undo operation.");
            Console.WriteLine("There is nothing to undo.");
        }
        else
        {
            Tuple<int, List<string>> Operation = Globals._normalUndoStack.First();
            if (Operation.Item1 == 0)
            {
                Globals._buffer.Add(Operation.Item2[0]);
                // Console.WriteLine(string.Join("\n", Globals._buffer));
            }
            else if (Operation.Item1 > 0)
            {
                foreach (string Item in Operation.Item2)
                {
                    Globals._buffer.Add(Item);
                }
            }
            else
            {
                Globals._buffer.Insert(Globals._buffer.Count + 1 + Operation.Item1, Operation.Item2[0]);
            }
            Globals._normalUndoStack.Pop();
            Serilog.Log.Information("Successfully undid.");
            Console.WriteLine("Undid last delete operation.");
        }

    }

    public void Help(string Args)
    {
        bool Basic = Args.ToLower() == "basic";
        bool Advanced = Args.ToLower() == "advanced";
        Console.WriteLine(File.ReadAllText(Globals._config.DefaultProgramFilesLocation + "help.txt"));
        if (Basic || !(Basic | Advanced))
        {
            Console.WriteLine(File.ReadAllText(Globals._config.DefaultProgramFilesLocation + "basic.txt"));
        }
        if (Advanced || !(Basic | Advanced))
        {
            Console.WriteLine(File.ReadAllText(Globals._config.DefaultProgramFilesLocation + "advanced.txt"));
        }
        Serilog.Log.Information("Help messages have been sent successfully.");
    }

    public void Count(string Args)
    {
        string fileLocation = Args == "" ? Globals._currentFileLocation : Args.Substring(0, Args.IndexOf(' ') == -1 ? Args.Length : Args.IndexOf(' '));
        // string furtherArgs = Args.IndexOf(' ') == -1 ? "" : Args.Substring(Args.IndexOf(' ') + 1);

        try{
            using (StreamReader sr = new StreamReader(fileLocation + ".txt"))
            {
                int cnt = 0;
                while (!sr.EndOfStream)
                {
                    sr.ReadLine();
                    cnt++;
                }
                Serilog.Log.Information($"Successfully count the number of books in \"{fileLocation}.txt\".");
                Serilog.Log.Debug($"There are {cnt} of books in \"{fileLocation}.txt\".");
                Console.WriteLine($"Number of books in \"{fileLocation}.txt\": {cnt}");
            }
        }
        catch
        {
            Serilog.Log.Warning("Failed to count the books because the file location is invalid.");
            Console.WriteLine("File location is not valid.");
        }
    }

    public int Position(string Barcode)
    {
        Serilog.Log.Debug($"Binary searching {Barcode} in the booklist.");
        int l = 0, r = Globals._booklist.Count - 1;
        while (l <= r)
        {
            int m = (l + r) >> 1;
            if (Globals._booklist[m] == Barcode) break;
            else if (String.Compare(Globals._booklist[m], Barcode) < 0) l = m + 1;
            else r = m - 1;
        }
        Serilog.Log.Debug($"Binary searched {Barcode} in position {(l > r ? -1 : (l + r) >> 1)}");
        return l > r ? -1 : (l + r) >> 1;
    }

    public void Check(string Args)
    {
        string fileLocation = Args == "" ? Globals._currentFileLocation : Args.Substring(0, Args.IndexOf(' ') == -1 ? Args.Length : Args.IndexOf(' '));
        Serilog.Log.Debug($"Checking {fileLocation}.txt barcodes.");
        try
        {
            long StartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            using (StreamReader sr = new StreamReader(fileLocation + ".txt"))
            {
                while (!sr.EndOfStream)
                {
                    string Barcode = sr.ReadLine();
                    int Pos = Position(Barcode);
                    if (Pos == -1)
                    {
                        Console.WriteLine($"WARNING: BOOK CODE {Barcode} NOT FOUND IN THE BOOKLIST.");
                    }
                    else if (Globals._config.ShowAllBooksPosition)
                    {
                        Console.WriteLine($"Found Book {Barcode} in booklist position of {Globals._originalBooklistIndex[Pos]}.");
                    }
                }
            }
            long EndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Serilog.Log.Information($"Successfully check all books in \"{fileLocation}\".txt.");
            Serilog.Log.Debug($"Checking time in \"{fileLocation}\".txt: {Math.Round(EndTime / 1000.0 - StartTime / 1000.0, 3)} second(s).");
            Console.WriteLine($"Successfully check all the books in \"{fileLocation}.txt\" in {Math.Round(EndTime / 1000.0 - StartTime / 1000.0, 3)} second(s).");
        }
        catch
        {
            Serilog.Log.Warning("Can't check the books because the file location is not valid.");
            Console.WriteLine("File location is not valid.");
        }
    }

    public void ReloadBooklist(string Args)
    {
        // backup the booklist
        List<string> Backup = Globals._booklist;
        // clear the booklist
        Globals._booklist.Clear();
        string fileLocation = Args == "" ? Globals._config.DefaultBooklistLocation : Args;
        // Console.WriteLine(Args);
        try
        {
            using (StreamReader sr = new StreamReader(fileLocation + ".txt"))
            {
                while (!sr.EndOfStream)
                {
                    string Book = sr.ReadLine();
                    Globals._booklist.Add(Book.Substring(0, Book.IndexOf('|')));
                }
            }
            Globals._booklist.Sort();

            Globals._originalBooklistIndex = new int[Globals._booklist.Count];
            using (StreamReader sr = new StreamReader(fileLocation + ".txt"))
            {
                int index = 1;
                while (!sr.EndOfStream)
                {
                    string Book = sr.ReadLine();
                    string Barcode = Book.Substring(0, Book.IndexOf('|'));
                    int Pos = Position(Barcode);
                    Globals._originalBooklistIndex[Pos] = index++;
                }
            }
            Serilog.Log.Information("Booklist loaded successfully.");
            Console.WriteLine("Booklist has been successfully reloaded.");
        }
        catch
        {
            Serilog.Log.Warning("Reload unsuccessful becaue the file location is invalid.");
            Console.WriteLine("File location is invalid.");
            Globals._booklist = Backup;
            return;
        }
    }

    public void ReloadConfig()
    {
        try{
            Serilog.Log.Debug("Reloading config.");
        }catch{
            ;
        }

        dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText(Globals._config.DefaultProgramFilesLocation + "config.json"));
        Globals._config = json.ToObject(typeof(Config));

        try{
            Serilog.Log.Debug("Config reloaded successfully.");
        }catch{
            ;
        }
    }

    public void Config(string Args)
    {
        int Pos = Args.IndexOf(' ');
        string ConfigName = Pos == -1 ? Args : Args.Substring(0, Pos);
        if (Args == "")
        {
            Serilog.Log.Debug("Listing out all the config options and values.");
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText(Globals._config.DefaultProgramFilesLocation + "config.json"));
            Dictionary<string, dynamic> NewConfig = json.ToObject(typeof(Dictionary<string, dynamic>));
            foreach (KeyValuePair<string, dynamic> Item in NewConfig)
            {
                Console.WriteLine($"{Item.Key}: {Item.Value}");
            }
            Serilog.Log.Information("Config options and values has been listed successfully.");
        }
        else if (!Globals._config.configs.Contains(ConfigName))
        {
            Serilog.Log.Warning($"Configuration name {ConfigName} is invalid.");
            Console.WriteLine("Configuration Name is wrong. Please check if there are any spelling mistakes.");
        }
        else
        {
            Serilog.Log.Debug($"Editing config {ConfigName}.");
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText(Globals._config.DefaultProgramFilesLocation + "config.json"));
            Dictionary<string, dynamic> NewConfig = json.ToObject(typeof(Dictionary<string, dynamic>));
            if (Pos == -1)
            {
                Console.WriteLine($"{ConfigName}: {NewConfig[ConfigName]}");
            }
            else
            {
                string Value = Args.Substring(Pos + 1);
                if (Array.IndexOf(Globals._config.configs, ConfigName) >= 4)
                {
                    try
                    {
                        NewConfig[ConfigName] = Convert.ToBoolean(Value);
                    }
                    catch
                    {
                        Serilog.Log.Warning($"Value {Value} for {ConfigName} is invalid.");
                        Console.WriteLine("The value is invalid.");
                        return;
                    }
                }
                else
                {
                    Serilog.Log.Information($"Old value {NewConfig[ConfigName]} for {ConfigName} is changing to new value {Value}.");
                    NewConfig[ConfigName] = Value;
                }
                Console.WriteLine($"Successfully changed {ConfigName} value to {Value}.");
            }
            Serilog.Log.Debug($"Updating to new config.");
            File.WriteAllText(Globals._config.DefaultProgramFilesLocation + "config.json", Newtonsoft.Json.JsonConvert.SerializeObject(NewConfig));
        }
        ReloadConfig();
    }

    public void Quit()
    {
        Serilog.Log.Information("Program is now quitting.");
        Globals._running = false;
    }
}