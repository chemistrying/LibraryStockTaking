namespace LibrarySystemApi;

public class Trie
{
    public class TrieNode
    {
        public string? Word { get; set; }
        public int Count;
        public Dictionary<char, int> Children = [];
    }

    private List<TrieNode> _nodeArray = [new() { Word = "" }];

    public void Insert(string newWord)
    {
        string currPrefix = "";
        int currIndex = 0;
        for (int i = 0; i < newWord.Length; i++)
        {
            Serilog.Log.Debug($"Current Count: {_nodeArray.Count}");
            char newChar = newWord[i];
            currPrefix += newChar;

            Serilog.Log.Debug($"Current Prefix: {currPrefix}");
            Serilog.Log.Debug($"Current Index: {currIndex}");

            if (!_nodeArray[currIndex].Children.ContainsKey(newChar))
            {   
                // add the new index
                _nodeArray[currIndex].Children.Add(newChar, _nodeArray.Count);

                // add the node to the array
                _nodeArray.Add(new TrieNode()
                {
                    Word = currPrefix,
                    Count = 0,
                    Children = []
                });
            }
            
            currIndex = _nodeArray[currIndex].Children[newChar];

            if (i + 1 == newWord.Length)
            {
                // add count
                _nodeArray[currIndex].Count++;
            }
        }
    }

    public void Delete(string word)
    {
        int currIndex = 0;
        for (int i = 0; i < word.Length; i++)
        {
            currIndex = _nodeArray[currIndex].Children[word[i]];
            
            if (i + 1 == word.Length)
            {
                // reduce count
                _nodeArray[currIndex].Count--;
            }
        }
    }

    public int Query(string word)
    {
        int currIndex = 0;
        for (int i = 0; i < word.Length; i++)
        {
            currIndex = _nodeArray[currIndex].Children[word[i]];
        }

        return _nodeArray[currIndex].Count;
    }
}