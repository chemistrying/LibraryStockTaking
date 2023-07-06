using Discord.WebSocket;

public class Stocker
{
    private SortedSet<string> Container;

    public Stocker()
    {
        Container = new SortedSet<string>();
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
                    Add(Barcode);
                }
            }
        }
    }

    public bool Add(string Barcode)
    {
        return Container.Add(Barcode);
    }

    public bool Remove(string Barcode)
    {
        return Container.Remove(Barcode);
    }

    public bool Duped(string Barcode)
    {
        return Container.Contains(Barcode);
    }
}