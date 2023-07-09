using Discord.WebSocket;

public class Stocker
{
    private SortedSet<string> Container;
    private Dictionary<ulong, SortedSet<string>> Bookshelves;

    public Stocker()
    {
        Container = new SortedSet<string>();
        Bookshelves = new Dictionary<ulong, SortedSet<string>>();
    }

    public void Load(IReadOnlyCollection<SocketTextChannel> channels) 
    {
        foreach (var channel in channels)
        {
            if (channel.Name != "general")
            {
                string[] Barcodes = File.ReadAllLines($"tempdoc\\{channel.Name}.txt");
                foreach (string Barcode in Barcodes)
                {
                    Add(Barcode, channel.Id);
                }
            }
        }
    }

    public bool Add(string Barcode, ulong Source)
    {
        if (!Bookshelves.Keys.Contains(Source)) {
            Bookshelves.Add(Source, new SortedSet<string>());
        }
        Bookshelves[Source].Add(Barcode);
        return Container.Add(Barcode);
    }

    public bool Remove(string Barcode, ulong Source)
    {
        Bookshelves[Source].Remove(Barcode);
        return Container.Remove(Barcode);
    }

    public bool Duped(string Barcode)
    {
        return Container.Contains(Barcode);
    }

    public string CheckLocation(string Barcode) {
        foreach (var Bookshelf in Bookshelves) {
            if (Bookshelf.Value.Contains(Barcode)) {
                return Globals._shelfName[Bookshelf.Key];
            }
        }
        return "None";
    }
}