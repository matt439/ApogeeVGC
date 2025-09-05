using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Moves;

namespace ApogeeVGC.Sim.Utils.Extensions;

public static class ChoiceTools
{
    /// <summary>
    /// Gets a human-readable name for a choice.
    /// </summary>
    public static string GetChoiceName(this Choice choice)
    {
        // Handle doubles choices
        if (choice.IsDoublesChoice())
        {
            return choice.GetChoiceDescription();
        }

        // Handle team preview choices
        if (choice.IsTeamPreviewChoice())
        {
            return choice.GetTeamPreviewDescription();
        }

        // Handle single choices
        return choice switch
        {
            // Move 1
            Choice.Move1NormalFoe1 => "Move 1 → Foe 1",
            Choice.Move1NormalFoe2 => "Move 1 → Foe 2", 
            Choice.Move1NormalAlly => "Move 1 → Ally",
            Choice.Move1AllAdjacentFoes => "Move 1 → All Adjacent Foes",
            Choice.Move1Field => "Move 1 → Field",
            Choice.Move1AllySide => "Move 1 → Ally Side",
            Choice.Move1Self => "Move 1 → Self",

            Choice.Move1NormalFoe1WithTera => "Move 1 → Foe 1 (with Tera)",
            Choice.Move1NormalFoe2WithTera => "Move 1 → Foe 2 (with Tera)",
            Choice.Move1NormalAllyWithTera => "Move 1 → Ally (with Tera)",
            Choice.Move1AllAdjacentFoesWithTera => "Move 1 → All Adjacent Foes (with Tera)",
            Choice.Move1FieldWithTera => "Move 1 → Field (with Tera)",
            Choice.Move1AllySideWithTera => "Move 1 → Ally Side (with Tera)",
            Choice.Move1SelfWithTera => "Move 1 → Self (with Tera)",

            // Move 2
            Choice.Move2NormalFoe1 => "Move 2 → Foe 1",
            Choice.Move2NormalFoe2 => "Move 2 → Foe 2",
            Choice.Move2NormalAlly => "Move 2 → Ally",
            Choice.Move2AllAdjacentFoes => "Move 2 → All Adjacent Foes",
            Choice.Move2Field => "Move 2 → Field",
            Choice.Move2AllySide => "Move 2 → Ally Side",
            Choice.Move2Self => "Move 2 → Self",

            Choice.Move2NormalFoe1WithTera => "Move 2 → Foe 1 (with Tera)",
            Choice.Move2NormalFoe2WithTera => "Move 2 → Foe 2 (with Tera)",
            Choice.Move2NormalAllyWithTera => "Move 2 → Ally (with Tera)",
            Choice.Move2AllAdjacentFoesWithTera => "Move 2 → All Adjacent Foes (with Tera)",
            Choice.Move2FieldWithTera => "Move 2 → Field (with Tera)",
            Choice.Move2AllySideWithTera => "Move 2 → Ally Side (with Tera)",
            Choice.Move2SelfWithTera => "Move 2 → Self (with Tera)",

            // Move 3
            Choice.Move3NormalFoe1 => "Move 3 → Foe 1",
            Choice.Move3NormalFoe2 => "Move 3 → Foe 2",
            Choice.Move3NormalAlly => "Move 3 → Ally",
            Choice.Move3AllAdjacentFoes => "Move 3 → All Adjacent Foes",
            Choice.Move3Field => "Move 3 → Field",
            Choice.Move3AllySide => "Move 3 → Ally Side",
            Choice.Move3Self => "Move 3 → Self",

            Choice.Move3NormalFoe1WithTera => "Move 3 → Foe 1 (with Tera)",
            Choice.Move3NormalFoe2WithTera => "Move 3 → Foe 2 (with Tera)",
            Choice.Move3NormalAllyWithTera => "Move 3 → Ally (with Tera)",
            Choice.Move3AllAdjacentFoesWithTera => "Move 3 → All Adjacent Foes (with Tera)",
            Choice.Move3FieldWithTera => "Move 3 → Field (with Tera)",
            Choice.Move3AllySideWithTera => "Move 3 → Ally Side (with Tera)",
            Choice.Move3SelfWithTera => "Move 3 → Self (with Tera)",

            // Move 4
            Choice.Move4NormalFoe1 => "Move 4 → Foe 1",
            Choice.Move4NormalFoe2 => "Move 4 → Foe 2",
            Choice.Move4NormalAlly => "Move 4 → Ally",
            Choice.Move4AllAdjacentFoes => "Move 4 → All Adjacent Foes",
            Choice.Move4Field => "Move 4 → Field",
            Choice.Move4AllySide => "Move 4 → Ally Side",
            Choice.Move4Self => "Move 4 → Self",

            Choice.Move4NormalFoe1WithTera => "Move 4 → Foe 1 (with Tera)",
            Choice.Move4NormalFoe2WithTera => "Move 4 → Foe 2 (with Tera)",
            Choice.Move4NormalAllyWithTera => "Move 4 → Ally (with Tera)",
            Choice.Move4AllAdjacentFoesWithTera => "Move 4 → All Adjacent Foes (with Tera)",
            Choice.Move4FieldWithTera => "Move 4 → Field (with Tera)",
            Choice.Move4AllySideWithTera => "Move 4 → Ally Side (with Tera)",
            Choice.Move4SelfWithTera => "Move 4 → Self (with Tera)",

            // Other choices
            Choice.Switch1 => "Switch to Pokemon 1",
            Choice.Switch2 => "Switch to Pokemon 2",
            Choice.Switch3 => "Switch to Pokemon 3",
            Choice.Switch4 => "Switch to Pokemon 4",
            Choice.Struggle => "Struggle",
            Choice.Quit => "Quit",
            Choice.None => "None",
            Choice.Invalid => "Invalid",
            _ => choice.ToString(),
        };
    }

