# MoveFlags Shared Mutation via ShallowClone Leaking to Library Templates

**Commit:** `19780c09`
**Date:** 2026-03-13

## Problem
Moves that modify their `Flags` at runtime (e.g., Shell Side Arm toggling `Flags.Contact`) were permanently mutating the shared library template's `MoveFlags` instance, causing flag contamination that persisted across battles.

## Root Cause
`ActiveMove.ShallowClone()` used `MemberwiseClone`, which copies object references rather than creating new instances. Since `MoveFlags` is a reference type (record), the cloned `ActiveMove` shared the same `MoveFlags` object as the library template. When a move like Shell Side Arm set `Flags.Contact = true` on the active move, it mutated the library template's flags directly. Similarly, `ResetFromTemplate` assigned the template's `Flags` reference without copying, so even moves reset between uses would carry the contaminated flags forward.

## Fix
Added `clone.Flags = clone.Flags with { }` in `ShallowClone` and changed `ResetFromTemplate` to use `template.Flags with { }` (record copy expression), ensuring each `ActiveMove` gets its own independent `MoveFlags` instance.

## Files Changed
- `ApogeeVGC/Sim/Moves/ActiveMove.cs` -- Deep-copy Flags in ShallowClone and ResetFromTemplate using record `with` expression

## Pattern
Shallow copy aliasing a mutable reference type, causing cross-instance state contamination.
