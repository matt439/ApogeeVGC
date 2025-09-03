using ApogeeVGC.Sim.GameObjects;

namespace ApogeeVGC.Sim.Utils.Extensions;

public static class GenderIdExtensions
{
    public static string GenderIdString(this GenderId id)
    {
        return id switch
        {
            GenderId.M => "M",
            GenderId.F => "F",
            GenderId.N => string.Empty,
            GenderId.Empty => string.Empty,
            _ => throw new ArgumentOutOfRangeException(nameof(id), id, null),
        };
    }
}