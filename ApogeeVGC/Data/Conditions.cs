using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Ui;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Data;

//public enum SpecialImmunityId
//{
//    Prankster,
//    Paralysis,
//    Burn,
//    Trapped,
//    Powder,
//    Sandstorm,
//    Hail,
//    Freeze,
//    Poison,
//    Toxic,
//}

public record Conditions
{
    public IReadOnlyDictionary<ConditionId, Condition> ConditionsData { get; }
    private readonly Library _library;

    public Conditions(Library library)
    {
        _library = library;
        ConditionsData = new ReadOnlyDictionary<ConditionId, Condition>(CreateConditions());
    }

    private Dictionary<ConditionId, Condition> CreateConditions()
    {
        return new Dictionary<ConditionId, Condition>
        {
            [ConditionId.Burn] = new()
            {
                Id = ConditionId.Burn,
                Name = "Burn",
                ConditionEffectType = ConditionEffectType.Status,
                ImmuneTypes = [PokemonType.Fire],
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (sourceEffect is null)
                    {
                        throw new ArgumentNullException($"Source effect is null when trying to apply" +
                                                        $"{ConditionId.Burn} to" + $"pokemon {target.Name}.");
                    }

                    bool debug = battle.PrintDebug;
                    if (!debug) return new VoidReturn();

                    Condition burn = _library.Conditions[ConditionId.Burn];

                    switch (sourceEffect.EffectType)
                    {
                        case EffectType.Item:
                            if (sourceEffect is Item { Id: ItemId.FlameOrb })
                            {
                                UiGenerator.PrintStatusEvent(target, burn, sourceEffect);
                            }
                            break;
                        case EffectType.Ability:
                            if (sourceEffect is Ability ability)
                            {
                                UiGenerator.PrintStatusEvent(target, burn, ability, source);
                            }
                            break;
                        case EffectType.Move:
                            UiGenerator.PrintStatusEvent(target, burn);
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
                    return new VoidReturn();
                },
                OnResidualOrder = 10,
                OnResidual = (battle, pokemon, _, _) =>
                {
                    battle.Damage(pokemon.BaseMaxHp / 16);
                },
            },
            [ConditionId.Paralysis] = new()
            {
                Id = ConditionId.Paralysis,
                Name = "Paralysis",
                ConditionEffectType = ConditionEffectType.Status,
                ImmuneTypes = [PokemonType.Electric],
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (!battle.PrintDebug) return new VoidReturn();

                    Condition paralysis = _library.Conditions[ConditionId.Paralysis];

                    if (sourceEffect is Ability ability)
                    {
                        UiGenerator.PrintStatusEvent(target, paralysis, ability, source);
                    }
                    else
                    {
                        UiGenerator.PrintStatusEvent(target, paralysis);
                    }
                    return new VoidReturn();
                },
                OnModifySpePriority = -101,
                OnModifySpe = (battle, spe, pokemon) =>
                {
                    spe = battle.FinalModify(spe);
                    if (!pokemon.HasAbility(AbilityId.QuickFeet))
                    {
                        spe = (int)Math.Floor(spe * 50.0 / 100);
                    }
                    return spe;
                },
                OnBeforeMovePriority = 1,
                OnBeforeMove = (battle, pokemon, _, _) =>
                {
                    if (!battle.RandomChance(1, 4)) return new VoidReturn();
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintCantEvent(pokemon, _library.Conditions[ConditionId.Paralysis]);
                    }
                    return false;
                },
            },
            [ConditionId.Sleep] = new()
            {
                Id = ConditionId.Sleep,
                Name = "Sleep",
                ConditionEffectType = ConditionEffectType.Status,
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (battle.PrintDebug)
                    {
                        Condition sleep = _library.Conditions[ConditionId.Sleep];
                        switch (sourceEffect)
                        {
                            case Ability ability:
                                UiGenerator.PrintStatusEvent(target, sleep, ability, source);
                                break;
                            case ActiveMove move:
                                UiGenerator.PrintStatusEvent(target, sleep, move);
                                break;
                            default:
                                UiGenerator.PrintStatusEvent(target, sleep);
                                break;
                        }
                    }

                    battle.EffectState.StartTime = battle.Random(2, 5);
                    battle.EffectState.Time = battle.EffectState.StartTime;

                    Condition nightmare = _library.Conditions[ConditionId.Nightmare];

                    if (!target.RemoveVolatile(nightmare)) return new VoidReturn();

                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintEndEvent(target, nightmare);
                    }
                    return new VoidReturn();
                },
                OnBeforeMovePriority = 10,
                OnBeforeMove = (battle, pokemon, _, move) =>
                {
                    if (pokemon.HasAbility(AbilityId.EarlyBird))
                    {
                        pokemon.StatusState.Time--;
                    }

                    pokemon.StatusState.Time--;

                    if (pokemon.StatusState.Time <= 0)
                    {
                        pokemon.CureStatus();
                        return new VoidReturn();
                    }
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintCantEvent(pokemon, _library.Conditions[ConditionId.Sleep]);
                    }
                    if (move.SleepUsable ?? false)
                    {
                        return new VoidReturn();
                    }
                    return false;
                },
            },
            [ConditionId.Freeze] = new()
            {
                Id = ConditionId.Freeze,
                Name = "Freeze",
                ConditionEffectType = ConditionEffectType.Status,
                ImmuneTypes = [PokemonType.Ice],
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (battle.PrintDebug)
                    {
                        Condition freeze = _library.Conditions[ConditionId.Freeze];
                        switch (sourceEffect)
                        {
                            case Ability ability:
                                UiGenerator.PrintStatusEvent(target, freeze, ability, source);
                                break;
                            default:
                                UiGenerator.PrintStatusEvent(target, freeze);
                                break;
                        }
                    }

                    if (target.Species.Id == SpecieId.ShayminSky &&
                        target.BaseSpecies.BaseSpecies == SpecieId.Shaymin)
                    {
                        target.FormeChange(SpecieId.Shaymin, battle.Effect, true);
                    }
                    return new VoidReturn();
                },
                OnBeforeMovePriority = 10,
                OnBeforeMove = (battle, pokemon, _, move) =>
                {
                    if ((move.Flags.Defrost ?? false) &&
                        !(move.Id == MoveId.BurnUp && !pokemon.HasType(PokemonType.Fire)))
                    {
                        return new VoidReturn();
                    }
                    if (battle.RandomChance(1, 5))
                    {
                        pokemon.CureStatus();
                        return new VoidReturn();
                    }
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintCantEvent(pokemon, _library.Conditions[ConditionId.Freeze]);
                    }
                    return false;
                },
                OnModifyMove = (battle, move, pokemon, _) =>
                {
                    if (!(move.Flags.Defrost ?? false)) return;
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintCureStatusEvent(pokemon, _library.Conditions[ConditionId.Freeze], move);
                    }
                    pokemon.ClearStatus();
                },
                OnAfterMoveSecondary = (_, target, _, move) =>
                {
                    if (move.ThawsTarget ?? false)
                    {
                        target.CureStatus();
                    }
                },
                OnDamagingHit = (_, _, target, _, move) =>
                {
                    if (move.Type == MoveType.Fire && move.Category != MoveCategory.Status &&
                        move.Id != MoveId.PolarFlare)
                    {
                        target.CureStatus();
                    }
                },
            },
            [ConditionId.Poison] = new()
            {
                Id = ConditionId.Poison,
                Name = "Poison",
                ConditionEffectType = ConditionEffectType.Status,
                ImmuneTypes = [PokemonType.Poison, PokemonType.Steel],
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (!battle.PrintDebug) return new VoidReturn();

                    Condition poison = _library.Conditions[ConditionId.Poison];

                    if (sourceEffect.EffectType == EffectType.Ability)
                    {
                        UiGenerator.PrintStatusEvent(target, poison, sourceEffect, source);
                    }
                    else
                    {
                        UiGenerator.PrintStatusEvent(target, poison);
                    }
                    return new VoidReturn();
                },
            },
            [ConditionId.Toxic] = new()
            {
                Id = ConditionId.Toxic,
                Name = "Toxic",
                ConditionEffectType = ConditionEffectType.Status,
                ImmuneTypes = [PokemonType.Poison, PokemonType.Steel],
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    battle.EffectState.Stage = 0;
                    if (!battle.PrintDebug) return new VoidReturn();

                    Condition toxic = _library.Conditions[ConditionId.Toxic];
                    switch (sourceEffect)
                    {
                        case Item { Id: ItemId.ToxicOrb }:
                            UiGenerator.PrintStatusEvent(target, toxic, sourceEffect);
                            break;
                        case Ability ability:
                            UiGenerator.PrintStatusEvent(target, toxic, ability, source);
                            break;
                        default:
                            UiGenerator.PrintStatusEvent(target, toxic);
                            break;
                    }
                    return new VoidReturn();
                },
                OnSwitchIn = (battle, _) =>
                {
                    battle.EffectState.Stage = 0;
                },
                OnResidualOrder = 9,
                OnResidual = (battle, pokemon, _, _) =>
                {
                    if (battle.EffectState.Stage < 15)
                    {
                        battle.EffectState.Stage++;
                    }
                    battle.Damage(battle.ClampIntRange(pokemon.BaseMaxHp / 16, 1, null) *
                                  (battle.EffectState.Stage ?? 0));
                },
            },
            [ConditionId.Confusion] = new()
            {
                Id = ConditionId.Confusion,
                Name = "Confusion",
                ConditionEffectType = ConditionEffectType.Condition,
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (battle.PrintDebug)
                    {
                        Condition confusion = _library.Conditions[ConditionId.Confusion];
                        switch (sourceEffect)
                        {
                            case Condition { Id: ConditionId.LockedMove }:
                                UiGenerator.PrintStartEvent(target, confusion,
                                    _library.Conditions[ConditionId.LockedMove]);
                                break;
                            case Ability ability:
                                UiGenerator.PrintStartEvent(target, confusion, ability,
                                    source);
                                break;
                            default:
                                UiGenerator.PrintStartEvent(target, confusion);
                                break;
                        }
                    }
                    int min = 2;
                    if (sourceEffect is ActiveMove { Id: MoveId.AxeKick })
                    {
                        min = 3;
                    }
                    battle.EffectState.Time = battle.Random(min, 6);
                    return new VoidReturn();
                },
                OnEnd = (battle, target) =>
                {
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintEndEvent(target, _library.Conditions[ConditionId.Confusion]);
                    }
                },
                OnBeforeMovePriority = 3,
                OnBeforeMove = (battle, pokemon, _, _) =>
                {
                    pokemon.Volatiles[ConditionId.Confusion].Time--;
                    if (pokemon.Volatiles[ConditionId.Confusion].Time < 1)
                    {
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.Confusion]);
                        return new VoidReturn();
                    }
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintActivateEvent(pokemon, _library.Conditions[ConditionId.Confusion]);
                    }
                    if (!battle.RandomChance(33, 100))
                    {
                        return new VoidReturn();
                    }
                    battle.ActiveTarget = pokemon;
                    int damage = battle.GetConfusionDamage(pokemon, 40);

                    ActiveMove activeMove = new()
                    {
                        Name = "Confused",
                        Id = MoveId.Confused,
                        Accuracy = IntTrueUnion.FromTrue(),
                        Num = 100200,
                        Type = MoveType.Normal,
                    };
                    battle.Damage(damage, pokemon, pokemon, BattleDamageEffect.FromIEffect(activeMove));
                    return false;
                },
            },
            [ConditionId.Flinch] = new()
            {
                Id = ConditionId.Flinch,
                Name = "Flinch",
                ConditionEffectType = ConditionEffectType.Condition,
                Duration = 1,
                OnBeforeMovePriority = 8,
                OnBeforeMove = (battle, pokemon, _, _) =>
                {
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintCantEvent(pokemon, _library.Conditions[ConditionId.Flinch]);
                    }
                    battle.RunEvent(EventId.Flinch, pokemon);
                    return false;
                },
            },
            [ConditionId.ChoiceLock] = new()
            {
                Id = ConditionId.ChoiceLock,
                NoCopy = true,
                ConditionEffectType = ConditionEffectType.Condition,
                OnStart = (battle, _, _, _) =>
                {
                    if (battle.ActiveMove is null)
                    {
                        throw new InvalidOperationException("Active move is null when trying to apply" +
                                                            $"{ConditionId.ChoiceLock}.");
                    }
                    if (battle.ActiveMove.HasBounced ?? false)
                    {
                        return false;
                    }
                    if (battle.ActiveMove.SourceEffect is MoveEffectStateId { MoveId: MoveId.Snatch })
                    {
                        return false;
                    }
                    battle.EffectState.Move = battle.ActiveMove.Id;
                    return new VoidReturn();
                },
                OnBeforeMove = (battle, pokemon, _, move) =>
                {
                    if (!(pokemon.GetItem()?.IsChoice ?? false))
                    {
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.ChoiceLock]);
                        return new VoidReturn();
                    }

                    if (pokemon.IgnoringItem() ||
                        move.Id == battle.EffectState.Move ||
                        move.Id == MoveId.Struggle) return new VoidReturn();

                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintFailEvent(pokemon);
                    }
                    return false;
                },
                OnDisableMove = (battle, pokemon) =>
                {
                    if (!pokemon.GetItem()?.IsChoice == true ||
                        battle.EffectState.Move != null && !pokemon.HasMove((MoveId)battle.EffectState.Move))
                    {
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.ChoiceLock]);
                        return;
                    }
                    if (pokemon.IgnoringItem())
                    {
                        return;
                    }

                    foreach (MoveSlot moveSlot in pokemon.MoveSlots.Where(moveSlot =>
                                 moveSlot.Move != battle.EffectState.Move))
                    {
                        pokemon.DisableMove(moveSlot.Id, false, battle.EffectState.SourceEffect);
                    }
                },
            },
            [ConditionId.LeechSeed] = new()
            {
                Id = ConditionId.LeechSeed,
                Name = "Leech Seed",
                ConditionEffectType = ConditionEffectType.Condition,
                ImmuneTypes = [PokemonType.Grass],
                AssociatedMove = MoveId.LeechSeed,
                OnStart = (battle, target, _, _) =>
                {
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintStartEvent(target, _library.Moves[MoveId.LeechSeed].ToActiveMove());
                    }
                    return new VoidReturn();
                },
                OnResidualOrder = 8,
                OnResidual = (battle, pokemon, _, _) =>
                {
                    Pokemon? target = battle.GetAtSlot(pokemon.Volatiles[ConditionId.LeechSeed].SourceSlot);
                    if (target is null || target.Fainted || target.Hp <= 0)
                    {
                        return;
                    }

                    IntFalseUnion? damage = battle.Damage(pokemon.BaseMaxHp / 8, pokemon, target);
                    if (damage is IntIntFalseUnion d)
                    {
                        battle.Heal(d.Value, target, pokemon);
                    }
                },
            },
            [ConditionId.TrickRoom] = new()
            {
                Id = ConditionId.TrickRoom,
                Name = "Trick Room",
                ConditionEffectType = ConditionEffectType.Condition,
                Duration = 5,
                DurationCallback = (_, _, _, _) => 5,
                OnFieldStart = (battle, _, source, _) =>
                {
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintFieldStartEvent(_library.Conditions[ConditionId.TrickRoom], 
                            null, source);
                    }
                },
                OnFieldRestart = (battle, _, _, _) =>
                {
                    battle.Field.RemovePseudoWeather(_library.Conditions[ConditionId.TrickRoom]);
                },
                OnFieldResidualOrder = 27,
                OnFieldResidualSubOrder = 1,
                OnFieldEnd = (battle, _) =>
                {
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintFieldEndEvent(_library.Conditions[ConditionId.TrickRoom]);
                    }
                },
            },
            [ConditionId.Stall] = new()
            {
                // Protect, Detect, Endure counter
                Id = ConditionId.Stall,
                Name = "Stall",
                Duration = 2,
                CounterMax = 729,
                ConditionEffectType = ConditionEffectType.Condition,
                OnStart = (battle, _, _, _) =>
                {
                    battle.EffectState.Counter = 3;
                    return new VoidReturn();
                },
                OnStallMove = (battle, pokemon) =>
                {
                    int counter = battle.EffectState.Counter ?? 1;
                    bool success = battle.RandomChance(1, counter);
                    if (!success)
                    {
                        pokemon.DeleteVolatile(ConditionId.Stall);
                    }
                    return success;
                },
                OnRestart = (battle, _, _, _) =>
                {
                    if (battle.Effect is Condition condition && battle.EffectState.Counter < condition.CounterMax)
                    {
                        battle.EffectState.Counter *= 3;
                    }
                    battle.EffectState.Duration = 2;
                    return new VoidReturn();
                },
            },
            [ConditionId.Protect] = new()
            {
                Id = ConditionId.Protect,
                Name = "Protect",
                Duration = 1,
                ConditionEffectType = ConditionEffectType.Condition,
                AssociatedMove = MoveId.Protect,
                OnStart = (battle, target, _, _) =>
                {
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintSingleTurnEvent(target, _library.Conditions[ConditionId.Protect]);
                    }
                    return new VoidReturn();
                },
                OnTryHitPriority = 3,
                OnTryHit = (battle, target, source, move) =>
                {
                    if (!(move.Flags.Protect ?? false))
                    {
                        return new VoidReturn();
                    }
                    if (move.SmartTarget ?? false)
                    {
                        move.SmartTarget = false;
                    }
                    else if (battle.PrintDebug)
                    {
                        UiGenerator.PrintActivateEvent(target, _library.Conditions[ConditionId.Protect]);
                    }

                    EffectState? lockedMove = source.GetVolatile(ConditionId.LockedMove);
                    if (lockedMove is not null && source.Volatiles[ConditionId.LockedMove].Duration == 2)
                    {
                        source.RemoveVolatile(_library.Conditions[ConditionId.LockedMove]);
                    }
                    return new Undefined();
                },
            },
            [ConditionId.Tailwind] = new()
            {
                Id = ConditionId.Tailwind,
                Name = "Tailwind",
                ConditionEffectType = ConditionEffectType.Condition,
                AssociatedMove = MoveId.Tailwind,
                Duration = 4,
                DurationCallback = (_, _, _, _) => 4,
                OnSideStart = (battle, side, _, _) =>
                {
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintSideStartEvent(side, _library.Conditions[ConditionId.Tailwind]);
                    }
                },
                OnModifySpe = (battle, _, _) =>
                {
                    battle.ChainModify(2);
                    return new VoidReturn();
                },
                OnSideResidualOrder = 26,
                OnSideResidualSubOrder = 5,
                OnSideEnd = (battle, side) =>
                {
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintSideEndEvent(side, _library.Conditions[ConditionId.Tailwind]);
                    }
                },
            },
            [ConditionId.Reflect] = new()
            {
                Id = ConditionId.Reflect,
                Name = "Reflect",
                ConditionEffectType = ConditionEffectType.Condition,
                AssociatedMove = MoveId.Reflect,
                Duration = 5,
                DurationCallback = (_, _, source, _) => source.HasItem(ItemId.LightClay) ? 8 : 5,
                OnAnyModifyDamage = (battle, _, source, target, move) =>
                {
                    if (target != source &&
                        battle.EffectState.Target is SideEffectStateTarget side &&
                        side.Side.HasAlly(target) &&
                        battle.GetCategory(move) == MoveCategory.Physical)
                    {
                        if (!target.GetMoveHitData(move).Crit && !(move.Infiltrates ?? false))
                        {
                            return battle.ActivePerHalf > 1 ? battle.ChainModify([2732, 4096]) :
                                battle.ChainModify(0.5);
                        }
                    }
                    return new VoidReturn();
                },
                OnSideStart = (battle, side, _, _) =>
                {
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintSideStartEvent(side, _library.Conditions[ConditionId.Reflect]);
                    }
                },
                OnSideResidualOrder = 26,
                OnSideResidualSubOrder = 1,
                OnSideEnd = (battle, side) =>
                {
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintSideEndEvent(side, _library.Conditions[ConditionId.Reflect]);
                    }
                },
            },
            [ConditionId.LightScreen] = new()
            {
                Id = ConditionId.LightScreen,
                Name = "Light Screen",
                ConditionEffectType = ConditionEffectType.Condition,
                AssociatedMove = MoveId.LightScreen,
                Duration = 5,
                DurationCallback = (_, _, source, _) => source.HasItem(ItemId.LightClay) ? 8 : 5,
                OnAnyModifyDamage = (battle, _, source, target, move) =>
                {
                    if (target != source &&
                        battle.EffectState.Target is SideEffectStateTarget side &&
                        side.Side.HasAlly(target) &&
                        battle.GetCategory(move) == MoveCategory.Special)
                    {
                        if (!target.GetMoveHitData(move).Crit && !(move.Infiltrates ?? false))
                        {
                            return battle.ActivePerHalf > 1 ? battle.ChainModify([2732, 4096]) :
                                battle.ChainModify(0.5);
                        }
                    }
                    return new VoidReturn();
                },
                OnSideStart = (battle, side, _, _) =>
                {
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintSideStartEvent(side, _library.Conditions[ConditionId.LightScreen]);
                    }
                },
                OnSideResidualOrder = 26,
                OnSideResidualSubOrder = 2,
                OnSideEnd = (battle, side) =>
                {
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintSideEndEvent(side, _library.Conditions[ConditionId.LightScreen]);
                    }
                },
            },
            [ConditionId.ElectricTerrain] = new()
            {
                Id = ConditionId.ElectricTerrain,
                Name = "Electric Terrain",
                ConditionEffectType = ConditionEffectType.Terrain,
                Duration = 5,
                DurationCallback = (_, source, _, _) => source.HasItem(ItemId.TerrainExtender) ? 8 : 5,
                OnSetStatus = (battle, status, target, _, effect) =>
                {
                    if (status.Id == ConditionId.Sleep &&
                        target.IsGrounded() &&
                        !target.IsSemiInvulnerable())
                    {
                        if (battle.PrintDebug && effect is Condition { Id: ConditionId.Yawn } or
                                Move { Id: MoveId.Yawn } or
                                    ActiveMove { Secondaries: not null })
                        {
                            UiGenerator.PrintActivateEvent(target, _library.Conditions[ConditionId.ElectricTerrain]);
                        }
                        return false;
                    }
                    
                    return new VoidReturn();
                },
                OnTryAddVolatile = (battle, status, target, _, _) =>
                {
                    if (!target.IsGrounded() || target.IsSemiInvulnerable()) return new VoidReturn();
                    if (status.Id == ConditionId.Yawn)
                    {
                        if (battle.PrintDebug)
                        {
                            UiGenerator.PrintActivateEvent(target, _library.Conditions[ConditionId.ElectricTerrain]);
                        }
                        return null;
                    }
                    return new VoidReturn();
                },
                OnBasePowerPriority = 6,
                OnBasePower = (battle, _, attacker, _, move) =>
                {
                    if (move.Type == MoveType.Electric &&
                        attacker.IsGrounded() &&
                        !attacker.IsSemiInvulnerable())
                    {
                        return battle.ChainModify([5325, 4096]);
                    }
                    return new VoidReturn();
                },
                OnFieldStart = (battle, _, source, effect) =>
                {
                    if (!battle.PrintDebug) return;

                    Condition et = _library.Conditions[ConditionId.ElectricTerrain];

                    if (effect is Ability ability)
                    {
                        UiGenerator.PrintFieldStartEvent(et, ability, source);
                    }
                    else
                    {
                        UiGenerator.PrintFieldStartEvent(et);
                    }
                },
                OnFieldResidualOrder = 27,
                OnFieldResidualSubOrder = 7,
                OnFieldEnd = (battle, _) =>
                {
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintFieldEndEvent(_library.Conditions[ConditionId.ElectricTerrain]);
                    }
                },
            },
            [ConditionId.QuarkDrive] = new()
            {
                Id = ConditionId.QuarkDrive,
                Name = "Quark Drive",
                ConditionEffectType = ConditionEffectType.Condition,
                NoCopy = true,
                AssociatedAbility = AbilityId.QuarkDrive,
                OnStart = (battle, pokemon, _, effect) =>
                {
                    if (effect is Item item)
                    {
                        if (item.Id == ItemId.BoosterEnergy)
                        {
                            battle.EffectState.FromBooster = true;
                            if (battle.PrintDebug)
                            {
                                UiGenerator.PrintActivateEvent(pokemon,
                                    _library.Conditions[ConditionId.QuarkDrive],
                                    _library.Items[pokemon.Item]);
                            }
                        }
                    }
                    else if (battle.PrintDebug)
                    {
                        UiGenerator.PrintActivateEvent(pokemon, _library.Conditions[ConditionId.QuarkDrive]);
                    }
                    battle.EffectState.BestStat = pokemon.GetBestStat(false, true);
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintStartEvent(pokemon, _library.Conditions[ConditionId.QuarkDrive]);
                    }
                    return new VoidReturn();
                },
                OnModifyAtkPriority = 5,
                OnModifyAtk = (battle, _, pokemon, _, _) =>
                {
                    if (battle.EffectState.BestStat != StatIdExceptHp.Atk || pokemon.IgnoringAbility())
                    {
                        return new VoidReturn();
                    }
                    return battle.ChainModify([5325, 4096]);
                },
                OnModifyDefPriority = 6,
                OnModifyDef = (battle, _, pokemon, _, _) =>
                {
                    if (battle.EffectState.BestStat != StatIdExceptHp.Def || pokemon.IgnoringAbility())
                    {
                        return new VoidReturn();
                    }
                    return battle.ChainModify([5325, 4096]);
                },
                OnModifySpAPriority = 5,
                OnModifySpA = (battle, _, pokemon, _, _) =>
                {
                    if (battle.EffectState.BestStat != StatIdExceptHp.SpA || pokemon.IgnoringAbility())
                    {
                        return new VoidReturn();
                    }
                    return battle.ChainModify([5325, 4096]);
                },
                OnModifySpDPriority = 6,
                OnModifySpD = (battle, _, pokemon, _, _) =>
                {
                    if (battle.EffectState.BestStat != StatIdExceptHp.SpD || pokemon.IgnoringAbility())
                    {
                        return new VoidReturn();
                    }
                    return battle.ChainModify([5325, 4096]);
                },
                OnModifySpe = (battle, _, pokemon) =>
                {
                    if (battle.EffectState.BestStat != StatIdExceptHp.Spe || pokemon.IgnoringAbility())
                    {
                        return new VoidReturn();
                    }
                    return (int)battle.ChainModify(1.5);
                },
                OnEnd = (battle, pokemon) =>
                {
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintEndEvent(pokemon, _library.Conditions[ConditionId.QuarkDrive]);
                    }
                },
            },
        };
    }
}