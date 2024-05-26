namespace LibrarySystemApi.Models;

public class Config
{
    public string DefaultBooklistLocation;
    public string DefaultSaveLocation;
    public string DefaultProgramFilesLocation;
    public string LoggingLevel;
    public string Version;
    public bool ShowAllBooksPosition;
    public bool Autocheck;
    public bool AutoCapitalize;
    public bool AutoZero;
    public bool DetailedBooklist;
    public int BookshelfTreeProfile;
    public string[] Options =
    [
        "DefaultBooklistLocation",
        "DefaultSaveLocation",
        "DefaultProgramFilesLocation",
        "LoggingLevel",
        "Version",
        "ShowAllBooksPosition",
        "Autocheck",
        "AutoCapitalize",
        "AutoZero",
        "DetailedBooklist",
        "BookshelfTreeProfile"
    ];
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
        AutoCapitalize = false;
        AutoZero = false;
        DetailedBooklist = false;
        BookshelfTreeProfile = 0;
    }
}