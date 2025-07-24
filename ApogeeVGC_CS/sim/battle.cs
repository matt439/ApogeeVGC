using ApogeeVGC_CS.sim;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Numerics;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC_CS.sim
{
    public enum ChannelID
    {
        Channel0 = 0,
        Channel1 = 1,
        Channel2 = 2,
        Channel3 = 3,
        Channel4 = 4
    }

    public class ChannelMessages : Dictionary<ChannelID, List<string>> { }

    public static class BattleUtils
    {
        public static Dictionary<int, List<string>> ExtractChannelMessages(string message, IEnumerable<int> channelIds)
        {
            var splitRegex = new Regex(@"\|split\|p([1234])\n(.*)\n(.*)|.+", RegexOptions.Multiline);
            var channelIdSet = new HashSet<int>(channelIds);
            var channelMessages = new Dictionary<int, List<string>>();
            foreach (var id in channelIdSet)
            {
                channelMessages[id] = new List<string>();
            }
            channelMessages[-1] = new List<string>();

            foreach (Match match in splitRegex.Matches(message))
            {
                var playerMatch = match.Groups[1].Value;
                var secretMessage = match.Groups[2].Value;
                var sharedMessage = match.Groups[3].Value;
                int player = string.IsNullOrEmpty(playerMatch) ? 0 : int.Parse(playerMatch);

                foreach (var channelId in channelIdSet)
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
        public Format? Format { get; set; }
        public Id FormatId { get; set; } = new();
        public Action<string, object>? Send { get; set; }
        public PRNG? Prng { get; set; }
        public PRNGSeed? Seed { get; set; }
        public object? Rated { get; set; } // bool or string
        public PlayerOptions? P1 { get; set; }
        public PlayerOptions? P2 { get; set; }
        public PlayerOptions? P3 { get; set; }
        public PlayerOptions? P4 { get; set; }
        public bool? Debug { get; set; }
        public bool? ForceRandomChance { get; set; }
        public bool? Deserialized { get; set; }
        public bool? StrictChoices { get; set; }
    }

    public class EventListenerWithoutPriority
    {
        public required IEffect Effect { get; set; }
        public Pokemon? Target { get; set; }
        public int? Index { get; set; }
        public Delegate? Callback { get; set; }
        public object? State { get; set; }
        public Delegate? End { get; set; }
        public object[]? EndCallArgs { get; set; }
        public object EffectHolder { get; set; } = new();
    }

    public class EventListener : EventListenerWithoutPriority
    {
        public int? Order { get; set; }
        public int Priority { get; set; }
        public int SubOrder { get; set; }
        public int? EffectOrder { get; set; }
        public int? Speed { get; set; }
    }

    // Part type (union)
    // string | number | boolean | Pokemon | Side | Effect | Move | null | undefined;
    public class Part
    {
        public object? Value { get; set; }
    }

    public enum RequestState
    {
        TeamPreview,
        Move,
        Switch,
        None
    }
    public class FaintQueueEntry
    {
        public Pokemon Target { get; set; }
        public Pokemon? Source { get; set; }
        public IEffect? Effect { get; set; }
    }

    public class Battle
    {
        // Readonly properties
        public Id Id { get; } = new();
        public bool DebugMode { get; }
        public bool? ForceRandomChance { get; }
        public bool Deserialized { get; }
        public bool StrictChoices { get; }
        public Format Format { get; }
        public EffectState FormatData { get; }
        public GameType GameType { get; }
        public int ActivePerHalf { get; }
        public Field Field { get; }
        public Side[] Sides { get; }
        public PRNGSeed PrngSeed { get; }

        // Mutable properties
        public ModdedDex Dex { get; set; }
        public int Gen { get; set; }
        public RuleTable RuleTable { get; set; }
        public PRNG Prng { get; set; }
        public object Rated { get; set; } // bool or string
        public bool ReportExactHP { get; set; }
        public bool ReportPercentages { get; set; }
        public bool SupportCancel { get; set; }

        public BattleActions Actions { get; set; }
        public BattleQueue Queue { get; set; }
        public List<FaintQueueEntry> FaintQueue { get; } = new();

        public List<string> Log { get; } = new();
        public List<string> InputLog { get; } = new();
        public List<string> MessageLog { get; } = new();
        public int SentLogPos { get; set; }
        public bool SentEnd { get; set; }
        public bool SentRequests { get; set; } = true;

        public RequestState RequestState { get; set; }
        public int Turn { get; set; }
        public bool MidTurn { get; set; }
        public bool Started { get; set; }
        public bool Ended { get; set; }
        public string? Winner { get; set; }

        public IEffect Effect { get; set; }
        public EffectState EffectState { get; set; }

        public IAnyObject Event { get; set; }
        public IAnyObject? Events { get; set; }
        public int EventDepth { get; set; }

        public ActiveMove? ActiveMove { get; set; }
        public Pokemon? ActivePokemon { get; set; }
        public Pokemon? ActiveTarget { get; set; }

        public ActiveMove? LastMove { get; set; }
        public Id? LastSuccessfulMoveThisTurn { get; set; }
        public int LastMoveLine { get; set; }
        public int LastDamage { get; set; }
        public int EffectOrder { get; set; }
        public bool QuickClawRoll { get; set; }
        public List<int> SpeedOrder { get; } = new();

        public object? TeamGenerator { get; set; }

        public HashSet<string> Hints { get; } = new();

        // Constants
        public string NOT_FAIL { get; } = "";
        public int HIT_SUBSTITUTE { get; } = 0;
        public bool FAIL { get; } = false;
        public object? SILENT_FAIL { get; } = null;

        public Action<string, object> Send { get; }

        // Methods
        public Func<double, int?, double> Trunc { get; set; }
        public Func<object, int?, int?, int> ClampIntRange { get; set; }
        public Func<object, Id> ToID { get; set; }

        public Battle(BattleOptions options)
        {
            // TODO: Implement constructor logic
        }

        public IAnyObject ToJSON()
        {
            throw new NotImplementedException();
            // TODO - implement JSON serialization
        }

        public static Battle FromJSON(object serialized)
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

        public void ResetRNG(PRNGSeed? seed = null)
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

        public static int ComparePriority(IAnyObject a, IAnyObject b)
        {
            // TODO: Implement priority comparison
            throw new NotImplementedException();
        }

        public static int CompareRedirectOrder(IAnyObject a, IAnyObject b)
        {
            // TODO: Implement redirect order comparison
            throw new NotImplementedException();
        }

        public static int CompareLeftToRightOrder(IAnyObject a, IAnyObject b)
        {
            // TODO: Implement left-to-right order comparison
            throw new NotImplementedException();
        }

        public void SpeedSort<T>(List<T> list, Func<T, T, int>? comparator = null) where T : IAnyObject
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

        public IAnyObject SingleEvent(string eventId, IEffect effect, object? state = null,
            object? target = null, object? source = null, IEffect? sourceEffect = null,
            object? relayVar = null, Delegate? customCallback = null)
        {
            throw new NotImplementedException();
            // TODO: Implement single event handling logic
        }

        public IAnyObject RunEvent(string eventId, object? target = null, object? source = null,
            IEffect? sourceEffect = null, object? relayVar = null, bool onEffect = false,
            bool fastExit = false)
        {
            throw new NotImplementedException();
            // TODO: Implement event running logic
        }

        public IAnyObject PriorityEvent(string eventId, object target, object? source = null,
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

        public bool ForceWin(SideID? side = null)
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
            // Implementation for getting possible switches
            return new List<Pokemon>();
        }

        public bool SwapPosition(Pokemon pokemon, int NewPosition, string? attributes = null)
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

        public void CheckEVBalance()
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

        public void GetActionSpeed(IAnyObject action)
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

        public bool Choose(SideID side, string input)
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

        public void UndoChoice(SideID sideId)
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

        public void AddSplit(SideID side, IReadOnlyList<Part> secret, IReadOnlyList<Part>? shared = null)
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

        public void SetPlayer(SideID slot, PlayerOptions options)
        {
            throw new NotImplementedException();
        }

        public void SendUpdates()
        {
            throw new NotImplementedException();
        }

        public Side GetSide(SideID sideid)
        {
            throw new NotImplementedException();
        }

        public int GetOverFlowedTurnCount()
        {
            throw new NotImplementedException();
        }

        public EffectState InitEffectState(EffectState obj, int? effectOrder)
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
