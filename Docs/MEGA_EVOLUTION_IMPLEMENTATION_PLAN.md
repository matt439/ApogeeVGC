# Mega Evolution Implementation Plan

## Overview

Add Mega Evolution support to ApogeeVGC. The existing Terastallization implementation serves as the primary template — both are once-per-side battle mechanics that trigger before the move, change the Pokemon's forme, and fire an `AfterX` event.

## Current State

### Already Exists
- **Event system**: `EventId.AfterMega`, `OnAfterMegaEventInfo`, `OnAnyAfterMegaEventInfo` — fully wired into the event handler mapper
- **FormeId enum**: `Mega`, `MegaX`, `MegaY` entries
- **SpecieId enum**: ~75 mega species entries (e.g., `VenusaurMega`, `CharizardMegaX`)
- **Species data**: Mega forme entries with stats, types, and abilities already defined in `Data/SpeciesData/`
- **Utility**: `FormeIdTools.IsMegaForme()` extension method
- **Item event handlers**: 3 items already respond to `OnAnyAfterMega` (Eject Button, Mirror Herb, White Herb)
- **BattleQueue comment**: Line 199 explicitly marks where mega evolution should be inserted

### Missing
- Pokemon properties (`CanMegaEvo`)
- Item `MegaStone` property and ~48 mega stone item definitions
- Choice/action plumbing for the mega option
- `BattleActions.MegaEvolve()` method
- Battle lifecycle dispatch
- Move request exposure
- Format rules to enable/disable mega

---

## Implementation Steps

### Phase 1: Core Data Model

#### 1.1 Add `MegaStone` property to `Item.Core.cs`

**File:** `Sim/Items/Item.Core.cs`

Add a property mapping base species to mega forme species:

```csharp
/// <summary>
/// Maps base species name to mega evolution species ID.
/// e.g., { "Venusaur": SpecieId.VenusaurMega }
/// </summary>
public IReadOnlyDictionary<SpecieId, SpecieId>? MegaStone { get; init; }
```

Also update `Fling` to return `BasePower = 80` for mega stones (matching Showdown):

```csharp
if (MegaStone != null)
{
    return new FlingData { BasePower = 80 };
}
```

#### 1.2 Add `OnTakeItem` behavior for mega stones

Mega stones cannot be removed from the Pokemon that can mega evolve with them. This is handled via the existing `OnTakeItem` event on each mega stone item definition. Alternatively, a shared handler can be defined once and reused.

#### 1.3 Add `CanMegaEvo` property to `Pokemon.Core.cs`

**File:** `Sim/PokemonClasses/Pokemon.Core.cs`

Add near the existing Terastallization properties (~line 176):

```csharp
// Mega Evolution
public SpecieId? CanMegaEvo { get; set; }
```

This stores the target mega forme species ID, or `null` if the Pokemon cannot mega evolve. Unlike Showdown's string-based approach, use the strongly-typed `SpecieId`.

#### 1.4 Add `CanMegaEvo` to `PokemonPerspective.cs`

**File:** `Sim/PokemonClasses/PokemonPerspective.cs`

```csharp
public SpecieId? CanMegaEvo { get; init; }
```

#### 1.5 Add `CanMegaEvo` to Pokemon clone method

**File:** `Sim/PokemonClasses/Pokemon.Core.cs` (~line 515 area, near `CanTerastallize` clone)

```csharp
CanMegaEvo = source.CanMegaEvo;
```

---

### Phase 2: Choice & Action Plumbing

#### 2.1 Add `MegaEvo` to EventType enum

**Files:** `Sim/Events/EventType.cs` and `Sim/SideClasses/EventType.cs`

```csharp
public enum EventType
{
    None,
    Terastallize,
    MegaEvo,
}
```

#### 2.2 Add `Mega` flag to `Choice.cs`

**File:** `Sim/Choices/Choice.cs`

```csharp
public bool Mega { get; set; }
```

#### 2.3 Add `Mega` property to `ChosenAction.cs`

**File:** `Sim/Choices/ChosenAction.cs`

```csharp
public SpecieId? Mega { get; init; }
```

#### 2.4 Add `Mega` property to `MoveAction.cs`

