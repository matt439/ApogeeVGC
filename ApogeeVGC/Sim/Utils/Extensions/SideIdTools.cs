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
            SideId.P1 => "P1",
            SideId.P2 => "P2",
            _ => throw new ArgumentOutOfRangeException(nameof(sideId), "Invalid SideId."),
        };
    }
}