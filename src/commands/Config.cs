public class Config
{
    public string DefaultBooklistLocation;
    public string DefaultSaveLocation;
    public bool ShowAllBooksPosition;
    public string[] configs = new string[3] {"DefaultBooklistLocation", "DefaultSaveLocation", "ShowAllBooksPosition"};
    // public Type[] configTypes = new Type[3] {typeof(string), typeof(string), typeof(bool)};

    public Config()
    {
        DefaultBooklistLocation = "booklist";
        DefaultSaveLocation = "foo";
        ShowAllBooksPosition = false;
    }
}