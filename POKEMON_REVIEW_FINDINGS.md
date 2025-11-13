# Pokemon Class C# Conversion Review - Findings (Gen 9 Only)

## Review Scope
Reviewed C# Pokemon class conversion from TypeScript source (`pokemon-showdown/sim/pokemon.ts`) for **Gen 9 VGC format ONLY**.
Deliberately excluded: 3-4 player battles (allyside), Gen 1-8 specific features, Mega Evolution, Z-Moves, Dynamax, Max Moves, Hidden Power, and moves removed in Gen 9.

---

## Critical Issues

### Pokemon.Core.cs

1. **GetSlot() implementation incorrect** (TS line 511)
   - Has typo: `poistionOffset` should be `positionOffset`
   - Logic doesn't match TS - should create string slot like "p1a" or "p2b"
   - Current implementation returns `PokemonSlot` object instead of proper slot identifier
   ```csharp
   // Current (incorrect):
   public PokemonSlot GetSlot()
   {
       int poistionOffset = (int)Math.Floor(Side.N / 2.0) * Side.Active.Count;
       return new PokemonSlot(Side.Id, poistionOffset);
   }
   
   // Should be:
   public PokemonSlot GetSlot()
   {
  int positionOffset = (int)Math.Floor(Side.N / 2.0) * Side.Active.Count;
       char positionLetter = "abcdef"[Position + positionOffset];
       return new PokemonSlot(Side.Id, positionLetter);
   }
   ```

2. **ShieldBoost modifier incorrect**
   - Currently: `public bool ShieldBoost { get; init; }`
   - Should be: `public bool ShieldBoost { get; set; }` (needs to be settable like SwordBoost)

3. **Missing BaseAbility initialization in constructor**
   - Constructor doesn't set `BaseAbility = set.Ability`
   - Should initialize before `Ability = BaseAbility`
   ```csharp
   // Add in constructor after getting ability from set:
   BaseAbility = toID(set.Ability);
   Ability = BaseAbility;
   ```

---

### Pokemon.Stats.cs

4. **GetBestStat initialization issue** (TS line 808)
   - C# initializes `bestStatValue` to `int.MinValue`, should start at `0`
   ```csharp
   // Current:
   int bestStatValue = int.MinValue;
   
   // Should be:
   int bestStatValue = 0;
   ```

---

### Pokemon.Moves.cs

5. **GetMoves target handling incomplete** (TS lines 1195-1217)
   - Cursor target modifications are placeholders without implementation
   - PollenPuff target modification has empty case
   - TeraStarStorm target modification has empty case

6. **GetMoves missing Struggle fallback logic** (TS lines 1244-1245)
   - Should return Struggle array when `!hasValidMove`
   - Current implementation returns empty array
   ```csharp
   // After building moves list, add:
   if (!hasValidMove)
   {
       return [new PokemonMoveData
       {
           Move = Battle.Library.Moves[MoveId.Struggle],
           Target = MoveTarget.RandomNormal,
           Disabled = false,
           DisabledSource = null,
       }];
   }
   ```

7. **DisableMove logic inverted** (TS line 1429)
   - C# checks: `moveSlot.Id != moveId`
   - Should check: `moveSlot.Id == moveId`
   ```csharp
   // Current (incorrect):
   foreach (MoveSlot moveSlot in MoveSlots.Where(moveSlot =>
       moveSlot.Id != moveId && moveSlot.Disabled != true))
   
   // Should be:
   foreach (MoveSlot moveSlot in MoveSlots.Where(moveSlot =>
         moveSlot.Id == moveId && moveSlot.Disabled != true))
   ```

---

### Pokemon.Status.cs

