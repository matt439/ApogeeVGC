using ApogeeVGC_CS.data;
using ApogeeVGC_CS.sim;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
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
        public Action<string, object>? Send { get; init; }
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
        public int? Order { get; init; }
        public int Priority { get; init; }
        public int SubOrder { get; init; }
        public int? EffectOrder { get; init; }
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
        public required Id Id { get; init; }
        public required bool DebugMode { get; init; }
        public bool? ForceRandomChance { get; init; }
        public required bool Deserialized { get; init; }
        public required bool StrictChoices { get; init; }
        public required Format Format { get; init; }
        public required EffectState FormatData { get; init; }
        public required GameType GameType { get; init; }
        public required int ActivePerHalf
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
        public required Field Field { get; init; }
        public required Side[] Sides
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
        public required PrngSeed PrngSeed { get; init; }
        public required ModdedDex Dex { get; init; }
        public required ModdedDex BaseDex { get; init; } // Added to emulate how the JavaScript version works
        public required int Gen { get; init; }
        public required RuleTable RuleTable { get; init; }
        public required Prng Prng { get; set; }
        public required BoolStringUnion Rated { get; init; }
        public required bool ReportExactHp { get; init; }
        public required bool ReportPercentages { get; init; }
        public required bool SupportCancel { get; init; }

        public required BattleActions Actions { get; init; }
        public BattleQueue Queue { get; init; }
        public required List<FaintQueueEntry> FaintQueue { get; init; }

        public required List<string> Log { get; init; }
        public required List<string> InputLog { get; init; }
        public required List<string> MessageLog { get; init; }
        public required int SentLogPos { get; init; }
        public required bool SentEnd { get; init; }
        public static bool SentRequests => true;

        public required RequestState RequestState { get; init; }
        public required int Turn { get; init; }
        public required bool MidTurn { get; init; }
        public required bool Started { get; init; }
        public required bool Ended { get; init; }
        public string? Winner { get; init; }

        public required IEffect Effect { get; init; }
        public required EffectState EffectState { get; init; }

        public required object Event { get; init; }
        public object? Events { get; init; }
        public required int EventDepth { get; init; }

        public ActiveMove? ActiveMove { get; set; }
        public Pokemon? ActivePokemon { get; set; }
        public Pokemon? ActiveTarget { get; set; }

        public ActiveMove? LastMove { get; set; }
        public Id? LastSuccessfulMoveThisTurn { get; init; }
        public required int LastMoveLine { get; init; }
        public required int LastDamage { get; init; }
        public required int EffectOrder { get; init; }
        public required bool QuickClawRoll { get; init; }
        public required List<int> SpeedOrder { get; init; }

        public object? TeamGenerator { get; init; }

        public required HashSet<string> Hints { get; init; }

        // Constants
        public static string NotFail => "";
        public static int HitSubstitute => 0;
        public static bool Fail => false;
        public static object? SilentFail => null;

        public Action<string, object> Send { get; init; }

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

            Dex = BaseDex.ForFormat(Format);
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

            for (var i = 0; i < ActivePerHalf * 2; i++)
            {
                SpeedOrder.Add(i);
            }

            TeamGenerator = null;

            Hints = [];

            Send = options.Send ?? ((channel, message) => { });

            // Create input options for logging
            var inputOptions = new Dictionary<string, object>
            {
                ["formatid"] = options.FormatId,
                ["seed"] = PrngSeed
            };

            if (Rated is true or string)
            {
                inputOptions["rated"] = Rated;
            }

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
            foreach (string rule in RuleTable.Keys)
            {
                if ("+-*!".Contains(rule[0])) continue;

                var subFormat = Dex.Formats.Get(rule);
                if (!subFormat.Exists) continue;
                // Check if format has event handlers (excluding specific ones handled elsewhere)
                var excludedHandlers = new HashSet<string>
                {
                    "onBegin", "onTeamPreview", "onBattleStart", "onValidateRule",
                    "onValidateTeam", "onChangeSet", "onValidateSet"
                };

                bool hasEventHandler = subFormat.GetType()
                    .GetProperties()
                    .Any(prop => prop.Name.StartsWith("On") &&
                                 !excludedHandlers.Contains(prop.Name));

                if (hasEventHandler)
                {
                    Field.AddPseudoWeather(rule);
                }
            }

            // Set up players
            var sideIds = new[] { SideId.P1, SideId.P2, SideId.P3, SideId.P4 };
            foreach (var sideId in sideIds)
            {
                var playerOptions = sideId switch
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
            return ActivePokemon != null
                && ActivePokemon.IsActive
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


        /**
	     * The default sort order for actions, but also event listeners.
	     *
	     * 1. Order, low to high (default last)
	     * 2. Priority, high to low (default 0)
	     * 3. Speed, high to low (default 0)
	     * 4. SubOrder, low to high (default 0)
	     * 5. EffectOrder, low to high (default 0)
	     *
	     */
        public static int ComparePriority(IAnyObject a, IAnyObject b)
        {
            // Order comparison (lower values first, null = last)
            var orderResult = (a.Order ?? int.MaxValue).CompareTo(b.Order ?? int.MaxValue);
            if (orderResult != 0) return orderResult;

            // Priority comparison (higher values first)
            var priorityResult = (b.Priority ?? 0).CompareTo(a.Priority ?? 0);
            if (priorityResult != 0) return priorityResult;

            // Speed comparison (higher values first)
            var speedResult = (b.Speed ?? 0).CompareTo(a.Speed ?? 0);
            if (speedResult != 0) return speedResult;

            // SubOrder comparison (lower values first)
            var subOrderResult = (a.SubOrder ?? 0).CompareTo(b.SubOrder ?? 0);
            if (subOrderResult != 0) return subOrderResult;

            // EffectOrder comparison (lower values first)
            return (a.EffectOrder ?? 0).CompareTo(b.EffectOrder ?? 0);
        }

        public static int CompareRedirectOrder(IAnyObject a, IAnyObject b)
        {
            // Priority comparison (higher values first)
            var priorityResult = (b.Priority ?? 0).CompareTo(a.Priority ?? 0);
            if (priorityResult != 0) return priorityResult;

            // Speed comparison (higher values first)
            var speedResult = (b.Speed ?? 0).CompareTo(a.Speed ?? 0);
            if (speedResult != 0) return speedResult;

            // AbilityState EffectOrder comparison (inverted, only if both have ability states)
            if (a.AbilityState != null && b.AbilityState != null)
            {
                return a.AbilityState.EffectOrder.CompareTo(b.AbilityState.EffectOrder);
            }

            return 0;
        }

        public static int CompareLeftToRightOrder(IAnyObject a, IAnyObject b)
        {
            // Order comparison (lower values first, null = last)
            var orderResult = (a.Order ?? int.MaxValue).CompareTo(b.Order ?? int.MaxValue);
            if (orderResult != 0) return orderResult;

            // Priority comparison (higher values first)
            var priorityResult = (b.Priority ?? 0).CompareTo(a.Priority ?? 0);
            if (priorityResult != 0) return priorityResult;

            // Index comparison (lower values first)
            return (a.Index ?? 0).CompareTo(b.Index ?? 0);
        }

        /// <summary>Sort a list, resolving speed ties the way the games do.</summary>
        public void SpeedSort<T>(List<T> list, Func<T, T, int>? comparator = null) where T : IAnyObject
        {
            comparator ??= (a, b) => ComparePriority(a, b);

            if (list.Count < 2) return;

            int sorted = 0;

            // This is a Selection Sort - not the fastest sort in general, but
            // actually faster than QuickSort for small arrays like the ones
            // SpeedSort is used for.
            // More importantly, it makes it easiest to resolve speed ties properly.
            while (sorted + 1 < list.Count)
            {
                var nextIndexes = new List<int> { sorted };

                // Grab list of next indexes
                for (int i = sorted + 1; i < list.Count; i++)
                {
                    int delta = comparator(list[nextIndexes[0]], list[i]);
                    if (delta < 0) continue;
                    if (delta > 0) nextIndexes = [i];
                    if (delta == 0) nextIndexes.Add(i);
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
            var actives = GetAllActive();
            if (effect == null && Effect != null)
                effect = Effect;

            // Sort by speed descending (fastest first)
            SpeedSort(actives, (a, b) => b.Speed.CompareTo(a.Speed));

            foreach (var pokemon in actives)
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
            throw new NotImplementedException();
            // TODO: Implement single event handling logic
        }

        public object RunEvent(
            string eventId,
            RunEventTarget? target = null,
            RunEventSource? source = null,
            IEffect? sourceEffect = null,
            object? relayVar = null,
            bool onEffect = false,
            bool fastExit = false)
        {
            throw new NotImplementedException();
        }

        public object PriorityEvent(string eventId, PriorityEventTarget target,
            Pokemon? source = null, IEffect? effect = null, object? relayVar = null,
            bool onEffect = false)
        {
            throw new NotImplementedException();
        }

        public EventListener ResolvePriority(EventListenerWithoutPriority listener,
            string callbackName)
        {
            throw new NotImplementedException();
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
            var type = effect.GetType();
            var property = type.GetProperty(callbackName, BindingFlags.Public | BindingFlags.Instance);

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
            return effect.EffectType == EffectType.Ability || effect.EffectType == EffectType.Item;
        }

        private static bool IsInnateAbilityOrItem(IEffect effect)
        {
            // Innate abilities/items - Status effects with ability/item prefix
            if (effect.EffectType != EffectType.Status) return false;

            var idParts = effect.Id.ToString().Split(':');
            return idParts.Length > 0 && (idParts[0] == "ability" || idParts[0] == "item");
        }

        public List<EventListener> FindEventHandlers(FindEventHandlersTarget target,
            string eventName, Pokemon? source = null)
        {
            throw new NotImplementedException();
        }

        public List<EventListener> FindPokemonEventHandlers(Pokemon pokemon,
            string callbackName, string? getKey = null)
        {
            throw new NotImplementedException();
        }

        //public List<EventListener> FindPokemonEventHandlers(Pokemon pokemon,
        //    string callbackName, string? getKey = null)
        //{
        //    var handlers = new List<EventListener>();

        //    // Check status condition (burn, paralysis, sleep, etc.)
        //    var status = pokemon.GetStatus();
        //    var callback = GetCallback(pokemon, status, callbackName);
        //    if (callback != null || (!string.IsNullOrEmpty(getKey) && HasProperty(pokemon.StatusState, getKey)))
        //    {
        //        handlers.Add(ResolvePriority(new EventListenerWithoutPriority
        //        {
        //            Effect = status,
        //            Callback = callback,
        //            State = pokemon.StatusState,
        //            End = CreateClearStatusDelegate(pokemon),
        //            EffectHolder = pokemon
        //        }, callbackName));
        //    }

        //    // Check volatile conditions (temporary effects)
        //    foreach (var kvp in pokemon.Volatiles)
        //    {
        //        string id = kvp.Key;
        //        EffectState volatileState = kvp.Value;
        //        var volatile = Dex.Conditions.GetById(new Id(id));

        //        callback = GetCallback(pokemon, volatile, callbackName);
        //        if (callback != null || (!string.IsNullOrEmpty(getKey) && HasProperty(volatileState, getKey)))
        //        {
        //            handlers.Add(ResolvePriority(new EventListenerWithoutPriority
        //            {
        //                Effect = volatile,
        //                Callback = callback,
        //                State = volatileState,
        //                End = CreateRemoveVolatileDelegate(pokemon),
        //                EffectHolder = pokemon
        //            }, callbackName));
        //    }

        //    return handlers;
        //}

        //// Helper methods for creating cleanup delegates
        //private static Delegate CreateClearStatusDelegate(Pokemon pokemon)
        //{
        //    return new Action(() => pokemon.ClearStatus());
        //}

        //private static Delegate CreateRemoveVolatileDelegate(Pokemon pokemon)
        //{
        //    return new Action<string>(volatileId => pokemon.RemoveVolatile(volatileId));
        //}

        //private static Delegate CreateClearAbilityDelegate(Pokemon pokemon)
        //{
        //    return new Action(() => pokemon.ClearAbility());
        //}

        //private static Delegate CreateClearItemDelegate(Pokemon pokemon)
        //{
        //    return new Action(() => pokemon.ClearItem());
        //}

        //private static Delegate CreateRemoveSlotConditionDelegate(Side side, Pokemon pokemon, Id conditionId)
        //{
        //    return new Action(() => side.RemoveSlotCondition(pokemon, conditionId.ToString()));
        //}

        public List<EventListener> FindBattleEventHandlers(string callbackName,
            string? getKey = null, Pokemon? customHolder = null)
        {
            throw new NotImplementedException();
        }

        public List<EventListener> FindFieldEventHandlers(Field field, string callbackName,
            string? getKey = null, Pokemon? customHolder = null)
        {
            throw new NotImplementedException();
        }

        public List<EventListener> FindSideEventHandlers(Side side, string callbackName,
            string? getKey = null, Pokemon? customHolder = null)
        {
            var handlers = new List<EventListener>();

            foreach (var kvp in side.SideConditions)
            {
                string id = kvp.Key;
                EffectState sideConditionData = kvp.Value;

                // Get the condition from the dex
                var sideCondition = Dex.Conditions.GetById(new Id(id));

                // Get the callback for this condition
                var callback = GetCallback(side, sideCondition, callbackName);

                // Check if we should include this handler
                bool hasCallback = callback != null;
                bool hasSpecialKey = !string.IsNullOrEmpty(getKey) &&
                                   HasProperty(sideConditionData, getKey);

                if (hasCallback || hasSpecialKey)
                {
                    // Create the event listener without priority info first
                    var listenerWithoutPriority = new EventListenerWithoutPriority
                    {
                        Effect = sideCondition,
                        Callback = callback,
                        State = sideConditionData,
                        End = customHolder != null ? null : CreateRemoveSideConditionDelegate(side),
                        EffectHolder = customHolder ?? (object)side
                    };

                    // Resolve priority and create full event listener
                    var eventListener = ResolvePriority(listenerWithoutPriority, callbackName);
                    handlers.Add(eventListener);
                }
            }

            return handlers;
        }

        // Helper method to check if an EffectState has a specific property
        private static bool HasProperty(EffectState effectState, string propertyName)
        {
            // Check named properties first
            var property = typeof(EffectState).GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (property != null)
            {
                return property.GetValue(effectState) != null;
            }

            // Fallback to ExtraData
            return effectState.ExtraData?.ContainsKey(propertyName) ?? false;
        }

        // Helper method to create a delegate for removing side conditions
        private static Delegate? CreateRemoveSideConditionDelegate(Side side)
        {
            return new Action<string>(conditionId => side.RemoveSideCondition(conditionId));
        }

        public void OnEvent(string eventId, Format target, params object[] rest)
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

        public List<ChoiceRequest> GetRequests(RequestState type)
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

        public bool MaybeTriggerEndlessBattleClause(bool[] trappedBySide, string[] stalenessBySide)
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
            SpreadDamageEffect? effect = null, bool instafaint = false)
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
            throw new NotImplementedException();
        }

        public bool? CheckWin(FaintQueueEntry? faintData = null)
        {
            throw new NotImplementedException();
        }

        public void GetActionSpeed(object action)
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

        public void AddSplit(SideId side, IReadOnlyList<Part> secret, IReadOnlyList<Part>? shared = null)
        {
            throw new NotImplementedException();
        }

        public void Add(params AddPart[] parts)
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

        public void RetargetLastMove(Pokemon newTarget)
        {
            throw new NotImplementedException();
        }

        public void Debug(string activity)
        {
            throw new NotImplementedException();
        }

        public string GetDebugLog()
        {
            throw new NotImplementedException();
        }

        public void DebugError(string activity)
        {
            throw new NotImplementedException();
        }

        public List<PokemonSet> GetTeam(PlayerOptions options)
        {
            throw new NotImplementedException();
        }

        public void ShowOpenTeamSheets()
        {
            throw new NotImplementedException();
        }

        public void SetPlayer(SideId slot, PlayerOptions options)
        {
            throw new NotImplementedException();
        }

        public void SendUpdates()
        {
            throw new NotImplementedException();
        }

        public Side GetSide(SideId sideid)
        {
            throw new NotImplementedException();
        }

        public int GetOverFlowedTurnCount()
        {
            throw new NotImplementedException();
        }

        public EffectState InitEffectState(EffectState obj, int? effectOrder = null)
        {
            throw new NotImplementedException();
        }

        public void ClearEffectState(EffectState state)
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }
    }
}
