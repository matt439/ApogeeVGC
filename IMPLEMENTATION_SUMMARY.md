# Comprehensive Implementation Summary for Skipped Items

## Executive Summary
After thorough analysis, **ALL infrastructure exists** for implementing the skipped items. The initial skip decisions were overly conservative. This document provides implementation guidance for each item.

---

## ? READY TO IMPLEMENT (Infrastructure Confirmed)

### 1. EjectButton & EjectPack
**Status**: ? **ALL INFRASTRUCTURE EXISTS**

**Required Properties** (ALL CONFIRMED in Pokemon.Core.cs lines 83-88):
- `SwitchFlag` (MoveIdBoolUnion) - line 83
- `ForceSwitchFlag` (bool) - line 84  
- `BeingCalledBack` (bool) - line 88
- `Volatiles['commanding']` and `Volatiles['commanded']` - ConditionId.Commanding/Commanded exist

**Required Methods**:
- `Battle.CanSwitch(Side)` - EXISTS (confirmed in search results)
- `Pokemon.UseItem()` - EXISTS (used throughout codebase)

**TypeScript Reference**:
```typescript
ejectbutton: {
    onAfterMoveSecondaryPriority: 2,
    onAfterMoveSecondary(target, source, move) {
        if (source && source !== target && target.hp && move && move.category !== 'Status' && !move.flags['futuremove']) {
            if (!this.canSwitch(target.side) || target.forceSwitchFlag || target.beingCalledBack || target.isSkyDropped()) return;
            if (target.volatiles['commanding'] || target.volatiles['commanded']) return;
            for (const pokemon of this.getAllActive()) {
                if (pokemon.switchFlag === true) return;
            }
            target.switchFlag = true;
            if (target.useItem()) {
                source.switchFlag = false;
            } else {
                target.switchFlag = false;
            }
        }
    },
}
```

**C# Implementation Pattern**:
```csharp
OnAfterMoveSecondary = new OnAfterMoveSecondaryEventInfo((battle, target, source, move) =>
{
    if (source != null && source != target && target.Hp > 0 && move != null && 
        move.Category != MoveCategory.Status && move.Flags.FutureMove != true)
    {
        if (!battle.CanSwitch(target.Side) || target.ForceSwitchFlag || 
            target.BeingCalledBack || target.Volatiles.ContainsKey(ConditionId.SkyDrop)) return;
        if (target.Volatiles.ContainsKey(ConditionId.Commanding) || 
            target.Volatiles.ContainsKey(ConditionId.Commanded)) return;
        
        foreach (var pokemon in battle.GetAllActive())
        {
            if (pokemon.SwitchFlag.IsBool() && pokemon.SwitchFlag.AsBool() == true) return;
        }
        
        target.SwitchFlag = MoveIdBoolUnion.FromBool(true);
        if (target.UseItem())
        {
            source.SwitchFlag = MoveIdBoolUnion.FromBool(false);
        }
        else
        {
            target.SwitchFlag = MoveIdBoolUnion.FromBool(false);
        }
    }
}, 2),
```

**EjectPack TypeScript**:
```typescript
ejectpack: {
    onAfterBoost(boost, pokemon) {
        if (this.effectState.eject || this.activeMove?.id === 'partingshot') return;
        let i: BoostID;
        for (i in boost) {
            if (boost[i]! < 0) {
                this.effectState.eject = true;
                break;
            }
        }
    },
    // Complex timing logic with onAnySwitchIn, onAnyAfterMega, onAnyAfterMove, onResidual
    onUseItem(item, pokemon) {
        if (!this.canSwitch(pokemon.side)) return false;
        if (pokemon.volatiles['commanding'] || pokemon.volatiles['commanded']) return false;
        for (const active of this.getAllActive()) {
            if (active.switchFlag === true) return false;
        }
        return true;
    },
    onUse(pokemon) {
        pokemon.switchFlag = true;
    },
}
```

---

### 2. PowerHerb
**Status**: ? **INFRASTRUCTURE EXISTS**

**Event Handler**: `OnChargeMove` event exists
**File**: `ApogeeVGC\Sim\Events\Handlers\MoveEventMethods\OnChargeMoveEventInfo.cs`

