using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FieldClasses;

namespace ApogeeVGC.Sim.Generators;

public static class FieldGenerator
{
    public static Field GenerateTestField(IBattle battle)
    {
        return new Field(battle);
    }
}