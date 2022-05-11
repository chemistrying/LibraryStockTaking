public class Format
{
    string FrontFormat;
    string BackFormat;
    public Format()
    {
        FrontFormat = "";
        BackFormat = "";
    }
    public void Change(string Args)
    {
        if (Args.Contains('*'))
        {
            int pos = Args.IndexOf('*');
            FrontFormat = Args.Substring(0, pos);
            BackFormat = Args.Substring(pos + 1);
        }
        else
        {
            FrontFormat = Args;
            BackFormat = "";
        }
    }
    public string GetFormat(string Input)
    {
        return FrontFormat + Input + BackFormat;
    }
}