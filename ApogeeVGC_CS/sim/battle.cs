using System;
using System.Collections.Generic;

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
        public Effect Effect { get; set; } = new();
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

    public class Battle
    {
        // TODO
    }
}
