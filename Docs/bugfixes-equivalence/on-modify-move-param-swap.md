# OnModifyMove Source/Target Parameter Swap and Debug Cleanup

**Commit:** `6fa8aad6`
**Date:** 2026-03-11

## Problem
The equivalence test batch runner could deadlock when capturing Showdown process output, and leftover debug code (Press Enter prompt, synchronous stdout/stderr reads) made the batch runner unreliable.

## Root Cause
The Showdown process runner used synchronous `ReadToEnd()` on both stdout and stderr before `WaitForExit()`. If the process wrote enough to fill the output buffer, it would block waiting for the reader, while the reader was waiting for the process to exit -- a classic deadlock. Additionally, a leftover `Console.ReadLine()` prompt blocked automated batch runs.

## Fix
Changed stdout/stderr reading to asynchronous (`ReadToEndAsync()`) with `WaitForExit()` called first, and added a timeout with process kill if it hangs. Removed the interactive "Press Enter" prompt.

## Files Changed
- `ApogeeVGC/Sim/Core/Driver.EquivalenceBatch.cs` — Async stdout/stderr reads to prevent deadlock, remove interactive prompt, add timeout handling

## Pattern
Synchronous stdout/stderr capture deadlock. When a child process writes to both stdout and stderr, reading one synchronously can block if the other's buffer fills. Always read both streams asynchronously before waiting for process exit.
