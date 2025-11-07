using ApogeeVGC.Sim.BattleClasses;

namespace ApogeeVGC.Sim.Core;

public interface IBattleController
{
    // void SubmitChoice(SideId sideId, Choice choice);
    BattlePerspective GetCurrentPerspective(SideId sideId);
}