**File:** `Sim/Actions/MoveAction.cs`

```csharp
public SpecieId? Mega { get; init; }
```

#### 2.5 Add `CanMegaEvo` to `PokemonMoveRequestData.cs`

**File:** `Sim/Choices/PokemonMoveRequestData.cs`

```csharp
public SpecieId? CanMegaEvo { get; set; }
```

#### 2.6 Add `MegaEvo` to `ActionId` enum

**File:** `Sim/Actions/ActionId.cs`

```csharp
// PokemonAction
Shift,
RunSwitch,
Event,
Terastallize,
MegaEvo,
```

#### 2.7 Allow `MegaEvo` in `PokemonAction.cs`

**File:** `Sim/Actions/PokemonAction.cs`

Update the validation:

```csharp
if (value is ActionId.Shift or ActionId.RunSwitch or ActionId.Event
    or ActionId.Terastallize or ActionId.MegaEvo)
```

---

### Phase 3: Battle Engine Integration

#### 3.1 Create `BattleActions.MegaEvolution.cs`

**File:** `Sim/BattleClasses/BattleActions.MegaEvolution.cs` (new file)

Two methods, following the Showdown pattern from `battle-actions.ts:1863-1917`:

**`CanMegaEvo(Pokemon pokemon)`** — called during Pokemon initialization and switch-in to determine if a Pokemon can mega evolve:
1. Get the Pokemon's held item
2. If item has no `MegaStone`, return `null`
3. Check if the mega stone maps from this Pokemon's base species to a mega forme
4. Return the target `SpecieId` or `null`
5. Special case: Rayquaza can mega evolve without a mega stone if it knows Dragon Ascent and holds no Z-crystal (not relevant since Z-moves are not implemented, but worth noting)

**`RunMegaEvo(Pokemon pokemon)`** — executes the mega evolution:
1. Get `pokemon.CanMegaEvo`; if null, return false
2. Call `pokemon.FormeChange(megaSpecieId, pokemon.GetItem(), isPermanent: true)`
3. Disable mega evolution for all allies on the same side:
   ```csharp
   foreach (Pokemon ally in pokemon.Side.Pokemon)
   {
       ally.CanMegaEvo = null;
   }
   ```
4. Fire `Battle.RunEvent(EventId.AfterMega, pokemon)`
5. Return true

#### 3.2 Add mega evolution to BattleQueue

**File:** `Sim/BattleClasses/BattleQueue.cs` (~line 199)

Replace the comment `// Note: Mega Evolution and Dynamax are deliberately excluded as per requirements` with:

```csharp
// Add MegaEvo action if the player chose to mega evolve
if (ma.Mega.HasValue &&
    ma.Pokemon is { CanMegaEvo: not null })
{
    actions.InsertRange(0, ResolveAction(new PokemonAction
    {
        Choice = ActionId.MegaEvo,
        Pokemon = ma.Pokemon,
    }));
}
```

**Important ordering:** In Showdown, mega evolution resolves before Terastallization. The `InsertRange(0, ...)` calls mean the *last* inserted action runs *first*. So mega evolution should be inserted *after* Terastallization in the code (so it appears earlier in the queue). Verify this matches the Showdown turn order.

#### 3.3 Add dispatch in `Battle.Lifecycle.cs`

**File:** `Sim/BattleClasses/Battle.Lifecycle.cs` (~line 551, after the Terastallize case)

```csharp
case ActionId.MegaEvo:
{
    var megaAction = (PokemonAction)action;
    Actions.RunMegaEvo(megaAction.Pokemon);
    break;
}
```

#### 3.4 Expose `CanMegaEvo` in move requests

**File:** `Sim/PokemonClasses/Pokemon.Requests.cs` (~line 218)

After the Terastallization block, add:

```csharp
// Handle Mega Evolution
if (lockedMove == null)
{
    if (CanMegaEvo is not null)
    {
        data = data with { CanMegaEvo = CanMegaEvo };
    }
}
```

#### 3.5 Initialize `CanMegaEvo` during Pokemon setup

**File:** `Sim/PokemonClasses/Pokemon.Core.cs` (~line 393, after `CanTerastallize = TeraType`)

