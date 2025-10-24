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
using ApogeeVGC.Sim.Ui;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Extensions;
using static ApogeeVGC.Sim.PokemonClasses.Pokemon;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleAsync : IBattle, IDisposable
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
            if (value is not 1)
            {
                throw new ArgumentOutOfRangeException(nameof(ActivePerHalf), "ActivePerHalf must be 1.");
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
                throw new ArgumentException("There must be exactly 2 sides in a battle.", nameof(Sides));
            }
            field = value;
        }
    }
    public PrngSeed PrngSeed { get; init; }
    public ModdedDex Dex { get; set; }
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
    public List<string> MessageLog { get; set; } = [];
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

    // TeamGenerator
    // Hints

    public static Undefined NotFail => new();
    public int HitSubstitute => 0;
    public static bool Fail => false;
    public const object? SilentFail = null;

    public Action<SendType, IEnumerable<string>> Send { get; init; }

    public Library Library { get; init; }
    public bool DisplayUi { get; init; }
    public Side P1 => Sides[0];
    public Side P2 => Sides[1];
    public static Side P3 => throw new Exception("3v3 battles are not implemented.");
    public static Side P4 => throw new Exception("4v4 battles are not implemented.");
    private HashSet<string> Hints { get; set; } = [];

    private bool _disposed;

    public BattleAsync(BattleOptions options, Library library)
    {
        Library = library;
        Dex = new ModdedDex(Library);
        RuleTable = new RuleTable();
        Field = new Field(this);

        Format = options.Format ?? Library.Formats[options.Id];
        // RuleTable
        Id = BattleId.Default;
        DebugMode = options.Debug;
        ForceRandomChange = options.ForceRandomChance;
        Deserialized = options.Deserialized;
        StrictChoices = options.StrictChoices;
        FormatData = InitEffectState(Format.FormatId);
        GameType = Format.GameType;
        Sides = new List<Side>(2)
        {
            [0] = new Side(this),
            [1] = new Side(this),
        };
        ActivePerHalf = 1;
        Prng = options.Prng ?? new Prng(options.Seed);
        PrngSeed = Prng.StartingSeed;

        Rated = options.Rated ?? false;

        Queue = new BattleQueue(this);
        Actions = new BattleActions(this);

        Effect = null!; // TODO: Fix nullability
        EffectState = InitEffectState();

        for (int i = 0; i < ActivePerHalf * 2; i++)
        {
            SpeedOrder.Add(i);
        }

        // TeamGenerator
        // Hints

        Send = options.Send ?? ((type, messages) => { });

        // InputOptions

        if (options.P1 is not null)
        {
            SetPlayer(SideId.P1, options.P1);
        }
        if (options.P2 is not null)
        {
            SetPlayer(SideId.P2, options.P2);
        }
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
        return ForceRandomChange ?? Prng.RandomChance(numerator, denominator);
    }

    public T Sample<T>(IReadOnlyList<T> items)
    {
        return Prng.Sample(items);
    }

    public void ResetRng(PrngSeed? seed = null)
    {
        Prng = new Prng(seed);
    }

    /// <summary>
    /// Determines if the current active move is suppressing abilities.
    /// Returns true if:
    /// - There's an active Pokemon that is active (not fainted)
    /// - The active Pokemon is not the target (or Gen &lt; 8)
    /// - There's an active move that ignores abilities
    /// - The target doesn't have an Ability Shield
    /// Used for abilities like Mold Breaker, Teravolt, Turboblaze and moves like
    /// Sunsteel Strike, Moongeist Beam that ignore target abilities.
    /// </summary>
    public bool SuppressingAbility(Pokemon? target = null)
    {
        // Check if there's an active Pokemon and it's currently active
        if (ActivePokemon is not { IsActive: true })
        {
            return false;
        }

        // In Gen 8+, moves can't suppress their user's own ability
        // In earlier gens, they could
        if (ActivePokemon == target && Gen >= 8)
        {
            return false;
        }

        // Check if there's an active move that ignores abilities
        if (ActiveMove is not { IgnoreAbility: true })
        {
            return false;
        }

        // Ability Shield protects against ability suppression
        return target?.HasItem(ItemId.AbilityShield) != true;
    }

    public void SetActiveMove(ActiveMove? move = null, Pokemon? pokemon = null, Pokemon? target = null)
    {
        ActiveMove = move;
        ActivePokemon = pokemon;
        ActiveTarget = target ?? pokemon;
    }

    public void ClearActiveMove(bool failed = false)
    {
        if (ActiveMove != null)
        {
            if (!failed)
            {
                LastMove = ActiveMove;
            }
            ActiveMove = null;
            ActivePokemon = null;
            ActiveTarget = null;
        }
    }

    public void UpdateSpeed()
    {
        foreach (Pokemon pokemon in GetAllActive())
        {
            pokemon.UpdateSpeed();
        }
    }

    /// <summary>
    /// The default sort order for actions, but also event listeners.
    /// 
    /// 1. Order, low to high (default last)
    /// 2. Priority, high to low (default 0)
    /// 3. Speed, high to low (default 0)
    /// 4. SubOrder, low to high (default 0)
    /// 5. EffectOrder, low to high (default 0)
    /// 
    /// This is a static comparison function that doesn't reference battle state.
    /// </summary>
    public static int ComparePriority(IPriorityComparison a, IPriorityComparison b)
    {
        // 1. Order comparison (lower values first, Max = last)
        // ActionOrder.Max represents items without explicit order
        int orderResult = a.Order.CompareTo(b.Order);
        if (orderResult != 0) return orderResult;

        // 2. Priority comparison (higher values first)
        int priorityResult = b.Priority.CompareTo(a.Priority); // Reversed for descending
        if (priorityResult != 0) return priorityResult;

        // 3. Speed comparison (higher values first)
        int speedResult = b.Speed.CompareTo(a.Speed); // Reversed for descending
        if (speedResult != 0) return speedResult;

        // 4. SubOrder comparison (lower values first)
        int subOrderResult = a.SubOrder.CompareTo(b.SubOrder);
        if (subOrderResult != 0) return subOrderResult;

        // 5. EffectOrder comparison (lower values first)
        return a.EffectOrder.CompareTo(b.EffectOrder);
    }

    /// <summary>
    /// Compares two event handlers for redirect order.
    /// Used to determine which redirect effect triggers first.
    /// 
    /// Order:
    /// 1. Priority (higher first)
    /// 2. Speed (higher first)
    /// 3. Ability state effect order (lower first, only if both are abilities)
    /// </summary>
    public static int CompareRedirectOrder(IPriorityComparison a, IPriorityComparison b)
    {
        // 1. Priority comparison (higher values first)
        int priorityResult = b.Priority.CompareTo(a.Priority); // Reversed for descending
        if (priorityResult != 0) return priorityResult;

        // 2. Speed comparison (higher values first)
        int speedResult = b.Speed.CompareTo(a.Speed); // Reversed for descending
        if (speedResult != 0) return speedResult;

        // 3. Ability state effect order comparison (lower values first, only if both have ability states)
        // This is used to break ties between abilities that redirect moves
        if (a is EventListener aListener && b is EventListener bListener)
        {
            // Check if both handlers are from abilities
            bool aHasAbilityState = aListener.Effect.EffectType == EffectType.Ability &&
                                    aListener is { EffectHolder: PokemonEffectHolder, State: not null };
            bool bHasAbilityState = bListener.Effect.EffectType == EffectType.Ability &&
                                    bListener is { EffectHolder: PokemonEffectHolder, State: not null };

            if (aHasAbilityState && bHasAbilityState)
            {
                // Negative sign to reverse the order (lower effectOrder comes first)
                return -(bListener.State!.EffectOrder.CompareTo(aListener.State!.EffectOrder));
            }
        }

        return 0;
    }

    /// <summary>
    /// Compares two event handlers for left-to-right order.
    /// Used for processing actions in field position order (left to right).
    /// 
    /// Order:
    /// 1. Order (higher first) - reversed comparison, with false treated as maximum value
    /// 2. Priority (higher first)
    /// 3. Index (higher first) - reversed comparison for left-to-right processing
    /// </summary>
    public static int CompareLeftToRightOrder(IPriorityComparison a, IPriorityComparison b)
    {
        // 1. Order comparison (higher values first, but treating false as lowest priority)
        // The negative sign reverses the comparison: higher order values come first
        // IntFalseUnion.CompareTo handles this: false > int values (false has lower priority)
        int orderResult = -(b.Order.CompareTo(a.Order));
        if (orderResult != 0) return orderResult;

        // 2. Priority comparison (higher values first)
        int priorityResult = b.Priority.CompareTo(a.Priority); // Reversed for descending
        if (priorityResult != 0) return priorityResult;

        // 3. Index comparison (higher values first for left-to-right)
        // This processes Pokemon from left to right on the field
        // Both handlers need to be EventListeners with valid indices
        if (a is EventListener aListener && b is EventListener bListener)
        {
            int aIndex = aListener.Index ?? 0;
            int bIndex = bListener.Index ?? 0;
            int indexResult = -(bIndex.CompareTo(aIndex));
            if (indexResult != 0) return indexResult;
        }

        return 0;
    }

    /// <summary>
    /// Sort a list, resolving speed ties the way the games do.
    /// 
    /// This uses a Selection Sort algorithm - not the fastest sort in general, but
    /// actually faster than QuickSort for small arrays like the ones SpeedSort is used for.
    /// More importantly, it makes it easiest to resolve speed ties properly through
    /// randomization via Prng.Shuffle().
    /// </summary>
    /// <typeparam name="T">Value that implements IPriorityComparison for sorting</typeparam>
    /// <param name="list">List to sort in-place</param>
    /// <param name="comparator">Comparison function (defaults to ComparePriority)</param>
    public void SpeedSort<T>(List<T> list, Func<T, T, int>? comparator = null)
        where T : IPriorityComparison
    {
        // Default to ComparePriority if no comparator provided
        comparator ??= (a, b) => ComparePriority(a, b);

        // Nothing to sort for lists with less than 2 elements
        if (list.Count < 2) return;

        int sorted = 0;

        // Selection Sort with speed tie resolution
        while (sorted + 1 < list.Count)
        {
            // Start with the first unsorted element
            List<int> nextIndexes = [sorted];

            // Find all elements that should come next (including ties)
            for (int i = sorted + 1; i < list.Count; i++)
            {
                int delta = comparator(list[nextIndexes[0]], list[i]);

                switch (delta)
                {
                    case < 0:
                        // Current element is already better, skip
                        continue;
                    case > 0:
                        // Found a better element, start new list
                        nextIndexes = [i];
                        break;
                    // delta == 0
                    default:
                        // Speed tie - add to list of tied elements
                        nextIndexes.Add(i);
                        break;
                }
            }

            // Place the next elements in their sorted positions
            for (int i = 0; i < nextIndexes.Count; i++)
            {
                int index = nextIndexes[i];
                if (index != sorted + i)
                {
                    // Swap elements into place
                    // nextIndexes is guaranteed to be in order, so it will never have
                    // been disturbed by an earlier swap
                    (list[sorted + i], list[index]) = (list[index], list[sorted + i]);
                }
            }

            // If there are multiple elements with the same priority (speed ties),
            // shuffle them randomly to fairly resolve the tie
            if (nextIndexes.Count > 1)
            {
                Prng.Shuffle(list, sorted, sorted + nextIndexes.Count);
            }

            sorted += nextIndexes.Count;
        }
    }

    public EventListener ResolvePriority(EventListenerWithoutPriority h, EventId callbackName)
    {
        // Get event metadata from Library
        EventIdInfo eventInfo = Library.Events[callbackName];

        // Look up order/priority/subOrder from the effect using the delegate system
        // These would need to be added to IEffect interface or accessed via reflection once
        IntFalseUnion? order = h.Effect.GetOrder(callbackName);
        int? priority = h.Effect.GetPriority(callbackName);
        int? subOrder = h.Effect.GetSubOrder(callbackName);

        // Calculate default subOrder if not set
        if (subOrder == 0)
        {
            subOrder = CalculateDefaultSubOrder(h);
        }

        // Determine effectOrder based on event type
        int effectOrder = eventInfo.UsesEffectOrder
            ? (h.State?.EffectOrder ?? 0)
            : 0;

        // Calculate speed if needed
        int speed = 0;
        if (eventInfo.UsesSpeed && h.EffectHolder is PokemonEffectHolder pokemonEffectHolder)
        {
            Pokemon pokemon = pokemonEffectHolder.Pokemon;
            speed = pokemon.Speed;

            // Special case for Magic Bounce
            if (h.Effect.EffectType == EffectType.Ability &&
                h.Effect is Ability { Id: AbilityId.MagicBounce } &&
                callbackName == EventId.AllyTryHitSide)
            {
                speed = pokemon.GetStat(StatIdExceptHp.Spe, unmodified: true, unboosted: true);
            }

            // Apply fractional speed adjustment for switch-in events
            if (eventInfo.UsesFractionalSpeed)
            {
                int fieldPositionValue = pokemon.Side.N * Sides.Count + pokemon.Position;
                speed -= SpeedOrder.IndexOf(fieldPositionValue) / (ActivePerHalf * 2);
            }
        }

        return new EventListener
        {
            Effect = h.Effect,
            Target = h.Target,
            Index = h.Index,
            Callback = h.Callback,
            State = h.State,
            End = h.End,
            EndCallArgs = h.EndCallArgs,
            EffectHolder = h.EffectHolder,
            Order = order ?? IntFalseUnion.FromFalse(),
            Priority = priority ?? 0,
            Speed = speed,
            SubOrder = subOrder ?? 0,
            EffectOrder = effectOrder,
        };
    }

    public bool CheckMoveMakesContact(Move move, Pokemon attacker, Pokemon defender, bool announcePads = false)
    {
        if (move.Flags.Contact is not true || !attacker.HasItem(ItemId.ProtectivePads))
        {
            return move.Flags.Contact is true;
        }

        if (!announcePads) return false;

        if (DisplayUi)
        {
            Add("-activate", defender, PartFuncUnion.FromIEffect(Effect));
            Add("-activate", attacker, "item: Protective Pads");
        }

        return false;
    }

    /// <summary>
    /// Gets a Pokemon by its full name string.
    /// Searches through all sides and returns the first Pokemon with a matching fullname.
    /// </summary>
    /// <param name="fullname">The full name of the Pokemon to find</param>
    /// <returns>The Pokemon if found, otherwise null</returns>
    public Pokemon? GetPokemon(string fullname)
    {
        return Sides.SelectMany(side =>
            side.Pokemon.Where(pokemon => pokemon.Fullname == fullname)).FirstOrDefault();
    }

    /// <summary>
    /// Gets a Pokemon by another Pokemon's full name.
    /// This overload extracts the fullname from the provided Pokemon and searches for a match.
    /// </summary>
    /// <param name="pokemon">The Pokemon whose fullname to search for</param>
    /// <returns>The Pokemon if found, otherwise null</returns>
    public Pokemon? GetPokemon(Pokemon pokemon)
    {
        return GetPokemon(pokemon.Fullname);
    }

    /// <summary>
    /// Gets all Pokemon from all sides in the battle.
    /// </summary>
    /// <returns>A list containing all Pokemon from both sides</returns>
    public List<Pokemon> GetAllPokemon()
    {
        List<Pokemon> pokemonList = [];
        foreach (Side side in Sides)
        {
            pokemonList.AddRange(side.Pokemon);
        }
        return pokemonList;
    }

    public List<Pokemon> GetAllActive(bool includeFainted = false)
    {
        List<Pokemon> pokemnoList = [];
        foreach (Side side in Sides)
        {
            pokemnoList.AddRange(side.Active.Where(pokemon => includeFainted || !pokemon.Fainted));
        }
        return pokemnoList;
    }

    public void MakeRequest(RequestState? type = null)
    {
        // Update request state if provided, otherwise use current state
        if (type.HasValue)
        {
            RequestState = type.Value;

            // Clear all sides' choices when starting a new request
            foreach (Side side in Sides)
            {
                side.ClearChoice();
            }
        }
        else
        {
            type = RequestState;
        }

        // Clear all active requests before generating new ones
        foreach (Side side in Sides)
        {
            side.ActiveRequest = null;
        }

        // Add team preview message if applicable
        if (type.Value == RequestState.TeamPreview)
        {
            // `pickedTeamSize = 6` means the format wants the user to select
            // the entire team order, unlike `pickedTeamSize = null` which
            // will only ask the user to select their lead(s).
            int? pickedTeamSize = RuleTable.PickedTeamSize;

            if (DisplayUi)
            {
                if (pickedTeamSize.HasValue)
                {
                    Add("teampreview", pickedTeamSize.Value);
                }
                else
                {
                    Add("teampreview");
                }
            }
        }

        // Generate appropriate requests for the current state
        var requests = GetRequests(type.Value);

        // Assign requests to each side
        for (int i = 0; i < Sides.Count; i++)
        {
            Sides[i].ActiveRequest = requests[i];
        }

        // Verify that choices aren't already done (would indicate a bug)
        if (Sides.All(side => side.IsChoiceDone()))
        {
            throw new InvalidOperationException("Choices are done immediately after a request");
        }
    }

    /// <summary>
    /// Clears the current request state and resets all sides' active requests and choices.
    /// Called when a turn's decision phase is complete or when canceling pending requests.
    /// </summary>
    public void ClearRequest()
    {
        RequestState = RequestState.None;
        foreach (Side side in Sides)
        {
            side.ActiveRequest = null;
            side.ClearChoice();
        }
    }

    public List<IChoiceRequest> GetRequests(RequestState type)
    {
        // Default to no request (null for each side)
        var requests = new IChoiceRequest?[Sides.Count];

        switch (type)
        {
            case RequestState.SwitchIn:
                for (int i = 0; i < Sides.Count; i++)
                {
                    Side side = Sides[i];
                    if (side.PokemonLeft <= 0) continue;

                    // Create a table of which active Pokemon need to switch
                    // Convert MoveIdBoolUnion to bool using IsTrue() method
                    var switchTable = side.Active
                        .Select(pokemon => pokemon.SwitchFlag.IsTrue())
                        .ToList();

                    // Only create a switch request if at least one Pokemon needs to switch
                    if (switchTable.Any(flag => flag))
                    {
                        requests[i] = new SwitchRequest
                        {
                            ForceSwitch = switchTable,
                            Side = side.GetRequestData(),
                        };
                    }
                }
                break;

            case RequestState.TeamPreview:
                for (int i = 0; i < Sides.Count; i++)
                {
                    Side side = Sides[i];
                    int? maxChosenTeamSize = RuleTable.PickedTeamSize > 0
                        ? RuleTable.PickedTeamSize
                        : null;

                    requests[i] = new TeamPreviewRequest
                    {
                        MaxChosenTeamSize = maxChosenTeamSize,
                        Side = side.GetRequestData(),
                    };
                }
                break;

            default:
                // Regular move requests
                for (int i = 0; i < Sides.Count; i++)
                {
                    Side side = Sides[i];
                    if (side.PokemonLeft <= 0) continue;

                    // Get move request data for each active Pokemon
                    var activeData = side.Active
                        .Select(pokemon => pokemon.GetMoveRequestData())
                        .Where(_ => true)
                        .ToList();

                    var moveRequest = new MoveRequest
                    {
                        Active = activeData,
                        Side = side.GetRequestData()
                    };

                    requests[i] = moveRequest;
                }
                break;
        }

        // Check if multiple requests exist (multiple players need to make choices)
        bool multipleRequestsExist = requests.Count(r => r != null) >= 2;

        // Finalize all requests
        for (int i = 0; i < Sides.Count; i++)
        {
            if (requests[i] != null)
            {
                // Set noCancel if cancellation is not supported or only one player is choosing
                if (!SupportCancel || !multipleRequestsExist)
                {
                    requests[i] = requests[i] switch
                    {
                        SwitchRequest sr => sr with { NoCancel = true },
                        TeamPreviewRequest tpr => tpr with { NoCancel = true },
                        MoveRequest mr => mr with { NoCancel = true },
                        _ => requests[i],
                    };
                }
            }
            else
            {
                // Create a wait request for sides that don't need to make a choice
                requests[i] = new WaitRequest
                {
                    Side = Sides[i].GetRequestData(),
                };
            }
        }

        return requests.Where(r => r != null).Cast<IChoiceRequest>().ToList();
    }

    public bool Tiebreak()
    {
        if (Ended) return false;

        InputLog.Add("> tiebreak");

        if (DisplayUi)
        {
            Add("message", "Time's up! Going to tiebreaker...");
        }

        // Count non-fainted Pokemon for each side
        var notFainted = Sides.Select(side =>
            side.Pokemon.Count(pokemon => !pokemon.Fainted)
        ).ToList();

        // Display Pokemon count per side
        if (DisplayUi)
        {
            string pokemonCountMessage = string.Join("; ",
                Sides.Select((side, i) => $"{side.Name}: {notFainted[i]} Pokemon left")
            );
            Add("-message", pokemonCountMessage);
        }

        // Filter sides with maximum Pokemon count
        int maxNotFainted = notFainted.Max();
        var tiedSides = Sides.Where((_, i) => notFainted[i] == maxNotFainted).ToList();

        if (tiedSides.Count <= 1)
        {
            return Win(tiedSides.FirstOrDefault());
        }

        // Calculate HP percentages
        var hpPercentage = tiedSides.Select(side =>
            side.Pokemon.Sum(pokemon => (double)pokemon.Hp / pokemon.MaxHp) * 100 / 6
        ).ToList();

        // Display HP percentage per side
        if (DisplayUi)
        {
            string hpPercentageMessage = string.Join("; ",
                tiedSides.Select((side, i) => $"{side.Name}: {Math.Round(hpPercentage[i])}% total HP left")
            );
            Add("-message", hpPercentageMessage);
        }

        // Filter sides with maximum HP percentage
        double maxPercentage = hpPercentage.Max();
        tiedSides = tiedSides.Where((_, i) => Math.Abs(hpPercentage[i] - maxPercentage) < double.Epsilon).ToList();

        if (tiedSides.Count <= 1)
        {
            return Win(tiedSides.FirstOrDefault());
        }

        // Calculate total HP
        var hpTotal = tiedSides.Select(side =>
            side.Pokemon.Sum(pokemon => pokemon.Hp)
        ).ToList();

        // Display total HP per side
        if (DisplayUi)
        {
            string hpTotalMessage = string.Join("; ",
                tiedSides.Select((side, i) => $"{side.Name}: {hpTotal[i]} total HP left")
            );
            Add("-message", hpTotalMessage);
        }

        // Filter sides with maximum total HP
        int maxTotal = hpTotal.Max();
        tiedSides = tiedSides.Where((_, i) => hpTotal[i] == maxTotal).ToList();

        if (tiedSides.Count <= 1)
        {
            return Win(tiedSides.FirstOrDefault());
        }

        return Tie();
    }

    public bool ForceWin(SideId? side = null)
    {
        // Battle already ended - cannot force a win
        if (Ended) return false;

        // Log the force win/tie command to input log
        string logEntry = side.HasValue ? $"> forcewin {side.Value}" : "> forcetie";
        InputLog.Add(logEntry);

        // Delegate to the Win method to handle the actual logic
        return Win(side);
    }

    public bool Tie()
    {
        return Win((Side?)null);
    }

    public bool Win(SideId? side = null)
    {
        // Convert Side to Side if provided
        Side? winningSide = side.HasValue ? GetSide(side.Value) : null;
        return Win(winningSide);
    }

    public bool Win(Side? side = null)
    {
        // Battle already ended
        if (Ended) return false;

        // Validate the side exists in the battle
        if (side != null && !Sides.Contains(side))
        {
            side = null;
        }

        Winner = side?.Name ?? string.Empty;

        if (DisplayUi)
        {
            // Print empty line for formatting
            Add("");

            // Print the appropriate win/tie message
            // Note: AllySide is not implemented in this codebase (see Side class)
            // The original TypeScript code checks for side?.allySide here
            if (side != null)
            {
                // Single side wins
                Add("win", side.Name);
            }
            else
            {
                // Tie
                Add("tie");
            }
        }

        // End the battle
        Ended = true;
        RequestState = RequestState.None;

        // Clear all active requests
        foreach (Side s in Sides)
        {
            s.ActiveRequest = null;
        }

        return true;
    }

    public bool Lose(SideId sideId)
    {
        Side side = GetSide(sideId);
        return Lose(side);
    }

    public bool Lose(Side? side)
    {
        // Can happen if a battle crashes
        if (side is null) return false;
        
        // Already no Pokémon left
        if (side.PokemonLeft <= 0) return false;

        // Force the side to lose by setting their Pokémon count to 0
        side.PokemonLeft = 0;

        // Faint the first active Pokémon if present
        side.Active.FirstOrDefault()?.Faint();

        // Show faint messages (lastFirst: false, forceCheck: true)
        FaintMessages(lastFirst: false, forceCheck: true);

        // Update requests if battle hasn't ended and this side had an active request
        if (!Ended && side.ActiveRequest != null)
        {
            // Send a wait request
            side.EmitRequest(new WaitRequest
            {
                Side = side.GetRequestData(),
            });
            
            // Clear any pending choices
            side.ClearChoice();
            
            // Commit choices if all sides are done choosing
            if (AllChoicesDone())
            {
                CommitChoices();
            }
        }

        return true;
    }

    public int CanSwitch(Side side)
    {
        return PossibleSwitches(side).Count;
    }

    /// <summary>
    /// Gets a random Pokémon that can be switched in for the given side.
    /// Returns null if no Pokémon are available to switch in.
    /// </summary>
    /// <param name="side">The side to get a random switchable Pokémon for</param>
    /// <returns>A random switchable Pokémon, or null if none available</returns>
    public Pokemon? GetRandomSwitchable(Side side)
    {
        var canSwitchIn = PossibleSwitches(side);
        return canSwitchIn.Count > 0 ? Sample(canSwitchIn) : null;
    }

    /// <summary>
    /// Gets all Pokémon that can be switched in for the given side.
    /// Only includes non-fainted Pokémon that are not currently active.
    /// </summary>
    /// <param name="side">The side to get possible switches for</param>
    /// <returns>A list of all Pokémon that can be switched in</returns>
    private static List<Pokemon> PossibleSwitches(Side side)
    {
        // No Pokémon left on the side
        if (side.PokemonLeft <= 0) return [];

        List<Pokemon> canSwitchIn = [];
        
        // Iterate through Pokemon starting after the active slots
        // Active Pokemon are at indices [0, side.Active.Count)
        // Bench Pokemon are at indices [side.Active.Count, side.Pokemon.Count)
        for (int i = side.Active.Count; i < side.Pokemon.Count; i++)
        {
            Pokemon pokemon = side.Pokemon[i];
            if (!pokemon.Fainted)
            {
                canSwitchIn.Add(pokemon);
            }
        }
        
        return canSwitchIn;
    }

    /// <summary>
    /// Swaps a Pokémon's position with another Pokémon on the same side.
    /// Used for moves like Ally Switch or game mechanics that change field positions.
    /// </summary>
    /// <param name="pokemon">The Pokémon to swap</param>
    /// <param name="newPosition">The target position index (0-based)</param>
    /// <param name="attributes">Optional attributes for the swap event</param>
    /// <returns>True if swap succeeded, false if invalid</returns>
    public bool SwapPosition(Pokemon pokemon, int newPosition, string? attributes = null)
    {
        // Validate the new position is within the active slots
        if (newPosition >= pokemon.Side.Active.Count)
        {
            throw new ArgumentException("Invalid swap position", nameof(newPosition));
        }

        // Get the Pokémon at the target position
        Pokemon target = pokemon.Side.Active[newPosition];

        // Special check: position 1 can be swapped even if empty/fainted
        // Other positions require a valid, non-fainted target
        if (newPosition != 1 && target.Fainted)
        {
            return false;
        }

        // Log the swap event
        if (DisplayUi)
        {
            if (!string.IsNullOrEmpty(attributes))
            {
                Add("swap", pokemon, newPosition, attributes);
            }
            else
            {
                Add("swap", pokemon, newPosition, "");
            }
        }

        // Perform the swap
        Side side = pokemon.Side;

        // Swap in the Pokemon array (full team roster)
        side.Pokemon[pokemon.Position] = target;
        side.Pokemon[newPosition] = pokemon;

        // Swap in the Active array (currently active Pokemon)
        side.Active[pokemon.Position] = side.Pokemon[pokemon.Position];
        side.Active[newPosition] = side.Pokemon[newPosition];

        // Update position properties
        target.Position = pokemon.Position;
        pokemon.Position = newPosition;

        // Trigger swap events for both Pokemon
        RunEvent(EventId.Swap, target, RunEventSource.FromNullablePokemon(pokemon));
        RunEvent(EventId.Swap, pokemon, RunEventSource.FromNullablePokemon(target));

        return true;
    }

    public Pokemon? GetAtSlot(PokemonSlot? slot)
    {
        if (slot is null) return null;
        Side side = GetSide(slot.SideId);
        int position = (int)slot.PositionLetter;
        int positionOffset = (int)Math.Floor(side.N / 2.0) * side.Active.Count;
        return side.Active[position - positionOffset];
    }

    public void Faint(Pokemon pokemon, Pokemon? source = null, IEffect? effect = null)
    {
        pokemon.Faint(source, effect);
    }

    public void EndTurn()
    {
        // Increment turn counter and reset last successful move
        Turn++;
        LastSuccessfulMoveThisTurn = null;

        // Process each side
        var trappedBySide = new List<bool>();
        var stalenessBySide = new List<StalenessId?>();

        foreach (Side side in Sides)
        {
            bool sideTrapped = true;
            StalenessId? sideStaleness = null;

            foreach (Pokemon pokemon in side.Active)
            {
                // Reset move tracking
                pokemon.MoveThisTurn = false;
                pokemon.NewlySwitched = false;
                pokemon.MoveLastTurnResult = pokemon.MoveThisTurnResult;
                pokemon.MoveThisTurnResult = null;

                // Reset turn-specific flags (except on turn 1)
                if (Turn != 1)
                {
                    pokemon.UsedItemThisTurn = false;
                    pokemon.StatsRaisedThisTurn = false;
                    pokemon.StatsLoweredThisTurn = false;
                    // It shouldn't be possible in a normal battle for a Pokemon to be damaged before turn 1's move selection
                    // However, this could be potentially relevant in certain OMs
                    pokemon.HurtThisTurn = null;
                }

                // Reset move disable tracking
                pokemon.MaybeDisabled = false;
                pokemon.MaybeLocked = null;

                // Clear disabled flags on all move slots
                foreach (MoveSlot moveSlot in pokemon.MoveSlots)
                {
                    moveSlot.Disabled = false;
                    moveSlot.DisabledSource = null;
                }

                // Run DisableMove event to determine which moves should be disabled
                RunEvent(EventId.DisableMove, pokemon);

                // Check each move for specific disable conditions
                foreach (MoveSlot moveSlot in pokemon.MoveSlots)
                {
                    var activeMove = Library.Moves[moveSlot.Id].ToActiveMove();

                    // Run DisableMove event on the specific move
                    SingleEvent(EventId.DisableMove, activeMove, null, pokemon);

                    // Disable moves with "cantusetwice" flag if used last turn
                    if (activeMove.Flags.CantUseTwice == true &&
                        pokemon.LastMove?.Id == moveSlot.Id)
                    {
                        pokemon.DisableMove(pokemon.LastMove.Id);
                    }
                }

                // If it was an illusion, it's not any more (Gen 7+)
                if (pokemon.GetLastAttackedBy() != null)
                {
                    pokemon.KnownType = true;
                }

                // Clean up attack tracking
                for (int i = pokemon.AttackedBy.Count - 1; i >= 0; i--)
                {
                    Attacker attack = pokemon.AttackedBy[i];
                    if (attack.Source.IsActive)
                    {
                        // Mark attack as not from this turn (create new record since it's immutable)
                        pokemon.AttackedBy[i] = attack with { ThisTurn = false };
                    }
                    else
                    {
                        // Remove attacks from Pokemon that are no longer active
                        pokemon.AttackedBy.RemoveAt(i);
                    }
                }

                // Update apparent type display (not Terastallized)
                if (pokemon.Terastallized == null)
                {
                    // In Gen 7+, the real type of every Pokemon is visible to all players via the bottom screen while making choices
                    // Get the visible Pokemon (accounting for Illusion)
                    Pokemon seenPokemon = pokemon.Illusion ?? pokemon;

                    // Get actual types as a string (e.g., "Fire/Flying")
                    string realTypeString = string.Join("/",
                        seenPokemon.GetTypes(excludeAdded: true).Select(t => t.ToString()));

                    string currentApparentType = string.Join("/", seenPokemon.ApparentType);
                    if (realTypeString != currentApparentType)
                    {
                        // Update apparent type (this is for display purposes)
                        if (DisplayUi)
                        {
                            Add("-start", pokemon, "typechange", realTypeString, "[silent]");
                        }
                        seenPokemon.ApparentType = seenPokemon.GetTypes(excludeAdded: true).ToList();

                        if (pokemon.AddedType != null)
                        {
                            // The typechange message removes the added type, so put it back
                            if (DisplayUi)
                            {
                                Add("-start", pokemon, "typeadd", pokemon.AddedType?.ToString() ??
                                    throw new InvalidOperationException("Added type should not be null"),
                                    "[silent]");
                            }
                        }
                    }
                }

                // Reset trapping status
                pokemon.Trapped = PokemonTrapped.False;
                pokemon.MaybeTrapped = false;

                // Run trap events
                RunEvent(EventId.TrapPokemon, pokemon);

                // Canceling switches would leak information if a foe might have a trapping ability
                if (pokemon.KnownType || Dex.GetImmunity(ConditionId.Trapped, pokemon.Types))
                {
                    RunEvent(EventId.MaybeTrapPokemon, pokemon);
                }

                // Check foe abilities for potential trapping
                foreach (Pokemon source in pokemon.Foes())
                {
                    // Get the species to check (accounting for Illusion)
                    Species species = (source.Illusion ?? source).Species;

                    // Check each ability slot the species could have
                    foreach (SpeciesAbilityType abilitySlot in Enum.GetValues<SpeciesAbilityType>())
                    {
                        var abilityId = species.Abilities.GetAbility(abilitySlot);
                        if (abilityId == null) continue;

                        // Skip if this is the source's current ability (already checked above)
                        if (abilityId == source.Ability) continue;

                        // Get the ability
                        Ability ability = Library.Abilities[abilityId.Value];

                        // Check if ability is banned
                        if (RuleTable.Has(ability.Id)) continue;

                        // Skip immunity check if type is known and already not immune
                        if (pokemon.KnownType && !Dex.GetImmunity(ConditionId.Trapped, pokemon.Types))
                            continue;

                        // Run the FoeMaybeTrapPokemon event for this potential ability
                        SingleEvent(EventId.FoeMaybeTrapPokemon, ability, null, pokemon, source);
                    }
                }

                // Skip if Pokemon fainted
                if (pokemon.Fainted) continue;

                // Update side-wide trap status
                sideTrapped = sideTrapped && pokemon.Trapped == PokemonTrapped.True;

                // Update side-wide staleness
                StalenessId? staleness = pokemon.VolatileStaleness ?? pokemon.Staleness;
                if (staleness != null)
                {
                    // External staleness takes priority
                    sideStaleness = sideStaleness == StalenessId.External ? sideStaleness : staleness;
                }

                // Increment active turn counter
                pokemon.ActiveTurns++;
            }

            // Store trap and staleness status for this side
            trappedBySide.Add(sideTrapped);
            stalenessBySide.Add(sideStaleness);

            // Update fainted Pokemon tracking
            side.FaintedLastTurn = side.FaintedThisTurn;
            side.FaintedThisTurn = null;
        }

        // Check for endless battle clause
        if (MaybeTriggerEndlessBattleClause(trappedBySide, stalenessBySide))
        {
            return;
        }

        // Display turn number
        if (DisplayUi)
        {
            Add("turn", Turn);
        }

        // Pre-calculate Quick Claw roll for Gen 2-3 (skipped for Gen 9)
        // Gen 9 doesn't use Quick Claw rolls the same way

        // Request move choices for the new turn
        MakeRequest(RequestState.Move);
    }

    public bool MaybeTriggerEndlessBattleClause(List<bool> trappedBySide, List<StalenessId?> stalenessBySide)
    {
        // Skip if still under the 100 turn minimum
        if (Turn <= 100) return false;

        // Turn limit check (not part of Endless Battle Clause, but hard limit)
        if (Turn >= 1000)
        {
            if (DisplayUi)
            {
                Add("-message", "It is turn 1000. You have hit the turn limit!");
            }
            Tie();
            return true;
        }

        // Warning messages for approaching turn limit
        if ((Turn >= 500 && Turn % 100 == 0) || // Every 100 turns past turn 500
            (Turn >= 900 && Turn % 10 == 0) ||  // Every 10 turns past turn 900
            Turn >= 990)                         // Every turn past turn 990
        {
            if (DisplayUi)
            {
                int turnsLeft = 1000 - Turn;
                string turnsLeftText = turnsLeft == 1 ? "1 turn" : $"{turnsLeft} turns";
                Add("bigerror", $"You will auto-tie if the battle doesn't end in {turnsLeftText} (on turn 1000).");
            }
        }

        // Check if Endless Battle Clause rule is enabled
        if (!RuleTable.Has(RuleId.EndlessBattleClause)) return false;

        // Are all Pokemon on every side stale, with at least one side containing an externally stale Pokemon?
        if (!stalenessBySide.All(s => s.HasValue) ||
            stalenessBySide.All(s => s != StalenessId.External))
        {
            return false;
        }

        // Can both sides switch to a non-stale Pokemon?
        var canSwitch = new List<bool>();
        for (int i = 0; i < trappedBySide.Count; i++)
        {
            bool trapped = trappedBySide[i];
            canSwitch.Add(false);

            if (trapped) continue;

            Side side = Sides[i];

            foreach (Pokemon pokemon in side.Pokemon)
            {
                if (pokemon is not { Fainted: false, VolatileStaleness: null, Staleness: null }) continue;
                canSwitch[i] = true;
                break;
            }
        }

        // If both sides can switch to non-stale Pokemon, clause doesn't trigger
        if (canSwitch.All(s => s)) return false;

        // Endless Battle Clause activates - determine winner by checking for restorative berry cycling
        var losers = new List<Side>();

        foreach (Side side in Sides)
        {
            bool berry = false;  // Has restorative berry
            bool cycle = false;  // Has Harvest/Pickup ability or Recycle move

            foreach (Pokemon pokemon in side.Pokemon)
            {
                // Check if Pokemon has a restorative berry
                berry = Pokemon.RestorativeBerries.Contains(pokemon.Set.Item);

                // Check if Pokemon has cycling ability (Harvest/Pickup)
                if (pokemon.Set.Ability is AbilityId.Harvest or AbilityId.Pickup)
                {
                    cycle = true;
                }

                // Check if Pokemon has Recycle move
                if (pokemon.Set.Moves.Contains(MoveId.Recycle))
                {
                    cycle = true;
                }

                // If both conditions are met, this side loses
                if (berry && cycle) break;
            }

            if (berry && cycle)
            {
                losers.Add(side);
            }
        }

        // Determine outcome based on number of losing sides
        if (losers.Count == 1)
        {
            Side loser = losers[0];
            if (DisplayUi)
            {
                Add("-message",
                    $"{loser.Name}'s team started with the rudimentary means to perform " +
                    "restorative berry-cycling and thus loses.");
            }
            return Win(loser.Foe);
        }

        if (losers.Count == Sides.Count)
        {
            if (DisplayUi)
            {
                Add("-message",
                    "Each side's team started with the rudimentary means to perform " +
                    "restorative berry-cycling.");
            }
        }

        return Tie();
    }

    public void Start()
    {
        // Deserialized games should use Restart()
        if (Deserialized) return;

        // Need all players to start
        if (!Sides.All(_ => true))
        {
            throw new InvalidOperationException($"Missing sides.");
        }

        if (Started)
        {
            throw new InvalidOperationException("Battle already started");
        }

        Started = true;

        // Set up foe relationships (standard 1v1 or 2v2)
        Sides[1].Foe = Sides[0];
        Sides[0].Foe = Sides[1];

        // If there are more than 2 sides (FFA - free-for-all)
        if (Sides.Count > 2)
        {
            Sides[2].Foe = Sides[3];
            Sides[3].Foe = Sides[2];
        }

        if (DisplayUi)
        {
            // Log generation
            Add("gen", Gen);

            // Log tier
            Add("tier", Format.Name);

            // Log rated status
            if (Rated)
            {
                string ratedMessage = Rated ? "" : Rated.ToString();
                Add("rated", ratedMessage);
            }
        }

        // Call format's OnBegin handler
        Format.OnBegin?.Invoke(this);

        // Call OnBegin for each rule in the rule table
        foreach (Format subFormat in from rule in RuleTable.Keys
                 let ruleString = rule.ToString()
                 where ruleString.Length <= 0 || !"+*-!".Contains(ruleString[0])
                 select Library.Rulesets[rule])
        {
            subFormat.OnBegin?.Invoke(this);
        }

        // Validate that all sides have at least one Pokemon
        if (Sides.Any(side => side.Pokemon.Count == 0))
        {
            throw new InvalidOperationException("Battle not started: A player has an empty team.");
        }

        // Check EV balance in debug mode
        if (DebugMode)
        {
            CheckEvBalance();
        }

        // Run team preview/selection phase
        RunPickTeam();

        // Add start action to queue
        Queue.InsertChoice(new StartGameAction());

        // Set mid-turn flag
        MidTurn = true;

        // Start turn loop if no request is pending
        if (RequestState == RequestState.None)
        {
            TurnLoop();
        }
    }

    public void Restart(Action<string, List<string>>? send)
    {
        if (!Deserialized)
        {
            throw new InvalidOperationException("Attempt to restart a battle which has not been deserialized");
        }
        throw new Exception("Not sure what this is suppsed to do");
    }

    public void RunPickTeam()
    {
        // onTeamPreview handlers are expected to show full teams to all active sides,
        // and send a 'teampreview' request for players to pick their leads / team order.
        Format.OnTeamPreview?.Invoke(this);

        foreach (RuleId rule in RuleTable.Keys)
        {
            string ruleString = rule.ToString();
            if (ruleString.Length > 0 && "+*-!".Contains(ruleString[0])) continue;
            Format subFormat = Library.Rulesets[rule];
            subFormat.OnTeamPreview?.Invoke(this);
        }

        if (RequestState == RequestState.TeamPreview)
        {
            return;
        }

        if (RuleTable.PickedTeamSize > 0)
        {
            // There was no onTeamPreview handler (e.g. Team Preview rule missing).
            // Players must still pick their own Pokémon, so we show them privately.
            if (DisplayUi)
            {
                Add("clearpoke");
            }

            foreach (Pokemon pokemon in GetAllPokemon())
            {
                // Get the details object and convert to string
                PokemonDetails detailsObj = pokemon.Details;

                // Create a modified copy for display (hide certain formes)
                var maskedDetails = new PokemonDetails
                {
                    Id = MaskSpeciesForTeamPreview(detailsObj.Id),
                    Level = detailsObj.Level,
                    Gender = detailsObj.Gender,
                    Shiny = false, // Always hide shiny in team preview
                    TeraType = detailsObj.TeraType,
                };

                // Convert to protocol string
                string detailsString = maskedDetails.ToString();

                AddSplit(pokemon.Side.Id,
                    [
                        new StringPart("poke"),
                    new StringPart(pokemon.Side.Id.ToString()),
                    new StringPart(detailsString),
                    new StringPart(""),
                    ]);
            }

            MakeRequest(RequestState.TeamPreview);
        }
    }

    public void CheckEvBalance()
    {
        if (!DisplayUi) return;

        bool? limitedEVs = null;

        foreach (Side side in Sides)
        {
            // Check if this side's Pokémon all have 510 or fewer total EVs
            bool sideLimitedEVs = !side.Pokemon.Any(pokemon =>
            {
                // Sum all EV values for this Pokémon
                int totalEvs = pokemon.Set.Evs.Values.Sum();
                return totalEvs > 510;
            });

            if (limitedEVs == null)
            {
                // First side - just record the limit status
                limitedEVs = sideLimitedEVs;
            }
            else if (limitedEVs != sideLimitedEVs)
            {
                // Sides have different EV limit adherence - show warning
                Add("bigerror", "Warning: One player isn't adhering to a 510 EV limit, and the other player is.");
            }
        }
    }

    /// <summary>
    /// Modifies a Pokémon's stat stages (boosts) during battle.
    /// 
    /// Process:
    /// 1. Validates the target has HP and is active
    /// 2. Runs ChangeBoost and TryBoost events for modification
    /// 3. Applies boosts via target.BoostBy() for each stat
    /// 4. Logs boost messages to battle log
    /// 5. Triggers AfterEachBoost and AfterBoost events
    /// 6. Updates statsRaisedThisTurn/statsLoweredThisTurn flags
    /// 
    /// Returns:
    /// - null if boost succeeded
    /// - 0 if target has no HP
    /// - false if target is inactive or no foes remain (Gen 9)
    /// </summary>
    public BoolZeroUnion? Boost(SparseBoostsTable boost, Pokemon? target = null, Pokemon? source = null,
        IEffect? effect = null, bool isSecondary = false, bool isSelf = false)
    {
        if (target is null && Event.Target is PokemonSingleEventTarget eventTarget)
        {
            target = eventTarget.Pokemon;
        }
        if (source is null && Event.Source is PokemonSingleEventSource eventSource)
        {
            source = eventSource.Pokemon;
        }
        effect ??= Event.Effect;

        // Validate target has HP
        if (target?.Hp <= 0) return new ZeroBoolZeroUnion();

        // Validate target is active
        if (!(target?.IsActive ?? false)) return new BoolBoolZeroUnion(false);

        // Gen 9: Check if any foes remain
        if (target.Side.FoePokemonLeft() <= 0) return new BoolBoolZeroUnion(false);

        // Run ChangeBoost event to allow modifications
        RelayVar modifiedBoost = RunEvent(EventId.ChangeBoost, target,
            RunEventSource.FromNullablePokemon(source), effect, boost) ?? boost;

        if (modifiedBoost is not SparseBoostsTableRelayVar modifiedBoostTable)
        {
            throw new InvalidOperationException("ChangeBoost event did not return a valid SparseBoostsTable.");
        }

        // Cap the boosts to valid ranges (-6 to +6)
        SparseBoostsTable cappedBoost = target.GetCappedBoost(modifiedBoostTable.Table);

        // Run TryBoost event to allow prevention
        RelayVar finalBoost = RunEvent(EventId.TryBoost, target, RunEventSource.FromNullablePokemon(source),
                                   effect, cappedBoost) ?? cappedBoost;

        if (finalBoost is not SparseBoostsTableRelayVar finalBoostTable)
        {
            throw new InvalidOperationException("TryBoost event did not return a valid SparseBoostsTable.");
        }

        bool? success = null;
        bool boosted = isSecondary;

        // Apply each boost
        foreach (BoostId boostId in Enum.GetValues<BoostId>())
        {
            int? boostValue = finalBoostTable.Table.GetBoost(boostId);
            if (!boostValue.HasValue) continue;

            // Create a sparse table for just this stat
            var currentBoost = new SparseBoostsTable();
            currentBoost.SetBoost(boostId, boostValue.Value);

            // Apply the boost and get the actual change
            int boostBy = target.BoostBy(currentBoost);

            // Determine message type
            string msg = "-boost";
            if (boostValue.Value < 0 || target.Boosts.GetBoost(boostId) == -6)
            {
                msg = "-unboost";
                boostBy = -boostBy;
            }

            if (boostBy != 0)
            {
                success = true;

                // Handle special cases
                EffectStateId effectId = effect?.EffectStateId ?? EffectStateId.FromEmpty();

                if (DisplayUi)
                {
                    if (effectId is MoveEffectStateId { MoveId: MoveId.BellyDrum } or
                        AbilityEffectStateId { AbilityId: AbilityId.AngerPoint })
                    {
                        // Use -setboost for moves that set boosts to maximum
                        Add("-setboost", target, boostId.ConvertToString(),
                            target.Boosts.GetBoost(boostId),
                            "[from]", PartFuncUnion.FromIEffect(effect!));
                    }
                    else if (effect is not null)
                    {
                        switch (effect.EffectType)
                        {
                            case EffectType.Move:
                                Add(msg, target, boostId.ConvertToString(), boostBy);
                                break;

                            case EffectType.Item:
                                Add(msg, target, boostId.ConvertToString(), boostBy,
                                    "[from]", $"item: {effect.Name}");
                                break;

                            default:
                                if (effect.EffectType == EffectType.Ability && !boosted)
                                {
                                    Add("-ability", target, PartFuncUnion.FromIEffect(effect), "boost");
                                    boosted = true;
                                }
                                Add(msg, target, boostId.ConvertToString(), boostBy);
                                break;
                        }
                    }
                }

                // Trigger AfterEachBoost event
                RunEvent(EventId.AfterEachBoost, target, RunEventSource.FromNullablePokemon(source), effect,
                    currentBoost);
            }
            else if (effect?.EffectType == EffectType.Ability)
            {
                if (DisplayUi && (isSecondary || isSelf))
                {
                    Add(msg, target, boostId.ConvertToString(), boostBy);
                }
            }
            else if (!isSecondary && !isSelf)
            {
                if (DisplayUi)
                {
                    Add(msg, target, boostId.ConvertToString(), boostBy);
                }
            }
        }

        // Trigger AfterBoost event
        RunEvent(EventId.AfterBoost, target, RunEventSource.FromNullablePokemon(source), effect, finalBoost);

        // Update turn flags
        if (success == true)
        {
            // Check if any boosts were positive or negative
            bool hasPositiveBoost = false;
            bool hasNegativeBoost = false;

            foreach (BoostId boostId in Enum.GetValues<BoostId>())
            {
                int? boostValue = finalBoostTable.Table.GetBoost(boostId);
                switch (boostValue)
                {
                    case null:
                        continue;
                    case > 0:
                        hasPositiveBoost = true;
                        break;
                    case < 0:
                        hasNegativeBoost = true;
                        break;
                }
            }

            if (hasPositiveBoost) target.StatsRaisedThisTurn = true;
            if (hasNegativeBoost) target.StatsLoweredThisTurn = true;
        }

        return success.HasValue ? new BoolBoolZeroUnion(success.Value) : null;
    }

    public SpreadMoveDamage SpreadDamage(SpreadMoveDamage damage, SpreadMoveTargets? targetArray = null,
        Pokemon? source = null, BattleDamageEffect? effect = null, bool instaFaint = false)
    {
        // Return early if no targets
        if (targetArray == null) return [0];

        var retVals = new SpreadMoveDamage();

        // Convert effect to Condition
        Condition? effectCondition;
        switch (effect)
        {
            case EffectBattleDamageEffect ebde:
                effectCondition = ebde.Effect as Condition;
                break;
            case DrainBattleDamageEffect:
                effectCondition = Library.Conditions[ConditionId.Drain];
                break;
            case RecoilBattleDamageEffect:
                effectCondition = Library.Conditions[ConditionId.Recoil];
                break;
            case null:
                effectCondition = null;
                break;
            default:
                throw new InvalidOperationException("Unknown BattleDamageEffect type.");
        }

        // Process each target
        for (int i = 0; i < damage.Count; i++)
        {
            SpreadMoveDamage curDamage = new([damage[i]]);

            // Extract Pokemon from union type
            Pokemon? target = targetArray[i] switch
            {
                PokemonPokemonUnion p => p.Pokemon,
                _ => null,
            };

            int targetDamage = curDamage[0].ToInt();

            // Handle undefined damage values
            if (curDamage[0] is UndefinedBoolIntUndefinedUnion)
            {
                retVals.Add(curDamage[0]);
                continue;
            }

            // Target has no HP - return 0
            if (target is not { Hp: > 0 })
            {
                retVals.Add(0);
                continue;
            }

            // Target is not active - return false
            if (!target.IsActive)
            {
                retVals.Add(false);
                continue;
            }

            // Clamp damage to minimum of 1 (if non-zero)
            if (targetDamage != 0)
                targetDamage = ClampIntRange(targetDamage, 1, null);

            // Run Damage event unless this is struggle recoil
            if (effectCondition?.Id != ConditionId.StruggleRecoil)
            {
                // Check weather immunity
                if (effectCondition?.EffectType == EffectType.Weather &&
                    !target.RunStatusImmunity(effectCondition.Id))
                {
                    Debug("weather immunity");
                    retVals.Add(0);
                    continue;
                }

                // Run Damage event
                RelayVar? damageResult = RunEvent(
                    EventId.Damage,
                    target,
                    RunEventSource.FromNullablePokemon(source),
                    effectCondition,
                    new IntRelayVar(targetDamage)
                );

                if (damageResult is not IntRelayVar damageInt)
                {
                    Debug("damage event failed");
                    retVals.Add(new Undefined());
                    continue;
                }

                targetDamage = damageInt.Value;
            }

            // Clamp damage again after events
            if (targetDamage != 0)
                targetDamage = ClampIntRange(targetDamage, 1, null);

            // Apply damage to target
            targetDamage = target.Damage(targetDamage, source, effectCondition);
            retVals.Add(targetDamage);

            // Track that the Pokemon was hurt this turn
            if (targetDamage != 0)
                target.HurtThisTurn = target.Hp;

            // Track source's last damage if this was a move
            if (source != null && effectCondition?.EffectType == EffectType.Move)
                source.LastDamage = targetDamage;

            // Log damage messages
            PrintDamageMessage(target, source, effectCondition);

            // Handle drain for moves (Gen 9 uses rounding)
            if (effect is EffectBattleDamageEffect { Effect: ActiveMove move })
            {
                if (targetDamage > 0 && effectCondition?.EffectType == EffectType.Move &&
                    move.Drain != null && source != null)
                {
                    int drainAmount = Trunc(Math.Round(targetDamage * move.Drain.Value.Item1 /
                                                       (double)move.Drain.Value.Item2));
                    Heal(drainAmount, source, target, new DrainBattleHealEffect());
                }
            }
        }

        // Handle instafaint if requested
        if (instaFaint)
        {
            for (int i = 0; i < targetArray.Count; i++)
            {
                if (retVals[i] is UndefinedBoolIntUndefinedUnion || retVals[i] == false)
                    continue;

                Pokemon? target = targetArray[i] switch
                {
                    PokemonPokemonUnion p => p.Pokemon,
                    _ => null,
                };

                if (!(target?.Hp <= 0)) continue;

                Debug($"instafaint: {string.Join(", ", FaintQueue.Select(entry => entry.Target.Name))}");
                FaintMessages(lastFirst: true);
            }
        }

        return retVals;
    }

    /// <summary>
    /// Applies damage to a single Pokémon.
    /// This is a convenience wrapper around SpreadDamage for single-target damage.
    /// </summary>
    /// <param name="damage">Amount of damage to deal</param>
    /// <param name="target">Target Pokémon (defaults to event target)</param>
    /// <param name="source">Source Pokémon causing the damage (defaults to event source)</param>
    /// <param name="effect">Effect causing the damage (defaults to current effect)</param>
    /// <param name="instafaint">If true, immediately processes fainting instead of queueing it</param>
    /// <returns>
    /// - The actual damage dealt (as int) if successful
    /// - 0 if target has no HP
    /// - false if target is not active
    /// - null if damage was prevented by an event
    /// </returns>
    public IntFalseUndefinedUnion Damage(int damage, Pokemon? target = null, Pokemon? source = null,
        BattleDamageEffect? effect = null, bool instafaint = false)
    {
        // Default target to event target if available
        if (target == null && Event.Target is PokemonSingleEventTarget eventTarget)
        {
            target = eventTarget.Pokemon;
        }

        // Default source to event source if available
        if (source == null && Event.Source is PokemonSingleEventSource eventSource)
        {
            source = eventSource.Pokemon;
        }

        // Default effect to current effect if available
        effect ??= BattleDamageEffect.FromIEffect(Effect);

        // Create single-element arrays for SpreadDamage
        var damageArray = new SpreadMoveDamage { damage };
        var targetArray = new SpreadMoveTargets
        {
            target ?? throw new InvalidOperationException(),
        };

        // Call SpreadDamage and return the first (only) result
        SpreadMoveDamage results = SpreadDamage(damageArray, targetArray, source, effect, instafaint);
        return results[0].ToIntFalseUndefinedUnion();
    }

    /// <summary>
    /// Applies damage directly to a Pokémon without triggering the Damage event.
    /// Used for recoil damage, struggle damage, confusion damage, and other effects
    /// that should bypass normal damage modification abilities/items.
    /// </summary>
    /// <param name="damage">Amount of damage to deal</param>
    /// <param name="target">Target Pokémon (defaults to event target)</param>
    /// <param name="source">Source Pokémon causing the damage (defaults to event source)</param>
    /// <param name="effect">Effect causing the damage (defaults to current effect)</param>
    /// <returns>The actual amount of damage dealt (0 if target has no HP or damage was 0)</returns>
    public int DirectDamage(int damage, Pokemon? target = null, Pokemon? source = null, IEffect? effect = null)
    {
        // Default target to event target if available
        if (target == null && Event.Target is PokemonSingleEventTarget eventTarget)
        {
            target = eventTarget.Pokemon;
        }

        // Default source to event source if available
        if (source == null && Event.Source is PokemonSingleEventSource eventSource)
        {
            source = eventSource.Pokemon;
        }

        // Default effect to current effect if available
        effect ??= Effect;

        // Return 0 if target has no HP
        if (target?.Hp <= 0) return 0;

        // Return 0 if no damage to deal
        if (damage == 0) return 0;

        // Clamp damage to minimum of 1
        damage = ClampIntRange(damage, 1, null);

        // Apply damage directly (bypasses Damage event)
        if (target == null) return damage;

        damage = target.Damage(damage, source, effect);

        // Log the damage with special cases for certain effects
        if (DisplayUi)
        {
            if (effect is Condition condition)
            {
                switch (condition.Id)
                {
                    case ConditionId.StruggleRecoil:
                        // Struggle recoil has special messaging
                        Add("-damage", target, target.GetHealth, "[from] recoil");
                        break;

                    case ConditionId.Confusion:
                        // Confusion damage has special messaging
                        Add("-damage", target, target.GetHealth, "[from] confusion");
                        break;

                    default:
                        // Regular direct damage message
                        Add("-damage", target, target.GetHealth);
                        break;
                }
            }
            else
            {
                // No effect or non-condition effect - simple damage message
                Add("-damage", target, target.GetHealth);
            }
        }

        // If target fainted from the damage, trigger faint immediately
        if (target.Fainted)
        {
            Faint(target);
        }

        return damage;
    }

    public IntFalseUnion Heal(int damage, Pokemon? target = null, Pokemon? source = null,
        BattleHealEffect? effect = null)
    {
        // Default target to event target if available
        if (target == null && Event.Target is PokemonSingleEventTarget eventTarget)
        {
            target = eventTarget.Pokemon;
        }

        // Default source to event source if available
        if (source == null && Event.Source is PokemonSingleEventSource eventSource)
        {
            source = eventSource.Pokemon;
        }

        // Convert BattleHealEffect to Condition
        Condition? effectCondition = effect switch
        {
            DrainBattleHealEffect => Library.Conditions[ConditionId.Drain],
            EffectBattleHealEffect ebhe => ebhe.Effect as Condition,
            null => Effect as Condition,
            _ => throw new InvalidOperationException("Unknown BattleHealEffect type."),
        };

        // Clamp damage to minimum of 1 if non-zero
        if (damage != 0 && damage <= 1)
        {
            damage = 1;
        }

        // Truncate damage
        damage = Trunc(damage);

        // Run TryHeal event (allows effects like Liquid Ooze to trigger even when nothing is healed)
        RelayVar? tryHealResult = RunEvent(
            EventId.TryHeal,
            RunEventTarget.FromNullablePokemon(target),
            RunEventSource.FromNullablePokemon(source),
            effectCondition,
            new IntRelayVar(damage)
        );

        // If event prevented healing, return the result
        if (tryHealResult is not IntRelayVar healAmount)
        {
            return new IntIntFalseUnion(0);
        }

        if (healAmount.Value == 0)
        {
            return new IntIntFalseUnion(0);
        }

        damage = healAmount.Value;

        // Return false if target has no HP
        if (target?.Hp <= 0)
        {
            return new FalseIntFalseUnion();
        }

        // Return false if target is not active
        if (target is { IsActive: false })
        {
            return new FalseIntFalseUnion();
        }

        // Return false if target is already at max HP
        if (target != null && target.Hp >= target.MaxHp)
        {
            return new FalseIntFalseUnion();
        }

        if (target is null)
        {
            throw new InvalidOperationException("Target Pokémon is null.");
        }

        // Apply healing to target
        int finalDamage = target.Heal(damage, source, effectCondition).ToInt();

        // Log healing messages based on effect type
        PrintHealMessage(target, source, effectCondition);

        // Run Heal event
        RunEvent(
            EventId.Heal,
            target,
            RunEventSource.FromNullablePokemon(source),
            effectCondition,
            new IntRelayVar(finalDamage)
        );

        return new IntIntFalseUnion(finalDamage);
    }

    /// <summary>
    /// Chains two damage modifiers together using fixed-point arithmetic.
    /// This is used for combining multiple modifiers (STAB, type effectiveness, abilities, etc.)
    /// in the damage calculation formula.
    /// </summary>
    /// <param name="previousMod">Previous modifier (number or [numerator, denominator])</param>
    /// <param name="nextMod">Next modifier to chain (number or [numerator, denominator])</param>
    /// <returns>The combined modifier as a decimal value</returns>
    public double Chain(double previousMod, double nextMod)
    {
        // Convert to fixed-point (4096-based)
        int prevFixed = Trunc((int)(previousMod * 4096));
        int nextFixed = Trunc((int)(nextMod * 4096));

        // Apply chaining formula: M'' = ((M * M') + 0x800) >> 12
        // The 0x800 (2048) is added for proper rounding
        return ((prevFixed * nextFixed + 2048) >> 12) / 4096.0;
    }

    public double Chain(int[] previousMod, double nextMod)
    {
        if (previousMod.Length != 2)
        {
            throw new ArgumentException("Fraction array must have exactly 2 elements [numerator, denominator]",
                nameof(previousMod));
        }

        // Convert fraction to fixed-point
        int prevFixed = Trunc(previousMod[0] * 4096 / previousMod[1]);
        int nextFixed = Trunc((int)(nextMod * 4096));

        return ((prevFixed * nextFixed + 2048) >> 12) / 4096.0;
    }

    public double Chain(double previousMod, int[] nextMod)
    {
        if (nextMod.Length != 2)
        {
            throw new ArgumentException("Fraction array must have exactly 2 elements [numerator, denominator]",
                nameof(nextMod));
        }

        int prevFixed = Trunc((int)(previousMod * 4096));
        int nextFixed = Trunc(nextMod[0] * 4096 / nextMod[1]);

        return ((prevFixed * nextFixed + 2048) >> 12) / 4096.0;
    }

    public double Chain(int[] previousMod, int[] nextMod)
    {
        if (previousMod.Length != 2)
        {
            throw new ArgumentException("Fraction array must have exactly 2 elements [numerator, denominator]",
                nameof(previousMod));
        }
        if (nextMod.Length != 2)
        {
            throw new ArgumentException("Fraction array must have exactly 2 elements [numerator, denominator]",
                nameof(nextMod));
        }

        int prevFixed = Trunc(previousMod[0] * 4096 / previousMod[1]);
        int nextFixed = Trunc(nextMod[0] * 4096 / nextMod[1]);

        return ((prevFixed * nextFixed + 2048) >> 12) / 4096.0;
    }

    public double ChainModify(int numerator, int denominator = 1)
    {
        // Get the current modifier from the event state as fixed-point
        // Default to 1.0 (4096 in fixed-point) if null
        int previousMod = Trunc((int)((Event.Modifier ?? 1.0) * 4096));

        // Convert the new modifier to fixed-point format
        int nextMod = Trunc(numerator * 4096 / denominator);

        // Chain the modifiers together and store back in the event
        // The >> 12 is a right shift by 12 bits (equivalent to dividing by 4096)
        // Add 2048 for proper rounding before the shift
        Event.Modifier = ((previousMod * nextMod + 2048) >> 12) / 4096.0;
        return nextMod;
    }

    public double ChainModify(int[] fraction)
    {
        if (fraction.Length != 2)
        {
            throw new ArgumentException("Fraction array must have exactly 2 elements [numerator, denominator]", nameof(fraction));
        }

        return ChainModify(fraction[0], fraction[1]);
    }

    public double ChainModify(double fraction)
    {
        if (fraction <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(fraction), "Fraction must be greater than 0.");
        }
        // Convert the double fraction to a fixed-point representation
        int fixedPointFraction = Trunc((int)(fraction * 4096));
        // Chain the fixed-point modification
        int previousMod = Trunc((int)((Event.Modifier ?? 1.0) * 4096));
        Event.Modifier = ((previousMod * fixedPointFraction + 2048) >> 12) / 4096.0;
        return fixedPointFraction;
    }

    public int Modify(int value, int numerator, int denominator = 1)
    {
        // Calculate the 4096-based fixed-point modifier
        int modifier = Trunc(numerator * 4096 / denominator);

        // Apply the modifier with proper rounding
        return Trunc((Trunc(value * modifier) + 2048 - 1) / 4096);
    }

    public int Modify(int value, int[] fraction)
    {
        if (fraction.Length != 2)
        {
            throw new ArgumentException("Fraction array must have exactly 2 elements [numerator, denominator]",
                nameof(fraction));
        }

        return Modify(value, fraction[0], fraction[1]);
    }

    public int Modify(int value, double fraction)
    {
        if (fraction <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(fraction), "Fraction must be greater than 0.");
        }
        // Convert the double fraction to a fixed-point representation
        int fixedPointFraction = Trunc((int)(fraction * 4096));
        // Apply the fixed-point modification
        return Trunc((Trunc(value * fixedPointFraction) + 2048 - 1) / 4096);
    }

    public StatsTable SpreadModify(StatsTable baseStats, PokemonSet set)
    {
        StatsTable modStats = new();

        // iterate through all stats in baseStats
        foreach (StatId statName in baseStats.Keys)
        {
            modStats[statName] = StatModify(baseStats, set, statName);
        }
        return modStats;
    }

    /// <summary>
    /// Calculate a single stat value using Pokemon's official stat calculation formula.
    /// HP uses: floor(floor(2 * base + IV + floor(EV/4) + 100) * level / 100 + 10)
    /// Other stats use: floor(floor(2 * base + IV + floor(EV/4)) * level / 100 + 5)
    /// Then nature modifiers are applied with 16-bit truncation.
    /// </summary>
    public int StatModify(StatsTable baseStats, PokemonSet set, StatId statName)
    {
        int stat = baseStats.GetStat(statName);
        int iv = set.Ivs.GetStat(statName);
        int ev = set.Evs.GetStat(statName);

        // HP calculation uses a different formula
        if (statName == StatId.Hp)
        {
            // HP = floor(floor(2 * base + IV + floor(EV/4) + 100) * level / 100 + 10)
            return Trunc(Trunc(2 * stat + iv + Trunc(ev / 4) + 100) * set.Level / 100 + 10);
        }

        // Other stats: floor(floor(2 * base + IV + floor(EV/4)) * level / 100 + 5)
        stat = Trunc(Trunc(2 * stat + iv + Trunc(ev / 4)) * set.Level / 100 + 5);

        // Apply nature modifiers
        Nature nature = set.Nature;

        // Natures are calculated with 16-bit truncation
        // This only affects Eternatus-Eternamax in Pure Hackmons
        if (nature.Plus == statName.ConvertToStatIdExceptId())
        {
            // Positive nature: multiply by 1.1 (110/100)
            // Overflow protection: cap at 595 if rule is enabled
            stat = RuleTable.Has(RuleId.OverflowStatMod) ? Math.Min(stat, 595) : stat;
            stat = Trunc(Trunc(stat * 110, 16) / 100);
        }
        else if (nature.Minus == statName.ConvertToStatIdExceptId())
        {
            // Negative nature: multiply by 0.9 (90/100)
            // Overflow protection: cap at 728 if rule is enabled
            stat = RuleTable.Has(RuleId.OverflowStatMod) ? Math.Min(stat, 728) : stat;
            stat = Trunc(Trunc(stat * 90, 16) / 100);
        }
        return stat;
    }

    public int FinalModify(int relayVar)
    {
        relayVar = Modify(relayVar, Event.Modifier ?? 1.0);
        Event.Modifier = 1.0;
        return relayVar;
    }

    public MoveCategory GetCategory(ActiveMove move)
    {
        return move.Category;
    }

    public int Randomizer(int baseDamage)
    {
        return Trunc(Trunc(baseDamage * (100 - Random(16))) / 100);
    }

    /// <summary>
    /// Returns whether a proposed target location for a move is valid.
    /// Validates targeting based on move type, battlefield positions, and adjacency rules.
    /// </summary>
    /// <param name="targetLoc">The target location (0 = no target, positive = foe, negative = ally)</param>
    /// <param name="source">The Pokémon using the move</param>
    /// <param name="targetType">The move's targeting type (e.g., Normal, AdjacentAlly, Any)</param>
    /// <returns>True if the target location is valid for this move, false otherwise</returns>
    public bool ValidTargetLoc(int targetLoc, Pokemon source, MoveTarget targetType)
    {
        // targetLoc of 0 is always valid (no specific target)
        if (targetLoc == 0) return true;

        // Get the number of active slots per side
        int numSlots = ActivePerHalf;

        // Get the source's relative location
        int sourceLoc = source.GetLocOf(source);

        // Target location must be within valid range
        if (Math.Abs(targetLoc) > numSlots) return false;

        // Determine if targeting self
        bool isSelf = sourceLoc == targetLoc;

        // Determine if targeting a foe (positive locations are foes in team battles)
        bool isFoe = targetLoc > 0;

        // Calculate the location directly across from the target
        int acrossFromTargetLoc = -(numSlots + 1 - targetLoc);

        // Determine if the target is adjacent to the source
        bool isAdjacent;
        if (targetLoc > 0)
        {
            // For foes: check if the across-field position is within 1 slot
            isAdjacent = Math.Abs(acrossFromTargetLoc - sourceLoc) <= 1;
        }
        else
        {
            // For allies: check if positions differ by exactly 1
            isAdjacent = Math.Abs(targetLoc - sourceLoc) == 1;
        }

        // Check validity based on move's target type
        return targetType switch
        {
            // Normal moves target any adjacent Pokémon
            MoveTarget.RandomNormal or
            MoveTarget.Scripted or
            MoveTarget.Normal => isAdjacent,

            // Ally-only moves
            MoveTarget.AdjacentAlly => isAdjacent && !isFoe,

            // Ally or self
            MoveTarget.AdjacentAllyOrSelf => isAdjacent && !isFoe || isSelf,

            // Foe-only moves
            MoveTarget.AdjacentFoe => isAdjacent && isFoe,

            // Any target except self
            MoveTarget.Any => !isSelf,

            // Self-targeting moves
            MoveTarget.Self => isSelf,

            // Unknown target type - invalid
            _ => false,
        };
    }

    public bool ValidTarget(Pokemon target, Pokemon source, MoveTarget targetType)
    {
        return ValidTargetLoc(source.GetLocOf(target), source, targetType);
    }

    public Pokemon? GetTarget(Pokemon pokemon, MoveId moveId, int targetLoc, Pokemon? originalTarget = null)
    {
        Move move = Library.Moves[moveId];
        return GetTarget(pokemon, move, targetLoc, originalTarget);
    }

    public Pokemon? GetTarget(Pokemon pokemon, Move move, int targetLoc, Pokemon? originalTarget = null)
    {
        bool tracksTarget = move.TracksTarget ?? false;

        // Stalwart and Propeller Tail abilities set tracksTarget in ModifyMove, 
        // but ModifyMove happens after getTarget, so we need to manually check for them here
        if (pokemon.HasAbility([AbilityId.Stalwart, AbilityId.PropellerTail]))
        {
            tracksTarget = true;
        }

        // If move tracks target and original target is still active, target it
        if (tracksTarget && originalTarget?.IsActive == true)
        {
            return originalTarget;
        }

        // Smart target moves (like Dragon Darts) intelligently retarget
        // banning Dragon Darts from directly targeting itself is done in side.ts, but
        // Dragon Darts can target itself if Ally Switch is used afterwards
        if (move.SmartTarget == true)
        {
            Pokemon curTarget = pokemon.GetAtLoc(targetLoc);
            return curTarget is { Fainted: false } ? curTarget : GetRandomTarget(pokemon, move);
        }

        // Fails if the target is the user and the move can't target its own position
        int selfLoc = pokemon.GetLocOf(pokemon);

        // Check if move is trying to target self when it shouldn't
        if ((move.Target is MoveTarget.AdjacentAlly or MoveTarget.Any or MoveTarget.Normal) &&
            targetLoc == selfLoc &&
            !pokemon.Volatiles.ContainsKey(ConditionId.LockedMove) && // Two-turn move (e.g., Fly, Dig)
            !pokemon.Volatiles.ContainsKey(ConditionId.IceBall) &&
            !pokemon.Volatiles.ContainsKey(ConditionId.Rollout))
        {
            // Future moves can target the user
            return (move.Flags.FutureMove == true) ? pokemon : null;
        }

        // Check if target location is valid
        if (move.Target != MoveTarget.RandomNormal && ValidTargetLoc(targetLoc, pokemon, move.Target))
        {
            Pokemon target = pokemon.GetAtLoc(targetLoc);

            // Handle fainted targets
            if (target.Fainted)
            {
                // Check if target is an ally
                if (target.IsAlly(pokemon))
                {
                    // Gen 5: AdjacentAllyOrSelf moves retarget to user when ally faints
                    if (move.Target == MoveTarget.AdjacentAllyOrSelf && Gen != 5)
                    {
                        return pokemon;
                    }
                    // Target is a fainted ally: attack shouldn't retarget
                    return target;
                }
            }

            // Target is unfainted: use selected target location
            if (target is { Fainted: false })
            {
                return target;
            }

            // Chosen target not valid, retarget randomly with getRandomTarget
        }

        return GetRandomTarget(pokemon, move);
    }

    public Pokemon? GetRandomTarget(Pokemon pokemon, MoveId move)
    {
        return GetRandomTarget(pokemon, Library.Moves[move]);
    }

    public Pokemon? GetRandomTarget(Pokemon pokemon, Move move)
    {
        // A move was used without a chosen target

        // For instance: Metronome chooses Ice Beam. Since the user didn't
        // choose a target when choosing Metronome, Ice Beam's target must
        // be chosen randomly.

        // The target is chosen randomly from possible targets, EXCEPT that
        // moves that can target either allies or foes will only target foes
        // when used without an explicit target.

        // Self-targeting moves always target the user
        if (move.Target is MoveTarget.Self or
                           MoveTarget.All or
                           MoveTarget.AllySide or
                           MoveTarget.AllyTeam or
                           MoveTarget.AdjacentAllyOrSelf)
        {
            return pokemon;
        }

        // Adjacent ally targeting
        if (move.Target == MoveTarget.AdjacentAlly)
        {
            // In singles, there are no allies to target
            if (GameType == GameType.Singles) return null;

            // Get adjacent allies and return random one if available
            var adjacentAllies = pokemon.AdjacentAllies();
            return adjacentAllies.Count > 0 ? Sample(adjacentAllies) : null;
        }

        // Singles battles: target the opponent's active Pokémon
        if (GameType == GameType.Singles)
        {
            return pokemon.Side.Foe.Active[0];
        }

        // Triples battles (activePerHalf > 2)
        if (ActivePerHalf > 2)
        {
            // Adjacent-only moves need special handling
            if (move.Target is MoveTarget.AdjacentFoe or
                              MoveTarget.Normal or
                              MoveTarget.RandomNormal)
            {
                // Even if a move can target an ally, auto-resolution will never make it target an ally
                // i.e. if both your opponents faint before you use Flamethrower, it will fail 
                // instead of targeting your ally
                var adjacentFoes = pokemon.AdjacentFoes();
                if (adjacentFoes.Count > 0)
                {
                    return Sample(adjacentFoes);
                }

                // No valid target at all, return slot directly across for any possible redirection
                // This calculates the position mirrored across the field
                int mirrorPosition = pokemon.Side.Foe.Active.Count - 1 - pokemon.Position;
                return pokemon.Side.Foe.Active[mirrorPosition];
            }
        }

        // Default: return random foe, or first active foe if random fails
        return pokemon.Side.RandomFoe() ?? pokemon.Side.Foe.Active[0];
    }

    public void CheckFainted()
    {
        // Iterate through all sides in the battle
        foreach (Pokemon pokemon in Sides.SelectMany(side => side.Active.Where(pokemon => pokemon.Fainted)))
        {
            // Set the status to fainted
            pokemon.Status = ConditionId.Fainted;

            // Mark that this Pokémon needs to be switched out
            pokemon.SwitchFlag = true;
        }
    }

    public bool? FaintMessages(bool lastFirst = false, bool forceCheck = false, bool checkWin = true)
    {
        // Battle already ended
        if (Ended) return null;

        int length = FaintQueue.Count;

        // Empty queue
        if (length == 0)
        {
            return forceCheck && CheckWin() == true;
        }

        // Reorder queue if requested (move last to first)
        if (lastFirst && FaintQueue.Count > 0)
        {
            FaintQueue lastFaintData = FaintQueue[^1];
            FaintQueue.RemoveAt(FaintQueue.Count - 1);
            FaintQueue.Insert(0, lastFaintData);
        }

        FaintQueue? faintData = null;

        // Process all faints in queue
        while (FaintQueue.Count > 0)
        {
            int faintQueueLeft = FaintQueue.Count;
            faintData = FaintQueue[0];
            FaintQueue.RemoveAt(0);

            Pokemon pokemon = faintData.Target;

            // Run BeforeFaint event - allows abilities/items to trigger
            RelayVar? beforeFaintResult = RunEvent(
                EventId.BeforeFaint,
                pokemon,
                RunEventSource.FromNullablePokemon(faintData.Source),
                faintData.Effect
            );

            // If Pokemon hasn't fainted yet and BeforeFaint didn't prevent it
            if (!pokemon.Fainted && (beforeFaintResult == null || IsRelayVarTruthy(beforeFaintResult)))
            {
                // Print faint message
                if (DisplayUi)
                {
                    Add("faint", pokemon);
                }

                // Update side's Pokemon count
                if (pokemon.Side.PokemonLeft > 0)
                {
                    pokemon.Side.PokemonLeft--;
                }

                // Track total fainted (capped at 100)
                if (pokemon.Side.TotalFainted < 100)
                {
                    pokemon.Side.TotalFainted++;
                }

                // Run Faint event - triggers fainting abilities/items
                RunEvent(
                    EventId.Faint,
                    pokemon,
                    RunEventSource.FromNullablePokemon(faintData.Source),
                    faintData.Effect
                );

                // End ability state
                Ability ability = pokemon.GetAbility();
                SingleEvent(EventId.End, ability, pokemon.AbilityState, pokemon);

                // End item state
                Item item = pokemon.GetItem();
                SingleEvent(EventId.End, item, pokemon.ItemState, pokemon);

                // Handle forme regression (e.g., Mega Evolution reverting)
                if (pokemon is { FormeRegression: true, Transformed: false })
                {
                    // BEFORE clearing volatiles: restore base species and ability
                    pokemon.BaseSpecies = Library.Species[pokemon.Set.Species];
                    pokemon.BaseAbility = pokemon.Set.Ability;
                }

                // Clear all volatile conditions
                pokemon.ClearVolatile(false);

                // Mark as fainted and inactive
                pokemon.Fainted = true;
                pokemon.Illusion = null;
                pokemon.IsActive = false;
                pokemon.IsStarted = false;
                pokemon.Terastallized = null;

                // Complete forme regression
                if (pokemon.FormeRegression)
                {
                    // AFTER clearing volatiles: update details and stats
                    pokemon.Details = pokemon.GetUpdatedDetails();
                    if (DisplayUi)
                    {
                        Add("detailschange", pokemon, pokemon.Details.ToString(), "[silent]");
                    }
                    pokemon.UpdateMaxHp();
                    pokemon.FormeRegression = false;
                }

                // Track this faint for the current turn
                pokemon.Side.FaintedThisTurn = pokemon;

                // If queue grew during processing, we need to check win
                if (FaintQueue.Count >= faintQueueLeft)
                {
                    checkWin = true;
                }
            }
        }

        // Check for battle end
        if (checkWin && CheckWin(faintData) == true)
        {
            return true;
        }

        // Run AfterFaint event with original queue length
        if (faintData != null && length > 0)
        {
            RunEvent(
                EventId.AfterFaint,
                faintData.Target,
                RunEventSource.FromNullablePokemon(faintData.Source),
                faintData.Effect,
                new IntRelayVar(length)
            );
        }

        return false;
    }

    public bool? CheckWin(FaintQueue? faintData = null)
    {
        // If all sides have no Pokemon left, it's a tie (or last-faint-wins in Gen 5+)
        if (Sides.All(side => side.PokemonLeft <= 0))
        {
            // In Gen 5+, the side whose Pokemon fainted last loses (so their foe wins)
            // In earlier gens, it's a tie
            if (faintData != null && Gen > 4)
            {
                return Win(faintData.Target.Side.Foe);
            }
            else
            {
                return Win((Side?)null); // Tie
            }
        }

        // Check if any side has defeated all opposing Pokemon
        foreach (Side side in Sides)
        {
            if (side.FoePokemonLeft() <= 0)
            {
                return Win(side);
            }
        }

        // Battle hasn't ended yet
        return null;
    }

    public void GetActionSpeed(IAction action)
    {
        // Only process move actions for priority calculation
        if (action is MoveAction moveAction)
        {
            var move = moveAction.Move.ToActiveMove();

            // Get base priority from the original move data (not the active move)
            // This ensures abilities like Prankster only apply once, not repeatedly
            int priority = Library.Moves[move.Id].Priority;

            // Run ModifyPriority events to allow effects to change priority
            // SingleEvent: for move-specific priority changes (e.g., Grassy Glide in Grassy Terrain)
            RelayVar? singleEventResult = SingleEvent(
                EventId.ModifyPriority,
                move,
                null,
                moveAction.Pokemon,
                null,
                null,
                priority
            );

            if (singleEventResult is IntRelayVar singlePriority)
            {
                priority = singlePriority.Value;
            }

            // RunEvent: for Pokemon-based priority changes (e.g., Prankster ability)
            RelayVar? runEventResult = RunEvent(
                EventId.ModifyPriority,
                moveAction.Pokemon,
                null,
                move,
                priority
            );

            if (runEventResult is IntRelayVar modifiedPriority)
            {
                priority = modifiedPriority.Value;
            }

            // Set the action's priority (includes fractional priority for tie-breaking)
            moveAction.Priority = priority + moveAction.FractionalPriority;

            // In Gen 6+, update the move's priority property
            // This is used by Quick Guard to block moves with artificially enhanced priority
            if (Gen > 5)
            {
                moveAction.Move.Priority = priority;
            }
        }

        // Get the Pokemon's action speed (factors in speed stat, paralysis, etc.)
        // The other Action types have constant speed values so do not need to be set.
        switch (action)
        {
            case SwitchAction switchAction:
                switchAction.Speed = switchAction.Pokemon.GetActionSpeed();
                break;
            case PokemonAction pokemonAction:
                pokemonAction.Speed = pokemonAction.Pokemon.GetActionSpeed();
                break;
        }
    }

    public bool RunAction(IAction action)
    {
        int? pokemonOriginalHp = action switch
        {
            PokemonAction pa => pa.Pokemon.Hp,
            MoveAction ma => ma.Pokemon.Hp,
            SwitchAction sa => sa.Pokemon.Hp,
            _ => null,
        };

        List<(Pokemon pokemon, int hp)> residualPokemon = [];

        // Returns whether or not we ended in a callback
        switch (action.Choice)
        {
            case ActionId.Start:
                {
                    foreach (Side side in Sides)
                    {
                        if (side.PokemonLeft > 0)
                            side.PokemonLeft = side.Pokemon.Count;

                        if (DisplayUi)
                        {
                            Add("teamsize", side.Id.ToString(), side.Pokemon.Count.ToString());
                        }
                    }

                    if (DisplayUi)
                    {
                        Add("start");
                    }

                    // Change Zacian/Zamazenta into their Crowned formes
                    foreach (Pokemon pokemon in GetAllPokemon())
                    {
                        Species? rawSpecies = null;
                        if (pokemon.Species.Id == SpecieId.Zacian && pokemon.Item == ItemId.RustedSword)
                        {
                            rawSpecies = Library.Species[SpecieId.ZacianCrowned];
                        }
                        else if (pokemon.Species.Id == SpecieId.Zamazenta && pokemon.Item == ItemId.RustedShield)
                        {
                            rawSpecies = Library.Species[SpecieId.ZamazentaCrowned];
                        }

                        if (rawSpecies == null) continue;

                        Species? species = pokemon.SetSpecie(rawSpecies, Effect);
                        if (species == null) continue;

                        pokemon.BaseSpecies = rawSpecies;
                        pokemon.Details = pokemon.GetUpdatedDetails();
                        pokemon.SetAbility(species.Abilities.GetAbility(SpeciesAbilityType.Slot0)
                            ?? throw new InvalidOperationException("Species has no ability in slot 0"),
                            isFromFormeChange: true);
                        pokemon.BaseAbility = pokemon.Ability;

                        // Replace Iron Head with Behemoth Blade/Bash
                        Dictionary<SpecieId, MoveId> behemothMoves = new()
                    {
                        { SpecieId.ZacianCrowned, MoveId.BehemothBlade },
                        { SpecieId.ZamazentaCrowned, MoveId.BehemothBash },
                    };

                        int ironHeadIndex = pokemon.BaseMoves.IndexOf(MoveId.IronHead);
                        if (ironHeadIndex >= 0)
                        {
                            Move move = Library.Moves[behemothMoves[rawSpecies.Id]];
                            pokemon.BaseMoveSlots[ironHeadIndex] = new MoveSlot
                            {
                                Move = move.Id,
                                Id = move.Id,
                                Pp = move.NoPpBoosts ? move.BasePp : move.BasePp * 8 / 5,
                                MaxPp = move.NoPpBoosts ? move.BasePp : move.BasePp * 8 / 5,
                                Target = move.Target,
                                Disabled = false,
                                DisabledSource = null,
                                Used = false,
                            };
                            pokemon.MoveSlots = [.. pokemon.BaseMoveSlots];
                        }
                    }

                    // Call format's OnBattleStart handler
                    Format.OnBattleStart?.Invoke(this);

                    foreach (RuleId rule in RuleTable.Keys)
                    {
                        string ruleString = rule.ToString();
                        if (ruleString.Length > 0 && "+*-!".Contains(ruleString[0])) continue;
                        Format subFormat = Library.Rulesets[rule];
                        subFormat.OnBattleStart?.Invoke(this);
                    }

                    foreach (Side side in Sides)
                    {
                        for (int i = 0; i < side.Active.Count; i++)
                        {
                            if (side.PokemonLeft <= 0)
                            {
                                // Forfeited before starting
                                side.Active[i] = side.Pokemon[i];
                                side.Active[i].Fainted = true;
                                side.Active[i].Hp = 0;
                            }
                            else
                            {
                                Actions.SwitchIn(side.Pokemon[i], i);
                            }
                        }
                    }

                    foreach (Pokemon pokemon in GetAllPokemon())
                    {
                        Condition speciesCondition = Library.Conditions[pokemon.Species.Conditon];
                        SingleEvent(EventId.Start, speciesCondition, pokemon.SpeciesState, pokemon);
                    }

                    MidTurn = true;
                    break;
                }

            case ActionId.Move:
                {
                    var moveAction = (MoveAction)action;
                    if (!moveAction.Pokemon.IsActive) return false;
                    if (moveAction.Pokemon.Fainted) return false;
                    Actions.RunMove(moveAction.Move, moveAction.Pokemon, moveAction.TargetLoc,
                        new BattleActions.RunMoveOptions
                        {
                            SourceEffect = moveAction.SourceEffect,
                            OriginalTarget = moveAction.OriginalTarget,
                        });
                    break;
                }

            case ActionId.Terastallize:
                {
                    var teraAction = (PokemonAction)action;
                    Actions.Terastallize(teraAction.Pokemon);
                    break;
                }

            case ActionId.BeforeTurnMove:
                {
                    var btmAction = (MoveAction)action;
                    if (!btmAction.Pokemon.IsActive) return false;
                    if (btmAction.Pokemon.Fainted) return false;
                    Debug($"before turn callback: {btmAction.Move.Id}");
                    Pokemon? target = GetTarget(btmAction.Pokemon, btmAction.Move, btmAction.TargetLoc);
                    if (target == null) return false;
                    if (btmAction.Move.BeforeTurnCallback == null)
                        throw new InvalidOperationException("beforeTurnMove has no beforeTurnCallback");
                    btmAction.Move.BeforeTurnCallback(this, btmAction.Pokemon, target,
                        btmAction.Move.ToActiveMove());
                    break;
                }

            case ActionId.PriorityChargeMove:
                {
                    var pcmAction = (MoveAction)action;
                    if (!pcmAction.Pokemon.IsActive) return false;
                    if (pcmAction.Pokemon.Fainted) return false;
                    Debug($"priority charge callback: {pcmAction.Move.Id}");
                    if (pcmAction.Move.PriorityChargeCallback == null)
                        throw new InvalidOperationException("priorityChargeMove has no priorityChargeCallback");
                    pcmAction.Move.PriorityChargeCallback(this, pcmAction.Pokemon);
                    break;
                }

            case ActionId.Event:
                {
                    var eventAction = (PokemonAction)action;
                    RunEvent(eventAction.Event ??
                             throw new InvalidOperationException("Event action must have an event"),
                        eventAction.Pokemon);
                    break;
                }

            case ActionId.Team:
                {
                    var teamAction = (TeamAction)action;
                    if (teamAction.Index == 0)
                    {
                        teamAction.Pokemon.Side.Pokemon = [];
                    }
                    teamAction.Pokemon.Side.Pokemon.Add(teamAction.Pokemon);
                    teamAction.Pokemon.Position = teamAction.Index;
                    // We return here because the update event would crash since there are no active pokemon yet
                    return false;
                }

            case ActionId.Pass:
                return false;

            case ActionId.InstaSwitch:
            case ActionId.Switch:
                {
                    var switchAction = (SwitchAction)action;
                    if (switchAction.Choice == ActionId.Switch && switchAction.Pokemon.Status != ConditionId.None)
                    {
                        Ability naturalCure = Library.Abilities[AbilityId.NaturalCure];
                        SingleEvent(EventId.CheckShow, naturalCure, null, switchAction.Pokemon);
                    }

                    Actions.SwitchIn(switchAction.Target, switchAction.Pokemon.Position, switchAction.SourceEffect);
                    break;
                }

            case ActionId.RevivalBlessing:
                {
                    var rbAction = (SwitchAction)action;
                    rbAction.Pokemon.Side.PokemonLeft++;
                    if (rbAction.Target.Position < rbAction.Pokemon.Side.Active.Count)
                    {
                        Queue.AddChoice(new SwitchAction
                        {
                            Choice = ActionId.InstaSwitch,
                            Pokemon = rbAction.Target,
                            Target = rbAction.Target,
                            Order = 3,
                        });
                    }
                    rbAction.Target.Fainted = false;
                    rbAction.Target.FaintQueued = false;
                    rbAction.Target.SubFainted = false;
                    rbAction.Target.Status = ConditionId.None;
                    rbAction.Target.Hp = 1; // Needed so HP functions work
                    rbAction.Target.SetHp(rbAction.Target.MaxHp / 2);

                    if (DisplayUi)
                    {
                        Add("-heal", rbAction.Target, rbAction.Target.GetHealth, "[from] move: Revival Blessing");
                    }

                    rbAction.Pokemon.Side.RemoveSlotCondition(rbAction.Pokemon, ConditionId.RevivalBlessing);
                    break;
                }

            case ActionId.RunSwitch:
                {
                    var rsAction = (PokemonAction)action;
                    Actions.RunSwitch(rsAction.Pokemon);
                    break;
                }

            case ActionId.Shift:
                {
                    var shiftAction = (PokemonAction)action;
                    if (!shiftAction.Pokemon.IsActive) return false;
                    if (shiftAction.Pokemon.Fainted) return false;
                    SwapPosition(shiftAction.Pokemon, 1);
                    break;
                }

            case ActionId.BeforeTurn:
                EachEvent(EventId.BeforeTurn);
                break;

            case ActionId.Residual:
                if (DisplayUi)
                {
                    Add("");
                }

                ClearActiveMove(failed: true);
                UpdateSpeed();
                residualPokemon = GetAllActive()
                    .Select(p => (p, p.GetUndynamaxedHp()))
                    .ToList();
                FieldEvent(EventId.Residual);

                if (!Ended && DisplayUi)
                {
                    Add("upkeep");
                }
                break;
        }

        // Phazing (Roar, etc)
        foreach (Side side in Sides)
        {
            foreach (Pokemon pokemon in side.Active)
            {
                if (pokemon.ForceSwitchFlag)
                {
                    if (pokemon.Hp > 0) Actions.DragIn(pokemon.Side, pokemon.Position);
                    pokemon.ForceSwitchFlag = false;
                }
            }
        }

        ClearActiveMove();

        // Fainting
        FaintMessages();
        if (Ended) return true;

        // Switching (fainted pokemon, U-turn, Baton Pass, etc)
        if (Queue.Peek()?.Choice == ActionId.InstaSwitch)
        {
            return false;
        }

        // Emergency Exit / Wimp Out check (Gen 5+)
        if (action.Choice != ActionId.Start)
        {
            EachEvent(EventId.Update);
            foreach ((Pokemon pokemon, int originalHp) in residualPokemon)
            {
                int maxHp = pokemon.GetUndynamaxedHp(pokemon.MaxHp);
                if (pokemon.Hp > 0 && pokemon.GetUndynamaxedHp() <= maxHp / 2 && originalHp > maxHp / 2)
                {
                    RunEvent(EventId.EmergencyExit, pokemon);
                }
            }
        }

        if (action.Choice == ActionId.RunSwitch)
        {
            var runSwitchAction = (PokemonAction)action;
            Pokemon pokemon = runSwitchAction.Pokemon;
            if (pokemon.Hp > 0 && pokemon.Hp <= pokemon.MaxHp / 2 &&
                pokemonOriginalHp > pokemon.MaxHp / 2)
            {
                RunEvent(EventId.EmergencyExit, pokemon);
            }
        }

        // Check for switches
        var switches = Sides
            .Select(side => side.Active.Any(p => p.SwitchFlag.IsTrue()))
            .ToList();

        for (int i = 0; i < Sides.Count; i++)
        {
            bool reviveSwitch = false; // Used to ignore the fake switch for Revival Blessing
            if (switches[i] && CanSwitch(Sides[i]) == 0)
            {
                foreach (Pokemon pokemon in Sides[i].Active)
                {
                    IEffect? revivalBlessing = Sides[i].GetSlotCondition(pokemon.Position,
                        ConditionId.RevivalBlessing);
                    if (revivalBlessing != null)
                    {
                        reviveSwitch = true;
                        continue;
                    }
                    pokemon.SwitchFlag = false;
                }
                if (!reviveSwitch) switches[i] = false;
            }
            else if (switches[i])
            {
                foreach (Pokemon pokemon in Sides[i].Active)
                {
                    if (pokemon.Hp > 0 &&
                        pokemon.SwitchFlag.IsTrue() &&
                        pokemon.SwitchFlag != MoveId.RevivalBlessing &&
                        !pokemon.SkipBeforeSwitchOutEventFlag)
                    {
                        RunEvent(EventId.BeforeSwitchOut, pokemon);
                        pokemon.SkipBeforeSwitchOutEventFlag = true;
                        FaintMessages(); // Pokemon may have fainted in BeforeSwitchOut
                        if (Ended) return true;
                        if (pokemon.Fainted)
                        {
                            switches[i] = Sides[i].Active.Any(p => p.SwitchFlag.IsTrue());
                        }
                    }
                }
            }
        }

        foreach (bool playerSwitch in switches)
        {
            if (playerSwitch)
            {
                MakeRequest(RequestState.SwitchIn);
                return true;
            }
        }

        // In Gen 8+, speed is updated dynamically
        IAction? nextAction = Queue.Peek();
        if (nextAction?.Choice == ActionId.Move)
        {
            // Update the queue's speed properties and sort it
            UpdateSpeed();
            foreach (IAction queueAction in Queue.List)
            {
                GetActionSpeed(queueAction);
            }
            Queue.Sort();
        }

        return false;
    }

    /// <summary>
    /// Generally called at the beginning of a turn, to go through the
    /// turn one action at a time.
    /// 
    /// If there is a mid-turn decision (like U-Turn), this will return
    /// and be called again later to resume the turn.
    /// </summary>
    public void TurnLoop()
    {
        // Add empty line for formatting
        UiGenerator.PrintEmptyLine();

        // Add timestamp in Unix epoch seconds
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        UiGenerator.PrintMessage($"t:|{timestamp}");

        // Clear request state if it exists
        if (RequestState != RequestState.None)
        {
            RequestState = RequestState.None;
        }

        // First time through - set up turn structure
        if (!MidTurn)
        {
            // Insert BeforeTurn action at the front of the queue
            Queue.InsertChoice(new BeforeTurnAction());

            // Add Residual action at the end of the queue
            Queue.AddChoice(new ResidualAction());

            MidTurn = true;
        }

        // Process actions one at a time
        while (Queue.Shift() is { } action)
        {
            RunAction(action);

            // Exit early if we need to wait for a request or battle ended
            if (RequestState != RequestState.None || Ended)
            {
                return;
            }
        }

        // Turn is complete
        EndTurn();
        MidTurn = false;
        Queue.Clear();
    }

    /// <summary>
    /// Takes a choice passed from the client. Starts the next
    /// turn if all required choices have been made.
    /// </summary>
    /// <param name="sideId">The ID of the side making the choice</param>
    /// <param name="input">The choice being made</param>
    /// <returns>True if the choice was valid and processed, false otherwise</returns>
    public bool Choose(SideId sideId, Choice input)
    {
        Side side = GetSide(sideId);

        if (!side.Choose(input))
        {
            if (string.IsNullOrEmpty(side.GetChoice().Error))
            {
                side.EmitChoiceError(
                    $"Unknown error for choice: {input}. If you're not using a custom client," +
                    $"please report this as a bug.");
            }
            return false;
        }

        if (!side.IsChoiceDone())
        {
            side.EmitChoiceError($"Incomplete choice: {input} - missing other pokemon");
            return false;
        }

        if (AllChoicesDone())
        {
            CommitChoices();
        }

        return true;
    }

    /// <summary>
    /// Convenience method for easily making choices for multiple sides.
    /// If inputs are provided, applies them to the corresponding sides.
    /// If no inputs are provided, auto-chooses for all sides.
    /// </summary>
    /// <param name="inputs">Optional list of choices for each side. Pass null or empty list for auto-choice.</param>
    public void MakeChoices(List<Choice>? inputs = null)
    {
        if (inputs is { Count: > 0 })
        {
            // Apply provided choices to each corresponding side
            for (int i = 0; i < inputs.Count; i++)
            {
                Choice input = inputs[i];
                Sides[i].Choose(input);
            }
        }
        else
        {
            // Auto-choose for all sides
            foreach (Side side in Sides)
            {
                side.AutoChoose();
            }
        }

        // Commit all choices
        CommitChoices();
    }

    public void CommitChoices()
    {
        UpdateSpeed();

        // Sometimes you need to make switch choices mid-turn (e.g. U-turn,
        // fainting). When this happens, the rest of the turn is saved (and not
        // re-sorted), but the new switch choices are sorted and inserted before
        // the rest of the turn.
        var oldQueue = Queue.List.ToList(); // Create a copy of the current queue
        Queue.Clear();

        if (!AllChoicesDone())
        {
            throw new InvalidOperationException("Not all choices done");
        }

        // Log each side's choice to the input log
        foreach (Side side in Sides)
        {
            string? choice = side.GetChoice().ToString();
            if (!string.IsNullOrEmpty(choice))
            {
                InputLog.Add($">{side.Id} {choice}");
            }
        }

        // Add each side's actions to the queue
        foreach (Side side in Sides)
        {
            Queue.AddChoice(side.Choice.Actions);
        }

        ClearRequest();

        // Sort the new actions by priority/speed
        Queue.Sort();

        // Append the old queue actions after the new ones
        Queue.List.AddRange(oldQueue);

        // Clear request state
        RequestState = RequestState.None;
        foreach (Side side in Sides)
        {
            side.ActiveRequest = null;
        }

        // Start executing the turn
        TurnLoop();

        // Workaround for tests - send updates if log is getting large
        if (Log.Count - SentLogPos > 500)
        {
            SendUpdates();
        }
    }

    public void UndoChoice(SideId sideId)
    {
        Side side = GetSide(sideId);

        // No active request - nothing to undo
        if (RequestState == RequestState.None)
            return;

        // Check if undo would leak information
        if (side.GetChoice().CantUndo)
        {
            side.EmitChoiceError("Can't undo: A trapping/disabling effect would cause undo to leak information");
            return;
        }

        bool updated = false;

        // If undoing a move selection, update disabled moves for each Pokémon
        if (side.RequestState == RequestState.Move)
        {
            foreach (ChosenAction action in side.GetChoice().Actions)
            {
                // Skip if not a move action
                if (action.Choice != ChoiceType.Move)
                    continue;

                Pokemon? pokemon = action.Pokemon;
                if (pokemon == null)
                    continue;

                // Update the request for this Pokémon, refreshing disabled moves
                if (side.UpdateRequestForPokemon(pokemon, req =>
                        side.UpdateDisabledRequest(pokemon, req)))
                {
                    updated = true;
                }
            }
        }

        // Clear the current choice
        side.ClearChoice();

        // If we updated any move availability, send the updated request to the client
        if (updated && side.ActiveRequest != null)
        {
            side.EmitRequest(side.ActiveRequest, updatedRequest: true);
        }
    }

    /// <summary>
    /// Returns true if both decisions are complete.
    /// Checks if all sides have completed their choices for the current turn.
    /// When supportCancel is disabled, locks completed choices to prevent information leaks.
    /// </summary>
    /// <returns>True if all sides have completed their choices, false otherwise</returns>
    public bool AllChoicesDone()
    {
        int totalActions = 0;

        foreach (Side side in Sides.Where(side => side.IsChoiceDone()))
        {
            // If cancellation is not supported, lock the choice to prevent undoing
            // This prevents information leaks from trapping/disabling effects
            if (!SupportCancel)
            {
                Choice choice = side.GetChoice();
                choice.CantUndo = true;
            }

            totalActions++;
        }

        return totalActions >= Sides.Count;
    }

    public void Hint(string hint, bool once = false, Side? side = null)
    {
        // Create a unique key for this hint
        string hintKey = side != null ? $"{side.Id}|{hint}" : hint;

        // If this hint has already been shown and once=true, skip it
        if (Hints.Contains(hintKey)) return;

        // Send the hint to the appropriate recipient(s)
        if (side != null)
        {
            AddSplit(side.Id, ["-hint", hint]);
        }
        else
        {
            Add("-hint", hint);
        }

        // Mark this hint as shown if once=true
        if (once) Hints.Add(hintKey);
    }

    public void AddSplit(SideId side, Part[] secret, Part[]? shared = null)
    {
        // Add the split marker with the side ID
        Log.Add($"| split |{side}");

        // Add the secret parts (visible only to the specified side)
        Add(secret.Select(p => (PartFuncUnion)p).ToArray());

        // Add the shared parts (visible to all sides) or empty line
        if (shared is { Length: > 0 })
        {
            Add(shared.Select(p => (PartFuncUnion)p).ToArray());
        }
        else
        {
            Log.Add(string.Empty);
        }
    }

    public void Add(params PartFuncUnion[] parts)
    {
        // Check if any part is a function that generates side-specific content
        bool hasFunction = parts.Any(part => part is FuncPartFuncUnion);

        if (!hasFunction)
        {
            // Simple case: all parts are direct values
            // Extract Part from PartPartFuncUnion before formatting
            string message = $"|{string.Join("|", parts.Select(FormatPartFuncUnion))}";
            Log.Add(message);
            return;
        }

        // Complex case: some parts are functions
        SideId? side = null;
        var secret = new List<string>();
        var shared = new List<string>();

        foreach (PartFuncUnion part in parts)
        {
            if (part is FuncPartFuncUnion funcPart)
            {
                // Execute the function to get side-specific content
                SideSecretSharedResult result = funcPart.Func();

                // Validate that all functions use the same side
                if (side.HasValue && side.Value != result.Side)
                {
                    throw new InvalidOperationException("Multiple sides passed to add");
                }

                side = result.Side;
                secret.Add(result.Secret.ToString());
                shared.Add(result.Shared.ToString());
            }
            else if (part is PartPartFuncUnion directPart)
            {
                // Direct value: add to both secret and shared
                string formatted = FormatPart(directPart.Part);
                secret.Add(formatted);
                shared.Add(formatted);
            }
        }

        // Add the split message
        if (side.HasValue)
        {
            AddSplit(side.Value,
                secret.Select(Part (s) => new StringPart(s)).ToArray(),
                shared.Select(Part (s) => new StringPart(s)).ToArray());
        }
    }

    public void AddMove(params StringNumberDelegateObjectUnion[] args)
    {
        // Track this line's position for later attribute additions
        LastMoveLine = Log.Count;

        // Format and add the move line to the log
        string message = $"|{string.Join("|", args.Select(FormatArg))}";
        Log.Add(message);
    }

    public void AttrLastMove(params StringNumberDelegateObjectUnion[] args)
    {
        // No last move to attribute to
        if (LastMoveLine < 0) return;

        // Special handling for animation lines with [still]
        if (Log[LastMoveLine].StartsWith("|-anim|"))
        {
            if (args.Any(arg => FormatArg(arg) == "[still]"))
            {
                // Remove the animation line entirely
                Log.RemoveAt(LastMoveLine);
                LastMoveLine = -1;
                return;
            }
        }
        else if (args.Any(arg => FormatArg(arg) == "[still]"))
        {
            // If no animation plays, hide the target (index 4) to prevent information leak
            string[] parts = Log[LastMoveLine].Split('|');
            if (parts.Length > 4)
            {
                parts[4] = string.Empty;
                Log[LastMoveLine] = string.Join("|", parts);
            }
        }

        // Append the attributes to the last move line
        string attributes = $"|{string.Join("|", args.Select(FormatArg))}";
        Log[LastMoveLine] += attributes;
    }

    /// <summary>
    /// Updates the target of the most recently logged move.
    /// Used when a move's target changes after it was initially logged (e.g., through redirection).
    /// </summary>
    /// <param name="newTarget">The new target Pokémon to display in the move log</param>
    public void RetargetLastMove(Pokemon newTarget)
    {
        // No last move to retarget
        if (LastMoveLine < 0) return;

        // Parse the log line (format: |move|attacker|moveName|target|...)
        string[] parts = Log[LastMoveLine].Split('|');

        // Index 4 is the target field in the move log format
        if (parts.Length > 4)
        {
            parts[4] = newTarget.ToString();
            Log[LastMoveLine] = string.Join("|", parts);
        }
    }

    /// <summary>
    /// Logs debug information to the battle log.
    /// Only adds the message if debug mode is enabled.
    /// </summary>
    /// <param name="activity">The debug message to log</param>
    public void Debug(string activity)
    {
        if (DebugMode)
        {
            Add("debug", activity);
        }
    }

    /// <summary>
    /// Extracts and returns all debug messages from the battle log.
    /// In the TypeScript version, this uses extractChannelMessages with channel -1.
    /// </summary>
    /// <returns>A string containing all debug messages, separated by newlines</returns>
    public string GetDebugLog()
    {
        // Extract debug channel messages (channel -1 in the original)
        // This would need the extractChannelMessages function implementation
        // For now, we'll filter for debug messages manually
        var debugMessages = Log
            .Where(line => line.StartsWith("|debug|"))
            .Select(line => line[7..]); // Remove "|debug|" prefix

        return string.Join("\n", debugMessages);
    }

    /// <summary>
    /// Logs a debug error message to the battle log.
    /// Unlike Debug(), this always logs regardless of debug mode setting.
    /// Used for important errors that should always be recorded.
    /// </summary>
    /// <param name="activity">The error message to log</param>
    public void DebugError(string activity)
    {
        Add("debug", activity);
    }

    public static IReadOnlyList<PokemonSet> GetTeam(PlayerOptions options)
    {
        return options.Team ?? throw new InvalidOperationException();
    }

    /// <summary>
    /// Generates and sends team preview information at the start of a battle.
    /// Creates sanitized team data that can be safely shown to both players,
    /// hiding sensitive information like exact EVs/IVs while showing species,
    /// items, abilities, and moves.
    /// </summary>
    public void ShowOpenTeamSheets()
    {
        // Only show team sheets at the very start of the battle
        if (Turn != 0) return;

        foreach (Side side in Sides)
        {
            var team = new List<PokemonSet>();

            foreach (Pokemon pokemon in side.Pokemon)
            {
                PokemonSet set = pokemon.Set;

                // Create sanitized set with visible information only
                var newSet = new PokemonSet
                {
                    Name = string.Empty, // Hide nicknames
                    Species = set.Species,
                    Item = set.Item,
                    Ability = set.Ability,
                    Moves = set.Moves,
                    Nature = new Nature { Id = NatureId.None }, // Hide nature
                    Gender = pokemon.Gender,
                    Evs = new StatsTable(), // Hide exact EVs
                    Ivs = new StatsTable(), // Hide exact IVs
                    Level = set.Level,
                    Shiny = set.Shiny,
                    TeraType = set.TeraType, // Will be set below for Gen 9
                };

                // Special handling for Zacian/Zamazenta with their signature items
                // This prevents the client from flagging them as illusions when they use their signature move
                if (set is { Species: SpecieId.Zacian, Item: ItemId.RustedSword } or
                    { Species: SpecieId.Zamazenta, Item: ItemId.RustedShield })
                {
                    // Convert to Crowned forme
                    SpecieId crownedSpecies = set.Species == SpecieId.Zacian
                        ? SpecieId.ZacianCrowned
                        : SpecieId.ZamazentaCrowned;

                    newSet = newSet with { Species = crownedSpecies };

                    // Replace Iron Head with signature move
                    var crownedMoves = new Dictionary<SpecieId, MoveId>
                    {
                        { SpecieId.ZacianCrowned, MoveId.BehemothBlade },
                        { SpecieId.ZamazentaCrowned, MoveId.BehemothBash },
                    };

                    int ironHeadIndex = newSet.Moves.ToList().IndexOf(MoveId.IronHead);
                    if (ironHeadIndex >= 0)
                    {
                        var movesList = newSet.Moves.ToList();
                        movesList[ironHeadIndex] = crownedMoves[crownedSpecies];
                        newSet = newSet with { Moves = movesList };
                    }
                }

                team.Add(newSet);
            }

            // Send the sanitized team data to the client
            // Note: You'll need to implement a Teams.Pack() equivalent method
            // that serializes the team data into the format expected by the client
            string packedTeam = PackTeam(team);
            Add("showteam", side.Id.ToString(), packedTeam);
        }
    }

    public void SetPlayer(SideId slot, PlayerOptions options)
    {
        Side? side;
        bool didSomething = true;

        // Convert Side enum to array index (P1=0, P2=1)
        int slotNum = slot == SideId.P1 ? 0 : 1;

        if (!Sides[slotNum].Initialised)
        {
            // Create new player
            var team = GetTeam(options);
            string playerName = options.Name ?? $"Player {slotNum + 1}";
            side = new Side(playerName, this, slot, [.. team])
            {
                Name = playerName,
                Avatar = options.Avatar ?? string.Empty,
                Team = [.. team],
                Pokemon = [],
                Active = [],
                SideConditions = [],
                SlotConditions = [],
                Choice = new Choice
                {
                    CantUndo = false,
                    Actions = [],
                    ForcedSwitchesLeft = 0,
                    ForcedPassesLeft = 0,
                    SwitchIns = [],
                    Terastallize = false,
                },
            };

            Sides[slotNum] = side;
        }
        else
        {
            // Edit existing player
            side = Sides[slotNum];
            didSomething = false;

            // Update name if different
            if (!string.IsNullOrEmpty(options.Name) && side.Name != options.Name)
            {
                side.Name = options.Name;
                didSomething = true;
            }

            // Update avatar if different
            if (!string.IsNullOrEmpty(options.Avatar) && side.Avatar != options.Avatar)
            {
                side.Avatar = options.Avatar;
                didSomething = true;
            }

            // Prevent team changes for existing players
            if (options.Team != null)
            {
                throw new InvalidOperationException($"Player {slot} already has a team!");
            }
        }

        // Exit early if no changes were made
        if (!didSomething) return;

        // Log the player setup
        string optionsJson = System.Text.Json.JsonSerializer.Serialize(options);
        InputLog.Add($"> player {slot} {optionsJson}");

        // Add player info to battle log
        string rating = options.Rating?.ToString() ?? string.Empty;
        Log.Add($"|player|{side.Id}|{side.Name}|{side.Avatar}|{rating}");

        // Start battle if all sides are ready and battle hasn't started
        if (Sides.All(playerSide => !playerSide.Initialised) && !Started)
        {
            Start();
        }
    }

    public void SendUpdates()
    {
        // Don't send if there are no new log entries
        if (SentLogPos >= Log.Count) return;

        // Send new log entries to clients
        var updates = Log.Skip(SentLogPos).ToList();
        Send(SendType.Update, updates);

        // Send requests to players if not already sent
        if (!SentRequests)
        {
            foreach (Side side in Sides)
            {
                side.EmitRequest();
            }
            //SentRequests = true;
        }

        // Update the position marker
        SentLogPos = Log.Count;

        // Send end-of-battle summary if battle ended and not already sent
        if (!SentEnd && Ended)
        {
            // Build the battle log object
            var log = new Dictionary<string, object>
            {
                ["winner"] = Winner ?? string.Empty,
                ["seed"] = PrngSeed,
                ["turns"] = Turn,
                ["p1"] = Sides[0].Name,
                ["p2"] = Sides[1].Name,
                ["p1team"] = Sides[0].Team,
                ["p2team"] = Sides[1].Team,
                ["score"] = new List<int> { Sides[0].PokemonLeft, Sides[1].PokemonLeft },
                ["inputLog"] = InputLog,
            };

            // Add P3/P4 data only if they exist (for multi-battles)
            if (Sides.Count > 2)
            {
                log["p3"] = Sides[2].Name;
                log["p3team"] = Sides[2].Team;
                log["score"] = new List<int>
            {
                Sides[0].PokemonLeft,
                Sides[1].PokemonLeft,
                Sides[2].PokemonLeft,
            };
            }

            if (Sides.Count > 3)
            {
                log["p4"] = Sides[3].Name;
                log["p4team"] = Sides[3].Team;
                log["score"] = new List<int>
            {
                Sides[0].PokemonLeft,
                Sides[1].PokemonLeft,
                Sides[2].PokemonLeft,
                Sides[3].PokemonLeft,
            };
            }

            // Serialize and send the end message
            string logJson = System.Text.Json.JsonSerializer.Serialize(log);
            Send(SendType.End, new List<string> { logJson });
            SentEnd = true;
        }
    }

    private Side GetSide(SideId id)
    {
        return id switch
        {
            SideId.P1 => Sides[0],
            SideId.P2 => Sides[1],
            _ => throw new ArgumentOutOfRangeException(nameof(id), $"Invalid Side: {id}"),
        };
    }
    public int GetOverflowedTurnCount()
    {
        return Gen >= 8 ? (Turn - 1) % 256 : Turn - 1;
    }

    /// <summary>
    /// Initializes an EffectState object with proper effect ordering.
    /// Effect order is used to determine priority when multiple effects trigger.
    /// - Effects with explicit effectOrder use that value
    /// - Effects on active Pokemon/entities get auto-incremented order
    /// - Effects on inactive targets get order 0
    /// </summary>
    public EffectState InitEffectState(EffectStateId? id = null, int? effectOrder = null, Pokemon? target = null)
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
            // Only increment for active Pokemon, otherwise use 0
            // Use the battle's master counter for active effects
            finalEffectOrder = target.IsActive ? EffectOrder++ : 0;
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

            // TODO: Initialize other properties as needed
        };
    }

    public EffectState InitEffectState(EffectStateId id, Pokemon? source, PokemonSlot? sourceSlot, int? duration)
    {
        // Use the first overload to handle basic initialization and effect ordering
        // Pass the source Pokemon as the target for effect ordering purposes
        EffectState state = InitEffectState(id, effectOrder: null, target: source);

        // Add the additional properties specific to this overload
        state.Source = source;
        state.SourceSlot = sourceSlot;
        state.Duration = duration;

        return state;
    }

    public EffectState InitEffectState(EffectStateId id, Side target, Pokemon source, PokemonSlot sourceSlot,
        bool isSlotCondition, int? duration)
    {
        // Use the first overload to handle basic initialization and effect ordering
        // Side conditions are considered inactive for effect ordering purposes
        EffectState state = InitEffectState(id, effectOrder: null, target: null);
        // Add the additional properties specific to this overload
        state.Target = target;
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

    /// <summary>
    /// Disposes of battle resources and breaks circular references.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Dispose managed resources
            Field.Destroy();

            foreach (Side side in Sides)
            {
                side.Destroy();
            }

            Queue.Clear();
            Log.Clear();
            InputLog.Clear();
            MessageLog.Clear();
            FaintQueue.Clear();
        }

        // Mark as disposed
        _disposed = true;
    }

    /// <summary>
    /// Public method for backwards compatibility with original API
    /// </summary>
    public void Destroy()
    {
        Dispose();
    }

    public IBattle Copy()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Truncate a number to an unsigned 32-bit integer.
    /// If bits is specified, the number is scaled, truncated, then unscaled.
    /// This is used for precise damage calculations in Pokemon battles.
    /// </summary>
    public int Trunc(int num, int bits = 0)
    {
        if (bits == 0)
        {
            // Simple case: just return the integer as-is
            return num;
        }

        // For 16-bit truncation (used in nature calculations):
        // Truncate to 16 bits by masking with 0xFFFF (65535)
        // This matches the game's behavior for overflow prevention
        if (bits == 16)
        {
            return num & 0xFFFF;
        }

        // For other bit counts, scale up by 2^bits, truncate, then scale back down
        // This effectively performs: Math.Floor(num / (2^bits)) * (2^bits)
        int divisor = 1 << bits; // 2^bits
        return (num / divisor) * divisor;
    }

    public int Trunc(double num, int bits = 0)
    {
        return Trunc((int)Math.Floor(num), bits);
    }

    public int ClampIntRange(int num, int? min, int? max)
    {
        if (num < min)
        {
            return min.Value;
        }
        return num > max ? max.Value : num;
    }

    #region Helpers

    /// <summary>
    /// Packs a team into a string format for transmission to the client.
    /// This is a placeholder - you'll need to implement the actual packing logic
    /// based on your client's expected format.
    /// </summary>
    private static string PackTeam(List<PokemonSet> team)
    {
        // TODO: Implement team packing logic
        // This should serialize the team data into the format your client expects
        // The original uses Teams.pack() from the @pkmn/sets library
        throw new NotImplementedException("Team packing logic needs to be implemented");
    }

    /// <summary>
    /// Formats a Part for output to the battle log.
    /// Converts various Part types to their string representations.
    /// </summary>
    private static string FormatPart(Part part)
    {
        return part switch
        {
            StringPart s => s.Value,
            IntPart i => i.Value.ToString(),
            DoublePart d => d.Value.ToString("F"),
            BoolPart b => b.Value.ToString().ToLowerInvariant(),
            PokemonPart p => p.Pokemon.ToString(),
            SidePart s => s.Side.Id.ToString(),
            MovePart m => m.Move.Name,
            EffectPart e => e.Effect.Name,
            UndefinedPart => "undefined",
            _ => string.Empty,
        };
    }

    /// <summary>
    /// Formats a StringNumberDelegateObjectUnion argument for output.
    /// Converts various union types to their string representations.
    /// </summary>
    private static string FormatArg(StringNumberDelegateObjectUnion arg)
    {
        return arg switch
        {
            StringStringNumberDelegateObjectUnion s => s.Value,
            IntStringNumberDelegateObjectUnion i => i.Value.ToString(),
            DoubleStringNumberDelegateObjectUnion d => d.Value.ToString("F"),
            DelegateStringNumberDelegateObjectUnion del => del.Delegate.Method.Name,
            ObjectStringNumberDelegateObjectUnion obj => obj.Object.ToString() ?? string.Empty,
            _ => string.Empty,
        };
    }

    /// <summary>
    /// Logs a damage message to the battle log based on the effect causing the damage.
    /// Handles special cases like partially trapped, powder, and confusion.
    /// </summary>
    private void PrintDamageMessage(Pokemon target, Pokemon? source, Condition? effect)
    {
        if (!DisplayUi) return;

        // Get the effect name, converting "tox" to "psn" for display
        string? effectName = effect?.FullName == "tox" ? "psn" : effect?.FullName;

        switch (effect?.Id)
        {
            case ConditionId.PartiallyTrapped:
                // Get the source effect from the volatile condition
                if (target.Volatiles.TryGetValue(ConditionId.PartiallyTrapped, out EffectState? ptState) &&
                    ptState.SourceEffect != null)
                {
                    Add("-damage", target, target.GetHealth, $"[from] {ptState.SourceEffect.FullName}", "[partiallytrapped]");
                }
                break;

            case ConditionId.Powder:
                Add("-damage", target, target.GetHealth, "[silent]");
                break;

            case ConditionId.Confusion:
                Add("-damage", target, target.GetHealth, "[from] confusion");
                break;

            default:
                if (effect?.EffectType == EffectType.Move || string.IsNullOrEmpty(effectName))
                {
                    // Simple damage from a move or no effect
                    Add("-damage", target, target.GetHealth);
                }
                else if (source != null && (source != target || effect?.EffectType == EffectType.Ability))
                {
                    // Damage from effect with source
                    Add("-damage", target, target.GetHealth, $"[from] {effectName}", $"[of] {source}");
                }
                else
                {
                    // Damage from effect without source
                    Add("-damage", target, target.GetHealth, $"[from] {effectName}");
                }
                break;
        }
    }

    /// <summary>
    /// Logs a heal message to the battle log based on the effect causing the healing.
    /// </summary>
    private void PrintHealMessage(Pokemon target, Pokemon? source, Condition? effect)
    {
        if (!DisplayUi) return;

        // Get the health status for the log message
        var healthFunc = target.GetHealth;

        // Determine if this is a drain effect
        bool isDrain = effect?.Id == ConditionId.Drain;

        if (isDrain && source != null)
        {
            // Drain healing shows the source
            Add("-heal", target, healthFunc, "[from] drain", $"[of] {source}");
        }
        else if (effect != null && effect.Id != ConditionId.None)
        {
            // Healing from a specific effect
            string effectName = effect.FullName == "tox" ? "psn" : effect.FullName;
            if (source != null && source != target)
            {
                Add("-heal", target, healthFunc, $"[from] {effectName}", $"[of] {source}");
            }
            else
            {
                Add("-heal", target, healthFunc, $"[from] {effectName}");
            }
        }
        else
        {
            // Simple heal with no effect
            Add("-heal", target, healthFunc);
        }
    }

    /// <summary>
    /// Formats a PartFuncUnion for output to the battle log.
    /// Extracts the Part from PartPartFuncUnion and formats it.
    /// </summary>
    private static string FormatPartFuncUnion(PartFuncUnion partFuncUnion)
    {
        return partFuncUnion switch
        {
            PartPartFuncUnion p => FormatPart(p.Part),
            FuncPartFuncUnion => throw new InvalidOperationException(
                "Cannot format a function PartFuncUnion directly. This should be handled in the Add method."),
            _ => string.Empty,
        };
    }


    /// <summary>
    /// Invokes a DelegateEffectDelegate by attempting common delegate signatures.
    /// Falls back to reflection only when necessary.
    /// Optimized to minimize allocations by reusing a fixed-size array.
    /// </summary>
    private RelayVar? InvokeDelegateEffectDelegate(Delegate del, bool hasRelayVar, RelayVar relayVar,
        SingleEventTarget? target, SingleEventSource? source, IEffect? sourceEffect)
    {
        // Cache parameter info to avoid repeated reflection calls
        var parameters = del.Method.GetParameters();
        int paramCount = parameters.Length;

        // Most common signature: (IBattle battle, ...)
        if (paramCount == 0)
        {
            return (RelayVar?)del.DynamicInvoke(null);
        }

        // Optimize for the most common cases (1-5 parameters)
        // This avoids array allocation for the majority of callbacks
        object? result;
        switch (paramCount)
        {
            case 1:
                result = del.DynamicInvoke(BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source, sourceEffect));
                return (RelayVar?)result;
            case 2:
                result = del.DynamicInvoke(
                    BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source, sourceEffect),
                    BuildSingleArg(parameters[1], hasRelayVar, relayVar, target, source, sourceEffect, 1)
                );
                return (RelayVar?)result;
            case 3:
                result = del.DynamicInvoke(
                    BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source, sourceEffect),
                    BuildSingleArg(parameters[1], hasRelayVar, relayVar, target, source, sourceEffect, 1),
                    BuildSingleArg(parameters[2], hasRelayVar, relayVar, target, source, sourceEffect, 2)
                );
                return (RelayVar?)result;
            case 4:
                result = del.DynamicInvoke(
                    BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source, sourceEffect),
                    BuildSingleArg(parameters[1], hasRelayVar, relayVar, target, source, sourceEffect, 1),
                    BuildSingleArg(parameters[2], hasRelayVar, relayVar, target, source, sourceEffect, 2),
                    BuildSingleArg(parameters[3], hasRelayVar, relayVar, target, source, sourceEffect, 3)
                );
                return (RelayVar?)result;
        }

        // Fallback for 5+ parameters (rare)
        // Use array allocation for these cases
        object?[] args = new object?[paramCount];
        int argIndex = 0;

        // First parameter is typically IBattle (this)
        if (parameters[0].ParameterType.IsAssignableFrom(typeof(IBattle)))
        {
            args[argIndex++] = this;
        }

        // Add relayVar if it was explicitly provided and if the delegate expects it
        if (hasRelayVar)
        {
            args[argIndex++] = relayVar;
        }

        // Add remaining standard parameters: target, source, sourceEffect
        while (argIndex < paramCount)
        {
            Type paramType = parameters[argIndex].ParameterType;

            // Try to match target parameter
            if (target != null)
            {
                EventTargetParameter? targetParam = EventTargetParameter.FromSingleEventTarget(target, paramType);
                if (targetParam != null)
                {
                    args[argIndex++] = targetParam.ToObject();
                    continue;
                }
            }

            // Try to match source parameter
            if (source != null)
            {
                EventSourceParameter? sourceParam = EventSourceParameter.FromSingleEventSource(source, paramType);
                if (sourceParam != null)
                {
                    args[argIndex++] = sourceParam.ToObject();
                    continue;
                }
            }

            // Try to match sourceEffect parameter
            if (sourceEffect != null && paramType.IsInstanceOfType(sourceEffect))
            {
                args[argIndex++] = sourceEffect;
                continue;
            }

            // If we couldn't match, add null
            args[argIndex++] = null;
        }

        result = del.DynamicInvoke(args);
        return (RelayVar?)result;
    }

    /// <summary>
    /// Builds a single argument for delegate invocation.
    /// Used by the optimized fast-path for common parameter counts.
    /// </summary>
    private object? BuildSingleArg(System.Reflection.ParameterInfo param, bool hasRelayVar, RelayVar relayVar,
        SingleEventTarget? target, SingleEventSource? source, IEffect? sourceEffect, int position = 0)
    {
        Type paramType = param.ParameterType;

        // First parameter is typically IBattle
        if (position == 0 && paramType.IsAssignableFrom(typeof(IBattle)))
        {
            return this;
        }

        // Second parameter might be relayVar if explicitly provided
        if (position == 1 && hasRelayVar)
        {
            return relayVar;
        }

        // Adjust position if IBattle was first
        int adjustedPos = position;
        if (position > 0 && paramType.IsAssignableFrom(typeof(IBattle)))
        {
            adjustedPos--;
        }
        if (hasRelayVar && adjustedPos > 0)
        {
            adjustedPos--;
        }

        // Try to match standard parameters in order: target, source, sourceEffect
        switch (adjustedPos)
        {
            case 0:
                // Try target first
                if (target != null)
                {
                    EventTargetParameter? targetParam = EventTargetParameter.FromSingleEventTarget(target, paramType);
                    if (targetParam != null) return targetParam.ToObject();
                }
                break;
            case 1:
                // Try source second
                if (source != null)
                {
                    EventSourceParameter? sourceParam = EventSourceParameter.FromSingleEventSource(source, paramType);
                    if (sourceParam != null) return sourceParam.ToObject();
                }
                break;
            case 2:
                // Try sourceEffect third
                if (sourceEffect != null && paramType.IsInstanceOfType(sourceEffect))
                {
                    return sourceEffect;
                }
                break;
        }

        return null;
    }

    private int CalculateDefaultSubOrder(EventListenerWithoutPriority listener)
    {
        // Effect type hierarchy for subOrder
        int subOrder = listener.Effect.EffectType switch
        {
            EffectType.Condition => 2,
            EffectType.Weather => 5,
            EffectType.Format => 5,
            EffectType.Rule => 5,
            EffectType.Ruleset => 5,
            EffectType.Ability => 7,
            EffectType.Item => 8,
            _ => 0,
        };

        // Refine for conditions
        if (listener.Effect.EffectType == EffectType.Condition && listener.State?.Target != null)
        {
            subOrder = listener.State.Target switch
            {
                SideEffectStateTarget when listener.State.IsSlotCondition == true => 3,  // Slot condition
                SideEffectStateTarget => 4,                                       // Side condition
                FieldEffectStateTarget => 5,                                      // Field condition
                _ => subOrder,
            };
        }

        // Special abilities
        if (listener.Effect is Ability ability)
        {
            subOrder = ability.Id switch
            {
                AbilityId.PoisonTouch => 6,
                AbilityId.PerishBody => 6,
                AbilityId.Stall => 9,
                _ => subOrder,
            };
        }

        return subOrder;
    }

    /// <summary>
    /// Masks species IDs that change forme on battle start to prevent information leakage.
    /// Returns a masked version (with -*) for species that need hiding, otherwise returns the original.
    /// </summary>
    private static SpecieId MaskSpeciesForTeamPreview(SpecieId speciesId)
    {
        return speciesId switch
        {
            // Zacian/Zamazenta without Crowned forme should be masked
            SpecieId.Zacian => SpecieId.Zacian, // Keep as-is, will add masking in SpecieId enum if needed
            SpecieId.Zamazenta => SpecieId.Zamazenta, // Keep as-is

            // Xerneas formes should be masked
            SpecieId.Xerneas or SpecieId.XerneasNeutral or SpecieId.XerneasActive => SpecieId.Xerneas,

            // Don't mask Crowned formes
            SpecieId.ZacianCrowned => SpecieId.ZacianCrowned,
            SpecieId.ZamazentaCrowned => SpecieId.ZamazentaCrowned,

            // All other species pass through unchanged
            _ => speciesId,
        };
    }

    #endregion
}