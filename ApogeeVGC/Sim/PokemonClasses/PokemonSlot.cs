using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Sim.PokemonClasses;

public class PokemonSlot(SideId sideId, int positionOffset)
{
    public SideId SideId { get; init; } = sideId;
    public PositionLetter PositionLetter { get; init; } = FromIntPosition(positionOffset);

    //private static Side FromInt(int sideId) => sideId switch
    //{
    //    0 => Side.P1,
    //    1 => Side.P2,
    //    _ => throw new ArgumentOutOfRangeException(nameof(sideId), "Side must be 0 or 1"),
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

    /// <summary>
    /// Gets the relative location from this slot to a target slot.
    /// Positive numbers indicate opponent's slots, negative for ally slots.
    /// </summary>
    public int GetRelativeLocation(PokemonSlot target)
    {
        // Convert position letter to 0-based index
        int targetPos = target.PositionLetter switch
        {
            PositionLetter.A => 1,
            PositionLetter.B => 2,
            PositionLetter.C => 3,
            PositionLetter.D => 4,
            PositionLetter.E => 5,
            PositionLetter.F => 6,
            _ => throw new ArgumentOutOfRangeException()
        };

        // Same side = negative, different side = positive
        if (SideId == target.SideId)
        {
            return -targetPos;
        }
        else
        {
            return targetPos;
        }
    }

    public override string ToString()
    {
        return $"{SideId}{PositionLetter.ToString().ToLowerInvariant()}";
    }
}