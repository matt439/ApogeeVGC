using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Player;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Core;

/// <summary>
/// Base class for battle simulators, containing shared logic for
/// battle management, player creation, team validation, and winner determination.
/// </summary>
public abstract class SimulatorBase : IBattleController
{
    public Battle? Battle { get; set; }
    public IPlayer? Player1 { get; set; }
    public IPlayer? Player2 { get; set; }
    public bool PrintDebug { get; set; }

    public BattlePerspective GetCurrentPerspective(SideId sideId)
    {
        if (Battle == null)
        {
            throw new InvalidOperationException("Battle is not initialized");
        }

        return Battle.GetPerspectiveForSide(sideId);
    }

    public virtual void ClearMessages()
    {
        // No-op by default; override in async simulator for GUI support
    }

    protected IPlayer GetPlayer(SideId sideId)
    {
        return sideId switch
        {
            SideId.P1 => Player1 ??
                         throw new InvalidOperationException("Player 1 is not initialized"),
            SideId.P2 => Player2 ??
                         throw new InvalidOperationException("Player 2 is not initialized"),
            _ => throw new ArgumentOutOfRangeException(nameof(sideId), $"Invalid SideId: {sideId}"),
        };
    }

    protected abstract IPlayer CreatePlayer(SideId sideId, PlayerOptions options);

    protected void OnUpdateRequested(object? sender, BattleUpdateEventArgs e)
    {
        var player = GetPlayer(e.SideId);
        player.UpdateEvents(e.Events);
    }

    protected SimulatorResult DetermineWinner()
    {
        if (!Battle!.Ended)
        {
            if (PrintDebug)
            {
                Console.WriteLine("WARNING: DetermineWinner called but battle hasn't ended yet");
            }

            return SimulatorResult.Tie;
        }

        if (!string.IsNullOrEmpty(Battle.Winner))
        {
            // Check against actual side names first
            string p1Name = Battle.P1.Name;
            string p2Name = Battle.P2.Name;

            if (Battle.Winner.Equals(p1Name, StringComparison.OrdinalIgnoreCase))
            {
                return SimulatorResult.Player1Win;
            }

            if (Battle.Winner.Equals(p2Name, StringComparison.OrdinalIgnoreCase))
            {
                return SimulatorResult.Player2Win;
            }

            // Fallback: check for side ID strings ("p1", "p2") for backwards compatibility
            if (Battle.Winner.Equals("p1", StringComparison.OrdinalIgnoreCase))
            {
                return SimulatorResult.Player1Win;
            }

            if (Battle.Winner.Equals("p2", StringComparison.OrdinalIgnoreCase))
            {
                return SimulatorResult.Player2Win;
            }

            // Unknown winner format
            Console.WriteLine($"WARNING: Unknown winner format: '{Battle.Winner}'");
            return SimulatorResult.Tie;
        }

        // No clear winner - use tiebreak
        return DetermineTiebreakWinner();
    }

    protected SimulatorResult DetermineTiebreakWinner()
    {
        if (PrintDebug)
        {
            Console.WriteLine("Executing tiebreaker...");
        }

        Battle!.Tiebreak();

        if (!string.IsNullOrEmpty(Battle.Winner))
        {
            return DetermineWinner();
        }

        return SimulatorResult.Tie;
    }

    protected static void ValidateTeams(TeamValidator validator, BattleOptions battleOptions, bool printDebug)
    {
        var player1Result = validator.ValidateTeam(battleOptions.Player1Options.Team);
        var player2Result = validator.ValidateTeam(battleOptions.Player2Options.Team);

        var allProblems = new List<string>();

        if (!player1Result.IsValid)
        {
            allProblems.Add($"Player 1 ({battleOptions.Player1Options.Name}) team validation failed:");
            foreach (var problem in player1Result.Problems)
            {
                allProblems.Add($"  - {problem}");
            }
        }

        if (!player2Result.IsValid)
        {
            allProblems.Add($"Player 2 ({battleOptions.Player2Options.Name}) team validation failed:");
            foreach (var problem in player2Result.Problems)
            {
                allProblems.Add($"  - {problem}");
            }
        }

        if (allProblems.Count > 0)
        {
            var errorMessage = string.Join(Environment.NewLine, allProblems);
            if (printDebug)
            {
                Console.WriteLine($"[Simulator] Team validation failed:{Environment.NewLine}{errorMessage}");
            }

            throw new InvalidOperationException($"Team validation failed:{Environment.NewLine}{errorMessage}");
        }

        if (printDebug)
        {
            Console.WriteLine("[Simulator] Team validation passed for both players");
        }
    }

    // Force win/lose/tie methods for testing purposes

    /// <summary>
    /// Forces the specified side to win the battle.
    /// </summary>
    public virtual void ForceWin(SideId sideId)
    {
        if (Battle == null)
        {
            throw new InvalidOperationException("Battle is not initialized");
        }

        var side = Battle.Sides.First(s => s.Id == sideId);
        Battle.Win(side);
    }

    /// <summary>
    /// Forces a tie in the battle.
    /// </summary>
    public virtual void ForceTie()
    {
        if (Battle == null)
        {
            throw new InvalidOperationException("Battle is not initialized");
        }

        Battle.Tie();
    }

    /// <summary>
    /// Forces the specified side to lose the battle.
    /// </summary>
    public virtual void ForceLose(SideId sideId)
    {
        if (Battle == null)
        {
            throw new InvalidOperationException("Battle is not initialized");
        }

        var side = Battle.Sides.First(s => s.Id == sideId);
        Battle.Lose(side);
    }
}
