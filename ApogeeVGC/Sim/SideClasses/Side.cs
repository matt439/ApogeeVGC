using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using System.Text.Json.Nodes;
using static ApogeeVGC.Sim.PokemonClasses.Pokemon;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC.Sim.SideClasses;

public class Side
{
    public IBattle Battle { get; }
    public SideId Id { get; }
    public int N { get; set; }

    public string Name { get; set; }
    public string Avatar { get; set; }

    // Only used in multi battles so not implemented in this program
    // public Side? AllySide { get; init; } = null;
    public List<PokemonSet> Team { get; set; }
    public List<Pokemon> Pokemon { get; set; }
    public List<Pokemon> Active { get; set; }
    public Side Foe
    {
        get
        {
            if (field is null)
            {
                throw new InvalidOperationException("Foe side not set yet");
            }
            return field;
        }
        set;
    } = null!; // set in battle.start()

    public int PokemonLeft { get; set; }

    public Pokemon? FaintedLastTurn { get; set; }
    public Pokemon? FaintedThisTurn { get; set; }
    public int TotalFainted { get; set; }

    public Dictionary<ConditionId, EffectState> SideConditions { get; set; }
    public List<Dictionary<ConditionId, EffectState>> SlotConditions { get; set; }

    public IChoiceRequest? ActiveRequest { get; set; }
    public Choice Choice { get; set; }
    public bool Initialised { get; init; }

    public RequestState RequestState { get; set; }

    public Side(string name, IBattle battle, SideId sideNum, PokemonSet[] team)
    {
        // Copy side scripts from battle if needed

        Battle = battle;
        Id = sideNum;
        //N = sideNum;

        Name = name;
        Avatar = string.Empty;

        Team = team.ToList();
        Pokemon = [];
        foreach (PokemonSet set in Team)
        {
            AddPokemon(set);
        }

        Active = battle.GameType switch
        {
            GameType.Doubles => [null!, null!],
            _ => [null!],
        };

        PokemonLeft = Pokemon.Count;

        SideConditions = [];
        SlotConditions = [];

        // Initialize slot conditions for each active slot
        for (int i = 0; i < Active.Count; i++)
        {
            SlotConditions.Add(new Dictionary<ConditionId, EffectState>());
        }

        Choice = new Choice
        {
            CantUndo = false,
            Actions = [],
            ForcedSwitchesLeft = 0,
            ForcedPassesLeft = 0,
            SwitchIns = [],
            Terastallize = false,
        };
        Initialised = true;
    }

    public Side(IBattle battle)
    {
        Battle = battle;
        Id = SideId.P1;
        Name = string.Empty;
        Avatar = string.Empty;
        Team = [];
        Pokemon = [];
        Active = [];
        PokemonLeft = 0;
        SideConditions = [];
        SlotConditions = [];
        Choice = new Choice
        {
            CantUndo = false,
            Actions = [],
            ForcedSwitchesLeft = 0,
            ForcedPassesLeft = 0,
            SwitchIns = [],
            Terastallize = false,
        };
        Initialised = false;
    }

    public JsonObject ToJson()
    {
        throw new NotImplementedException();
    }

    private Pokemon? AddPokemon(PokemonSet set)
    {
        if (Pokemon.Count >= 24) return null;
        var newPokemon = new Pokemon(Battle, set, this)
        {
            Position = Pokemon.Count,
        };
        Pokemon.Add(newPokemon);
        PokemonLeft++;
        return newPokemon;
    }

    // CanDynamaxNow() // not implemented

