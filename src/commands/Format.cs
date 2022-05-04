public class Format
{
    String FrontFormat;
    String BackFormat;
    public Format()
    {
        FrontFormat = "";
        BackFormat = "";
    }
    public void Change(String Args)
    {
        if (Args.Contains('*'))
        {
            int pos = Args.IndexOf('*');
            FrontFormat = Args.Substring(0, pos - 1);
            BackFormat = Args.Substring(pos + 1);
        }
        else
        {
            FrontFormat = Args;
            BackFormat = "";
        }
    }
    public String GetFormat(String Input)
    {
        return FrontFormat + Input + BackFormat;
    }
}