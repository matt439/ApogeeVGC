using System.Diagnostics;
using ApogeeVGC.Data;
using ApogeeVGC.Mcts;
using ApogeeVGC.Mcts.Ensemble;
using ApogeeVGC.Mcts.Ensemble.MiniModels;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.FormatClasses;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// Ensemble MCTS player for Showdown ladder battles.
/// Uses the shadow battle for MCTS simulation with time-based search
/// (runs as many iterations as possible within the turn time budget).
/// </summary>
public sealed class ShowdownPlayerEnsemble : IShowdownPlayer
{
    private readonly MctsSearchEnsemble _search;
    private readonly Library _library;
    private readonly FormatId _formatId;
    private ShadowBattle? _shadowBattle;
    private SideId _sideId;

    /// <summary>
    /// Time budget per move decision in seconds.
    /// Showdown's turn timer is ~150s; we use most of it with a safety margin.
    /// </summary>
    public int TimeBudgetSeconds { get; set; } = 8;

    public string Name => "Ensemble";

    public ShowdownPlayerEnsemble(Library library, FormatId formatId)
    {
        _library = library;
        _formatId = formatId;

        IMiniModel[] models =
        [
            new DamageMaxMiniModel(),
            new KOSeekingMiniModel(),
            new KOAvoidanceMiniModel(),
            new TypePositioningMiniModel(),
            new DamageMinMiniModel(),
            new SpeedAdvantageMiniModel(),
            new StatusSpreadingMiniModel(),
            new ProtectPredictionMiniModel(),
        ];

        string configPath = Environment.GetEnvironmentVariable("APOGEE_ENSEMBLE_CONFIG")
                            ?? Path.Combine("Tools", "DLModel", "ensemble_config.json");
        EnsembleConfig config = EnsembleConfig.Load(configPath);

        var ensemble = new EnsembleEvaluator(models, config);
        var mctsConfig = new MctsConfig
        {
            NumIterations = int.MaxValue, // Time-based, not iteration-based
        };
        _search = new MctsSearchEnsemble(mctsConfig, ensemble);
    }

    /// <summary>
    /// Initialize the shadow battle from the ShowdownState at team preview.
    /// Must be called before the first ChooseBattle().
    /// </summary>
    public void InitializeShadowBattle(ShowdownState state, SideId sideId)
    {
        _sideId = sideId;
        _shadowBattle = new ShadowBattle(_library, _formatId);
        _shadowBattle.Initialize(state);
        Console.WriteLine($"[Ensemble] Shadow battle initialized (we are {sideId})");
    }

    /// <summary>
    /// Advance the shadow battle after each turn using inferred choices.
    /// Called by the agent after processing battle protocol lines.
    /// </summary>
    /// <summary>
    /// Advance the shadow battle to match the real Showdown state.
    /// </summary>
    public void AdvanceShadowTurn(string? p1Choice, string? p2Choice, ShowdownState state)
    {
        _shadowBattle?.AdvanceTurn(p1Choice, p2Choice, state);
    }

    public (LegalAction BestA, LegalAction? BestB) ChooseBattle(
        BattlePerspective perspective,
        LegalActionSet actions,
        bool[] maskA,
        bool[] maskB)
    {
        // Single action per slot — no search needed
        if (actions.SlotA.Count == 1 && actions.SlotB.Count <= 1)
        {
            return (actions.SlotA[0],
                actions.SlotB.Count > 0 ? actions.SlotB[0] : null);
        }

        // Get shadow battle for MCTS
        Battle? battle = _shadowBattle?.GetBattle();
        if (battle == null)
        {
            Console.WriteLine("[Ensemble] WARNING: No shadow battle — falling back to first legal action");
            return (actions.SlotA[0],
                actions.SlotB.Count > 0 ? actions.SlotB[0] : null);
        }

        // Time-based search: run until budget expires
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(TimeBudgetSeconds));
        var stopwatch = Stopwatch.StartNew();

        (LegalAction bestA, LegalAction? bestB) = _search.Search(
            battle, _sideId, actions, cts.Token);

        stopwatch.Stop();

        // Log search stats
        int iterations = _search.LastSearchIterations;
        Console.WriteLine(
            $"  Search: {iterations:N0} iterations in {stopwatch.Elapsed.TotalSeconds:F1}s " +
            $"({iterations / stopwatch.Elapsed.TotalSeconds:F0} iter/s)");

        // Prevent both slots switching to same Pokemon
        if (bestB.HasValue &&
            bestA.ChoiceType == ChoiceType.Switch &&
            bestB.Value.ChoiceType == ChoiceType.Switch &&
            bestA.SwitchIndex == bestB.Value.SwitchIndex)
        {
            // Fall back to first non-conflicting action for slot B
            foreach (LegalAction alt in actions.SlotB)
            {
                if (alt.ChoiceType != ChoiceType.Switch || alt.SwitchIndex != bestA.SwitchIndex)
                {
                    bestB = alt;
                    break;
                }
            }
        }

        return (bestA, bestB);
    }
}
