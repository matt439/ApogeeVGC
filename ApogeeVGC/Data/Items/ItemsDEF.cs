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

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract

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
            [ItemId.DestinyKnot] = new()
            {
                Id = ItemId.DestinyKnot,
                Name = "Destiny Knot",
                SpriteNum = 95,
                Fling = new FlingData { BasePower = 10 },
                OnAttract = new OnAttractEventInfo((battle, target, source) =>
                {
                    battle.Debug($"attract intercepted: {target} from {source}");
                    if (source == null || source == target) return;
                    if (!source.Volatiles.ContainsKey(ConditionId.Attract))
                    {
                        source.AddVolatile(ConditionId.Attract, target);
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
                OnPlate = PokemonType.Dragon,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Dragon)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                OnTakeItem = new OnTakeItemEventInfo(
                    (Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion>)(
                        (_, _, pokemon, source, _) =>
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
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
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
                OnPlate = PokemonType.Dark,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Dark)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                OnTakeItem = new OnTakeItemEventInfo(
                    (Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion>)(
                        (_, _, pokemon, source, _) =>
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
                OnPlate = PokemonType.Ground,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Ground)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                OnTakeItem = new OnTakeItemEventInfo(
                    (Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion>)(
                        (_, _, pokemon, source, _) =>
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
                OnAfterMoveSecondary = new OnAfterMoveSecondaryEventInfo(
                    (battle, target, source, move) =>
                    {
                        if (source != null && source != target && target.Hp > 0 && move != null &&
                            move.Category != MoveCategory.Status && move.Flags.FutureMove != true)
                        {
                            if (battle.CanSwitch(target.Side) == 0 || target.ForceSwitchFlag ||
                                target.BeingCalledBack)
                                return;
                            if (target.Volatiles.ContainsKey(ConditionId.Commanding) ||
                                target.Volatiles.ContainsKey(ConditionId.Commanded))
                                return;

                            foreach (Pokemon pokemon in battle.GetAllActive())
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
                OnAfterBoost = new OnAfterBoostEventInfo((battle, boost, _, _, _) =>
                {
                    // Don't trigger if already set to eject
                    // Note: OnAfterBoost is called with target = item holder, so we can use
                    // battle.EffectState which is set to the item holder's ItemState
                    if (battle.EffectState.Eject == true) return;
                    // Don't trigger from Parting Shot (user switches out anyway)
                    if (battle.ActiveMove?.Id == MoveId.PartingShot) return;
                    // Check if any stat was lowered
                    if (boost.Atk is < 0 ||
                        boost.Def is < 0 ||
                        boost.SpA is < 0 ||
                        boost.SpD is < 0 ||
                        boost.Spe is < 0 ||
                        boost.Accuracy is < 0 ||
                        boost.Evasion is < 0)
                    {
                        battle.EffectState.Eject = true;
                    }
                }),
                // OnAny* handlers: the pokemon parameter is the event target (Pokemon switching in/moving),
                // NOT the item holder. Use battle.EffectState.Target to get the item holder.
                OnAnySwitchIn = new OnAnySwitchInEventInfo((battle, _) =>
                {
                    if (battle.EffectState.Eject != true) return;
                    if (battle.EffectState.Target is not PokemonEffectStateTarget target) return;
                    target.Pokemon.UseItem();
                }, -4),
                OnAnyAfterMega = new OnAnyAfterMegaEventInfo((battle, _) =>
                {
                    if (battle.EffectState.Eject != true) return;
                    if (battle.EffectState.Target is not PokemonEffectStateTarget target) return;
                    target.Pokemon.UseItem();
                }),
                OnAnyAfterMove = new OnAnyAfterMoveEventInfo((battle, _, _, _) =>
                {
                    if (battle.EffectState.Eject != true) return BoolVoidUnion.FromVoid();
                    if (battle.EffectState.Target is not PokemonEffectStateTarget target)
                        return BoolVoidUnion.FromVoid();
                    target.Pokemon.UseItem();
                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((battle, _, _, _) =>
                {
                    if (battle.EffectState.Eject != true) return;
                    if (battle.EffectState.Target is not PokemonEffectStateTarget target) return;
                    target.Pokemon.UseItem();
                }, order: 29),
                // OnUse validates whether item can be used, returning false blocks the use
                OnUse = new OnUseEventInfo((Func<Battle, Pokemon, BoolVoidUnion>)((battle, pokemon) =>
                {
                    // Can't switch if no available switches
                    if (battle.CanSwitch(pokemon.Side) == 0) return BoolVoidUnion.FromBool(false);
                    // Can't eject if Commanding/Commanded
                    if (pokemon.Volatiles.ContainsKey(ConditionId.Commanding) ||
                        pokemon.Volatiles.ContainsKey(ConditionId.Commanded))
                    {
                        return BoolVoidUnion.FromBool(false);
                    }

                    // Can't eject if another Pokemon already has switch flag set
                    foreach (Pokemon active in battle.GetAllActive())
                    {
                        if (active.SwitchFlag.IsTrue()) return BoolVoidUnion.FromBool(false);
                    }

                    // Allow use and set switch flag
                    pokemon.SwitchFlag = true;
                    return BoolVoidUnion.FromBool(true);
                })),
                OnEnd = new OnEndEventInfo((battle, _) => { battle.EffectState.Eject = null; }),
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
                OnSwitchIn = new OnSwitchInEventInfo((battle, pokemon) =>
                {
                    if (!pokemon.IgnoringItem() &&
                        battle.Field.IsTerrain(ConditionId.ElectricTerrain, null))
                    {
                        pokemon.UseItem();
                    }
                }, -1),
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
                OnHit = new OnHitEventInfo((battle, target, _, move) =>
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
                    OnTryEatItem.FromFunc((battle, _, pokemon) =>
                    {
                        RelayVar? canHeal = battle.RunEvent(EventId.TryHeal, pokemon, null,
                            battle.Effect,
                            pokemon.BaseMaxHp / 4);
                        if (canHeal is BoolRelayVar { Value: false })
                        {
                            return BoolVoidUnion.FromBool(false);
                        }

                        return BoolVoidUnion.FromVoid();
                    })),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((_, _) => { })),
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
                    new OnModifyDamageEventInfo((battle, damage, _, target, move) =>
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
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
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
                OnUpdate = new OnUpdateEventInfo((_, pokemon) =>
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
                    OnTryEatItem.FromFunc((battle, _, pokemon) =>
                    {
                        RelayVar? canHeal = battle.RunEvent(EventId.TryHeal, pokemon, null,
                            battle.Effect,
                            pokemon.BaseMaxHp / 3);
                        if (canHeal is BoolRelayVar { Value: false })
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
                OnPlate = PokemonType.Fighting,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Fighting)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                OnTakeItem = new OnTakeItemEventInfo(
                    (Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion>)(
                        (_, _, pokemon, source, _) =>
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
                OnPlate = PokemonType.Fire,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Fire)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                OnTakeItem = new OnTakeItemEventInfo(
                    (Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion>)(
                        (_, _, pokemon, source, _) =>
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
                OnModifyWeight = new OnModifyWeightEventInfo((_, weighthg, _) =>
                    IntVoidUnion.FromInt(weighthg / 2)),
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
                OnDamage = new OnDamageEventInfo((battle, damage, target, _, effect) =>
                {
                    // 1/10 chance to survive a hit that would otherwise KO
                    if (battle.RandomChance(1, 10) && damage >= target.Hp &&
                        effect is { EffectType: EffectType.Move })
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
                OnDamage = new OnDamageEventInfo((_, damage, target, _, effect) =>
                {
                    // Survives one hit that would otherwise KO (when at full HP)
                    if (target.Hp == target.MaxHp && damage >= target.Hp &&
                        effect is { EffectType: EffectType.Move })
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
}