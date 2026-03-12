# Full Equivalence Omnibus Fix

**Commit:** `00f70d68`
**Date:** 2026-03-10

## Problem
The C# simulator had numerous protocol-level divergences from Showdown across many subsystems, preventing full equivalence test passes. Issues included incorrect action ordering, wrong protocol formatting (status abbreviations, stat names, item/side names), broken event return semantics, incorrect damage attribution, and flawed choice replay logic.

## Root Cause
This was a broad collection of porting bugs accumulated during the initial TypeScript-to-C# translation. Key categories included:
- **Action ordering:** `Order` fields used `int.MaxValue` instead of Showdown's explicit order constants (2, 4, 300, etc.), and `RunSwitchAction`/`PokemonAction` did not propagate order values.
- **Protocol formatting:** Status conditions used enum `ToString()` (e.g., `Burn`) instead of Showdown abbreviations (`brn`); stat names used full words instead of abbreviations (`atk`, `def`, `spa`, `spd`, `spe`); `Item.FullName` was `"Item: X"` instead of `"item: X"`; side display format missed the side name.
- **Event semantics:** `SingleEvent` returned `null` instead of `relayVar` when handler returned `undefined`; `OnTryHit` did not map `null` returns to `NullRelayVar` (NOT_FAIL); Protect's `OnTryHit` returned `Empty` instead of `null`.
- **Damage/healing attribution:** `PrintDamageMessage` accepted only `Condition` instead of `IEffect`, losing item/ability attribution; move heals (Strength Sap) incorrectly showed `[from]` tags.
- **Choice replay:** The equivalence test runner fed choices linearly instead of matching them to the requesting side, breaking on mid-turn forced switches.
- **Miscellaneous:** `critRatio` defaulted to 0 instead of 1; `Pokemon.ToString()` used `IsActive` instead of `Position >= 0`; `InitEffectState` incorrectly set the source as the target; form change HP preservation didn't maintain ratios; `ClearChoice()` not called at start of `Choose()`.

## Fix
Applied fixes across 25+ files touching action ordering, protocol formatting, event dispatch semantics, damage/heal logging, choice replay logic, and numerous smaller corrections. Also added infrastructure improvements: `PokemonSlot.Equals`/`GetHashCode`, `GetProtocolEffectName` helper, proper Showdown protocol line filtering, RNG call counting, and `DisplayName` on `PokemonDetails`.

## Files Changed
- `ApogeeVGC/Sim/Actions/BeforeTurnAction.cs` — Set `Order` to 4
- `ApogeeVGC/Sim/Actions/FieldAction.cs` — Make `Order` settable, default to `false`
- `ApogeeVGC/Sim/Actions/PokemonAction.cs` — Make `Order` settable, default to `false`
- `ApogeeVGC/Sim/Actions/ResidualAction.cs` — Set `Order` to 300
- `ApogeeVGC/Sim/Actions/RunSwitchAction.cs` — Make `Order` settable, default to `false`
- `ApogeeVGC/Sim/Actions/StartGameAction.cs` — Set `Order` to 2
- `ApogeeVGC/Sim/BattleClasses/Battle.Combat.cs` — Track `effectForLogging` separately from `effectCondition` for proper attribution
- `ApogeeVGC/Sim/BattleClasses/Battle.Core.cs` — Fix `InitEffectState` not setting source as target
- `ApogeeVGC/Sim/BattleClasses/Battle.Events.cs` — Fix `SingleEvent` to return `relayVar` instead of `null` for undefined handler returns
- `ApogeeVGC/Sim/BattleClasses/Battle.Fainting.cs` — Remove premature `CheckFainted`/`FlushEvents` calls
- `ApogeeVGC/Sim/BattleClasses/Battle.Lifecycle.cs` — Defer faint replacement switches until queue is empty
- `ApogeeVGC/Sim/BattleClasses/Battle.Logging.cs` — Add `GetProtocolEffectName`, fix side/effect formatting, fix heal/damage attribution
- `ApogeeVGC/Sim/BattleClasses/Battle.Sorting.cs` — Fix `CalculateDefaultSubOrder` for conditions, add `RunSwitchAction` speed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.Damage.cs` — Fix default `critRatio` from 0 to 1
- `ApogeeVGC/Sim/BattleClasses/BattleActions.HitSteps.cs` — Handle `NullRelayVar` in TryHit result conversion
- `ApogeeVGC/Sim/BattleClasses/BattleActions.MoveHit.cs` — Fix spread move slot formatting
- `ApogeeVGC/Sim/BattleClasses/BattleActions.Moves.cs` — Move log message before `getMoveTargets`
- `ApogeeVGC/Sim/BattleClasses/BattleActions.ResultCombining.cs` — Fix priority comparison direction
- `ApogeeVGC/Sim/BattleClasses/BattleActions.Switch.cs` — Check `Fainted` flag in unfainted active check
- `ApogeeVGC/Sim/BattleClasses/BattleQueue.cs` — Propagate order to `RunSwitchAction` and `PokemonAction`
- `ApogeeVGC/Sim/Items/Item.Core.cs` — Fix `FullName` casing to `"item: X"`
- `ApogeeVGC/Sim/PokemonClasses/Pokemon.Core.cs` — Fix `ToString()`, status abbreviations, `DisplayName`, form details
- `ApogeeVGC/Sim/PokemonClasses/Pokemon.Helpers.cs` — Add `DisplayName` to `PokemonDetails`
- `ApogeeVGC/Sim/PokemonClasses/Pokemon.Status.cs` — Use `Battle.Effect` instead of `Battle.Event.Effect` for sourceEffect
- `ApogeeVGC/Sim/PokemonClasses/Pokemon.Transformation.cs` — Preserve HP ratio on form change
- `ApogeeVGC/Sim/PokemonClasses/PokemonSlot.cs` — Add `Equals`/`GetHashCode`
- `ApogeeVGC/Sim/SideClasses/Side.Choices.cs` — Add `ClearChoice()` at start of `Choose()`
- `ApogeeVGC/Sim/SideClasses/Side.Conditions.cs` — Pass side as target in `InitEffectState`
- `ApogeeVGC/Sim/Utils/Extensions/StatIdTools.cs` — Use Showdown stat abbreviations
- `ApogeeVGC/Sim/Utils/Gen5Rng.cs` — Add `CallCount` and `TraceEnabled` for debugging
- `ApogeeVGC/Sim/Utils/Prng.cs` — Expose `Gen5` property for debugging
- `ApogeeVGC/Sim/Core/Driver.EquivalenceTest.cs` — Rewrite choice replay loop, improve log filtering and diagnostics
- `ApogeeVGC/Sim/Events/EventHandlerInfoMapper.cs` — Add `Eat`/`Use` event mappings for items
- `ApogeeVGC/Sim/Events/Handlers/EventMethods/OnTryHitEventInfo.cs` — Map `null` handler return to `NullRelayVar`
- `ApogeeVGC/Data/Conditions/ConditionsPQR.cs` — Fix Protect `OnTryHit` param order and return value

## Pattern
Accumulated translation debt from a large JS-to-C# port. Many individual issues (ordering constants, formatting conventions, null semantics, event return values) that each caused small protocol divergences. Systematic equivalence testing against Showdown's protocol output is the only reliable way to catch these.
