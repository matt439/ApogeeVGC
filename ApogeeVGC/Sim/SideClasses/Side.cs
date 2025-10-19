using System.Text.Json.Nodes;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

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

    //addSideCondition(
    //    status: string | Condition, source: Pokemon | 'debug' | null = null, sourceEffect: Effect | null = null
    //) : boolean {
    //    if (!source && this.battle.event?.target) source = this.battle.event.target;
    //    if (source === 'debug') source = this.active[0];
    //    if (!source) throw new Error(`setting sidecond without a source`);
    //    if (!source.getSlot) source = (source as any as Side).active[0];

    //    status = this.battle.dex.conditions.get(status);
    //    if (this.sideConditions[status.id]) {
    //        if (!(status as any).onSideRestart) return false;
    //        return this.battle.singleEvent('SideRestart', status, this.sideConditions[status.id], this, source, sourceEffect);
    //    }
    //    this.sideConditions [status.id] = this.battle.initEffectState({
    //        id: status.id,
    //        target: this,
    //        source,
    //        sourceSlot: source.getSlot(),
    //        duration: status.duration,
    //    });
    //    if (status.durationCallback) {

    //        this.sideConditions[status.id].duration =
    //            status.durationCallback.call(this.battle, this.active[0], source, sourceEffect);
    //    }
    //    if (!this.battle.singleEvent('SideStart', status, this.sideConditions[status.id], this, source, sourceEffect)) {
    //        delete this.sideConditions[status.id];
    //        return false;
    //    }
    //    this.battle.runEvent('SideConditionStart', this, source, status);
    //    return true;
    //}

    public bool AddSideCondition(ConditionId status, Pokemon? source = null, IEffect? sourceEffect = null)
    {
        throw new NotImplementedException();
    }

    public bool AddSideCondition(Condition status, Pokemon? source = null, IEffect? sourceEffect = null)
    {
        throw new NotImplementedException();
    }

    public IEffect? GetSideCondition(ConditionId status)
    {
        throw new NotImplementedException();
    }

    public IEffect? GetSideCondition(IEffect status)
    {
        throw new NotImplementedException();
    }

    public IEffect? GetSideConditionData(ConditionId status)
    {
        throw new NotImplementedException();
    }

    public IEffect? GetSideConditionData(IEffect status)
    {
        throw new NotImplementedException();
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

    public RelayVar AddSlotCondition(PokemonIntUnion target, ConditionId status, Pokemon? source = null,
        IEffect? sourceEffect = null)
    {
        throw new NotImplementedException();
    }

    public RelayVar AddSlotCondition(PokemonIntUnion target, Condition status, Pokemon? source = null,
        IEffect? sourceEffect = null)
    {
        throw new NotImplementedException();
    }

    public IEffect? GetSlotCondition(PokemonIntUnion target, IEffect status)
    {
        throw new NotImplementedException();
    }

    public IEffect? GetSlotCondition(PokemonIntUnion target, ConditionId status)
    {
        throw new NotImplementedException();
    }

    public bool RemoveSlotCondition(PokemonIntUnion target, IEffect status)
    {
        throw new NotImplementedException();
    }

    public bool RemoveSlotCondition(PokemonIntUnion target, ConditionId status)
    {
        throw new NotImplementedException();
    }

    public void Send(List<object> parts)
    {
        throw new NotImplementedException();
    }

    public void EmitRequest(IChoiceRequest? update = null, bool updatedRequest = false)
    {
        throw new NotImplementedException();
    }

    public void EmitChoiceError(string message)
    {
        throw new NotImplementedException();
    }

    public bool IsChoiceDone()
    {
        throw new NotImplementedException();
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