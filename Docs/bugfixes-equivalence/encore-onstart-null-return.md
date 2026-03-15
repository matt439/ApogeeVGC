# Encore OnStart Returning Null (Failure) Instead of Void (Success)

## Summary

Encore's `OnStart` handler returned `null`, which `AddVolatile` interprets as failure, immediately removing the volatile. Showdown's `singleEvent` defaults an undefined `relayVar` to `true`, so an implicit undefined return means success.

## Root Cause

The C# `OnStart` handler for Encore returned `null` after performing its setup logic. In `AddVolatile`, the return value of the `OnStart` single event determines whether the volatile is kept or removed. A `null` return was treated as failure, causing Encore to be immediately removed after being added.

## Fix

Changed the return to `RelayVar.FromVoid()`, which matches the `_ => true` catch-all in `AddVolatile`, correctly indicating success.

## Commit

`f3a174d3` — Fix Encore OnStart returning null (failure) instead of void (success)

## Files Changed

- `ApogeeVGC/Data/Conditions/ConditionsDEF.cs`
