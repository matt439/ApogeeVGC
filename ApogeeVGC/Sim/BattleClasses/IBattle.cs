using ApogeeVGC.Data;
using ApogeeVGC.Sim.Actions;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
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
    bool DisplayUi { get; }
    Field Field { get; }

    /// <summary>
    /// Whether to report exact HP values in battle output.
    /// When true, shows "HP/MaxHP" format for all players.
    /// When false, uses percentage (Gen 7+) or pixel-based display.
    /// </summary>
    bool ReportExactHp { get; }

    /// <summary>
    /// Whether to report HP as percentages instead of pixels (Gen 3-6).
    /// Overridden by ReportExactHp if that is true.
    /// </summary>
    bool ReportPercentages { get; }

    IEffect Effect { get; }
    EffectState EffectState { get; }

    //Side Side1 { get; }
    //Side Side2 { get; }
    //int? BattleSeed { get; set; }

    GameType GameType { get; }

    Pokemon? ActiveTarget { get; set; }
    ActiveMove? ActiveMove { get; set; }

    BattleActions Actions { get; }


    RelayVar? SingleEvent(EventId eventId, IEffect effect, EffectState? state = null,
        SingleEventTarget? target = null, SingleEventSource? source = null, IEffect? sourceEffect = null,
        RelayVar? relayVar = null, EffectDelegate? customCallback = null);

    RelayVar? RunEvent(EventId eventId, RunEventTarget? target = null, RunEventSource? source = null,
        IEffect? sourceEffect = null, RelayVar? relayVar = null, bool? onEffect = null, bool? fastExit = null);

    RelayVar? PriorityEvent(EventId eventId, PokemonSideBattleUnion target, Pokemon? source = null,
        IEffect? effect = null, RelayVar? relayVar = null, bool onEffect = false);

    void EachEvent(EventId eventId, IEffect? effect = null, bool? relayVar = null);

    BoolZeroUnion? Boost(SparseBoostsTable boost, Pokemon? target = null, Pokemon? source = null,
        IEffect? effect = null, bool isSecondary = false, bool isSelf = false);

    IntFalseUndefinedUnion Damage(int damage, Pokemon? target = null, Pokemon? source = null,
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

    int Trunc(double num, int bits = 0);

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

    EffectState InitEffectState(EffectStateId id, Side target, Pokemon source, PokemonSlot sourceSlot,
        bool isSlotCondition, int? duration);

    MoveCategory GetCategory(ActiveMove move);

    List<Pokemon> GetAllActive(bool includeFainted = false);

    int StatModify(StatsTable baseStats, PokemonSet set, StatId statName);

    bool SuppressingAbility(Pokemon? target = null);

    int CanSwitch(Side side);

    RuleTable RuleTable { get; }

    T Sample<T>(IReadOnlyList<T> items);

    Action<SendType, IEnumerable<string>> Send { get; }

    bool StrictChoices { get; }

    bool ValidTargetLoc(int targetLoc, Pokemon source, MoveTarget targetType);

    RequestState RequestState { get; }

    bool Ended { get; }

    void RetargetLastMove(Pokemon newTarget);

    List<FaintQueue> FaintQueue { get; }

    int Turn { get; }

    int EffectOrder { get; set; }

    void SpeedSort<T>(List<T> list, Func<T, T, int>? comparator = null) where T : IPriorityComparison;

    List<int> SpeedOrder { get; set; }

    void FieldEvent(EventId eventId, List<Pokemon>? targets = null);

    Pokemon? GetRandomSwitchable(Side side);

    bool? FaintMessages(bool lastFirst = false, bool forceCheck = false, bool checkWin = true);

    bool? CheckWin(FaintQueue? faintData = null);

    MoveId? LastSuccessfulMoveThisTurn { get; set; }

    Pokemon? GetTarget(Pokemon pokemon, MoveId moveId, int targetLoc, Pokemon? originalTarget = null);

    Pokemon? GetTarget(Pokemon pokemon, Move move, int targetLoc, Pokemon? originalTarget = null);

    void SetActiveMove(ActiveMove? move = null, Pokemon? pokemon = null, Pokemon? target = null);

    void Faint(Pokemon pokemon, Pokemon? source = null, IEffect? effect = null);

    void AttrLastMove(params StringNumberDelegateObjectUnion[] args);

    int DirectDamage(int damage, Pokemon? target = null, Pokemon? source = null, IEffect? effect = null);

    public SpreadMoveDamage SpreadDamage(SpreadMoveDamage damage, SpreadMoveTargets? targetArray = null,
        Pokemon? source = null, BattleDamageEffect? effect = null, bool instaFaint = false);

    int HitSubstitute { get; }

    void Debug(string activity);

    void Dispose();

    void SendUpdates();

    void SetPlayer(SideId slot, PlayerOptions options);

    void UndoChoice(SideId sideId);

    bool Choose(SideId sideId, Choice input);

    bool ForceWin(SideId? side = null);

    List<string> InputLog { get; }

    bool Tie();

    bool Lose(SideId sideId);

    void ResetRng(PrngSeed? seed = null);

    Prng Prng { get; }

    bool Tiebreak();

    void Add(params PartFuncUnion[] parts);

    void ShowOpenTeamSheets();
}