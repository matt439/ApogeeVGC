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
    /// This includes both dual-slot and single-slot scenarios, plus team preview choices.
    /// </summary>
    /// <returns>String containing all enum entries for doubles battles</returns>
    public static string GenerateDoublesChoiceEnumEntries()
    {
        var sb = new StringBuilder();
        sb.AppendLine("    // =============================================================================");
        sb.AppendLine("    // TEAM PREVIEW CHOICES (4-SLOT FORMAT)");
        sb.AppendLine("    // =============================================================================");
        sb.AppendLine("    // Team Preview combinations: Select 4 different Pokémon from roster of 6");
        sb.AppendLine("    // Pattern: TeamPreviewSlot1Select{A}_Slot2Select{B}_Slot3Select{C}_Slot4Select{D}");
        sb.AppendLine("    // where A, B, C, D are all different values from 1-6");
        sb.AppendLine();

        int teamPreviewCombinations = 0;
        
        // Generate all 4-slot team preview combinations
        for (int slot1 = 1; slot1 <= 6; slot1++)
        {
            for (int slot2 = 1; slot2 <= 6; slot2++)
            {
                if (slot2 == slot1) continue;
                
                for (int slot3 = 1; slot3 <= 6; slot3++)
                {
                    if (slot3 == slot1 || slot3 == slot2) continue;
                    
                    for (int slot4 = 1; slot4 <= 6; slot4++)
                    {
                        if (slot4 == slot1 || slot4 == slot2 || slot4 == slot3) continue;
                        
                        string enumEntry = $"TeamPreviewSlot1Select{slot1}_Slot2Select{slot2}_Slot3Select{slot3}_Slot4Select{slot4}";
                        sb.AppendLine($"    {enumEntry},");
                        teamPreviewCombinations++;
                    }
                }
            }
        }
        
        sb.AppendLine();
        sb.AppendLine($"    // Total Team Preview combinations: {teamPreviewCombinations} (6 × 5 × 4 × 3 = 360)");
        sb.AppendLine();

        sb.AppendLine("    // =============================================================================");
        sb.AppendLine("    // DOUBLES BATTLE COMBINATIONS (AUTO-GENERATED)");
        sb.AppendLine("    // =============================================================================");
        sb.AppendLine();

        // All possible single choices (moves + switches + struggle)
        var allSingleChoices = SingleMoveChoices
            .Concat(SingleSwitchChoices)
            .Concat(SingleStruggleChoices)
            .ToArray();

        int totalCombinations = teamPreviewCombinations;

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
        sb.AppendLine($"    // Total combinations generated: {totalCombinations}");
        sb.AppendLine($"    // Breakdown: Team Preview ({teamPreviewCombinations}) + Dual-slot + Single-slot1 + Single-slot2");
        
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
            // Pattern: Slot1{ChoiceName}_Slot2Empty
            int emptyIndex = choiceName.IndexOf("_Slot2Empty", StringComparison.Ordinal);
            string slot1Name = choiceName.Substring(5, emptyIndex - 5); // Remove "Slot1" prefix
            if (Enum.TryParse(slot1Name, out Choice slot1Choice))
                return (slot1Choice, null);
        }
        else if (choiceName.StartsWith("Slot1Empty_Slot2"))
        {
            // Slot1 empty, Slot2 active
            // Pattern: Slot1Empty_Slot2{ChoiceName}
            string slot2Name = choiceName.Substring(16); // Remove "Slot1Empty_Slot2" prefix
            if (Enum.TryParse(slot2Name, out Choice slot2Choice))
                return (null, slot2Choice);
        }
        else
        {
            // Dual-slot case
            // Pattern: Slot1{ChoiceName}_Slot2{ChoiceName}
            int slot2Index = choiceName.IndexOf("_Slot2", StringComparison.Ordinal);
            if (slot2Index == -1)
            {
                throw new ArgumentException($"Invalid doubles choice format - missing '_Slot2': {doublesChoice}");
            }
            
            string slot1Name = choiceName.Substring(5, slot2Index - 5); // Remove "Slot1" prefix
            string slot2Name = choiceName.Substring(slot2Index + 6); // Remove "_Slot2" prefix (6 characters)

            // Parse back to enum values
            if (Enum.TryParse(slot1Name, out Choice slot1Choice) && 
                Enum.TryParse(slot2Name, out Choice slot2Choice))
            {
                return (slot1Choice, slot2Choice);
            }
            
            // Debug information for troubleshooting
            throw new ArgumentException($"Could not parse choices from: {doublesChoice}. " +
                                      $"Extracted slot1Name: '{slot1Name}', slot2Name: '{slot2Name}'");
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
                        // This can happen if not all combinations have been generated yet
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
    /// Diagnostic method to help troubleshoot when GenerateValidDoublesChoices returns empty results.
    /// This method provides detailed information about what combinations were attempted and why they failed.
    /// </summary>
    public static void DiagnoseDoublesChoiceGeneration(Choice[]? slot1Choices, Choice[]? slot2Choices)
    {
        Console.WriteLine("=== Diagnosing Doubles Choice Generation ===");
        
        if (slot1Choices == null && slot2Choices == null)
        {
            Console.WriteLine("ERROR: Both slot choice arrays are null");
            return;
        }
        
        Console.WriteLine($"Slot1 choices count: {slot1Choices?.Length ?? 0}");
        Console.WriteLine($"Slot2 choices count: {slot2Choices?.Length ?? 0}");
        
        if (slot1Choices != null)
        {
            Console.WriteLine("Slot1 choices: " + string.Join(", ", slot1Choices));
        }
        
        if (slot2Choices != null)
        {
            Console.WriteLine("Slot2 choices: " + string.Join(", ", slot2Choices));
        }
        
        int totalAttempts = 0;
        int invalidCombinations = 0;
        int enumNotFound = 0;
        int successful = 0;
        
        // Test dual-slot combinations
        if (slot1Choices != null && slot2Choices != null)
        {
            Console.WriteLine("\nTesting dual-slot combinations:");
            foreach (Choice slot1Choice in slot1Choices)
            {
                foreach (Choice slot2Choice in slot2Choices)
                {
                    totalAttempts++;
                    
                    if (IsInvalidCombination(slot1Choice, slot2Choice))
                    {
                        invalidCombinations++;
                        Console.WriteLine($"  INVALID: {slot1Choice} + {slot2Choice} - Invalid combination");
                        continue;
                    }
                    
                    try
                    {
                        Choice doublesChoice = CreateDoublesChoice(slot1Choice, slot2Choice);
                        successful++;
                        Console.WriteLine($"  SUCCESS: {slot1Choice} + {slot2Choice} -> {doublesChoice}");
                    }
                    catch (Exception ex)
                    {
                        enumNotFound++;
                        Console.WriteLine($"  FAILED: {slot1Choice} + {slot2Choice} - {ex.Message}");
                    }
                }
            }
        }
        
        // Test single-slot combinations
        if (slot1Choices != null && slot2Choices == null)
        {
            Console.WriteLine("\nTesting slot1-only combinations:");
            foreach (Choice slot1Choice in slot1Choices)
            {
                totalAttempts++;
                try
                {
                    Choice doublesChoice = CreateDoublesChoice(slot1Choice, null);
                    successful++;
                    Console.WriteLine($"  SUCCESS: {slot1Choice} + null -> {doublesChoice}");
                }
                catch (Exception ex)
                {
                    enumNotFound++;
                    Console.WriteLine($"  FAILED: {slot1Choice} + null - {ex.Message}");
                }
            }
        }
        
        if (slot1Choices == null && slot2Choices != null)
        {
            Console.WriteLine("\nTesting slot2-only combinations:");
            foreach (Choice slot2Choice in slot2Choices)
            {
                totalAttempts++;
                try
                {
                    Choice doublesChoice = CreateDoublesChoice(null, slot2Choice);
                    successful++;
                    Console.WriteLine($"  SUCCESS: null + {slot2Choice} -> {doublesChoice}");
                }
                catch (Exception ex)
                {
                    enumNotFound++;
                    Console.WriteLine($"  FAILED: null + {slot2Choice} - {ex.Message}");
                }
            }
        }
        
        Console.WriteLine("\n=== Summary ===");
        Console.WriteLine($"Total attempts: {totalAttempts}");
        Console.WriteLine($"Invalid combinations: {invalidCombinations}");
        Console.WriteLine($"Enum not found: {enumNotFound}");
        Console.WriteLine($"Successful: {successful}");
        
        if (successful == 0)
        {
            Console.WriteLine("\nWARNING: No valid doubles choices could be generated!");
            if (enumNotFound > 0)
            {
                Console.WriteLine("This is likely because the Choice enum doesn't contain all the required doubles choice combinations.");
                Console.WriteLine("You may need to run ChoiceGenerator.WriteDoublesChoicesToFile() and update your Choice.cs enum.");
            }
        }
    }

    /// <summary>
    /// Checks if the Choice enum contains all necessary doubles choice combinations for the given single choices.
    /// This helps identify missing enum entries that need to be added.
    /// </summary>
    public static bool ValidateDoublesChoiceEnumCompleteness(Choice[]? slot1Choices, Choice[]? slot2Choices)
    {
        if (slot1Choices == null || slot2Choices == null)
        {
            return false;
        }
        
        Console.WriteLine("=== Validating Doubles Choice Enum Completeness ===");
        var missingCombinations = new List<string>();
        
        // Check dual-slot combinations
        foreach (Choice slot1Choice in slot1Choices)
        {
            foreach (Choice slot2Choice in slot2Choices)
            {
                if (IsInvalidCombination(slot1Choice, slot2Choice)) continue;
                
                string enumName = GenerateEnumEntry(slot1Choice, slot2Choice);
                if (!Enum.TryParse<Choice>(enumName, out _))
                {
                    missingCombinations.Add(enumName);
                }
            }
        }
        
        // Check single-slot combinations
        foreach (Choice slot1Choice in slot1Choices)
        {
            string enumName = GenerateSingleSlotEntry(slot1Choice, isSlot1: true);
            if (!Enum.TryParse<Choice>(enumName, out _))
            {
                missingCombinations.Add(enumName);
            }
        }
        
        foreach (Choice slot2Choice in slot2Choices)
        {
            string enumName = GenerateSingleSlotEntry(slot2Choice, isSlot1: false);
            if (!Enum.TryParse<Choice>(enumName, out _))
            {
                missingCombinations.Add(enumName);
            }
        }
        
        if (missingCombinations.Count > 0)
        {
            Console.WriteLine($"Found {missingCombinations.Count} missing enum combinations:");
            foreach (string missing in missingCombinations.Take(10)) // Show first 10
            {
                Console.WriteLine($"  {missing}");
            }
            if (missingCombinations.Count > 10)
            {
                Console.WriteLine($"  ... and {missingCombinations.Count - 10} more");
            }
            Console.WriteLine("\nTo fix this, run ChoiceGenerator.WriteDoublesChoicesToFile() and add the generated enums to Choice.cs");
            return false;
        }
        
        Console.WriteLine("All required doubles choice combinations are present in the enum.");
        return true;
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

        Console.WriteLine("=== Choice Generation Statistics ===");
        Console.WriteLine();
        
        // Team Preview Statistics
        Console.WriteLine("TEAM PREVIEW (4-slot format):");
        Console.WriteLine($"  Combinations: 6 × 5 × 4 × 3 = 360");
        Console.WriteLine();

        // Doubles Battle Statistics
        Console.WriteLine("DOUBLES BATTLE:");
        Console.WriteLine($"  Single move choices: {moveCount}");
        Console.WriteLine($"  Single switch choices: {switchCount}");
        Console.WriteLine($"  Single struggle choices: {struggleCount}");
        Console.WriteLine($"  Total single choices: {totalSingleChoices}");
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
        int teamPreviewCombinations = 360; // 6 × 5 × 4 × 3
        int totalCombinations = teamPreviewCombinations + actualDualSlot + singleSlotCombinations;

        Console.WriteLine($"  Dual-slot combinations: {actualDualSlot}");
        Console.WriteLine($"  Single-slot combinations: {singleSlotCombinations}");
        Console.WriteLine($"  Doubles total: {actualDualSlot + singleSlotCombinations}");
        Console.WriteLine($"  Invalid dual-slot combinations filtered: {theoreticalDualSlot - actualDualSlot}");
        Console.WriteLine();
        Console.WriteLine($"GRAND TOTAL: {totalCombinations} combinations");
        Console.WriteLine($"  Team Preview: {teamPreviewCombinations}");
        Console.WriteLine($"  Doubles Battle: {actualDualSlot + singleSlotCombinations}");
    }

    /// <summary>
    /// Generates all possible team preview choices for 4-slot format.
    /// Each player selects 4 different Pokémon from their roster of 6.
    /// </summary>
    /// <returns>String containing all team preview enum entries</returns>
    public static string GenerateTeamPreviewChoiceEnumEntries()
    {
        var sb = new StringBuilder();
        sb.AppendLine("    // =============================================================================");
        sb.AppendLine("    // TEAM PREVIEW CHOICES (4-SLOT FORMAT)");
        sb.AppendLine("    // =============================================================================");
        sb.AppendLine("    // Team Preview combinations: Select 4 different Pokémon from roster of 6");
        sb.AppendLine("    // Pattern: TeamPreviewSlot1Select{A}_Slot2Select{B}_Slot3Select{C}_Slot4Select{D}");
        sb.AppendLine("    // where A, B, C, D are all different values from 1-6");
        sb.AppendLine();

        int teamPreviewCombinations = 0;
        
        // Generate all 4-slot team preview combinations
        for (int slot1 = 1; slot1 <= 6; slot1++)
        {
            for (int slot2 = 1; slot2 <= 6; slot2++)
            {
                if (slot2 == slot1) continue;
                
                for (int slot3 = 1; slot3 <= 6; slot3++)
                {
                    if (slot3 == slot1 || slot3 == slot2) continue;
                    
                    for (int slot4 = 1; slot4 <= 6; slot4++)
                    {
                        if (slot4 == slot1 || slot4 == slot2 || slot4 == slot3) continue;
                        
                        string enumEntry = $"TeamPreviewSlot1Select{slot1}_Slot2Select{slot2}_Slot3Select{slot3}_Slot4Select{slot4}";
                        sb.AppendLine($"    {enumEntry},");
                        teamPreviewCombinations++;
                    }
                }
            }
        }
        
        sb.AppendLine();
        sb.AppendLine($"    // Total Team Preview combinations: {teamPreviewCombinations} (6 × 5 × 4 × 3 = 360)");
        sb.AppendLine();
        
        return sb.ToString();
    }

    /// <summary>
    /// Writes the generated team preview choices to a file for easy copying into Choice.cs.
    /// </summary>
    public static void WriteTeamPreviewChoicesToFile(string filePath = "TeamPreviewChoices.txt")
    {
        string content = GenerateTeamPreviewChoiceEnumEntries();
        File.WriteAllText(filePath, content);
        Console.WriteLine($"Team preview choices written to: {filePath}");
        Console.WriteLine($"Total lines: {content.Split('\n').Length}");
    }

    /// <summary>
    /// Gets statistics about team preview combinations.
    /// </summary>
    public static void PrintTeamPreviewStats()
    {
        Console.WriteLine("=== Team Preview Choice Generation Statistics ===");
        Console.WriteLine("4-slot format: Select 4 Pokémon from roster of 6");
        Console.WriteLine("Order matters, no duplicates allowed");
        Console.WriteLine();
        Console.WriteLine($"Total combinations: 6 × 5 × 4 × 3 = 360");
        Console.WriteLine("Pattern: TeamPreviewSlot1Select{A}_Slot2Select{B}_Slot3Select{C}_Slot4Select{D}");
        Console.WriteLine("Where A, B, C, D are all different values from 1-6");
    }

    /// <summary>
    /// Test method to verify DecodeDoublesChoice works correctly.
    /// This method can be called during development to validate the parsing logic.
    /// </summary>
    public static void TestDecodeDoublesChoice()
    {
        Console.WriteLine("=== Testing DecodeDoublesChoice Method ===");
        
        try
        {
            // Test the problematic case mentioned in the issue
            Choice testChoice = CreateDoublesChoice(Choice.Move1NormalFoe1, Choice.Move1NormalFoe1);
            var (slot1, slot2) = DecodeDoublesChoice(testChoice);
            
            Console.WriteLine($"Test 1 - Dual Move1NormalFoe1:");
            Console.WriteLine($"  Choice: {testChoice}");
            Console.WriteLine($"  Decoded Slot1: {slot1}");
            Console.WriteLine($"  Decoded Slot2: {slot2}");
            Console.WriteLine($"  Success: {slot1 == Choice.Move1NormalFoe1 && slot2 == Choice.Move1NormalFoe1}");
            Console.WriteLine();
            
            // Test single slot cases
            Choice singleSlot1 = CreateDoublesChoice(Choice.Move2NormalFoe2, null);
            var (s1_1, s1_2) = DecodeDoublesChoice(singleSlot1);
            
            Console.WriteLine($"Test 2 - Single Slot1:");
            Console.WriteLine($"  Choice: {singleSlot1}");
            Console.WriteLine($"  Decoded Slot1: {s1_1}");
            Console.WriteLine($"  Decoded Slot2: {s1_2}");
            Console.WriteLine($"  Success: {s1_1 == Choice.Move2NormalFoe2 && s1_2 == null}");
            Console.WriteLine();
            
            Choice singleSlot2 = CreateDoublesChoice(null, Choice.Switch3);
            var (s2_1, s2_2) = DecodeDoublesChoice(singleSlot2);
            
            Console.WriteLine($"Test 3 - Single Slot2:");
            Console.WriteLine($"  Choice: {singleSlot2}");
            Console.WriteLine($"  Decoded Slot1: {s2_1}");
            Console.WriteLine($"  Decoded Slot2: {s2_2}");
            Console.WriteLine($"  Success: {s2_1 == null && s2_2 == Choice.Switch3}");
            Console.WriteLine();
            
            Console.WriteLine("All tests completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Test failed with error: {ex.Message}");
        }
    }
}