using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Sim.Utils.Extensions;

public static class SideIdTools
{
    public static SideId GetOppositeSide(this SideId sideId)
    {
        return sideId switch
        {
            SideId.Side1 => SideId.Side2,
            SideId.Side2 => SideId.Side1,
            _ => throw new ArgumentOutOfRangeException(nameof(sideId), "Invalid SideId.")
        };
    }
}