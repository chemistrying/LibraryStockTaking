using System.Text;

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

    public string Zeroify(string Barcode)
    {
        bool ok = true;
        // check if the Barcode is all numbers
        for (int i = 0; i < Barcode.Length; i++)
        {
            ok &= (Barcode[i] >= '0' && Barcode[i] <= '9');
        }

        if (ok)
        {
            // Console.WriteLine(Barcode);
            string Front = new String('0', Math.Max(0, 5 - Barcode.Length));
            Barcode = Front + Barcode;
        }

        return Barcode;
    }

    public string AutoFormat(string Barcode)
    {
        if (Barcode.Length == 0)
        {
            return Barcode;
        }

        if (Globals._config.AutoCapitalize) Barcode = Barcode.ToUpper();
        if (Globals._config.AutoZero)
        {
            // Console.WriteLine("OK");
            if (Barcode.Length < 5)
            {
                Barcode = Zeroify(Barcode);
            }
            else if (Barcode.First() == 'C' && Barcode.Length < 6)
            {
                // Console.WriteLine("SAD");
                // Console.WriteLine(Barcode);
                Barcode = 'C' + Zeroify(Barcode.Substring(1));
                // File.AppendAllText("files\\zeros.txt", Barcode + '\n');
            }
        }

        return Barcode;
    }

    public void ReadInput(string Barcode)
    {
        // Console.WriteLine($"DEBUG: {Barcode}");
        Barcode = AutoFormat(Barcode);

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

            // Only output book information when detailed booklist is used
            if (Globals._config.DetailedBooklist)
            {
                try
                {
                    Book CurrBook = Globals._detailBooklist[Barcode];
                    Console.WriteLine($"{CurrBook.Callno1} {CurrBook.Callno2} {CurrBook.Name}");
                }
                catch
                {
                    Serilog.Log.Warning("No detailed booklist is stored in the system! Can't print book info to users!");
                    Console.WriteLine("DetailedBooklist is turned on but no detailed booklist is stored in the system. You may want to run /process {fileLocation} first to use detailed booklist or turn off detailed booklist by running command /config DetailedBooklist false.");
                }
            }
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
        Globals._currentFileLocation = fileLocation;

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
        string fileLocation = (Args == "" ? Globals._currentFileLocation : Args.Substring(0, Args.IndexOf(' ') == -1 ? Args.Length : Args.IndexOf(' ')));
        // string furtherArgs = Args.IndexOf(' ') == -1 ? "" : Args.Substring(Args.IndexOf(' ') + 1);

        try
        {
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
        string fileLocation = Globals._config.DefaultProgramFilesLocation + (Args == "" ? Globals._config.DefaultBooklistLocation : Args);
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

        // Only auto processing the new booklist if auto processing is on
        if (Globals._config.DetailedBooklist)
        {
            Globals._commands.Process(fileLocation);
        }
    }

    public void ReloadConfig()
    {
        try
        {
            Serilog.Log.Debug("Reloading config.");
        }
        catch
        {
            ;
        }

        dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText(Globals._config.DefaultProgramFilesLocation + "config.json"));
        Globals._config = json.ToObject(typeof(Config));

        try
        {
            Serilog.Log.Debug("Config reloaded successfully.");
        }
        catch
        {
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
        else if (!Globals._config.Options.Contains(ConfigName))
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
                if (Array.IndexOf(Globals._config.Options, ConfigName) >= 4)
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

    public void Version()
    {
        Config("Version");
    }

    /*
        Process a booklist.
        First convert to UTF-8, then split the information of each book, and merge into a booklist.
    */

    public string Utfy(string Stuff)
    {
        Encoding big5 = Encoding.GetEncoding("big5");
        Encoding utf16 = Encoding.Unicode;

        byte[] Big5Bytes = big5.GetBytes(Stuff);
        byte[] Utf8Bytes = Encoding.Convert(big5, utf16, Big5Bytes);

        char[] Utf8Chars = new char[utf16.GetCharCount(Utf8Bytes, 0, Utf8Bytes.Length)];
        utf16.GetChars(Utf8Bytes, 0, Utf8Bytes.Length, Utf8Chars, 0);
        return new string(Utf8Chars);
    }

    public void Process(string Args)
    {
        // int Pos = Args.IndexOf(' ');
        if (Args.Length == 0)
        {
            Serilog.Log.Warning("Invalid arguments for processing the booklist.");
            Console.WriteLine("You must provide the input booklist and the output location in order to work.");
            return;
        }

        string Input = Args + ".txt";

        using (StreamReader sr = new StreamReader(Input))
        {
            sr.ReadLine();
            while (!sr.EndOfStream)
            {
                // Console.WriteLine(sr.ReadLine());
                // Book CurrBook = new Book(sr);
                string[] Blocks = sr.ReadLine().Split("| ");
                Book CurrBook = new Book(Blocks);
                Globals._detailBooklist.Add(CurrBook.Acno, CurrBook);
            }
        }
    }

    /*
        Check every single textfile in a folder, and merge the results together
        It returns the non-existence books.
    */

    public void Exist(string Args)
    {
        long StartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        string Location = Globals._config.DefaultProgramFilesLocation + Args;

        // Test the validity of the folder
        try
        {
            string[] Test = Directory.GetFiles(Location);
        }
        catch
        {
            Serilog.Log.Warning($"The directory {Location} is invalid.");
            Console.WriteLine("The directory is invalid.");
            return;
        }

        string[] FileNames = Directory.GetFiles(Location);
        if (FileNames.Length == 0)
        {
            Serilog.Log.Warning($"There are no files in the directory {Location}.");
            Console.WriteLine("There are no files in the directory.");
        }

        int TotalBarcodes = 0; // Include invalid, duplicated
        HashSet<string> Barcodes = new HashSet<string>();
        bool[] Existence = new bool[Globals._booklist.Count];
        int InvalidCnt = 0; // Include duplicated invalid barcodes

        HashSet<KeyValuePair<int, string>> Invalids = new HashSet<KeyValuePair<int, string>>();

        foreach (string FileName in FileNames)
        {
            if (FileName.Length < 4)
            {
                continue;
            }
            else if (FileName.Substring(FileName.Length - 4) != ".txt")
            {
                continue;
            }

            // Console.WriteLine(FileName); 

            using (StreamReader sr = new StreamReader(FileName))
            {
                while (!sr.EndOfStream)
                {
                    string Temp = AutoFormat(sr.ReadLine());
                    // Console.WriteLine(Temp);

                    int Pos = Position(Temp);
                    if (Pos != -1)
                    {
                        Barcodes.Add(Temp);
                        Existence[Pos] = true;
                    }
                    else
                    {
                        Invalids.Add(new KeyValuePair<int, string>(TotalBarcodes + 1, Temp));
                        InvalidCnt++;
                    }
                    TotalBarcodes++;
                }
            }
        }

        Console.WriteLine($"Merged a total of {Barcodes.Count} barcode(s).");
        Console.WriteLine($"Found {TotalBarcodes - InvalidCnt - Barcodes.Count} duplicated barcode(s).");
        Console.WriteLine($"Found {InvalidCnt} invalid barcodes(s).");
        Console.WriteLine($"{Globals._booklist.Count - Barcodes.Count} book(s) are missing.");

        using (StreamWriter sw = new StreamWriter(Globals._config.DefaultProgramFilesLocation + "report.txt"))
        {
            for (int i = 0; i < Globals._booklist.Count; i++)
            {
                if (!Existence[i])
                {
                    sw.WriteLine(Globals._booklist[i]);
                }
            }
        }


        HashSet<string> CheckDuplicated = new HashSet<string>();
        foreach (string FileName in FileNames)
        {
            if (FileName.Length < 4)
            {
                continue;
            }
            else if (FileName.Substring(FileName.Length - 4) != ".txt")
            {
                continue;
            }

            // Console.WriteLine(FileName);

            int Dup = 0;
            using (StreamReader sr = new StreamReader(FileName))
            {
                using (StreamWriter sw = new StreamWriter(Globals._config.DefaultProgramFilesLocation + "StockTakingData_02082022.txt"))
                {
                    while (!sr.EndOfStream)
                    {
                        string Temp = AutoFormat(sr.ReadLine());
                        if (!CheckDuplicated.Contains(Temp))
                        {
                            sw.WriteLine(Temp);
                            CheckDuplicated.Add(Temp);
                        }
                        else if (Position(Temp) != -1)
                        {
                            Dup++;
                        }
                    }
                }
            }
            Console.WriteLine($"Check Dup: {Dup}");
        }

        using (StreamWriter sw = new StreamWriter(Globals._config.DefaultProgramFilesLocation + "invalidReport.txt"))
        {
            foreach (var Invalid in Invalids)
            {
                sw.WriteLine($"{Invalid.Key} {Invalid.Value}");
            }
        }

        long EndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Console.WriteLine($"Used {Math.Round(EndTime / 1000.0 - StartTime / 1000.0, 3)} second(s) to check existence.");
    }

    /* 
        Search a book using its barcode and return all the related information
    */
    public void Search(string Args)
    {
        // Check the validity of the barcode
        int Pos = Position(Args);
        if (Pos == -1)
        {
            Serilog.Log.Warning($"{Args} can't be found in the booklist.");
            Console.WriteLine("The book can't be found in the booklist. Please check if you have typed the barcode correcly.");
        }
        else
        {
            Book CurrentBook = Globals._detailBooklist[Args];
            Console.WriteLine($"{CurrentBook.Acno} | {CurrentBook.Callno1} | {CurrentBook.Callno2} | {CurrentBook.Name} | {CurrentBook.Status} | {CurrentBook.Publisher} | {CurrentBook.Author} | {CurrentBook.Language} | {CurrentBook.Category}");
        }
    }
}