using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Mcts;

/// <summary>
/// A single legal action for one active slot, with its vocab index
/// and enough information to construct a ChosenAction.
/// </summary>
public readonly struct LegalAction
{
    public int VocabIndex { get; init; }
    public ChoiceType ChoiceType { get; init; }
    public MoveId MoveId { get; init; }
    public int TargetLoc { get; init; }
    public int SwitchIndex { get; init; }
    public MoveType? Terastallize { get; init; }
}

/// <summary>
/// The set of legal actions for both active slots in a single turn.
/// </summary>
public sealed class LegalActionSet
{
    public required IReadOnlyList<LegalAction> SlotA { get; init; }
    public required IReadOnlyList<LegalAction> SlotB { get; init; }
}

public sealed class ActionMapper
{
    private readonly Vocab _vocab;
    private readonly Random _rng = new();

    public ActionMapper(Vocab vocab)
    {
        _vocab = vocab;
    }

    public LegalActionSet GetLegalActions(IChoiceRequest request, BattlePerspective perspective)
    {
        return request switch
        {
            MoveRequest mr => GetLegalActionsForMove(mr, perspective),
            SwitchRequest sr => GetLegalActionsForSwitch(sr, perspective),
            _ => new LegalActionSet
            {
                SlotA = [new LegalAction { VocabIndex = Vocab.NoneActionIndex, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None }],
                SlotB = [],
            },
        };
    }

    private LegalActionSet GetLegalActionsForMove(MoveRequest request, BattlePerspective perspective)
    {
        var slotA = request.Active.Count > 0
            ? GetSlotLegalActions(request.Active[0], request, perspective, 0)
            : [new LegalAction { VocabIndex = Vocab.NoneActionIndex, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None }];

        var slotB = request.Active.Count > 1
            ? GetSlotLegalActions(request.Active[1], request, perspective, 1)
            : [];

        return new LegalActionSet { SlotA = slotA, SlotB = slotB };
    }

    private List<LegalAction> GetSlotLegalActions(
        PokemonMoveRequestData? pokemonRequest,
        MoveRequest request,
        BattlePerspective perspective,
        int slotIndex)
    {
        if (pokemonRequest == null)
        {
            return [new LegalAction { VocabIndex = Vocab.NoneActionIndex, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None }];
        }

        var actions = new List<LegalAction>();

        // Check tera availability
        MoveType? teraType = pokemonRequest.CanTerastallize switch
        {
            MoveTypeMoveTypeFalseUnion mtfu => mtfu.MoveType,
            _ => null,
        };

        // Add move actions
        for (var i = 0; i < pokemonRequest.Moves.Count; i++)
        {
            PokemonMoveData move = pokemonRequest.Moves[i];
            if (IsDisabled(move.Disabled)) continue;

            int vocabIdx = _vocab.GetMoveActionIndex(move.Id);
            int targetLoc = GetTargetLocation(move.Move.Target);

            actions.Add(new LegalAction
            {
                VocabIndex = vocabIdx,
                ChoiceType = ChoiceType.Move,
                MoveId = move.Id,
                TargetLoc = targetLoc,
            });

            // Add tera variant if available
            if (teraType.HasValue)
            {
                actions.Add(new LegalAction
                {
                    VocabIndex = vocabIdx, // Same vocab index — model doesn't distinguish
                    ChoiceType = ChoiceType.Move,
                    MoveId = move.Id,
                    TargetLoc = targetLoc,
                    Terastallize = teraType,
                });
            }
        }

        // Add switch actions (if not trapped)
        bool isTrapped = pokemonRequest.Trapped == true;
        if (!isTrapped)
        {
            AddSwitchActions(actions, request.Side, perspective);
        }

        // Fallback: if everything is disabled and can't switch, use first move (Struggle)
        if (actions.Count == 0 && pokemonRequest.Moves.Count > 0)
        {
            PokemonMoveData firstMove = pokemonRequest.Moves[0];
            actions.Add(new LegalAction
            {
                VocabIndex = _vocab.GetMoveActionIndex(firstMove.Id),
                ChoiceType = ChoiceType.Move,
                MoveId = firstMove.Id,
                TargetLoc = GetTargetLocation(firstMove.Move.Target),
            });
        }

        if (actions.Count == 0)
        {
            actions.Add(new LegalAction { VocabIndex = Vocab.NoneActionIndex, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None });
        }

        return actions;
    }

