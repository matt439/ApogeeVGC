using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Sim.PokemonClasses;

public class PokemonSlot(SideId sideId, int positionOffset)
{
    public SideId SideId { get; init; } = sideId;
    public PositionLetter PositionLetter { get; init; } = FromIntPosition(positionOffset);

    //private static SideId FromInt(int sideId) => sideId switch
    //{
    //    0 => SideId.P1,
    //    1 => SideId.P2,
    //    _ => throw new ArgumentOutOfRangeException(nameof(sideId), "SideId must be 0 or 1"),
    //};

    private static PositionLetter FromIntPosition(int positionOffset) => positionOffset switch
    {
        0 => PositionLetter.A,
        1 => PositionLetter.B,
        2 => PositionLetter.C,
        3 => PositionLetter.D,
        4 => PositionLetter.E,
        5 => PositionLetter.F,
        _ => throw new ArgumentOutOfRangeException(nameof(positionOffset), "PositionOffset must be between 0 and 5"),
    };
}