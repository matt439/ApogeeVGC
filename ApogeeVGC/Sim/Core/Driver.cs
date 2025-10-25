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

        // Start the AI players (don't await - they run in background)
        var p1Task = p1.StartAsync();
        var p2Task = p2.StartAsync();

        // Start consuming the omniscient stream to see battle output
        var streamConsumerTask = Task.Run(async () =>
        {
            try
            {
                await foreach (string chunk in streams.Omniscient.ReadAllAsync())
                {
                    Console.WriteLine(chunk);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Stream consumer error: {ex.Message}");
            }
        });

        // Initialize the battle by writing commands to the omniscient stream
        string startCommand = $@">start {JsonSerializer.Serialize(spec)}
>player p1 {JsonSerializer.Serialize(p1Spec)}
>player p2 {JsonSerializer.Serialize(p2Spec)}";

        await streams.Omniscient.WriteAsync(startCommand);

        // Wait for all tasks to complete (or any to fault)
        try
        {
            await Task.WhenAny(
                Task.WhenAll(p1Task, p2Task, streamConsumerTask),
                Task.Delay(TimeSpan.FromMinutes(5)) // Timeout after 5 minutes
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Battle error: {ex.Message}");
        }
        finally
        {
            // Cleanup
            streams.Dispose();
            Console.WriteLine("Battle completed or timed out.");
        }
    }
}