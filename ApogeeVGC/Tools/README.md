# Species Data Validation Tools

This directory contains tools to validate and inspect the species data conversion from TypeScript (pokemon-showdown) to C#.

## Available Tools

### 1. Validate Species Data
Performs a comprehensive validation comparing the C# species data against the TypeScript source.

**Usage:**
```bash
cd ApogeeVGC
dotnet run --validate-species
```

**Output:**
- Total species count comparison
- List of missing species (if any)
- List of extra species (if any)
- Validation of species ordering by Pokédex number
- Distribution of species across number ranges

### 2. Species Data Report
Generates a detailed categorized report of any differences between TypeScript and C# data.

**Usage:**
```bash
cd ApogeeVGC
dotnet run --species-report
```

**Output:**
- Categorized missing species:
  - CAP (Create-A-Pokémon) entries
  - PokéStar Studios actors
  - Gigantamax forms
  - Cosmetic variations
  - Other entries
- Recommendations for each category
- Summary statistics

### 3. Species Search
Interactive search tool to find specific species and view their details.

**Usage:**
```bash
cd ApogeeVGC
dotnet run --search-species <term1> <term2> ...
```

**Examples:**
```bash
# Search for a single species
dotnet run --search-species pikachu

# Search for multiple species
dotnet run --search-species pikachu charizard mewtwo

# Search by partial name
dotnet run --search-species "mega"
```

**Output:**
- Pokédex number
- Full species name
- Forme (if applicable)
- Types
- Abilities (including hidden ability)

## Tool Files

- **ValidateSpeciesData.cs** - Core validation logic
- **SpeciesDataReport.cs** - Detailed categorized reporting
- **SpeciesSearch.cs** - Interactive species search

## Understanding the Results

### Species Ordering
All species are validated to ensure they are in correct order by their `num` field (Pokédex number). This ensures consistency with the official Pokédex numbering.

### Missing Species Categories

#### ? CAP Pokémon (Typically Excluded)
Fan-created Pokémon from the Smogon Create-A-Pokémon project. These are not official Game Freak Pokémon and are usually excluded from main implementations.

#### ? PokéStar Studios (Typically Excluded)
Special actors from Black 2/White 2's PokéStar Studios minigame. These are not real Pokémon and are usually excluded unless specifically implementing that feature.

#### ? Gigantamax Forms (Consider Adding)
Official Gigantamax forms from Sword/Shield. If you're supporting Gen 8 mechanics, you may want to add these.

#### ? Cosmetic Variations (Optional)
Purely cosmetic variations that don't affect gameplay:
- Vivillon wing patterns
- Alcremie cream/swirl combinations
- Deerling/Sawsbuck seasonal forms
- Minior core colors
- Other aesthetic variations

These are optional and depend on whether you need complete forme coverage.

### Special Characters
The tools account for special characters in Pokémon names:
- `Farfetch'd` vs `farfetchd`
- `Flabébé` vs `flabebe`
- `Type: Null` vs `typenull`

These differences are normal and don't indicate missing data.

## Validation Report

A comprehensive validation report is available in the root directory:
- **SPECIES_VALIDATION_REPORT.md** - Complete analysis and recommendations

## Adding the Tools to Your Workflow

The validation tools are integrated into the main Program.cs and can be run as command-line arguments:

```csharp
// In Program.cs
if (args.Length > 0 && args[0] == "--validate-species")
{
    ValidateSpeciesData.Run();
    return;
}
```

This allows you to run validation without interfering with normal application startup.

## Continuous Validation

Consider running these tools:
- After updating species data
- Before major releases
- When syncing with new pokemon-showdown updates
- As part of your CI/CD pipeline

---
*Tools developed for ApogeeVGC species data validation*