    /// <summary>
    /// Checks if a choice is any kind of move choice (including with targeting).
    /// </summary>
    public static bool IsMoveChoice(this Choice choice)
    {
        string choiceStr = choice.ToString();
        return choiceStr.StartsWith("Move") && !choiceStr.Contains("WithTera");
    }

    /// <summary>
    /// Checks if a choice is any kind of move with Tera choice.
    /// </summary>
    public static bool IsMoveWithTeraChoice(this Choice choice)
    {
        return choice.ToString().Contains("WithTera");
    }

    /// <summary>
    /// Gets the move number (1-4) from a move choice.
    /// </summary>
    public static int GetMoveNumber(this Choice choice)
    {
        string choiceStr = choice.ToString();
        if (choiceStr.StartsWith("Move1")) return 1;
        if (choiceStr.StartsWith("Move2")) return 2;
        if (choiceStr.StartsWith("Move3")) return 3;
        if (choiceStr.StartsWith("Move4")) return 4;
        throw new ArgumentException($"Not a valid move choice: {choice}");
    }

    /// <summary>
    /// Gets the targeting information from a move choice.
    /// </summary>
    public static MoveSlotTarget GetMoveTarget(this Choice choice)
    {
        string choiceStr = choice.ToString();
        
        if (!IsMoveChoice(choice) && !IsMoveWithTeraChoice(choice))
            throw new ArgumentException($"Not a move choice: {choice}");

        if (choiceStr.Contains("NormalFoe1")) return MoveSlotTarget.NormalFoe1;
        if (choiceStr.Contains("NormalFoe2")) return MoveSlotTarget.NormalFoe2;
        if (choiceStr.Contains("NormalAlly")) return MoveSlotTarget.NormalAlly;
        if (choiceStr.Contains("AllAdjacentFoes")) return MoveSlotTarget.AllAdjacentFoes;
        if (choiceStr.Contains("Field")) return MoveSlotTarget.Field;
        if (choiceStr.Contains("AllySide")) return MoveSlotTarget.AllySide;
        if (choiceStr.Contains("Self")) return MoveSlotTarget.Self;

        throw new ArgumentException($"Could not determine target for choice: {choice}");
    }

    /// <summary>
    /// Checks if a choice uses Terastallization.
    /// </summary>
    public static bool UsesTera(this Choice choice)
    {
        return IsMoveWithTeraChoice(choice);
    }

    /// <summary>
    /// Converts a move with Tera choice to its non-Tera equivalent.
    /// </summary>
    public static Choice ConvertMoveWithTeraToMove(this Choice choice)
    {
        if (!IsMoveWithTeraChoice(choice))
            throw new ArgumentException($"Choice is not a Tera move: {choice}");

        string choiceStr = choice.ToString().Replace("WithTera", "");
        if (Enum.TryParse(choiceStr, out Choice result))
            return result;

        throw new ArgumentException($"Could not convert Tera choice to regular move: {choice}");
    }

