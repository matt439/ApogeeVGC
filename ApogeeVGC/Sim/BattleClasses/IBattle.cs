using ApogeeVGC.Data;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.Core;
//using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
//using ApogeeVGC.Sim.Turns;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.BattleClasses;

public interface IBattle
{
    RelayVar? SingleEvent(EventId eventId, IEffect effect, EffectState? state, SingleEventTarget? target,
        SingleEventSource? source, IEffect? sourceEffect, RelayVar? relayVar, Delegate? customCallback);

    RelayVar? RunEvent(EventId eventId, RunEventTarget? target, PokemonFalseUnion? source, IEffect? sourceEffect,
        RelayVar? relayVar, bool? onEffect, bool? fastExit);

    void EachEvent(EventId eventId, IEffect? effect, bool? relayVar);

    BoolZeroUnion? Boost(SparseBoostsTable boost, Pokemon? target = null, Pokemon? source = null,
        IEffect? effect = null, bool isSecondary = false, bool isSelf = false);

    IntFalseUnion? Damage(int damage, Pokemon? target = null, Pokemon? source = null,
        BattleDamageEffect? effect = null, bool instafaint = false);

    int FinalModify(int relayVar);

    double ChainModify(int numerator, int denominator = 1);
    double ChainModify(int[] numerator, int denominator = 1);
    double ChainModify(double numerator, int denominator = 1);
    double ChainModify(double[] numerator, int denominator = 1);

    bool CheckMoveMakesContact(Move move, Pokemon attacker, Pokemon defender, bool announcePads = false);

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

    Library Library { get; }
    //Random Random { get; }
    bool PrintDebug { get; }
    Field Field { get; }

    IEffect Effect { get; }
    EffectState EffectState { get; }

    Side Side1 { get; }
    Side Side2 { get; }
    int? BattleSeed { get; set; }

    IBattle Copy();

    bool IsGameComplete { get; }

    GameType GameType { get; }

    //Turn CurrentTurn { get; }

    //BattleChoice[] GenerateChoicesForMcts(PlayerId playerId);

    EffectState InitEffectState(EffectStateId? id, int? effectOrder, Pokemon? target);

    void Start();
}