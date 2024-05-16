public class Verdict
{
    List<string> Feedback;
    public Verdict()
    {
        Feedback = new List<string>();
    }

    public void Add(string Comment)
    {
        Feedback.Add(Comment);
    }

    public string ReturnFeedback() {
        string Result = "";
        foreach (string Comment in Feedback)
        {
            Result += Comment;
            Result += '\n';
        }
        return Result;
    }

    public void Reset()
    {
        Feedback.RemoveRange(0, Feedback.Count);
    }
}