    /// <summary>
    /// Converts a regular move choice to its Tera equivalent.
    /// </summary>
    public static Choice ConvertMoveToMoveWithTera(this Choice choice)
    {
        if (!IsMoveChoice(choice))
            throw new ArgumentException($"Choice is not a regular move: {choice}");

        string choiceStr = choice + "WithTera";
        if (Enum.TryParse(choiceStr, out Choice result))
            return result;

        throw new ArgumentException($"Could not convert move to Tera choice: {choice}");
    }

    /// <summary>
    /// Checks if a choice is a switch choice.
    /// </summary>
    public static bool IsSwitchChoice(this Choice choice)
    {
        return choice is >= Choice.Switch1 and <= Choice.Switch4;
    }

    /// <summary>
    /// Gets the switch index (0-3) from a switch choice.
    /// </summary>
    public static int GetSwitchIndexFromChoice(this Choice choice)
    {
        if (!IsSwitchChoice(choice))
            throw new ArgumentException($"Choice is not a switch choice: {choice}");

        return choice switch
        {
            Choice.Switch1 => 0,
            Choice.Switch2 => 1,
            Choice.Switch3 => 2,
            Choice.Switch4 => 3,
            _ => throw new ArgumentException($"Invalid switch choice: {choice}"),
        };
    }

    /// <summary>
    /// Creates a switch choice from an index (0-3).
    /// </summary>
    public static Choice GetChoiceFromSwitchIndex(int index)
    {
        return index switch
        {
            0 => Choice.Switch1,
            1 => Choice.Switch2,
            2 => Choice.Switch3,
            3 => Choice.Switch4,
            _ => throw new ArgumentOutOfRangeException(nameof(index), "Switch index must be between 0 and 3."),
        };
    }

    /// <summary>
    /// Checks if a choice is a struggle choice.
    /// </summary>
    public static bool IsStruggleChoice(this Choice choice)
    {
        return choice == Choice.Struggle;
    }

    public static Choice GetAllAdjacentFoesMoveChoice(int moveIndex)
    {
        return moveIndex switch
        {
            1 => Choice.Move1AllAdjacentFoes,
            2 => Choice.Move2AllAdjacentFoes,
            3 => Choice.Move3AllAdjacentFoes,
            4 => Choice.Move4AllAdjacentFoes,
            _ => throw new ArgumentOutOfRangeException(nameof(moveIndex), "Move index must be between 1 and 4."),
        };
    }



    // =============================================================================
    // TEAM PREVIEW EXTENSION METHODS
    // =============================================================================

    /// <summary>
    /// Checks if a choice is a team preview combination choice.
    /// </summary>
    public static bool IsTeamPreviewChoice(this Choice choice)
    {
        return choice.ToString().StartsWith("TeamPreview");
    }

    /// <summary>
    /// Decodes a team preview choice into individual slot selections.
    /// Returns the Pokémon index (1-6) for each slot.
    /// </summary>
    public static (int slot1Selection, int slot2Selection) DecodeTeamPreviewChoice(this Choice choice)
    {
        if (!IsTeamPreviewChoice(choice))
        {
            throw new ArgumentException($"Not a valid team preview choice: {choice}");
        }

        string choiceName = choice.ToString();
        
        // Pattern: TeamPreviewSlot1Select{X}_Slot2Select{Y}
        int slot1Index = choiceName.IndexOf("Slot1Select", StringComparison.Ordinal);
        int slot2Index = choiceName.IndexOf("_Slot2Select", StringComparison.Ordinal);
        
        if (slot1Index == -1 || slot2Index == -1)
        {
            throw new ArgumentException($"Invalid team preview choice format: {choice}");
        }

        string slot1SelectStr = choiceName.Substring(slot1Index + 11, slot2Index - slot1Index - 11);
        string slot2SelectStr = choiceName[(slot2Index + 12)..];

        if (!int.TryParse(slot1SelectStr, out int slot1Selection) ||
            !int.TryParse(slot2SelectStr, out int slot2Selection))
        {
            throw new ArgumentException($"Could not parse team preview selections from: {choice}");
        }

        return (slot1Selection, slot2Selection);
    }

