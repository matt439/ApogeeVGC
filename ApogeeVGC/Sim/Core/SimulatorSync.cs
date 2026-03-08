using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Player;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Core;

/// <summary>
/// Synchronous battle simulator for AI training (MCTS).
/// No async/threading complexity - perfect for fast rollouts.
/// </summary>
public class SimulatorSync : SimulatorBase
{
    public SimulatorResult Run(Library library, BattleOptions battleOptions, bool printDebug = true)
    {
        Battle = new Battle(battleOptions, library);
        Player1 = CreatePlayer(SideId.P1, battleOptions.Player1Options);
        Player2 = CreatePlayer(SideId.P2, battleOptions.Player2Options);
        PrintDebug = printDebug;

        // Validate teams against format rules
        var validator = new TeamValidator(library, Battle.Format);
        ValidateTeams(validator, battleOptions, printDebug);

        // Subscribe to Battle events
        Battle.ChoiceRequested += OnChoiceRequested;
        Battle.UpdateRequested += OnUpdateRequested;
        Battle.BattleEnded += OnBattleEnded;

        try
        {
            // Start the battle - this will set up team preview or first turn
            Battle.Start();

            // Main battle loop - keep processing until battle ends
            while (!Battle.Ended)
            {
                if (Battle.RequestState != RequestState.None)
                {
                    if (Battle.Ended)
                    {
                        break;
                    }

                    Battle.RequestPlayerChoices();
                }
                else
                {
                    break;
                }
            }

            return DetermineWinner();
        }
        catch (Exception ex)
        {
            if (PrintDebug)
            {
                Console.WriteLine($"Battle error: {ex.GetType().Name}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine(
                        $"Inner exception: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                }

                Console.WriteLine("Stack trace (last 10 frames):");
                var frames = ex.StackTrace?.Split('\n').Take(10);
                if (frames != null)
                {
                    foreach (var frame in frames)
                    {
                        Console.WriteLine($"  {frame.Trim()}");
                    }
                }
            }

            throw;
        }
        finally
        {
            // Unsubscribe from events
            if (Battle != null)
            {
                Battle.ChoiceRequested -= OnChoiceRequested;
                Battle.UpdateRequested -= OnUpdateRequested;
                Battle.BattleEnded -= OnBattleEnded;
            }
        }
    }

    /// <summary>
    /// Event handler for when Battle requests a choice from a player.
    /// Synchronously gets the choice and submits it to the battle.
    /// Falls back to AutoChoose if the player's choice is rejected.
    /// </summary>
    private void OnChoiceRequested(object? sender, BattleChoiceRequestEventArgs e)
    {
        IPlayer player = GetPlayer(e.SideId);
        Side side = Battle!.Sides.First(s => s.Id == e.SideId);

        // Get choice synchronously
        Choice choice = player.GetChoiceSync(e.Request, e.RequestType, () => e.Perspective);

        // If the choice is empty, use AutoChoose
        if (choice.Actions.Count == 0)
        {
            side.AutoChoose();
            // AutoChoose builds the choice directly on the side. We must NOT
            // re-submit via Battle.Choose because side.Choose -> ClearChoice()
            // would recalculate ForcedSwitchesLeft from SwitchFlags that
            // ChooseSwitch already cleared, causing switch actions to be rejected.
            if (Battle.AllChoicesDone()) Battle.CommitChoices();
            return;
        }

        // Submit the choice — if it fails, fall back to AutoChoose.
        if (!Battle.Choose(e.SideId, choice))
        {
            side.AutoChoose();
            if (Battle.AllChoicesDone()) Battle.CommitChoices();
        }
    }

    /// <summary>
    /// Event handler for battle end notification.
    /// </summary>
    private void OnBattleEnded(object? sender, BattleEndedEventArgs e)
    {
        // Battle ended - no action needed, just for notification
    }

    protected override IPlayer CreatePlayer(SideId sideId, PlayerOptions options)
    {
        return options.Type switch
        {
            Player.PlayerType.Random => new PlayerRandom(sideId, options, this),
            Player.PlayerType.Console => throw new NotSupportedException(
                "Console player cannot be used in synchronous mode"),
            Player.PlayerType.Gui => throw new NotSupportedException(
                "GUI player cannot be used in synchronous mode"),
            Player.PlayerType.Mcts => Mcts.PlayerMcts.Create(sideId, options, this),
            Player.PlayerType.MctsStandalone => new Mcts.PlayerMctsStandalone(
                sideId, options, this,
                new Mcts.MctsSearchStandalone(options.MctsConfig ?? new Mcts.MctsConfig())),
            Player.PlayerType.MctsDL => Mcts.PlayerMctsDL.Create(sideId, options, this),
            Player.PlayerType.Greedy => new PlayerGreedy(sideId, options, this),
            _ => throw new ArgumentOutOfRangeException($"Unknown player type: {options.Type}"),
        };
    }
}
