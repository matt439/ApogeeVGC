using ApogeeVGC.Data;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class Battle
{
    public bool DebugMode { get; init; }
    public Format Format { get; init; }
    public EffectState FormatData { get; init; }
    public GameType GameType { get; init; }

    /// <summary>
    /// The number of active pokemon per half-field.
    /// Currently restricted to singles (1) and Doubles (2). Triples (3) are not supported.
    /// </summary>
    public int ActivePerHalf
    {
        get;
        init
        {
            if (value is < 1 or > 2)
            {
                throw new ArgumentOutOfRangeException(nameof(ActivePerHalf),
                    "ActivePerHalf must be 1 (singles) or 2 (doubles).");
            }

            field = value;
        }
    }

    public Field Field { get; init; }

    public List<Side> Sides
    {
        get;
        init
        {
            if (value.Count != 2)
            {
                throw new ArgumentException("There must be exactly 2 sides in a battle.",
                    nameof(Sides));
            }

            field = value;
        }
    }

    public PrngSeed PrngSeed { get; init; }
    public ModdedDex Dex { get; set; }

    /// <summary>
    /// Generation number - hardcoded to 9 as this implementation only supports Gen 9 mechanics.
    /// Earlier generations (1-8) have significantly different mechanics and are not supported.
    /// </summary>
    public int Gen => 9;

    public RuleTable RuleTable { get; set; }

    public Prng Prng { get; set; }
    public bool Rated { get; set; }
    public bool ReportExactHp { get; set; } = false;
    public bool ReportPercentages { get; set; } = false;
    public bool SupportCancel { get; set; } = false;

    public BattleActions Actions { get; set; }
    public BattleQueue Queue { get; set; }
    public List<FaintQueue> FaintQueue { get; init; } = [];

    public List<string> Log { get; set; } = [];
    public List<string> InputLog { get; set; } = [];

    // Note: MessageLog is commented out as it's not currently used in this implementation.
    // It was part of the original Pokemon Showdown codebase but isn't required for our battle system.
    // Uncomment if you need separate message logging for debugging or replay purposes.
    //public List<string> MessageLog { get; set; } = [];

    public List<BattleMessage> PendingMessages { get; set; } = [];
    public int SentLogPos { get; set; }
    public bool SentEnd { get; set; }
    public static bool SentRequests => true;

    public RequestState RequestState { get; set; } = RequestState.None;
    public int Turn { get; set; }
    public bool MidTurn { get; set; }
    public bool Started { get; set; }
    public bool Ended { get; set; }
    public string? Winner { get; set; }

    public IEffect Effect { get; set; }
    public EffectState EffectState { get; set; }

    public Event Event { get; set; } = new();
    public Event? Events { get; set; } = null;
    public int EventDepth { get; set; }

    public ActiveMove? ActiveMove { get; set; }
    public Pokemon? ActivePokemon { get; set; }
    public Pokemon? ActiveTarget { get; set; }

    public ActiveMove? LastMove { get; set; }
    public MoveId? LastSuccessfulMoveThisTurn { get; set; }
    public int LastMoveLine { get; set; } = -1;
    public int LastDamage { get; set; }
    public int EffectOrder { get; set; }
    public bool QuickClawRoll { get; set; }
    public List<int> SpeedOrder { get; set; } = [];

    // Note: TeamGenerator is not implemented as we only support constructed teams, not random battles.
    // Random battle team generation would require implementing the full Pokemon Showdown team generator.
    // TeamGenerator teamGenerator = null;

    public static Undefined NotFail => new();
    public static int HitSubstitute => 0;
    public static bool Fail => false;
    public const object? SilentFail = null;

    public Action<SendType, IEnumerable<string>> Send { get; init; }

    public Library Library { get; init; }
    public bool DisplayUi { get; init; }
    public Side P1 => Sides[0];
    public Side P2 => Sides[1];
    private HashSet<string> Hints { get; } = [];

    /// <summary>
    /// Battle history for debugging and analysis.
    /// </summary>
    public BattleHistory History { get; } = new();

    public Battle(BattleOptions options, Library library)
    {
        Library = library;
        Dex = new ModdedDex(Library);
        Field = new Field(this);

        Format = options.Format ?? Library.Formats[options.Id];
        RuleTable = Format.RuleTable ?? new RuleTable();
        DebugMode = options.Debug;
        DisplayUi = true; // Always display UI for battle streams
        FormatData = InitEffectState(Format.FormatId);
        GameType = Format.GameType;

        // Create sides with temporary Foe references (will be set properly below)
        var side1 = new Side(options.Player1Options.Name, this, SideId.P1,
            options.Player1Options.Team.ToArray())
        {
            N = 0,
        };

        var side2 = new Side(options.Player2Options.Name, this, SideId.P2,
            options.Player2Options.Team.ToArray())
        {
            N = 1,
        };

        // Set up bidirectional Foe relationships
        side1.Foe = side2;
        side2.Foe = side1;

        Sides = [side1, side2];

        ActivePerHalf = 1;
        Prng = new Prng(options.Seed);
        PrngSeed = Prng.StartingSeed;

        Rated = options.Rated ?? false;

        Queue = new BattleQueue(this);
        Actions = new BattleActions(this);

        // Initialize effect as an empty Format.
        Effect = new Format
        {
            Name = "EmptyEffect",
        };
        EffectState = InitEffectState();

        for (int i = 0; i < ActivePerHalf * 2; i++)
        {
            SpeedOrder.Add(i);
        }

        Send = options.Send ?? ((_, _) => { });

        Debug("Battle constructor complete.");
    }

    public int Random(int m, int n)
    {
        return Prng.Random(m, n);
    }

    public int Random(int n)
    {
        return Prng.Random(n);
    }

    public double Random()
    {
        return Prng.Random();
    }

    public bool RandomChance(int numerator, int denominator)
    {
        return Prng.RandomChance(numerator, denominator);
    }

    public T Sample<T>(IReadOnlyList<T> items)
    {
        return Prng.Sample(items);
    }

    public void ResetRng(PrngSeed? seed = null)
    {
        Prng = new Prng(seed);
    }

    public static IReadOnlyList<PokemonSet> GetTeam(PlayerOptions options)
    {
        return options.Team ?? throw new InvalidOperationException();
    }

    /// <summary>
    /// Initializes an EffectState object with proper effect ordering.
    /// Effect order is used to determine priority when multiple effects trigger.
    /// - Effects with explicit effectOrder use that value
    /// - Effects on active Pokemon get auto-incremented order
    /// - Effects on non-Pokemon targets (Side, Field, Battle) get auto-incremented order
    /// - Effects on inactive Pokemon or with no target get order 0
    /// 
    /// TypeScript equivalent: initEffectState(obj, effectOrder)
    /// Logic: effectOrder is incremented when obj.id exists AND obj.target exists AND
    /// (obj.target is not a Pokemon OR obj.target is an active Pokemon)
    /// </summary>
    public EffectState InitEffectState(EffectStateId? id = null, int? effectOrder = null,
        EffectStateTarget? target = null)
    {
        // Create new EffectState with the provided or default ID
        EffectStateId effectId = id ?? EffectStateId.FromEmpty();

        int finalEffectOrder;

        if (effectOrder.HasValue)
        {
            // If an effect order is explicitly provided, use it
            finalEffectOrder = effectOrder.Value;
        }
        else if (effectId != EffectStateId.FromEmpty() && target != null)
        {
            // Auto-assign effect order for effects on targets
            // Increment if:
            // - Target is NOT a Pokemon (Side, Field, Battle always increment)
            // - OR target IS a Pokemon AND is active
            bool shouldIncrement = target switch
            {
                PokemonEffectStateTarget pokemon => pokemon.Pokemon
                    .IsActive, // Pokemon: only if active
                SideEffectStateTarget => true, // Side: always increment
                FieldEffectStateTarget => true, // Field: always increment
                BattleEffectStateTarget => true, // Battle: always increment
                _ => false,
            };

            finalEffectOrder = shouldIncrement ? EffectOrder++ : 0;
        }
        else
        {
            // Effects with no ID or no target get a default order of 0
            finalEffectOrder = 0;
        }

        // Create and return the EffectState
        return new EffectState
        {
            Id = effectId,
            EffectOrder = finalEffectOrder,
            Duration = null,
            Target = target,
        };
    }

    public EffectState InitEffectState(EffectStateId id, Pokemon? source, PokemonSlot? sourceSlot,
        int? duration)
    {
        // Use the first overload to handle basic initialization and effect ordering
        // Pass the source Pokemon wrapped in EffectStateTarget for effect ordering purposes
        EffectStateTarget? targetWrapper =
            source != null ? new PokemonEffectStateTarget(source) : null;
        EffectState state = InitEffectState(id, effectOrder: null, target: targetWrapper);

        // Add the additional properties specific to this overload
        state.Source = source;
        state.SourceSlot = sourceSlot;
        state.Duration = duration;

        return state;
    }

    public EffectState InitEffectState(EffectStateId id, Side target, Pokemon source,
        PokemonSlot sourceSlot,
        bool isSlotCondition, int? duration)
    {
        // Use the first overload to handle basic initialization and effect ordering
        // Side conditions get proper effect ordering (sides always increment)
        EffectState state = InitEffectState(id, effectOrder: null,
            target: new SideEffectStateTarget(target));

        // Add the additional properties specific to this overload
        state.Source = source;
        state.SourceSlot = sourceSlot;
        state.IsSlotCondition = isSlotCondition;
        state.Duration = duration;

        return state;
    }

    public void ClearEffectState(ref EffectState state)
    {
        EffectStateTarget? prevTarget = state.Target;
        state = new EffectState
        {
            Id = EffectStateId.FromEmpty(),
            Target = prevTarget,
            EffectOrder = 0,
        };
    }

    public BattlePerspective GetPerspectiveForSide(SideId sideId,
        BattlePerspectiveType battlePerspectiveType = BattlePerspectiveType.InBattle)
    {
        Side playerSide = GetSide(sideId);
        return new BattlePerspective
        {
            PerspectiveType = battlePerspectiveType,
            Field = Field.GetPerspective(),
            PlayerSide = playerSide.GetPlayerPerspective(),
            OpponentSide = playerSide.Foe.GetOpponentPerspective(),
            TurnCounter = Turn,
        };
    }

    public Battle Copy()
    {
        throw new NotImplementedException();
    }
}