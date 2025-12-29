using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
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
    private partial Dictionary<ItemId, Item> CreateItemsJkl()
    {
        return new Dictionary<ItemId, Item>
        {
            // J items
            [ItemId.JabocaBerry] = new()
            {
                Id = ItemId.JabocaBerry,
                Name = "Jaboca Berry",
                SpriteNum = 230,
                IsBerry = true,
                NaturalGift = (100, "Dragon"),
                OnDamagingHit = new OnDamagingHitEventInfo((battle, damage, target, source, move) =>
                {
                    if (move.Category == MoveCategory.Physical && source.Hp > 0 &&
                        source.IsActive &&
                        !source.HasAbility(AbilityId.MagicGuard))
                    {
                        if (target.EatItem())
                        {
                            var damageAmount = source.BaseMaxHp /
                                               (target.HasAbility(AbilityId.Ripen) ? 4 : 8);
                            battle.Damage(damageAmount, source, target);
                        }
                    }
                }),
                // OnEat: empty function
                Num = 211,
                Gen = 4,
            },

            // K items
            [ItemId.KasibBerry] = new()
            {
                Id = ItemId.KasibBerry,
                Name = "Kasib Berry",
                SpriteNum = 233,
                IsBerry = true,
                NaturalGift = (80, "Ghost"),
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, source, target, move) =>
                    {
                        if (move.Type == MoveType.Ghost && target.GetMoveHitData(move).TypeMod > 0)
                        {
                            var hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                         move.Flags.BypassSub != true &&
                                         !(move.Infiltrates == true && battle.Gen >= 6);
                            if (hitSub) return damage;

                            if (target.EatItem())
                            {
                                battle.Debug("-50% reduction");
                                battle.Add("-enditem", target, "item: Kasib Berry", "[weaken]");
                                battle.ChainModify(0.5);
                                return battle.FinalModify(damage);
                            }
                        }

                        return damage;
                    }),
                // OnEat: empty function
                Num = 196,
                Gen = 4,
            },
            [ItemId.KebiaBerry] = new()
            {
                Id = ItemId.KebiaBerry,
                Name = "Kebia Berry",
                SpriteNum = 234,
                IsBerry = true,
                NaturalGift = (80, "Poison"),
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, source, target, move) =>
                    {
                        if (move.Type == MoveType.Poison && target.GetMoveHitData(move).TypeMod > 0)
                        {
                            var hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                         move.Flags.BypassSub != true &&
                                         !(move.Infiltrates == true && battle.Gen >= 6);
                            if (hitSub) return damage;

                            if (target.EatItem())
                            {
                                battle.Debug("-50% reduction");
                                battle.Add("-enditem", target, "item: Kebia Berry", "[weaken]");
                                battle.ChainModify(0.5);
                                return battle.FinalModify(damage);
                            }
                        }

                        return damage;
                    }),
                // OnEat: empty function
                Num = 190,
                Gen = 4,
            },
            [ItemId.KeeBerry] = new()
            {
                Id = ItemId.KeeBerry,
                Name = "Kee Berry",
                SpriteNum = 593,
                IsBerry = true,
                NaturalGift = (100, "Fairy"),
                OnAfterMoveSecondary =
                    new OnAfterMoveSecondaryEventInfo((battle, target, source, move) =>
                    {
                        if (move.Category == MoveCategory.Physical)
                        {
                            // TODO: Skip if it's Present with healing - Present not implemented yet
                            target.EatItem();
                        }
                    }),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((battle, pokemon) =>
                {
                    battle.Boost(new SparseBoostsTable { Def = 1 });
                })),
                Num = 687,
                Gen = 6,
            },
            [ItemId.KelpsyBerry] = new()
            {
                Id = ItemId.KelpsyBerry,
                Name = "Kelpsy Berry",
                SpriteNum = 235,
                IsBerry = true,
                NaturalGift = (90, "Fighting"),
                // onEat: false - used for EV reduction, not relevant in Gen 9 battles
                Num = 170,
                Gen = 3,
            },
            [ItemId.KingsRock] = new()
            {
                Id = ItemId.KingsRock,
                Name = "King's Rock",
                SpriteNum = 236,
                Fling = new FlingData
                {
                    BasePower = 30,
                },
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, pokemon, _) =>
                {
                    if (move.Category != MoveCategory.Status)
                    {
                        if (move.Secondaries == null) move.Secondaries = [];

                        // Check if flinch already exists
                        foreach (var secondary in move.Secondaries)
                        {
                            if (secondary.VolatileStatus == ConditionId.Flinch) return;
                        }

                        // Add 10% flinch chance
                        move.Secondaries = [..move.Secondaries, new SecondaryEffect
                        {
                            Chance = 10,
                            VolatileStatus = ConditionId.Flinch,
                        }];
                    }
                }, -1),
                Num = 221,
                Gen = 2,
            },

            // L items
            [ItemId.LaggingTail] = new()
            {
                Id = ItemId.LaggingTail,
                Name = "Lagging Tail",
                SpriteNum = 237,
                Fling = new FlingData { BasePower = 10 },
                // TODO: OnFractionalPriority: -0.1
                Num = 279,
                Gen = 4,
            },
            [ItemId.LansatBerry] = new()
            {
                Id = ItemId.LansatBerry,
                Name = "Lansat Berry",
                SpriteNum = 238,
                IsBerry = true,
                NaturalGift = (100, "Flying"),
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
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((battle, pokemon) =>
                {
                    pokemon.AddVolatile(ConditionId.FocusEnergy);
                })),
                Num = 206,
                Gen = 3,
            },
            [ItemId.LaxIncense] = new()
            {
                Id = ItemId.LaxIncense,
                Name = "Lax Incense",
                SpriteNum = 240,
                Fling = new FlingData { BasePower = 10 },
                OnModifyAccuracy = new OnModifyAccuracyEventInfo(
                    (battle, accuracy, target, source, move) =>
                    {
                        battle.Debug("lax incense - decreasing accuracy");
                        battle.ChainModify([3686, 4096]);
                        return battle.FinalModify(accuracy);
                    }, -2),
                Num = 255,
                Gen = 3,
                // IsNonstandard = "Past", // Not supported in Item class
            },
            [ItemId.LeafStone] = new()
            {
                Id = ItemId.LeafStone,
                Name = "Leaf Stone",
                SpriteNum = 241,
                Fling = new FlingData { BasePower = 30 },
                Num = 85,
                Gen = 1,
            },
            [ItemId.Leek] = new()
            {
                Id = ItemId.Leek,
                Name = "Leek",
                SpriteNum = 475,
                Fling = new FlingData { BasePower = 60 },
                OnModifyCritRatio =
                    new OnModifyCritRatioEventInfo((battle, critRatio, user, target, move) =>
                    {
                        var baseSpeciesName = user.BaseSpecies.Name.ToLower();
                        if (baseSpeciesName == "farfetch'd" || baseSpeciesName == "sirfetch'd" ||
                            baseSpeciesName == "farfetchd" || baseSpeciesName == "sirfetchd")
                        {
                            return critRatio + 2;
                        }

                        return critRatio;
                    }),
                Num = 259,
                Gen = 8,
                // IsNonstandard = "Past", // Not supported in Item class
            },
            [ItemId.Leftovers] = new()
            {
                Id = ItemId.Leftovers,
                Name = "Leftovers",
                SpriteNum = 242,
                Fling = new FlingData { BasePower = 10 },
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    if (pokemon == null)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Debug("Leftovers OnResidual: pokemon is null");
                        }

                        return;
                    }

                    battle.Heal(pokemon.BaseMaxHp / 16);
                })
                {
                    Order = 5,
                    SubOrder = 4,
                },
                Num = 234,
                Gen = 2,
            },
            [ItemId.LeppaBerry] = new()
            {
                Id = ItemId.LeppaBerry,
                Name = "Leppa Berry",
                SpriteNum = 244,
                IsBerry = true,
                NaturalGift = (80, "Fighting"),
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Hp == 0) return;
                    if (pokemon.MoveSlots.Any(move => move.Pp == 0))
                    {
                        pokemon.EatItem();
                    }
                }),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((battle, pokemon) =>
                {
                    var moveSlot = pokemon.MoveSlots.FirstOrDefault(move => move.Pp == 0) ??
                                   pokemon.MoveSlots.FirstOrDefault(move => move.Pp < move.MaxPp);
                    if (moveSlot == null) return;

                    moveSlot.Pp += 10;
                    if (moveSlot.Pp > moveSlot.MaxPp) moveSlot.Pp = moveSlot.MaxPp;
                    // TODO: Fix battle.Add to include move name
                    battle.Add("-activate", pokemon, "item: Leppa Berry", "[consumed]");
                })),
                Num = 154,
                Gen = 3,
            },
            [ItemId.LevelBall] = new()
            {
                Id = ItemId.LevelBall,
                Name = "Level Ball",
                SpriteNum = 246,
                Num = 493,
                Gen = 2,
                IsPokeball = true,
            },
            [ItemId.LiechiBerry] = new()
            {
                Id = ItemId.LiechiBerry,
                Name = "Liechi Berry",
                SpriteNum = 248,
                IsBerry = true,
                NaturalGift = (100, "Grass"),
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
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((battle, pokemon) =>
                {
                    battle.Boost(new SparseBoostsTable { Atk = 1 });
                })),
                Num = 201,
                Gen = 3,
            },
            [ItemId.LifeOrb] = new()
            {
                Id = ItemId.LifeOrb,
                Name = "Life Orb",
                SpriteNum = 249,
                Fling = new FlingData { BasePower = 30 },
                OnModifyDamage =
                    new OnModifyDamageEventInfo((battle, damage, source, target, move) =>
                    {
                        battle.ChainModify([5324, 4096]);
                        return battle.FinalModify(damage);
                    }),
                OnAfterMoveSecondarySelf =
                    new OnAfterMoveSecondarySelfEventInfo((battle, source, target, move) =>
                    {
                        if (source != null && source != target && move != null &&
                            move.Category != MoveCategory.Status && !source.ForceSwitchFlag)
                        {
                            // Life Orb recoil damage
                            battle.Damage(source.BaseMaxHp / 10, source, source, null);
                        }
                    }),
                Num = 270,
                Gen = 4,
            },
            [ItemId.LightBall] = new()
            {
                Id = ItemId.LightBall,
                Name = "Light Ball",
                SpriteNum = 251,
                Fling = new FlingData
                {
                    BasePower = 30,
                    Status = ConditionId.Paralysis
                },
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, pokemon, target, move) =>
                {
                    if (pokemon.BaseSpecies.Name == "Pikachu")
                    {
                        battle.ChainModify(2);
                        return battle.FinalModify(atk);
                    }

                    return atk;
                }, 1),
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, pokemon, target, move) =>
                {
                    if (pokemon.BaseSpecies.Name == "Pikachu")
                    {
                        battle.ChainModify(2);
                        return battle.FinalModify(spa);
                    }

                    return spa;
                }, 1),
                Num = 236,
                Gen = 2,
            },
            [ItemId.LightClay] = new()
            {
                Id = ItemId.LightClay,
                Name = "Light Clay",
                SpriteNum = 252,
                Fling = new FlingData { BasePower = 30 },
                // Functionality implemented in Reflect/Light Screen conditions (extends duration from 5 to 8 turns)
                Num = 269,
                Gen = 4,
            },
            [ItemId.LoadedDice] = new()
            {
                Id = ItemId.LoadedDice,
                Name = "Loaded Dice",
                SpriteNum = 751,
                Fling = new FlingData { BasePower = 30 },
                // Partially implemented in BattleActions.HitStepMoveHitLoop
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, pokemon, _) =>
                {
                    // Remove multiaccuracy property to guarantee maximum hits
                    if (move.MultiAccuracy == true)
                    {
                        move.MultiAccuracy = null;
                    }
                }),
                Num = 1886,
                Gen = 9,
            },
            [ItemId.LoveBall] = new()
            {
                Id = ItemId.LoveBall,
                Name = "Love Ball",
                SpriteNum = 258,
                Num = 496,
                Gen = 2,
                IsPokeball = true,
            },
            [ItemId.LoveSweet] = new()
            {
                Id = ItemId.LoveSweet,
                Name = "Love Sweet",
                SpriteNum = 705,
                Fling = new FlingData { BasePower = 10 },
                Num = 1110,
                Gen = 8,
            },
            [ItemId.LuckyPunch] = new()
            {
                Id = ItemId.LuckyPunch,
                Name = "Lucky Punch",
                SpriteNum = 261,
                Fling = new FlingData { BasePower = 40 },
                OnModifyCritRatio =
                    new OnModifyCritRatioEventInfo((battle, critRatio, user, target, move) =>
                    {
                        if (user.BaseSpecies.Name == "Chansey")
                        {
                            return critRatio + 2;
                        }

                        return critRatio;
                    }),
                Num = 256,
                Gen = 2,
                // IsNonstandard = "Past", // Not supported in Item class
            },
            [ItemId.LumBerry] = new()
            {
                Id = ItemId.LumBerry,
                Name = "Lum Berry",
                SpriteNum = 262,
                IsBerry = true,
                NaturalGift = (80, "Flying"),
                OnAfterSetStatus =
                    new OnAfterSetStatusEventInfo(
                        (battle, status, pokemon, source, effect) => { pokemon.EatItem(); }, -1),
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Status != ConditionId.None ||
                        pokemon.Volatiles.ContainsKey(ConditionId.Confusion))
                    {
                        pokemon.EatItem();
                    }
                }),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((battle, pokemon) =>
                {
                    pokemon.CureStatus();
                    pokemon.RemoveVolatile(battle.Library.Conditions[ConditionId.Confusion]);
                })),
                Num = 157,
                Gen = 3,
            },
            [ItemId.LuminousMoss] = new()
            {
                Id = ItemId.LuminousMoss,
                Name = "Luminous Moss",
                SpriteNum = 595,
                Fling = new FlingData { BasePower = 30 },
                OnDamagingHit = new OnDamagingHitEventInfo((battle, damage, target, source, move) =>
                {
                    if (move.Type == MoveType.Water)
                    {
                        target.UseItem();
                    }
                }),
                Boosts = new SparseBoostsTable { SpD = 1 },
                Num = 648,
                Gen = 6,
            },
            [ItemId.LureBall] = new()
            {
                Id = ItemId.LureBall,
                Name = "Lure Ball",
                SpriteNum = 264,
                Num = 494,
                Gen = 2,
                IsPokeball = true,
            },
            [ItemId.LustrousGlobe] = new()
            {
                Id = ItemId.LustrousGlobe,
                Name = "Lustrous Globe",
                SpriteNum = 742,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    // Palkia has species number 484
                    if (user.BaseSpecies.Num == 484 &&
                        (move.Type == MoveType.Water || move.Type == MoveType.Dragon))
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                OnTakeItem = new OnTakeItemEventInfo((Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion>)(
                    (_, item, pokemon, source, _) =>
                    {
                        // Palkia (num 484) can't have this item removed
                        if (source?.BaseSpecies.Num == 484 || pokemon.BaseSpecies.Num == 484)
                        {
                            return BoolVoidUnion.FromBool(false); // Prevent removal
                        }
                        return BoolVoidUnion.FromBool(true); // Allow removal
                    })),
                ForcedForme = "Palkia-Origin",
                Num = 1778,
                Gen = 8,
            },
            [ItemId.LustrousOrb] = new()
            {
                Id = ItemId.LustrousOrb,
                Name = "Lustrous Orb",
                SpriteNum = 265,
                Fling = new FlingData { BasePower = 60 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    // Palkia has species number 484
                    if (user.BaseSpecies.Num == 484 &&
                        (move.Type == MoveType.Water || move.Type == MoveType.Dragon))
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 136,
                Gen = 4,
            },
            [ItemId.LuxuryBall] = new()
            {
                Id = ItemId.LuxuryBall,
                Name = "Luxury Ball",
                SpriteNum = 266,
                Num = 11,
                Gen = 3,
                IsPokeball = true,
            },
        };
    }
}