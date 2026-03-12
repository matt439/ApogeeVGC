# Substitute Not Blocking Status Moves Through TryPrimaryHit

**Commit:** `bf854d42`
**Date:** 2026-03-12

## Problem
Substitute was not blocking status moves (Yawn, Encore, etc.) that target the Pokemon behind the substitute. These moves were executing successfully despite the substitute, diverging from Showdown.

## Root Cause
When Substitute's TryPrimaryHit handler returned `null` (meaning "blocked, but not a failure"), the handler wrapper passed through C# `null`, and the move-hit code treated `null` as "no handler fired, continue." In Showdown, `null` is falsy and causes the move to be blocked without logging a failure message. The distinction between "no handler ran" (C# `null`) and "handler returned null" (JS falsy) was lost.

## Fix
Updated `OnTryPrimaryHitEventInfo` to convert a handler's `null` return to `NullRelayVar`, and updated the TryPrimaryHit result interpretation in `BattleActions.MoveHit.cs` to treat `NullRelayVar` as blocked (falsy) while keeping C# `null` as "no handler fired" (continue).

## Files Changed
- `ApogeeVGC/Sim/Events/Handlers/EventMethods/OnTryPrimaryHitEventInfo.cs` -- convert null handler result to NullRelayVar
- `ApogeeVGC/Sim/BattleClasses/BattleActions.MoveHit.cs` -- treat NullRelayVar as blocked in TryPrimaryHit result handling

## Pattern
JS null vs C# null semantics: distinguishing "no handler ran" from "handler explicitly returned null (falsy/blocking)" requires an explicit NullRelayVar wrapper in C# since both map to null without it.
