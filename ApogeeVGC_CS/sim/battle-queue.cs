namespace ApogeeVGC_CS.sim
{
    public enum MoveActionChoice
    {
        Move,
        BeforeTurnMove,
        PriorityChargeMove,
    }

    //public enum MoveActionOrder
    //{
    //    S3,
    //    S5,
    //    S200,
    //    S201,
    //    S199,
    //    S106,
    //}

    public class MoveAction : IAction
    {
        public required MoveActionChoice Choice { get; init; }
        public required int Order
        { 
            get; 
            init
            {
                if (value is not (3 or 5 or 200 or 201 or 199 or 106))
                {
                    throw new ArgumentException("Order must be one of the predefined values: 3, 5, 200, 201, 199, or 106.");
                }
                field = value;
            }
        }
        public required int Priority { get; init; }
        public required double FractionalPriority { get; init; }
        public required int Speed { get; init; }
        public required Pokemon Pokemon { get; init; }
        public required int TargetLoc { get; init; }
        public required Pokemon OriginalTarget { get; init; }
        public required Id MoveId { get; init; }
        public required Move Move { get; init; }
        public required MoveActionMega Mega { get; init; }
        public string? ZMove { get; init; }
        public string? MaxMove { get; init; }
        public IEffect? SourceEffect { get; init; }
    }

    public enum SwitchActionChoice
    {
        Switch,
        InstaSwitch,
        RevivalBlessing,
    }

    public enum SwitchActionOrder
    {
        S3,
        S6,
        S103,
    }

    public class SwitchAction : IAction
    {
        public required SwitchActionChoice Choice { get; init; }
        public required SwitchActionOrder Order { get; init; }
        public required int Priority { get; init; }
        public required int Speed { get; init; }
        public required Pokemon Pokemon { get; init; }
        public required Pokemon Target { get; init; }
        public IEffect? SourceEffect { get; init; }
    }

    public class TeamAction : IAction
    {
        public static string Choice => "team";
        public required int Priority { get; init; }
        public static int Speed => 1;
        public required Pokemon Pokemon { get; init; }
        public required int Index { get; init; }
    }

    public enum FieldActionChoice
    {
        Start,
        Residual,
        Pass,
        BeforeTurn,
    }

    public class FieldAction : IAction
    {
        public required FieldActionChoice Choice { get; init; }
        public required int Priority { get; init; }
        public static int Speed => 1;
        public static Pokemon? Pokemon => null;
    }

    public enum PokemonActionChoice
    {
        MegaEvo,
        MegaEvoX,
        MegaEvoY,
        Shift,
        RunSwitch,
        Event,
        RunDynamax,
        Terastallize,
    }

    public class PokemonAction : IAction
    {
        public required PokemonActionChoice Choice { get; init; }
        public required int Priority { get; init; }
        public required int Speed { get; init; }
        public required Pokemon Pokemon { get; init; }
        public Pokemon? Dragger { get; init; } // For "runSwitch"
        public string? Event { get; init; } // For "event"
    }

    /// <summary>
    /// MoveAction | SwitchAction | TeamAction | FieldAction | PokemonAction
    /// </summary>
    public interface IAction;

    public class ActionChoice
    {
        public required string Choice { get; init; }
        public Dictionary<string, object>? ExtraProperties { get; init; }
    }

    public class BattleQueue
    {
        public Battle Battle { get; }
        public List<IAction> List { get; } = [];

        public BattleQueue(Battle battle)
        {
            Battle = battle;

            // This would use ObjectExtensions.Assign()
            //const queueScripts = battle.format.queue || battle.dex.data.Scripts.queue;
            //if (queueScripts) Object.assign(this, queueScripts);
        }

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

        /**
         * Takes an ActionChoice, and fills it out into a full Action object.
         *
         * Returns an array of Actions because some ActionChoices (like mega moves)
         * resolve to two Actions (mega evolution + use move)
         */
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