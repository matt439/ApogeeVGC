using ApogeeVGC.Data;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.BattleClasses;

public class BattleAsync : IBattle
{
    
    
    
    public int ActivePerHalf { get; init; }
    public BattleQueue Queue { get; init; }
    public Event? Event { get; init; }
    public ModdedDex Dex { get; init; }
    public int Gen => 9;
    public Library Library { get; init; }
    public bool PrintDebug { get; init; }
    public Field Field { get; init; }
    public IEffect Effect { get; init; }
    public EffectState EffectState { get; init; }
    public Side Side1 { get; init; }
    public Side Side2 { get; init; }
    public int? BattleSeed { get; set; }
    public bool IsGameComplete { get; init; }
    public GameType GameType { get; init; }
    public Pokemon? ActiveTarget { get; set; }
    public ActiveMove? ActiveMove { get; set; }

    public RelayVar? SingleEvent(EventId eventId, IEffect effect, EffectState? state = null,
        SingleEventTarget? target = null, SingleEventSource? source = null, IEffect? sourceEffect = null,
        RelayVar? relayVar = null, Delegate? customCallback = null)
    {
        throw new NotImplementedException();
    }

    public RelayVar? RunEvent(EventId eventId, RunEventTarget? target = null, RunEventSource? source = null,
        IEffect? sourceEffect = null, RelayVar? relayVar = null, bool? onEffect = null, bool? fastExit = null)
    {
        throw new NotImplementedException();
    }

    public void EachEvent(EventId eventId, IEffect? effect, bool? relayVar)
    {
        throw new NotImplementedException();
    }

    public BoolZeroUnion? Boost(SparseBoostsTable boost, Pokemon? target = null, Pokemon? source = null,
        IEffect? effect = null, bool isSecondary = false, bool isSelf = false)
    {
        throw new NotImplementedException();
    }

    public IntFalseUnion? Damage(int damage, Pokemon? target = null, Pokemon? source = null,
        BattleDamageEffect? effect = null, bool instafaint = false)
    {
        throw new NotImplementedException();
    }

    public IntFalseUnion? Heal(int damage, Pokemon? target = null, Pokemon? source = null,
        BattleHealEffect? effect = null)
    {
        throw new NotImplementedException();
    }

    public StatsTable SpreadModify(StatsTable baseStats, PokemonSet set)
    {
        throw new NotImplementedException();
    }

    public int FinalModify(int relayVar)
    {
        throw new NotImplementedException();
    }

    public double ChainModify(int numerator, int denominator = 1)
    {
        throw new NotImplementedException();
    }

    public double ChainModify(int[] numerator, int denominator = 1)
    {
        throw new NotImplementedException();
    }

    public double ChainModify(double numerator, int denominator = 1)
    {
        throw new NotImplementedException();
    }

    public double ChainModify(double[] numerator, int denominator = 1)
    {
        throw new NotImplementedException();
    }

    public bool CheckMoveMakesContact(Move move, Pokemon attacker, Pokemon defender, bool announcePads = false)
    {
        throw new NotImplementedException();
    }

    public bool RandomChance(int numerator, int denominator)
    {
        throw new NotImplementedException();
    }

    public int Random(int m, int n)
    {
        throw new NotImplementedException();
    }

    public int Random(int n)
    {
        throw new NotImplementedException();
    }

    public double Random()
    {
        throw new NotImplementedException();
    }

    public IBattle Copy()
    {
        throw new NotImplementedException();
    }

    public int ClampIntRange(int num, int? min, int? max)
    {
        throw new NotImplementedException();
    }

    public Pokemon? GetAtSlot(PokemonSlot? slot)
    {
        throw new NotImplementedException();
    }

    public int GetConfusionDamage(Pokemon pokemon, int basePower)
    {
        throw new NotImplementedException();
    }

    public EffectState InitEffectState(EffectStateId? id, int? effectOrder, Pokemon? target)
    {
        throw new NotImplementedException();
    }

    public MoveCategory GetCategory(ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public void Start()
    {
        throw new NotImplementedException();
    }

    public List<Pokemon> GetAllActive(bool? includeFainted = null)
    {
        throw new NotImplementedException();
    }

    public int StatModify(StatsTable baseStats, PokemonSet set, StatId statName)
    {
        throw new NotImplementedException();
    }

    public BoolVoidUnion? SuppressingAbility(Pokemon? target = null)
    {
        throw new NotImplementedException();
    }
}