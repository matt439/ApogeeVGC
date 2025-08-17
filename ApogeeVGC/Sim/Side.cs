using ApogeeVGC.Data;
using ApogeeVGC.Player;

namespace ApogeeVGC.Sim;

public class Side
{
    public required Team Team { get; init; }
    public required PlayerId PlayerId { get; init; }
}

public static class SideGenerator
{
    public static Side GenerateTestSide(Library library, string trainerName, PlayerId playerId)
    {
        return new Side
        {
            PlayerId = playerId,
            Team = TeamGenerator.GenerateTestTeam(library, trainerName)
        };
    }
}