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
            new Side(this),
            new Side(this),
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
                    new StringPart(string.Empty),
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
            string packedTeam = Teams.Pack(team);
            Add("showteam", side.Id.ToString(), packedTeam);
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

    

    #region Helpers

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