    public Choice GetChoice()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        throw new NotImplementedException();
    }

    public SideRequestData GetRequestData(bool forAlly = false)
    {
        SideRequestData data = new()
        {
            Name = Name,
            Id = Id,
            Pokemon = Pokemon.Select(p => p.GetSwitchRequestData(forAlly)).ToList(),
        };
        return data;
    }

    public Pokemon? RandomFoe()
    {
        var actives = Foes();
        return actives.Count == 0 ? null : Battle.Sample(actives);
    }

    public List<Side> FoeSidesWithConditions()
    {
        return [Foe];
    }

    public int FoePokemonLeft()
    {
        return Foe.PokemonLeft;
    }

    public List<Pokemon> Allies(bool all = false)
    {
        var allies = Active.Where(_ => true).ToList();
        if (!all) allies = allies.Where(ally => ally.Hp > 0).ToList();
        return allies;
    }

    public List<Pokemon> Foes(bool all = false)
    {
        return Foe.Allies(all);
    }

    public List<Pokemon> ActiveTeam()
    {
        return Battle.Sides[N % 2].Active.Concat(Battle.Sides[N % 2 + 2].Active).ToList();
    }

    public bool HasAlly(Pokemon pokemon)
    {
        return pokemon.Side == this;
    }

    public bool AddSideCondition(ConditionId status, Pokemon? source = null, IEffect? sourceEffect = null)
    {
        Condition condition = Battle.Library.Conditions[status];
        return AddSideCondition(condition, source, sourceEffect);
    }

    public bool AddSideCondition(Condition status, Pokemon? source = null, IEffect? sourceEffect = null)
    {
        // Step 1: Source resolution
        if (source == null && Battle is BattleAsync { Event.Target: PokemonSingleEventTarget eventTarget })
        {
            source = eventTarget.Pokemon;
        }
        if (source == null)
            throw new InvalidOperationException("Setting side condition without a source");

        // Step 3: Restart handling
        if (SideConditions.TryGetValue(status.Id, out EffectState? condition))
        {
            // If no onSideRestart handler, return false
            if (status.OnRestart == null)
                return false;

            // Call the restart handler
            RelayVar? restartResult = Battle.SingleEvent(EventId.SideRestart, status, condition, this, source, sourceEffect);
            return restartResult is BoolRelayVar { Value: true };
        }

        // Step 4: Create EffectState
        EffectState effectState = Battle.InitEffectState(status.Id, source, source.GetSlot(), status.Duration);

        // Step 5: Duration callback
        if (status.DurationCallback != null)
        {
            effectState.Duration = status.DurationCallback(Battle, Active[0], source, sourceEffect);
        }

        SideConditions[status.Id] = effectState;

        // Step 6: SideStart event
        RelayVar? sideStartResult = Battle.SingleEvent(EventId.SideStart, status, effectState, this, source, sourceEffect);
        if (sideStartResult is not BoolRelayVar { Value: true })
        {
            SideConditions.Remove(status.Id);
            return false;
        }

        // Step 7: Run SideConditionStart event
        Battle.RunEvent(EventId.SideConditionStart, this, source, status);

        // Step 8: Success
        return true;
    }

    public Condition? GetSideCondition(ConditionId status)
    {
        Condition condition = Battle.Library.Conditions[status];
        return !SideConditions.ContainsKey(condition.Id) ? null : condition;
    }

    public Condition? GetSideCondition(Condition status)
    {
        return !SideConditions.ContainsKey(status.Id) ? null : status;
    }

    public EffectState? GetSideConditionData(ConditionId status)
    {
        Condition condition = Battle.Library.Conditions[status];
        return SideConditions.GetValueOrDefault(condition.Id);
    }

    public EffectState? GetSideConditionData(Condition status)
    {
        return SideConditions.GetValueOrDefault(status.Id);
    }

    public bool RemoveSideCondition(ConditionId status)
    {
        Condition condition = Battle.Library.Conditions[status];
        if (!SideConditions.TryGetValue(condition.Id, out EffectState? sideCondition)) return false;
        Battle.SingleEvent(EventId.SideEnd, condition, sideCondition, this);
        SideConditions.Remove(condition.Id);
        return true;
    }

    public bool RemoveSideCondition(Condition status)
    {
        return RemoveSideCondition(status.Id);
    }

    public bool AddSlotCondition(PokemonIntUnion target, ConditionId status, Pokemon? source = null,
        IEffect? sourceEffect = null)
    {
        Condition condition = Battle.Library.Conditions[status];
        return AddSlotCondition(target, condition, source, sourceEffect);
    }

    public bool AddSlotCondition(PokemonIntUnion target, Condition status, Pokemon? source = null,
        IEffect? sourceEffect = null)
    {
        // Step 1: Source resolution
        if (source == null && Battle is BattleAsync { Event.Target: PokemonSingleEventTarget eventTarget })
        {
            source = eventTarget.Pokemon;
        }
        if (source == null)
            throw new InvalidOperationException("Setting slot condition without a source");

        // Step 2: Convert target to position if it's a Pokemon
        int targetSlot = target switch
        {
            PokemonPokemonIntUnion pokemon => pokemon.Pokemon.Position,
            IntPokemonIntUnion intValue => intValue.Value,
            _ => throw new InvalidOperationException("Invalid target type"),
        };

        // Step 3: Validate slot index
        if (targetSlot < 0 || targetSlot >= SlotConditions.Count)
        {
            throw new InvalidOperationException($"Invalid slot index: {targetSlot}");
        }

        // Step 4: Restart handling - if condition already exists
        if (SlotConditions[targetSlot].TryGetValue(status.Id, out EffectState? condition))
        {
            // If no onRestart handler, return false
            if (status.OnRestart == null)
                return false;

            // Call the restart handler
            RelayVar? restartResult = Battle.SingleEvent(EventId.Restart, status, condition, this, source, sourceEffect);
            return restartResult is BoolRelayVar { Value: true };
        }

        // Step 5: Create EffectState
        EffectState conditionState = Battle.InitEffectState(status.Id, source, source.GetSlot(), status.Duration);
        conditionState.IsSlotCondition = true;

        // Step 6: Duration callback
        if (status.DurationCallback != null)
        {
            conditionState.Duration = status.DurationCallback(Battle, Active[0], source, sourceEffect);
        }

        SlotConditions[targetSlot][status.Id] = conditionState;

        // Step 7: Start event
        RelayVar? startResult = Battle.SingleEvent(EventId.Start, status, conditionState,
            Active[targetSlot], source, sourceEffect);

        if (startResult is BoolRelayVar { Value: true }) return true;
        SlotConditions[targetSlot].Remove(status.Id);
        return false;
    }

    public IEffect? GetSlotCondition(PokemonIntUnion target, Condition status)
    {
        // Convert target to position if it's a Pokemon
        int targetSlot = target switch
        {
            PokemonPokemonIntUnion pokemon => pokemon.Pokemon.Position,
            IntPokemonIntUnion intValue => intValue.Value,
            _ => throw new InvalidOperationException("Invalid target type"),
        };

        // Validate slot index
        if (targetSlot < 0 || targetSlot >= SlotConditions.Count)
        {
            throw new InvalidOperationException($"Invalid slot index: {targetSlot}");
        }

        // Check if condition exists in the slot
        if (!SlotConditions[targetSlot].ContainsKey(status.Id))
            return null;

        return status;
    }

    public IEffect? GetSlotCondition(PokemonIntUnion target, ConditionId status)
    {
        Condition condition = Battle.Library.Conditions[status];
        return GetSlotCondition(target, condition);
    }

    public bool RemoveSlotCondition(PokemonIntUnion target, Condition status)
    {
        // Convert target to position if it's a Pokemon
        int targetSlot = target switch
        {
            PokemonPokemonIntUnion pokemon => pokemon.Pokemon.Position,
            IntPokemonIntUnion intValue => intValue.Value,
            _ => throw new InvalidOperationException("Invalid target type"),
        };

        // Validate slot index
        if (targetSlot < 0 || targetSlot >= SlotConditions.Count)
        {
            throw new InvalidOperationException($"Invalid slot index: {targetSlot}");
        }

        // Check if condition exists in the slot
        if (!SlotConditions[targetSlot].TryGetValue(status.Id, out EffectState? conditionState))
            return false;

        // Trigger End event
        Battle.SingleEvent(EventId.End, status, conditionState, Active[targetSlot]);

        // Remove the condition
        SlotConditions[targetSlot].Remove(status.Id);

        return true;
    }

    public bool RemoveSlotCondition(PokemonIntUnion target, ConditionId status)
    {
        Condition condition = Battle.Library.Conditions[status];
        return RemoveSlotCondition(target, condition);
    }

    public void Send(params object[] parts)
    {
        string sideUpdate = "|" + string.Join("|", parts.Select(part =>
        {
            if (part is Func<Side, object> func)
                return func(this).ToString() ?? string.Empty;
            return part.ToString() ?? string.Empty;
        }));

        Battle.Send(SendType.SideUpdate, [$"{Id}\n{sideUpdate}"]);
    }

    public void EmitRequest(IChoiceRequest? update = null, bool updatedRequest = false)
    {
        update ??= ActiveRequest;

        if (updatedRequest && update is MoveRequest moveRequest)
        {
            moveRequest.Update = true;
        }
        else if (updatedRequest && update is SwitchRequest switchRequest)
        {
            switchRequest.Update = true;
        }

        string json = JsonSerializer.Serialize(update);
        Battle.Send(SendType.SideUpdate, [$"{Id}\n|request|{json}"]);
        ActiveRequest = update;
    }

    public bool EmitChoiceError(string message,
        (Pokemon pokemon, Func<PokemonMoveRequestData, BoolVoidUnion> update)? updateInfo = null)
    {
        Choice.Error = message;

        bool? updated = updateInfo.HasValue
            ? UpdateRequestForPokemon(updateInfo.Value.pokemon, updateInfo.Value.update)
            : null;

        string type = updated == true ? "[Unavailable choice]" : "[Invalid choice]";
        Battle.Send(SendType.SideUpdate, [$"{Id}\n|error|{type} {message}"]);

        if (updated == true)
        {
            EmitRequest(ActiveRequest, true);
        }

        if (Battle.StrictChoices)
        {
            throw new InvalidOperationException($"{type} {message}");
        }

        return false;
    }

    public bool IsChoiceDone()
    {
        if (RequestState == RequestState.None) return true;
        if (Choice.ForcedSwitchesLeft > 0) return false;

        if (RequestState == RequestState.TeamPreview)
        {
            return Choice.Actions.Count >= PickedTeamSize();
        }

        GetChoiceIndex();
        return Choice.Actions.Count >= Active.Count;
    }

    public bool ChooseMove(MoveIdIntUnion? moveText = null, int targetLoc = 0, EventType eventType = EventType.None)
    {
        // Step 1: Validate request state
        if (RequestState != RequestState.Move)
        {
            return EmitChoiceError($"Can't move: You need a {RequestState} response");
        }

        // Step 2: Get the active pokemon index
        int index = GetChoiceIndex();
        if (index >= Active.Count)
        {
            return EmitChoiceError("Can't move: You sent more choices than unfainted Pokémon.");
        }

        // Step 3: Determine auto-choose and get pokemon
        bool autoChoose = moveText == null;
        Pokemon pokemon = Active[index];

        // Step 4: Parse moveText (name or index)
        PokemonMoveRequestData request = pokemon.GetMoveRequestData();
        var moveid = MoveId.None;

        if (autoChoose) moveText = 1; // Default to first move

        // Handle the union type: either MoveId or int
        switch (moveText)
        {
            case IntMoveIdIntUnion intUnion:
            {
                // Parse a one-based move index
                int moveIndex = intUnion.Value - 1;
                if (moveIndex < 0 || moveIndex >= request.Moves.Count)
                {
                    return EmitChoiceError($"Can't move: Your {pokemon.Name} doesn't have a move {intUnion.Value}");
                }
                moveid = request.Moves[moveIndex].Id;
                break;
            }
            case MoveIdMoveIdIntUnion moveIdUnion:
            {
                // Parse a move ID directly
                moveid = moveIdUnion.MoveId;
            
                // Find the move in the request
                bool found = request.Moves.Any(pokemonMoveData => pokemonMoveData.Id == moveid);

                if (!found)
                {
                    return EmitChoiceError($"Can't move: Your {pokemon.Name} doesn't have a move matching {moveid}");
                }
                break;
            }
        }

        // Step 5: Get available moves
        var moves = pokemon.GetMoves();
        
        // Step 6: Auto-choose first available move if needed
        if (autoChoose)
        {
            foreach (PokemonMoveData pokemonMoveData in request.Moves)
            {
                if (pokemonMoveData.Disabled is MoveIdMoveIdBoolUnion or BoolMoveIdBoolUnion { Value: true })
                {
                    continue;
                }

                moveid = pokemonMoveData.Id;
                break;
            }
        }

        Move move = Battle.Library.Moves[moveid];

        // Step 7: Validate targeting
        if (autoChoose)
        {
            targetLoc = 0;
        }
        else if (Battle.Actions.TargetTypeChoices(move.Target))
        {
            if (targetLoc == 0 && Active.Count >= 2)
            {
                return EmitChoiceError($"Can't move: {move.Name} needs a target");
            }
            if (!Battle.ValidTargetLoc(targetLoc, pokemon, move.Target))
            {
                return EmitChoiceError($"Can't move: Invalid target for {move.Name}");
            }
        }
        else
        {
            if (targetLoc != 0)
            {
                return EmitChoiceError($"Can't move: You can't choose a target for {move.Name}");
            }
        }

        // Step 8: Handle locked moves (multi-turn moves like Outrage)
        var lockedMove = pokemon.GetLockedMove();
        if (lockedMove != null)
        {
            int lockedMoveTargetLoc = pokemon.LastMoveTargetLoc ?? 0;
            
            // Note: In the original TS code, it checks pokemon.volatiles[lockedMoveID]?.targetLoc
            // but EffectState doesn't have a targetLoc property in our C# implementation
            // This would need to be added to EffectState if this functionality is needed

            if (pokemon.MaybeLocked ?? false) Choice.CantUndo = true;

            Choice.Actions = [.. Choice.Actions, new ChosenAction
            {
                Choice = ChoiceType.Move,
                Pokemon = pokemon,
                TargetLoc = lockedMoveTargetLoc,
                MoveId = lockedMove.Value,
            }];

            return true;
        }

        // Step 9: Handle Struggle when no moves have PP
        if (moves.Count == 0)
        {
            // Gen 4 and earlier announce Pokemon has no moves left
            if (Battle.Gen <= 4)
            {
                Send("-activate", pokemon, "move: Struggle");
            }

            if (pokemon.MaybeLocked ?? false) Choice.CantUndo = true;

            Choice.Actions = [.. Choice.Actions, new ChosenAction
            {
                Choice = ChoiceType.Move,
                Pokemon = pokemon,
                MoveId = MoveId.Struggle,
            }];

            return true;
        }

        // Step 10: Check for disabled moves
        bool isEnabled = false;
        string disabledSource = string.Empty;

        foreach (PokemonMoveData m in moves.Where(m => m.Id == moveid))
        {
            if (m.Disabled is null or BoolMoveIdBoolUnion { Value: false })
            {
                isEnabled = true;
                break;
            }
            else if (m.DisabledSource != null)
            {
                disabledSource = m.DisabledSource.Name;
            }
        }

        if (!isEnabled)
        {
            if (autoChoose)
            {
                throw new InvalidOperationException("autoChoose chose a disabled move");
            }

            return EmitChoiceError(
                $"Can't move: {pokemon.Name}'s {move.Name} is disabled",
                (pokemon, req =>
                    UpdateDisabledRequestForMove(pokemon, req, moveid, disabledSource))
            );
        }

        // Step 11: Terastallization (Gen 9 only)
        bool terastallize = eventType == EventType.Terastallize;

        if (terastallize && request.CanTerastallize is null or FalseMoveTypeFalseUnion)
        {
            return EmitChoiceError($"Can't move: {pokemon.Name} can't Terastallize.");
        }

        if (terastallize && Choice.Terastallize)
        {
            return EmitChoiceError("Can't move: You can only Terastallize once per battle.");
        }

        if (terastallize && Battle.Gen != 9)
        {
            return EmitChoiceError("Can't move: You can only Terastallize in Gen 9.");
        }

        // Step 12: Add action to choice
        Choice.Actions = [.. Choice.Actions, new ChosenAction
        {
            Choice = ChoiceType.Move,
            Pokemon = pokemon,
            TargetLoc = targetLoc,
            MoveId = moveid,
            Terastallize = terastallize ? pokemon.TeraType : null,
        }];

        // Step 13: Handle maybeDisabled flag
        if ((pokemon.MaybeDisabled) && 
            (Battle.GameType == GameType.Singles || 
             (Battle.Gen <= 3 && !Battle.Actions.TargetTypeChoices(move.Target))))
        {
            Choice.CantUndo = true;
        }

        // Step 14: Update choice flags
        if (terastallize)
        {
            Choice.Terastallize = true;
        }

        return true;
    }

    private BoolVoidUnion UpdateDisabledRequestForMove(Pokemon pokemon, PokemonMoveRequestData req, 
        MoveId moveid, string disabledSource)
    {
        bool updated = UpdateDisabledRequest(pokemon, req);

        foreach (PokemonMoveData m in req.Moves)
        {
            if (m.Id != moveid) continue;

            // Check if we need to update the disabled state
            bool needsUpdate = m.Disabled is null or BoolMoveIdBoolUnion { Value: false } ||
                               m.DisabledSource?.Name != disabledSource;

            if (needsUpdate)
            {
                updated = true;
            }
            break;
        }

        return BoolVoidUnion.FromBool(updated);
    }

    public bool UpdateDisabledRequest(Pokemon pokemon, PokemonMoveRequestData req)
    {
        bool updated = false;

        // Clear maybeLocked if it's set
        if (pokemon.MaybeLocked ?? false)
        {
            pokemon.MaybeLocked = false;
            req.MaybeLocked = null;
            updated = true;
        }

        // Handle maybeDisabled in non-singles formats
        if (pokemon.MaybeDisabled && Battle.GameType != GameType.Singles)
        {
            // Gen 4+ behavior
            if (Battle.Gen >= 4)
            {
                pokemon.MaybeDisabled = false;
                req.MaybeDisabled = null;
                updated = true;
            }

            // Update individual move disabled states
            foreach (PokemonMoveData m in req.Moves)
            {
                MoveSlot? moveData = pokemon.GetMoveData(m.Id);
                BoolHiddenUnion? disabled = moveData?.Disabled;

                // Check if move should be marked as disabled
                if (disabled != null &&
                    (Battle.Gen >= 4 || Battle.Actions.TargetTypeChoices(m.Target ?? MoveTarget.None)))
                {
                    m.Disabled = true;
                    updated = true;
                }
            }
        }

        // If all moves are disabled or only Struggle is available
        bool allMovesDisabled = req.Moves.All(m =>
            m.Disabled is BoolMoveIdBoolUnion { Value: true } ||
            m.Id == MoveId.Struggle);

        if (allMovesDisabled)
        {
            // Disable Terastallization (Gen 9 mechanic)
            if (req.CanTerastallize is not null and not FalseMoveTypeFalseUnion)
            {
                req.CanTerastallize = null;
                updated = true;
            }
        }

        return updated;
    }

    public bool UpdateRequestForPokemon(Pokemon pokemon, Func<PokemonMoveRequestData, BoolVoidUnion> update)
    {
        // Ensure we have an active request with Pokemon data
        if (ActiveRequest is not MoveRequest moveRequest || moveRequest.Active == null)
        {
            throw new InvalidOperationException("Can't update a request without active Pokemon");
        }

        // Find the Pokemon in the request
        PokemonMoveRequestData? req = moveRequest.Active[pokemon.Position];
        if (req == null)
        {
            throw new InvalidOperationException("Pokemon not found in request's active field");
        }

        // Apply the update function
        BoolVoidUnion result = update(req);

        // Return true if the update function returned true, otherwise default to true
        return result switch
        {
            BoolBoolVoidUnion { Value: var b } => b,
            _ => true,
        };
    }

    public SideBoolUnion ChooseSwitch(PokemonIntUnion? slotText = null)
    {
        // Step 1: Validate request state
        if (RequestState != RequestState.Move && RequestState != RequestState.Switch)
        {
            return EmitChoiceError($"Can't switch: You need a {RequestState} response");
        }

        // Step 2: Get the active pokemon index
        int index = GetChoiceIndex();
        if (index >= Active.Count)
        {
            if (RequestState == RequestState.Switch)
            {
                return EmitChoiceError("Can't switch: You sent more switches than Pokémon that need to switch");
            }
            return EmitChoiceError("Can't switch: You sent more choices than unfainted Pokémon");
        }

        // Step 3: Get the currently active pokemon
        Pokemon pokemon = Active[index];
        int slot;

        // Step 4: Determine the target slot
        if (slotText == null)
        {
            // Auto-select mode
            if (RequestState != RequestState.Switch)
            {
                return EmitChoiceError("Can't switch: You need to select a Pokémon to switch in");
            }

            // Check for Revival Blessing slot condition
            if (SlotConditions[pokemon.Position].ContainsKey(ConditionId.RevivalBlessing))
            {
                // Find first fainted Pokemon
                slot = 0;
                while (slot < Pokemon.Count && !Pokemon[slot].Fainted)
                {
                    slot++;
                }
            }
            else
            {
                // Normal forced switch - auto-select first available
                if (Choice.ForcedSwitchesLeft <= 0)
                {
                    return ChoosePass();
                }

                slot = Active.Count;
                while (slot < Pokemon.Count &&
                       (Choice.SwitchIns.Contains(slot) || Pokemon[slot].Fainted))
                {
                    slot++;
                }
            }
        }
        else
        {
            // Parse the slot from the union type
            slot = slotText switch
            {
                IntPokemonIntUnion intUnion => intUnion.Value - 1, // Convert from 1-based to 0-based
                PokemonPokemonIntUnion pokemonUnion => pokemonUnion.Pokemon.Position,
                _ => -1, // Invalid - will trigger error below
            };

            // If slot is still invalid after parsing, return error
            if (slot < 0)
            {
                return EmitChoiceError($"Can't switch: Invalid switch target \"{slotText}\"");
            }
        }

        // Step 5: Validate slot index
        if (slot >= Pokemon.Count)
        {
            return EmitChoiceError($"Can't switch: You do not have a Pokémon in slot {slot + 1} to switch to");
        }
        else if (slot < Active.Count &&
                 !SlotConditions[pokemon.Position].ContainsKey(ConditionId.RevivalBlessing))
        {
            return EmitChoiceError("Can't switch: You can't switch to an active Pokémon");
        }
        else if (Choice.SwitchIns.Contains(slot))
        {
            return EmitChoiceError($"Can't switch: The Pokémon in slot {slot + 1} can only switch in once");
        }

        // Step 6: Get target Pokemon
        Pokemon targetPokemon = Pokemon[slot];

        // Step 7: Handle Revival Blessing special case
        if (SlotConditions[pokemon.Position].ContainsKey(ConditionId.RevivalBlessing))
        {
            if (!targetPokemon.Fainted)
            {
                return EmitChoiceError("Can't switch: You have to pass to a fainted Pokémon");
            }

            // Decrement forced switches (clamp to prevent negative)
            Choice.ForcedSwitchesLeft = Math.Max(0, Choice.ForcedSwitchesLeft - 1);
            pokemon.SwitchFlag = false;

            Choice.Actions = [.. Choice.Actions, new ChosenAction
            {
                MoveId = MoveId.RevivalBlessing,
                Choice = ChoiceType.RevivalBlessing,
                Pokemon = pokemon,
                Target = targetPokemon,
            }];

            return true;
        }

        // Step 8: Validate target is not fainted (for normal switches)
        if (targetPokemon.Fainted)
        {
            return EmitChoiceError("Can't switch: You can't switch to a fainted Pokémon");
        }

        // Step 9: Handle move phase switching (check for trapped)
        if (RequestState == RequestState.Move)
        {
            if (pokemon.Trapped == PokemonTrapped.True)
            {
                return EmitChoiceError(
                    "Can't switch: The active Pokémon is trapped",
                    (pokemon, req =>
                    {
                        bool updated = false;

                        if (req.MaybeTrapped != null)
                        {
                            req.MaybeTrapped = null;
                            updated = true;
                        }

                        if (req.Trapped != true)
                        {
                            req.Trapped = true;
                            updated = true;
                        }

                        return BoolVoidUnion.FromBool(updated);
                    })
                );
            }
            if (pokemon.MaybeTrapped)
            {
                Choice.CantUndo = true;
            }
        }
        else if (RequestState == RequestState.Switch)
        {
            // Step 10: Handle forced switches
            if (Choice.ForcedSwitchesLeft <= 0)
            {
                throw new InvalidOperationException("Player somehow switched too many Pokemon");
            }
            Choice.ForcedSwitchesLeft--;
        }

        // Step 11: Record the switch
        Choice.SwitchIns.Add(slot);

        ChoiceType choiceType = RequestState == RequestState.Switch
            ? ChoiceType.InstaSwitch
            : ChoiceType.Switch;

        Choice.Actions = [.. Choice.Actions, new ChosenAction
        {
            MoveId = MoveId.None,
            Choice = choiceType,
            Pokemon = pokemon,
            Target = targetPokemon,
        }];

        return true;
    }

    /// <summary>
    /// The number of pokemon you must choose in Team Preview.
    /// 
    /// Note that PS doesn't support choosing fewer than this number of pokemon.
    /// In the games, it is sometimes possible to bring fewer than this, but
    /// since that's nearly always a mistake, we haven't gotten around to
    /// supporting it.
    /// </summary>
    public int PickedTeamSize()
    {
        int pokemonLength = Pokemon.Count;
        int ruleTableSize = Battle.RuleTable.PickedTeamSize ?? int.MaxValue;
        return Math.Min(pokemonLength, ruleTableSize);
    }

    public bool ChooseTeam(string data = "")
    {
        // Step 1: Validate request state
        if (RequestState != RequestState.TeamPreview)
        {
            return EmitChoiceError("Can't choose for Team Preview: You're not in a Team Preview phase");
        }

        // Step 2: Parse positions from input
        List<int> positions;
        if (string.IsNullOrWhiteSpace(data))
        {
            positions = [];
        }
        else
        {
            char separator = data.Contains(',') ? ',' : ' ';
            positions = data.Split(separator)
                .Select(s => int.TryParse(s.Trim(), out int val) ? val - 1 : -1) // Convert to 0-based
                .ToList();
        }

        int pickedTeamSize = PickedTeamSize();

        // Step 3: Auto-fill positions if needed
        if (positions.Count == 0)
        {
            // No input - use all Pokémon in order
            for (int i = 0; i < pickedTeamSize; i++)
            {
                positions.Add(i);
            }
        }
        else if (positions.Count < pickedTeamSize)
        {
            // Partial input - fill remaining slots with unused Pokémon
            for (int i = 0; i < pickedTeamSize && positions.Count < pickedTeamSize; i++)
            {
                if (!positions.Contains(i))
                {
                    positions.Add(i);
                }
            }
        }
        else if (positions.Count > pickedTeamSize)
        {
            // Too many positions - trim to size
            positions = positions.Take(pickedTeamSize).ToList();
        }

        // Step 4: Validate positions
        for (int index = 0; index < positions.Count; index++)
        {
            int pos = positions[index];

            // Check if position is valid
            if (pos < 0 || pos >= Pokemon.Count)
            {
                return EmitChoiceError($"Can't choose for Team Preview: You do not have a Pokémon in slot {pos + 1}");
            }

            // Check for duplicates
            if (positions.IndexOf(pos) != index)
            {
                return EmitChoiceError($"Can't choose for Team Preview: The Pokémon in slot {pos + 1} can only switch in once");
            }
        }

        // Step 5: Create team actions
        for (int index = 0; index < positions.Count; index++)
        {
            int pos = positions[index];
            Choice.SwitchIns.Add(pos);
            Choice.Actions = [.. Choice.Actions, new ChosenAction
            {
                MoveId = MoveId.None,
                Choice = ChoiceType.Team,
                Index = index,
                Pokemon = Pokemon[pos],
                Priority = -index, // Earlier picks have higher priority
            }];
        }

        return true;
    }

    public bool ChooseShift()
    {
        throw new NotImplementedException("This is only used in Triple Battles which are not yet implemented.");
    }

    public void ClearChoice()
    {
        int forcedSwitches = 0;
        int forcedPasses = 0;

        // Calculate forced switches if we're in switch request state
        if (Battle.RequestState == RequestState.Switch)
        {
            // Count active Pokemon that need to switch out
            int canSwitchOut = Active.Count(pokemon => pokemon.SwitchFlag.IsTrue());

            // Count bench Pokemon available to switch in (not active, not fainted)
            int canSwitchIn = Pokemon
                .Skip(Active.Count) // Skip active slots
                .Count(pokemon => pokemon is { Fainted: false });

            // Can only force as many switches as we have Pokemon to switch in
            forcedSwitches = Math.Min(canSwitchOut, canSwitchIn);

            // Any switches we can't fulfill become forced passes
            forcedPasses = canSwitchOut - forcedSwitches;
        }

        // Reset choice to default state
        Choice = new Choice
        {
            CantUndo = false,
            Error = string.Empty,
            Actions = [],
            ForcedSwitchesLeft = forcedSwitches,
            ForcedPassesLeft = forcedPasses,
            SwitchIns = [],
            Terastallize = false,
        };
    }

    //    choose(input: string)
    //    {
    //        if (!this.requestState)
    //        {
    //            return this.emitChoiceError(
    //                this.battle.ended ? `Can't do anything: The game is over` : `Can't do anything: It's not your turn`
    //            );
    //        }

    //        if (this.choice.cantUndo)
    //        {
    //            return this.emitChoiceError(`Can't undo: A trapping/disabling effect would cause undo to leak information`);

    //        }

    //        this.clearChoice();

    //        const choiceStrings = (input.startsWith('team ') ? [input] : input.split(','));

    //        if (choiceStrings.length > this.active.length)
    //        {
    //            return this.emitChoiceError(
    //				`Can't make choices: You sent choices for ${choiceStrings.length} Pokémon, but this is a ${this.battle.gameType} game!`
    //            );
    //        }

    //        for (const choiceString of choiceStrings) {
    //            let[choiceType, data] = Utils.splitFirst(choiceString.trim(), ' ');
    //            data = data.trim();
    //            if (choiceType === 'testfight')
    //            {
    //                choiceType = 'move';
    //                data = 'testfight';
    //            }

    //            switch (choiceType)
    //            {
    //                case 'move':
    //                    const original = data;
    //                    const error = () => this.emitChoiceError(`Conflicting arguments for "move": ${ original}`);
    //                    let targetLoc: number | undefined;
    //                    let event: 'mega' | 'megax' | 'megay' | 'zmove' | 'ultra' | 'dynamax' | 'terastallize' | '' = '';
    //				while (true) {
    //					// If data ends with a number, treat it as a target location.
    //					// We need to special case 'Conversion 2' so it doesn't get
    //					// confused with 'Conversion' erroneously sent with the target
    //					// '2' (since Conversion targets 'self', targetLoc can't be 2).
    //					if (/\s(?:-|\+)?[1 - 3]$/.test(data) && toID(data) !== 'conversion2') {
    //						if (targetLoc !== undefined) return error();
    //    targetLoc = parseInt(data.slice(-2));
    //						data = data.slice(0, -2).trim();
    //} else if (data.endsWith(' mega')) {
    //						if (event) return error();
    //						event = 'mega';
    //data = data.slice(0, -5);
    //} else if (data.endsWith(' megax')) {
    //						if (event) return error();
    //						event = 'megax';
    //data = data.slice(0, -6);
    //} else if (data.endsWith(' megay')) {
    //						if (event) return error();
    //						event = 'megay';
    //data = data.slice(0, -6);
    //} else if (data.endsWith(' zmove')) {
    //						if (event) return error();
    //						event = 'zmove';
    //data = data.slice(0, -6);
    //} else if (data.endsWith(' ultra')) {
    //						if (event) return error();
    //						event = 'ultra';
    //data = data.slice(0, -6);
    //} else if (data.endsWith(' dynamax')) {
    //						if (event) return error();
    //						event = 'dynamax';
    //data = data.slice(0, -8);
    //} else if (data.endsWith(' gigantamax')) {
    //						if (event) return error();
    //						event = 'dynamax';
    //data = data.slice(0, -11);
    //} else if (data.endsWith(' max')) {
    //						if (event) return error();
    //						event = 'dynamax';
    //data = data.slice(0, -4);
    //} else if (data.endsWith(' terastal')) {
    //						if (event) return error();
    //						event = 'terastallize';
    //data = data.slice(0, -9);
    //} else if (data.endsWith(' terastallize')) {
    //						if (event) return error();
    //						event = 'terastallize';
    //data = data.slice(0, -13);
    //} else {
    //						break;
    //					}
    //				}
    //				if (!this.chooseMove(data, targetLoc, event)) return false;
    //break;

    //            case 'switch':
    //    this.chooseSwitch(data);
    //    break;
    //case 'shift':
    //    if (data) return this.emitChoiceError(`Unrecognized data after "shift": ${ data}`);
    //    if (!this.chooseShift()) return false;
    //    break;
    //case 'team':
    //    if (!this.chooseTeam(data)) return false;
    //    break;
    //case 'pass':
    //case 'skip':
    //    if (data) return this.emitChoiceError(`Unrecognized data after "pass": ${ data}`);
    //    if (!this.choosePass()) return false;
    //    break;
    //case 'auto':
    //case 'default':
    //    this.autoChoose();
    //    break;
    //default:
    //    this.emitChoiceError(`Unrecognized choice: ${ choiceString}`);
    //    break;
    //}
    //		}

    //		return !this.choice.error;
    //	}

    public bool Choose(object input)
    {
        throw new NotImplementedException();
    }

    public int GetChoiceIndex(bool isPass = false)
    {
        throw new NotImplementedException();
    }

    public SideBoolUnion ChoosePass()
    {
        throw new NotImplementedException();
    }

    public bool AutoChoose()
    {
        throw new NotImplementedException();
    }

    public void Destroy()
    {
        throw new NotImplementedException();
    }
}