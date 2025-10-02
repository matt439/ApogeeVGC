using ApogeeVGC.Data;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Turns;
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

    double ChainModify(int numerator, int denominator = 1);
    double ChainModify(int[] numerator, int denominator = 1);
    double ChainModify(double numerator, int denominator = 1);
    double ChainModify(double[] numerator, int denominator = 1);

    bool CheckMoveMakesContact(Move move, Pokemon attacker, Pokemon defender, bool announcePads = false);

    bool RandomChance(int numerator, int denominator);

    Library Library { get; }
    Random Random { get; }
    bool PrintDebug { get; }
    Field Field { get; }

    IEffect Effect { get; }
    EffectState EffectState { get; }

    Side Side1 { get; }
    Side Side2 { get; }
    int? BattleSeed { get; set; }

    IBattle Copy();

    bool IsGameComplete { get; }
    Turn CurrentTurn { get; }

    BattleChoice[] GenerateChoicesForMcts(PlayerId playerId);

    void Start();
}