```csharp
CanMegaEvo = Battle.Actions.CanMegaEvo(this);
```

Also initialize on switch-in if needed (check if Showdown recalculates `canMegaEvo` on switch-in — it does, so this should also be set in the switch-in path).

---

### Phase 4: Choice Validation

#### 4.1 Add mega evolution validation to `Side.Choices.cs`

**File:** `Sim/SideClasses/Side.Choices.cs` (~line 224, alongside Terastallization validation)

Add a parallel block for mega:

```csharp
// Step 11b: Mega Evolution
bool mega = eventType == EventType.MegaEvo;

if (mega && request.CanMegaEvo is null)
{
    return EmitChoiceError($"Can't move: {pokemon.Name} can't Mega Evolve.");
}

if (mega && Choice.Mega)
{
    return EmitChoiceError("Can't move: You can only Mega Evolve once per battle.");
}
```

Update the `ChosenAction` creation (~line 243) to include `Mega`:

```csharp
Mega = mega ? pokemon.CanMegaEvo : null,
```

Update the choice flags (~line 264):

```csharp
if (mega)
{
    Choice.Mega = true;
}
```

#### 4.2 Add mega to `ProcessChosenMoveAction`

**File:** `Sim/SideClasses/Side.Choices.cs` (~line 720)

Update to detect mega in addition to tera:

```csharp
private bool ProcessChosenMoveAction(ChosenAction action)
{
    EventType eventType = action.Terastallize != null
        ? EventType.Terastallize
        : action.Mega != null
            ? EventType.MegaEvo
            : EventType.None;

    return ChooseMove(
        moveText: action.MoveId,
        targetLoc: action.TargetLoc ?? 0,
        eventType: eventType
    );
}
```

#### 4.3 Propagate `Mega` from `ChosenAction` to `MoveAction`

Find where `ChosenAction` is converted to `MoveAction` in the battle queue resolution and ensure the `Mega` property is carried over.

---

### Phase 5: Mega Stone Item Data

#### 5.1 Add mega stone `ItemId` entries

**File:** `Sim/Items/ItemId.cs`

Add ~48 mega stone item IDs. Examples:
```csharp
Venusaurite,
CharizarditeX,
CharizarditeY,
BlastoisiniteRite,
Abomasite,
Absolite,
// ... etc
```

Reference: `pokemon-showdown/data/items.ts` for the complete list.

#### 5.2 Create mega stone item definitions

**File:** `Data/Items/` — add to appropriate alphabetical files (ItemsABC.cs, ItemsDEF.cs, etc.)

Each mega stone follows this pattern:

```csharp
[ItemId.Venusaurite] = new()
{
    Id = ItemId.Venusaurite,
    Name = "Venusaurite",
    Num = 659,
    Gen = 6,
    MegaStone = new Dictionary<SpecieId, SpecieId>
    {
        [SpecieId.Venusaur] = SpecieId.VenusaurMega,
    }.AsReadOnly(),
    OnTakeItem = OnTakeItemEventInfo.Create((battle, item, pokemon) =>
    {
        return item.MegaStone?.ContainsKey(pokemon.BaseSpecies.BaseSpecies) != true;
    }),
},
```

For X/Y variants (Charizard, Mewtwo):
```csharp
[ItemId.CharizarditeX] = new()
{
    Id = ItemId.CharizarditeX,
    Name = "Charizardite X",
    MegaStone = new Dictionary<SpecieId, SpecieId>
    {
        [SpecieId.Charizard] = SpecieId.CharizardMegaX,
    }.AsReadOnly(),
    // ...
},
```

There are approximately 48 mega stones to define. The data can be transcribed from `pokemon-showdown/data/items.ts`.

#### 5.3 Update species data

All 75 mega species already have entries in `Data/SpeciesData/` with correct stats, types, abilities, `BaseSpecies`, and `Forme`. However, they are **missing properties required for mega evolution to function at runtime**. Two categories of changes are needed:

##### 5.3.1 Mega species entries — add `RequiredItem`

Every mega species entry (except Rayquaza-Mega) needs a `RequiredItem` pointing to its mega stone. This is used by team validation to enforce that the correct item is held, and referenced by `CanMegaEvo()` for the Rayquaza special case check.

