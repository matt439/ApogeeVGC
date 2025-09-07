using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.GameObjects;

namespace ApogeeVGC.Sim.Choices;

public record DoubleSlotChoice
{
    public required SlotChoice Slot1Choice { get; init; }
    public required SlotChoice Slot2Choice { get; init; }

    public Trainer Trainer => Slot1Choice.Trainer;
    public SideId SideId => Slot1Choice.SideId;

    public DoubleSlotChoice(SlotChoice slot1Choice, SlotChoice slot2Choice)
    {
        // 1. Assign properties first to ensure they're available for validation
        Slot1Choice = slot1Choice ?? throw new ArgumentNullException(nameof(slot1Choice));
        Slot2Choice = slot2Choice ?? throw new ArgumentNullException(nameof(slot2Choice));

        // 2. Validate slot positions
        SlotId slot1Id = Slot1Choice.SlotId;
        SlotId slot2Id = Slot2Choice.SlotId;

        if (slot1Id != SlotId.Slot1)
            throw new ArgumentException($"Slot1Choice must be for Slot1, but was for {slot1Id}.");
        if (slot2Id != SlotId.Slot2)
            throw new ArgumentException($"Slot2Choice must be for Slot2, but was for {slot2Id}.");

        // 3. Validate basic requirements
        if (Slot1Choice.Trainer != Slot2Choice.Trainer)
        {
            throw new ArgumentException("Both choices must be for the same trainer.");
        }
        if (Slot1Choice.SideId != Slot2Choice.SideId)
        {
            throw new ArgumentException("Both choices must be for the same side.");
        }

        // 4. Validate choice-specific conflicts using pattern matching
        ValidateChoiceConflicts(Slot1Choice, Slot2Choice);
    }

    private static void ValidateChoiceConflicts(SlotChoice slot1Choice, SlotChoice slot2Choice)
    {
        switch (slot1Choice, slot2Choice)
        {
            case (SlotChoice.MoveChoice move1, SlotChoice.MoveChoice move2):
                ValidateDoubleMoveConflicts(move1, move2);
                break;

            case (SlotChoice.SwitchChoice switch1, SlotChoice.SwitchChoice switch2):
                ValidateDoubleSwitchConflicts(switch1, switch2);
                break;

            case (SlotChoice.MoveChoice moveChoice, SlotChoice.SwitchChoice switchChoice):
                ValidateMixedChoiceConflicts(moveChoice, switchChoice);
                break;

            case (SlotChoice.SwitchChoice switchChoice, SlotChoice.MoveChoice moveChoice):
                ValidateMixedChoiceConflicts(moveChoice, switchChoice);
                break;
        }
    }

    private static void ValidateDoubleMoveConflicts(SlotChoice.MoveChoice move1, SlotChoice.MoveChoice move2)
    {
        if (move1.Attacker == move2.Attacker)
        {
            throw new ArgumentException("A Pokémon cannot make two move choices.");
        }
    }

    private static void ValidateDoubleSwitchConflicts(SlotChoice.SwitchChoice switch1, SlotChoice.SwitchChoice switch2)
    {
        if (switch1.SwitchOutPokemon == switch2.SwitchOutPokemon)
        {
            throw new ArgumentException("A Pokémon cannot be switched out twice.");
        }
        if (switch1.SwitchInPokemon == switch2.SwitchInPokemon)
        {
            throw new ArgumentException("A Pokémon cannot be switched in twice.");
        }
        if (switch1.SwitchOutPokemon == switch2.SwitchInPokemon ||
            switch2.SwitchOutPokemon == switch1.SwitchInPokemon)
        {
            throw new ArgumentException("A Pokémon cannot be switched out and in at the same time.");
        }
    }

    private static void ValidateMixedChoiceConflicts(SlotChoice.MoveChoice move, SlotChoice.SwitchChoice switchChoice)
    {
        // Critical validation: Cannot use a move with a Pokémon that's being switched out
        if (move.Attacker == switchChoice.SwitchOutPokemon)
        {
            throw new ArgumentException("A Pokémon cannot use a move and be switched out at the same time.");
        }

        // Edge case validation: Cannot switch in a Pokémon that's already using a move
        // (This would be unusual but technically possible if someone creates invalid state)
        if (move.Attacker == switchChoice.SwitchInPokemon)
        {
            throw new ArgumentException("A Pokémon cannot use a move and be switched in at the same time.");
        }
    }
}