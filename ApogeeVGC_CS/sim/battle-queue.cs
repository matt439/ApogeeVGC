namespace ApogeeVGC_CS.sim
{
    public class MoveAction : IAction
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
        public IEffect? SourceEffect { get; set; }
    }

    public class SwitchAction : IAction
    {
        public string Choice { get; set; } = string.Empty; // "switch", "instaswitch", "revivalblessing"
        public int Order { get; set; }
        public int Priority { get; set; }
        public int Speed { get; set; }
        public Pokemon Pokemon { get; set; } = new();
        public Pokemon Target { get; set; } = new();
        public IEffect? SourceEffect { get; set; }
    }

    public class TeamAction : IAction
    {
        public string Choice { get; set; } = "team";
        public int Priority { get; set; }
        public int Speed { get; set; } = 1;
        public Pokemon Pokemon { get; set; } = new();
        public int Index { get; set; }
    }

    // Field action (not done by a Pokémon)
    public class FieldAction : IAction
    {
        public string Choice { get; set; } = string.Empty; // "start", "residual", "pass", "beforeTurn"
        public int Priority { get; set; }
        public int Speed { get; set; } = 1;
        public object? Pokemon { get; set; } = null;
    }

    // Generic Pokémon action
    public class PokemonAction : IAction
    {
        public string Choice { get; set; } = string.Empty; // "megaEvo", "megaEvoX", etc.
        public int Priority { get; set; }
        public int Speed { get; set; }
        public Pokemon Pokemon { get; set; } = new();
        public Pokemon? Dragger { get; set; } // For "runSwitch"
        public string? Event { get; set; } // For "event"
    }

    // export type Action = MoveAction | SwitchAction | TeamAction | FieldAction | PokemonAction;
    public interface IAction
    {

    }

    // ActionChoice: flexible action structure
    public class ActionChoice
    {
        public string Choice { get; set; } = string.Empty;
        public Dictionary<string, object>? ExtraProperties { get; set; }
    }

    public class BattleQueue
    {
        public Battle Battle { get; }
        public List<IAction> List { get; } = new();

        public IAction? Shift()
        {
            throw new NotImplementedException("Shift method is not implemented yet.");
        }

        public IAction? Peek(bool? end)
        {
            throw new NotImplementedException("Shift method is not implemented yet.");
        }

        public int Push(IAction action)
        {
            throw new NotImplementedException("Push method is not implemented yet.");
        }

        public int Unshift(IAction action)
        {
            throw new NotImplementedException("Push method is not implemented yet.");
        }

        public IEnumerator<IAction> Entries()
        {
            throw new NotImplementedException("Entries method is not implemented yet.");
        }

        public IAction[] ResolveAction(ActionChoice action, bool midTurn = false)
        {
            throw new NotImplementedException("ResolveAction method is not implemented yet.");
        }

        public void PrioritizeAction(IAction action, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("PrioritizeAction method is not implemented yet.");
        }

        public void ChangeAction(Pokemon pokemon, ActionChoice action)
        {
            throw new NotImplementedException("ChangeAction method is not implemented yet.");
        }

        public void AddChoice(List<ActionChoice> choices)
        {
            throw new NotImplementedException("AddChoice method is not implemented yet.");
        }

        public IAction? WillAct()
        {
            throw new NotImplementedException("WillAct method is not implemented yet.");
        }

        public MoveAction? WillMove(Pokemon pokemon)
        {
            throw new NotImplementedException("WillMove method is not implemented yet.");
        }

        public bool CancelAction(Pokemon pokemon)
        {
            throw new NotImplementedException("CancelAction method is not implemented yet.");
        }

        public bool CancelMove(Pokemon pokemon)
        {
            throw new NotImplementedException("CancelMove method is not implemented yet.");
        }

        public IAction? WillSwitch(Pokemon pokemon)
        {
            throw new NotImplementedException("WillSwitch method is not implemented yet.");
        }

        public bool InsertChoice(List<ActionChoice> choices, bool midTurn = false)
        {
            throw new NotImplementedException("InsertChoice method is not implemented yet.");
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public string Debug(IAction? action = null)
        {
            throw new NotImplementedException("Debug method is not implemented yet.");
        }

        public BattleQueue Sort()
        {
            throw new NotImplementedException("Sort method is not implemented yet.");
        }

    }
}