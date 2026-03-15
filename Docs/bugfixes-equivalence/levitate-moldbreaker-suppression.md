# Levitate Not Checking Mold Breaker Suppression in IsGrounded

**Commit:** `beea2112`
**Date:** 2026-03-13

## Problem
Pokemon with the Levitate ability were incorrectly treated as ungrounded even when the attacking Pokemon had Mold Breaker (or similar ability-suppressing abilities), causing Ground-type moves to miss when they should have hit.

## Root Cause
Showdown's `isGrounded()` checks both `hasAbility('levitate')` and `!battle.suppressingAbility(this)` before granting Levitate's ground immunity. The C# `IsGrounded` method only checked `HasAbility(AbilityId.Levitate)` without verifying whether the ability was being suppressed by the attacker's Mold Breaker, Teravolt, or Turboblaze. This meant Levitate always provided immunity regardless of the attacking context.

## Fix
Added `&& !Battle.SuppressingAbility(this)` to the Levitate check in `IsGrounded`, so Levitate immunity is only granted when the ability is not being suppressed.

## Files Changed
- `ApogeeVGC/Sim/PokemonClasses/Pokemon.Immunity.cs` -- Added SuppressingAbility check to Levitate branch in IsGrounded

## Pattern
Missing ability suppression check, causing an ability to activate when it should be negated by the opponent's ability.
