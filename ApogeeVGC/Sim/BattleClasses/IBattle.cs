using ApogeeVGC.Data;
using ApogeeVGC.Sim.Actions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Ui;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.BattleClasses;

public interface IBattle
{
    int ActivePerHalf { get; }
    BattleQueue Queue { get; }
    Event? Event { get; }
    ModdedDex Dex { get; }
    int Gen { get; }

    Library Library { get; }
    //Random Random { get; }
    bool PrintDebug { get; }
    Field Field { get; }

    IEffect Effect { get; }
    EffectState EffectState { get; }

    //Side Side1 { get; }
    //Side Side2 { get; }
    //int? BattleSeed { get; set; }

    GameType GameType { get; }

    Pokemon? ActiveTarget { get; set; }
    ActiveMove? ActiveMove { get; set; }

    public BattleActions Actions { get; }


    RelayVar? SingleEvent(EventId eventId, IEffect effect, EffectState? state = null,
        SingleEventTarget? target = null, SingleEventSource? source = null, IEffect? sourceEffect = null,
        RelayVar? relayVar = null, EffectDelegate? customCallback = null);

    RelayVar? RunEvent(EventId eventId, RunEventTarget? target = null, RunEventSource? source = null,
        IEffect? sourceEffect = null, RelayVar? relayVar = null, bool? onEffect = null, bool? fastExit = null);

    void EachEvent(EventId eventId, IEffect? effect = null, bool? relayVar = null);

    BoolZeroUnion? Boost(SparseBoostsTable boost, Pokemon? target = null, Pokemon? source = null,
        IEffect? effect = null, bool isSecondary = false, bool isSelf = false);

    IntFalseUnion? Damage(int damage, Pokemon? target = null, Pokemon? source = null,
        BattleDamageEffect? effect = null, bool instafaint = false);

    IntFalseUnion? Heal(int damage, Pokemon? target = null, Pokemon? source = null, BattleHealEffect? effect = null);

    Pokemon? GetRandomTarget(Pokemon pokemon, MoveId move);
    Pokemon? GetRandomTarget(Pokemon pokemon, Move move);

    void GetActionSpeed(IAction action);

    StatsTable SpreadModify(StatsTable baseStats, PokemonSet set);

    //void Add(BattleAddId id);

    int FinalModify(int relayVar);

    //double ChainModify(int numerator, int denominator = 1);
    //double ChainModify(int[] numerator, int denominator = 1);
    //double ChainModify(double numerator, int denominator = 1);
    //double ChainModify(double[] numerator, int denominator = 1);

    double ChainModify(int numerator, int denominator = 1);
    double ChainModify(int[] fraction);
    double ChainModify(double fraction);

    int Modify(int value, int numerator, int denominator = 1);
    int Modify(int value, int[] fraction);
    int Modify(int value, double fraction);

    void ClearEffectState(ref EffectState state);

    bool CheckMoveMakesContact(Move move, Pokemon attacker, Pokemon defender, bool announcePads = false);

    List<Side> Sides { get; }

    int Trunc(int num, int bits = 0);

    bool RandomChance(int numerator, int denominator);

    /// <summary>
    /// Returns a random integer within the specified range.
    /// </summary>
    /// <param name="m">The inclusive minimum value.</param>
    /// <param name="n">The exclusive maximum value.</param>
    /// <returns>A random integer x where m ≤ x &lt; n.</returns>
    /// <exception cref="ArgumentException">Thrown when m is greater than or equal to n.</exception>
    int Random(int m, int n);

    /// <summary>
    /// Returns a random integer from 0 up to but not including the specified maximum.
    /// </summary>
    /// <param name="n">The exclusive maximum value.</param>
    /// <returns>A random integer x where 0 ≤ x &lt; n.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when n is less than or equal to 0.</exception>
    int Random(int n);

    /// <summary>
    /// Returns a random double-precision floating-point number.
    /// </summary>
    /// <returns>A random double x where 0.0 ≤ x &lt; 1.0.</returns>
    double Random();

    int Randomizer(int baseDamage);

    IBattle Copy();

    int ClampIntRange(int num, int? min, int? max);
    //Func<int, int?, int?, int> ClampIntRange { get; }

    Pokemon? GetAtSlot(PokemonSlot? slot);

    //Turn CurrentTurn { get; }

    //BattleChoice[] GenerateChoicesForMcts(PlayerId playerId);

    EffectState InitEffectState(EffectStateId? id = null, int? effectOrder = null, Pokemon? target = null);

    EffectState InitEffectState(EffectStateId id, Pokemon? source, PokemonSlot? sourceSlot, int? duration);

    MoveCategory GetCategory(ActiveMove move);

    void Start();

    List<Pokemon> GetAllActive(bool includeFainted = false);

    int StatModify(StatsTable baseStats, PokemonSet set, StatId statName);

    bool SuppressingAbility(Pokemon? target = null);
}