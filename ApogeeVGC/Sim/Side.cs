using ApogeeVGC.Data;

namespace ApogeeVGC.Sim;

public class Side
{
    public required Team Team { get; init; }
}

public static class SideGenerator
{
    public static Side GenerateTestSide(Library library, string trainerName)
    {
        return new Side
        {
            Team = TeamGenerator.GenerateTestTeam(library, trainerName)
        };
    }
}