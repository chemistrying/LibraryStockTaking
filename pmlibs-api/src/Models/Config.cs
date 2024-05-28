namespace LibrarySystemApi.Models;

public class Config
{
    public string DefaultSaveLocation;
    public string DefaultProgramFilesLocation;
    public string LoggingLevel;
    public string Version;
    public bool AutoCapitalize;
    public bool AutoZero;
    public int BookshelfTreeProfile;
    public string[] Options =
    [
        "DefaultSaveLocation",
        "DefaultProgramFilesLocation",
        "LoggingLevel",
        "Version",
        "AutoCapitalize",
        "AutoZero",
        "BookshelfTreeProfile"
    ];
    // public Type[] configTypes = new Type[5] {typeof(string), typeof(string), typeof(bool), typeof(bool), typeof(bool)};

    public Config()
    {
        DefaultSaveLocation = "exports";
        DefaultProgramFilesLocation = "files";
        LoggingLevel = "Information";
        Version = "0.0.0";
        AutoCapitalize = false;
        AutoZero = false;
        BookshelfTreeProfile = 0;
    }
}