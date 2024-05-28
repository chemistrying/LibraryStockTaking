using Newtonsoft.Json;

namespace LibrarySystemApi.Models;

public static class Globals
{
    public static Config Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"))!;
    public static string? ActiveSessionId = null;
}