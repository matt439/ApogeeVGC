using ApogeeVGC.Data;
using ApogeeVGC.Player;

namespace ApogeeVGC.Sim;

public static class SideGenerator
{
    public static Side GenerateTestSide(Library library, string trainerName, PlayerId playerId,
        bool printDebug = false)
    {
        return new Side
        {
            PlayerId = playerId,
            Team = TeamGenerator.GenerateTestTeam(library, trainerName, printDebug),
            PrintDebug = printDebug,
        };
    }
}