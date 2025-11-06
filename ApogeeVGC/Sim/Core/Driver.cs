using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Generators;
using System.Text.Json;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Player;

namespace ApogeeVGC.Sim.Core;

public enum DriverMode
{
    //RandomVsRandom,
    //RandomVsRandomEvaluation,
    //RandomVsRandomEvaluationDoubles,
    ConsoleVsRandom,
    ConsoleVsRandomDoubles,
    //ConsoleVsConsole,
    //ConsoleVsMcts,
    //MctsVsRandom,
    //MctsVsRandomEvaluation,
    //MctsVsRandomEvaluationDoubles,
}

public class Driver
{
    private Library Library { get; } = new();

    public void Start(DriverMode mode)
    {
        switch (mode)
        {
            case DriverMode.ConsoleVsRandom:
            case DriverMode.ConsoleVsRandomDoubles:
                StartTest().GetAwaiter().GetResult();
                break;
            default:
                throw new NotImplementedException($"Driver mode {mode} is not implemented.");
        }
    }

    private async Task RunConsolveVsRandom()
    {
        Simulator simulator = new()
        {

        };

        throw new NotImplementedException();
    }

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
        Task p1Task = p1.StartAsync();
        Task p2Task = p2.StartAsync();

        // Start consuming the omniscient stream to see battle output
        Task streamConsumerTask = Task.Run(async () =>
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
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner stack trace: {ex.InnerException.StackTrace}");
                }
            }
        });

        // Initialize the battle by writing commands to the omniscient stream
        string startCommand = $"""
                               >start {JsonSerializer.Serialize(spec)}
                               >player p1 {JsonSerializer.Serialize(p1Spec)}
                               >player p2 {JsonSerializer.Serialize(p2Spec)}
                               """;

        await streams.Omniscient.WriteAsync(startCommand);

        // Wait for all tasks to complete (or timeout)
        try
        {
            Task allTasks = Task.WhenAll(p1Task, p2Task, streamConsumerTask);
            Task timeoutTask = Task.Delay(TimeSpan.FromMinutes(5));

            Task completedTask = await Task.WhenAny(allTasks, timeoutTask);

            if (completedTask == timeoutTask)
            {
                Console.WriteLine("Battle timed out after 5 minutes.");
            }
            else
            {
                // Wait for the allTasks to ensure we capture any exceptions
                await allTasks;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Battle error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                Console.WriteLine($"Inner stack trace: {ex.InnerException.StackTrace}");
            }
        }
        finally
        {
            // Ensure all tasks are completed before disposing streams
            // Give tasks a short grace period to finish after stream closure
            try
            {
                await Task.WhenAll(p1Task, p2Task, streamConsumerTask).WaitAsync(TimeSpan.FromSeconds(5));
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Warning: Some tasks did not complete within grace period.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Task completion error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner stack trace: {ex.InnerException.StackTrace}");
                }
            }

            // Cleanup
            streams.Dispose();
            Console.WriteLine("Battle completed or timed out.");
        }
    }
}