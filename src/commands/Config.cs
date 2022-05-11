public class Config
{
    public string DefaultBooklistLocation;
    public string DefaultSaveLocation;
    public bool ShowAllBooksPosition;
    public bool Autocheck;
    public bool BlockInvalidInputs;
    public string[] configs = new string[5] {"DefaultBooklistLocation", "DefaultSaveLocation", "ShowAllBooksPosition", "Autocheck", "BlockInvalidInputs"};
    // public Type[] configTypes = new Type[5] {typeof(string), typeof(string), typeof(bool), typeof(bool), typeof(bool)};

    public Config()
    {
        DefaultBooklistLocation = "booklist";
        DefaultSaveLocation = "foo";
        ShowAllBooksPosition = false;
        Autocheck = false;
        BlockInvalidInputs = false;
    }
}