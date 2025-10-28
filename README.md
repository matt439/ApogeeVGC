# ApogeeVGC - Pokémon VGC Battle Simulator

## Project Overview

ApogeeVGC is a **Pokémon Video Game Championship (VGC) battle simulator** built in C# (.NET 9). This project simulates Pokémon battles following official VGC Double Battle mechanics, including moves, abilities, status conditions, weather, terrain, and complex battle interactions. It is are conversion from Pokemon Showdown (written in TypeScript) and uses OOP.

## Pokemon Showdown Reference
The project references Pokémon Showdown's TypeScript code for mechanics and data structures. Key files include:
- `RUNEVENT-TS.md` - Event system documentation
- `SINGLEEVENT-TS.md` - Single event handling documentation
- `SIM-PROTOCOL.md` - Simulation protocol overview
- `SIMULATOR.md` - Core simulator mechanics

## Technology Stack

- **Framework**: .NET 9.0
- **Language**: C# (preview language features enabled)
- **Pattern**: Asynchronous battle simulation with event-driven architecture
- **Format**: VGC Doubles (2v2 Pokémon battles)

## Project Structure

```
ApogeeVGC/
├── Sim/       # Battle simulation engine
│   ├── Abilities/      # Ability implementations
│   ├── BattleClasses/           # Battle state and orchestration
│   ├── Core/      # Core driver and battle loop
│   ├── FieldClasses/# Field conditions (weather, terrain)
│   ├── PokemonClasses/     # Pokemon entity and behavior
│   ├── SideClasses/             # Team/side state and conditions
│   └── Utils/       # Shared utilities and extensions
│       ├── Extensions/          # Extension methods (ShowdownIdTools, etc.)
│       ├── Unions/       # Discriminated unions for type safety
│       └── State.*.cs     # State management utilities
├── Data/          # Static game data (abilities, moves, species)
├── ShowDownTSRef.cs             # Reference TypeScript from Pokémon Showdown
└── Program.cs # Entry point
```

## Key Concepts

### Battle Simulation
- **Doubles Format**: All battles are 2v2 (two Pokémon per side)
- **Asynchronous**: Battle actions are processed asynchronously with delegates
- **Event-Driven**: Abilities and effects trigger based on battle events
- **State Management**: Immutable state snapshots for battle history/replay

### Core Classes
- **`Battle`/`BattleAsync`**: Orchestrates battle turns and action resolution
- **`Pokemon`**: Individual Pokémon with stats, moves, abilities, and status
- **`Side`**: Team state including active Pokémon and side conditions
- **`Field`**: Global battle conditions (weather, terrain, trick room)
- **`Ability`**: Ability effects that trigger on battle events
- **`Driver`**: Main simulation loop and battle initialization

### Design Patterns
- **Partial Classes**: Large classes like `Pokemon`, `Side`, and `Battle` are split into logical files (`.Core.cs`, `.Conditions.cs`, `.Transformation.cs`, etc.)
- **Union Types**: `SpecificUnion` and similar types provide discriminated unions for type-safe battle entities
- **Showdown Compatibility**: Uses Showdown IDs for Pokémon, moves, and abilities

## Domain Knowledge

This is a **competitive Pokémon** simulator focusing on:
- Official VGC rulesets and mechanics
- Complex ability interactions (e.g., Intimidate, Weather Ball)
- Status conditions (paralysis, burn, freeze, sleep, poison)
- Field effects (weather, terrain, trick room, tailwind)
- Type effectiveness and damage calculation
- Speed calculation and turn order
- Team preview and battle switching

## Technologies & Libraries
- Uses modern C# features: records, pattern matching, nullable reference types
- Async/await for battle simulation
- Extension methods for clean syntax
- Strong typing with custom union types
