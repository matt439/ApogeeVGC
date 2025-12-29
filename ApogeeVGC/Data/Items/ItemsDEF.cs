using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.ItemSpecific;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Items;

public partial record Items
{
    private partial Dictionary<ItemId, Item> CreateItemsDef()
    {
        return new Dictionary<ItemId, Item>
        {
            [ItemId.DampRock] = new()
            {
                Id = ItemId.DampRock,
                Name = "Damp Rock",
                SpriteNum = 88,
                Fling = new FlingData { BasePower = 60 },
                Num = 285,
                Gen = 4,
            },
            [ItemId.DawnStone] = new()
            {
                Id = ItemId.DawnStone,
                Name = "Dawn Stone",
                SpriteNum = 92,
                Fling = new FlingData { BasePower = 80 },
                Num = 109,
                Gen = 4,
            },
            [ItemId.DeepSeaScale] = new()
            {
                Id = ItemId.DeepSeaScale,
                Name = "Deep Sea Scale",
                SpriteNum = 93,
                Fling = new FlingData { BasePower = 30 },
                OnModifySpD = new OnModifySpDEventInfo((battle, spd, pokemon, _, _) =>
                {
                    if (pokemon.BaseSpecies.Name == "Clamperl")
                    {
                        battle.ChainModify(2);
                        return battle.FinalModify(spd);
                    }

                    return spd;
                }, 2),
                Num = 227,
                Gen = 3,
                // IsNonstandard = "Past", // TODO: Not supported in Item class
            },
            [ItemId.DeepSeaTooth] = new()
            {
                Id = ItemId.DeepSeaTooth,
                Name = "Deep Sea Tooth",
                SpriteNum = 94,
                Fling = new FlingData { BasePower = 90 },
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, pokemon, _, _) =>
                {
                    if (pokemon.BaseSpecies.Name == "Clamperl")
                    {
                        battle.ChainModify(2);
                        return battle.FinalModify(spa);
                    }

                    return spa;
                }, 1),
                Num = 226,
                Gen = 3,
                // IsNonstandard = "Past", // TODO: Not supported in Item class
            },
            [ItemId.DestinyKnot] = new()
            {
                Id = ItemId.DestinyKnot,
                Name = "Destiny Knot",
                SpriteNum = 95,
                Fling = new FlingData { BasePower = 10 },
                OnAttract = new OnAttractEventInfo((battle, target, source) =>
                {
                    battle.Debug($"attract intercepted: {target.Name} from {source.Name}");
                    if (source == null || source == target) return;
                    if (!source.Volatiles.ContainsKey(ConditionId.Attract))
                    {
                        source.AddVolatile(ConditionId.Attract);
                    }
                }, -100),
                Num = 280,
                Gen = 4,
            },
            [ItemId.DiveBall] = new()
            {
                Id = ItemId.DiveBall,
                Name = "Dive Ball",
                SpriteNum = 101,
                Num = 7,
                Gen = 3,
                IsPokeball = true,
            },
            [ItemId.DracoPlate] = new()
            {
                Id = ItemId.DracoPlate,
                Name = "Draco Plate",
                SpriteNum = 105,
                // OnPlate = 'Dragon', // TODO: Not relevant for Gen 9 standard play
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Type == MoveType.Dragon)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                OnTakeItem = new OnTakeItemEventInfo((Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion>)(
                    (_, item, pokemon, source, _) =>
                    {
                        if (source?.BaseSpecies.Num == 493 || pokemon.BaseSpecies.Num == 493)
                        {
                            return BoolVoidUnion.FromBool(false);
                        }
                        return BoolVoidUnion.FromBool(true);
                    })),
                ForcedForme = "Arceus-Dragon",
                Num = 311,
                Gen = 4,
            },
            [ItemId.DragonFang] = new()
            {
                Id = ItemId.DragonFang,
                Name = "Dragon Fang",
                SpriteNum = 106,
                Fling = new FlingData { BasePower = 70 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Type == MoveType.Dragon)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 250,
                Gen = 2,
            },
            [ItemId.DragonScale] = new()
            {
                Id = ItemId.DragonScale,
                Name = "Dragon Scale",
                SpriteNum = 108,
                Fling = new FlingData { BasePower = 30 },
                Num = 235,
                Gen = 2,
            },
            [ItemId.DreadPlate] = new()
            {
                Id = ItemId.DreadPlate,
                Name = "Dread Plate",
                SpriteNum = 110,
                // OnPlate = 'Dark', // TODO: Not relevant for Gen 9 standard play
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Type == MoveType.Dark)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                OnTakeItem = new OnTakeItemEventInfo((Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion>)(
                    (_, item, pokemon, source, _) =>
                    {
                        if (source?.BaseSpecies.Num == 493 || pokemon.BaseSpecies.Num == 493)
                        {
                            return BoolVoidUnion.FromBool(false);
                        }
                        return BoolVoidUnion.FromBool(true);
                    })),
                ForcedForme = "Arceus-Dark",
                Num = 312,
                Gen = 4,
            },
            [ItemId.DreamBall] = new()
            {
                Id = ItemId.DreamBall,
                Name = "Dream Ball",
                SpriteNum = 111,
                Num = 576,
                Gen = 5,
                IsPokeball = true,
            },
            [ItemId.DubiousDisc] = new()
            {
                Id = ItemId.DubiousDisc,
                Name = "Dubious Disc",
                SpriteNum = 113,
                Fling = new FlingData { BasePower = 50 },
                Num = 324,
                Gen = 4,
            },
            [ItemId.DuskBall] = new()
            {
                Id = ItemId.DuskBall,
                Name = "Dusk Ball",
                SpriteNum = 115,
                Num = 13,
                Gen = 4,
                IsPokeball = true,
            },
            [ItemId.DuskStone] = new()
            {
                Id = ItemId.DuskStone,
                Name = "Dusk Stone",
                SpriteNum = 116,
                Fling = new FlingData { BasePower = 80 },
                Num = 108,
                Gen = 4,
            },

            // E items
            [ItemId.EarthPlate] = new()
            {
                Id = ItemId.EarthPlate,
                Name = "Earth Plate",
                SpriteNum = 117,
                // OnPlate = 'Ground', // TODO: Not relevant for Gen 9 standard play
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Type == MoveType.Ground)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                OnTakeItem = new OnTakeItemEventInfo((Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion>)(
                    (_, item, pokemon, source, _) =>
                    {
                        if (source?.BaseSpecies.Num == 493 || pokemon.BaseSpecies.Num == 493)
                        {
                            return BoolVoidUnion.FromBool(false);
                        }
                        return BoolVoidUnion.FromBool(true);
                    })),
                ForcedForme = "Arceus-Ground",
                Num = 305,
                Gen = 4,
            },
            [ItemId.EjectButton] = new()
            {
                Id = ItemId.EjectButton,
                Name = "Eject Button",
                SpriteNum = 118,
                Fling = new FlingData { BasePower = 30 },
                OnAfterMoveSecondary = new OnAfterMoveSecondaryEventInfo((battle, target, source, move) =>
                {
                    if (source != null && source != target && target.Hp > 0 && move != null &&
                        move.Category != MoveCategory.Status && move.Flags.FutureMove != true)
                    {
                        if (battle.CanSwitch(target.Side) == 0 || target.ForceSwitchFlag ||
                            target.BeingCalledBack || target.Volatiles.ContainsKey(ConditionId.SkyDrop))
                            return;
                        if (target.Volatiles.ContainsKey(ConditionId.Commanding) ||
                            target.Volatiles.ContainsKey(ConditionId.Commanded))
                            return;

                        foreach (var pokemon in battle.GetAllActive())
                        {
                            if (pokemon.SwitchFlag.IsTrue()) return;
                        }

                        target.SwitchFlag = true;
                        if (target.UseItem())
                        {
                            source.SwitchFlag = false;
                        }
                        else
                        {
                            target.SwitchFlag = false;
                        }
                    }
                }, 2),
                Num = 547,
                Gen = 5,
            },
            [ItemId.EjectPack] = new()
            {
                Id = ItemId.EjectPack,
                Name = "Eject Pack",
                SpriteNum = 714,
                Fling = new FlingData { BasePower = 50 },
                OnAfterBoost = new OnAfterBoostEventInfo((battle, boost, target, source, effect) =>
                {
                    // Don't trigger if already set to eject
                    if (target.ItemState.Eject == true) return;
                    // Don't trigger from Parting Shot (user switches out anyway)
                    if (battle.ActiveMove?.Id == MoveId.PartingShot) return;
                    // Check if any stat was lowered
                    if ((boost.Atk.HasValue && boost.Atk < 0) ||
                        (boost.Def.HasValue && boost.Def < 0) ||
                        (boost.SpA.HasValue && boost.SpA < 0) ||
                        (boost.SpD.HasValue && boost.SpD < 0) ||
                        (boost.Spe.HasValue && boost.Spe < 0) ||
                        (boost.Accuracy.HasValue && boost.Accuracy < 0) ||
                        (boost.Evasion.HasValue && boost.Evasion < 0))
                    {
                        target.ItemState.Eject = true;
                    }
                }),
                OnAnySwitchIn = new OnAnySwitchInEventInfo((battle, pokemon) =>
                {
                    if (pokemon.ItemState.Eject != true) return;
                    TryUseEjectPack(battle, pokemon);
                }, -4),
                OnAnyAfterMega = new OnAnyAfterMegaEventInfo((battle, pokemon) =>
                {
                    if (pokemon.ItemState.Eject != true) return;
                    TryUseEjectPack(battle, pokemon);
                }),
                OnAnyAfterMove = new OnAnyAfterMoveEventInfo((battle, target, source, move) =>
                {
                    if (target.ItemState.Eject != true) return BoolVoidUnion.FromVoid();
                    TryUseEjectPack(battle, target);
                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((battle, pokemon, source, effect) =>
                {
                    if (pokemon.ItemState.Eject != true) return;
                    TryUseEjectPack(battle, pokemon);
                }, order: 29),
                OnUse = new OnUseEventInfo((Action<Battle, Pokemon>)((battle, pokemon) =>
                {
                    pokemon.SwitchFlag = true;
                })),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    pokemon.ItemState.Eject = null;
                }),
                Num = 1119,
                Gen = 8,
            },
            [ItemId.Electirizer] = new()
            {
                Id = ItemId.Electirizer,
                Name = "Electirizer",
                SpriteNum = 119,
                Fling = new FlingData { BasePower = 80 },
                Num = 322,
                Gen = 4,
            },
            [ItemId.ElectricSeed] = new()
            {
                Id = ItemId.ElectricSeed,
                Name = "Electric Seed",
                SpriteNum = 664,
                Fling = new FlingData { BasePower = 10 },
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    if (!pokemon.IgnoringItem() &&
                        battle.Field.IsTerrain(ConditionId.ElectricTerrain, null))
                    {
                        pokemon.UseItem();
                    }
                }),
                OnTerrainChange = new OnTerrainChangeEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.Field.IsTerrain(ConditionId.ElectricTerrain, null))
                    {
                        pokemon.UseItem();
                    }
                }),
                Boosts = new SparseBoostsTable { Def = 1 },
                Num = 881,
                Gen = 7,
            },
            [ItemId.EnigmaBerry] = new()
            {
                Id = ItemId.EnigmaBerry,
                Name = "Enigma Berry",
                SpriteNum = 124,
                IsBerry = true,
                NaturalGift = (100, "Bug"),
                OnHit = new OnHitEventInfo((battle, target, source, move) =>
                {
                    if (move != null && target.GetMoveHitData(move).TypeMod > 0)
                    {
                        if (target.EatItem())
                        {
                            battle.Heal(target.BaseMaxHp / 4);
                        }
                    }

                    return BoolEmptyVoidUnion.FromVoid();
                }),
                OnTryEatItem = new OnTryEatItemEventInfo(
                    OnTryEatItem.FromFunc((battle, item, pokemon) =>
                    {
                        var canHeal = battle.RunEvent(EventId.TryHeal, pokemon, null, battle.Effect,
                            pokemon.BaseMaxHp / 4);
                        if (canHeal is BoolRelayVar boolVar && !boolVar.Value)
                        {
                            return BoolVoidUnion.FromBool(false);
                        }

                        return BoolVoidUnion.FromVoid();
                    })),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((battle, pokemon) => { })),
                Num = 208,
                Gen = 3,
            },
            [ItemId.Eviolite] = new()
            {
                Id = ItemId.Eviolite,
                Name = "Eviolite",
                SpriteNum = 130,
                Fling = new FlingData { BasePower = 40 },
                OnModifyDef = new OnModifyDefEventInfo((battle, def, pokemon, _, _) =>
                {
                    if (pokemon.BaseSpecies.Nfe)
                    {
                        battle.ChainModify(1.5);
                        return battle.FinalModify(def);
                    }

                    return def;
                }, 2),
                OnModifySpD = new OnModifySpDEventInfo((battle, spd, pokemon, _, _) =>
                {
                    if (pokemon.BaseSpecies.Nfe)
                    {
                        battle.ChainModify(1.5);
                        return battle.FinalModify(spd);
                    }

                    return spd;
                }, 2),
                Num = 538,
                Gen = 5,
            },
            [ItemId.ExpertBelt] = new()
            {
                Id = ItemId.ExpertBelt,
                Name = "Expert Belt",
                SpriteNum = 132,
                Fling = new FlingData { BasePower = 10 },
                OnModifyDamage =
                    new OnModifyDamageEventInfo((battle, damage, source, target, move) =>
                    {
                        if (move != null && target.GetMoveHitData(move).TypeMod > 0)
                        {
                            battle.ChainModify([4915, 4096]);
                            return battle.FinalModify(damage);
                        }

                        return damage;
                    }),
                Num = 268,
                Gen = 4,
            },

            // F items
            [ItemId.FairyFeather] = new()
            {
                Id = ItemId.FairyFeather,
                Name = "Fairy Feather",
                SpriteNum = 754,
                Fling = new FlingData { BasePower = 10 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Type == MoveType.Fairy)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 2401,
                Gen = 9,
            },
            [ItemId.FastBall] = new()
            {
                Id = ItemId.FastBall,
                Name = "Fast Ball",
                SpriteNum = 137,
                Num = 492,
                Gen = 2,
                IsPokeball = true,
            },
            [ItemId.FigyBerry] = new()
            {
                Id = ItemId.FigyBerry,
                Name = "Figy Berry",
                SpriteNum = 140,
                IsBerry = true,
                NaturalGift = (80, "Bug"),
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Hp <= pokemon.MaxHp / 4 ||
                        (pokemon.Hp <= pokemon.MaxHp / 2 &&
                         pokemon.HasAbility(AbilityId.Gluttony) &&
                         pokemon.AbilityState.Gluttony == true))
                    {
                        pokemon.EatItem();
                    }
                }),
                OnTryEatItem = new OnTryEatItemEventInfo(
                    OnTryEatItem.FromFunc((battle, item, pokemon) =>
                    {
                        var canHeal = battle.RunEvent(EventId.TryHeal, pokemon, null, battle.Effect,
                            pokemon.BaseMaxHp / 3);
                        if (canHeal is BoolRelayVar boolVar && !boolVar.Value)
                        {
                            return BoolVoidUnion.FromBool(false);
                        }

                        return BoolVoidUnion.FromVoid();
                    })),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((battle, pokemon) =>
                {
                    battle.Heal(pokemon.BaseMaxHp / 3);
                    if (pokemon.Set.Nature.Minus == StatIdExceptHp.Atk)
                    {
                        pokemon.AddVolatile(ConditionId.Confusion);
                    }
                })),
                Num = 159,
                Gen = 3,
            },
            [ItemId.FireStone] = new()
            {
                Id = ItemId.FireStone,
                Name = "Fire Stone",
                SpriteNum = 142,
                Fling = new FlingData { BasePower = 30 },
                Num = 82,
                Gen = 1,
            },
            [ItemId.FistPlate] = new()
            {
                Id = ItemId.FistPlate,
                Name = "Fist Plate",
                SpriteNum = 143,
                // OnPlate = 'Fighting', // TODO: Not relevant for Gen 9 standard play
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Type == MoveType.Fighting)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                OnTakeItem = new OnTakeItemEventInfo((Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion>)(
                    (_, item, pokemon, source, _) =>
                    {
                        if (source?.BaseSpecies.Num == 493 || pokemon.BaseSpecies.Num == 493)
                        {
                            return BoolVoidUnion.FromBool(false);
                        }
                        return BoolVoidUnion.FromBool(true);
                    })),
                ForcedForme = "Arceus-Fighting",
                Num = 303,
                Gen = 4,
            },
            [ItemId.FlameOrb] = new()
            {
                Id = ItemId.FlameOrb,
                Name = "Flame Orb",
                SpriteNum = 145,
                Fling = new FlingData
                {
                    BasePower = 30,
                    Status = ConditionId.Burn,
                },
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    battle.Debug(
                        $"FlameOrb OnResidual: Called for {pokemon?.Name ?? "null pokemon"}");

                    if (pokemon == null)
                    {
                        battle.Debug("FlameOrb OnResidual: pokemon is null, returning");
                        return;
                    }

                    battle.Debug($"FlameOrb OnResidual: Calling TrySetStatus for {pokemon.Name}");

                    pokemon.TrySetStatus(ConditionId.Burn, pokemon);
                })
                {
                    Order = 28,
                    SubOrder = 3,
                },
                Num = 273,
                Gen = 4,
            },
            [ItemId.FlamePlate] = new()
            {
                Id = ItemId.FlamePlate,
                Name = "Flame Plate",
                SpriteNum = 146,
                // OnPlate = 'Fire', // TODO: Not relevant for Gen 9 standard play
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Type == MoveType.Fire)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                OnTakeItem = new OnTakeItemEventInfo((Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion>)(
                    (_, item, pokemon, source, _) =>
                    {
                        if (source?.BaseSpecies.Num == 493 || pokemon.BaseSpecies.Num == 493)
                        {
                            return BoolVoidUnion.FromBool(false);
                        }
                        return BoolVoidUnion.FromBool(true);
                    })),
                ForcedForme = "Arceus-Fire",
                Num = 298,
                Gen = 4,
            },
            [ItemId.FloatStone] = new()
            {
                Id = ItemId.FloatStone,
                Name = "Float Stone",
                SpriteNum = 147,
                Fling = new FlingData { BasePower = 30 },
                OnModifyWeight = new OnModifyWeightEventInfo((battle, weighthg, pokemon) =>
                {
                    // Halve the weight (truncate to integer)
                    return IntVoidUnion.FromInt(weighthg / 2);
                }),
                Num = 539,
                Gen = 5,
            },
            [ItemId.FlowerSweet] = new()
            {
                Id = ItemId.FlowerSweet,
                Name = "Flower Sweet",
                SpriteNum = 708,
                Fling = new FlingData { BasePower = 0 },
                Num = 1113,
                Gen = 8,
            },
            [ItemId.FocusBand] = new()
            {
                Id = ItemId.FocusBand,
                Name = "Focus Band",
                SpriteNum = 150,
                Fling = new FlingData { BasePower = 10 },
                OnDamage = new OnDamageEventInfo((battle, damage, target, source, effect) =>
                {
                    // 1/10 chance to survive a hit that would otherwise KO
                    if (battle.RandomChance(1, 10) && damage >= target.Hp &&
                        effect != null && effect.EffectType == EffectType.Move)
                    {
                        battle.Add("-activate", target, "item: Focus Band");
                        return target.Hp - 1;
                    }

                    return damage;
                }, -40),
                Num = 230,
                Gen = 2,
            },
            [ItemId.FocusSash] = new()
            {
                Id = ItemId.FocusSash,
                Name = "Focus Sash",
                SpriteNum = 151,
                Fling = new FlingData { BasePower = 10 },
                OnDamage = new OnDamageEventInfo((battle, damage, target, source, effect) =>
                {
                    // Survives one hit that would otherwise KO (when at full HP)
                    if (target.Hp == target.MaxHp && damage >= target.Hp &&
                        effect != null && effect.EffectType == EffectType.Move)
                    {
                        if (target.UseItem())
                        {
                            return target.Hp - 1;
                        }
                    }

                    return damage;
                }, -40),
                Num = 275,
                Gen = 4,
            },
            [ItemId.FriendBall] = new()
            {
                Id = ItemId.FriendBall,
                Name = "Friend Ball",
                SpriteNum = 153,
                Num = 497,
                Gen = 2,
                IsPokeball = true,
            },
        };
    }

    /// <summary>
    /// Helper method for Eject Pack to validate and trigger item use.
    /// </summary>
    private static void TryUseEjectPack(Battle battle, Pokemon pokemon)
    {
        // Can't switch if no available switches
        if (battle.CanSwitch(pokemon.Side) == 0) return;
        // Can't eject if Commanding/Commanded
        if (pokemon.Volatiles.ContainsKey(ConditionId.Commanding) ||
            pokemon.Volatiles.ContainsKey(ConditionId.Commanded)) return;
        // Can't eject if another Pokemon already has switch flag set
        foreach (var active in battle.GetAllActive())
        {
            if (active.SwitchFlag.IsTrue()) return;
        }
        // Try to use the item
        pokemon.UseItem();
    }
}