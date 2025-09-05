using System;
using System.IO;
using System.Linq;
using System.Text;

// Standalone version of the Choice enum for generation
public enum Choice
{
    // Move 1
    Move1NormalFoe1,
    Move1NormalFoe2,
    Move1NormalAlly,
    Move1AllAdjacentFoes,
    Move1Field,
    Move1AllySide,
    Move1Self,

    Move1NormalFoe1WithTera,
    Move1NormalFoe2WithTera,
    Move1NormalAllyWithTera,
    Move1AllAdjacentFoesWithTera,
    Move1FieldWithTera,
    Move1AllySideWithTera,
    Move1SelfWithTera,

    // Move 2
    Move2NormalFoe1,
    Move2NormalFoe2,
    Move2NormalAlly,
    Move2AllAdjacentFoes,
    Move2Field,
    Move2AllySide,
    Move2Self,

    Move2NormalFoe1WithTera,
    Move2NormalFoe2WithTera,
    Move2NormalAllyWithTera,
    Move2AllAdjacentFoesWithTera,
    Move2FieldWithTera,
    Move2AllySideWithTera,
    Move2SelfWithTera,

    // Move 3
    Move3NormalFoe1,
    Move3NormalFoe2,
    Move3NormalAlly,
    Move3AllAdjacentFoes,
    Move3Field,
    Move3AllySide,
    Move3Self,

    Move3NormalFoe1WithTera,
    Move3NormalFoe2WithTera,
    Move3NormalAllyWithTera,
    Move3AllAdjacentFoesWithTera,
    Move3FieldWithTera,
    Move3AllySideWithTera,
    Move3SelfWithTera,

    // Move 4
    Move4NormalFoe1,
    Move4NormalFoe2,
    Move4NormalAlly,
    Move4AllAdjacentFoes,
    Move4Field,
    Move4AllySide,
    Move4Self,

    Move4NormalFoe1WithTera,
    Move4NormalFoe2WithTera,
    Move4NormalAllyWithTera,
    Move4AllAdjacentFoesWithTera,
    Move4FieldWithTera,
    Move4AllySideWithTera,
    Move4SelfWithTera,

    // Switches
    Switch1,
    Switch2,
    Switch3,
    Switch4,

    // Struggle
    Struggle,

    // Other
    Quit,
    None,
    Invalid,
}

/// <summary>
/// Standalone choice generator for doubles battles including single-slot scenarios
/// </summary>
public static class StandaloneChoiceGenerator
{
    // All single-slot choices
    private static readonly Choice[] SingleMoveChoices = 
    [
        // Move 1
        Choice.Move1NormalFoe1, Choice.Move1NormalFoe2, Choice.Move1NormalAlly, Choice.Move1AllAdjacentFoes,
        Choice.Move1Field, Choice.Move1AllySide, Choice.Move1Self,
        Choice.Move1NormalFoe1WithTera, Choice.Move1NormalFoe2WithTera, Choice.Move1NormalAllyWithTera,
        Choice.Move1AllAdjacentFoesWithTera, Choice.Move1FieldWithTera, Choice.Move1AllySideWithTera, Choice.Move1SelfWithTera,
        
        // Move 2
        Choice.Move2NormalFoe1, Choice.Move2NormalFoe2, Choice.Move2NormalAlly, Choice.Move2AllAdjacentFoes,
        Choice.Move2Field, Choice.Move2AllySide, Choice.Move2Self,
        Choice.Move2NormalFoe1WithTera, Choice.Move2NormalFoe2WithTera, Choice.Move2NormalAllyWithTera,
        Choice.Move2AllAdjacentFoesWithTera, Choice.Move2FieldWithTera, Choice.Move2AllySideWithTera, Choice.Move2SelfWithTera,
        
        // Move 3
        Choice.Move3NormalFoe1, Choice.Move3NormalFoe2, Choice.Move3NormalAlly, Choice.Move3AllAdjacentFoes,
        Choice.Move3Field, Choice.Move3AllySide, Choice.Move3Self,
        Choice.Move3NormalFoe1WithTera, Choice.Move3NormalFoe2WithTera, Choice.Move3NormalAllyWithTera,
        Choice.Move3AllAdjacentFoesWithTera, Choice.Move3FieldWithTera, Choice.Move3AllySideWithTera, Choice.Move3SelfWithTera,
        
        // Move 4
        Choice.Move4NormalFoe1, Choice.Move4NormalFoe2, Choice.Move4NormalAlly, Choice.Move4AllAdjacentFoes,
        Choice.Move4Field, Choice.Move4AllySide, Choice.Move4Self,
        Choice.Move4NormalFoe1WithTera, Choice.Move4NormalFoe2WithTera, Choice.Move4NormalAllyWithTera,
        Choice.Move4AllAdjacentFoesWithTera, Choice.Move4FieldWithTera, Choice.Move4AllySideWithTera, Choice.Move4SelfWithTera,
    ];

    private static readonly Choice[] SingleSwitchChoices = 
    [
        Choice.Switch1, Choice.Switch2, Choice.Switch3, Choice.Switch4
    ];

    private static readonly Choice[] SingleStruggleChoices = 
    [
        Choice.Struggle
    ];

