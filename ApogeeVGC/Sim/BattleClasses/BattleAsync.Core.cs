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
using System.Drawing;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    public int LastDamage { get; set; } = 0;
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
        if (newPosition != 1 && (target.Fainted))
        {
            return false;
        }

        // Log the swap event
        UiGenerator.PrintSwapEvent(pokemon, newPosition, attributes);

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

                // Update type visibility (Gen 7+)
                // If Pokemon was attacked and illusion wasn't broken, reveal its type
                if (pokemon.GetLastAttackedBy() != null && Gen >= 7)
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

                // Update apparent type display (Gen 7+ and not Terastallized)
                if (Gen >= 7 && pokemon.Terastallized == null)
                {
                    // Get the visible Pokemon (accounting for Illusion)
                    Pokemon seenPokemon = pokemon.Illusion ?? pokemon;

                    // Get actual types as a string (e.g., "Fire/Flying")
                    string realTypeString = string.Join("/",
                        seenPokemon.GetTypes(excludeAdded: true).Select(t => t.ToString()));

                    // Update apparent type if it changed
                    string currentApparentType = string.Join("/", seenPokemon.ApparentType);
                    if (realTypeString != currentApparentType)
                    {
                        // Update apparent type (this is for display purposes)
                        seenPokemon.ApparentType = seenPokemon.GetTypes(excludeAdded: true).ToList();
                    }
                }

                // Reset trapping status
                pokemon.Trapped = PokemonTrapped.False;
                pokemon.MaybeTrapped = false;

                // Run trap events
                RunEvent(EventId.TrapPokemon, pokemon);

                // Check if Pokemon can potentially be trapped
                if (!pokemon.KnownType || Dex.GetImmunity(ConditionId.Trapped, pokemon.Types))
                {
                    RunEvent(EventId.MaybeTrapPokemon, pokemon);
                }

                // Check foe abilities for potential trapping (Gen 3+)
                if (Gen > 2)
                {
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

                            // Skip immunity check if type is known and already immune
                            if (pokemon.KnownType && !Dex.GetImmunity(ConditionId.Trapped, pokemon.Types))
                                continue;

                            // Run the FoeMaybeTrapPokemon event for this potential ability
                            SingleEvent(EventId.FoeMaybeTrapPokemon, ability, null, pokemon, source);
                        }
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
        UiGenerator.PrintTurnEvent(Turn);

        // Pre-calculate Quick Claw roll for Gen 2-3
        if (Gen == 2)
        {
            QuickClawRoll = RandomChance(60, 256);
        }
        if (Gen == 3)
        {
            QuickClawRoll = RandomChance(1, 5);
        }

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
            UiGenerator.PrintMessage("It is turn 1000. You have hit the turn limit!");
            Tie();
            return true;
        }

        // Warning messages for approaching turn limit
        if ((Turn >= 500 && Turn % 100 == 0) || // Every 100 turns past turn 500
            (Turn >= 900 && Turn % 10 == 0) ||  // Every 10 turns past turn 900
            Turn >= 990)                         // Every turn past turn 990
        {
            int turnsLeft = 1000 - Turn;
            string turnsLeftText = turnsLeft == 1 ? "1 turn" : $"{turnsLeft} turns";
            UiGenerator.PrintBigError($"You will auto-tie if the battle doesn't end in {turnsLeftText} (on turn 1000).");
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

            if (trapped) continue; // Changed from 'break' - should only skip this side

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
            UiGenerator.PrintMessage(
                $"{loser.Name}'s team started with the rudimentary means to perform " +
                "restorative berry-cycling and thus loses.");
            return Win(loser.Foe);
        }

        if (losers.Count == Sides.Count)
        {
            UiGenerator.PrintMessage(
                "Each side's team started with the rudimentary means to perform " +
                "restorative berry-cycling.");
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

        // Log generation
        UiGenerator.PrintMessage($"gen|{Gen}");

        // Log tier
        UiGenerator.PrintMessage($"tier|{Format.Name}");

        // Log rated status
        if (Rated)
        {
            string ratedMessage = Rated ? "" : Rated.ToString();
            UiGenerator.PrintMessage($"rated|{ratedMessage}");
        }

        // Call format's OnBegin handler
        Format.OnBegin?.Invoke(this);

        // Call OnBegin for each rule in the rule table
        foreach (Format subFormat in from rule in RuleTable.Keys let ruleString = rule.ToString()
                 where ruleString.Length <= 0 || !"+*-!".Contains(ruleString[0]) select Library.Rulesets[rule])
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
        Queue.InserChoice(new StartGameChoice());

        // Set mid-turn flag
        MidTurn = true;

        // Start turn loop if no request is pending
        if (RequestState == RequestState.None)
        {
            TurnLoop();
        }
    }

    //public void Restart(Action<string, List<string>>? send)
    //{
    //    throw new InvalidOperationException("This method relies on the battle being serialized which" +
    //                                        "cannot be done in this simulator implmentation.");
    //}

    public void RunPickTeam()
    {
        throw new NotImplementedException();
    }

    public void CheckEvBalance()
    {
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
                UiGenerator.PrintBigError(
                    "Warning: One player isn't adhering to a 510 EV limit, and the other player is.");
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


 //   spreadDamage(
	//	damage: SpreadMoveDamage, targetArray: (false | Pokemon | null)[] | null = null,
	//	source: Pokemon | null = null, effect: 'drain' | 'recoil' | Effect | null = null, instafaint = false
	//) {
	//	if (!targetArray) return [0];
	//	const retVals: (number | false | undefined)[] = [];
	//	if (typeof effect === 'string' || !effect) effect = this.dex.conditions.getByID((effect || '') as ID);
	//	for (const [i, curDamage] of damage.entries()) {
	//		const target = targetArray[i];
	//		let targetDamage = curDamage;
	//		if (!(targetDamage || targetDamage === 0)) {
	//			retVals[i] = targetDamage;
	//			continue;
	//		}
	//		if (!target || !target.hp) {
	//			retVals[i] = 0;
	//			continue;
	//		}
	//		if (!target.isActive) {
	//			retVals[i] = false;
	//			continue;
	//		}
	//		if (targetDamage !== 0) targetDamage = this.clampIntRange(targetDamage, 1);

	//		if (effect.id !== 'struggle-recoil') { // Struggle recoil is not affected by effects
	//			if (effect.effectType === 'Weather' && !target.runStatusImmunity(effect.id)) {
	//				this.debug('weather immunity');
	//				retVals[i] = 0;
	//				continue;
	//			}
	//			targetDamage = this.runEvent('Damage', target, source, effect, targetDamage, true);
	//			if (!(targetDamage || targetDamage === 0)) {
	//				this.debug('damage event failed');
	//				retVals[i] = curDamage === true ? undefined : targetDamage;
	//				continue;
	//			}
	//		}
	//		if (targetDamage !== 0) targetDamage = this.clampIntRange(targetDamage, 1);

	//		if (this.gen <= 1) {
	//			if (this.dex.currentMod === 'gen1stadium' ||
	//				!['recoil', 'drain', 'leechseed'].includes(effect.id) && effect.effectType !== 'Status') {
	//				this.lastDamage = targetDamage;
	//			}
	//		}

	//		retVals[i] = targetDamage = target.damage(targetDamage, source, effect);
	//		if (targetDamage !== 0) target.hurtThisTurn = target.hp;
	//		if (source && effect.effectType === 'Move') source.lastDamage = targetDamage;

	//		const name = effect.fullname === 'tox' ? 'psn' : effect.fullname;
	//		switch (effect.id) {
	//		case 'partiallytrapped':
	//			this.add('-damage', target, target.getHealth, '[from] ' + target.volatiles['partiallytrapped'].sourceEffect.fullname, '[partiallytrapped]');
	//			break;
	//		case 'powder':
	//			this.add('-damage', target, target.getHealth, '[silent]');
	//			break;
	//		case 'confused':
	//			this.add('-damage', target, target.getHealth, '[from] confusion');
	//			break;
	//		default:
	//			if (effect.effectType === 'Move' || !name) {
	//				this.add('-damage', target, target.getHealth);
	//			} else if (source && (source !== target || effect.effectType === 'Ability')) {
	//				this.add('-damage', target, target.getHealth, `[from] ${name}`, `[of] ${source}`);
	//			} else {
	//				this.add('-damage', target, target.getHealth, `[from] ${name}`);
	//			}
	//			break;
	//		}

	//		if (targetDamage && effect.effectType === 'Move') {
	//			if (this.gen <= 1 && effect.recoil && source) {
	//				if (this.dex.currentMod !== 'gen1stadium' || target.hp > 0) {
	//					const amount = this.clampIntRange(Math.floor(targetDamage * effect.recoil[0] / effect.recoil[1]), 1);
	//					this.damage(amount, source, target, 'recoil');
	//				}
	//			}
	//			if (this.gen <= 4 && effect.drain && source) {
	//				const amount = this.clampIntRange(Math.floor(targetDamage * effect.drain[0] / effect.drain[1]), 1);
	//				// Draining can be countered in gen 1
	//				if (this.gen <= 1) this.lastDamage = amount;
	//				this.heal(amount, source, target, 'drain');
	//			}
	//			if (this.gen > 4 && effect.drain && source) {
	//				const amount = Math.round(targetDamage * effect.drain[0] / effect.drain[1]);
	//				this.heal(amount, source, target, 'drain');
	//			}
	//		}
	//	}

	//	if (instafaint) {
	//		for (const [i, target] of targetArray.entries()) {
	//			if (!retVals[i] || !target) continue;

	//			if (target.hp <= 0) {
	//				this.debug(`instafaint: ${this.faintQueue.map(entry => entry.target.name)}`);
	//				this.faintMessages(true);
	//				if (this.gen <= 2) {
	//					target.faint();
	//					if (this.gen <= 1) {
	//						this.queue.clear();
	//						// Fainting clears accumulated Bide damage
	//						for (const pokemon of this.getAllActive()) {
	//							if (pokemon.volatiles['bide']?.damage) {
	//								pokemon.volatiles['bide'].damage = 0;
	//								this.hint("Desync Clause Mod activated!");
	//								this.hint("In Gen 1, Bide's accumulated damage is reset to 0 when a Pokemon faints.");
	//							}
	//						}
	//					}
	//				}
	//			}
	//		}
	//	}

	//	return retVals;
	//}

    public List<IntFalseUnion?> SpreadDamage(SpreadMoveDamage damage, List<PokemonFalseUnion?>? targetArray,
        Pokemon? source = null, BattleDamageEffect? effect = null, bool instaFaint = false)
    {
        // Return early if no targets
        if (targetArray == null)
            return [new IntIntFalseUnion(0)];

        var retVals = new List<IntFalseUnion?>();

        Condition effectCondition;

        switch (effect)
        {
            case EffectBattleDamageEffect ebde:
                if (ebde.Effect is not Condition cond)
                {
                    throw new InvalidOperationException("EffectBattleDamageEffect does not contain a Condition.");
                }
                effectCondition = cond;
                break;
            case DrainBattleDamageEffect:
                effectCondition = Library.Conditions[ConditionId.Drain];
                break;
            case RecoilBattleDamageEffect:
                effectCondition = Library.Conditions[ConditionId.Recoil];
                break;
            default:
                throw new InvalidOperationException("Unknown BattleDamageEffect type.");
        }

        // Process each target
        for (int i = 0; i < damage.Count; i++)
        {
            IntBoolUnion? curDamage = damage[i];

            // Skip if damage is null/undefined
            if (curDamage == null)
            {
                retVals.Add(null);
                continue;
            }

            // Extract Pokemon from union type
            Pokemon? target = targetArray[i] switch
            {
                PokemonPokemonUnion p => p.Pokemon,
                _ => null,
            };

            int targetDamage = curDamage.ToInt();

            // Target has no HP - return 0
            if (target is not { Hp: > 0 })
            {
                retVals.Add(new IntIntFalseUnion(0));
                continue;
            }

            // Target is not active - return false
            if (!target.IsActive)
            {
                retVals.Add(new FalseIntFalseUnion());
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
                    retVals.Add(new IntIntFalseUnion(0));
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
                    retVals.Add(null);
                    continue;
                }

                targetDamage = damageInt.Value;
            }

            // Clamp damage again after events
            if (targetDamage != 0)
                targetDamage = ClampIntRange(targetDamage, 1, null);

            // Apply damage to target
            targetDamage = target.Damage(targetDamage, source, effectCondition);
            retVals.Add(new IntIntFalseUnion(targetDamage));

            // Track that the Pokemon was hurt this turn
            if (targetDamage != 0)
                target.HurtThisTurn = target.Hp;

            // Track source's last damage if this was a move
            if (source != null && effectCondition?.EffectType == EffectType.Move)
                source.LastDamage = targetDamage;

            switch (effectCondition?.Id)
            {
                case ConditionId.PartiallyTrapped:
                    break;

                case ConditionId.Powder:
                    break;

                case ConditionId.Confusion:
                    break;

                default:
                    string effectName = effectCondition?.Name == "Toxic" ? "Poison" : effectCondition?.Name ?? "";
                    break;
            }

            // Note: Gen 5+ drain/recoil handling removed since you're Gen 9 only
            // Those effects are now handled in the move's effect handlers
        }

        // Handle instafaint if requested
        if (instaFaint)
        {
            for (int i = 0; i < targetArray.Count; i++)
            {
                if (retVals[i] == null || retVals[i] is FalseIntFalseUnion)
                    continue;

                Pokemon? target = targetArray[i] switch
                {
                    PokemonPokemonUnion p => p.Pokemon,
                    _ => null,
                };

                if (target?.Hp <= 0)
                {
                    Debug($"instafaint: {string.Join(", ", FaintQueue.Select(entry => entry.Target.Name))}");
                    FaintMessages(lastFirst: true);
                }
            }
        }

        return retVals;
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