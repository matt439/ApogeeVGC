using ApogeeVGC.Data;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Generators;

public static class SideGenerator
{
    public static Side GenerateTestSide(Library library, string trainerName, PlayerId playerId, SideId sideId,
        bool printDebug = false)
    {
        return new Side
        {
            PlayerId = playerId,
            Team = TeamGenerator.GenerateTestTeam(library, trainerName, printDebug),
            PrintDebug = printDebug,
            SideId = sideId,
        };
    }
}