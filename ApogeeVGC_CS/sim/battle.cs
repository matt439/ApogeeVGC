using System.Text.Json;
using System.Text.RegularExpressions;
using ApogeeVGC_CS.lib;

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
        public object? Rated
        {
            get;
            init // bool or string
            {
                if (value is bool or string)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("Rated must be a boolean or string.");
                }
            }
        }
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
        public required object EffectHolder
        {
            get;
            init // can be Pokemon, Side, Field, or Battle
            {
                if (value is Pokemon or Side or Field or Battle)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("EffectHolder must be a Pokemon, Side, Field, or Battle.");
                }
            }
        } 
    }

    public class EventListener : EventListenerWithoutPriority
    {
        public int? Order { get; init; }
        public int Priority { get; init; }
        public int SubOrder { get; init; }
        public int? EffectOrder { get; init; }
        public int? Speed { get; init; }
    }


    public enum PartType
    {
        String,
        Number,
        Boolean,
        Pokemon,
        Side,
        Effect,
        Move,
        Null,
        Undefined
    }

    public class Part
    {
        public object? Value
        {
            get;
            init // string | number | boolean | Pokemon | Side | Effect | Move | null | undefined
            {
                if (value is string or int or bool or Pokemon or Side or IEffect or Move or null)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("Value must be a string, number," +
                                                "boolean, Pokemon, Side, Effect, Move, null, or undefined.");
                }
            }
        }
        public PartType Type
        {
            get
            {
                return Value switch
                {
                    string => PartType.String,
                    int => PartType.Number,
                    bool => PartType.Boolean,
                    Pokemon => PartType.Pokemon,
                    Side => PartType.Side,
                    IEffect => PartType.Effect,
                    Move => PartType.Move,
                    null => PartType.Null,
                    _ => PartType.Undefined
                };
            }
        }
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
        public required Prng Prng { get; init; }
        public required object Rated
        {
            get;
            init // bool or string
            {
                if (value is bool or string)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("Rated must be a boolean or string.");
                }
            }
        } 
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

        public ActiveMove? ActiveMove { get; init; }
        public Pokemon? ActivePokemon { get; init; }
        public Pokemon? ActiveTarget { get; init; }

        public ActiveMove? LastMove { get; init; }
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
            // TODO: Implement string representation
            return $"Battle: {Format}";
        }

        public double Random(int? m = null, int? n = null)
        {
            // TODO: Implement random number generation
            throw new NotImplementedException();
        }

        public bool RandomChance(int numerator, int denominator)
        {
            // TODO: Implement random chance calculation
            throw new NotImplementedException();
        }

        public T Sample<T>(IReadOnlyList<T> items)
        {
            // TODO: Implement random sampling
            throw new NotImplementedException();
        }

        public void ResetRng(PrngSeed? seed = null)
        {
            // TODO: Implement RNG reset
            throw new NotImplementedException();
        }

        public bool SuppressingAbility(Pokemon? target = null)
        {
            // TODO: Implement ability suppression check
            throw new NotImplementedException();
        }

        public void SetActiveMove(ActiveMove? move = null, Pokemon? pokemon = null, Pokemon? target = null)
        {
            // TODO: Implement active move setting
            throw new NotImplementedException();
        }

        public void ClearActiveMove(bool failed = false)
        {
            // TODO: Implement active move clearing
            throw new NotImplementedException();
        }

        public void UpdateSpeed()
        {
            // TODO: Implement speed update for all active Pokemon
            throw new NotImplementedException();
        }

        public static int ComparePriority(object a, object b)
        {
            // TODO: Implement priority comparison
            throw new NotImplementedException();
        }

        public static int CompareRedirectOrder(object a, object b)
        {
            // TODO: Implement redirect order comparison
            throw new NotImplementedException();
        }

        public static int CompareLeftToRightOrder(object a, object b)
        {
            // TODO: Implement left-to-right order comparison
            throw new NotImplementedException();
        }

        public void SpeedSort<T>(List<T> list, Func<T, T, int>? comparator = null)
        {
            // TODO: Implement speed sorting with tie resolution
            throw new NotImplementedException();
        }

        public void EachEvent(string eventId, IEffect? effect = null, object? relayVar = null)
        {
            throw new NotImplementedException();
            // TODO: Implement event handling logic
        }
        // fieldEvent(eventid: string, targets?: Pokemon[])
        public void FieldEvent(string eventId, IReadOnlyList<Pokemon>? targets = null)
        {
            throw new NotImplementedException();
            // TODO: Implement field event handling logic
        }

        public object SingleEvent(string eventId, IEffect effect, object? state = null,
            object? target = null, object? source = null, IEffect? sourceEffect = null,
            object? relayVar = null, Delegate? customCallback = null)
        {
            throw new NotImplementedException();
            // TODO: Implement single event handling logic
        }

        public object RunEvent(string eventId, object? target = null, object? source = null,
            IEffect? sourceEffect = null, object? relayVar = null, bool onEffect = false,
            bool fastExit = false)
        {
            throw new NotImplementedException();
            // TODO: Implement event running logic
        }

        public object PriorityEvent(string eventId, object target, object? source = null,
            IEffect? effect = null, object? relayVar = null, bool onEffect = false)
        {
            throw new NotImplementedException();
        }

        public EventListener ResolvePriority(EventListenerWithoutPriority listener, string callbackName)
        {
            throw new NotImplementedException();
        }

        public Delegate? GetCallback(object target, IEffect effect, string callbackName)
        {
            throw new NotImplementedException();
        }

        public List<EventListener> FindEventHandlers(object target, string eventName, object? source = null)
        {
            throw new NotImplementedException();
        }

        public List<EventListener> FindPokemonEventHandlers(Pokemon pokemon, string callbackName, string? getKey = null)
        {
            throw new NotImplementedException();
        }

        public List<EventListener> FindBattleEventHandlers(string callbackName, string? getKey = null, Pokemon? customHolder = null)
        {
            throw new NotImplementedException();
        }

        public List<EventListener> FindFieldEventHandlers(Field field, string callbackName, string? getKey = null, Pokemon? customHolder = null)
        {
            throw new NotImplementedException();
        }

        public List<EventListener> FindSideEventHandlers(Side side, string callbackName, string? getKey = null, Pokemon? customHolder = null)
        {
            throw new NotImplementedException();
        }

        public void OnEvent(string eventId, IEffect target, params object[] rest)
        {
            throw new NotImplementedException();
        }

        public bool CheckMoveMakesContact(ActiveMove move, Pokemon attacker, Pokemon defender, bool announcePads = false)
        {
            throw new NotImplementedException();
        }

        public Pokemon? GetPokemon(string fullname)
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

        public void MakeRequest(RequestState? type)
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

        public bool Win(object? side = null)
        {
            throw new NotImplementedException();
        }

        public bool Lose(object? side = null)
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

        public void Restart(Action<string, object>? send = null)
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

        public int?[] SpreadDamage(SpreadMoveDamage damage, IReadOnlyList<Pokemon?>? targetArray = null,
            Pokemon? source = null, IEffect? effect = null, bool instafaint = false)
        {
            throw new NotImplementedException();
        }

        public int? Damage(int damage, Pokemon? target = null, Pokemon? source = null,
            IEffect? effect = null, bool instafaint = false)
        {
            throw new NotImplementedException();
        }

        public int DirectDamage(int damage, Pokemon? target = null, Pokemon? source = null,
            IEffect? effect = null)
        {
            throw new NotImplementedException();
        }

        public int? Heal(int damage, Pokemon? target = null, Pokemon? source = null,
            IEffect? effect = null)
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

        public bool ValidTargetLoc(int targetLoc, Pokemon source, string targetType)
        {
            throw new NotImplementedException();
        }

        public bool ValidTarget(Pokemon target, Pokemon source, string targetType)
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

        public void Add(params object[] parts)
        {
            throw new NotImplementedException();
        }

        public void AddMove(params object[] args)
        {
            throw new NotImplementedException();
        }

        public void AttrLastMove(params object[] args)
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
