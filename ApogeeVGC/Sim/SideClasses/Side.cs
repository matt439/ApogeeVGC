using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;
using System.Text.Json;
using System.Text.Json.Nodes;

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

    //send(...parts: (string | number | Function | AnyObject)[]) {
    //    const sideUpdate = '|' + parts.map(part => {
    //        if (typeof part !== 'function') return part;
    //        return part(this);
    //    }).join('|');
    //    this.battle.send('sideupdate', `${this.id
    //    }\n${sideUpdate}`);
    //}

    //emitRequest(update: ChoiceRequest = this.activeRequest!, updatedRequest = false) {
    //    if (updatedRequest) (this.activeRequest as MoveRequest | SwitchRequest).update = true;
    //    this.battle.send('sideupdate', `${ this.id}\n | request |${ JSON.stringify(update)}`);
    //    this.activeRequest = update;
    //}

    //emitChoiceError(
    //    message: string, update ?: { pokemon: Pokemon, update: (req: PokemonMoveRequestData) => boolean | void }
    //) {
    //    this.choice.error = message;
    //    const updated = update ? this.updateRequestForPokemon(update.pokemon, update.update) : null;
    //    const type = `[${ updated ? 'Unavailable' : 'Invalid'}
    //    choice]`;
    //    this.battle.send('sideupdate', `${ this.id}\n | error |${ type} ${ message}`);
    //    if (updated) this.emitRequest(this.activeRequest!, true);
    //    if (this.battle.strictChoices) throw new Error(`${ type } ${ message}`);
    //    return false;
    //}

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

    public bool EmitChoiceError(
        string message,
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

    public bool ChooseMove(string? moveText = null, int targetLoc = 0, EventType eventType = EventType.None)
    {
        throw new NotImplementedException();
    }

    public bool ChooseMove(int? moveText = null, int targetLoc = 0, EventType eventType = EventType.None)
    {
        throw new NotImplementedException();
    }

    public bool UpdateDisabledRequest(Pokemon pokemon, PokemonMoveRequestData req)
    {
        throw new NotImplementedException();
    }

    public bool UpdateRequestForPokemon(Pokemon pokemon, Func<PokemonMoveRequestData, BoolVoidUnion> update)
    {
        throw new NotImplementedException();
    }

    public SideBoolUnion ChooseSwitch(string? slotText = null)
    {
        throw new NotImplementedException();
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

    public bool ChooseTeam(object data)
    {
        throw new NotImplementedException();
    }

    public bool ChooseShift()
    {
        throw new NotImplementedException();
    }

    public void ClearChoice()
    {
        throw new NotImplementedException();
    }

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