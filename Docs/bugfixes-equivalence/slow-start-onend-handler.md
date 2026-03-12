# Slow Start Missing OnEnd Handler for Faint

**Commit:** `409e38f6`, `7d327a16`
**Date:** 2026-03-12

## Problem
When a Pokemon with Slow Start fainted, the `|-end|Slow Start|[silent]` protocol message was not emitted, causing equivalence test divergence. Initially the OnEnd handler was removed based on a misreading of Showdown's source, then it was added back with corrected behavior.

## Root Cause
The first commit (409e38f6) incorrectly removed the OnEnd handler entirely, based on the belief that Showdown's Slow Start has no onEnd. However, Showdown's compiled distribution does include an onEnd handler that emits `|-end|...|[silent]` when the ability ends (e.g., on faint). The second commit (7d327a16) restored the handler with correct logic, including a guard against `BeingCalledBack` to avoid spurious messages on switch-out.

## Fix
Restored the `OnEnd` handler for Slow Start that emits `|-end|Slow Start|[silent]` when the Pokemon is not being called back (i.e., on faint or ability suppression, not on voluntary switch).

## Files Changed
- `ApogeeVGC/Data/Abilities/AbilitiesSTU.cs` — removed then re-added OnEnd handler with BeingCalledBack guard

## Pattern
Showdown source vs compiled distribution mismatch: The Showdown TypeScript source may not match the compiled/distributed JavaScript. Always verify behavior against the actual running code or protocol logs, not just the source files.