**Currently (VenusaurMega as example):**
```csharp
[SpecieId.VenusaurMega] = new()
{
    Id = SpecieId.VenusaurMega,
    Num = 3,
    Name = "Venusaur-Mega",
    BaseSpecies = SpecieId.Venusaur,
    Forme = FormeId.Mega,
    Types = [PokemonType.Grass, PokemonType.Poison],
    // ... stats, abilities, height, weight, color ...
},
```

**Needs to become:**
```csharp
[SpecieId.VenusaurMega] = new()
{
    Id = SpecieId.VenusaurMega,
    Num = 3,
    Name = "Venusaur-Mega",
    BaseSpecies = SpecieId.Venusaur,
    Forme = FormeId.Mega,
    Types = [PokemonType.Grass, PokemonType.Poison],
    // ... stats, abilities, height, weight, color ...
    RequiredItem = ItemId.Venusaurite,           // <-- ADD
},
```

**Scope:** 74 mega species entries need `RequiredItem` added. The one exception is Rayquaza-Mega which uses `RequiredMove` instead.

**Note:** Megas do NOT need `BattleOnly`. In Showdown, `battleOnly` is for formes that revert mid-battle when a condition ends (e.g., Darmanitan-Zen, Castform weather forms). Mega evolution is permanent for the battle — it does not revert on switch-out or fainting.

##### 5.3.2 Rayquaza-Mega — add `RequiredMove`

Rayquaza-Mega is the only mega that evolves via a move instead of an item:

```csharp
[SpecieId.RayquazaMega] = new()
{
    // ... existing fields ...
    RequiredMove = MoveId.DragonAscent,          // <-- ADD
},
```

##### 5.3.3 Base species entries — add `OtherFormes`

Base species that can mega evolve should have an `OtherFormes` entry pointing to their mega forme(s). This is used by `CanMegaEvo()` for the Rayquaza special case (checking if the first other forme is a mega with a required move).

**Current state:** Only 16 of ~75 base species have `OtherFormes` set (all in the 301-400 Dex range). The remaining ~59 base species are missing it entirely.

**Example — Venusaur currently has no `OtherFormes`, needs:**
```csharp
[SpecieId.Venusaur] = new()
{
    // ... existing fields ...
    OtherFormes = [FormeId.Mega],                // <-- ADD
},
```

**For X/Y species like Charizard:**
```csharp
OtherFormes = [FormeId.MegaX, FormeId.MegaY],
```

**Scope:** ~59 base species entries need `OtherFormes` added. The 16 already present (in `SpeciesData0301to0350.cs` and `SpeciesData0351to0400.cs`) should be verified for correctness.

##### 5.3.4 Complete list of species data changes by file