    public static void Main(string[] args)
    {
        Console.WriteLine("=== ApogeeVGC Enhanced Doubles Choice Generator ===");
        Console.WriteLine("Now supporting single-slot scenarios (last Pokémon situations)");
        Console.WriteLine();

        try
        {
            // Print statistics
            PrintCombinationStats();
            Console.WriteLine();

            // Generate combinations
            Console.WriteLine("Generating enhanced doubles choice combinations...");
            string enumEntries = GenerateDoublesChoiceEnumEntries();
            
            // Write to file
            string outputPath = Path.Combine(Environment.CurrentDirectory, "EnhancedDoublesChoices.txt");
            File.WriteAllText(outputPath, enumEntries);
            
            Console.WriteLine($"Generation complete! Output written to: {outputPath}");
            Console.WriteLine();
            
            // Show preview
            Console.WriteLine("=== Preview (first 20 entries) ===");
            string[] lines = enumEntries.Split('\n');
            for (int i = 0; i < Math.Min(20, lines.Length); i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i]))
                    Console.WriteLine(lines[i]);
            }
            
            if (lines.Length > 20)
            {
                Console.WriteLine($"... and {lines.Length - 20} more lines");
            }

            Console.WriteLine();
            Console.WriteLine("Key Features:");
            Console.WriteLine("? Dual-slot combinations (both Pokémon active)");
            Console.WriteLine("? Single-slot combinations (last Pokémon scenarios)");
            Console.WriteLine("? Proper validation and filtering");
            Console.WriteLine("? Support for empty slot detection");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static string GenerateDoublesChoiceEnumEntries()
    {
        var sb = new StringBuilder();
        sb.AppendLine("    // =============================================================================");
        sb.AppendLine("    // ENHANCED DOUBLES BATTLE COMBINATIONS (AUTO-GENERATED)");
        sb.AppendLine("    // =============================================================================");
        sb.AppendLine();

        var allSingleChoices = SingleMoveChoices
            .Concat(SingleSwitchChoices)
            .Concat(SingleStruggleChoices)
            .ToArray();

        int totalCombinations = 0;

        // Generate dual-slot combinations
        sb.AppendLine("    // Dual-slot combinations (both slots active)");
        foreach (var slot1Choice in allSingleChoices)
        {
            foreach (var slot2Choice in allSingleChoices)
            {
                if (IsInvalidCombination(slot1Choice, slot2Choice))
                    continue;

                string enumEntry = $"Slot1{slot1Choice}_Slot2{slot2Choice}";
                sb.AppendLine($"    {enumEntry},");
                totalCombinations++;
            }
        }

        sb.AppendLine();
        sb.AppendLine("    // Single-slot combinations (one slot empty)");
        
        // Generate single-slot combinations: Slot1 only
        sb.AppendLine("    // Slot1 active, Slot2 empty (last Pokémon in slot 1)");
        foreach (var slot1Choice in allSingleChoices)
        {
            string enumEntry = $"Slot1{slot1Choice}_Slot2Empty";
            sb.AppendLine($"    {enumEntry},");
            totalCombinations++;
        }

        sb.AppendLine();
        // Generate single-slot combinations: Slot2 only
        sb.AppendLine("    // Slot1 empty, Slot2 active (last Pokémon in slot 2)");
        foreach (var slot2Choice in allSingleChoices)
        {
            string enumEntry = $"Slot1Empty_Slot2{slot2Choice}";
            sb.AppendLine($"    {enumEntry},");
            totalCombinations++;
        }

        sb.AppendLine();
        sb.AppendLine($"    // Total enhanced combinations generated: {totalCombinations}");
        
        return sb.ToString();
    }

    private static bool IsInvalidCombination(Choice slot1Choice, Choice slot2Choice)
    {
        // Can't switch the same Pokemon in both slots
        if (IsSwitchChoice(slot1Choice) && IsSwitchChoice(slot2Choice))
        {
            return slot1Choice == slot2Choice;
        }
        return false;
    }

    private static bool IsSwitchChoice(Choice choice)
    {
        return SingleSwitchChoices.Contains(choice);
    }

    private static void PrintCombinationStats()
    {
        int moveCount = SingleMoveChoices.Length;
        int switchCount = SingleSwitchChoices.Length;
        int struggleCount = SingleStruggleChoices.Length;
        int totalSingleChoices = moveCount + switchCount + struggleCount;

        Console.WriteLine("=== Enhanced Generation Statistics ===");
        Console.WriteLine($"Single move choices: {moveCount}");
        Console.WriteLine($"Single switch choices: {switchCount}");
        Console.WriteLine($"Single struggle choices: {struggleCount}");
        Console.WriteLine($"Total single choices: {totalSingleChoices}");
        Console.WriteLine();
        
        // Calculate combinations
        var allSingleChoices = SingleMoveChoices
            .Concat(SingleSwitchChoices)
            .Concat(SingleStruggleChoices)
            .ToArray();

        int theoreticalDualSlot = totalSingleChoices * totalSingleChoices;
        int actualDualSlot = 0;

        foreach (var slot1Choice in allSingleChoices)
        {
            foreach (var slot2Choice in allSingleChoices)
            {
                if (!IsInvalidCombination(slot1Choice, slot2Choice))
                    actualDualSlot++;
            }
        }

        int singleSlotCombinations = totalSingleChoices * 2; // Slot1Empty + Slot2Empty scenarios
        int totalCombinations = actualDualSlot + singleSlotCombinations;
        
        Console.WriteLine($"Dual-slot combinations: {actualDualSlot}");
        Console.WriteLine($"Single-slot combinations: {singleSlotCombinations} ({totalSingleChoices} × 2 slots)");
        Console.WriteLine($"Total combinations: {totalCombinations}");
        Console.WriteLine($"Invalid dual-slot combinations filtered: {theoreticalDualSlot - actualDualSlot}");
        Console.WriteLine();
        Console.WriteLine("Use Cases:");
        Console.WriteLine($"- Normal doubles battles: {actualDualSlot} combinations");
        Console.WriteLine($"- Last Pokémon scenarios: {singleSlotCombinations} combinations");
    }
}