    /// <summary>
    /// Creates a team preview choice from individual slot selections.
    /// </summary>
    /// <param name="slot1Selection">Pokémon index (1-6) for slot 1</param>
    /// <param name="slot2Selection">Pokémon index (1-6) for slot 2</param>
    /// <returns>The corresponding team preview choice</returns>
    public static Choice CreateTeamPreviewChoice(int slot1Selection, int slot2Selection)
    {
        if (slot1Selection < 1 || slot1Selection > 6 || slot2Selection < 1 || slot2Selection > 6)
        {
            throw new ArgumentException("Selections must be between 1 and 6");
        }

        if (slot1Selection == slot2Selection)
        {
            throw new ArgumentException("Cannot select the same Pokémon for both slots");
        }

        string enumName = $"TeamPreviewSlot1Select{slot1Selection}_Slot2Select{slot2Selection}";
        
        if (Enum.TryParse(enumName, out Choice teamPreviewChoice))
        {
            return teamPreviewChoice;
        }
        
        throw new ArgumentException($"Team preview choice not found in enum: {enumName}");
    }

    /// <summary>
    /// Gets a human-readable description of a team preview choice.
    /// </summary>
    public static string GetTeamPreviewDescription(this Choice choice)
    {
        if (!IsTeamPreviewChoice(choice))
        {
            throw new ArgumentException($"Not a team preview choice: {choice}");
        }

        (int slot1, int slot2) = DecodeTeamPreviewChoice(choice);
        return $"Slot 1: Pokémon {slot1} | Slot 2: Pokémon {slot2}";
    }

    /// <summary>
    /// Gets the Pokémon selection for slot 1 from a team preview choice.
    /// </summary>
    public static int GetSlot1Selection(this Choice choice)
    {
        if (!IsTeamPreviewChoice(choice))
        {
            throw new ArgumentException($"Not a team preview choice: {choice}");
        }

        return DecodeTeamPreviewChoice(choice).slot1Selection;
    }

    /// <summary>
    /// Gets the Pokémon selection for slot 2 from a team preview choice.
    /// </summary>
    public static int GetSlot2Selection(this Choice choice)
    {
        if (!IsTeamPreviewChoice(choice))
        {
            throw new ArgumentException($"Not a team preview choice: {choice}");
        }

        return DecodeTeamPreviewChoice(choice).slot2Selection;
    }

    /// <summary>
    /// Generates all valid team preview choices.
    /// </summary>
    /// <returns>Array of all valid team preview combinations</returns>
    public static Choice[] GenerateAllTeamPreviewChoices(int teamCount = 6)
    {
        var choices = new List<Choice>();
        
        for (int slot1 = 1; slot1 <= teamCount; slot1++)
        {
            for (int slot2 = 1; slot2 <= teamCount; slot2++)
            {
                if (slot1 == slot2) continue; // Can't select same Pokémon for both slots
                try
                {
                    Choice choice = CreateTeamPreviewChoice(slot1, slot2);
                    choices.Add(choice);
                }
                catch
                {
                    // Skip if the choice doesn't exist in the enum
                }
            }
        }
        
        return choices.ToArray();
    }