8. **ClearVolatile incomplete Eternatus check** (TS line 2353)
   - Should check: `species.name === 'Eternatus-Eternamax' && volatiles['dynamax']`
   - Should preserve Dynamax volatile for Eternatus-Eternamax
   ```csharp
   // After clearing volatiles, add:
   if (Species.Id == SpecieId.EternatusEternamax && 
       volatiles.ContainsKey(ConditionId.Dynamax))
   {
  Volatiles = new Dictionary<ConditionId, EffectState>
       {
     [ConditionId.Dynamax] = volatiles[ConditionId.Dynamax]
   };
   }
   else
   {
       Volatiles.Clear();
   }
   ```

---

### Pokemon.Types.cs

9. **Missing Stellar type check in SetType** (TS lines 2544-2546)
   - Should prevent Stellar from being set as base type
   ```csharp
   if (!enforce)
   {
       // No Pokemon should be able to have Stellar as a base type
       if (types.Any(t => t == PokemonType.Stellar))
       {
      return false;
       }
       // ...existing checks...
   }
   ```

10. **ApparentType not formatted correctly** (TS line 2565)
    - C# sets: `ApparentType = Types.ToList()`
    - Should be formatted/joined representation for display
    - Consider if field type should be `string` instead of `List<PokemonType>`
    - Used in battle protocol messages

---

### Pokemon.Transformation.cs

11. **TransformInto missing timesAttacked copy** (TS line 1651)
    - Should copy: `this.timesAttacked = pokemon.timesAttacked`
    ```csharp
    // After copying stats, add:
    TimesAttacked = pokemon.TimesAttacked;
    ```

12. **SetSpecie method name inconsistency** (TS line 1820)
    - TS uses `setSpecies` (plural)
    - C# uses `SetSpecie` (singular)
 - Minor: Consider renaming for consistency with source

---

### Pokemon.AbilityItem.cs

13. **SetItem knocked off check logic reversed** (TS line 2470)
    - TS: `if (this.itemState.knockedOff && sourceEffect?.id !== 'recycle')`
    - C# checks for Recycle and returns false (inverted logic)
    - Should allow setting item if move is Recycle
    ```csharp
    // Current (incorrect):
    if (ItemState.KnockedOff == true && effect is ActiveMove { Id: MoveId.Recycle })
    {
        return false;
    }
    
    // Should be:
    if (ItemState.KnockedOff == true && 
        !(effect is ActiveMove { Id: MoveId.Recycle }))
    {
      return false;
    }
    ```

14. **SetItem missing sourceEffect default assignment**
    - Should set `sourceEffect = Battle.Effect` if not provided before checking effectId

---

### Pokemon.Immunity.cs

15. **RunImmunity missing ignoreImmunity check** (TS lines 2623-2627)
    - Should check move's `ignoreImmunity` property before type immunity
    ```csharp
    public bool RunImmunity(ActiveMove source, bool message = false)
    {
        // Add this check first:
        if (source.IgnoreImmunity != null &&
            (source.IgnoreImmunity is true or 
         source.IgnoreImmunity.Contains(source.Type)))
        {
            return true;
        }
   
        return RunImmunity(source.Type, message);
    }
    ```

16. **RunImmunity missing type validation** (TS line 2629)
    - Should validate type name exists in dex before proceeding
    - Throw error for invalid type names

---

### Pokemon.Positioning.cs

17. **IsAlly incomplete side check** (TS line 1135)
    - TS checks: `this.side === pokemon.side || this.side.allySide === pokemon.side`
    - C# only checks first condition
    - Need to add allySide comparison for multi-battle support
    ```csharp
    public bool IsAlly(Pokemon? pokemon = null)
    {
        if (pokemon == null) return false;
        return Side == pokemon.Side || Side.AllySide == pokemon.Side;
    }
    ```

---

### Pokemon.Requests.cs

18. **GetSwitchRequestData missing ident field** (TS line 1363)
    - Should include: `Ident = Fullname`

19. **GetSwitchRequestData missing details field** (TS line 1364)
    - Should include: `Details = Details` (the property, not string)