    private LegalActionSet GetLegalActionsForSwitch(SwitchRequest request, BattlePerspective perspective)
    {
        // Determine which slots need to switch
        bool slotANeeds = request.ForceSwitch.Count > 0 && request.ForceSwitch[0];
        bool slotBNeeds = request.ForceSwitch.Count > 1 && request.ForceSwitch[1];

        var slotAActions = new List<LegalAction>();
        var slotBActions = new List<LegalAction>();

        if (slotANeeds)
        {
            AddSwitchActions(slotAActions, request.Side, perspective);
            if (slotAActions.Count == 0)
                slotAActions.Add(new LegalAction { VocabIndex = Vocab.NoneActionIndex, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None });
        }

        if (slotBNeeds)
        {
            AddSwitchActions(slotBActions, request.Side, perspective);
            if (slotBActions.Count == 0)
                slotBActions.Add(new LegalAction { VocabIndex = Vocab.NoneActionIndex, ChoiceType = ChoiceType.Pass, MoveId = MoveId.None });
        }

        return new LegalActionSet { SlotA = slotAActions, SlotB = slotBActions };
    }

    private void AddSwitchActions(List<LegalAction> actions, SideRequestData sideData, BattlePerspective perspective)
    {
        for (var i = 0; i < sideData.Pokemon.Count; i++)
        {
            PokemonSwitchRequestData pokemon = sideData.Pokemon[i];
            if (pokemon.Active || pokemon.Condition == ConditionId.Fainted || pokemon.Reviving)
                continue;

            // Get species from the perspective for accurate vocab lookup
            SpecieId specieId = perspective.PlayerSide.Pokemon[i].Species;
            int switchVocabIdx = _vocab.GetSwitchActionIndex(specieId);

            actions.Add(new LegalAction
            {
                VocabIndex = switchVocabIdx,
                ChoiceType = ChoiceType.Switch,
                MoveId = MoveId.None,
                SwitchIndex = i,
            });
        }
    }

    /// <summary>
    /// Build a boolean mask over the action vocabulary for one slot's legal actions.
    /// </summary>
    public bool[] BuildLegalMask(IReadOnlyList<LegalAction> actions)
    {
        var mask = new bool[_vocab.NumActions];
        for (var i = 0; i < actions.Count; i++)
        {
            mask[actions[i].VocabIndex] = true;
        }
        return mask;
    }

    /// <summary>
    /// Convert selected actions back into a Choice for the battle engine.
    /// </summary>
    public Choice BuildChoice(LegalAction slotA, LegalAction? slotB)
    {
        var actions = new List<ChosenAction>();

        actions.Add(BuildChosenAction(slotA));

        if (slotB.HasValue)
        {
            actions.Add(BuildChosenAction(slotB.Value));
        }

        return new Choice { Actions = actions };
    }

    private static ChosenAction BuildChosenAction(LegalAction action)
    {
        return action.ChoiceType switch
        {
            ChoiceType.Move => new ChosenAction
            {
                Choice = ChoiceType.Move,
                MoveId = action.MoveId,
                TargetLoc = action.TargetLoc,
                Terastallize = action.Terastallize,
            },
            ChoiceType.Switch => new ChosenAction
            {
                Choice = ChoiceType.Switch,
                MoveId = MoveId.None,
                Index = action.SwitchIndex,
            },
            _ => new ChosenAction
            {
                Choice = ChoiceType.Pass,
                MoveId = MoveId.None,
            },
        };
    }

    private int GetTargetLocation(MoveTarget targetType)
    {
        return targetType switch
        {
            MoveTarget.Normal or MoveTarget.Any or MoveTarget.AdjacentFoe
                => _rng.Next(1, 3), // Random target: 1 or 2
            MoveTarget.AdjacentAlly
                => -_rng.Next(1, 3),
            MoveTarget.AdjacentAllyOrSelf
                => -_rng.Next(1, 3),
            _ => 0, // Auto-targeting
        };
    }

    private static bool IsDisabled(MoveIdBoolUnion? disabled)
    {
        return disabled is not null && disabled.IsTrue();
    }
}
