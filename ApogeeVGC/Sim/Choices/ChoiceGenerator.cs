using System.Text;

namespace ApogeeVGC.Sim.Choices;

/// <summary>
/// Code generation utility for creating all possible doubles battle choice combinations.
/// This generates the enum entries for Choice.cs to handle doubles battles.
/// </summary>
public static class ChoiceGenerator
{
    // All single-slot choices from the current enum
    private static readonly Choice[] SingleMoveChoices = 
    [
        // Move 1
        Choice.Move1NormalFoe1,
        Choice.Move1NormalFoe2,
        Choice.Move1NormalAlly,
        Choice.Move1AllAdjacentFoes,
        Choice.Move1Field,
        Choice.Move1AllySide,
        Choice.Move1Self,
        Choice.Move1NormalFoe1WithTera,
        Choice.Move1NormalFoe2WithTera,
        Choice.Move1NormalAllyWithTera,
        Choice.Move1AllAdjacentFoesWithTera,
        Choice.Move1FieldWithTera,
        Choice.Move1AllySideWithTera,
        Choice.Move1SelfWithTera,
        
        // Move 2
        Choice.Move2NormalFoe1,
        Choice.Move2NormalFoe2,
        Choice.Move2NormalAlly,
        Choice.Move2AllAdjacentFoes,
        Choice.Move2Field,
        Choice.Move2AllySide,
        Choice.Move2Self,
        Choice.Move2NormalFoe1WithTera,
        Choice.Move2NormalFoe2WithTera,
        Choice.Move2NormalAllyWithTera,
        Choice.Move2AllAdjacentFoesWithTera,
        Choice.Move2FieldWithTera,
        Choice.Move2AllySideWithTera,
        Choice.Move2SelfWithTera,
        
        // Move 3
        Choice.Move3NormalFoe1,
        Choice.Move3NormalFoe2,
        Choice.Move3NormalAlly,
        Choice.Move3AllAdjacentFoes,
        Choice.Move3Field,
        Choice.Move3AllySide,
        Choice.Move3Self,
        Choice.Move3NormalFoe1WithTera,
        Choice.Move3NormalFoe2WithTera,
        Choice.Move3NormalAllyWithTera,
        Choice.Move3AllAdjacentFoesWithTera,
        Choice.Move3FieldWithTera,
        Choice.Move3AllySideWithTera,
        Choice.Move3SelfWithTera,
        
        // Move 4
        Choice.Move4NormalFoe1,
        Choice.Move4NormalFoe2,
        Choice.Move4NormalAlly,
        Choice.Move4AllAdjacentFoes,
        Choice.Move4Field,
        Choice.Move4AllySide,
        Choice.Move4Self,
        Choice.Move4NormalFoe1WithTera,
        Choice.Move4NormalFoe2WithTera,
        Choice.Move4NormalAllyWithTera,
        Choice.Move4AllAdjacentFoesWithTera,
        Choice.Move4FieldWithTera,
        Choice.Move4AllySideWithTera,
        Choice.Move4SelfWithTera,
    ];

    private static readonly Choice[] SingleSwitchChoices = 
    [
        Choice.Switch1,
        Choice.Switch2,
        Choice.Switch3,
        Choice.Switch4,
    ];

    private static readonly Choice[] SingleStruggleChoices = 
    [
        Choice.Struggle,
    ];

