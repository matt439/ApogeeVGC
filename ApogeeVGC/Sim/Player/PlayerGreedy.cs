using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Player;

/// <summary>
/// Greedy max-damage AI player. Picks the move with the highest estimated damage
/// (BasePower × TypeEffectiveness × STAB) each turn. Falls back to a random move
/// when no damaging option exists. Never switches voluntarily.
/// </summary>
public class PlayerGreedy(SideId sideId, PlayerOptions options, IBattleController battleController)
    : IPlayer
{
    public SideId SideId { get; } = sideId;
    public PlayerOptions Options { get; } = options;
    public PlayerUiType UiType => PlayerUiType.None;
    public IBattleController BattleController { get; } = battleController;
    public bool PrintDebug { get; } = options.PrintDebug;

    private readonly Prng _random = options.Seed is null ? new Prng(null) : new Prng(options.Seed);
    private readonly TypeChart _typeChart = new();

    public Choice GetChoiceSync(IChoiceRequest choiceRequest, BattleRequestType requestType,
        Func<BattlePerspective> perspectiveFactory)
    {
        return choiceRequest switch
        {
            TeamPreviewRequest tpr => GetRandomTeamPreviewChoice(tpr),
            MoveRequest mr => GetGreedyMoveChoice(mr, perspectiveFactory),
            SwitchRequest sr => GetRandomSwitchChoice(sr),
            _ => throw new NotImplementedException(
                $"Request type {choiceRequest.GetType().Name} not implemented"),
        };
    }

    public Task<Choice> GetNextChoiceAsync(IChoiceRequest choiceRequest,
        BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
        // Greedy player needs the perspective factory for targeting info,
        // but the async path provides a snapshot — wrap it.
        Choice choice = choiceRequest switch
        {
            TeamPreviewRequest tpr => GetRandomTeamPreviewChoice(tpr),
            MoveRequest mr => GetGreedyMoveChoice(mr, () => perspective),
            SwitchRequest sr => GetRandomSwitchChoice(sr),
            _ => throw new NotImplementedException(
                $"Request type {choiceRequest.GetType().Name} not implemented"),
        };
        ChoiceSubmitted?.Invoke(this, choice);
        return Task.FromResult(choice);
    }

    public void UpdateUi(BattlePerspective perspective) { }
    public void UpdateEvents(IEnumerable<BattleEvent> events) { }
    public event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    public event EventHandler<Choice>? ChoiceSubmitted;
    public Task NotifyTimeoutWarningAsync(TimeSpan remainingTime) => Task.CompletedTask;
    public Task NotifyChoiceTimeoutAsync() => Task.CompletedTask;

    // ── Greedy move selection ──────────────────────────────────────────

    private Choice GetGreedyMoveChoice(MoveRequest request, Func<BattlePerspective> perspectiveFactory)
    {
        BattlePerspective perspective = perspectiveFactory();
        var actions = new List<ChosenAction>(request.Active.Count);

        for (int pokemonIndex = 0; pokemonIndex < request.Active.Count; pokemonIndex++)
        {
            PokemonMoveRequestData? pokemonRequest = request.Active[pokemonIndex];

            if (pokemonRequest == null)
            {
                actions.Add(new ChosenAction
                {
                    Choice = ChoiceType.Pass,
                    Pokemon = null,
                    MoveId = MoveId.None,
                });
                continue;
            }

            // Attacker info for STAB
            PokemonPerspective? attacker = pokemonIndex < perspective.PlayerSide.Active.Count
                ? perspective.PlayerSide.Active[pokemonIndex]
                : null;

            // Collect active opponents
            var opponents = new List<(PokemonPerspective Poke, int Slot)>();
            for (int oi = 0; oi < perspective.OpponentSide.Active.Count; oi++)
            {
                PokemonPerspective? opp = perspective.OpponentSide.Active[oi];
                if (opp is { Fainted: false })
                    opponents.Add((opp, oi + 1)); // target loc is 1-indexed
            }

            // Score all damaging moves against all opponents
            double bestScore = -1;
            int bestMoveIndex = -1;
            int bestTargetLoc = 0;

            for (int mi = 0; mi < pokemonRequest.Moves.Length; mi++)
            {
                PokemonMoveData moveData = pokemonRequest.Moves[mi];
                if (IsDisabled(moveData.Disabled)) continue;

                Move move = moveData.Move;
                if (move.Category == MoveCategory.Status || move.BasePower <= 0) continue;

                bool isSpread = IsSpreadMove(move.Target);

                if (isSpread)
                {
                    // Spread moves hit all opponents — sum the scores
                    double totalScore = 0;
                    foreach ((PokemonPerspective opp, _) in opponents)
                    {
                        totalScore += ScoreMove(move, attacker, opp);
                    }

                    // Spread moves do 0.75× in doubles
                    totalScore *= 0.75;

                    if (totalScore > bestScore)
                    {
                        bestScore = totalScore;
                        bestMoveIndex = mi;
                        bestTargetLoc = 0; // auto-target for spread
                    }
                }
                else if (IsFoeTargeting(move.Target))
                {
                    // Single-target: score against each opponent individually
                    foreach ((PokemonPerspective opp, int slot) in opponents)
                    {
                        double score = ScoreMove(move, attacker, opp);
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMoveIndex = mi;
                            bestTargetLoc = slot;
                        }
                    }
                }
                else
                {
                    // Self/ally targeting — just treat as a low-priority option
                    double score = move.BasePower;
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMoveIndex = mi;
                        bestTargetLoc = 0;
                    }
                }
            }

            // Fall back to a random enabled move if no damaging move found
            if (bestMoveIndex < 0)
            {
                bestMoveIndex = PickRandomEnabledMove(pokemonRequest);
                bestTargetLoc = GetTargetLoc(pokemonRequest.Moves[bestMoveIndex].Move.Target);
            }

            PokemonMoveData selected = pokemonRequest.Moves[bestMoveIndex];

            actions.Add(new ChosenAction
            {
                Choice = ChoiceType.Move,
                Pokemon = null,
                MoveId = selected.Id,
                TargetLoc = bestTargetLoc,
            });
        }

        return new Choice { Actions = actions };
    }

    private double ScoreMove(Move move, PokemonPerspective? attacker, PokemonPerspective target)
    {
        double effectiveness = _typeChart
            .GetMoveEffectiveness(target.Types, move.Type)
            .ToMultiplier();

        if (effectiveness == 0) return 0; // immune

        double stab = 1.0;
        if (attacker != null && move.Type is not (MoveType.Stellar or MoveType.Unknown))
        {
            PokemonType moveAsPokemonType = move.Type.ConvertToPokemonType();
            if (attacker.Types.Contains(moveAsPokemonType))
                stab = 1.5;
        }

        return move.BasePower * effectiveness * stab;
    }

    private int PickRandomEnabledMove(PokemonMoveRequestData pokemonRequest)
    {
        var enabled = new List<int>();
        for (int i = 0; i < pokemonRequest.Moves.Length; i++)
        {
            if (!IsDisabled(pokemonRequest.Moves[i].Disabled))
                enabled.Add(i);
        }

        if (enabled.Count > 0)
            return enabled[_random.Random(0, enabled.Count)];

        // All disabled — return first move (engine will convert to Struggle)
        return 0;
    }

    private int GetTargetLoc(MoveTarget target)
    {
        return target switch
        {
            MoveTarget.Normal or MoveTarget.Any or MoveTarget.AdjacentFoe
                => _random.Random(1, 3),
            MoveTarget.AdjacentAlly => -_random.Random(1, 3),
            MoveTarget.AdjacentAllyOrSelf => -_random.Random(1, 3),
            _ => 0,
        };
    }

    private static bool IsFoeTargeting(MoveTarget target) =>
        target is MoveTarget.Normal or MoveTarget.Any or MoveTarget.AdjacentFoe;

    private static bool IsSpreadMove(MoveTarget target) =>
        target is MoveTarget.AllAdjacentFoes or MoveTarget.AllAdjacent or MoveTarget.All;

    private static bool IsDisabled(MoveIdBoolUnion? disabled) =>
        disabled is not null && disabled.IsTrue();

    // ── Team preview & forced switch (same as PlayerRandom) ────────────

    private Choice GetRandomTeamPreviewChoice(TeamPreviewRequest request)
    {
        var pokemon = request.Side.Pokemon;
        int bringCount = request.MaxChosenTeamSize ?? pokemon.Count;
        var order = Enumerable.Range(0, pokemon.Count).ToList();

        for (int i = order.Count - 1; i > 0; i--)
        {
            int j = _random.Random(0, i + 1);
            (order[i], order[j]) = (order[j], order[i]);
        }

        order = order.Take(bringCount).ToList();

        var actions = order.Select((originalPokemonIndex, newPosition) => new ChosenAction
        {
            Choice = ChoiceType.Team,
            Pokemon = null,
            MoveId = MoveId.None,
            Index = newPosition,
            TargetLoc = originalPokemonIndex,
            Priority = -newPosition,
        }).ToList();

        return new Choice { Actions = actions };
    }

    private Choice GetRandomSwitchChoice(SwitchRequest request)
    {
        var actions = new List<ChosenAction>();
        var usedSlots = new HashSet<int>();

        for (int i = 0; i < request.ForceSwitch.Count; i++)
        {
            if (!request.ForceSwitch[i]) continue;

            var available = request.Side.Pokemon
                .Select((p, index) => new { PokemonData = p, OriginalIndex = index })
                .Where(x => !x.PokemonData.Active
                             && x.PokemonData.Condition != ConditionId.Fainted
                             && !x.PokemonData.Reviving
                             && !usedSlots.Contains(x.OriginalIndex))
                .ToList();

            if (available.Count == 0)
            {
                actions.Add(new ChosenAction { Choice = ChoiceType.Pass, MoveId = MoveId.None });
                continue;
            }

            int randomIndex = _random.Random(0, available.Count);
            var selected = available[randomIndex];
            usedSlots.Add(selected.OriginalIndex);

            actions.Add(new ChosenAction
            {
                Choice = ChoiceType.Switch,
                Pokemon = null,
                MoveId = MoveId.None,
                Index = selected.OriginalIndex,
            });
        }

        return new Choice { Actions = actions };
    }
}
