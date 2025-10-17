using ApogeeVGC.Data;
using ApogeeVGC.Player;
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
using System.Diagnostics.Metrics;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;
using String = System.String;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleAsync : IBattle
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
    public int SentLogPos { get; set; } = 0;
    public bool SentEnd { get; set; } = false;
    public static bool SentRequests => true;

    public RequestState RequestState { get; set; } = RequestState.None;
    public int Turn { get; set; } = 0;
    public bool MidTurn { get; set; } = false;
    public bool Started { get; set; } = false;
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
    public MoveId? LastSuccessfulMoveThisTurn { get; set; } = null;
    public int LastMoveLine { get; set; } = -1;
    public int LastDamage { get; set; } = 0;
    public int EffectOrder { get; set; }
    public bool QuickClawRoll { get; set; } = false;
    public List<int> SpeedOrder { get; set; } = [];

    // TeamGenerator
    // Hints

    public static Undefined NotFail => new();
    public static int HitSubstiture => 0;
    public static bool Fail => false;
    public const object? SilentFail = null;

    public Action<SendType, IReadOnlyList<string>> Send { get; init; }

    public Library Library { get; init; }
    public bool PrintDebug { get; init; }
    public Side P1 => Sides[0];
    public Side P2 => Sides[1];
    public static Side P3 => throw new Exception("3v3 battles are not implemented.");
    public static Side P4 => throw new Exception("4v4 battles are not implemented.");

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

        Send = options.Send ?? ((_, _) => { });

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
    /// <typeparam name="T">Type that implements IPriorityComparison for sorting</typeparam>
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

    private EventListener ResolvePriority(EventListenerWithoutPriority h, EventId callbackName)
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
        UiGenerator.PrintActivateEvent(defender, Effect);
        UiGenerator.PrintActivateEvent(attacker, Library.Items[ItemId.ProtectivePads]);
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

        // Generate appropriate requests for the current state
        List<IChoiceRequest> requests = GetRequests(type.Value);

        // Assign requests to each side
        for (int i = 0; i < Sides.Count; i++)
        {
            Sides[i].ActiveRequest = requests[i];
        }

        // Note: In the TypeScript version, sentRequests is set to false
        // The C# version has SentRequests as a static property that always returns true
        // This may need to be changed to an instance property if the behavior is needed

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
        UiGenerator.PrintMessage("Time's up! Going to tiebreaker...");

        // Count non-fainted Pokemon for each side
        var notFainted = Sides.Select(side =>
            side.Pokemon.Count(pokemon => !pokemon.Fainted)
        ).ToList();

        // Display Pokemon count per side
        string pokemonCountMessage = string.Join("; ",
            Sides.Select((side, i) => $"{side.Name}: {notFainted[i]} Pokemon left")
        );
        UiGenerator.PrintMessage(pokemonCountMessage);

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
        string hpPercentageMessage = string.Join("; ",
            tiedSides.Select((side, i) => $"{side.Name}: {Math.Round(hpPercentage[i])}% total HP left")
        );
        UiGenerator.PrintMessage(hpPercentageMessage);

        // Filter sides with maximum HP percentage
        double maxPercentage = hpPercentage.Max();
        tiedSides = tiedSides.Where((_, i) => Math.Abs(hpPercentage[i] - maxPercentage) < double.Epsilon).
            ToList();

        if (tiedSides.Count <= 1)
        {
            return Win(tiedSides.FirstOrDefault());
        }

        // Calculate total HP
        var hpTotal = tiedSides.Select(side =>
            side.Pokemon.Sum(pokemon => pokemon.Hp)
        ).ToList();

        // Display total HP per side
        string hpTotalMessage = string.Join("; ",
            tiedSides.Select((side, i) => $"{side.Name}: {hpTotal[i]} total HP left")
        );
        UiGenerator.PrintMessage(hpTotalMessage);

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
        // Convert SideId to Side if provided
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

        // Print empty line for formatting
        UiGenerator.PrintEmptyLine();

        // Print the appropriate win/tie message
        // Note: AllySide is not implemented in this codebase (see Side class)
        // The original TypeScript code checks for side?.allySide here
        if (side != null)
        {
            // Single side wins
            UiGenerator.PrintWinEvent(side);
        }
        else
        {
            // Tie
            UiGenerator.PrintTieEvent();
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

    public bool Lose(SideId side)
    {
        throw new NotImplementedException();
    }

    public bool Lose(Side side)
    {
        throw new NotImplementedException();
    }

    public int CanSwitch(Side side)
    {
        return PossibleSwitches(side).Count;
    }

    public Pokemon? GetRandomSwitchable(Side side)
    {
        throw new NotImplementedException();
    }

    private List<Pokemon> PossibleSwitches(Side side)
    {
        throw new NotImplementedException();
    }

    public bool SwapPosition(Pokemon pokemon, int newPosition, string? attribures = null)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public void EndTurn()
    {
        throw new NotImplementedException();
    }

    public bool MaybeTriggerEndlessBattleClause(List<bool> trappedBySide, List<StalenessId?> stalenessBySide)
    {
        throw new NotImplementedException();
    }

    public void Start()
    {
        throw new NotImplementedException();
    }

    public void Restart(Action<string, List<string>>? send)
    {
        throw new NotImplementedException();
    }

    public void RunPickTeam()
    {
        throw new NotImplementedException();
    }

    public void CheckEvBalance()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Modifies a Pokémon's stat stages (boosts) during battle.
    /// 
    /// Process:
    /// 1. Validates the target has HP and is active
    /// 2. Runs ChangeBoost and TryBoost events for modification
    /// 3. Applies boosts via target.BoostBy() for each stat
    /// 4. Logs boost messages using UiGenerator methods
    /// 5. Triggers AfterEachBoost and AfterBoost events
    /// 6. Updates statsRaisedThisTurn/statsLoweredThisTurn flags
    /// 
    /// Returns:
    /// - null if boost succeeded
    /// - 0 if target has no HP
    /// - false if target is inactive or no foes remain (Gen 6+)
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

        // Gen 6+: Check if any foes remain
        if (Gen > 5 && target.Side.FoePokemonLeft() <= 0) return new BoolBoolZeroUnion(false);

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
            throw new InvalidOperationException("ChangeBoost event did not return a valid SparseBoostsTable.");
        }

        bool? success = null;

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

            // Determine if this is a boost or unboost for messaging
            bool isUnboost = boostValue.Value < 0 || target.Boosts.GetBoost(boostId) == -6;

            if (boostBy != 0)
            {
                success = true;

                // Handle special cases
                EffectStateId effectId = effect?.EffectStateId ?? EffectStateId.FromEmpty();
                if (effectId is MoveEffectStateId { MoveId: MoveId.BellyDrum } or
                    AbilityEffectStateId { AbilityId: AbilityId.AngerPoint })
                {
                    // Use PrintSetBoostEvent for moves that set boosts to maximum
                    UiGenerator.PrintSetBoostEvent(target, boostId, boostBy, effect ??
                        throw new ArgumentNullException(nameof(effect)));
                }
                else if (effect is not null)
                {
                    switch (effect.EffectType)
                    {
                        case EffectType.Move:
                            // Regular move boost/unboost
                            break;

                        case EffectType.Item:
                            // Item-triggered boost/unboost (messages handled by UI)
                            break;

                        default:
                            // Ability or other effect type
                            if (effect.EffectType == EffectType.Ability && !isSecondary)
                            {
                                if (effect is not Ability ability)
                                {
                                    throw new InvalidOperationException("Effect is not an Ability.");
                                }
                                UiGenerator.PrintAbilityEvent(target, ability);
                                //boosted = true;
                            }
                            break;
                    }

                    if (isUnboost)
                    {
                        UiGenerator.PrintUnboostEvent(target, boostId, -boostBy, effect);
                    }
                    else
                    {
                        UiGenerator.PrintBoostEvent(target, boostId, boostBy, effect);
                    }

                    break;
                }
                // Trigger AfterEachBoost event
                RunEvent(EventId.AfterEachBoost, target, RunEventSource.FromNullablePokemon(source), effect,
                    currentBoost);
            }
            else if (effect?.EffectType == EffectType.Ability)
            {
                // Ability boost that failed
                if (!isSecondary && !isSelf) continue;
                if (isUnboost)
                {
                    UiGenerator.PrintUnboostEvent(target, boostId, 0, effect);
                }
                else
                {
                    UiGenerator.PrintBoostEvent(target, boostId, 0, effect);
                }
            }
            else if (!isSecondary && !isSelf)
            {
                // Failed boost that should be announced
                if (isUnboost)
                {
                    UiGenerator.PrintUnboostEvent(target, boostId, 0, effect ?? throw new ArgumentNullException(nameof(effect)));
                }
                else
                {
                    UiGenerator.PrintBoostEvent(target, boostId, 0, effect ?? throw new ArgumentNullException(nameof(effect)));
                }
            }
        }

        // Trigger AfterBoost event
        RunEvent(EventId.AfterBoost, target, RunEventSource.FromNullablePokemon(source), effect, finalBoost);

        // Update turn flags
        if (success == true)
        {
            // Check if any boosts were positive
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

    public List<IntFalseUnion?> SpreadDamage(SpreadMoveDamage damage, List<PokemonFalseUnion?>? targetArray,
        Pokemon? source = null, BattleDamageEffect? effect = null, bool instaFaint = false)
    {
        throw new NotImplementedException();
    }

    public IntFalseUnion? Damage(int damage, Pokemon? target = null, Pokemon? source = null,
        BattleDamageEffect? effect = null, bool instafaint = false)
    {
        throw new NotImplementedException();
    }

    public int DirectDamage(int damage, Pokemon? target = null, Pokemon? source = null, IEffect? effect = null)
    {
        throw new NotImplementedException();
    }

    public IntFalseUnion? Heal(int damage, Pokemon? target = null, Pokemon? source = null,
        BattleHealEffect? effect = null)
    {
        throw new NotImplementedException();
    }

    public int Chain(int previousMod, int nextMod)
    {
        throw new NotImplementedException();
    }

    public int Chain(List<int> previousMod, int nextMod)
    {
        throw new NotImplementedException();
    }

    public int Chain(int previousMod, List<int> nextMod)
    {
        throw new NotImplementedException();
    }

    public int Chain(List<int> previousMod, List<int> nextMod)
    {
        throw new NotImplementedException();
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

    public bool ValidTargetLoc(int targetLoc, Pokemon source, PokemonType targetType)
    {
        throw new NotImplementedException();
    }

    public bool ValidTarget(Pokemon target, Pokemon source, PokemonType targetType)
    {
        throw new NotImplementedException();
    }

    public Pokemon? GetTarget(Pokemon pokemon, MoveId move, int targetLoc, Pokemon? originalTarget = null)
    {
        throw new NotImplementedException();
    }

    public Pokemon? GetTarget(Pokemon pokemon, Move move, int targetLoc, Pokemon? originalTarget = null)
    {
        throw new NotImplementedException();
    }

    public Pokemon? GetRandomTarget(Pokemon pokemon, MoveId move)
    {
        return GetRandomTarget(pokemon, Library.Moves[move]);
    }

    public Pokemon? GetRandomTarget(Pokemon pokemon, Move move)
    {
        throw new NotImplementedException();
    }

    public void CheckFainted()
    {
        throw new NotImplementedException();
    }
    public bool? FaintMessages(bool lastFirst = false, bool forceCheck = false, bool checkWin = true)
    {
        throw new NotImplementedException();
    }

    public bool? CheckWin(FaintQueue? faintData = null)
    {
        faintData = FaintQueue[0];
        throw new NotImplementedException();
    }

    public void GetActionSpeed(IAction action)
    {
        throw new NotImplementedException();
    }

    public bool RunAction(IAction action)
    {
        throw new NotImplementedException();
    }

    public void TurnLoop()
    {
        throw new NotImplementedException();
    }

    public bool Choose(SideId sideId, Choice input)
    {
        throw new NotImplementedException();
    }

    public void MakeChoices(List<Choice> inputs)
    {
        throw new NotImplementedException();
    }

    public void CommitChoices()
    {
        throw new NotImplementedException();
    }

    public void UndoChoice(SideId sideId)
    {
        throw new NotImplementedException();
    }

    public bool AllChoicesDone()
    {
        throw new NotImplementedException();
    }

    public void Hint(string hint, bool? once = null, Side? side = null)
    {
        throw new NotImplementedException();
    }

    // AddSplit()
    // Add()
    // AddMove()
    // AttrLastMove()

    public void RetargetLastMove(Pokemon newTarget)
    {
        throw new NotImplementedException();
    }

    public void Debug(string activityString)
    {
        throw new NotImplementedException();
    }

    public string GetDebugLog()
    {
        throw new NotImplementedException();
    }

    public void DebugError()
    {
        throw new NotImplementedException();
    }

    private static IReadOnlyList<PokemonSet> GetTeam(PlayerOptions options)
    {
        return options.Team ?? throw new InvalidOperationException();
    }

    public void ShowOpenTeamSheets()
    {
        throw new NotImplementedException();
    }

    private void SetPlayer(SideId slot, PlayerOptions options)
    {
        Side? side;
        bool didSomething = true;

        // Convert SideId enum to array index (P1=0, P2=1)
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
        throw new NotImplementedException();
    }

    private Side GetSide(SideId id)
    {
        return id switch
        {
            SideId.P1 => Sides[0],
            SideId.P2 => Sides[1],
            _ => throw new ArgumentOutOfRangeException(nameof(id), $"Invalid SideId: {id}"),
        };
    }
    public int GetOverflowedTurnCount()
    {
        throw new NotImplementedException();
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

    public void Destroy()
    {
        throw new NotImplementedException();
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

    #endregion
}