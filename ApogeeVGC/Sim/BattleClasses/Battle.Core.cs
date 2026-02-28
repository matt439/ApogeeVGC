using ApogeeVGC.Data;
using ApogeeVGC.Sim.Actions;
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

    /// <summary>
    /// Lightweight counter tracking the number of log entries that would have been added.
    /// Used for safety checks (infinite-loop detection) when DisplayUi is false to avoid
    /// expensive string formatting.
    /// </summary>
    public int LogMessageCount { get; set; }

    // Note: MessageLog is commented out as it's not currently used in this implementation.
    // It was part of the original Pokemon Showdown codebase but isn't required for our battle system.
    // Uncomment if you need separate message logging for debugging or replay purposes.
    //public List<string> MessageLog { get; set; } = [];

    public List<BattleEvent> PendingEvents { get; set; } = [];
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

    /// <summary>
    /// Pool of reusable <see cref="List{EventListener}"/> instances to eliminate
    /// per-call allocations inside <see cref="FindEventHandlers"/>.
    /// Lists are rented at the start of <see cref="RunEvent"/> and returned at the end.
    /// </summary>
    private readonly Stack<List<EventListener>> _handlerListPool = new();

    /// <summary>
    /// Pool of reusable <see cref="Event"/> instances to eliminate per-call
    /// allocations inside <see cref="RunEvent"/> and <see cref="SingleEvent"/>.
    /// Events are rented at entry, populated with context, and returned in the finally block.
    /// </summary>
    private readonly Stack<Event> _eventPool = new();

    /// <summary>
    /// Pool of reusable <see cref="List{Pokemon}"/> instances to eliminate per-call
    /// allocations in methods like <see cref="EachEvent"/> that need a temporary sorted list.
    /// </summary>
    private readonly Stack<List<Pokemon>> _pokemonListPool = new();

    internal List<Pokemon> RentPokemonList()
    {
        if (_pokemonListPool.Count > 0)
        {
            var list = _pokemonListPool.Pop();
            list.Clear();
            return list;
        }
        return new List<Pokemon>(4);
    }

    internal void ReturnPokemonList(List<Pokemon> list)
    {
        list.Clear();
        _pokemonListPool.Push(list);
    }

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

    /// <summary>
    /// Maximum number of turns before the battle is automatically ended as a tie.
    /// If null or 0, no turn limit is enforced.
    /// </summary>
    public int? MaxTurns { get; init; }

    public Battle(BattleOptions options, Library library)
    {
        Library = library;
        Dex = new ModdedDex(Library);
        Field = new Field(this);

        Format = options.Format ?? Library.Formats[options.Id];
        RuleTable = Format.RuleTable ?? new RuleTable();
        DebugMode = options.Debug;
        DisplayUi = options.DisplayUi ?? !options.Sync;
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

        // Set ActivePerHalf based on game type
        ActivePerHalf = GameType switch
        {
            GameType.Singles => 1,
            GameType.Doubles => 2,
            _ => throw new NotImplementedException($"GameType {GameType} is not supported")
        };

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

        MaxTurns = options.MaxTurns;

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

    /// <summary>
    /// Creates a deep copy of this Battle, including all Sides, Pokemon, Field, and Queue state.
    /// The copy uses a fresh RNG and skips logs/history/events/pools.
    /// All internal Pokemon references are properly remapped to the copied instances.
    /// </summary>
    public Battle Copy()
    {
        var pokemonMap = new Dictionary<Pokemon, Pokemon>();
        return new Battle(this, pokemonMap);
    }

    /// <summary>
    /// Internal copy constructor. Creates a deep copy with proper Pokemon reference remapping.
    /// </summary>
    internal Battle(Battle source, Dictionary<Pokemon, Pokemon> pokemonMap)
    {
        // Share immutable data
        Library = source.Library;
        Dex = source.Dex;
        Format = source.Format;
        RuleTable = source.RuleTable;
        FormatData = source.FormatData;
        GameType = source.GameType;
        ActivePerHalf = source.ActivePerHalf;
        DebugMode = false;
        DisplayUi = false;
        MaxTurns = source.MaxTurns;
        PrngSeed = source.PrngSeed;
        Send = (_, _) => { };

        // Fresh RNG for simulation
        Prng = new Prng(null);

        // --- Pass 1: Deep copy Sides (which creates all Pokemon copies) ---
        var side1 = source.P1.Copy(this, pokemonMap);
        var side2 = source.P2.Copy(this, pokemonMap);
        side1.Foe = side2;
        side2.Foe = side1;
        Sides = [side1, side2];

        // Deep copy Field
        Field = source.Field.Copy(this, pokemonMap);

        // --- Pass 2: Remap all Pokemon references ---
        side1.RemapPokemonReferences(pokemonMap, source.P1);
        side2.RemapPokemonReferences(pokemonMap, source.P2);
        Field.RemapPokemonReferences(pokemonMap);

        // Copy BattleQueue with remapped actions
        Queue = new BattleQueue(this);
        foreach (var action in source.Queue.List)
            Queue.List.Add(RemapAction(action, pokemonMap));

        // Fresh BattleActions
        Actions = new BattleActions(this);

        // Copy battle state
        Turn = source.Turn;
        MidTurn = source.MidTurn;
        Started = source.Started;
        Ended = source.Ended;
        Winner = source.Winner;
        RequestState = source.RequestState;
        Rated = source.Rated;
        ReportExactHp = source.ReportExactHp;
        ReportPercentages = source.ReportPercentages;
        SupportCancel = source.SupportCancel;
        LastDamage = source.LastDamage;
        EffectOrder = source.EffectOrder;
        QuickClawRoll = source.QuickClawRoll;
        LastSuccessfulMoveThisTurn = source.LastSuccessfulMoveThisTurn;
        LastMoveLine = source.LastMoveLine;
        SpeedOrder = new List<int>(source.SpeedOrder);

        // Effect (library reference, share)
        Effect = source.Effect;
        EffectState = source.EffectState.DeepClone();
        EffectState.RemapPokemonReferences(pokemonMap);

        // Event system (fresh)
        Event = new Event();
        Events = null;
        EventDepth = 0;

        // Remap Pokemon references in battle state
        ActivePokemon = EffectState.RemapPokemon(source.ActivePokemon, pokemonMap);
        ActiveTarget = EffectState.RemapPokemon(source.ActiveTarget, pokemonMap);
        ActiveMove = CopyActiveMove(source.ActiveMove, pokemonMap);
        LastMove = CopyActiveMove(source.LastMove, pokemonMap);

        // Copy FaintQueue with remapped Pokemon references
        FaintQueue = source.FaintQueue
            .Select(fq => new FaintQueue
            {
                Target = pokemonMap.TryGetValue(fq.Target, out var t) ? t : fq.Target,
                Source = fq.Source is not null
                    ? EffectState.RemapPokemon(fq.Source, pokemonMap)
                    : null,
                Effect = fq.Effect,
            })
            .ToList();

        // Skipped: Log, InputLog, PendingEvents, History, object pools, Hints
    }

    /// <summary>
    /// Remaps an IAction's Pokemon references using the pokemonMap.
    /// Action types are records, so we use 'with' expressions for clean copying.
    /// </summary>
    private static IAction RemapAction(IAction action, Dictionary<Pokemon, Pokemon> pokemonMap)
    {
        return action switch
        {
            MoveAction ma => ma with
            {
                Pokemon = pokemonMap.TryGetValue(ma.Pokemon, out var mp) ? mp : ma.Pokemon,
                OriginalTarget = ma.OriginalTarget is not null
                    ? EffectState.RemapPokemon(ma.OriginalTarget, pokemonMap)
                    : null,
            },
            SwitchAction sa => sa with
            {
                Pokemon = pokemonMap.TryGetValue(sa.Pokemon, out var sp) ? sp : sa.Pokemon,
                Target = pokemonMap.TryGetValue(sa.Target, out var st) ? st : sa.Target,
            },
            PokemonAction pa => pa with
            {
                Pokemon = pokemonMap.TryGetValue(pa.Pokemon, out var pp) ? pp : pa.Pokemon,
                Dragger = pa.Dragger is not null
                    ? EffectState.RemapPokemon(pa.Dragger, pokemonMap)
                    : null,
            },
            TeamAction ta => ta with
            {
                Pokemon = pokemonMap.TryGetValue(ta.Pokemon, out var tp) ? tp : ta.Pokemon,
            },
            _ => action,
        };
    }

    /// <summary>
    /// Creates a copy of an ActiveMove with Pokemon references remapped.
    /// </summary>
    private static ActiveMove? CopyActiveMove(ActiveMove? move, Dictionary<Pokemon, Pokemon> pokemonMap)
    {
        if (move is null) return null;

        return move with
        {
            HitTargets = null,
            Allies = null,
            AuraBooster = null,
            RuinedAtk = null,
            RuinedDef = null,
            RuinedSpA = null,
            RuinedSpD = null,
            MoveHitData = null,
        };
    }
}