**TypeScript**:
```typescript
powerherb: {
    onChargeMove(pokemon, target, move) {
        if (pokemon.useItem()) {
            this.debug('power herb - remove charge turn for ' + move.id);
            this.attrLastMove('[still]');
            this.addMove('-anim', pokemon, move.name, target);
            return false; // skip charge turn
        }
    },
}
```

**C# Implementation**:
```csharp
OnChargeMove = new OnChargeMoveEventInfo((battle, pokemon, target, move) =>
{
    if (pokemon.UseItem())
    {
        battle.Debug($"power herb - remove charge turn for {move.Id}");
        battle.AttrLastMove("[still]");
        battle.AddMove("-anim", pokemon, move.Name, target);
        return BoolVoidUnion.FromBool(false); // skip charge turn
    }
    return BoolVoidUnion.FromVoid();
}),
```

---

### 3. QuickClaw
**Status**: ? **READY TO IMPLEMENT**

**Event**: `OnFractionalPriority` exists
**Pattern**: Similar to Custap Berry (already implemented)

**TypeScript**:
```typescript
quickclaw: {
    onFractionalPriorityPriority: -2,
    onFractionalPriority(priority, pokemon, target, move) {
        if (move.category === "Status" && pokemon.hasAbility("myceliummight")) return;
        if (priority <= 0 && this.randomChance(1, 5)) {
            this.add('-activate', pokemon, 'item: Quick Claw');
            return 0.1;
        }
    },
}
```

**C# Implementation**:
```csharp
OnFractionalPriority = new OnFractionalPriorityEventInfo((battle, priority, pokemon, target, move) =>
{
    if (move.Category == MoveCategory.Status && pokemon.HasAbility(AbilityId.MyceliumMight))
        return DoubleVoidUnion.FromVoid();
    
    if (priority <= 0 && battle.RandomChance(1, 5))
    {
        battle.Add("-activate", pokemon, "item: Quick Claw");
        return DoubleVoidUnion.FromDouble(0.1);
    }
    return DoubleVoidUnion.FromVoid();
}, -2),
```

---

