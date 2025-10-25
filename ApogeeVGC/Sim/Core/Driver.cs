using ApogeeVGC.Data;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Generators;
using System.Text.Json;

namespace ApogeeVGC.Sim.Core;

public class Driver
{
    private Library Library { get; } = new();

    public async Task StartTest()
    {
        PlayerStreams streams = BattleStreamExtensions.GetPlayerStreams(new BattleStream(Library));

        // Battle specification
        var spec = new
        {
            formatid = "gen9customgame",
        };

        PlayerOptions p1Spec = new()
        {
            Name = "Bot 1",
            Team = TeamGenerator.GenerateTestTeam(Library),
        };

        PlayerOptions p2Spec = new()
        {
            Name = "Bot 2",
            Team = TeamGenerator.GenerateTestTeam(Library),
        };

        var p1 = new RandomPlayerAi(streams.P1);
        var p2 = new RandomPlayerAi(streams.P2);

        Console.WriteLine($"p1 is {p1.GetType().Name}");
        Console.WriteLine($"p2 is {p2.GetType().Name}");

        // Start the AI players
        _ = p1.StartAsync();
        _ = p2.StartAsync();

        // Start consuming the omniscient stream to see battle output
        _ = Task.Run(async () =>
        {
            await foreach (string chunk in streams.Omniscient.ReadAllAsync())
            {
                Console.WriteLine(chunk);
            }
        });

        // Initialize the battle by writing commands to the omniscient stream
        string startCommand = $"""
                               >start {JsonSerializer.Serialize(spec)}
                                           >player p1 {JsonSerializer.Serialize(p1Spec)}
                                           >player p2 {JsonSerializer.Serialize(p2Spec)}
                               """;

        await streams.Omniscient.WriteAsync(startCommand);
    }
}