public class Commands
{
    public Commands()
    {

    }

    public void Save(String Args)
    {
        String fileLocation = Args.Length == 0 ? Globals._currentFileLocation : Args.Substring(0, Args.IndexOf(' ') == -1 ? Args.Length : Args.IndexOf(' '));
        String furtherArgs = Args.IndexOf(' ') == -1 ? "" : Args.Substring(Args.IndexOf(' ') + 1);
        if (furtherArgs != "-y")
        {
            string Confirm = "";
            do
            {
                Console.Write($"Confirm saving into \"{fileLocation}.txt\" this file? [Y|N] ");
                Confirm = Console.ReadLine();
            }
            while (Confirm != "Y" && Confirm != "N");
            if (Confirm == "N")
            {
                Console.WriteLine("Save operation cancelled.");
            }
        }
        
        using (StreamWriter sw = File.AppendText(fileLocation + ".txt")){
            foreach (String Barcode in Globals.Buffer)
            {
                sw.WriteLine(Barcode);
            }
        }
    }
}