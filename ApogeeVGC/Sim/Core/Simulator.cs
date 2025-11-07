using ApogeeVGC.Data;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;

namespace ApogeeVGC.Sim.Core;

public enum SimulatorResult
{
    Player1Win,
    Player2Win,
}

public class Simulator : IPlayerController, IBattleController
{
    public Battle? Battle { get; set; }
    public IPlayer? Player1 { get; set; }
    public IPlayer? Player2 { get; set; }
    public bool PrintDebug { get; set; }

    //public Simulator(Library library, BattleOptions battleOptions, bool printDebug = true)
    //{
    //    Battle = new Battle(battleOptions, library, this);
    //    Player1 = CreatePlayer(SideId.P1, battleOptions.Player1Options);
    //    Player2 = CreatePlayer(SideId.P2, battleOptions.Player2Options);
    //    PrintDebug = printDebug;
    //}

    public Task<Choice> RequestChoiceAsync(SideId sideId, IChoiceRequest choiceRequest,
        BattleRequestType requestType, BattlePerspective perspective,
        CancellationToken cancellationToken)
    {
        IPlayer player = GetPlayer(sideId);
        return player.GetNextChoiceAsync(choiceRequest, requestType, perspective,
            cancellationToken);
    }

    public void UpdatePlayerUi(SideId sideId, BattlePerspective perspective)
    {
        IPlayer player = GetPlayer(sideId);
        player.UpdateUi(perspective);
    }

    public BattlePerspective GetCurrentPerspective(SideId sideId)
    {
        if (Battle == null)
        {
            throw new InvalidOperationException("Battle is not initialized");
        }

        return Battle.GetPerspectiveForSide(sideId);
    }

    public PlayerUiType GetPlayerUiType(SideId sideId)
    {
        IPlayer player = GetPlayer(sideId);
        return player.UiType;
    }

    public async Task<SimulatorResult> Run(Library library, BattleOptions battleOptions,
        bool printDebug = true)
    {
        Battle = new Battle(battleOptions, library, this);
        Player1 = CreatePlayer(SideId.P1, battleOptions.Player1Options);
        Player2 = CreatePlayer(SideId.P2, battleOptions.Player2Options);
        PrintDebug = printDebug;

        if (PrintDebug)
        {
            Console.WriteLine("Starting battle simulation...");
        }

        try
        {
            // Set a reasonable timeout for the entire battle
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(30));

            // Start the battle asynchronously
            await Battle.StartAsync(cancellationTokenSource.Token);

            // The battle should now be complete or cancelled
            if (PrintDebug)
            {
                Console.WriteLine("Battle has ended.");
            }

            return DetermineWinner();
        }
        catch (OperationCanceledException)
        {
            if (PrintDebug)
            {
                Console.WriteLine("Battle was cancelled due to timeout");
            }

            return DetermineTiebreakWinner();
        }
        catch (Exception ex)
        {
            if (PrintDebug)
            {
                Console.WriteLine($"Battle error: {ex.Message}");
            }

            throw;
        }
    }

    //public SimulatorResult RunSync()
    //{
    //    return Run().GetAwaiter().GetResult();
    //}

    private SimulatorResult DetermineWinner()
    {
        // Check if we have a winner
        if (!string.IsNullOrEmpty(Battle.Winner))
        {
            // Winner is stored as side ID string ("p1" or "p2")
            bool isP1Winner = Battle.Winner.Equals("p1", StringComparison.OrdinalIgnoreCase);

            if (PrintDebug)
            {
                string winnerName = isP1Winner ? "Player 1" : "Player 2";
                Console.WriteLine($"Winner: {winnerName}");
            }

            return isP1Winner ? SimulatorResult.Player1Win : SimulatorResult.Player2Win;
        }

        // No clear winner - use tiebreak
        if (PrintDebug)
        {
            Console.WriteLine("Battle ended without a clear winner");
        }

        return DetermineTiebreakWinner();
    }

    private SimulatorResult DetermineTiebreakWinner()
    {
        //int side1TeamHealthTotal = Battle.Side1.HealthTeamTotal;
        //int side2TeamHealthTotal = Battle.Side2.HealthTeamTotal;

        //if (side1TeamHealthTotal > side2TeamHealthTotal)
        //    return Battle.Side1.PlayerId == PlayerId.Player1 ? SimulatorResult.Player1Win : SimulatorResult.Player2Win;
        //if (side2TeamHealthTotal > side1TeamHealthTotal)
        //    return Battle.Side2.PlayerId == PlayerId.Player1 ? SimulatorResult.Player1Win : SimulatorResult.Player2Win;

        //// Pick randomly if still tied. Use battle's random for consistency
        //var rand = new Random(Battle.BattleSeed ?? 1);
        //return rand.Next(2) == 0 ? SimulatorResult.Player1Win : SimulatorResult.Player2Win;

        // For now, always return Player 1 as winner in tiebreak
        Console.WriteLine("Tiebreaker applied: Player 1 declared winner by default.");
        return SimulatorResult.Player1Win;
    }

    private IPlayer CreatePlayer(SideId sideId, PlayerOptions options)
    {
        return options.Type switch
        {
            PlayerType.Random => new PlayerRandom(sideId, options, this),
            PlayerType.Gui => CreateGuiPlayer(sideId, options),
            PlayerType.Mcts => throw new NotImplementedException("MCTS player not implemented yet"),
            _ => throw new ArgumentOutOfRangeException($"Unknown player type: {options.Type}"),
        };
    }

    private PlayerGui CreateGuiPlayer(SideId sideId, PlayerOptions options)
    {
        // PlayerGui constructor now handles GuiWindow from options
        return new PlayerGui(sideId, options, this);
    }

    private IPlayer GetPlayer(SideId sideId)
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
}