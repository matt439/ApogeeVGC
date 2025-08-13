using ApogeeVGC_CS.data;
using System;
using System.Drawing;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC_CS.sim
{
    public enum ChannelId
    {
        Channel0 = 0,
        Channel1 = 1,
        Channel2 = 2,
        Channel3 = 3,
        Channel4 = 4
    }

    public class ChannelMessages : Dictionary<ChannelId, List<string>>;

    public static class BattleUtils
    {
        public static Dictionary<int, List<string>> ExtractChannelMessages(string message, IEnumerable<int> channelIds)
        {
            var splitRegex = new Regex(@"\|split\|p([1234])\n(.*)\n(.*)|.+", RegexOptions.Multiline);
            var channelIdSet = new HashSet<int>(channelIds);
            var channelMessages = new Dictionary<int, List<string>>();
            foreach (int id in channelIdSet)
            {
                channelMessages[id] = [];
            }
            channelMessages[-1] = [];

            foreach (Match match in splitRegex.Matches(message))
            {
                string playerMatch = match.Groups[1].Value;
                string secretMessage = match.Groups[2].Value;
                string sharedMessage = match.Groups[3].Value;
                int player = string.IsNullOrEmpty(playerMatch) ? 0 : int.Parse(playerMatch);

                foreach (int channelId in channelIdSet)
                {
                    string line = match.Value;
                    if (player != 0)
                    {
                        line = (channelId == -1 || player == channelId) ? secretMessage : sharedMessage;
                        if (string.IsNullOrEmpty(line)) continue;
                    }
                    channelMessages[channelId].Add(line);
                }
            }
            return channelMessages;
        }
    }

    public class BattleOptions
    {
        public Format? Format { get; init; }
        public required Id FormatId { get; init; }
        public Action<string, StrListStrUnion>? Send { get; init; }
        public Prng? Prng { get; init; }
        public PrngSeed? PrngSeed { get; init; }
        public BoolStringUnion? Rated { get; init; }
        public PlayerOptions? P1 { get; init; }
        public PlayerOptions? P2 { get; init; }
        public PlayerOptions? P3 { get; init; }
        public PlayerOptions? P4 { get; init; }
        public bool? Debug { get; init; }
        public bool? ForceRandomChance { get; init; }
        public bool? Deserialized { get; init; }
        public bool? StrictChoices { get; init; }
    }

    public class EventListenerWithoutPriority
    {
        public required IEffect Effect { get; init; }
        public Pokemon? Target { get; init; }
        public int? Index { get; init; }
        public Delegate? Callback { get; init; }
        public EffectState? State { get; init; }
        public Delegate? End { get; init; }
        public object[]? EndCallArgs { get; init; }
        public required EventEffectHolder EffectHolder { get; init; }
    }

    public class EventListener : EventListenerWithoutPriority
    {
        public required IntFalseUnion Order { get; init; }
        public int Priority { get; init; }
        public int SubOrder { get; init; }
        public int? EffectOrder { get; init; }
        public required EffectState AbilityState { get; init; }
        public int? Speed { get; init; }
    }

    public enum RequestState
    {
        TeamPreview,
        Move,
        Switch,
        None,
    }

    public class FaintQueueEntry
    {
        public required Pokemon Target { get; init; }
        public Pokemon? Source { get; init; }
        public IEffect? Effect { get; init; }
    }

    public class SideSecretShared
    {
        public SideId Side { get; set; }
        public string Secret { get; set; } = string.Empty;
        public string Shared { get; set; } = string.Empty;
    }

    public delegate SideSecretShared SideSecretSharedFactory();

    public class Battle
    {
        public Id Id { get; init; }
        public bool DebugMode { get; init; }
        public bool? ForceRandomChance { get; init; }
        public bool Deserialized { get; init; }
        public bool StrictChoices { get; init; }
        public Format Format { get; init; }
        public EffectState FormatData { get; init; }
        public GameType GameType { get; init; }
        public int ActivePerHalf
        {
            get;
            init // 1, 2, or 3
            {
                if (value is 1 or 2 or 3)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "ActivePerHalf must be 1, 2, or 3.");
                }
            }
        }
        public Field Field { get; init; }
        public Side[] Sides
        {
            get;
            init // Array of sides, size 2 or 4
            {
                if (value.Length is 2 or 4)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("Sides must be an array of size 2 or 4.");
                }
            }
        }
        public PrngSeed PrngSeed { get; init; }
        public ModdedDex Dex { get; init; }
        public ModdedDex BaseDex { get; init; } // Added to emulate how the JavaScript version works
        public int Gen { get; init; }
        public RuleTable RuleTable { get; init; }
        public Prng Prng { get; set; }
        public BoolStringUnion Rated { get; init; }
        public bool ReportExactHp { get; init; }
        public bool ReportPercentages { get; init; }
        public bool SupportCancel { get; init; }

        public BattleActions Actions { get; init; }
        public BattleQueue Queue { get; init; }
        public List<FaintQueueEntry> FaintQueue { get; init; }

        public List<string> Log { get; init; }
        public List<string> InputLog { get; init; }
        public List<string> MessageLog { get; init; }
        public int SentLogPos { get; set; }
        public bool SentEnd { get; set; }
        public static bool SentRequests => true;

        public RequestState RequestState { get; init; }
        public int Turn { get; init; }
        public bool MidTurn { get; init; }
        public bool Started { get; init; }
        public bool Ended { get; init; }
        public string? Winner { get; init; }

        public IEffect Effect { get; set; }
        public EffectState EffectState { get; set; }

        public object Event { get; set; }
        public object? Events { get; init; }
        public int EventDepth { get; set; }

        public ActiveMove? ActiveMove { get; set; }
        public Pokemon? ActivePokemon { get; set; }
        public Pokemon? ActiveTarget { get; set; }

        public ActiveMove? LastMove { get; set; }
        public Id? LastSuccessfulMoveThisTurn { get; init; }
        public int LastMoveLine { get; init; }
        public int LastDamage { get; init; }
        public int EffectOrder { get; set; }
        public bool QuickClawRoll { get; init; }
        public List<int> SpeedOrder { get; init; }

        public object? TeamGenerator { get; init; }

        public HashSet<string> Hints { get; init; }

        // Constants
        public static string NotFail => "";
        public static int HitSubstitute => 0;
        public static bool Fail => false;
        public static object? SilentFail => null;

        public Action<string, StrListStrUnion> Send { get; init; }

        // Methods
        //public Func<int, int?, int> Trunc { get; init; }
        // public Func<object, int?, int?, int> ClampIntRange { get; init; }
        // public Func<object, Id> ToId { get; init; }

        public Battle(BattleOptions options, ModdedDex baseDex)
        {
            BaseDex = baseDex;
            Log = [];
            //Add(["t:"]);

            Format = options.Format ?? BaseDex.Formats.Get(options.FormatId.ToString(), true);

            // TODO: Add support for non-base formats
            //Dex = BaseDex.ForFormat(Format);
            Dex = BaseDex;
            Gen = Dex.Gen;
            RuleTable = Dex.Formats.GetRuleTable(Format);

            Id = new Id();

            DebugMode = Format.Debug || (options.Debug ?? false);
            // Require debug mode and explicitly passed true/false
            ForceRandomChance = (DebugMode && options.ForceRandomChance.HasValue)
                ? options.ForceRandomChance
                : null;

            Deserialized = options.Deserialized ?? false;
            StrictChoices = options.StrictChoices ?? false;
            var effectState = new EffectState() { Id = Format.Id, EffectOrder = 0 };
            FormatData = InitEffectState(effectState);
            GameType = Format.GameType;
            Field = new Field(this);
            Sides = new Side[options is { P1: not null, P2: not null } ? 2 : 4];

            if (GameType == GameType.Triples)
            {
                ActivePerHalf = 3;
            }
            else if (GameType == GameType.Doubles || Format.PlayerCount > 2)
            {
                ActivePerHalf = 2;
            }
            else
            {
                ActivePerHalf = 1;
            }

            Prng = options.Prng ?? new Prng(options.PrngSeed);
            PrngSeed = Prng.StartingSeed;
            Rated = options.Rated ?? false;
            ReportExactHp = Format.Debug;
            ReportPercentages = false;
            SupportCancel = false;

            Queue = new BattleQueue(this);
            Actions = new BattleActions(this);
            FaintQueue = [];

            InputLog = [];
            MessageLog = [];
            SentLogPos = 0;
            SentEnd = false;

            RequestState = RequestState.None;
            Turn = 0;
            MidTurn = false;
            Started = false;
            Ended = false;

            Effect = ConditionConstants.EmptyCondition; // Could be a different default effect
            EffectState = InitEffectState(new EffectState { Id = Id.Empty, EffectOrder = 0 });

            Event = new object(); // Placeholder for event object
            Events = null;
            EventDepth = 0;

            ActiveMove = null;
            ActivePokemon = null;
            ActiveTarget = null;

            LastMove = null;
            LastMoveLine = -1;
            LastSuccessfulMoveThisTurn = null;
            LastDamage = 0;
            EffectOrder = 0;
            QuickClawRoll = false;
            SpeedOrder = [];

            for (int i = 0; i < ActivePerHalf * 2; i++)
            {
                SpeedOrder.Add(i);
            }

            TeamGenerator = null;

            Hints = [];

            Send = options.Send ?? ((_, _) => { });

            // Create input options for logging
            Dictionary<string, object> inputOptions = new Dictionary<string, object>
            {
                ["formatid"] = options.FormatId,
                ["seed"] = PrngSeed,
                ["rated"] = Rated
            };

            // Version logging (if version system is implemented)
            // TODO: Implement version system
            /*
            if (__version?.Head != null)
            {
                InputLog.Add($"> version {__version.Head}");
            }
            if (__version?.Origin != null)
            {
                InputLog.Add($"> version - origin {__version.Origin}");
            }
            */

            InputLog.Add($"> start {JsonSerializer.Serialize(inputOptions)}");

            Add("gametype", GameType);

            // Process rules - timing is early enough to hook into ModifySpecies event
            foreach (string rule in from rule in RuleTable.Keys where
                         !"+-*!".Contains(rule[0]) let subFormat = Dex.Formats.Get(rule)
                            where subFormat.Exists let excludedHandlers = new HashSet<string>
                     {
                         "onBegin", "onTeamPreview", "onBattleStart", "onValidateRule",
                         "onValidateTeam", "onChangeSet", "onValidateSet"
                     } let hasEventHandler = subFormat.GetType()
                         .GetProperties()
                         .Any(prop => prop.Name.StartsWith("On") &&
                                      !excludedHandlers.Contains(prop.Name)) where hasEventHandler select rule)
            {
                Field.AddPseudoWeather(rule);
            }

            // Set up players
            SideId[] sideIds = new[] { SideId.P1, SideId.P2, SideId.P3, SideId.P4 };
            foreach (SideId sideId in sideIds)
            {
                PlayerOptions? playerOptions = sideId switch
                {
                    SideId.P1 => options.P1,
                    SideId.P2 => options.P2,
                    SideId.P3 => options.P3,
                    SideId.P4 => options.P4,
                    _ => null
                };

                if (playerOptions != null)
                {
                    SetPlayer(sideId, playerOptions);
                }
            }

        }

        public object ToJson()
        {
            throw new NotImplementedException();
            // TODO - implement JSON serialization
        }

        public static Battle FromJson(object serialized)
        {
            // TODO: Implement JSON deserialization
            throw new NotImplementedException();
        }

        public Side P1 => Sides[0];
        public Side P2 => Sides[1];
        public Side? P3 => Sides.Length > 2 ? Sides[2] : null;
        public Side? P4 => Sides.Length > 3 ? Sides[3] : null;

        public override string ToString()
        {
            return $"Battle: {Format}";
        }

        public int Random(int? m = null, int? n = null)
        {
            return (int)Prng.Random(m, n);
        }

        public bool RandomChance(int numerator, int denominator)
        {
            if (ForceRandomChance.HasValue)
                return ForceRandomChance.Value;
            return Prng.RandomChance(numerator, denominator);
        }

        public T Sample<T>(IReadOnlyList<T> items)
        {
            return Prng.Sample(items);
        }

        public void ResetRng(PrngSeed? seed = null)
        {
            Prng = new Prng(seed ?? PrngSeed);
            Add("message", "The battle's RNG was reset.");
        }

        public bool SuppressingAbility(Pokemon? target = null)
        {
            return ActivePokemon is { IsActive: true }
                   && (ActivePokemon != target || Gen < 8)
                   && ActiveMove != null
                   && (ActiveMove.IgnoreAbility ?? false)
                   && (target == null || !target.HasItem("Ability Shield"));
        }

        public void SetActiveMove(ActiveMove? move = null, Pokemon? pokemon = null, Pokemon? target = null)
        {
            ActiveMove = move;
            ActivePokemon = pokemon;
            ActiveTarget = target ?? pokemon;
        }

        public void ClearActiveMove(bool failed = false)
        {
            if (ActiveMove == null) return;
            if (!failed)
            {
                LastMove = ActiveMove;
            }
            ActiveMove = null;
            ActivePokemon = null;
            ActiveTarget = null;
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
        /// </summary>
        public static int ComparePriority(object objA, object objB)
        {
            AnyObject a = objA as AnyObject ?? new AnyObject(objA);
            AnyObject b = objB as AnyObject ?? new AnyObject(objB);

            // Order comparison (lower values first, null/false = last)
            // Using int.MaxValue as default for null/false
            int aOrder = a.Order switch
            {
                IntIntFalseUnion intOrder => intOrder.Value,
                _ => int.MaxValue
            };

            int bOrder = b.Order switch
            {
                IntIntFalseUnion intOrder => intOrder.Value,
                _ => int.MaxValue
            };

            int orderResult = aOrder.CompareTo(bOrder);
            if (orderResult != 0) return orderResult;

            // Priority comparison (higher values first)
            int priorityResult = Nullable.Compare(b.Priority, a.Priority);
            if (priorityResult != 0) return priorityResult;

            // Speed comparison (higher values first)
            int speedResult = (b.Speed ?? 0).CompareTo(a.Speed ?? 0);
            if (speedResult != 0) return speedResult;

            // SubOrder comparison (lower values first)
            int subOrderResult = Nullable.Compare(b.SubOrder, a.SubOrder);
            return subOrderResult != 0 ? subOrderResult :
                // EffectOrder comparison (lower values first)
                (a.EffectOrder ?? 0).CompareTo(b.EffectOrder ?? 0);
        }

        /// <summary>
        /// Comparison function for redirect order sorting.
        /// Used for sorting event handlers when fastExit is true.
        /// 
        /// 1. Priority, high to low (default 0)
        /// 2. Speed, high to low (default 0) 
        /// 3. AbilityState EffectOrder, low to high (only if both have ability states)
        /// </summary>
        public static int CompareRedirectOrder(object objA, object objB)
        {
            AnyObject a = objA as AnyObject ?? new AnyObject(objA);
            AnyObject b = objB as AnyObject ?? new AnyObject(objB);

            // Priority comparison (higher values first)
            int priorityResult = Nullable.Compare(b.Priority, a.Priority);
            if (priorityResult != 0) return priorityResult;

            // Speed comparison (higher values first)
            int speedResult = (b.Speed ?? 0).CompareTo(a.Speed ?? 0);
            if (a.AbilityState == null) return 0;
            if (b.AbilityState != null)
                return speedResult != 0
                    ? speedResult
                    :
                    // AbilityState EffectOrder comparison (lower values first, only if both have ability states)
                    a.AbilityState.EffectOrder.CompareTo(b.AbilityState.EffectOrder);
            return 0;
        }

        /// <summary>
        /// Comparison function for left-to-right order sorting.
        /// Used for specific events like "Invulnerability", "TryHit", "DamagingHit", "EntryHazard".
        /// 
        /// 1. Order, low to high (default last)
        /// 2. Priority, high to low (default 0)
        /// 3. Index, low to high (default 0) - for array position ordering
        /// </summary>
        public static int CompareLeftToRightOrder(object objA, object objB)
        {
            AnyObject a = objA as AnyObject ?? new AnyObject(objA);
            AnyObject b = objB as AnyObject ?? new AnyObject(objB);

            // Order comparison (lower values first, null/false = last)
            // Using 4294967296 as default for null/false (same as TypeScript)
            int aOrder = a.Order switch
            {
                IntIntFalseUnion intOrder => intOrder.Value,
                _ => int.MaxValue
            };

            int bOrder = b.Order switch
            {
                IntIntFalseUnion intOrder => intOrder.Value,
                _ => int.MaxValue
            };

            int orderResult = aOrder.CompareTo(bOrder);
            if (orderResult != 0) return orderResult;

            // Priority comparison (higher values first)
            int priorityResult = Nullable.Compare(b.Priority, a.Priority);
            return priorityResult != 0
                ? priorityResult
                :
                // Index comparison (lower values first) - for left-to-right array ordering
                (a.Index ?? 0).CompareTo(b.Index ?? 0);
        }

        /// <summary>Sort a list, resolving speed ties the way the games do.</summary>
        public void SpeedSort<T>(List<T> list, Func<T, T, int>? comparator = null)
        {
            comparator ??= (x, y) => ComparePriority(x!, y!);

            if (list.Count < 2) return;

            int sorted = 0;

            // This is a Selection Sort - not the fastest sort in general, but
            // actually faster than QuickSort for small arrays like the ones
            // SpeedSort is used for.
            // More importantly, it makes it easiest to resolve speed ties properly.
            while (sorted + 1 < list.Count)
            {
                List<int> nextIndexes = new List<int> { sorted };

                // Grab list of next indexes
                for (int i = sorted + 1; i < list.Count; i++)
                {
                    int delta = comparator(list[nextIndexes[0]], list[i]);
                    switch (delta)
                    {
                        case < 0:
                            continue;
                        case > 0:
                            nextIndexes = [i];
                            break;
                        case 0:
                            nextIndexes.Add(i);
                            break;
                    }
                }

                // Put list of next indexes where they belong
                for (int i = 0; i < nextIndexes.Count; i++)
                {
                    int index = nextIndexes[i];
                    if (index != sorted + i)
                    {
                        // nextIndexes is guaranteed to be in order, so it will never have
                        // been disturbed by an earlier swap
                        (list[sorted + i], list[index]) = (list[index], list[sorted + i]);
                    }
                }

                // Shuffle tied elements for fair randomization
                if (nextIndexes.Count > 1)
                {
                    Prng.Shuffle(list, sorted, sorted + nextIndexes.Count);
                }

                sorted += nextIndexes.Count;
            }
        }

        /// <summary>
        /// Runs an event with no source on each Pokémon on the field, in Speed order.
        /// </summary>
        public void EachEvent(string eventId, IEffect? effect = null, object? relayVar = null)
        {
            List<Pokemon> actives = GetAllActive();
            effect ??= Effect;

            // Sort by speed descending (fastest first)
            SpeedSort(actives, (a, b) => b.Speed.CompareTo(a.Speed));

            foreach (Pokemon pokemon in actives)
            {
                RunEvent(eventId, pokemon, null, effect, relayVar);
            }

            if (eventId == "Weather" && Gen >= 7)
            {
                // TODO: further research when updates happen
                EachEvent("Update");
            }
        }

        public void FieldEvent(string eventId, IReadOnlyList<Pokemon>? targets = null)
        {
            throw new NotImplementedException();
            // TODO: Implement field event handling logic
        }

        public object SingleEvent(
           string eventId,
           IEffect effect,
           SingleEventState state,
           SingleEventTarget target,
           SingleEventSource? source = null,
           IEffect? sourceEffect = null,
           object? relayVar = null,
           Delegate? customCallback = null)
        {
            // Check for infinite recursion or other stack issues
            if (EventDepth >= 8)
            {
                Add("message", "STACK LIMIT EXCEEDED");
                Add("message", "PLEASE REPORT IN BUG THREAD");
                Add("message", $"Event: {eventId}");
                Add("message", $"Parent event: {(Event is BattleEvent battleEvent ? battleEvent.Id :
                    "unknown")}");
                throw new StackOverflowException("Event stack depth exceeded");
            }

            if (Log.Count - SentLogPos > 1000)
            {
                Add("message", "LINE LIMIT EXCEEDED");
                Add("message", "PLEASE REPORT IN BUG THREAD");
                Add("message", $"Event: {eventId}");
                Add("message", $"Parent event: {(Event is BattleEvent battleEvent ? battleEvent.Id :
                    "unknown")}");
                throw new InvalidOperationException("Infinite loop detected");
            }

            // Optional debugging
            // Debug($"Event: {eventId} (depth {EventDepth})");

            // Handle relay variable defaults
            bool hasRelayVar = true;
            if (relayVar == null)
            {
                relayVar = true;
                hasRelayVar = false;
            }

            // Early exit conditions for suppressed effects

            // Status effect where the Pokémon's status has changed
            if (effect.EffectType == EffectType.Status &&
                target is PokemonSingleEventTarget pokemonTarget &&
                pokemonTarget.Pokemon.Status != effect.Id)
            {
                return relayVar; // Status has changed, don't trigger the effect
            }

            // Ability suppressed by Mold Breaker on switch-in
            if (eventId == "SwitchIn" &&
                effect.EffectType == EffectType.Ability &&
                effect is Ability { Flags.Breakable: true } &&
                target is PokemonSingleEventTarget moldBreakerTarget &&
                SuppressingAbility(moldBreakerTarget.Pokemon))
            {
                Debug($"{eventId} handler suppressed by Mold Breaker");
                return relayVar;
            }

            // Item ignored by the Pokémon
            if (eventId != "Start" &&
                eventId != "TakeItem" &&
                effect.EffectType == EffectType.Item &&
                target is PokemonSingleEventTarget itemTarget &&
                itemTarget.Pokemon.IgnoringItem())
            {
                Debug($"{eventId} handler suppressed by Embargo, Klutz or Magic Room");
                return relayVar;
            }

            // Ability ignored by the Pokémon
            if (eventId != "End" &&
                effect.EffectType == EffectType.Ability &&
                target is PokemonSingleEventTarget abilityTarget &&
                abilityTarget.Pokemon.IgnoringAbility())
            {
                Debug($"{eventId} handler suppressed by Gastro Acid or Neutralizing Gas");
                return relayVar;
            }

            // Weather suppressed by Abilities like Air Lock
            if (effect.EffectType == EffectType.Weather &&
                eventId != "FieldStart" &&
                eventId != "FieldResidual" &&
                eventId != "FieldEnd" &&
                Field.SuppressingWeather())
            {
                Debug($"{eventId} handler suppressed by Air Lock");
                return relayVar;
            }

            // Get the callback function
            Delegate? callback = customCallback ?? GetCallback(target, effect, $"on{eventId}");
            if (callback == null) return relayVar;

            // Save the current battle state
            IEffect parentEffect = Effect;
            EffectState parentEffectState = EffectState;
            object parentEvent = Event;

            try
            {
                // Update battle state for this event
                Effect = effect;
                EffectState = state switch
                {
                    EffectStateSingleEventState effectState => effectState.EffectState,
                    _ => InitEffectState(new EffectState { Id = Id.Empty, EffectOrder = 0 })
                };

                Event = new BattleEvent
                {
                    Id = eventId,
                    Target = target,
                    Source = source ?? throw new ArgumentNullException(nameof(source)),
                    Effect = sourceEffect
                };

                EventDepth++;

                // Build the argument list for the callback
                List<object> args = new List<object>();
                if (hasRelayVar)
                    args.Add(relayVar);

                args.Add(target);
                args.Add(source);
                args.Add(sourceEffect ?? ConditionConstants.EmptyCondition);

                // Invoke the callback
                object? returnVal;
                try
                {
                    returnVal = callback.DynamicInvoke(args.ToArray());
                }
                catch (Exception ex)
                {
                    Debug($"Error in event handler: {ex.Message}");
                    returnVal = null;
                }

                // Return either the callback's return value or the original relayVar
                return returnVal ?? relayVar;
            }
            finally
            {
                // Restore the previous battle state
                EventDepth--;
                Effect = parentEffect;
                EffectState = parentEffectState;
                Event = parentEvent;
            }
        }

        //// Helper class to represent the event context
        //private class BattleEvent
        //{
        //    public required string Id { get; set; }
        //    public required SingleEventTarget Target { get; set; }
        //    public required SingleEventSource Source { get; set; }
        //    public required IEffect Effect { get; set; }
        //}



        /// <summary>
        /// Runs an event with handlers across the battle.
        /// </summary>
        /// <param name="eventId">The event identifier (e.g. "Damage", "ModifyDamage", "Weather")</param>
        /// <param name="target">The primary target of the event (Pokemon, Pokemon[], Side or Battle)</param>
        /// <param name="source">The source causing the event (Pokemon or string identifier)</param>
        /// <param name="sourceEffect">The effect causing the event</param>
        /// <param name="relayVar">The initial value passed to handlers, often modified by them</param>
        /// <param name="onEffect">Whether to only run the handler on the sourceEffect</param>
        /// <param name="fastExit">Whether to use fast exit comparison for handlers</param>
        /// <returns>The final relayVar value after all handlers have processed it</returns>
        public object RunEvent(
            string eventId,
            RunEventTarget? target = null,
            RunEventSource? source = null,
            IEffect? sourceEffect = null,
            object? relayVar = null,
            bool onEffect = false,
            bool fastExit = false)
        {
            // Stack depth check to prevent infinite recursion
            if (EventDepth >= 8)
            {
                Add("message", "STACK LIMIT EXCEEDED");
                Add("message", "PLEASE REPORT IN BUG THREAD");
                Add("message", $"Event: {eventId}");
                Add("message", $"Parent event: {(Event is BattleEvent battleEvent ? battleEvent.Id : "unknown")}");
                throw new StackOverflowException("Stack overflow in event system");
            }

            // Default target is this battle
            target ??= new BattleRunEventTarget(this);

            // Determine effect source for handler lookup
            Pokemon? effectSource = null;
            if (source is PokemonRunEventSource pokemonSource)
            {
                effectSource = pokemonSource.Pokemon;
            }

            // Find all handlers for this event
            List<EventListener> handlers = FindEventHandlers(target switch
            {
                PokemonRunEventTarget pokemonTarget => new PokemonFindEventHandlersTarget(pokemonTarget.Pokemon),
                PokemonListRunEventTarget pokemonListTarget => new PokemonListFindEventHandlersTarget(pokemonListTarget.PokemonList),
                SideRunEventTarget sideTarget => new SideFindEventHandlersTarget(sideTarget.Side),
                BattleRunEventTarget => new BattleFindEventHandlersTarget(this),
                _ => throw new ArgumentException($"Unsupported target type: {target.GetType().Name}")
            }, eventId, effectSource);

            // Add source effect handler if specified
            if (onEffect)
            {
                if (sourceEffect == null)
                {
                    throw new ArgumentException("onEffect specified without a sourceEffect");
                }

                Delegate? callback = GetCallback(target switch
                {
                    PokemonRunEventTarget pokemonTarget => new PokemonGetCallbackTarget(pokemonTarget.Pokemon),
                    SideRunEventTarget sideTarget => new SideGetCallbackTarget(sideTarget.Side),
                    BattleRunEventTarget => new BattleGetCallbackTarget(this),
                    _ => throw new ArgumentException("Array targets not supported with onEffect")
                }, sourceEffect, $"on{eventId}");

                if (callback != null)
                {
                    if (target is PokemonListRunEventTarget)
                    {
                        throw new ArgumentException("Array targets not supported with onEffect");
                    }

                    EventEffectHolder effectHolder = target switch
                    {
                        PokemonRunEventTarget pokemonTarget => (EventEffectHolder)pokemonTarget.Pokemon,
                        SideRunEventTarget sideTarget => sideTarget.Side,
                        BattleRunEventTarget => this,
                        _ => throw new ArgumentException($"Unsupported target type: {target.GetType().Name}")
                    };

                    EventListenerWithoutPriority listenerWithoutPriority = new EventListenerWithoutPriority
                    {
                        Effect = sourceEffect,
                        Callback = callback,
                        State = InitEffectState(new EffectState { Id = Id.Empty, EffectOrder = 0 }),
                        End = null,
                        EffectHolder = effectHolder
                    };

                    EventListener resolvedListener = ResolvePriority(listenerWithoutPriority, $"on{eventId}");
                    handlers.Insert(0, resolvedListener); // Add to beginning with unshift
                }
            }

            // Sort handlers based on event type
            if (new[] { "Invulnerability", "TryHit", "DamagingHit", "EntryHazard" }.Contains(eventId))
            {
                handlers.Sort(CompareLeftToRightOrder);
            }
            else if (fastExit)
            {
                handlers.Sort(CompareRedirectOrder);
            }
            else
            {
                SpeedSort(handlers);
            }

            // Set up relay variables
            bool hasRelayVar = true;
            List<object> args = new List<object>(); // Arguments to pass to callbacks

            if (relayVar == null)
            {
                relayVar = true;
                hasRelayVar = false;
            }
            else
            {
                args.Add(relayVar);
            }

            // Add standard arguments
            args.Add(target);
            args.Add(source ?? new NullRunEventSource());
            args.Add(sourceEffect ?? ConditionConstants.EmptyCondition);

            // Save parent event and set up this event
            object parentEvent = Event;
            BattleEvent newEvent = new BattleEvent
            {
                Id = eventId,
                Target = target,
                Source = source ?? throw new ArgumentNullException(nameof(source)),
                Effect = sourceEffect,
                Modifier = 1
            };
            Event = newEvent;
            EventDepth++;

            try
            {
                // Handle array targets specially
                List<object>? targetRelayVars = null;
                if (target is PokemonListRunEventTarget pokemonListTarget)
                {
                    targetRelayVars = [];
                    if (relayVar is IEnumerable<object> relayVarArray)
                    {
                        // Copy array values
                        targetRelayVars.AddRange(relayVarArray);
                    }
                    else
                    {
                        // Initialize with true for each target
                        for (int i = 0; i < pokemonListTarget.PokemonList.Count; i++)
                        {
                            targetRelayVars.Add(true);
                        }
                    }
                }

                // Process each handler
                foreach (EventListener handler in handlers)
                {
                    // Handle array targets - if this handler has an index
                    if (handler.Index.HasValue)
                    {
                        int index = handler.Index.Value;

                        // Skip if this target's relay var is falsy (unless it's 0 for DamagingHit)
                        if (targetRelayVars != null &&
                            index < targetRelayVars.Count &&
                            !IsTruthy(targetRelayVars[index]) &&
                            !(Equals(targetRelayVars[index], 0) && eventId == "DamagingHit"))
                        {
                            continue;
                        }

                        // Update target in args and event
                        if (handler.Target is { } targetPokemon)
                        {
                            args[hasRelayVar ? 1 : 0] = targetPokemon;
                            newEvent.Target = new PokemonRunEventTarget(targetPokemon);
                        }

                        // Update relay var for this target
                        if (hasRelayVar && targetRelayVars != null && index < targetRelayVars.Count)
                        {
                            args[0] = targetRelayVars[index];
                        }
                    }

                    IEffect effect = handler.Effect;
                    EventEffectHolder effectHolder = handler.EffectHolder;

                    // Check if this handler should be suppressed

                    switch (effect.EffectType)
                    {
                        // Status has changed
                        case EffectType.Status when
                            effectHolder is PokemonEventEffectHolder pokemonEffectHolder &&
                            pokemonEffectHolder.Pokemon.Status != effect.Id:
                            continue;
                        // Ability suppressed by Mold Breaker
                        case EffectType.Ability when
                            effect is Ability { Flags.Breakable: true } &&
                            SuppressingAbility(effectHolder switch
                            {
                                PokemonEventEffectHolder pokemonHolder => pokemonHolder.Pokemon,
                                _ => null
                            }):
                            // Always suppress if breakable flag is set
                            Debug($"{eventId} handler suppressed by Mold Breaker");
                            continue;
                        // Custom abilities without num property - handle specific events
                        case EffectType.Ability when effect is Ability { Num: 0 }:
                        {
                            // These events are suppressed for custom abilities when Mold Breaker is active
                            HashSet<string> attackingEvents = new HashSet<string>
                            {
                                "BeforeMove", "BasePower", "Immunity", "RedirectTarget",
                                "Heal", "SetStatus", "CriticalHit", "ModifyAtk", "ModifyDef",
                                "ModifySpA", "ModifySpD", "ModifySpe", "ModifyAccuracy", "ModifyBoost",
                                "ModifyDamage", "ModifySecondaries", "ModifyWeight", "TryAddVolatile",
                                "TryHit", "TryHitSide", "TryMove", "Boost", "DragOut", "Effectiveness"
                            };

                            if (attackingEvents.Contains(eventId) &&
                                SuppressingAbility(effectHolder switch
                                {
                                    PokemonEventEffectHolder pokemonHolder => pokemonHolder.Pokemon,
                                    _ => null
                                }) || eventId == "Damage" &&
                                sourceEffect?.EffectType == EffectType.Move &&
                                SuppressingAbility(effectHolder switch
                                {
                                    PokemonEventEffectHolder pokemonHolder => pokemonHolder.Pokemon,
                                    _ => null
                                }))
                            {
                                Debug($"{eventId} handler suppressed by Mold Breaker");
                                continue;
                            }

                            break;
                        }
                    }

                    // Item ignored by Embargo, Klutz or Magic Room
                    if (eventId != "Start" && eventId != "SwitchIn" && eventId != "TakeItem" &&
                        effect.EffectType == EffectType.Item &&
                        effectHolder is PokemonEventEffectHolder itemHolderPokemon &&
                        itemHolderPokemon.Pokemon.IgnoringItem())
                    {
                        if (eventId != "Update")
                        {
                            Debug($"{eventId} handler suppressed by Embargo, Klutz or Magic Room");
                        }
                        continue;
                    }

                    // Ability ignored by Gastro Acid or Neutralizing Gas

                    if (eventId != "End" &&
                        effect.EffectType == EffectType.Ability &&
                        effectHolder is PokemonEventEffectHolder abilityHolderPokemon &&
                        abilityHolderPokemon.Pokemon.IgnoringAbility())
                    {
                        if (eventId != "Update")
                        {
                            Debug($"{eventId} handler suppressed by Gastro Acid or Neutralizing Gas");
                        }
                        continue;
                    }

                    // Weather suppressed by Air Lock
                    if ((effect.EffectType == EffectType.Weather || eventId == "Weather") &&
                        eventId != "Residual" && eventId != "End" &&
                        Field.SuppressingWeather())
                    {
                        Debug($"{eventId} handler suppressed by Air Lock");
                        continue;
                    }

                    // Invoke the callback and get the result
                    object? returnVal;
                    if (handler.Callback is { } callback)
                    {
                        // Save battle state
                        IEffect parentEffect = Effect;
                        EffectState parentEffectState = EffectState;

                        // Set up state for this handler
                        Effect = handler.Effect;
                        EffectState = handler.State ?? InitEffectState(new EffectState { Id = Id.Empty, EffectOrder = 0 });
                        EffectState.ExtraData["target"] = handler.EffectHolder;

                        try
                        {
                            // Call the handler
                            returnVal = callback.DynamicInvoke(args.ToArray());
                        }
                        finally
                        {
                            // Restore battle state
                            Effect = parentEffect;
                            EffectState = parentEffectState;
                        }
                    }
                    else
                    {
                        // Non-function callbacks are treated as direct return values
                        returnVal = handler.Callback;
                    }

                    // Process the return value
                    if (returnVal == null) continue;
                    relayVar = returnVal;

                    // Early exit if result is falsy or fastExit is true
                    if (!IsTruthy(relayVar) || fastExit)
                    {
                        if (handler.Index.HasValue && targetRelayVars != null &&
                            handler.Index.Value < targetRelayVars.Count)
                        {
                            // Update this target's relay var
                            targetRelayVars[handler.Index.Value] = relayVar;

                            // Check if all targets are falsy to exit the whole event
                            if (targetRelayVars.All(v => !IsTruthy(v)))
                            {
                                break;
                            }
                        }
                        else
                        {
                            // For non-array targets, just exit
                            break;
                        }
                    }

                    // Update the args for the next handler
                    if (hasRelayVar)
                    {
                        args[0] = relayVar;
                    }
                }

                // Apply modifiers to numeric results
                if (relayVar is not int intRelayVar)
                    return target is PokemonListRunEventTarget ? targetRelayVars! : relayVar;
                int modifier = newEvent.Modifier;
                if (modifier != 1)
                {
                    relayVar = Modify(intRelayVar, modifier);
                }

                // Return the appropriate result
                return target is PokemonListRunEventTarget ? targetRelayVars! : relayVar;
            }
            finally
            {
                // Always clean up event state
                EventDepth--;
                Event = parentEvent;
            }
        }

        // Helper method to check if a value should be considered "truthy"
        private static bool IsTruthy(object? value)
        {
            return value switch
            {
                null => false,
                bool boolValue => boolValue,
                int intValue => intValue != 0,
                double doubleValue => doubleValue != 0,
                string stringValue => !string.IsNullOrEmpty(stringValue),
                _ => true // Objects are truthy
            };
        }

        // Event context class to store current event data
        public class BattleEvent
        {
            public required string Id { get; set; }
            public required RunEventTarget Target { get; set; }
            public required RunEventSource Source { get; set; }
            public required IEffect? Effect { get; set; }
            public int Modifier { get; set; } = 1;
        }

        public object PriorityEvent(
            string eventId,
            PriorityEventTarget target,
            Pokemon? source = null,
            IEffect? effect = null,
            object? relayVar = null,
            bool onEffect = false)
        {
            throw new NotImplementedException();
        }

        public EventListener ResolvePriority(EventListenerWithoutPriority listener, string callbackName)
        {
            // Get explicit priority values from the effect if they exist
            int? order = GetEffectProperty<int>(listener.Effect, $"{callbackName}Order");
            int priority = GetEffectProperty<int>(listener.Effect, $"{callbackName}Priority") ?? 0;
            int subOrder = GetEffectProperty<int>(listener.Effect, $"{callbackName}SubOrder") ?? 0;

            // If subOrder is not explicitly defined, calculate a default based on effect type
            if (subOrder == 0)
            {
                subOrder = GetDefaultSubOrder(listener);
            }

            int? effectOrder = null;
            // For certain events, effectOrder is used as a tiebreaker
            if (callbackName.EndsWith("SwitchIn") || callbackName.EndsWith("RedirectTarget"))
            {
                effectOrder = listener.State?.EffectOrder;
            }

            int? speed = null;
            if (listener.EffectHolder is not PokemonEventEffectHolder pokemonEvent)
                return new EventListener
                {
                    Effect = listener.Effect,
                    Callback = listener.Callback,
                    State = listener.State,
                    End = listener.End,
                    EndCallArgs = listener.EndCallArgs,
                    EffectHolder = listener.EffectHolder,
                    Target = listener.Target,
                    Index = listener.Index,
                    Order = order ?? throw new InvalidOperationException(),
                    Priority = priority,
                    SubOrder = subOrder,
                    EffectOrder = effectOrder,
                    Speed = speed,
                    AbilityState = new EffectState()
                    {
                        Id = Id.Empty,
                        EffectOrder = 0,
                    }
                };
            Pokemon pokemon = pokemonEvent.Pokemon;
            speed = pokemon.Speed;
            // Special speed calculation for Magic Bounce
            if (listener.Effect is { EffectType: EffectType.Ability, Name: "Magic Bounce" } &&
                callbackName == "onAllyTryHitSide")
            {
                speed = pokemon.GetStat(StatIdExceptHp.Spe, true, true);
            }
            // Adjust speed for onSwitchIn events to handle speed ties deterministically
            if (!callbackName.EndsWith("SwitchIn"))
                return new EventListener
                {
                    Effect = listener.Effect,
                    Callback = listener.Callback,
                    State = listener.State,
                    End = listener.End,
                    EndCallArgs = listener.EndCallArgs,
                    EffectHolder = listener.EffectHolder,
                    Target = listener.Target,
                    Index = listener.Index,
                    Order = order ?? throw new InvalidOperationException(),
                    Priority = priority,
                    SubOrder = subOrder,
                    EffectOrder = effectOrder,
                    Speed = speed,
                    AbilityState = new EffectState()
                    {
                        Id = Id.Empty,
                        EffectOrder = 0,
                    },
                };
            int fieldPositionValue = pokemon.Side.N * Sides.Length + pokemon.Position;
            int speedIndex = SpeedOrder.IndexOf(fieldPositionValue);
            if (speedIndex >= 0)
            {
                // Subtract a small fraction to maintain order without changing integer speed
                speed -= speedIndex / (ActivePerHalf * 2);
            }

            // Construct the final EventListener with all priority values
            return new EventListener
            {
                Effect = listener.Effect,
                Callback = listener.Callback,
                State = listener.State,
                End = listener.End,
                EndCallArgs = listener.EndCallArgs,
                EffectHolder = listener.EffectHolder,
                Target = listener.Target,
                Index = listener.Index,
                Order = order ?? throw new InvalidOperationException(),
                Priority = priority,
                SubOrder = subOrder,
                EffectOrder = effectOrder,
                Speed = speed,
                AbilityState = new EffectState()
                {
                    Id = Id.Empty,
                    EffectOrder = 0,
                },
            };
        }

        private static int GetDefaultSubOrder(EventListenerWithoutPriority listener)
        {
            // Default sub-order values based on extensive community research
            Dictionary<EffectType, int> effectTypeOrder = new Dictionary<EffectType, int>
            {
                [EffectType.Condition] = 2,
                [EffectType.Weather] = 5,
                [EffectType.Format] = 5,
                [EffectType.Rule] = 5,
                [EffectType.Ruleset] = 5,
                [EffectType.Ability] = 7,
                [EffectType.Item] = 8,
            };

            if (!effectTypeOrder.TryGetValue(listener.Effect.EffectType, out int defaultSubOrder)) return 0;
            switch (listener.Effect.EffectType)
            {
                // Refine sub-order for specific conditions and abilities
                case EffectType.Condition when listener.EffectHolder is SideEventEffectHolder:
                {
                    // Check if it's a slot-specific condition (e.g., Spikes) or a side-wide one (e.g., Tailwind)
                    bool isSlotCondition = listener.State?.ExtraData.ContainsKey("isSlotCondition") ?? false;
                    return isSlotCondition ? 3 : 4;
                }
                case EffectType.Condition when listener.EffectHolder is FieldEventEffectHolder:
                    return 5; // Field conditions like terrains
                case EffectType.Ability when listener.Effect.Name is "Poison Touch" or "Perish Body":
                    return 6;
                case EffectType.Ability when listener.Effect.Name == "Stall":
                    return 9;
                case EffectType.Pokemon:
                case EffectType.Move:
                case EffectType.Item:
                case EffectType.Format:
                case EffectType.Nature:
                case EffectType.Ruleset:
                case EffectType.Weather:
                case EffectType.Status:
                case EffectType.Terrain:
                case EffectType.Rule:
                case EffectType.ValidatorRule:
                case EffectType.Learnset:
                default:
                    return defaultSubOrder;
            }
        }

        // Helper to get a property value from an effect using reflection
        private T? GetEffectProperty<T>(IEffect effect, string propertyName) where T : struct
        {
            PropertyInfo? propInfo = effect.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (propInfo != null && propInfo.PropertyType == typeof(T))
            {
                return (T?)propInfo.GetValue(effect);
            }
            return null;
        }
        public Delegate? GetCallback(GetCallbackTarget target, IEffect effect, string callbackName)
        {
            Delegate? callback = callbackName switch
            {
                "onAnySwitchIn" => effect.OnAnySwitchIn,
                "onSwitchIn" => effect.OnSwitchIn,
                "onStart" => effect.OnStart,
                _ => GetCallbackFromEffect(effect, callbackName) // fallback to reflection
            };

            // The special Gen 5+ logic is now much cleaner:
            if (callback == null &&
                target is PokemonGetCallbackTarget &&
                Gen >= 5 &&
                callbackName == "onSwitchIn" &&
                effect.OnAnySwitchIn == null &&  // Direct property access!
                (IsAbilityOrItem(effect) || IsInnateAbilityOrItem(effect)) &&
                effect.OnStart != null)
            {
                callback = effect.OnStart;
            }

            return callback;
        }

        // Helper method to get a callback from an effect using reflection
        private static Delegate? GetCallbackFromEffect(IEffect effect, string callbackName)
        {
            // Use reflection to get the callback by name for other callbacks not in IEffect
            Type type = effect.GetType();
            PropertyInfo? property = type.GetProperty(callbackName, BindingFlags.Public | BindingFlags.Instance);

            if (property?.PropertyType == typeof(Delegate) ||
                property?.PropertyType.IsSubclassOf(typeof(Delegate)) == true ||
                property?.PropertyType.IsSubclassOf(typeof(MulticastDelegate)) == true)
            {
                return property.GetValue(effect) as Delegate;
            }

            return null;
        }

        // Helper methods to check if an effect is an ability or item
        private static bool IsAbilityOrItem(IEffect effect)
        {
            return effect.EffectType is EffectType.Ability or EffectType.Item;
        }

        private static bool IsInnateAbilityOrItem(IEffect effect)
        {
            // Innate abilities/items - Status effects with ability/item prefix
            if (effect.EffectType != EffectType.Status) return false;

            string[] idParts = effect.Id.ToString().Split(':');
            return idParts.Length > 0 && (idParts[0] == "ability" || idParts[0] == "item");
        }

        public List<EventListener> FindEventHandlers(FindEventHandlersTarget target,
            string eventName, Pokemon? source = null)
        {
            var handlers = new List<EventListener>();
            var excludedEvents = new HashSet<string> { "BeforeTurn", "Update", "Weather", "WeatherChange", "TerrainChange" };
            bool prefixedHandlers = !excludedEvents.Contains(eventName);

            switch (target)
            {
                // Case 1: Target is an array of Pokemon
                case PokemonListFindEventHandlersTarget pokemonArrayTarget:
                {
                    for (int i = 0; i < pokemonArrayTarget.PokemonList.Count; i++)
                    {
                        Pokemon pokemon = pokemonArrayTarget.PokemonList[i];
                        var curHandlers =
                            FindEventHandlers(new PokemonFindEventHandlersTarget(pokemon), eventName, source);
                        foreach (EventListener handler in curHandlers)
                        {
                            // Re-assign target and index for handlers from the array
                            var updatedListener = new EventListener
                            {
                                Effect = handler.Effect,
                                Callback = handler.Callback,
                                State = handler.State,
                                End = handler.End,
                                EndCallArgs = handler.EndCallArgs,
                                EffectHolder = handler.EffectHolder,
                                Target = pokemon,
                                Index = i,
                                Order = handler.Order,
                                Priority = handler.Priority,
                                SubOrder = handler.SubOrder,
                                EffectOrder = handler.EffectOrder,
                                Speed = handler.Speed,
                                AbilityState = new EffectState()
                                {
                                    Id = Id.Empty,
                                    EffectOrder = 0,
                                },
                            };
                            handlers.Add(updatedListener);
                        }
                    }
                    return handlers;
                }
                // Case 2: Target is a single Pokemon
                case PokemonFindEventHandlersTarget pokemonTarget when
                    (pokemonTarget.Pokemon.IsActive || source?.IsActive == true):
                {
                    handlers.AddRange(FindPokemonEventHandlers(pokemonTarget.Pokemon,
                        $"on{eventName}"));
                    if (prefixedHandlers)
                    {
                        foreach (Pokemon allyActive in pokemonTarget.Pokemon.AlliesAndSelf())
                        {
                            handlers.AddRange(FindPokemonEventHandlers(allyActive,
                                $"onAlly{eventName}"));
                            handlers.AddRange(FindPokemonEventHandlers(allyActive,
                                $"onAny{eventName}"));
                        }
                        foreach (Pokemon foeActive in pokemonTarget.Pokemon.Foes())
                        {
                            handlers.AddRange(FindPokemonEventHandlers(foeActive,
                                $"onFoe{eventName}"));
                            handlers.AddRange(FindPokemonEventHandlers(foeActive,
                                $"onAny{eventName}"));
                        }
                    }
                    // After handling Pokemon-specific events, bubble up to the side level
                    target = new SideFindEventHandlersTarget(pokemonTarget.Pokemon.Side);
                    break;
                }
            }

            // Handle source-specific events
            if (source != null && prefixedHandlers)
            {
                handlers.AddRange(FindPokemonEventHandlers(source, $"onSource{eventName}"));
            }

            // Case 3: Target is a Side (or has bubbled up from a Pokemon)
            if (target is SideFindEventHandlersTarget sideTarget)
            {
                foreach (Side side in Sides)
                {
                    // Handle events on all active Pokemon on the side
                    foreach (Pokemon activePokemon in side.Active)
                    {
                        if (side == sideTarget.Side || side == sideTarget.Side.AllySide)
                        {
                            handlers.AddRange(FindPokemonEventHandlers(activePokemon, $"on{eventName}"));
                        }
                        else if (prefixedHandlers)
                        {
                            handlers.AddRange(FindPokemonEventHandlers(activePokemon, $"onFoe{eventName}"));
                        }
                        if (prefixedHandlers)
                        {
                            handlers.AddRange(FindPokemonEventHandlers(activePokemon,
                                $"onAny{eventName}"));
                        }
                    }

                    // Handle events on the side itself
                    if (side is { N: >= 2, AllySide: not null }) continue;
                    if (side == sideTarget.Side || side == sideTarget.Side.AllySide)
                    {
                        handlers.AddRange(FindSideEventHandlers(side, $"on{eventName}"));
                    }
                    else if (prefixedHandlers)
                    {
                        handlers.AddRange(FindSideEventHandlers(side, $"onFoe{eventName}"));
                    }
                    if (prefixedHandlers)
                    {
                        handlers.AddRange(FindSideEventHandlers(side, $"onAny{eventName}"));
                    }
                }
            }

            // Finally, add field-level and battle-level handlers
            handlers.AddRange(FindFieldEventHandlers(Field, $"on{eventName}"));
            handlers.AddRange(FindBattleEventHandlers($"on{eventName}"));

            return handlers;
        }
        public List<EventListener> FindPokemonEventHandlers(Pokemon pokemon,
            string callbackName, string? getKey = null)
        {
            var handlers = new List<EventListener>();

            // Handle status effects
            Condition status = pokemon.GetStatus();
            Delegate? callback = GetCallback(pokemon, status, callbackName);

            bool hasStatusCallback = callback != null;
            bool hasStatusSpecialKey = !string.IsNullOrEmpty(getKey) &&
                                       HasProperty(pokemon.StatusState, getKey);

            if (hasStatusCallback || hasStatusSpecialKey)
            {
                var statusListenerWithoutPriority = new EventListenerWithoutPriority
                {
                    Effect = status,
                    Callback = callback,
                    State = pokemon.StatusState,
                    End = CreateClearStatusDelegate(pokemon),
                    EffectHolder = pokemon
                };

                EventListener statusEventListener = ResolvePriority(statusListenerWithoutPriority, callbackName);
                handlers.Add(statusEventListener);
            }

            // Handle volatile status effects
            foreach ((string id, EffectState volatileState) in pokemon.Volatiles)
            {
                // Get the volatile condition from the dex
                Condition vol = Dex.Conditions.GetById(new Id(id));
                Delegate? volatileCallback = GetCallback(pokemon, vol, callbackName);

                bool hasVolatileCallback = volatileCallback != null;
                bool hasVolatileSpecialKey = !string.IsNullOrEmpty(getKey) &&
                                             HasProperty(volatileState, getKey);

                if (!hasVolatileCallback && !hasVolatileSpecialKey) continue;
                EventListenerWithoutPriority volatileListenerWithoutPriority = new EventListenerWithoutPriority
                {
                    Effect = vol,
                    Callback = volatileCallback,
                    State = volatileState,
                    End = CreateRemoveVolatileDelegate(pokemon),
                    EffectHolder = pokemon
                };

                EventListener volatileEventListener = ResolvePriority(volatileListenerWithoutPriority, callbackName);
                handlers.Add(volatileEventListener);
            }

            // Handle ability
            Ability ability = pokemon.GetAbility();
            Delegate? abilityCallback = GetCallback(pokemon, ability, callbackName);

            bool hasAbilityCallback = abilityCallback != null;
            bool hasAbilitySpecialKey = !string.IsNullOrEmpty(getKey) &&
                                       HasProperty(pokemon.AbilityState, getKey);

            if (hasAbilityCallback || hasAbilitySpecialKey)
            {
                EventListenerWithoutPriority abilityListenerWithoutPriority = new EventListenerWithoutPriority
                {
                    Effect = ability,
                    Callback = abilityCallback,
                    State = pokemon.AbilityState,
                    End = CreateClearAbilityDelegate(pokemon),
                    EffectHolder = pokemon
                };

                EventListener abilityEventListener = ResolvePriority(abilityListenerWithoutPriority, callbackName);
                handlers.Add(abilityEventListener);
            }

            // Handle item
            Item item = pokemon.GetItem();
            Delegate? itemCallback = GetCallback(pokemon, item, callbackName);

            bool hasItemCallback = itemCallback != null;
            bool hasItemSpecialKey = !string.IsNullOrEmpty(getKey) &&
                                    HasProperty(pokemon.ItemState, getKey);

            if (hasItemCallback || hasItemSpecialKey)
            {
                EventListenerWithoutPriority itemListenerWithoutPriority = new EventListenerWithoutPriority
                {
                    Effect = item,
                    Callback = itemCallback,
                    State = pokemon.ItemState,
                    End = CreateClearItemDelegate(pokemon),
                    EffectHolder = pokemon
                };

                EventListener itemEventListener = ResolvePriority(itemListenerWithoutPriority, callbackName);
                handlers.Add(itemEventListener);
            }

            // Handle species (base species)
            Species species = pokemon.BaseSpecies;
            Delegate? speciesCallback = GetCallback(pokemon, species, callbackName);

            if (speciesCallback != null)
            {
                EventListenerWithoutPriority speciesListenerWithoutPriority = new EventListenerWithoutPriority
                {
                    Effect = species,
                    Callback = speciesCallback,
                    State = pokemon.SpeciesState,
                    End = CreateEmptyDelegate(), // Empty function like in JS
                    EffectHolder = pokemon
                };

                EventListener speciesEventListener = ResolvePriority(speciesListenerWithoutPriority, callbackName);
                handlers.Add(speciesEventListener);
            }

            // Handle slot conditions
            Side side = pokemon.Side;
            if (pokemon.Position >= side.SlotConditions.Count) return handlers;
            Dictionary<string, EffectState> slotConditionsForPosition = side.SlotConditions[pokemon.Position];
            foreach (KeyValuePair<string, EffectState> slotKvp in slotConditionsForPosition)
            {
                string conditionId = slotKvp.Key;
                EffectState slotConditionState = slotKvp.Value;

                // Get the slot condition from the dex
                Condition slotCondition = Dex.Conditions.GetById(new Id(conditionId));
                Delegate? slotCallback = GetCallback(pokemon, slotCondition, callbackName);

                bool hasSlotCallback = slotCallback != null;
                bool hasSlotSpecialKey = !string.IsNullOrEmpty(getKey) &&
                                         HasProperty(slotConditionState, getKey);

                if (!hasSlotCallback && !hasSlotSpecialKey) continue;
                EventListenerWithoutPriority slotListenerWithoutPriority = new EventListenerWithoutPriority
                {
                    Effect = slotCondition,
                    Callback = slotCallback,
                    State = slotConditionState,
                    End = CreateRemoveSlotConditionDelegate(side),
                    EndCallArgs = [side, pokemon, slotCondition.Id],
                    EffectHolder = pokemon
                };

                EventListener slotEventListener = ResolvePriority(slotListenerWithoutPriority, callbackName);
                handlers.Add(slotEventListener);
            }

            return handlers;
        }

        // Helper methods to create delegates for Pokemon-specific cleanup functions
        private static Delegate CreateClearStatusDelegate(Pokemon pokemon)
        {
            return new Func<bool>(pokemon.ClearStatus);
        }

        private static Delegate CreateRemoveVolatileDelegate(Pokemon pokemon)
        {
            return new Func<string, bool>(pokemon.RemoveVolatile);
        }

        private static Delegate CreateClearAbilityDelegate(Pokemon pokemon)
        {
            return new Func<Id?>(pokemon.ClearAbility);
        }

        private static Delegate CreateClearItemDelegate(Pokemon pokemon)
        {
            return new Func<bool>(pokemon.ClearItem);
        }

        private static Delegate CreateEmptyDelegate()
        {
            return new Action(() => { }); // Empty function
        }

        private static Delegate CreateRemoveSlotConditionDelegate(Side side)
        {
            return new Func<Side, Pokemon, Id, bool>((_, p, id) => side.RemoveSlotCondition(p, id.ToString()));
        }

        //public List<EventListener> FindBattleEventHandlers(string callbackName,
        //    string? getKey = null, Pokemon? customHolder = null)
        //{
        //    throw new NotImplementedException();
        //}

        public List<EventListener> FindBattleEventHandlers(string callbackName,
            string? getKey = null, Pokemon? customHolder = null)
        {
            List<EventListener> handlers = new List<EventListener>();

            // Handle format-level callbacks
            Delegate? callback = GetCallback(this, Format, callbackName);

            bool hasFormatCallback = callback != null;
            bool hasFormatSpecialKey = !string.IsNullOrEmpty(getKey) &&
                                      HasProperty(FormatData, getKey);

            if (hasFormatCallback || hasFormatSpecialKey)
            {
                EventListenerWithoutPriority formatListenerWithoutPriority = new EventListenerWithoutPriority
                {
                    Effect = Format,
                    Callback = callback,
                    State = FormatData,
                    End = null, // Format handlers typically don't have cleanup
                    EffectHolder = customHolder ?? throw new ArgumentNullException(nameof(customHolder))
                };

                EventListener formatEventListener = ResolvePriority(formatListenerWithoutPriority, callbackName);
                handlers.Add(formatEventListener);
            }

            // Handle dynamic event handlers (if Events is implemented)
            if (Events is not Dictionary<string, List<EventHandlerData>> eventDict) return handlers;
            if (!eventDict.TryGetValue(callbackName, out List<EventHandlerData>? eventHandlers)) return handlers;
            handlers.AddRange(from handler in eventHandlers
                let state = handler.Target.EffectType == EffectType.Format
                    ? FormatData
                    : null
                select new EventListener
                {
                    Effect = handler.Target,
                    Callback = handler.Callback,
                    State = state,
                    End = null,
                    EffectHolder = (EventEffectHolder)customHolder ?? this,
                    Priority = handler.Priority,
                    Order = handler.Order,
                    SubOrder = handler.SubOrder,
                    EffectOrder = handler.EffectOrder,
                    Speed = handler.Speed,
                    AbilityState = new EffectState() { Id = Id.Empty, EffectOrder = 0, },
                });

            return handlers;
        }

        // Helper class for dynamic event handler data
        public class EventHandlerData
        {
            public required IEffect Target { get; init; }
            public required Delegate Callback { get; init; }
            public int Priority { get; init; }
            public int? Order { get; init; }
            public int SubOrder { get; init; }
            public int? EffectOrder { get; init; }
            public int? Speed { get; init; }
        }

        public List<EventListener> FindFieldEventHandlers(Field field, string callbackName,
            string? getKey = null, Pokemon? customHolder = null)
        {
            List<EventListener> handlers = new List<EventListener>();

            // Handle pseudo weather conditions
            foreach ((string id, EffectState pseudoWeatherState) in field.PseudoWeather)
            {
                // Get the pseudo weather condition from the dex
                Condition pseudoWeather = Dex.Conditions.GetById(new Id(id));

                // Get the callback for this condition
                Delegate? callback = GetCallback(field, pseudoWeather, callbackName);

                // Check if we should include this handler
                bool hasCallback = callback != null;
                bool hasSpecialKey = !string.IsNullOrEmpty(getKey) &&
                                   HasProperty(pseudoWeatherState, getKey);

                if (!hasCallback && !hasSpecialKey) continue;
                // Create the event listener without priority info first
                EventListenerWithoutPriority listenerWithoutPriority = new EventListenerWithoutPriority
                {
                    Effect = pseudoWeather,
                    Callback = callback,
                    State = pseudoWeatherState,
                    End = customHolder != null ? null : CreateRemovePseudoWeatherDelegate(field),
                    EffectHolder = (EventEffectHolder?)(customHolder ?? throw new ArgumentNullException(nameof(customHolder))) ?? field
                };

                // Resolve priority and create full event listener
                EventListener eventListener = ResolvePriority(listenerWithoutPriority, callbackName);
                handlers.Add(eventListener);
            }

            // Handle weather
            Condition weather = field.GetWeather();
            Delegate? weatherCallback = GetCallback(field, weather, callbackName);

            bool hasWeatherCallback = weatherCallback != null;
            bool hasWeatherSpecialKey = !string.IsNullOrEmpty(getKey) &&
                                       HasProperty(field.WeatherState, getKey);

            if (hasWeatherCallback || hasWeatherSpecialKey)
            {
                EventListenerWithoutPriority weatherListenerWithoutPriority = new EventListenerWithoutPriority
                {
                    Effect = weather,
                    Callback = weatherCallback,
                    State = field.WeatherState,
                    End = customHolder != null ? null : CreateClearWeatherDelegate(field),
                    EffectHolder = (EventEffectHolder?)(customHolder ?? throw new ArgumentNullException(nameof(customHolder))) ?? field
                };

                EventListener weatherEventListener = ResolvePriority(weatherListenerWithoutPriority, callbackName);
                handlers.Add(weatherEventListener);
            }

            // Handle terrain
            Condition terrain = field.GetTerrain();
            Delegate? terrainCallback = GetCallback(field, terrain, callbackName);

            bool hasTerrainCallback = terrainCallback != null;
            bool hasTerrainSpecialKey = !string.IsNullOrEmpty(getKey) &&
                                       HasProperty(field.TerrainState, getKey);

            if (!hasTerrainCallback && !hasTerrainSpecialKey) return handlers;
            EventListenerWithoutPriority terrainListenerWithoutPriority = new EventListenerWithoutPriority
            {
                Effect = terrain,
                Callback = terrainCallback,
                State = field.TerrainState,
                End = customHolder != null ? null : CreateClearTerrainDelegate(field),
                EffectHolder = (EventEffectHolder?)(customHolder ?? throw new ArgumentNullException(nameof(customHolder))) ?? field
            };

            EventListener terrainEventListener = ResolvePriority(terrainListenerWithoutPriority, callbackName);
            handlers.Add(terrainEventListener);

            return handlers;
        }

        // Helper method to create a delegate for removing pseudo weather
        private static Delegate? CreateRemovePseudoWeatherDelegate(Field field)
        {
            return new Action<string>(conditionId => field.RemovePseudoWeather(conditionId));
        }

        // Helper method to create a delegate for clearing weather
        private static Delegate? CreateClearWeatherDelegate(Field field)
        {
            return new Action(() => field.ClearWeather());
        }

        // Helper method to create a delegate for clearing terrain
        private static Delegate? CreateClearTerrainDelegate(Field field)
        {
            return new Action(() => field.ClearTerrain());
        }

        public List<EventListener> FindSideEventHandlers(Side side, string callbackName,
            string? getKey = null, Pokemon? customHolder = null)
        {
            List<EventListener> handlers = new List<EventListener>();

            foreach ((string id, EffectState sideConditionData) in side.SideConditions)
            {
                // Get the condition from the dex
                Condition sideCondition = Dex.Conditions.GetById(new Id(id));

                // Get the callback for this condition
                Delegate? callback = GetCallback(side, sideCondition, callbackName);

                // Check if we should include this handler
                bool hasCallback = callback != null;
                bool hasSpecialKey = !string.IsNullOrEmpty(getKey) &&
                                   HasProperty(sideConditionData, getKey);

                if (!hasCallback && !hasSpecialKey) continue;
                // Create the event listener without priority info first
                EventListenerWithoutPriority listenerWithoutPriority = new EventListenerWithoutPriority
                {
                    Effect = sideCondition,
                    Callback = callback,
                    State = sideConditionData,
                    End = customHolder != null ? null : CreateRemoveSideConditionDelegate(side),
                        
                    EffectHolder = (EventEffectHolder?)(customHolder ?? throw new ArgumentNullException(nameof(customHolder))) ?? side
                };

                // Resolve priority and create full event listener
                EventListener eventListener = ResolvePriority(listenerWithoutPriority, callbackName);
                handlers.Add(eventListener);
            }

            return handlers;
        }

        // Helper method to check if an EffectState has a specific property
        private static bool HasProperty(EffectState effectState, string propertyName)
        {
            // Check named properties first
            PropertyInfo? property = typeof(EffectState).GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (property != null)
            {
                return property.GetValue(effectState) != null;
            }

            // Fallback to ExtraData
            return effectState.ExtraData.ContainsKey(propertyName);
        }

        // Helper method to create a delegate for removing side conditions
        private static Delegate? CreateRemoveSideConditionDelegate(Side side)
        {
            return new Action<string>(conditionId => side.RemoveSideCondition(conditionId));
        }

        public void OnEvent(string eventId, Format target, params AnyObject[] rest)
        {
            throw new NotImplementedException();
        }

        public bool CheckMoveMakesContact(ActiveMove move, Pokemon attacker, Pokemon defender,
            bool announcePads = false)
        {
            throw new NotImplementedException();
        }

        public Pokemon? GetPokemon(string fullname)
        {
            throw new NotImplementedException();
        }

        public Pokemon? GetPokemon(Pokemon pokemon)
        {
            throw new NotImplementedException();
        }

        public List<Pokemon> GetAllPokemon()
        {
            throw new NotImplementedException();
        }

        public List<Pokemon> GetAllActive(bool? includeFainted = false)
        {
            throw new NotImplementedException();
        }

        public void MakeRequest(RequestState? type = null)
        {
            throw new NotImplementedException();
        }

        public void ClearRequest()
        {
            throw new NotImplementedException();
        }

        public List<IChoiceRequest> GetRequests(RequestState type)
        {
            throw new NotImplementedException();
        }

        public bool Tiebreak()
        {
            throw new NotImplementedException();
        }

        public bool ForceWin(SideId? side = null)
        {
            throw new NotImplementedException();
        }

        public bool Tie()
        {
            throw new NotImplementedException();
        }

        public bool Win(SideId side)
        {
            throw new NotImplementedException();
        }

        public bool Win(Side side)
        {
            throw new NotImplementedException();
        }

        public bool Win(string? side = null)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public Pokemon? GetRandomSwitchable(Side side)
        {
            throw new NotImplementedException();
        }

        private List<Pokemon> PossibleSwitches(Side side)
        {
            throw new NotImplementedException();
        }

        public bool SwapPosition(Pokemon pokemon, int newPosition, string? attributes = null)
        {
            throw new NotImplementedException();
        }

        public Pokemon? GetAtSlot(PokemonSlot? slot)
        {
            if (slot == null) return null;
            // TODO: Implement slot lookup logic
            throw new NotImplementedException();
        }

        public void Faint(Pokemon target, Pokemon? source = null, IEffect? effect = null)
        {
            throw new NotImplementedException();
        }

        public void EndTurn()
        {
            throw new NotImplementedException();
        }

        public bool? MaybeTriggerEndlessBattleClause(bool[] trappedBySide, string[] stalenessBySide)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Restart(Action<string, List<string>>? send = null)
        {
            // Implementation for restarting the battle
            throw new NotImplementedException();
        }

        public void CheckEvBalance()
        {
            // Implementation for checking EV balance
            throw new NotImplementedException();
        }

        public bool? Boost(SparseBoostsTable boost, Pokemon? target = null, Pokemon? source = null,
            IEffect? effect = null, bool isSecondary = false, bool isSelf = false)
        {
            throw new NotImplementedException();
        }

        public List<IntFalseUnion?> SpreadDamage(SpreadMoveDamage damage,
            List<SpreadDamageTarget>? targetArray = null, Pokemon? source = null,
            DamageEffect? effect = null, bool instafaint = false)
        {
            throw new NotImplementedException();
        }

        public IntFalseUnion? Damage(int damage, Pokemon? target = null, Pokemon? source = null,
            DamageEffect? effect = null, bool instafaint = false)
        {
            throw new NotImplementedException();
        }

        public int DirectDamage(int damage, Pokemon? target = null, Pokemon? source = null,
            IEffect? effect = null)
        {
            throw new NotImplementedException();
        }

        public IntFalseUnion Heal(int damage, Pokemon? target = null, Pokemon? source = null,
            HealEffect? effect = null)
        {
            throw new NotImplementedException();
        }

        public int Chain(int previousMod, int nextMod)
        {
            throw new NotImplementedException();
        }

        public int Chain(int previousMod, int[]  nextMod)
        {
            throw new NotImplementedException();
        }

        public int Chain(int[] previousMod, int nextMod)
        {
            throw new NotImplementedException();
        }

        public int Chain(int[] previousMod, int[] nextMod)
        {
            throw new NotImplementedException();
        }

        public void ChainModify(int numerator, int denominator = 1)
        {
            throw new NotImplementedException();
        }

        public void ChainModify(int[] numerator, int denominator = 1)
        {
            throw new NotImplementedException();
        }

        public int Modify(int value, int numerator, int denominator = 1)
        {
            throw new NotImplementedException();
        }

        public int Modify(int value, int[] numerator, int denominator = 1)
        {
            throw new NotImplementedException();
        }

        public StatsTable SpreadModify(StatsTable baseStats, PokemonSet set)
        {
            throw new NotImplementedException();
        }

        public StatsTable NatureModify(StatsTable baseStats, PokemonSet set)
        {
            throw new NotImplementedException();
        }

        public int FinalModify(int relayVar)
        {
            throw new NotImplementedException();
        }

        public MoveCategory GetCategory(string move)
        {
            throw new NotImplementedException();
        }

        public MoveCategory GetCategory(Move move)
        {
            throw new NotImplementedException();
               }

        public int Randomizer(int baseDamage)
        {
            throw new NotImplementedException();
        }

        public bool ValidTargetLoc(int targetLoc, Pokemon source, MoveTarget targetType)
        {
            throw new NotImplementedException();
        }

        public bool ValidTarget(Pokemon target, Pokemon source, MoveTarget targetType)
        {
            throw new NotImplementedException();
        }

        public Pokemon? GetTarget(Pokemon pokemon, string move, int targetLoc, Pokemon? originalTarget)
        {
            throw new NotImplementedException();
        }

        public Pokemon? GetTarget(Pokemon pokemon, Move move, int targetLoc, Pokemon? originalTarget)
        {
            throw new NotImplementedException();
        }

        public Pokemon? GetRandomTarget(Pokemon pokemon, Move move)
        {
            throw new NotImplementedException();
        }

        public Pokemon? GetRandomTarget(Pokemon pokemon, string move)
        {
            throw new NotImplementedException();
        }

        public void CheckFainted()
        {
            throw new NotImplementedException();
        }

        public bool? FaintMessages(bool lastFirst = false, bool forceCheck = false, bool checkWin = true)
        {
            if (Ended) return null;

            int length = FaintQueue.Count;
            if (length == 0)
            {
                if (forceCheck && CheckWin() == true) return true;
                return false;
            }

            if (lastFirst)
            {
                // Move last element to front (equivalent to unshift + pop)
                FaintQueueEntry lastElement = FaintQueue[^1];
                FaintQueue.RemoveAt(FaintQueue.Count - 1);
                FaintQueue.Insert(0, lastElement);
            }

            FaintQueueEntry? faintData = null;

            while (FaintQueue.Count > 0)
            {
                int faintQueueLeft = FaintQueue.Count;
                faintData = FaintQueue[0];
                FaintQueue.RemoveAt(0); // equivalent to shift()

                Pokemon pokemon = faintData.Target;

                if (faintData.Source != null && (pokemon.Fainted ||
                                                 RunEvent("BeforeFaint", pokemon, faintData.Source, faintData.Effect) is false or null)) continue;
                Add("faint", pokemon);

                if (pokemon.Side.PokemonLeft > 0) pokemon.Side.PokemonLeft--;
                if (pokemon.Side.TotalFainted < 100) pokemon.Side.TotalFainted++;

                RunEvent("Faint", pokemon, faintData.Source ?? throw new InvalidOperationException(), faintData.Effect);
                SingleEvent("End", pokemon.GetAbility(), pokemon.AbilityState, pokemon);
                SingleEvent("End", pokemon.GetItem(), pokemon.ItemState, pokemon);

                if (pokemon is { FormeRegression: true, Transformed: false })
                {
                    // before clearing volatiles
                    pokemon.BaseSpecies = Dex.Species.Get(pokemon.Set.Species ?? pokemon.Set.Name);
                    pokemon.BaseAbility = new Id(pokemon.Set.Ability ?? string.Empty);
                }

                pokemon.ClearVolatile(false);
                pokemon.Fainted = true;
                pokemon.Illusion = null;
                pokemon.IsActive = false;
                pokemon.IsStarted = false;
                pokemon.Terastallized = null; // equivalent to delete

                if (pokemon.FormeRegression)
                {
                    // after clearing volatiles
                    pokemon.Details = pokemon.GetUpdatedDetails();
                    Add("detailschange", pokemon, pokemon.Details, "[silent]");
                    pokemon.FormeRegression = false;
                }

                pokemon.Side.FaintedThisTurn = pokemon;
                if (FaintQueue.Count >= faintQueueLeft) checkWin = true;
            }

            switch (Gen)
            {
                case <= 1:
                {
                    // in gen 1, fainting skips the rest of the turn
                    // residuals don't exist in gen 1
                    Queue.Clear();

                    // Fainting clears accumulated Bide damage
                    foreach (Pokemon pokemon in GetAllActive())
                    {
                        if (!pokemon.Volatiles.TryGetValue("bide", out EffectState? bideState) ||
                            bideState.ExtraData?.ContainsKey("damage") != true) continue;
                        bideState.ExtraData["damage"] = 0;
                        Hint("Desync Clause Mod activated!");
                        Hint("In Gen 1, Bide's accumulated damage is reset to 0 when a Pokemon faints.");
                    }

                    break;
                }
                case <= 3 when GameType == GameType.Singles:
                {
                    // in gen 3 or earlier, fainting in singles skips to residuals
                    foreach (Pokemon pokemon in GetAllActive())
                    {
                        if (Gen <= 2)
                        {
                            // in gen 2, fainting skips moves only
                            Queue.CancelMove(pokemon);
                        }
                        else
                        {
                            // in gen 3, fainting skips all moves and switches
                            Queue.CancelAction(pokemon);
                        }
                    }

                    break;
                }
            }

            if (checkWin && CheckWin(faintData) == true) return true;

            if (faintData != null && length > 0)
            {
                RunEvent("AfterFaint", faintData.Target, faintData.Source, faintData.Effect, length);
            }

            return false;
        }

        public bool? CheckWin(FaintQueueEntry? faintData = null)
        {
            throw new NotImplementedException();
        }

        public void GetActionSpeed(AnyObject action)
        {
            throw new NotImplementedException();
        }

        public bool? RunAction(Action action)
        {
            throw new NotImplementedException();
        }

        public void TurnLoop()
        {
            throw new NotImplementedException();
        }

        public bool Choose(SideId side, string input)
        {
            throw new NotImplementedException();
        }

        public void MakeChoices(params string[] inputs)
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

        public void Hint(string hint, bool once = false, Side? side = null)
        {
            throw new NotImplementedException();
        }
        // addSplit(side: SideID, secret: Part[], shared?: Part[])

        public void AddSplit(SideId side, IReadOnlyList<string> secret, IReadOnlyList<string>? shared = null)
        {
            // Use shared if provided, otherwise use secret for both
            shared ??= secret;

            // Format the split message: |split|p{side}\n{secret}\n{shared}
            string sideStr = side switch
            {
                SideId.P1 => "1",
                SideId.P2 => "2", 
                SideId.P3 => "3",
                SideId.P4 => "4",
                _ => throw new ArgumentException($"Invalid side: {side}")
            };

            string secretMsg = string.Join("|", secret);
            string sharedMsg = string.Join("|", shared);
            
            string splitMessage = $"|split|p{sideStr}\n{secretMsg}\n{sharedMsg}";
            Log.Add(splitMessage);
        }

        public void Debug(string activity)
        {
            if (DebugMode)
            {
                Add("debug", activity);
            }
        }

        public void Add(params AddPart[] parts)
        {
            // Check if any parts are functions
            bool hasFunctionParts = parts.Any(part => part is FuncAddPart);

            if (!hasFunctionParts)
            {
                // Simple case: all parts are strings, join them and add to log
                string[] stringParts = parts.Select(part => ConvertPartToString(part)).ToArray();
                Log.Add($"|{string.Join("|", stringParts)}");
                return;
            }

            // Complex case: handle split messaging
            SideId? side = null;
            List<string> secret = [];
            List<string> shared = [];

            foreach (AddPart part in parts)
            {
                switch (part)
                {
                    case FuncAddPart functionPart:
                    {
                        SideSecretShared split = functionPart.Func();
                        if (side.HasValue && side.Value != split.Side)
                        {
                            throw new InvalidOperationException("Multiple sides passed to add");
                        }
                        side = split.Side;
                        secret.Add(split.Secret);
                        shared.Add(split.Shared);
                        break;
                    }
                    default:
                        string stringValue = ConvertPartToString(part);
                        secret.Add(stringValue);
                        shared.Add(stringValue);
                        break;
                }
            }

            if (side.HasValue)
            {
                AddSplit(side.Value, secret, shared);
            }
        }

        // Convenience overload to maintain compatibility with existing string calls
        public void Add(params string[] parts)
        {
            Log.Add($"|{string.Join("|", parts)}");
        }

        private static string ConvertPartToString(AddPart part)
        {
            return part switch
            {
                PartAddPart partPart => ConvertPartCoreToString(partPart.Part),
                GameTypeAddPart gameTypePart => gameTypePart.GameType.ToString(),
                FuncAddPart => string.Empty, // Functions should be handled separately
                _ => string.Empty
            };
        }

        private static string ConvertPartCoreToString(Part part)
        {
            return part switch
            {
                StringPart stringPart => stringPart.Value,
                IntPart intPart => intPart.Value.ToString(),
                BoolPart boolPart => boolPart.Value.ToString().ToLower(),
                PokemonPart pokemonPart => pokemonPart.Pokemon.ToString(),
                SidePart sidePart => sidePart.Side.ToString(),
                EffectPart effectPart => effectPart.Effect.Name,
                MovePart movePart => movePart.Move.Name,
                NullPart => "null",
                _ => string.Empty
            };
        }

        public EffectState InitEffectState(EffectState template, int? effectOrder = null)
        {
            int finalEffectOrder;

            if (effectOrder.HasValue)
            {
                // If an effect order is explicitly provided, use it.
                finalEffectOrder = effectOrder.Value;
            }
            else if (!template.Id.IsEmpty && template.ExtraData.TryGetValue("target", out object? targetObj))
            {
                // The effect order is incremented only if the target is not an inactive Pokémon.
                bool isInactivePokemon = targetObj is Pokemon { IsActive: false };
                if (!isInactivePokemon)
                {
                    // Use the battle's master counter for active effects.
                    finalEffectOrder = EffectOrder++;
                }
                else
                {
                    // Effects on inactive Pokémon get a default order of 0.
                    finalEffectOrder = 0;
                }
            }
            else
            {
                // Effects with no target get a default order of 0.
                finalEffectOrder = 0;
            }

            // Since EffectOrder is an init-only property, we must create a new instance.
            return new EffectState
            {
                Id = template.Id,
                Duration = template.Duration,
                ExtraData = template.ExtraData,
                EffectOrder = finalEffectOrder
            };
        }

        public void SetPlayer(SideId slot, PlayerOptions options)
        {
            throw new NotImplementedException();
        }

        public void AddMove(params AddMoveArg[] args)
        {
            throw new NotImplementedException();
        }

        public void AttrLastMove(params AddMoveArg[] args)
        {
            throw new NotImplementedException();
        }

        public void SendUpdates()
        {
            // Send incremental log updates if there are new entries
            if (SentLogPos >= Log.Count) return;

            // Send only new log entries since last update
            List<string> newLogEntries = Log.Skip(SentLogPos).ToList();
            Send("update", newLogEntries);

            // Emit requests from all sides if not already sent
            if (!SentRequests)
            {
                foreach (Side side in Sides)
                {
                    side.EmitRequest();
                }
                // Note: SentRequests is static and always returns true in this implementation
                // In a full implementation, this would be an instance property that gets set to true here
            }

            // Update the position marker
            SentLogPos = Log.Count;

            // Send end-of-battle log if battle has ended and not already sent
            if (!SentEnd && Ended)
            {
                // Create comprehensive battle log object
                var battleLog = new
                {
                    winner = Winner,
                    seed = PrngSeed,
                    turns = Turn,
                    p1 = Sides[0].Name,
                    p2 = Sides[1].Name,
                    p3 = Sides.Length > 2 ? Sides[2]?.Name : null,
                    p4 = Sides.Length > 3 ? Sides[3]?.Name : null,
                    p1team = Sides[0].Team,
                    p2team = Sides[1].Team,
                    p3team = Sides.Length > 2 ? Sides[2]?.Team : null,
                    p4team = Sides.Length > 3 ? Sides[3]?.Team : null,
                    score = new List<int> { Sides[0].PokemonLeft, Sides[1].PokemonLeft },
                    inputLog = InputLog
                };

                // Add additional scores for 3+ player games
                if (Sides.Length > 2 && Sides[2] != null)
                {
                    battleLog.score.Add(Sides[2].PokemonLeft);
                }
                if (Sides.Length > 3 && Sides[3] != null)
                {
                    battleLog.score.Add(Sides[3].PokemonLeft);
                }

                // Serialize and send the final battle log
                string jsonLog = JsonSerializer.Serialize(battleLog, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

                Send("end", jsonLog);
                SentEnd = true;
            }
        }

        public void ShowOpenTeamSheets()
        {
            throw new NotImplementedException();
        }
    }
}