    /// <summary>
    /// Generates all possible doubles battle combinations as C# enum entries.
    /// This includes both dual-slot and single-slot scenarios.
    /// </summary>
    /// <returns>String containing all enum entries for doubles battles</returns>
    public static string GenerateDoublesChoiceEnumEntries()
    {
        var sb = new StringBuilder();
        sb.AppendLine("    // =============================================================================");
        sb.AppendLine("    // DOUBLES BATTLE COMBINATIONS (AUTO-GENERATED)");
        sb.AppendLine("    // =============================================================================");
        sb.AppendLine();

        // All possible single choices (moves + switches + struggle)
        var allSingleChoices = SingleMoveChoices
            .Concat(SingleSwitchChoices)
            .Concat(SingleStruggleChoices)
            .ToArray();

        int totalCombinations = 0;

        // Generate dual-slot combinations: Slot1 × Slot2
        sb.AppendLine("    // Dual-slot combinations (both slots active)");
        foreach (Choice slot1Choice in allSingleChoices)
        {
            foreach (Choice slot2Choice in allSingleChoices)
            {
                // Skip invalid combinations
                if (IsInvalidCombination(slot1Choice, slot2Choice))
                    continue;

                string enumEntry = GenerateEnumEntry(slot1Choice, slot2Choice);
                sb.AppendLine($"    {enumEntry},");
                totalCombinations++;
            }
        }

        sb.AppendLine();
        sb.AppendLine("    // Single-slot combinations (one slot empty)");
        
        // Generate single-slot combinations: Slot1 only (Slot2 empty)
        sb.AppendLine("    // Slot1 active, Slot2 empty");
        foreach (Choice slot1Choice in allSingleChoices)
        {
            string enumEntry = GenerateSingleSlotEntry(slot1Choice, isSlot1: true);
            sb.AppendLine($"    {enumEntry},");
            totalCombinations++;
        }

        sb.AppendLine();
        // Generate single-slot combinations: Slot2 only (Slot1 empty)
        sb.AppendLine("    // Slot1 empty, Slot2 active");
        foreach (Choice slot2Choice in allSingleChoices)
        {
            string enumEntry = GenerateSingleSlotEntry(slot2Choice, isSlot1: false);
            sb.AppendLine($"    {enumEntry},");
            totalCombinations++;
        }

        sb.AppendLine();
        sb.AppendLine($"    // Total doubles combinations generated: {totalCombinations}");
        sb.AppendLine($"    // Breakdown: Dual-slot + Single-slot1 + Single-slot2");
        
        return sb.ToString();
    }

    /// <summary>
    /// Generates the enum entry name for a combination of two choices.
    /// </summary>
    private static string GenerateEnumEntry(Choice slot1Choice, Choice slot2Choice)
    {
        string slot1Name = GetChoiceName(slot1Choice);
        string slot2Name = GetChoiceName(slot2Choice);
        return $"Slot1{slot1Name}_Slot2{slot2Name}";
    }

    /// <summary>
    /// Generates the enum entry name for a single-slot choice (other slot empty).
    /// </summary>
    private static string GenerateSingleSlotEntry(Choice choice, bool isSlot1)
    {
        string choiceName = GetChoiceName(choice);
        return isSlot1 ? $"Slot1{choiceName}_Slot2Empty" : $"Slot1Empty_Slot2{choiceName}";
    }

    /// <summary>
    /// Gets the choice name without the "Choice." prefix for enum generation.
    /// </summary>
    private static string GetChoiceName(Choice choice)
    {
        return choice.ToString();
    }

    /// <summary>
    /// Determines if a combination of choices is invalid.
    /// </summary>
    private static bool IsInvalidCombination(Choice slot1Choice, Choice slot2Choice)
    {
        // Can't switch the same Pokemon in both slots
        if (IsSwitchChoice(slot1Choice) && IsSwitchChoice(slot2Choice))
        {
            return slot1Choice == slot2Choice; // Same switch choice is invalid
        }

        // Other invalid combinations can be added here
        // For example:
        // - Can't target ally if ally is switching
        // - etc.

        return false;
    }

    /// <summary>
    /// Checks if a choice is a switch choice.
    /// </summary>
    private static bool IsSwitchChoice(Choice choice)
    {
        return SingleSwitchChoices.Contains(choice);
    }

    /// <summary>
    /// Checks if a choice is a move choice.
    /// </summary>
    public static bool IsMoveChoice(Choice choice)
    {
        return SingleMoveChoices.Contains(choice);
    }

    /// <summary>
    /// Checks if a choice is a struggle choice.
    /// </summary>
    public static bool IsStruggleChoice(Choice choice)
    {
        return choice == Choice.Struggle;
    }

