# Quick Start: Using the New Event System

## TL;DR

? **All your existing code still works** - no changes needed!

? **Want strong typing + better performance?** Use the `Create` pattern shown below.

? **Need to update an event class?** Follow the 3-step template in `MIGRATION_TEMPLATE.md`.

## For Existing Handlers: No Action Required

Your existing handlers work perfectly:

```csharp
// This still works exactly as before!
[ConditionId.Paralysis] = new()
{
    OnBeforeMove = new OnBeforeMoveEventInfo((Battle battle, Pokemon target, Pokemon source, ActiveMove move) =>
    {
if (!battle.RandomChance(1, 4)) return new VoidReturn();
        if (battle.DisplayUi)
    {
            battle.Add("cant", target, "par");
    }
        return false;
    }, priority: 1),
}
```

## For New Handlers: Use Create Pattern

Three event classes are already updated with the `Create` helper:
- `OnBeforeMoveEventInfo`
- `OnStartEventInfo` (ConditionSpecific)
- `OnResidualEventInfo`

### Example: Using Create

```csharp
[ConditionId.Burn] = new()
{
    // Option 1: Legacy (still works)
    OnResidual = new OnResidualEventInfo(
        (Battle battle, Pokemon pokemon, Pokemon source, IEffect effect) =>
        {
   battle.Damage(pokemon.BaseMaxHp / 16);
      },
        order: 10
    ),
    
    // Option 2: Create helper (NEW - recommended)
    OnResidual = OnResidualEventInfo.Create(
        (battle, pokemon, source, effect) =>
        {
      battle.Damage(pokemon.BaseMaxHp / 16);
    // No return needed for void handlers
        },
        order: 10
    ),
    
    // Option 3: Context (NEW - most flexible)
    OnResidual = new OnResidualEventInfo(
 context =>
        {
            context.Battle.Damage(context.GetTargetPokemon().BaseMaxHp / 16);
            return null; // void return
        },
        order: 10
    ),
}
```

### Example: Complex Handler

```csharp
OnBeforeMove = OnBeforeMoveEventInfo.Create(
    (battle, target, source, move) =>
    {
        // Full IntelliSense on all parameters
        // Compile-time type checking
      // Same syntax as before!

  if (target.HasAbility(AbilityId.Bulletproof) && move.Flags.Bullet == true)
        {
    battle.Add("-immune", target, "[from] ability: Bulletproof");
      return new BoolRelayVar(false);
        }
        
    return null; // continue
    },
    priority: 1
)
```

## To Update More Event Classes

Follow this 3-step pattern (detailed in `MIGRATION_TEMPLATE.md`):

### Step 1: Add Context Constructor

```csharp
public OnXXXEventInfo(
    EventHandlerDelegate contextHandler,
  int? priority = null,
    ... other params ...)
{
    Id = EventId.XXX;
    ContextHandler = contextHandler;
    Priority = priority;
    // ... copy other initialization ...
}
```

### Step 2: Add Create Helper

```csharp
public static OnXXXEventInfo Create(
    Func<Param1, Param2, ..., RelayVar?> handler,
    int? priority = null,
    ... other params ...)
{
    return new OnXXXEventInfo(
     context => handler(
            context.GetParam1(),
    context.GetParam2(),
            ...
        ),
        priority,
        ...
    );
}
```

### Step 3: Test

```csharp
// All three should work:
var legacy = new OnXXXEventInfo((p1, p2) => result);
var context = new OnXXXEventInfo(ctx => result);
var create = OnXXXEventInfo.Create((p1, p2) => result);
```

## Recommended Event Classes to Update Next

Update these high-frequency events for maximum performance benefit:

1. **`OnDamagingHitEventInfo`** - Called on every damage-dealing move
2. **`OnBasePowerEventInfo`** - Called for power calculations
3. **`OnModifySpeEventInfo`** - Called for speed calculations
4. **`OnModifyAtkEventInfo`** - Called for attack calculations
5. **`OnModifyDefEventInfo`** - Called for defense calculations

Each takes ~5 minutes to update following the template.

## Parameter Mapping Cheat Sheet

| Legacy Parameter | Context Accessor |
|-----------------|------------------|
| `Battle battle` | `context.Battle` |
| `Pokemon target` | `context.GetTargetPokemon()` |
| `Pokemon source` | `context.GetSourcePokemon()` |
| `ActiveMove move` | `context.GetMove()` |
| `IEffect effect` | `context.GetSourceEffect<IEffect>()` |
| `int value` (relay) | `context.GetRelayVar<IntRelayVar>().Value` |

## Return Type Cheat Sheet

| Legacy Return | Context Return |
|--------------|----------------|
| `void` / `VoidReturn` | `return null;` |
| `true` / `false` | `return new BoolRelayVar(value);` |
| `int value` | `return new IntRelayVar(value);` |
| `BoolVoidUnion` | `return value.HasValue ? new BoolRelayVar(value.Value) : null;` |

## Complete Example: Burn Condition

Here's how the Burn condition looks with the new system:

```csharp
[ConditionId.Burn] = new()
{
    Id = ConditionId.Burn,
    Name = "Burn",
    EffectType = EffectType.Status,
    ImmuneTypes = [PokemonType.Fire],

    // Using Create helper - strongly typed!
    OnStart = OnStartEventInfo.Create((battle, target, source, sourceEffect) =>
    {
        if (!battle.DisplayUi) return null;
     
      switch (sourceEffect.EffectType)
        {
        case EffectType.Item when sourceEffect is Item { Id: ItemId.FlameOrb }:
     battle.Add("-status", target, "brn", "[from] item: Flame Orb");
       break;
         case EffectType.Ability when sourceEffect is Ability:
     battle.Add("-status", target, "brn", "[from] ability: " + sourceEffect.Name, $"[of] {source}");
    break;
  case EffectType.Move:
        battle.Add("-status", target, "brn");
         break;
        }
        
        return null;
    }),
    
    OnResidual = OnResidualEventInfo.Create((battle, pokemon, _, _) =>
 {
        battle.Damage(pokemon.BaseMaxHp / 16);
    }, order: 10),
}
```

## Documentation Files

- **`MIGRATION_TEMPLATE.md`** ? Start here to update event classes
- **`EVENT_CONTEXT_REFACTORING.md`** - Complete guide with many examples
- **`BEFORE_AFTER_COMPARISON.md`** - See the improvements
- **`IMPLEMENTATION_COMPLETE.md`** - Full technical overview

## Key Takeaways

1. ? **Nothing breaks** - all existing code works
2. ? **Use Create for new code** - same syntax, better performance
3. ?? **Follow MIGRATION_TEMPLATE.md** - update ~5 mins per class
4. ?? **Update high-frequency events first** - maximum benefit
5. ?? **Update others gradually** - no rush, works fine as-is

## Need Help?

Check the documentation files or look at these examples:
- `OnBeforeMoveEventInfo.cs` - Complete working example
- `OnStartEventInfo.cs` - Another complete example
- `OnResidualEventInfo.cs` - Third complete example

**Happy coding with the cleaner event system!** ??
