using System;
using System.Collections.Generic;

namespace ApogeeVGC_CS.sim
{
    public class MoveAction
    {
        public string Choice { get; set; } = string.Empty; // "move", "beforeTurnMove", "priorityChargeMove"
        public int Order { get; set; }
        public int Priority { get; set; }
        public double FractionalPriority { get; set; }
        public int Speed { get; set; }
        public Pokemon Pokemon { get; set; } = new();
        public int TargetLoc { get; set; }
        public Pokemon OriginalTarget { get; set; } = new();
        public Id MoveId { get; set; } = new();
        public Move Move { get; set; } = new();
        public object? Mega { get; set; } // bool or "done"
        public string? ZMove { get; set; }
        public string? MaxMove { get; set; }
        public Effect? SourceEffect { get; set; }
    }

    public class SwitchAction
    {
        public string Choice { get; set; } = string.Empty; // "switch", "instaswitch", "revivalblessing"
        public int Order { get; set; }
        public int Priority { get; set; }
        public int Speed { get; set; }
        public Pokemon Pokemon { get; set; } = new();
        public Pokemon Target { get; set; } = new();
        public Effect? SourceEffect { get; set; }
    }

    public class TeamAction
    {
        public string Choice { get; set; } = "team";
        public int Priority { get; set; }
        public int Speed { get; set; } = 1;
        public Pokemon Pokemon { get; set; } = new();
        public int Index { get; set; }
    }

    // Field action (not done by a Pokémon)
    public class FieldAction
    {
        public string Choice { get; set; } = string.Empty; // "start", "residual", "pass", "beforeTurn"
        public int Priority { get; set; }
        public int Speed { get; set; } = 1;
        public object? Pokemon { get; set; } = null;
    }

    // Generic Pokémon action
    public class PokemonAction
    {
        public string Choice { get; set; } = string.Empty; // "megaEvo", "megaEvoX", etc.
        public int Priority { get; set; }
        public int Speed { get; set; }
        public Pokemon Pokemon { get; set; } = new();
        public Pokemon? Dragger { get; set; } // For "runSwitch"
        public string? Event { get; set; } // For "event"
    }

    // ActionChoice: flexible action structure
    public class ActionChoice
    {
        public string Choice { get; set; } = string.Empty;
        public Dictionary<string, object>? ExtraProperties { get; set; }
    }

    public class BattleQueue
    {
        // TODO
    }
}