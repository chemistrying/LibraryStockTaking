public class Config
{
    public string DefaultBooklistLocation;
    public string DefaultSaveLocation;
    public string DefaultProgramFilesLocation;
    public bool ShowAllBooksPosition;
    public bool Autocheck;
    public bool BlockInvalidInputs;
    public string[] configs = new string[6] {"DefaultBooklistLocation", "DefaultSaveLocation", "DefaultProgramFilesLocation", "ShowAllBooksPosition", "Autocheck", "BlockInvalidInputs"};
    // public Type[] configTypes = new Type[5] {typeof(string), typeof(string), typeof(bool), typeof(bool), typeof(bool)};

    public Config()
    {
        DefaultBooklistLocation = "files\\booklist";
        DefaultSaveLocation = "foo";
        DefaultProgramFilesLocation = "files\\";
        ShowAllBooksPosition = false;
        Autocheck = false;
        BlockInvalidInputs = false;
    }
}