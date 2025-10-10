using ApogeeVGC.Data;
using ApogeeVGC.Player;
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

public class BattleAsync : IBattle
{
    public BattleId Id { get; init; }
    public bool DebugMode { get; init; }
    public bool? ForceRandomChange { get; init; }
    public bool Deserialized { get; init; }
    public bool StrictChoices { get; init; }
    public Format Format { get; init; }
    public EffectState FormatData { get; init; }
    public GameType GameType { get; init; }
    public int ActivePerHalf
    {
        get;
        init
        {
            if (value is not 1 or 2 or 3)
            {
                throw new ArgumentOutOfRangeException(nameof(ActivePerHalf), "ActivePerHalf must be 1, 2, or 3.");
            }
            field = value;
        }
    }
    public Field Field { get; init; }
    public IReadOnlyList<Side> Sides
    {
        get;
        init
        {
            if (value.Count != 2)
            {
                throw new ArgumentException("There must be exactly 2 sides in a battle.", nameof(Sides));
            }
            field = value;
        }
    }
    public PrngSeed PrngSeed { get; init; }
    public ModdedDex Dex { get; set; }
    public int Gen => 9;
    // RuleTable

    Prng Prng { get; set; }
    public bool Rated { get; set; }
    public bool ReportExactHp { get; set; }
    public bool ReportPercentages { get; set; }
    public bool SupportCancel { get; set; }

    public BattleActions Actions { get; set; }
    public BattleQueue Queue { get; set; }
    public record FaintQueue
    {
        public required Pokemon Target { get; init; }
        public Pokemon? Source { get; init; }
        public IEffect? Effect { get; init; }
    }

    public List<string> Log { get; set; } = [];
    public List<string> InputLog { get; set; } = [];
    public List<string> MessageLog { get; set; } = [];
    public int SentLogPos { get; set; }
    public bool SentEnd { get; set; }
    public static bool SentRequests => true;

    public RequestState RequestState { get; set; }
    public int Turn { get; set; }
    public bool MidTurn { get; set; }
    public bool Started { get; set; }
    public bool Ended { get; set; }
    public PlayerId? Winnder { get; set; }

    public IEffect Effect { get; set; }
    public EffectState EffectState { get; set; }

    public Event Event { get; set; }
    public Event? Events { get; set; }
    public int EventDepth { get; set; }
    
    public ActiveMove? ActiveMove { get; set; }
    public Pokemon? ActivePokemon { get; set; }
    public Pokemon? ActiveTarget { get; set; }

    public ActiveMove? LastMove { get; set; }
    public MoveId? LastSuccessfulMoveThisTurn { get; set; }
    public int LastMoveLine { get; set; }
    public int LastDamage { get; set; }
    public int EffectOrder { get; set; }
    public bool QuickClawRoll { get; set; }
    public List<int> SpeedOrder { get; set; } = [];

    // TeamGenerator
    // Hints

    public static Undefined NotFail => new();
    public static int HitSubstiture => 0;
    public static bool Fail => false;
    public const object? SilentFail = null;

    public Action<SendType, IReadOnlyList<string>> Send { get; init; }

    //public Func<int, int?, int> Trunc { get; init; }
    //public Func<int, int?, int?, int> ClampIntRange { get; init; }


    public Library Library { get; init; }
    public bool PrintDebug { get; init; }

    public BattleAsync(BattleOptions options, Library library)
    {
        Library = library;

        Format = options.Format ?? Library.Formats[options.Id];
        // RuleTable
    }

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

    public int Trunc(int num, int bits = 0)
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