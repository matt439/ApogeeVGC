using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Sim.Utils.Extensions;

public static class SideIdTools
{
    public static SideId GetOppositeSide(this SideId sideId)
    {
        return sideId switch
        {
            SideId.P1 => SideId.P2,
            SideId.P2 => SideId.P1,
            _ => throw new ArgumentOutOfRangeException(nameof(sideId), "Invalid SideId."),
        };
    }

    public static string GetSideIdName(this SideId sideId)
    {
        return sideId switch
        {
            SideId.P1 => "p1",
            SideId.P2 => "p2",
            _ => throw new ArgumentOutOfRangeException(nameof(sideId), "Invalid SideId."),
        };
    }
}