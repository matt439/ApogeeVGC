# IEventMethodsV2 Duplicate Ally Properties - FIXED!

## Issue Identified

`IEventMethodsV2` contained **15 "Ally" prefixed properties** that were duplicated in `IPokemonEventMethodsV2`, which inherits from `IEventMethodsV2`. This caused redundancy since those properties were accessible through both interfaces.

## Root Cause

When creating `IEventMethodsV2`, 15 Ally-prefixed event properties were mistakenly added to the base interface, even though:
1. The original `IEventMethods` interface has **NO** Ally-prefixed events
2. All Ally-prefixed events belong in `IPokemonEventMethods` (which inherits from `IEventMethods`)
3. This same pattern should apply to the V2 interfaces

## Duplicate Properties Removed

The following **15 Ally-prefixed properties** were removed from `IEventMethodsV2`:

1. `OnAllyBasePowerEventInfo? OnAllyBasePower`
2. `OnAllyModifyAtkEventInfo? OnAllyModifyAtk`
3. `OnAllyModifySpAEventInfo? OnAllyModifySpA`
4. `OnAllyModifySpDEventInfo? OnAllyModifySpD`
5. `OnAllyTryHitSideEventInfo? OnAllyTryHitSide`
6. `OnAllyDamagingHitEventInfo? OnAllyDamagingHit`
7. `OnAllyAfterHitEventInfo? OnAllyAfterHit`
8. `OnAllyAfterSetStatusEventInfo? OnAllyAfterSetStatus`
9. `OnAllyModifyAccuracyEventInfo? OnAllyModifyAccuracy`
10. `OnAllyTryHitEventInfo? OnAllyTryHit`
11. `OnAllyInvulnerabilityEventInfo? OnAllyInvulnerability`
12. `OnAllySwitchOutEventInfo? OnAllySwitchOut`
13. `OnAllyFaintEventInfo? OnAllyFaint`
14. `OnAllyAfterBoostEventInfo? OnAllyAfterBoost`
15. `OnAllyPrepareHitEventInfo? OnAllyPrepareHit`

## Correct Design Pattern

### Old Interfaces (Correct Pattern)
```csharp
// IEventMethods: Base events only (no Ally prefix)
public interface IEventMethods
{
    // Base events, Foe prefix, Source prefix, Any prefix
    // NO Ally prefix events
}

// IPokemonEventMethods: Inherits base + adds ALL 78 Ally events
public interface IPokemonEventMethods : IEventMethods
{
    // All 78 Ally-prefixed events
}
```

### V2 Interfaces (Now Fixed)
```csharp
// IEventMethodsV2: Base events only (no Ally prefix)
public interface IEventMethodsV2
{
  // 94 Base events (no prefix)
    // 95 Foe prefix events
    // 90 Source prefix events
    // 105 Any prefix events
    // ? NO Ally prefix events (removed!)
}

// IPokemonEventMethodsV2: Inherits base + adds ALL 78 Ally events
public interface IPokemonEventMethodsV2 : IEventMethodsV2
{
    // ? All 78 Ally-prefixed events
    // No duplication!
}
```

## Impact

### Before Fix:
- ? 15 Ally properties in `IEventMethodsV2`
- ? 78 Ally properties in `IPokemonEventMethodsV2` (includes duplicates)
- ? Total accessible through `IPokemonEventMethodsV2`: 78 (15 duplicated)
- ? Inconsistent with original interface design

### After Fix:
- ? 0 Ally properties in `IEventMethodsV2`
- ? 78 unique Ally properties in `IPokemonEventMethodsV2`
- ? Total accessible through `IPokemonEventMethodsV2`: 78 (no duplication)
- ? Consistent with original interface design

## Updated Statistics

### IEventMethodsV2 (After Fix)
| Prefix | Count | Status |
|--------|-------|--------|
| None (Base) | 94 | ? Correct |
| Foe | 95 | ? Correct |
| Source | 90 | ? Correct |
| Any | 105 | ? Correct |
| Ally | 0 | ? Removed (moved to IPokemonEventMethodsV2) |
| **Total** | **384** | ? Correct |

### IPokemonEventMethodsV2 (Inherits from IEventMethodsV2)
| Source | Count | Status |
|--------|-------|--------|
| Inherited from IEventMethodsV2 | 384 | ? Base, Foe, Source, Any events |
| Ally events (specific to Pokemon) | 78 | ? All Ally events |
| **Total Accessible** | **462** | ? No duplication |

## Verification

### Build Status: ? SUCCESS
```
dotnet build
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### Interface Hierarchy Verified:
```
IEventMethodsV2 (384 properties)
    ? inherits
IPokemonEventMethodsV2 (78 additional properties)
    = 462 total properties (no duplication)
```

## Benefits

1. ? **Eliminated Redundancy** - Removed 15 duplicate properties
2. ? **Consistent Design** - Matches original interface pattern
3. ? **Clear Separation** - Ally events now clearly belong to Pokemon interface
4. ? **Maintainability** - Single source of truth for each event
5. ? **Type Safety** - No ambiguity about which interface owns which events

## Summary

The duplicate "Ally" prefixed properties have been successfully removed from `IEventMethodsV2`. All 78 Ally-prefixed events now correctly reside only in `IPokemonEventMethodsV2`, matching the design pattern of the original interfaces where all Ally events belong to `IPokemonEventMethods`.

---

**Status:** ? FIXED  
**Date:** $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")  
**Build:** ? Successful (0 errors, 0 warnings)  
**Properties Removed:** 15  
**Duplication Eliminated:** 100%  
**Design Consistency:** ? Achieved