    /// <summary>
    /// Checks if a team preview choice combination is valid.
    /// </summary>
    public static bool IsValidTeamPreviewChoice(int slot1Selection, int slot2Selection)
    {
        try
        {
            CreateTeamPreviewChoice(slot1Selection, slot2Selection);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // =============================================================================
    // DOUBLES BATTLE EXTENSION METHODS
    // =============================================================================

    /// <summary>
    /// Checks if a choice is a doubles combination choice.
    /// </summary>
    public static bool IsDoublesChoice(this Choice choice)
    {
        return ChoiceGenerator.IsDoublesChoice(choice);
    }

    /// <summary>
    /// Checks if a doubles choice represents a single-slot scenario.
    /// </summary>
    public static bool IsSingleSlotChoice(this Choice choice)
    {
        return ChoiceGenerator.IsSingleSlotChoice(choice);
    }

    /// <summary>
    /// Decodes a doubles choice into individual slot choices.
    /// Returns null for empty slots in single-slot scenarios.
    /// </summary>
    public static (Choice? slot1Choice, Choice? slot2Choice) DecodeDoublesChoice(this Choice choice)
    {
        return ChoiceGenerator.DecodeDoublesChoice(choice);
    }

    /// <summary>
    /// Creates a doubles choice from two individual slot choices.
    /// Pass null for empty slots in single-slot scenarios.
    /// </summary>
    public static Choice CreateDoublesChoice(Choice? slot1Choice, Choice? slot2Choice)
    {
        return ChoiceGenerator.CreateDoublesChoice(slot1Choice, slot2Choice);
    }

    /// <summary>
    /// Checks if a choice involves any move action (including in doubles combinations).
    /// </summary>
    public static bool ContainsMoveAction(this Choice choice)
    {
        if (IsMoveChoice(choice) || IsMoveWithTeraChoice(choice) || choice == Choice.Struggle)
            return true;

        if (!IsDoublesChoice(choice))
            return false;

        var (slot1, slot2) = DecodeDoublesChoice(choice);
        return (slot1 != null && ContainsMoveAction(slot1.Value)) || 
               (slot2 != null && ContainsMoveAction(slot2.Value));
    }

    /// <summary>
    /// Checks if a choice involves any switch action (including in doubles combinations).
    /// </summary>
    public static bool ContainsSwitchAction(this Choice choice)
    {
        if (IsSwitchChoice(choice))
            return true;

        if (!IsDoublesChoice(choice))
            return false;

        var (slot1, slot2) = DecodeDoublesChoice(choice);
        return (slot1 != null && IsSwitchChoice(slot1.Value)) || 
               (slot2 != null && IsSwitchChoice(slot2.Value));
    }

    /// <summary>
    /// Gets the slot 1 choice from a doubles choice, or the choice itself if it's a single choice.
    /// Returns null if slot 1 is empty.
    /// </summary>
    public static Choice? GetSlot1Choice(this Choice choice)
    {
        return !IsDoublesChoice(choice) ? choice : DecodeDoublesChoice(choice).slot1Choice;
    }

    /// <summary>
    /// Gets the slot 2 choice from a doubles choice.
    /// Returns null if slot 2 is empty or if it's a single choice.
    /// </summary>
    public static Choice? GetSlot2Choice(this Choice choice)
    {
        return !IsDoublesChoice(choice) ? null : DecodeDoublesChoice(choice).slot2Choice;
    }

    /// <summary>
    /// Generates all valid doubles choices from available single choices for each slot.
    /// Pass null for slots that have no available Pokémon.
    /// </summary>
    public static Choice[] GenerateDoublesChoices(Choice[]? slot1Choices, Choice[]? slot2Choices)
    {
        return ChoiceGenerator.GenerateValidDoublesChoices(slot1Choices, slot2Choices);
    }

    /// <summary>
    /// Checks if a doubles choice combination is valid.
    /// </summary>
    public static bool IsValidDoublesChoice(Choice? slot1Choice, Choice? slot2Choice)
    {
        try
        {
            CreateDoublesChoice(slot1Choice, slot2Choice);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets a human-readable description of a choice (including doubles combinations).
    /// </summary>
    public static string GetChoiceDescription(this Choice choice)
    {
        if (IsTeamPreviewChoice(choice))
        {
            return GetTeamPreviewDescription(choice);
        }

        if (!IsDoublesChoice(choice))
        {
            return GetChoiceName(choice);
        }

        var (slot1, slot2) = DecodeDoublesChoice(choice);
        
        string slot1Desc = slot1 != null ? GetChoiceName(slot1.Value) : "Empty";
        string slot2Desc = slot2 != null ? GetChoiceName(slot2.Value) : "Empty";
        
        return $"Slot 1: {slot1Desc} | Slot 2: {slot2Desc}";
    }

    /// <summary>
    /// Checks if a choice represents a scenario where only one Pokémon is active.
    /// </summary>
    public static bool IsLastPokemonScenario(this Choice choice)
    {
        return IsSingleSlotChoice(choice);
    }

    /// <summary>
    /// Gets which slot is active in a single-slot choice.
    /// Returns 1 for slot1 active, 2 for slot2 active, or 0 for dual-slot.
    /// </summary>
    public static int GetActiveSlot(this Choice choice)
    {
        if (!IsSingleSlotChoice(choice))
            return 0; // Dual-slot or not a doubles choice

        string choiceStr = choice.ToString();
        if (choiceStr.Contains("_Slot2Empty"))
            return 1; // Only slot 1 is active
        if (choiceStr.StartsWith("Slot1Empty_"))
            return 2; // Only slot 2 is active

        return 0;
    }
}