| SpeciesData File | Mega Species Needing `RequiredItem` | Base Species Needing `OtherFormes` |
|---|---|---|
| `0001to0050.cs` | VenusaurMega, CharizardMegaX, CharizardMegaY, BlastoiseMega, BeedrillMega, PidgeotMega, ClefableMega | Venusaur, Charizard, Blastoise, Beedrill, Pidgeot, Clefable |
| `0051to0100.cs` | AlakazamMega, VictreebelMega, SlowbroMega, GengarMega | Alakazam, Victreebel, Slowbro, Gengar |
| `0101to0150.cs` | KangaskhanMega, StarmieMega, PinsirMega, GyaradosMega, AerodactylMega, DragoniteMega, MewtwoMegaX, MewtwoMegaY | Kangaskhan, Starmie, Pinsir, Gyarados, Aerodactyl, Dragonite, Mewtwo |
| `0151to0200.cs` | MeganiummMega, FeraligatrMega, AmpharosMega | Meganium, Feraligatr, Ampharos |
| `0201to0250.cs` | SteelixMega, ScizorMega, HeracrossMega, SkarmoryMega, HoundoomMega, TyranitarMega | Steelix, Scizor, Heracross, Skarmory, Houndoom, Tyranitar |
| `0251to0300.cs` | SceptileMega, BlazikenMega, SwampertMega, GardevoirMega | Sceptile, Blaziken, Swampert, Gardevoir |
| `0301to0350.cs` | SableyeMega, MawileMega, AggronMega, MedichamMega, ManectricMega, SharpedomMega, CameruptMega, AltariaMega | (8 base species here already have `OtherFormes` — verify) |
| `0351to0400.cs` | BanetteMega, AbsolMega, GlalieMega, SalamenceMega, MetagrossMega, LatiasMega, LatiosMega, RayquazaMega (use `RequiredMove` instead) | (8 base species here already have `OtherFormes` — verify) |
| `0401to0450.cs` | LopunnyMega, GarchompMega, LucarioMega | Lopunny, Garchomp, Lucario |
| `0451to0500.cs` | AbomasnowMega, GalladeMega, FroslassMega | Abomasnow, Gallade, Froslass |
| `0501to0550.cs` | EmboarMega, ExcadrillMega, AudinoMega, ScolipedeMega | Emboar, Excadrill, Audino, Scolipede |
| `0551to0600.cs` | ScraftyMega | Scrafty |
| `0601to0650.cs` | EelektrossMega, ChandelureMega | Eelektross, Chandelure |
| `0651to0700.cs` | ChesnaughtMega, DelphoxMega, GreninjaMega, PyroarMega, FloetteMega, MalamarMega, BarbaracleMega, DragalgeMega | Chesnaught, Delphox, Greninja, Pyroar, Floette, Malamar, Barbaracle, Dragalge |
| `0701to0750.cs` | HawluchaMega, ZygardeMega, DiancieMega | Hawlucha, Zygarde, Diancie |
| `0751to0800.cs` | DrampaMega | Drampa |
| `0851to0900.cs` | FalinksMega | Falinks |

**Total edits:** ~74 mega species entries + ~75 base species entries = ~149 species data edits across 17 files.

---

### Phase 6: Format & Ruleset Support

#### 6.1 Add mega evolution rules

**File:** `Data/Rulesets.cs`

Consider adding:
- A `MegaEvolutionClause` rule (limits one mega per side — this is inherent in the mechanic, but a rule makes it explicit and ban-able)
- An option to ban all mega evolution for formats that don't support it

#### 6.2 Update formats that allow mega

**File:** `Data/Formats.cs`

Mega evolution was legal in Gen 6 and Gen 7 VGC. For Gen 9, it is not standard. Decide which formats should allow mega:
- Existing Gen 9 VGC regulations (A-I): **Do not allow mega** (keep as-is)
- New custom/NatDex formats: Allow mega
- A new format like `Gen9NatDex` or `Gen9MegaAllowed` could be added for testing

#### 6.3 Guard mega evolution behind format check

In `BattleActions.CanMegaEvo()`, check that the format allows mega evolution. This could be done via a format flag or rule table check:

```csharp
if (battle.RuleTable.Has("megarayquazaclause")) { ... }
// or
if (!battle.Format.AllowMega) return null;
```

---

### Phase 7: Edge Cases & Polish

#### 7.1 Mega evolution + item removal interactions

- Mega stones cannot be removed by Knock Off, Trick, Switcheroo, Thief, etc. from a Pokemon that can mega evolve with that stone
- This is handled by the `OnTakeItem` handler on each mega stone item
- Verify the item system respects `OnTakeItem` returning `false` to prevent removal

#### 7.2 Mega Rayquaza special case

