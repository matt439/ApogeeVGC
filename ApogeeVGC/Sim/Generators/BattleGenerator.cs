using ApogeeVGC.Data;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.FieldClasses;

namespace ApogeeVGC.Sim.Generators;

public static class BattleGenerator
{
    public static Core.Battle GenerateTestBattle(Library library, string trainerName1,
        string trainerName2, bool printDebug = false, int? seed = null)
    {
        return new Core.Battle
        {
            Library = library,
            Field = new Field(),
            Side1 = SideGenerator.GenerateTestSide(library, trainerName1, PlayerId.Player1, printDebug),
            Side2 = SideGenerator.GenerateTestSide(library, trainerName2, PlayerId.Player2, printDebug),
            PrintDebug = printDebug,
            BattleSeed = seed,
        };
    }
}

