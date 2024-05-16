using Discord.Net;
using Discord.WebSocket;

public class SlashCommandHandler
{
    private DiscordSocketClient _client;

    public SlashCommandHandler(DiscordSocketClient client)
    {
        _client = client;
        client.SlashCommandExecuted += HandleSlashCommandAsync;
    }

    private List<string> SlicedResponse(string Result)
    {
        string[] Lines = Result.Split('\n');

        int Sum = 1900, Pointer = -1;
        List<string> Resultant = new List<string>();
        foreach (string Line in Lines)
        {
            if (Sum + Line.Count() + 1 > 1900) 
            {
                Resultant.Add("");
                Sum = 0;
                Pointer++;
            }
            // Console.WriteLine(Sum);
            Resultant[Pointer] += Line;
            Resultant[Pointer] += '\n';
            Sum += Line.Count() + 1;
        }

        // if (Resultant.Count > 1)
        // {
        //     for (int i = 0; i < Resultant.Count; i++) 
        //     {
        //         Resultant[i] = $"```{Resultant[i]}```";
        //     }
        // }
        
        return Resultant;
    }

    public async Task HandleSlashCommandAsync(SocketSlashCommand command)
    {
        if (command.Data.Name == "start")
        {
            await Globals._commands.Start(((string)command.Data.Options.First()), _client);
            await command.RespondAsync("Executed.");
        }
        else if (command.Data.Name == "finish")
        {
            await Globals._commands.Finish(command.Channel.Id, _client, command.User);
            await command.RespondAsync("Executed.");
        }
        else
        {
            string Result;
            if (command.Data.Options.Count == 0)
            {
                Result = Globals._inputHandler.HandleInput($"/{command.Data.Name}", command.Channel.Id, _client, command.User);
            }
            else
            {
                Result = Globals._inputHandler.HandleInput($"/{command.Data.Name} {((string)command.Data.Options.First())}", command.Channel.Id, _client, command.User);
            }
            // Console.WriteLine(Result);
            // Need to slice results
            List<string> SlicedResponses = SlicedResponse(Result);

            for (int i = 0; i < SlicedResponses.Count; i++)
            {
                if (i == 0)
                {
                    await command.RespondAsync(SlicedResponses[i]);
                } 
                else 
                {
                    await command.Channel.SendMessageAsync(SlicedResponses[i]);
                }
            }
        }

        
        // await command.Channel.SendMessageAsync("Hi");
        // await command.RespondAsync($"You executed {command.Data.Name} with parameter {command.Data.Options.First().Value}!");
    }
}