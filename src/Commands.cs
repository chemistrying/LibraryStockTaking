public class Commands
{
    public Commands()
    {

    }

    public void LoadSystemMessage()
    {
        Console.WriteLine(File.ReadAllText(Globals._config.DefaultProgramFilesLocation + "systemMessage.txt"));
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
    }

    public void Del(int Args)
    {
        if (Args == 0)
        {
            // Remove last book
            if (Globals._buffer.Count == 0)
            {
                Console.Beep();
                Console.WriteLine("There is nothing for you to delete.");
            }
            else
            {
                Globals._normalUndoStack.Push(Tuple.Create(0, new List<string>() { Globals._buffer[Globals._buffer.Count - 1] }));
                // Console.WriteLine(Globals._buffer[Globals._buffer.Count - 1]);
                // Console.WriteLine(Globals._normalUndoStack.First().Item2[0]);
                Globals._buffer.RemoveAt(Globals._buffer.Count - 1);
                Console.WriteLine("Deleted previous input.");
            }
        }
        else if (Args > 0)
        {
            // Remove last x books
            Globals._normalUndoStack.Push(Tuple.Create(Args, Globals._buffer.GetRange(Math.Min(0, Globals._buffer.Count - 1 - Args), Math.Max(Globals._buffer.Count, Args))));
            Globals._buffer.RemoveRange(Math.Min(0, Globals._buffer.Count - 1 - Args), Math.Max(Globals._buffer.Count, Args));
            Console.WriteLine($"Deleted last {Math.Min(Globals._buffer.Count, Args)} inputs.");
        }
        else
        {
            // Remove a specific position book
            if (Globals._buffer.Count + Args < 0)
            {
                Console.Beep();
                Console.WriteLine("Index out of bound.");
            }
            else
            {
                Globals._normalUndoStack.Push(Tuple.Create(Args, new List<string>() { Globals._buffer[Globals._buffer.Count + Args] }));
                Globals._buffer.RemoveAt(Globals._buffer.Count + Args);
                Console.WriteLine($"Deleted the {Globals._buffer.Count + Args} input.");
            }
        }
    }

    public void Save(string Args)
    {
        if (Globals._buffer.Count == 0)
        {
            Console.WriteLine("There is nothing to save.");
            return;
        }
        string fileLocation = Args == "" ? Globals._currentFileLocation : Args.Substring(0, Args.IndexOf(' ') == -1 ? Args.Length : Args.IndexOf(' '));
        string furtherArgs = Args.IndexOf(' ') == -1 ? "" : Args.Substring(Args.IndexOf(' ') + 1);
        if (furtherArgs != "-y")
        {
            string Confirm = "";
            do
            {
                Console.Write($"Confirm saving into \"{fileLocation}.txt\" this file? [Y|N] ");
                Confirm = Console.ReadLine();
            } while (Confirm != "Y" && Confirm != "N");
            if (Confirm == "N")
            {
                Console.WriteLine("Save operation cancelled.");
                return;
            }
        }

        File.AppendAllText(fileLocation + ".txt", string.Join("\n", Globals._buffer) + "\n");
        Globals._buffer.Clear();
        Globals._normalUndoStack.Clear();
        Console.WriteLine($"Successfully saved to \"{fileLocation}.txt\".");
    }

    public void Undo()
    {
        if (Globals._normalUndoStack.Count == 0)
        {
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
    }

    public void Count(string Args)
    {
        string fileLocation = Args == "" ? Globals._currentFileLocation : Args.Substring(0, Args.IndexOf(' ') == -1 ? Args.Length : Args.IndexOf(' '));
        // string furtherArgs = Args.IndexOf(' ') == -1 ? "" : Args.Substring(Args.IndexOf(' ') + 1);

        using (StreamReader sr = new StreamReader(fileLocation + ".txt"))
        {
            int cnt = 0;
            while (!sr.EndOfStream)
            {
                sr.ReadLine();
                cnt++;
            }
            Console.WriteLine($"Number of books in \"{fileLocation}.txt\": {cnt}");
        }
    }

    public int Position(string Barcode)
    {
        int l = 0, r = Globals._booklist.Count - 1;
        while (l <= r)
        {
            int m = (l + r) >> 1;
            if (Globals._booklist[m] == Barcode) break;
            else if (String.Compare(Globals._booklist[m], Barcode) < 0) l = m + 1;
            else r = m - 1;
        }
        return l > r ? -1 : (l + r) >> 1;
    }

    public void Check(string Args)
    {
        string fileLocation = Args == "" ? Globals._currentFileLocation : Args.Substring(0, Args.IndexOf(' ') == -1 ? Args.Length : Args.IndexOf(' '));
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
            Console.WriteLine($"Successfully check all the books in \"{fileLocation}.txt\" in {Math.Round(EndTime / 1000.0 - StartTime / 1000.0, 3)} second(s).");
        }
        catch
        {
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
            // Console.WriteLine("OK");
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
            Console.WriteLine("Booklist has been successfully reloaded.");
        }
        catch
        {
            Console.WriteLine("File location is invalid.");
            Globals._booklist = Backup;
            return;
        }
    }

    public void ReloadConfig()
    {
        dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("config.json"));
        Globals._config = json.ToObject(typeof(Config));
    }

    public void Config(string Args)
    {
        int Pos = Args.IndexOf(' ');
        string ConfigName = Pos == -1 ? Args : Args.Substring(0, Pos);
        if (Args == "")
        {
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("config.json"));
            Dictionary<string, dynamic> NewConfig = json.ToObject(typeof(Dictionary<string, dynamic>));
            foreach (KeyValuePair<string, dynamic> Item in NewConfig)
            {
                Console.WriteLine($"{Item.Key}: {Item.Value}");
            }
        }
        else if (!Globals._config.configs.Contains(ConfigName))
        {
            Console.WriteLine("Configuration Name is wrong. Please check if there are any spelling mistakes.");
        }
        else
        {
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("config.json"));
            Dictionary<string, dynamic> NewConfig = json.ToObject(typeof(Dictionary<string, dynamic>));
            if (Pos == -1)
            {
                Console.WriteLine($"{ConfigName}: {NewConfig[ConfigName]}");
            }
            else
            {
                string Value = Args.Substring(Pos + 1);
                if (Array.IndexOf(Globals._config.configs, ConfigName) >= 3)
                {
                    try
                    {
                        NewConfig[ConfigName] = Convert.ToBoolean(Value);
                    }
                    catch
                    {
                        Console.WriteLine("The value is invalid.");
                        return;
                    }
                }
                else
                {
                    NewConfig[ConfigName] = Value;
                }
                Console.WriteLine($"Successfully changed {ConfigName} value to {Value}.");
            }
            File.WriteAllText("config.json", Newtonsoft.Json.JsonConvert.SerializeObject(NewConfig));
        }
        ReloadConfig();
    }

    public void Quit()
    {
        Globals._running = false;
    }
}