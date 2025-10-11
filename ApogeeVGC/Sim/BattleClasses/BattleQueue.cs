using ApogeeVGC.Sim.Actions;

namespace ApogeeVGC.Sim.BattleClasses;

public class BattleQueue(IBattle battle)
{
    public IBattle Battle { get; init; } = battle;
    public List<IAction> List { get; init; } = [];

    public IAction? WillAct()
    {
        foreach (IAction action in List)
        {
            if (action.Choice is ActionId.Move or ActionId.Switch or ActionId.InstaSwitch or ActionId.Shift)
            {
                return action;
            }
        }
        return null;
    }
}