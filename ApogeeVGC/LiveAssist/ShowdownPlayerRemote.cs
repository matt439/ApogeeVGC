using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.BattleClasses;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// Showdown player that delegates all decisions to a remote MctsWorkerServer.
/// The home client uses this to offload MCTS computation to EC2.
///
/// Unlike other IShowdownPlayer implementations, this player handles
/// battle lines and requests through the worker client rather than locally.
/// The ShowdownBattleAgent calls ChooseBattle, but for the remote player
/// the actual decision happens in HandleRequest via the orchestrator.
/// </summary>
public sealed class ShowdownPlayerRemote : IShowdownPlayer
{
    private readonly MctsWorkerClient _worker;
    private string? _lastChoice;

    public string Name => "Remote-Ensemble";

    public ShowdownPlayerRemote(MctsWorkerClient worker)
    {
        _worker = worker;
    }

    /// <summary>
    /// Forward battle protocol lines to the remote worker.
    /// </summary>
    public async Task SendBattleLinesAsync(string[] lines, CancellationToken ct = default)
    {
        await _worker.SendBattleLinesAsync(lines, ct);
    }

    /// <summary>
    /// Forward a request to the remote worker and get the decision.
    /// Returns the /choose command string, or null if no decision needed.
    /// </summary>
    public async Task<string?> SendRequestAsync(string requestJson, CancellationToken ct = default)
    {
        return await _worker.SendRequestAsync(requestJson, ct);
    }

    /// <summary>
    /// Initialize the worker for a new battle.
    /// </summary>
    public async Task InitAsync(string format, CancellationToken ct = default)
    {
        await _worker.SendInitAsync(format, ct);
    }

    /// <summary>
    /// Reset the worker between battles.
    /// </summary>
    public async Task ResetAsync(CancellationToken ct = default)
    {
        await _worker.SendResetAsync(ct);
    }

    // IShowdownPlayer — not used directly for remote player
    // The orchestrator calls SendRequestAsync instead
    public (LegalAction BestA, LegalAction? BestB) ChooseBattle(
        BattlePerspective perspective, LegalActionSet actions,
        bool[] maskA, bool[] maskB)
    {
        // This shouldn't be called for remote player — decisions come via SendRequestAsync
        throw new InvalidOperationException(
            "Remote player decisions are handled via SendRequestAsync, not ChooseBattle");
    }
}
