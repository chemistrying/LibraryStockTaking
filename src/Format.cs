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
        if (Args.Contains('+') && Args.Contains('*'))
        {
            int pos = Args.IndexOf('+');
            int pos2 = Args.IndexOf('*');
            // check if there is front formatting addition
            if (pos2 > pos){
                FrontFormat = Args.Substring(0, pos) + FrontFormat + Args.Substring(pos + 1, pos2 - pos - 1);
                pos = Args.IndexOf('+', pos + 1);
            }
            // check if there is back formatting addition
            if (pos != -1)
            {
                BackFormat = Args.Substring(pos2 + 1, pos - pos2 - 1) + BackFormat + Args.Substring(pos + 1);
            }
        }
        else if (Args.Contains('*'))
        {
            int pos = Args.IndexOf('*');
            FrontFormat = Args.Substring(0, pos);
            BackFormat = Args.Substring(pos + 1);
        }
        else if (Args.Contains('+'))
        {
            int pos = Args.IndexOf('+');
            FrontFormat = Args.Substring(0, pos) + FrontFormat + Args.Substring(pos + 1);
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