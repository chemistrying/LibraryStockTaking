public class Book
{
    public string Acno { get; }
    public string Callno1 { get; }
    public string Callno2 { get; }
    public string Name { get; }
    public string Status { get; }
    public string Publisher { get; }
    public string Author { get; }
    public string Language { get; }
    public string Category { get; }

    public Book(string[] Blocks)
    {
        for (int i = 0; i < 13; i++)
        {
            if (Blocks[i] == "")
            {
                Blocks[i] = "N/A";
            }
            switch (i)
            {
                case 0:
                    Acno = Blocks[i];
                    break;
                case 2:
                    Callno1 = Blocks[i];
                    break;
                case 4:
                    Callno2 = Blocks[i];
                    break;
                case 5:
                    Name = Blocks[i];
                    break;
                case 6:
                    Status = Blocks[i];
                    break;
                case 7:
                    Publisher = Blocks[i];
                    break;
                case 8:
                    Author = Blocks[i];
                    break;
                case 11:
                    Language = Blocks[i];
                    break;
                case 12:
                    Category = Blocks[i];
                    break;
                default:
                    break;
            }
        }
    }
}