public class Config
{
    public string DefaultBooklistLocation;
    public string DefaultSaveLocation;
    public bool ShowAllBooksPosition;
    public string[] configs = new string[3] {"DefaultBooklistLocation", "DefaultSaveLocation", "ShowAllBooksPosition"};

    public Config()
    {
        DefaultBooklistLocation = "booklist.txt";
        DefaultSaveLocation = "foo";
        ShowAllBooksPosition = false;
    }
}