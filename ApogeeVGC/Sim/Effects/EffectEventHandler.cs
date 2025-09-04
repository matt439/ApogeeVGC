using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Sim.Effects;

public static class EffectEventHandler
{
    public static void RunAllOnResidualEvents(Battle battle)
    {
        // create a set of all effects in the battle

        // check for OnResidualOrder on each effect, sort by order
        // if there are multiple with the same order, check for OnResidualSubOrder, sort by suborder

        // run OnResidual for each effect in order
    }
}