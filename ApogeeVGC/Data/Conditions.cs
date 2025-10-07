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
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (sourceEffect is null)
                    {
                        throw new ArgumentNullException($"Source effect is null when trying to apply" +
                                                        $"{ConditionId.Burn} to" + $"pokemon {target.Name}.");
                    }

                    bool debug = battle.PrintDebug;
                    if (!debug) return null;

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
                    return null;
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
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (!battle.PrintDebug) return null;

                    Condition paralysis = _library.Conditions[ConditionId.Paralysis];

                    if (sourceEffect is Ability ability)
                    {
                        UiGenerator.PrintStatusEvent(target, paralysis, ability, source);
                    }
                    else
                    {
                        UiGenerator.PrintStatusEvent(target, paralysis);
                    }
                    return null;
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
                    if (!battle.RandomChance(1, 4)) return true;
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

                    if (!target.RemoveVolatile(battle, nightmare)) return null;

                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintEndEvent(target, nightmare);
                    }
                    return null;
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
                        return null;
                    }
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintCantEvent(pokemon, _library.Conditions[ConditionId.Sleep]);
                    }
                    if (move.SleepUsable ?? false)
                    {
                        return null;
                    }
                    return false;
                },
            },
            [ConditionId.Freeze] = new()
            {
                Id = ConditionId.Freeze,
                Name = "Freeze",
                ConditionEffectType = ConditionEffectType.Status,
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
                    return null;
                },
                OnBeforeMovePriority = 10,
                OnBeforeMove = (battle, pokemon, _, move) =>
                {
                    if ((move.Flags.Defrost ?? false) &&
                        !(move.Id == MoveId.BurnUp && !pokemon.HasType(PokemonType.Fire)))
                    {
                        return null;
                    }
                    if (battle.RandomChance(1, 5))
                    {
                        pokemon.CureStatus();
                        return null;
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
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (!battle.PrintDebug) return null;

                    Condition poison = _library.Conditions[ConditionId.Poison];

                    if (sourceEffect.EffectType == EffectType.Ability)
                    {
                        UiGenerator.PrintStatusEvent(target, poison, sourceEffect, source);
                    }
                    else
                    {
                        UiGenerator.PrintStatusEvent(target, poison);
                    }
                    return null;
                },
            },
            [ConditionId.Toxic] = new()
            {
                Id = ConditionId.Toxic,
                Name = "Toxic",
                ConditionEffectType = ConditionEffectType.Status,
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    battle.EffectState.Stage = 0;
                    if (!battle.PrintDebug) return null;

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
                    return null;
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
                    return null;
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
                        pokemon.RemoveVolatile(battle, _library.Conditions[ConditionId.Confusion]);
                        return null;
                    }
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintActivateEvent(pokemon, _library.Conditions[ConditionId.Confusion]);
                    }
                    if (!battle.RandomChance(33, 100))
                    {
                        return null;
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
                    return null;
                },
                OnBeforeMove = (battle, pokemon, _, move) =>
                {
                    if (!pokemon.GetItem()?.IsChoice ?? false)
                    {
                        pokemon.RemoveVolatile(battle, _library.Conditions[ConditionId.ChoiceLock]);
                        return null;
                    }

                    if (pokemon.IgnoringItem() ||
                        move.Id == battle.EffectState.Move ||
                        move.Id == MoveId.Struggle) return null;

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
                        pokemon.RemoveVolatile(battle, _library.Conditions[ConditionId.ChoiceLock]);
                        return;
                    }
                    if (pokemon.IgnoringItem())
                    {
                        return;
                    }

                    foreach (MoveSlot moveSlot in pokemon.MoveSlots.Where(moveSlot =>
                                 moveSlot.Move.Id != battle.EffectState.Move))
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
                AssociatedMove = MoveId.LeechSeed,
                OnStart = (battle, target, _, _) =>
                {
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintStartEvent(target, _library.Moves[MoveId.LeechSeed].ToActiveMove());
                    }
                    return null;
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
                        UiGenerator.PrintFieldStartEvent(_library.Conditions[ConditionId.TrickRoom], source);
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
                    return null;
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
                    return null;
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
                    return null;
                },
                OnTryHitPriority = 3,
                OnTryHit = (battle, target, source, move) =>
                {
                    if (!(move.Flags.Protect ?? false))
                    {
                        return null;
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
                        source.RemoveVolatile(battle, _library.Conditions[ConditionId.LockedMove]);
                    }
                    return true;
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
                    return null;
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
                    return null;
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
                    return null;
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
            //[ConditionId.Leftovers] = new Condition
            //{
            //    Id = ConditionId.Leftovers,
            //    Name = "Leftovers",
            //    ConditionEffectType = ConditionEffectType.Condition,
            //    ConditionVolatility = ConditionVolatility.Volatile,
            //    OnResidualOrder = 5,
            //    OnResidualSubOrder = 4,
            //    Duration = 1, // Condition is refreshed each turn by the item effect
            //    OnResidual = (target, _, _, context) =>
            //    {
            //        int heal = target.UnmodifiedHp / 16;
            //        if (heal < 1) heal = 1;
            //        int actualHeal = target.Heal(heal);
            //        if (context.PrintDebug)
            //        {
            //            UiGenerator.PrintLeftoversHeal(target, actualHeal);
            //        }
            //    },
            //},
            //[ConditionId.ChoiceSpecs] = new Condition
            //{
            //    Id = ConditionId.ChoiceSpecs,
            //    Name = "Choice Specs",
            //    ConditionEffectType = ConditionEffectType.Condition,
            //    ConditionVolatility = ConditionVolatility.Volatile,
            //    OnModifySpAPriority = 1,
            //    OnModifySpA = (_) => 1.5,
            //},
            //[ConditionId.FlameOrb] = new Condition
            //{
            //    Id = ConditionId.FlameOrb,
            //    Name = "Flame Orb",
            //    ConditionEffectType = ConditionEffectType.Condition,
            //    ConditionVolatility = ConditionVolatility.Volatile,
            //    OnResidualOrder = 28,
            //    OnResidualSubOrder = 3,
            //    Duration = 1,
            //    OnResidual = (target, _, _, context) =>
            //    {
            //        if (target.HasCondition(ConditionId.Burn)) return;
            //        target.AddCondition(context.Library.Conditions[ConditionId.Burn], context, target,
            //            target.Item ?? throw new InvalidOperationException("The target should always be holding" +
            //                                                               "a flame orb here"));
            //    },
            //},
            //[ConditionId.AssaultVest] = new Condition
            //{
            //    Id = ConditionId.AssaultVest,
            //    Name = "Assault Vest",
            //    ConditionEffectType = ConditionEffectType.Condition,
            //    ConditionVolatility = ConditionVolatility.Volatile,
            //    OnModifySpDPriority = 1,
            //    OnModifySpD = (_) => 1.5,
            //    OnDisableMove = (pokemon, _, _) =>
            //    {
            //        // disable all moves which are status type (except me first)
            //        foreach (Move move in pokemon.Moves)
            //        {
            //            if (move.Category == MoveCategory.Status && move.Id != MoveId.MeFirst)
            //            {
            //                move.Disabled = true;
            //            }
            //        }
            //    },
            //    OnStart = (pokemon, _, _, _) =>
            //    {
            //        // disable all moves which are status type (except me first)
            //        foreach (Move move in pokemon.Moves)
            //        {
            //            if (move.Category == MoveCategory.Status && move.Id != MoveId.MeFirst)
            //            {
            //                move.Disabled = true;
            //            }
            //        }
            //        return true;
            //    },
            //},
            //[ConditionId.ElectricTerrain] = new Condition
            //{
            //    Id = ConditionId.ElectricTerrain,
            //    Name = "Electric Terrain",
            //    ConditionEffectType = ConditionEffectType.Terrain,
            //    ConditionVolatility = ConditionVolatility.Volatile,
            //},
            //[ConditionId.HadronEngine] = new Condition
            //{
            //    Id = ConditionId.HadronEngine,
            //    Name = "Hadron Engine",
            //    ConditionEffectType = ConditionEffectType.Condition,
            //    ConditionVolatility = ConditionVolatility.Volatile,
            //    OnModifySpAPriority = 5,
            //    OnModifySpA = (pokemon) => pokemon.HasCondition(ConditionId.ElectricTerrain) ? 5461.0 / 4096.0 : 1.0,
            //},
            //[ConditionId.Guts] = new Condition
            //{
            //    Id = ConditionId.Guts,
            //    Name = "Guts",
            //    ConditionEffectType = ConditionEffectType.Condition,
            //    ConditionVolatility = ConditionVolatility.Volatile,
            //    OnModifyAtkPriority = 5,
            //    OnModifyAtk = (pokemon) =>
            //    {
            //        if (pokemon.HasCondition(ConditionId.Burn) ||
            //            pokemon.HasCondition(ConditionId.Paralysis) ||
            //            pokemon.HasCondition(ConditionId.Poison) ||
            //            pokemon.HasCondition(ConditionId.Toxic))
            //        {
            //            return 1.5;
            //        }
            //        return 1.0;
            //    },
            //},
            //[ConditionId.FlameBody] = new Condition
            //{
            //    Id = ConditionId.FlameBody,
            //    Name = "Flame Body",
            //    ConditionEffectType = ConditionEffectType.Condition,
            //    ConditionVolatility = ConditionVolatility.Volatile,
            //    OnDamagingHit = (_, target, source, move, context) =>
            //    {
            //        if (!(move.Flags.Contact ?? false)) return;

            //        if (source.HasCondition(ConditionId.Burn) ||
            //            source.HasCondition(ConditionId.Freeze) ||
            //            source.HasCondition(ConditionId.Sleep) ||
            //            source.HasCondition(ConditionId.Paralysis) ||
            //            source.HasCondition(ConditionId.Poison) ||
            //            source.HasCondition(ConditionId.Toxic)) return;

            //        bool burned = context.Random.NextDouble() < 0.3;

            //        if (!burned) return;

            //        source.AddCondition(context.Library.Conditions[ConditionId.Burn], context, target,
            //            target.Ability ?? throw new InvalidOperationException("The target should always have" +
            //                                                                  "flame body ability here"));
            //        //if (context.PrintDebug)
            //        //{
            //        //    UiGenerator.PrintFlameBodyBurn(source, target);
            //        //}
            //    },
            //},
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
                                    _library.Conditions[ConditionId.QuarkDrive], pokemon.Item);
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
                    return null;
                },
                OnModifyAtkPriority = 5,
                OnModifyAtk = (battle, _, pokemon, _, _) =>
                {
                    if (battle.EffectState.BestStat != StatIdExceptHp.Atk || pokemon.IgnoringAbility(battle))
                    {
                        return null;
                    }
                    return battle.ChainModify([5325, 4096]);
                },
                OnModifyDefPriority = 6,
                OnModifyDef = (battle, _, pokemon, _, _) =>
                {
                    if (battle.EffectState.BestStat != StatIdExceptHp.Def || pokemon.IgnoringAbility(battle))
                    {
                        return null;
                    }
                    return battle.ChainModify([5325, 4096]);
                },
                OnModifySpAPriority = 5,
                OnModifySpA = (battle, _, pokemon, _, _) =>
                {
                    if (battle.EffectState.BestStat != StatIdExceptHp.SpA || pokemon.IgnoringAbility(battle))
                    {
                        return null;
                    }
                    return battle.ChainModify([5325, 4096]);
                },
                OnModifySpDPriority = 6,
                OnModifySpD = (battle, _, pokemon, _, _) =>
                {
                    if (battle.EffectState.BestStat != StatIdExceptHp.SpD || pokemon.IgnoringAbility(battle))
                    {
                        return null;
                    }
                    return battle.ChainModify([5325, 4096]);
                },
                OnModifySpe = (battle, _, pokemon) =>
                {
                    if (battle.EffectState.BestStat != StatIdExceptHp.Spe || pokemon.IgnoringAbility(battle))
                    {
                        return null;
                    }
                    return battle.ChainModify(1.5);
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