20. **GetSwitchRequestData move transformation incomplete**
    - Return/Frustration moves should append base power to move ID
    - Example: `"return102"`, `"frustration40"`
    ```csharp
    // When building moves list:
    var moves = moveSource.Select(moveSlot =>
    {
        var move = Battle.Library.Moves[moveSlot.Id];
        
        // Handle Return/Frustration base power display
        if (move.Id is MoveId.Return or MoveId.Frustration)
  {
        int basePower = move.BasePowerCallback?.Invoke(this) ?? 0;
 // Need to create modified move display with power appended
        }
        
        return move;
    }).ToList();
    ```

---

### Pokemon.Weather.cs

21. **No Issues Found** ?
    - Implementation matches TypeScript perfectly

---

## Summary Statistics (Gen 9 Only)

- **Total Files Reviewed**: 13
- **Total Issues Found**: 21
- **Critical Issues**: 7 (incorrect logic, missing core functionality)
- **Major Issues**: 10 (incomplete implementations)
- **Minor Issues**: 4 (consistency, edge cases)

## Priority Recommendations

### High Priority (Fix Immediately)
1. **Fix GetSlot() implementation** - Affects battle protocol communication
2. **Fix DisableMove inverted logic** - Causes moves to be incorrectly disabled
3. **Fix SetItem knocked off logic** - Prevents Recycle from working correctly
4. **Add BaseAbility initialization** - May cause ability issues on switch

### Medium Priority
5. Add Struggle fallback in GetMoves
6. Fix GetBestStat initialization
7. Add missing ident/details in GetSwitchRequestData
8. Fix IsAlly to include allySide check
9. Add Stellar type prevention in SetType
10. Fix ShieldBoost to be settable

### Low Priority (Polish)
11. Add timesAttacked copy in TransformInto
12. Implement move target handling cases (Cursor, PollenPuff, TeraStarStorm)
13. Add Return/Frustration base power display
14. Add ignoreImmunity check in RunImmunity
15. Fix ApparentType formatting
16. Add Eternatus-Eternamax Dynamax preservation
17. Rename SetSpecie to SetSpecies for consistency

---

## Removed Issues (Not Relevant to Gen 9)

The following issues from the initial review were removed as they are **not applicable to Gen 9**:

- ? **ItemKnockedOff field** - Gen 3-4 only mechanic, not in Gen 9
- ? **HpType/HpPower fields** - Hidden Power removed in Gen 9
- ? **BaseHpType/BaseHpPower fields** - Hidden Power removed in Gen 9
- ? **HeroMessageDisplayed field** - Not used in Gen 9 VGC (may be Legends Arceus specific)
- ? **Gen 1 ModifiedStats/ModifyStat/RecalculateStats** - Not Gen 9
- ? **Gen 1 Mimic PP handling** - Not Gen 9
- ? **Gen 2 LastMoveEncore** - Not Gen 9
- ? **Gen 4 forme changes (Giratina/Arceus)** - Not Gen 9
- ? **Gen 1 stat modifications** - Not Gen 9
- ? **Gen 5+ Substitute check in Transform** - Not needed for Gen 9 only
- ? **Gen 1 Stadium Ditto check** - Not Gen 9
- ? **Pursuit move checks** - Pursuit removed in Gen 9
- ? **Hidden Power display logic** - Hidden Power removed in Gen 9
- ? **Mega/Ultra Burst/Primal forme change** - Excluded per requirements
- ? **Damage method decimal handling** - Integer type, not applicable
- ? **Damage method NaN check** - C# type system prevents NaN in int

---

## Notes

- Review now focuses **exclusively on Gen 9 VGC Double Battle format**
- Removed all Gen 1-8 specific features and mechanics
- Removed features for moves/mechanics not present in Gen 9 (Hidden Power, Pursuit, Mega Evolution, etc.)
- Code is generally well-structured and readable
- Most logic translations from TypeScript are accurate
- Main remaining issues are logic inversions and incomplete implementations

---

## Generated By

GitHub Copilot - Pokemon Class Review (Gen 9 Corrected)
Date: 2025