    /// <summary>
    /// Writes the generated doubles choices to a file for easy copying into Choice.cs.
    /// </summary>
    public static void WriteDoublesChoicesToFile(string filePath = "DoublesChoices.txt")
    {
        string content = GenerateDoublesChoiceEnumEntries();
        File.WriteAllText(filePath, content);
        Console.WriteLine($"Doubles choices written to: {filePath}");
        Console.WriteLine($"Total lines: {content.Split('\n').Length}");
    }

    /// <summary>
    /// Helper method to decode a doubles choice back into individual slot choices.
    /// This will be useful for your battle system to process the choices.
    /// </summary>
    public static (Choice? slot1Choice, Choice? slot2Choice) DecodeDoublesChoice(Choice doublesChoice)
    {
        string choiceName = doublesChoice.ToString();
        
        if (!IsDoublesChoice(doublesChoice))
        {
            throw new ArgumentException($"Not a valid doubles choice: {doublesChoice}");
        }

        // Handle single-slot cases
        if (choiceName.Contains("_Slot2Empty"))
        {
            // Slot1 active, Slot2 empty
            string slot1Name = choiceName.Substring(5, choiceName.IndexOf("_Slot2Empty", StringComparison.Ordinal) - 5);
            if (Enum.TryParse(slot1Name, out Choice slot1Choice))
                return (slot1Choice, null);
        }
        else if (choiceName.StartsWith("Slot1Empty_Slot2"))
        {
            // Slot1 empty, Slot2 active
            string slot2Name = choiceName.Substring(16); // Remove "Slot1Empty_Slot2" prefix
            if (Enum.TryParse(slot2Name, out Choice slot2Choice))
                return (null, slot2Choice);
        }
        else
        {
            // Dual-slot case
            int slot2Index = choiceName.IndexOf("_Slot2", StringComparison.Ordinal);
            string slot1Name = choiceName.Substring(5, slot2Index - 5); // Remove "Slot1" prefix
            string slot2Name = choiceName[(slot2Index + 7)..]; // Remove "_Slot2" prefix

            // Parse back to enum values
            if (Enum.TryParse(slot1Name, out Choice slot1Choice) && 
                Enum.TryParse(slot2Name, out Choice slot2Choice))
            {
                return (slot1Choice, slot2Choice);
            }
        }

        throw new ArgumentException($"Could not decode doubles choice: {doublesChoice}");
    }

    /// <summary>
    /// Creates a doubles choice from two individual slot choices.
    /// Supports single-slot scenarios by passing null for empty slots.
    /// </summary>
    public static Choice CreateDoublesChoice(Choice? slot1Choice, Choice? slot2Choice)
    {
        // Validate input
        if (slot1Choice == null && slot2Choice == null)
        {
            throw new ArgumentException("At least one slot must have a choice");
        }

        string enumName;
        
        if (slot1Choice != null && slot2Choice != null)
        {
            // Dual-slot case
            if (IsInvalidCombination(slot1Choice.Value, slot2Choice.Value))
            {
                throw new ArgumentException($"Invalid combination: {slot1Choice} + {slot2Choice}");
            }
            enumName = GenerateEnumEntry(slot1Choice.Value, slot2Choice.Value);
        }
        else if (slot1Choice != null && slot2Choice == null)
        {
            // Slot1 active, Slot2 empty
            enumName = GenerateSingleSlotEntry(slot1Choice.Value, isSlot1: true);
        }
        else
        {
            // Slot1 empty, Slot2 active
            enumName = GenerateSingleSlotEntry(slot2Choice!.Value, isSlot1: false);
        }
        
        if (Enum.TryParse(enumName, out Choice doublesChoice))
        {
            return doublesChoice;
        }
        
        throw new ArgumentException($"Doubles choice not found in enum: {enumName}");
    }

    /// <summary>
    /// Checks if a choice is a doubles combination choice.
    /// </summary>
    public static bool IsDoublesChoice(Choice choice)
    {
        string choiceStr = choice.ToString();
        return (choiceStr.StartsWith("Slot1") && choiceStr.Contains("_Slot2")) ||
               choiceStr.Contains("Empty");
    }

