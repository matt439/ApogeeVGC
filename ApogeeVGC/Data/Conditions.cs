﻿using System.Collections.ObjectModel;
using System.Transactions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Ui;

namespace ApogeeVGC.Data;

public record Conditions
{
    public IReadOnlyDictionary<ConditionId, Condition> ConditionsData { get; }

    public Conditions()
    {
        ConditionsData = new ReadOnlyDictionary<ConditionId, Condition>(_conditions);
    }

    private readonly Dictionary<ConditionId, Condition> _conditions = new()
    {
        [ConditionId.Burn] = new Condition
        {
            Id = ConditionId.Burn,
            Name = "Burn",
            ConditionEffectType = ConditionEffectType.Status,
            ConditionVolatility = ConditionVolatility.NonVolatile,
            OnStart = (target, _, sourceEffect, context) =>
            {
                if (sourceEffect is null)
                {
                    throw new ArgumentNullException($"Source effect is null when trying to apply" +
                                                    $"{ConditionId.Burn} to" + $"pokemon {target.Name}.");
                }

                bool debug = context.PrintDebug;

                switch (sourceEffect.EffectType)
                {
                    case EffectType.Item:
                        if (sourceEffect is Item { Name: "Flame Orb" } && debug)
                        {
                            UiGenerator.PrintBurnStartFromFlameOrb(target);
                        }
                        break;
                    case EffectType.Ability:
                        if (sourceEffect is Ability ability && debug)
                        {
                            UiGenerator.PrintBurnStartFromAbility(target, ability);
                        }
                        break;
                    case EffectType.Move:
                        if (debug)
                        {
                            UiGenerator.PrintBurnStart(target);
                        }
                        break;
                    case EffectType.Specie:
                    case EffectType.Condition:
                    case EffectType.Format:
                        throw new InvalidOperationException($"Effect type {sourceEffect.EffectType} cannot" +
                                                            $"apply {ConditionId.Burn} to" +
                                                            $"pokemon {target.Name}.");
                    default:
                        throw new InvalidOperationException($"Unknown effect type {sourceEffect.EffectType}" +
                                                            $"when trying to apply {ConditionId.Burn} to" +
                                                            $"pokemon {target.Name}.");
                }
                return true;
            },
            OnResidualOrder = 10,
            OnResidual = (target, _, _, context) =>
            {
                int damage = target.UnmodifiedHp / 16;
                if (damage < 1) damage = 1;
                int actualDamage = target.Damage(damage);
                if (context.PrintDebug)
                { 
                    UiGenerator.PrintBurnDamage(target, actualDamage);
                }
            },
        },
        [ConditionId.Paralysis] = new Condition
        {
            Id = ConditionId.Paralysis,
            Name = "Paralysis",
            ConditionEffectType = ConditionEffectType.Status,
            ConditionVolatility = ConditionVolatility.NonVolatile,
            OnStart = (target, _, sourceEffect, context) =>
            {
                if (sourceEffect is null)
                {
                    throw new ArgumentNullException($"Source effect is null when trying to apply" +
                                                    $"{ConditionId.Paralysis} to" + $"pokemon {target.Name}.");
                }

                bool debug = context.PrintDebug;

                switch (sourceEffect.EffectType)
                {
                    case EffectType.Ability:
                        if (sourceEffect is Ability ability && debug)
                        {
                            UiGenerator.PrintParalysisStartFromAbility(target, ability);
                        }
                        break;
                    case EffectType.Move:
                        if (debug)
                        {
                            UiGenerator.PrintParalysisStart(target);
                        }
                        break;
                    case EffectType.Specie:
                    case EffectType.Condition:
                    case EffectType.Format:
                    case EffectType.Item:
                        throw new InvalidOperationException($"Effect type {sourceEffect.EffectType} cannot" +
                                                            $"apply {ConditionId.Paralysis} to" +
                                                            $"pokemon {target.Name}.");
                    default:
                        throw new InvalidOperationException($"Unknown effect type {sourceEffect.EffectType}" +
                                                            $"when trying to apply {ConditionId.Paralysis} to" +
                                                            $"pokemon {target.Name}.");
                }

                return true;
            },
            OnModifySpePriority = -101,
            // Quick Feet negates the speed drop from paralysis
            // It also increases speed by 50% when the pokemon is burned, paralyzed or poisoned
            // but that is handled in the Ability effects.
            OnModifySpe = (pokemon) => pokemon.Ability.Id == AbilityId.QuickFeet ? 1.0 : 0.5,
            OnBeforeMovePriority = 1,
            OnBeforeMove = (source, _, _, context) =>
            {
                bool paralyzed = context.Random.NextDouble() < 0.25;
                if (paralyzed && context.PrintDebug)
                {
                    UiGenerator.PrintParalysisPrevention(source);
                }
                return !paralyzed;
            },

        },
        [ConditionId.Sleep] = new Condition
        {
            Id = ConditionId.Sleep,
            Name = "Sleep",
            ConditionVolatility = ConditionVolatility.NonVolatile,
        },
        [ConditionId.Freeze] = new Condition
        {
            Id = ConditionId.Freeze,
            Name = "Freeze",
            ConditionVolatility = ConditionVolatility.NonVolatile,
        },
        [ConditionId.Poison] = new Condition
        {
            Id = ConditionId.Poison,
            Name = "Poison",
            ConditionVolatility = ConditionVolatility.NonVolatile,
        },
        [ConditionId.Toxic] = new Condition
        {
            Id = ConditionId.Toxic,
            Name = "Toxic",
            ConditionVolatility = ConditionVolatility.NonVolatile,
        },
        [ConditionId.Confusion] = new Condition
        {
            Id = ConditionId.Confusion,
            Name = "Confusion",
            ConditionVolatility = ConditionVolatility.Volatile,
        },
        [ConditionId.Flinch] = new Condition
        {
            Id = ConditionId.Flinch,
            Name = "Flinch",
            ConditionEffectType = ConditionEffectType.Condition,
            ConditionVolatility = ConditionVolatility.Volatile,
            Duration = 1,
            OnBeforeMovePriority = 8,
            OnBeforeMove = (source, _, _, context) =>
            {
                if (context.PrintDebug)
                {
                    UiGenerator.PrintFlinch(source);
                }
                return false;
            },
        },
        [ConditionId.ChoiceLock] = new Condition
        {
            Id = ConditionId.ChoiceLock,
            NoCopy = true,
            ConditionEffectType = ConditionEffectType.Condition,
            ConditionVolatility = ConditionVolatility.Volatile,
            // TODO: Check if hasBounced and snatch effects need to be handled
            OnStart = (_, _, _, _) => true,
            OnBeforeMove = (source, _, move, _) =>
            {
                if (!source.Item?.IsChoice ?? false)
                {
                    // If the source no longer has a choice item, remove the condition
                    if (!source.RemoveCondition(ConditionId.ChoiceLock))
                    {
                        throw new InvalidOperationException("Failed to remove Choice Lock condition.");
                    }
                    return true;
                }

                if (source.IgnoringItem) return true;
                if (source.LastMoveUsed is null) return true;
                if (move.Id == source.LastMoveUsed.Id) return true;
                if (move.Id == MoveId.Struggle) return true;

                return false;
            },
            OnDisableMove = (pokeon, usedMove, _) =>
            {
                if (!pokeon.Item?.IsChoice ?? false)
                {
                    // If the source no longer has a choice item, remove the condition
                    if (!pokeon.RemoveCondition(ConditionId.ChoiceLock))
                    {
                        throw new InvalidOperationException("Failed to remove Choice Lock condition.");
                    }
                    return;
                }
                if (pokeon.IgnoringItem) return;
                foreach (Move move in pokeon.Moves)
                {
                    if (move.Id != usedMove.Id)
                    {
                        move.Disabled = true;
                    }
                }
            },
        },
        [ConditionId.LeechSeed] = new Condition
        {
            Id = ConditionId.LeechSeed,
            Name = "Leech Seed",
            ConditionEffectType = ConditionEffectType.Status,
            ConditionVolatility = ConditionVolatility.Volatile,
            //OnStart = (target, _, _, context) =>
            //{
            //    if (context.PrintDebug)
            //    {
            //        UiGenerator.PrintLeechSeedStart(target);
            //    }
            //    return true;
            //},
            //OnResidualOrder = 8,
            //OnResidual = (target, source, _, context) =>
            //{
            //    if (target.CurrentHp <= 0)
            //    {
            //        //throw new InvalidOperationException("Target has fainted.");

            //        // If the target has fainted, do not apply Leech Seed damage.
            //        return;
            //    }

            //    int damage = target.UnmodifiedHp / 8;
            //    if (damage < 1) damage = 1;

            //    int actualDamage = target.Damage(damage);

            //    if (source is not null)
            //    {
            //        int actualHeal = source.Team.ActivePokemon.Heal(damage);
            //        if (context.PrintDebug)
            //        {
            //            UiGenerator.PrintLeechSeedDamage(target, actualDamage, source.Team.ActivePokemon,
            //                actualHeal);
            //        }
            //    }
            //    else
            //    {
            //        throw new ArgumentNullException($"Source is null when trying to apply" +
            //                                        $"{ConditionId.LeechSeed} damage to" + $"pokemon {target.Name}.");
            //    }
            //},
        },
        // This condition is the effect placed on each Pokemon by Trick Room.
        // Trick Room itself is a PseudoWeather condition on the field which applies
        // this condition to each Pokemon.
        // This condition doesn't have apply any effect to the Pokemon itself but could
        // be useful for UI etc.
        [ConditionId.TrickRoom] = new Condition
        {
            Id = ConditionId.TrickRoom,
            Name = "Trick Room",
            ConditionEffectType = ConditionEffectType.PseudoWeather,
            ConditionVolatility = ConditionVolatility.Volatile,
        },
        [ConditionId.Stall] = new Condition
        {
            Id = ConditionId.Stall,
            Name = "Stall",
            Duration = 2,
            CounterMax = 729,
            ConditionEffectType = ConditionEffectType.Condition,
            ConditionVolatility = ConditionVolatility.Volatile,
            OnStart = (target, _, _, _) =>
            {
                Condition? condition = target.GetCondition(ConditionId.Stall);
                if (condition is null)
                {
                    throw new NullReferenceException($"Condition {ConditionId.Stall} not found on" +
                                                     $"pokemon {target.Name}.");
                }

                condition.Counter = 3;
                return true;
            },
            OnStallMove = (pokemon, context) =>
            {
                Condition? condition = pokemon.GetCondition(ConditionId.Stall);
                if (condition is null)
                {
                    throw new NullReferenceException($"Condition {ConditionId.Stall} not found on" +
                                                     $"pokemon {pokemon.Name}.");
                }

                int counter = condition.Counter ?? 1;
                double successChance = 1.0 / counter;
                bool success = context.Random.NextDouble() < successChance;

                //success = false; // For testing purposes only, force stall to always fail.

                if (success) return success;

                if (!pokemon.RemoveCondition(ConditionId.Stall))
                {
                    throw new InvalidOperationException("Failed to remove Stall condition.");
                }
                return success;
            },
            OnRestart = (target, _, _, _) =>
            {
                Condition? condition = target.GetCondition(ConditionId.Stall);
                if (condition is null)
                {
                    throw new NullReferenceException($"Condition {ConditionId.Stall} not found on" +
                                                     $"pokemon {target.Name}.");
                }

                int counter = condition.Counter ?? 1;
                int counterMax = condition.CounterMax ?? 729;

                if (counter < counterMax)
                {
                    condition.Counter = counter * 3;
                }

                condition.Duration = 2;

                return true;
            },
        },
        [ConditionId.Protect] = new Condition
        {
            Id = ConditionId.Protect,
            Name = "Protect",
            Duration = 1,
            ConditionEffectType = ConditionEffectType.Condition,
            ConditionVolatility = ConditionVolatility.Volatile,
            // OnStart
            OnTryHitPriority = 3,
            // TODO: Check for smart target (dragon darts), outrage lock
            OnTryHit = (_, _, move, _) => !(move.Flags.Protect ?? false),
            //OnTurnEnd = (target, _) =>
            //{
            //    if (!target.RemoveCondition(ConditionId.Protect))
            //    {
            //        throw new InvalidOperationException("Failed to remove Protect condition.");
            //    }
            //},
        },
        [ConditionId.Tailwind] = new Condition
        {
            Id = ConditionId.Tailwind,
            Name = "Tailwind",
            ConditionEffectType = ConditionEffectType.SideCondition,
            ConditionVolatility = ConditionVolatility.Volatile,
            OnModifySpe = (_) => 2.0,
        },
        [ConditionId.Reflect] = new Condition
        {
            Id = ConditionId.Reflect,
            Name = "Reflect",
            ConditionEffectType = ConditionEffectType.SideCondition,
            ConditionVolatility = ConditionVolatility.Volatile,
            OnAnyModifyDamage = (_, source, target, move, isCrit, numPokemonSide) =>
            {
                if (target == source || move.Category != MoveCategory.Physical) return 1.0;
                if (isCrit || move.Infiltrates) return 1.0;
                if (numPokemonSide > 1) // 2 pokemon on the side
                {
                    return 2732 / 4096.0;
                }
                return 0.5;
            },
        },
        [ConditionId.LightScreen] = new Condition
        {
            Id = ConditionId.LightScreen,
            Name = "Light Screen",
            ConditionEffectType = ConditionEffectType.SideCondition,
            ConditionVolatility = ConditionVolatility.Volatile,
            OnAnyModifyDamage = (_, source, target, move, isCrit, numPokemonSide) =>
            {
                if (target == source || move.Category != MoveCategory.Special) return 1.0;
                if (isCrit || move.Infiltrates) return 1.0;
                if (numPokemonSide > 1) // 2 pokemon on the side
                {
                    return 2732 / 4096.0;
                }
                return 0.5;
            },
        },
        [ConditionId.Leftovers] = new Condition
        {
            Id = ConditionId.Leftovers,
            Name = "Leftovers",
            ConditionEffectType = ConditionEffectType.Condition,
            ConditionVolatility = ConditionVolatility.Volatile,
            OnResidualOrder = 5,
            OnResidualSubOrder = 4,
            Duration = 1, // Condition is refreshed each turn by the item effect
            OnResidual = (target, _, _, context) =>
            {
                int heal = target.UnmodifiedHp / 16;
                if (heal < 1) heal = 1;
                int actualHeal = target.Heal(heal);
                if (context.PrintDebug)
                {
                    UiGenerator.PrintLeftoversHeal(target, actualHeal);
                }
            },
        },
        [ConditionId.ChoiceSpecs] = new Condition
        {
            Id = ConditionId.ChoiceSpecs,
            Name = "Choice Specs",
            ConditionEffectType = ConditionEffectType.Condition,
            ConditionVolatility = ConditionVolatility.Volatile,
            OnModifySpAPriority = 1,
            OnModifySpA = (_) => 1.5,
        },
        [ConditionId.FlameOrb] = new Condition
        {
            Id = ConditionId.FlameOrb,
            Name = "Flame Orb",
            ConditionEffectType = ConditionEffectType.Condition,
            ConditionVolatility = ConditionVolatility.Volatile,
            OnResidualOrder = 28,
            OnResidualSubOrder = 3,
            Duration = 1,
            OnResidual = (target, _, _, context) =>
            {
                if (target.HasCondition(ConditionId.Burn)) return;
                target.AddCondition(context.Library.Conditions[ConditionId.Burn], context, target,
                    target.Item ?? throw new InvalidOperationException("The target should always be holding" +
                                                                       "a flame orb here"));
            },
        },
        [ConditionId.AssaultVest] = new Condition
        {
            Id = ConditionId.AssaultVest,
            Name = "Assault Vest",
            ConditionEffectType = ConditionEffectType.Condition,
            ConditionVolatility = ConditionVolatility.Volatile,
            OnModifySpDPriority = 1,
            OnModifySpD = (_) => 1.5,
            OnDisableMove = (pokemon, _, _) =>
            {
                // disable all moves which are status type (except me first)
                foreach (Move move in pokemon.Moves)
                {
                    if (move.Category == MoveCategory.Status && move.Id != MoveId.MeFirst)
                    {
                        move.Disabled = true;
                    }
                }
            },
            OnStart = (pokemon, _, _, _) =>
            {
                // disable all moves which are status type (except me first)
                foreach (Move move in pokemon.Moves)
                {
                    if (move.Category == MoveCategory.Status && move.Id != MoveId.MeFirst)
                    {
                        move.Disabled = true;
                    }
                }
                return true;
            },
        },
        [ConditionId.ElectricTerrain] = new Condition
        {
            Id = ConditionId.ElectricTerrain,
            Name = "Electric Terrain",
            ConditionEffectType = ConditionEffectType.Terrain,
            ConditionVolatility = ConditionVolatility.Volatile,
        },
        [ConditionId.HadronEngine] = new Condition
        {
            Id = ConditionId.HadronEngine,
            Name = "Hadron Engine",
            ConditionEffectType = ConditionEffectType.Condition,
            ConditionVolatility = ConditionVolatility.Volatile,
            OnModifySpAPriority = 5,
            OnModifySpA = (pokemon) => pokemon.HasCondition(ConditionId.ElectricTerrain) ? 5461.0 / 4096.0 : 1.0,
        },
        [ConditionId.Guts] = new Condition
        {
            Id = ConditionId.Guts,
            Name = "Guts",
            ConditionEffectType = ConditionEffectType.Condition,
            ConditionVolatility = ConditionVolatility.Volatile,
            OnModifyAtkPriority = 5,
            OnModifyAtk = (pokemon) =>
            {
                if (pokemon.HasCondition(ConditionId.Burn) ||
                    pokemon.HasCondition(ConditionId.Paralysis) ||
                    pokemon.HasCondition(ConditionId.Poison) ||
                    pokemon.HasCondition(ConditionId.Toxic))
                {
                    return 1.5;
                }
                return 1.0;
            },
        },
        [ConditionId.FlameBody] = new Condition
        {
            Id = ConditionId.FlameBody,
            Name = "Flame Body",
            ConditionEffectType = ConditionEffectType.Condition,
            ConditionVolatility = ConditionVolatility.Volatile,
            OnDamagingHit = (_, target, source, move, context) =>
            {
                if (!(move.Flags.Contact ?? false)) return;

                if (source.HasCondition(ConditionId.Burn) ||
                    source.HasCondition(ConditionId.Freeze) ||
                    source.HasCondition(ConditionId.Sleep) ||
                    source.HasCondition(ConditionId.Paralysis) ||
                    source.HasCondition(ConditionId.Poison) ||
                    source.HasCondition(ConditionId.Toxic)) return;

                bool burned = context.Random.NextDouble() < 0.3;

                if (!burned) return;

                source.AddCondition(context.Library.Conditions[ConditionId.Burn], context, target,
                    target.Ability ?? throw new InvalidOperationException("The target should always have" +
                                                                          "flame body ability here"));
                //if (context.PrintDebug)
                //{
                //    UiGenerator.PrintFlameBodyBurn(source, target);
                //}
            },
        },
        [ConditionId.QuarkDrive] = new Condition
        {
            Id = ConditionId.QuarkDrive,
            Name = "Quark Drive",
            ConditionEffectType = ConditionEffectType.Condition,
            ConditionVolatility = ConditionVolatility.Volatile,
            NoCopy = true,
            OnStart = (pokemon, _, _, context) =>
            {
                pokemon.BestStat = pokemon.GetBestStat(false, true);
                if (context.PrintDebug)
                {
                    UiGenerator.PrintQuarkDriveStart(pokemon, pokemon.BestStat ??
                                                              throw new InvalidOperationException());
                }
                return true;
            },
            OnModifyAtkPriority = 5,
            OnModifyAtk = (pokemon) =>
            {
                if (pokemon.BestStat is null) return 1.0;
                if (pokemon.BestStat != StatIdExceptHp.Atk) return 1.0;
                return pokemon.HasCondition(ConditionId.ElectricTerrain) ? 5325.0 / 4096.0 : 1.0;
            },
            OnModifyDefPriority = 6,
            OnModifyDef = (pokemon) =>
            {
                if (pokemon.BestStat is null) return 1.0;
                if (pokemon.BestStat != StatIdExceptHp.Def) return 1.0;
                return pokemon.HasCondition(ConditionId.ElectricTerrain) ? 5325.0 / 4096.0 : 1.0;
            },
            OnModifySpAPriority = 5,
            OnModifySpA = (pokemon) =>
            {
                if (pokemon.BestStat is null) return 1.0;
                if (pokemon.BestStat != StatIdExceptHp.SpA) return 1.0;
                return pokemon.HasCondition(ConditionId.ElectricTerrain) ? 5325.0 / 4096.0 : 1.0;
            },
            OnModifySpDPriority = 6,
            OnModifySpD = (pokemon) =>
            {
                if (pokemon.BestStat is null) return 1.0;
                if (pokemon.BestStat != StatIdExceptHp.SpD) return 1.0;
                return pokemon.HasCondition(ConditionId.ElectricTerrain) ? 5325.0 / 4096.0 : 1.0;
            },
            OnModifySpe = (pokemon) =>
            {
                if (pokemon.BestStat is null) return 1.0;
                if (pokemon.BestStat != StatIdExceptHp.Spe) return 1.0;
                return pokemon.HasCondition(ConditionId.ElectricTerrain) ? 1.5 : 1.0;
            },
        },
    };
}