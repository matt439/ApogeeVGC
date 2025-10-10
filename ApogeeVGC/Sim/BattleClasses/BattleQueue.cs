namespace ApogeeVGC.Sim.BattleClasses;

public class BattleQueue(IBattle battle)
{
    public IBattle Battle { get; init; } = battle;
    public List<Action> List { get; init; } = [];

    public Action? WillAct()
    {
        throw new NotImplementedException();
    }
}