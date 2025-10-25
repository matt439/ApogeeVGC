using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Unions;

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
                EffectType = EffectType.Status,
                ImmuneTypes = [PokemonType.Fire],
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (sourceEffect is null)
                    {
                        throw new ArgumentNullException($"Source effect is null when trying to apply" +
                                                        $"{ConditionId.Burn} to" + $"pokemon {target.Name}.");
                    }

                    if (!battle.DisplayUi) return new VoidReturn();

                    switch (sourceEffect.EffectType)
                    {
                        case EffectType.Item:
                            if (sourceEffect is Item { Id: ItemId.FlameOrb })
                            {
                                battle.Add("-status", target, "brn", "[from] item: Flame Orb");
                            }
                            break;
                        case EffectType.Ability:
                            if (sourceEffect is Ability)
                            {
                                battle.Add("-status", target, "brn", "[from] ability: " +
                                    sourceEffect.Name, $"[of] {source}");
                            }
                            break;
                        case EffectType.Move:
                            battle.Add("-status", target, "brn");
                            break;
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
                EffectType = EffectType.Status,
                ImmuneTypes = [PokemonType.Electric],
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (!battle.DisplayUi) return new VoidReturn();

                    if (sourceEffect is Ability)
                    {
                        battle.Add("-status", target, "par", "[from] ability: " + sourceEffect.Name,
                            $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-status", target, "par");
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
                    if (battle.DisplayUi)
                    {
                        battle.Add("cant", pokemon, "par");
                    }
                    return false;
                },
            },
            [ConditionId.Sleep] = new()
            {
                Id = ConditionId.Sleep,
                Name = "Sleep",
                EffectType = EffectType.Status,
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (battle.DisplayUi)
                    {
                        switch (sourceEffect)
                        {
                            case Ability:
                                battle.Add("-status", target, "slp", "[from] ability: " +
                                    sourceEffect.Name, $"[of] {source}");
                                break;
                            case ActiveMove:
                                battle.Add("-status", target, "slp", $"[from] move: {sourceEffect.Name}");
                                break;
                            default:
                                battle.Add("-status", target, "slp");
                                break;
                        }
                    }

                    battle.EffectState.StartTime = battle.Random(2, 5);
                    battle.EffectState.Time = battle.EffectState.StartTime;

                    Condition nightmare = _library.Conditions[ConditionId.Nightmare];

                    if (!target.RemoveVolatile(nightmare)) return new VoidReturn();

                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", target, "Nightmare", "[silent]");
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
                    if (battle.DisplayUi)
                    {
                        battle.Add("cant", pokemon, "slp");
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
                EffectType = EffectType.Status,
                ImmuneTypes = [PokemonType.Ice],
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (battle.DisplayUi)
                    {
                        switch (sourceEffect)
                        {
                            case Ability:
                                battle.Add("-status", target, "frz", "[from] ability: " +
                                    sourceEffect.Name, $"[of] {source}");
                                break;
                            default:
                                battle.Add("-status", target, "frz");
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
                    if (battle.DisplayUi)
                    {
                        battle.Add("cant", pokemon, "frz");
                    }
                    return false;
                },
                OnModifyMove = (battle, move, pokemon, _) =>
                {
                    if (!(move.Flags.Defrost ?? false)) return;
                    if (battle.DisplayUi)
                    {
                        battle.Add("-curestatus", pokemon, "frz", $"[from] move: {move}");
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
                EffectType = EffectType.Status,
                ImmuneTypes = [PokemonType.Poison, PokemonType.Steel],
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (!battle.DisplayUi) return new VoidReturn();

                    if (sourceEffect.EffectType == EffectType.Ability)
                    {
                        battle.Add("-status", target, "psn", "[from] ability: " +
                            sourceEffect.Name, $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-status", target, "psn");
                    }
                    return new VoidReturn();
                },
            },
            [ConditionId.Toxic] = new()
            {
                Id = ConditionId.Toxic,
                Name = "Toxic",
                EffectType = EffectType.Status,
                ImmuneTypes = [PokemonType.Poison, PokemonType.Steel],
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    battle.EffectState.Stage = 0;
                    if (!battle.DisplayUi) return new VoidReturn();

                    switch (sourceEffect)
                    {
                        case Item { Id: ItemId.ToxicOrb }:
                            battle.Add("-status", target, "tox", "[from] item: Toxic Orb");
                            break;
                        case Ability:
                            battle.Add("-status", target, "tox", "[from] ability: " +
                                                                 sourceEffect.Name, $"[of] {source}");
                            break;
                        default:
                            battle.Add("-status", target, "tox");
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
                EffectType = EffectType.Condition,
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (battle.DisplayUi)
                    {
                        switch (sourceEffect)
                        {
                            case Condition { Id: ConditionId.LockedMove }:
                                battle.Add("-start", target, "confusion", "[fatigue]");
                                break;
                            case Ability:
                                battle.Add("-start", target, "confusion", "[from] ability: " +
                                    sourceEffect.Name, $"[of] {source}");
                                break;
                            default:
                                battle.Add("-start", target, "confusion");
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
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", target, "confusion");
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
                    if (battle.DisplayUi)
                    {
                        battle.Add("-activate", pokemon, "confusion");
                    }
                    if (!battle.RandomChance(33, 100))
                    {
                        return new VoidReturn();
                    }
                    battle.ActiveTarget = pokemon;
                    int damage = battle.Actions.GetConfusionDamage(pokemon, 40);

                    ActiveMove activeMove = new()
                    {
                        Name = "Confused",
                        Id = MoveId.Confused,
                        Accuracy = IntTrueUnion.FromTrue(),
                        Num = 100200,
                        Type = MoveType.Normal,
                        MoveSlot = new MoveSlot(),
                    };
                    battle.Damage(damage, pokemon, pokemon, BattleDamageEffect.FromIEffect(activeMove));
                    return false;
                },
            },
            [ConditionId.Flinch] = new()
            {
                Id = ConditionId.Flinch,
                Name = "Flinch",
                EffectType = EffectType.Condition,
                Duration = 1,
                OnBeforeMovePriority = 8,
                OnBeforeMove = (battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("cant", pokemon, "flinch");
                    }
                    battle.RunEvent(EventId.Flinch, pokemon);
                    return false;
                },
            },
            [ConditionId.ChoiceLock] = new()
            {
                Id = ConditionId.ChoiceLock,
                NoCopy = true,
                EffectType = EffectType.Condition,
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
                    if (!(pokemon.GetItem().IsChoice ?? false))
                    {
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.ChoiceLock]);
                        return new VoidReturn();
                    }

                    if (pokemon.IgnoringItem() ||
                        move.Id == battle.EffectState.Move ||
                        move.Id == MoveId.Struggle) return new VoidReturn();

                    if (!battle.DisplayUi) return false;

                    battle.AddMove("move", StringNumberDelegateObjectUnion.FromObject(pokemon), move.Name);
                    battle.AttrLastMove("[still]");
                    battle.Debug("Disabled by Choice item lock");
                    battle.Add("-fail", pokemon);
                    return false;
                },
                OnDisableMove = (battle, pokemon) =>
                {
                    if (!pokemon.GetItem().IsChoice == true ||
                        battle.EffectState.Move != null && !pokemon.HasMove((MoveId)battle.EffectState.Move))
                    {
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.ChoiceLock]);
                        return;
                    }
                    if (pokemon.IgnoringItem()) return;

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
                EffectType = EffectType.Condition,
                ImmuneTypes = [PokemonType.Grass],
                AssociatedMove = MoveId.LeechSeed,
                OnStart = (battle, target, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, "move: Leech Seed");
                    }
                    return new VoidReturn();
                },
                OnResidualOrder = 8,
                OnResidual = (battle, pokemon, _, _) =>
                {
                    Pokemon? target = battle.GetAtSlot(pokemon.Volatiles[ConditionId.LeechSeed].SourceSlot);
                    if (target is null || target.Fainted || target.Hp <= 0)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Debug("Nothing to leech into");
                        }
                        return;
                    }
                    IntFalseUndefinedUnion damage =
                        battle.Damage(pokemon.BaseMaxHp / 8, pokemon, target);

                    if (damage is IntIntFalseUndefined d)
                    {
                        battle.Heal(d.Value, target, pokemon);
                    }
                },
            },
            [ConditionId.TrickRoom] = new()
            {
                Id = ConditionId.TrickRoom,
                Name = "Trick Room",
                EffectType = EffectType.Condition,
                Duration = 5,
                DurationCallback = (_, _, _, _) => 5,
                OnFieldStart = (battle, _, source, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-fieldstart", "move: Trick Room", $"[of] {source}");
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
                    if (battle.DisplayUi)
                    {
                        battle.Add("-fieldend", "move: Trick Room");
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
                EffectType = EffectType.Condition,
                OnStart = (battle, _, _, _) =>
                {
                    battle.EffectState.Counter = 3;
                    return new VoidReturn();
                },
                OnStallMove = (battle, pokemon) =>
                {
                    int counter = battle.EffectState.Counter ?? 1;
                    if (battle.DisplayUi)
                    {
                        battle.Debug($"Success change: {Math.Round(100.0 / counter)}%");
                    }
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
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Protect,
                OnStart = (battle, target, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singleturn", target, "Protect");
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
                    else if (battle.DisplayUi)
                    {
                        battle.Add("-activate", target, "move: Protect");
                    }

                    EffectState? lockedMove = source.GetVolatile(ConditionId.LockedMove);
                    if (lockedMove is not null && source.Volatiles[ConditionId.LockedMove].Duration == 2)
                    {
                        source.RemoveVolatile(_library.Conditions[ConditionId.LockedMove]);
                    }
                    return new Empty(); // in place of Battle.NOT_FAIL ("")
                },
            },
            [ConditionId.Tailwind] = new()
            {
                Id = ConditionId.Tailwind,
                Name = "Tailwind",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Tailwind,
                Duration = 4,
                DurationCallback = (_, _, _, _) => 4,
                OnSideStart = (battle, side, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", side, "move: Tailwind");
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
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sideend", side, "move: Tailwind");
                    }
                },
            },
            [ConditionId.Reflect] = new()
            {
                Id = ConditionId.Reflect,
                Name = "Reflect",
                EffectType = EffectType.Condition,
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
                            if (battle.DisplayUi)
                            {
                                battle.Debug("Reflect weaken");
                            }
                            return battle.ActivePerHalf > 1 ? battle.ChainModify([2732, 4096]) :
                                battle.ChainModify(0.5);
                        }
                    }
                    return new VoidReturn();
                },
                OnSideStart = (battle, side, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", side, "Reflect");
                    }
                },
                OnSideResidualOrder = 26,
                OnSideResidualSubOrder = 1,
                OnSideEnd = (battle, side) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sideend", side, "Reflect");
                    }
                },
            },
            [ConditionId.LightScreen] = new()
            {
                Id = ConditionId.LightScreen,
                Name = "Light Screen",
                EffectType = EffectType.Condition,
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
                            if (battle.DisplayUi)
                            {
                                battle.Debug("Light Screen weaken");
                            }
                            return battle.ActivePerHalf > 1 ? battle.ChainModify([2732, 4096]) :
                                battle.ChainModify(0.5);
                        }
                    }
                    return new VoidReturn();
                },
                OnSideStart = (battle, side, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", side, "move: Light Screen");
                    }
                },
                OnSideResidualOrder = 26,
                OnSideResidualSubOrder = 2,
                OnSideEnd = (battle, side) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sideend", side, "move: Light Screen");
                    }
                },
            },
            [ConditionId.ElectricTerrain] = new()
            {
                Id = ConditionId.ElectricTerrain,
                Name = "Electric Terrain",
                EffectType = EffectType.Terrain,
                Duration = 5,
                DurationCallback = (_, source, _, _) => source.HasItem(ItemId.TerrainExtender) ? 8 : 5,
                OnSetStatus = (battle, status, target, _, effect) =>
                {
                    if (status.Id == ConditionId.Sleep &&
                        (target.IsGrounded() ?? false) &&
                        !target.IsSemiInvulnerable())
                    {
                        if (battle.DisplayUi && effect is Condition { Id: ConditionId.Yawn } or
                                Move { Id: MoveId.Yawn } or
                                    ActiveMove { Secondaries: not null })
                        {
                            battle.Add("-activate", target, "move: Electric Terrain");
                        }
                        return false;
                    }
                    return new VoidReturn();
                },
                OnTryAddVolatile = (battle, status, target, _, _) =>
                {
                    if (!(target.IsGrounded() ?? false) || target.IsSemiInvulnerable()) return new VoidReturn();
                    if (status.Id == ConditionId.Yawn)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", target, "move: Electric Terrain");
                        }
                        return null;
                    }
                    return new VoidReturn();
                },
                OnBasePowerPriority = 6,
                OnBasePower = (battle, _, attacker, _, move) =>
                {
                    if (move.Type == MoveType.Electric &&
                        (attacker.IsGrounded() ?? false) &&
                        !attacker.IsSemiInvulnerable())
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Debug("electric terrain boost");
                        }
                        return battle.ChainModify([5325, 4096]);
                    }
                    return new VoidReturn();
                },
                OnFieldStart = (battle, _, source, effect) =>
                {
                    if (!battle.DisplayUi) return;

                    if (effect is Ability)
                    {
                        battle.Add("-fieldstart", "move: Electric Terrain", "[from] ability: " +
                            effect.Name, $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-fieldstart", "move: Electric Terrain");
                    }
                },
                OnFieldResidualOrder = 27,
                OnFieldResidualSubOrder = 7,
                OnFieldEnd = (battle, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-fieldend", "move: Electric Terrain");
                    }
                },
            },
            [ConditionId.QuarkDrive] = new()
            {
                Id = ConditionId.QuarkDrive,
                Name = "Quark Drive",
                EffectType = EffectType.Condition,
                NoCopy = true,
                AssociatedAbility = AbilityId.QuarkDrive,
                OnStart = (battle, pokemon, _, effect) =>
                {
                    if (effect is Item item)
                    {
                        if (item.Id == ItemId.BoosterEnergy)
                        {
                            battle.EffectState.FromBooster = true;
                            if (battle.DisplayUi)
                            {
                                battle.Add("-activate", pokemon, "ability: Quark Drive", "[fromitem]");
                            }
                        }
                    }
                    else if (battle.DisplayUi)
                    {
                        battle.Add("-activate", pokemon, "ability: Quark Drive");
                    }
                    battle.EffectState.BestStat = pokemon.GetBestStat(false, true);
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "quarkdrive" + battle.EffectState.BestStat);
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
                    if (battle.DisplayUi)
                    {
                        battle.Debug("Quark Drive atk boost");
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
                    if (battle.DisplayUi)
                    {
                        battle.Debug("Quark Drive def boost");
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
                    if (battle.DisplayUi)
                    {
                        battle.Debug("Quark Drive spa boost");
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
                    if (battle.DisplayUi)
                    {
                        battle.Debug("Quark Drive spd boost");
                    }
                    return battle.ChainModify([5325, 4096]);
                },
                OnModifySpe = (battle, _, pokemon) =>
                {
                    if (battle.EffectState.BestStat != StatIdExceptHp.Spe || pokemon.IgnoringAbility())
                    {
                        return new VoidReturn();
                    }
                    if (battle.DisplayUi)
                    {
                        battle.Debug("Quark Drive spe boost");
                    }
                    return (int)battle.ChainModify(1.5);
                },
                OnEnd = (battle, pokemon) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", pokemon, "Quark Drive");
                    }
                },
            },
        };
    }
}