    /// <summary>
    /// Checks if a doubles choice represents a single-slot scenario.
    /// </summary>
    public static bool IsSingleSlotChoice(Choice choice)
    {
        return choice.ToString().Contains("Empty");
    }

    /// <summary>
    /// Gets all possible doubles choices for given available single choices.
    /// Supports both dual-slot and single-slot scenarios.
    /// </summary>
    public static Choice[] GenerateValidDoublesChoices(Choice[]? slot1Choices, Choice[]? slot2Choices)
    {
        var validCombinations = new List<Choice>();

        // Handle dual-slot combinations
        if (slot1Choices != null && slot2Choices != null)
        {
            foreach (Choice slot1Choice in slot1Choices)
            {
                foreach (Choice slot2Choice in slot2Choices)
                {
                    if (IsInvalidCombination(slot1Choice, slot2Choice)) continue;
                    try
                    {
                        Choice doublesChoice = CreateDoublesChoice(slot1Choice, slot2Choice);
                        validCombinations.Add(doublesChoice);
                    }
                    catch
                    {
                        // Skip if the doubles choice doesn't exist in the enum
                    }
                }
            }
        }

        // Handle single-slot combinations
        if (slot1Choices != null && slot2Choices == null)
        {
            // Only slot1 is active
            foreach (Choice slot1Choice in slot1Choices)
            {
                try
                {
                    Choice doublesChoice = CreateDoublesChoice(slot1Choice, null);
                    validCombinations.Add(doublesChoice);
                }
                catch
                {
                    // Skip if the doubles choice doesn't exist in the enum
                }
            }
        }
        else if (slot1Choices == null && slot2Choices != null)
        {
            // Only slot2 is active
            foreach (Choice slot2Choice in slot2Choices)
            {
                try
                {
                    Choice doublesChoice = CreateDoublesChoice(null, slot2Choice);
                    validCombinations.Add(doublesChoice);
                }
                catch
                {
                    // Skip if the doubles choice doesn't exist in the enum
                }
            }
        }

        return validCombinations.ToArray();
    }

    /// <summary>
    /// Gets statistics about the generated combinations.
    /// </summary>
    public static void PrintCombinationStats()
    {
        int moveCount = SingleMoveChoices.Length;
        int switchCount = SingleSwitchChoices.Length;
        int struggleCount = SingleStruggleChoices.Length;
        int totalSingleChoices = moveCount + switchCount + struggleCount;

        Console.WriteLine("=== Doubles Choice Generation Statistics ===");
        Console.WriteLine($"Single move choices: {moveCount}");
        Console.WriteLine($"Single switch choices: {switchCount}");
        Console.WriteLine($"Single struggle choices: {struggleCount}");
        Console.WriteLine($"Total single choices: {totalSingleChoices}");
        Console.WriteLine();
        
        // Calculate dual-slot combinations
        var allSingleChoices = SingleMoveChoices
            .Concat(SingleSwitchChoices)
            .Concat(SingleStruggleChoices)
            .ToArray();

        int theoreticalDualSlot = totalSingleChoices * totalSingleChoices;
        int actualDualSlot = allSingleChoices.Sum(slot1Choice =>
            allSingleChoices.Count(slot2Choice =>
                !IsInvalidCombination(slot1Choice, slot2Choice)));

        // Calculate single-slot combinations
        int singleSlotCombinations = totalSingleChoices * 2; // Slot1Empty + Slot2Empty scenarios

        int totalCombinations = actualDualSlot + singleSlotCombinations;

        Console.WriteLine($"Dual-slot combinations: {actualDualSlot}");
        Console.WriteLine($"Single-slot combinations: {singleSlotCombinations}");
        Console.WriteLine($"Total combinations: {totalCombinations}");
        Console.WriteLine($"Invalid dual-slot combinations filtered: {theoreticalDualSlot - actualDualSlot}");
    }
}