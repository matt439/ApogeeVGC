using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Turns;

public sealed record GameplayTurn : Turn //(Side Side1Start, Side Side2Start, Field FieldStart, int TurnCounter)
    //: Turn(Side1Start, Side2Start, FieldStart, TurnCounter)
{
    // public override TimeSpan TurnTimeLimit => TimeSpan.FromSeconds(45);

    public BattleChoice? Side1Slot1Choice { get; set; }
    public BattleChoice? Side1Slot2Choice { get; set; }
    public BattleChoice? Side2Slot1Choice { get; set; }
    public BattleChoice? Side2Slot2Choice { get; set; }

    public void SetChoice(SideId side, SlotId slot, BattleChoice choice)
    {
        switch (side)
        {
            case SideId.Side1:
                switch (slot)
                {
                    case SlotId.Slot1:
                        Side1Slot1Choice = choice;
                        break;
                    case SlotId.Slot2:
                        Side1Slot2Choice = choice;
                        break;
                    case SlotId.Slot3:
                    case SlotId.Slot4:
                    case SlotId.Slot5:
                    case SlotId.Slot6:
                    default:
                        throw new InvalidOperationException();
                }
                break;
            case SideId.Side2:
                switch (slot)
                {
                    case SlotId.Slot1:
                        Side2Slot1Choice = choice;
                        break;
                    case SlotId.Slot2:
                        Side2Slot2Choice = choice;
                        break;
                    case SlotId.Slot3:
                    case SlotId.Slot4:
                    case SlotId.Slot5:
                    case SlotId.Slot6:
                    default:
                        throw new InvalidOperationException();
                }
                break;
            default:
                throw new InvalidOperationException();
        }
    }

    public BattleChoice? GetChoice(SideId side, SlotId slot)
    {
        return side switch
        {
            SideId.Side1 => slot switch
            {
                SlotId.Slot1 => Side1Slot1Choice,
                SlotId.Slot2 => Side1Slot2Choice,
                SlotId.Slot3 or SlotId.Slot4 or SlotId.Slot5 or SlotId.Slot6 =>
                    throw new InvalidOperationException($"Bench slot {slot} does not make active choices"),
                _ => throw new InvalidOperationException($"Invalid slot: {slot}"),
            },
            SideId.Side2 => slot switch
            {
                SlotId.Slot1 => Side2Slot1Choice,
                SlotId.Slot2 => Side2Slot2Choice,
                SlotId.Slot3 or SlotId.Slot4 or SlotId.Slot5 or SlotId.Slot6 =>
                    throw new InvalidOperationException($"Bench slot {slot} does not make active choices"),
                _ => throw new InvalidOperationException($"Invalid slot: {slot}"),
            },
            _ => throw new InvalidOperationException($"Invalid side: {side}"),
        };
    }
}