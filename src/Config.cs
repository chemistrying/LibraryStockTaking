public class Config
{
    public string DefaultBooklistLocation;
    public string DefaultSaveLocation;
    public string DefaultProgramFilesLocation;
    public string LoggingLevel;
    public string Version;
    public bool ShowAllBooksPosition;
    public bool Autocheck;
    public bool BlockInvalidInputs;
    public bool AutoCapitalize;
    public bool AutoZero;
    public bool DetailedBooklist;
    public string[] Options = new string[11] 
    {
        "DefaultBooklistLocation",
        "DefaultSaveLocation",
        "DefaultProgramFilesLocation",
        "LoggingLevel",
        "Version",
        "ShowAllBooksPosition",
        "Autocheck",
        "BlockInvalidInputs",
        "AutoCapitalize",
        "AutoZero",
        "DetailedBooklist"
    };
    // public Type[] configTypes = new Type[5] {typeof(string), typeof(string), typeof(bool), typeof(bool), typeof(bool)};

    public Config()
    {
        DefaultBooklistLocation = "booklist";
        DefaultSaveLocation = "foo";
        DefaultProgramFilesLocation = "files\\";
        LoggingLevel = "Information";
        Version = "0.0.0";
        ShowAllBooksPosition = false;
        Autocheck = false;
        BlockInvalidInputs = false;
        AutoCapitalize = false;
        AutoZero = false;
        DetailedBooklist = false;
    }
}