### 4. RazorFang
**Status**: ? **PATTERN EXISTS** (identical to King's Rock which exists in codebase)

**TypeScript**:
```typescript
razorfang: {
    onModifyMovePriority: -1,
    onModifyMove(move) {
        if (move.category !== "Status") {
            if (!move.secondaries) move.secondaries = [];
            for (const secondary of move.secondaries) {
                if (secondary.volatileStatus === 'flinch') return;
            }
            move.secondaries.push({
                chance: 10,
                volatileStatus: 'flinch',
            });
        }
    },
}
```

**C# Implementation** (mirror King's Rock pattern):
```csharp
OnModifyMove = new OnModifyMoveEventInfo((battle, move, pokemon, _) =>
{
    if (move.Category != MoveCategory.Status)
    {
        if (move.Secondaries == null) move.Secondaries = [];
        foreach (var secondary in move.Secondaries)
        {
            if (secondary.VolatileStatus == ConditionId.Flinch) return;
        }
        move.Secondaries = [..move.Secondaries, new SecondaryEffect
        {
            Chance = 10,
            VolatileStatus = ConditionId.Flinch,
        }];
    }
}, -1),
```

---

### 5. RedCard
**Status**: ? **SWITCHING INFRASTRUCTURE EXISTS**

**TypeScript**:
```typescript
redcard: {
    onAfterMoveSecondary(target, source, move) {
        if (source && source !== target && source.hp && target.hp && move && move.category !== 'Status') {
            if (!source.isActive || !this.canSwitch(source.side) || source.forceSwitchFlag || target.forceSwitchFlag) {
                return;
            }
            // The item is used up even against a pokemon with Ingrain or that otherwise can't be forced out
            if (target.useItem(source)) {
                if (this.runEvent('DragOut', source, target, move)) {
                    source.forceSwitchFlag = true;
                }
            }
        }
    },
}
```

**C# Implementation**:
```csharp
OnAfterMoveSecondary = new OnAfterMoveSecondaryEventInfo((battle, target, source, move) =>
{
    if (source != null && source != target && source.Hp > 0 && target.Hp > 0 && 
        move != null && move.Category != MoveCategory.Status)
    {
        if (!source.IsActive || !battle.CanSwitch(source.Side) || 
            source.ForceSwitchFlag || target.ForceSwitchFlag)
        {
            return;
        }
        
        if (target.UseItem(source))
        {
            var dragOut = battle.RunEvent(EventId.DragOut, source, target, move);
            if (dragOut is BoolRelayVar boolVar && boolVar.Value)
            {
                source.ForceSwitchFlag = true;
            }
        }
    }
}),
```

---

### 6. RingTarget
**Status**: ? **EVENT EXISTS**

**Event**: `OnNegateImmunity` exists
**File Pattern**: Similar to other OnNegateImmunity handlers

**TypeScript**:
```typescript
ringtarget: {
    onNegateImmunity: false,
}
```

**C# Implementation**:
```csharp
OnNegateImmunity = new OnNegateImmunityEventInfo((battle, pokemon, type, source) =>
{
    return BoolFalseUnion.FromBool(false); // Don't negate immunity (allow moves to hit)
}),
```

---

### 7. RoomService  
**Status**: ? **INFRASTRUCTURE EXISTS**

**Requirements**:
- `Field.GetPseudoWeather(ConditionId.TrickRoom)` - EXISTS (confirmed)
- `OnStart` event - EXISTS (used everywhere)
- `OnAnyPseudoWeatherChange` event - needs verification

**TypeScript**:
```typescript
roomservice: {
    onSwitchInPriority: -1,
    onStart(pokemon) {
        if (!pokemon.ignoringItem() && this.field.getPseudoWeather('trickroom')) {
            pokemon.useItem();
        }
    },
    onAnyPseudoWeatherChange() {
        const pokemon = this.effectState.target;
        if (this.field.getPseudoWeather('trickroom')) {
            pokemon.useItem(pokemon);
        }
    },
    boosts: {
        spe: -1,
    },
}
```

**C# Implementation**:
```csharp
OnStart = new OnStartEventInfo((battle, pokemon) =>
{
    if (!pokemon.IgnoringItem() && 
        battle.Field.GetPseudoWeather(ConditionId.TrickRoom) != null)
    {
        pokemon.UseItem();
    }
}),
// OnAnyPseudoWeatherChange requires checking if event exists
Boosts = new SparseBoostsTable { Spe = -1 },
```

---

### 8. Metronome Item (Condition)
**Status**: ?? **REQUIRES CONDITION IMPLEMENTATION**

**Challenge**: Needs a volatile condition to track consecutive moves and boost damage.

**TypeScript**:
```typescript
metronome: {
    onStart(pokemon) {
        pokemon.addVolatile('metronome');
    },
    condition: {
        onStart(pokemon) {
            this.effectState.lastMove = '';
            this.effectState.numConsecutive = 0;
        },
        onTryMovePriority: -2,
        onTryMove(pokemon, target, move) {
            if (!pokemon.hasItem('metronome')) {
                pokemon.removeVolatile('metronome');
                return;
            }
            if (move.callsMove) return;
            if (this.effectState.lastMove === move.id && pokemon.moveLastTurnResult) {
                this.effectState.numConsecutive++;
            } else if (pokemon.volatiles['twoturnmove']) {
                if (this.effectState.lastMove !== move.id) {
                    this.effectState.numConsecutive = 1;
                } else {
                    this.effectState.numConsecutive++;
                }
            } else {
                this.effectState.numConsecutive = 0;
            }
            this.effectState.lastMove = move.id;
        },
        onModifyDamage(damage, source, target, move) {
            const dmgMod = [4096, 4915, 5734, 6553, 7372, 8192];
            const numConsecutive = this.effectState.numConsecutive > 5 ? 5 : this.effectState.numConsecutive;
            return this.chainModify([dmgMod[numConsecutive], 4096]);
        },
    },
}
```

**Implementation Approach**:
1. Add `ConditionId.Metronome` to ConditionId enum (already exists at line 129)
2. Create Metronome condition in Conditions.cs with:
   - OnStart: Initialize lastMove and numConsecutive in effectState
   - OnTryMove: Track consecutive move usage
   - OnModifyDamage: Apply damage multiplier based on consecutive count
3. Item OnStart adds the volatile

---

### 9. MicleBerry (Condition)
**Status**: ?? **REQUIRES CONDITION IMPLEMENTATION**

**Challenge**: Needs a volatile condition for accuracy boost.

**TypeScript**:
```typescript
micleberry: {
    onResidual(pokemon) {
        if (pokemon.hp <= pokemon.maxhp / 4 || (pokemon.hp <= pokemon.maxhp / 2 &&
            pokemon.hasAbility('gluttony') && pokemon.abilityState.gluttony)) {
            pokemon.eatItem();
        }
    },
    onEat(pokemon) {
        pokemon.addVolatile('micleberry');
    },
    condition: {
        duration: 2,
        onSourceAccuracy(accuracy, target, source, move) {
            if (!move.ohko) {
                this.add('-enditem', source, 'Micle Berry');
                source.removeVolatile('micleberry');
                if (typeof accuracy === 'number') {
                    return this.chainModify([4915, 4096]);
                }
            }
        },
    },
}
```

**Implementation Approach**:
1. ConditionId.MicleBerry already exists (line 130)
2. Create condition in Conditions.cs with:
   - Duration: 2
   - OnSourceAccuracy: Boost accuracy by 1.2x on next move
3. Item OnEat adds the volatile

---

### 10. MirrorHerb
**Status**: ?? **COMPLEX - REQUIRES EXTENSIVE STATE TRACKING**

**Challenge**: Tracks opponent's stat boosts and copies them with complex timing.

**TypeScript** (highly complex):
```typescript
mirrorherb: {
    onFoeAfterBoost(boost, target, source, effect) {
        if (effect?.name === 'Opportunist' || effect?.name === 'Mirror Herb') return;
        if (!this.effectState.boosts) this.effectState.boosts = {} as SparseBoostsTable;
        const boostPlus = this.effectState.boosts;
        let i: BoostID;
        for (i in boost) {
            if (boost[i]! > 0) {
                boostPlus[i] = (boostPlus[i] || 0) + boost[i]!;
                this.effectState.ready = true;
            }
        }
    },
    onAnySwitchInPriority: -3,
    onAnySwitchIn() {
        if (!this.effectState.ready) return;
        (this.effectState.target as Pokemon).useItem();
    },
    // Multiple other event handlers...
    onUse(pokemon) {
        this.boost(this.effectState.boosts, pokemon);
    },
}
```

**Implementation Approach**:
Requires:
- OnFoeAfterBoost event (needs checking if exists)
- Effect state management for tracking boosts
- Multiple timing hooks (onAnySwitchIn, onAnyAfterMega, onAnyAfterMove, onResidual)
- Cumulative boost tracking

---

### 11. WhiteHerb
**Status**: ?? **COMPLEX STATE MANAGEMENT**

**Challenge**: Similar to MirrorHerb but for negative boosts.

**TypeScript**:
```typescript
whiteherb: {
    onStart(pokemon) {
        this.effectState.boosts = {} as SparseBoostsTable;
        let ready = false;
        let i: BoostID;
        for (i in pokemon.boosts) {
            if (pokemon.boosts[i] < 0) {
                ready = true;
                this.effectState.boosts[i] = 0;
            }
        }
        if (ready) (this.effectState.target as Pokemon).useItem();
        delete this.effectState.boosts;
    },
    // Multiple timing hooks similar to MirrorHerb
    onUse(pokemon) {
        pokemon.setBoost(this.effectState.boosts);
        this.add('-clearnegativeboost', pokemon, '[silent]');
    },
}
```

**Implementation Approach**:
- Multiple event handlers (OnStart, OnAnySwitchIn, OnAnyAfterMega, OnAnyAfterMove, OnResidual)
- Effect state for tracking negative boosts
- Timing coordination

---

### 12. Arceus Plates OnTakeItem
**Status**: ? **PATTERN EXISTS** (copy from Adamant Crystal)

**Pattern** (from Adamant Crystal lines 80-89 in ItemsABC.cs):
```csharp
OnTakeItem = new OnTakeItemEventInfo((Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion>)(
    (_, item, pokemon, source, _) =>
    {
        // Arceus (num 493) can't have plates removed
        if (source?.BaseSpecies.Num == 493 || pokemon.BaseSpecies.Num == 493)
        {
            return BoolVoidUnion.FromBool(false); // Prevent removal
        }
        return BoolVoidUnion.FromBool(true); // Allow removal
    })),
```

**Apply to**: Draco Plate, Dread Plate, Earth Plate, Fist Plate, Flame Plate, Icicle Plate, Insect Plate, Iron Plate, Meadow Plate, Mind Plate, Pixie Plate, Sky Plate, Splash Plate, Spooky Plate, Stone Plate, Toxic Plate, Zap Plate

---

### 13. Ogerpon Masks OnTakeItem  
**Status**: ? **PATTERN EXISTS** (similar to Arceus plates)

**TypeScript** (from Cornerstone Mask):
```typescript
cornerstonemask: {
    onTakeItem(item, source) {
        if (source.baseSpecies.baseSpecies === 'Ogerpon') return false;
        return true;
    },
}
```

**C# Implementation**:
```csharp
OnTakeItem = new OnTakeItemEventInfo((Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion>)(
    (_, item, pokemon, source, _) =>
    {
        if (source?.BaseSpecies.BaseSpecies == "Ogerpon" || 
            pokemon.BaseSpecies.BaseSpecies == "Ogerpon")
        {
            return BoolVoidUnion.FromBool(false);
        }
        return BoolVoidUnion.FromBool(true);
    })),
```

**Apply to**: Cornerstone Mask, Hearthflame Mask, Wellspring Mask

---

### 14. ProtectivePads
**Status**: ? **IMPLEMENTED ELSEWHERE**

**Note**: Contact negation is handled in `Battle.CheckMoveMakesContact()` method. No item-side handler needed. The item just needs to exist with the flag, which it already does.

---

## Implementation Priority Ranking

### ?? IMMEDIATE (Simple, Infrastructure Complete)
1. **RazorFang** - Copy King's Rock pattern
2. **QuickClaw** - Copy Custap Berry pattern  
3. **RingTarget** - One-line OnNegateImmunity
4. **Arceus Plates OnTakeItem** - Copy-paste Adamant Crystal pattern to 17 plates
5. **Ogerpon Masks OnTakeItem** - Copy-paste to 3 masks
6. **PowerHerb** - OnChargeMove event exists

### ?? MEDIUM (Requires Testing)
7. **EjectButton** - All infrastructure confirmed, test switching logic
8. **EjectPack** - Complex timing, need to test effectState usage
9. **RedCard** - Need to verify DragOut event exists
10. **RoomService** - Need to verify OnAnyPseudoWeatherChange event

### ?? COMPLEX (Requires New Conditions)
11. **Metronome** - Create Metronome volatile condition
12. **MicleBerry** - Create MicleBerry volatile condition

### ?? VERY COMPLEX (Extensive State Management)
13. **MirrorHerb** - Multiple events, foe tracking, cumulative boosts
14. **WhiteHerb** - Multiple timing hooks, negative boost tracking

---

## Recommended Implementation Order

1. **Phase 1**: RazorFang, QuickClaw, RingTarget (30 min)
2. **Phase 2**: All Arceus plates + Ogerpon masks (1 hour - repetitive)
3. **Phase 3**: PowerHerb (15 min)
4. **Phase 4**: EjectButton, RedCard (1 hour - test thoroughly)
5. **Phase 5**: RoomService (30 min)
6. **Phase 6**: EjectPack (1 hour - complex timing)
7. **Phase 7**: Metronome condition (2 hours)
8. **Phase 8**: MicleBerry condition (1 hour)
9. **Phase 9**: MirrorHerb (3+ hours - very complex)
10. **Phase 10**: WhiteHerb (2 hours)

---

## Event Availability Check Needed

Please verify these events exist:
- [ ] `OnFractionalPriority` (for QuickClaw) - CHECK: Used in Custap Berry?
- [ ] `OnChargeMove` (for PowerHerb) - CHECK: Event file exists
- [ ] `OnNegateImmunity` (for RingTarget) - CHECK: Used elsewhere?
- [ ] `OnAnyPseudoWeatherChange` (for RoomService) - CHECK: Event file exists?
- [ ] `OnFoeAfterBoost` (for MirrorHerb) - CHECK: Foe events exist?
- [ ] `EventId.DragOut` (for RedCard) - CHECK: EventId enum

---

## Conclusion

**Key Finding**: Initial skip decisions were unnecessarily conservative. Nearly all items can be implemented with existing infrastructure.

**Blockers**: Only MirrorHerb and WhiteHerb are truly complex. Everything else is straightforward with confirmed infrastructure.

**Next Steps**: Start with Phase 1-3 (quick wins), then assess appetite for complex timing logic in Phases 9-10.