Rayquaza mega evolves via Dragon Ascent (a move), not a held item. It requires:
- Check if species is Rayquaza and it knows Dragon Ascent
- No mega stone needed — `CanMegaEvo` is set based on move knowledge
- Rayquaza can hold any item while mega evolving
- Consider whether to implement this (it's banned in most formats anyway)

#### 7.3 Mega evolution reversion

Mega evolution is permanent for the duration of the battle — megas do NOT revert on switch-out (unlike Dynamax). However, verify that `FormeRegression` is *not* triggered for mega formes in the fainting/switch-out code in `Battle.Fainting.cs`.

#### 7.4 Transform + Mega

A transformed Pokemon cannot mega evolve. A Pokemon that has mega evolved and then gets Transformed into should copy the mega's stats. Verify `TransformInto()` handles this correctly (it should, since it copies species/stats generically).

#### 7.5 Illusion + Mega

When a Pokemon mega evolves, Illusion should break (the visual change reveals the true identity). Check if `FormeChange()` with `isPermanent: true` already handles this.

#### 7.6 Abilities triggered by mega evolution

Some abilities activate on mega evolution. The existing `OnAfterMega` / `OnAnyAfterMega` event handlers already support this. Verify that the Ability data files correctly define these handlers where needed. The following abilities have interactions:
- **Trace** — re-triggers on mega evolution of the target
- Any ability on the mega forme itself activates on entry (like Intimidate on Mega Salamence)

#### 7.7 MCTS / AI integration

**File:** `Mcts/StateEncoder.cs`, `Mcts/ActionMapper.cs`

The MCTS AI needs to know about the mega evolution option:
- `ActionMapper` should expose mega evolution as a possible action
- `StateEncoder` should encode whether mega evolution is available and whether it's been used

---

## File Change Summary

| File | Change Type | Description |
|------|-------------|-------------|
| `Sim/Items/Item.Core.cs` | Edit | Add `MegaStone` property |
| `Sim/Items/ItemId.cs` | Edit | Add ~48 mega stone item IDs |
| `Sim/PokemonClasses/Pokemon.Core.cs` | Edit | Add `CanMegaEvo` property + initialization + clone |
| `Sim/PokemonClasses/PokemonPerspective.cs` | Edit | Add `CanMegaEvo` property |
| `Sim/PokemonClasses/Pokemon.Requests.cs` | Edit | Expose `CanMegaEvo` in move request |
| `Sim/Choices/Choice.cs` | Edit | Add `Mega` flag |
| `Sim/Choices/ChosenAction.cs` | Edit | Add `Mega` property |
| `Sim/Choices/PokemonMoveRequestData.cs` | Edit | Add `CanMegaEvo` property |
| `Sim/Actions/ActionId.cs` | Edit | Add `MegaEvo` entry |
| `Sim/Actions/MoveAction.cs` | Edit | Add `Mega` property |
| `Sim/Actions/PokemonAction.cs` | Edit | Allow `ActionId.MegaEvo` |
| `Sim/Events/EventType.cs` | Edit | Add `MegaEvo` entry |
| `Sim/SideClasses/EventType.cs` | Edit | Add `MegaEvo` entry |
| `Sim/SideClasses/Side.Choices.cs` | Edit | Add mega validation logic |
| `Sim/BattleClasses/BattleActions.MegaEvolution.cs` | **New** | `CanMegaEvo()` and `RunMegaEvo()` |
| `Sim/BattleClasses/BattleQueue.cs` | Edit | Insert mega evolution action before move |
| `Sim/BattleClasses/Battle.Lifecycle.cs` | Edit | Add `MegaEvo` case to action dispatch |
| `Data/Items/ItemsABC.cs` through `ItemsVWX.cs` | Edit | Add ~48 mega stone item definitions |
| `Data/SpeciesData/` (17 files) | Edit | Add `RequiredItem` to ~74 mega species, `RequiredMove` to Rayquaza-Mega, `OtherFormes` to ~59 base species |
| `Data/Formats.cs` | Edit | Add format(s) that allow mega |
| `Data/Rulesets.cs` | Edit | Add mega-related rules |
| `Mcts/ActionMapper.cs` | Edit | Expose mega as AI action |
| `Mcts/StateEncoder.cs` | Edit | Encode mega availability |

## Estimated Scope

- **Core mechanic** (Phases 1-4): ~15 files edited, 1 new file, ~200-300 lines of new logic
- **Item data** (Phase 5.1-5.2): ~48 item definitions, ~500-600 lines of data
- **Species data** (Phase 5.3): ~149 species edits across 17 files — add `RequiredItem` to 74 mega species, `RequiredMove` to Rayquaza-Mega, `OtherFormes` to ~59 base species
- **Format/rules** (Phase 6): ~20-30 lines
- **Edge cases** (Phase 7): ~50-100 lines of fixes/guards
- **AI integration** (Phase 7.7): ~30-50 lines

The Terastallization implementation is a near-exact structural analog. Most changes follow the same pattern — add a property, thread it through choice/action/queue/dispatch, and fire the event.
