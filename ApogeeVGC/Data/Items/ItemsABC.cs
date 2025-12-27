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
using PokemonType = ApogeeVGC.Sim.PokemonClasses.PokemonType;

namespace ApogeeVGC.Data.Items;

public partial record Items
{
    private partial Dictionary<ItemId, Item> CreateItemsAbc()
    {
        return new Dictionary<ItemId, Item>
        {
            [ItemId.AbilityShield] = new()
            {
                Id = ItemId.AbilityShield,
                Name = "Ability Shield",
                SpriteNum = 746,
                Fling = new FlingData { BasePower = 30 },
                IgnoreKlutz = true,
                // Neutralizing Gas protection implemented in Pokemon.IgnoringAbility()
                // Mold Breaker protection implemented in Battle.SuppressingAbility()
                // TODO: OnSetAbility handler - need to figure out how to return null to block ability change
                Num = 1881,
                Gen = 9,
            },
            [ItemId.AbsorbBulb] = new()
            {
                Id = ItemId.AbsorbBulb,
                Name = "Absorb Bulb",
                SpriteNum = 2,
                Fling = new FlingData { BasePower = 30 },
                OnDamagingHit = new OnDamagingHitEventInfo((battle, damage, target, source, move) =>
                {
                    if (move.Type == MoveType.Water)
                    {
                        target.UseItem();
                    }
                }),
                Boosts = new SparseBoostsTable { SpA = 1 },
                Num = 545,
                Gen = 5,
            },
            [ItemId.AdamantCrystal] = new()
            {
                Id = ItemId.AdamantCrystal,
                Name = "Adamant Crystal",
                SpriteNum = 741,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    // Dialga has species number 483
                    if (user.BaseSpecies.Num == 483 &&
                        (move.Type == MoveType.Steel || move.Type == MoveType.Dragon))
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                // OnTakeItem - Dialga can't have this item removed
                // TODO: Implement proper OnTakeItem logic to prevent item removal for Dialga
                ForcedForme = "Dialga-Origin",
                Num = 1777,
                Gen = 8,
            },
            [ItemId.AdamantOrb] = new()
            {
                Id = ItemId.AdamantOrb,
                Name = "Adamant Orb",
                SpriteNum = 4,
                Fling = new FlingData { BasePower = 60 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    // Dialga has species number 483
                    if (user.BaseSpecies.Num == 483 &&
                        (move.Type == MoveType.Steel || move.Type == MoveType.Dragon))
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 135,
                Gen = 4,
            },
            [ItemId.AdrenalineOrb] = new()
            {
                Id = ItemId.AdrenalineOrb,
                Name = "Adrenaline Orb",
                SpriteNum = 660,
                Fling = new FlingData { BasePower = 30 },
                OnAfterBoost = new OnAfterBoostEventInfo((battle, boost, target, source, effect) =>
                {
                    // Adrenaline Orb activates if Intimidate is blocked by an ability like Hyper Cutter,
                    // which deletes boost.atk,
                    // but not if the holder's attack is already at -6 (or +6 if it has Contrary),
                    // which sets boost.atk to 0
                    if (target.Boosts.GetBoost(BoostId.Spe) == 6 ||
                        boost.GetBoost(BoostId.Atk) == 0)
                    {
                        return;
                    }

                    if (effect.Name == "Intimidate")
                    {
                        target.UseItem();
                    }
                }),
                Boosts = new SparseBoostsTable { Spe = 1 },
                Num = 846,
                Gen = 7,
            },
            [ItemId.AguavBerry] = new()
            {
                Id = ItemId.AguavBerry,
                Name = "Aguav Berry",
                SpriteNum = 5,
                IsBerry = true,
                NaturalGift = (80, "Dragon"),
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
                    // TODO: Get nature from Pokemon and check if minus stat is SpD
                    // if (pokemon.GetNature().Minus == StatIdExceptHp.SpD)
                    // {
                    //     pokemon.AddVolatile(ConditionId.Confusion);
                    // }
                })),
                Num = 162,
                Gen = 3,
            },
            [ItemId.AirBalloon] = new()
            {
                Id = ItemId.AirBalloon,
                Name = "Air Balloon",
                SpriteNum = 6,
                Fling = new FlingData { BasePower = 10 },
                OnStart = new OnStartEventInfo((battle, target) =>
                {
                    if (!target.IgnoringItem() &&
                        battle.Field.GetPseudoWeather(ConditionId.Gravity) == null)
                    {
                        battle.Add("-item", target, "Air Balloon");
                    }
                }),
                // Airborneness implemented in Pokemon.IsGrounded()
                OnDamagingHit = new OnDamagingHitEventInfo((battle, damage, target, source, move) =>
                {
                    battle.Add("-enditem", target, "Air Balloon");
                    target.Item = ItemId.None;
                    var itemState = target.ItemState;
                    battle.ClearEffectState(ref itemState);
                    battle.RunEvent(EventId.AfterUseItem, target, null, null,
                        _library.Items[ItemId.AirBalloon]);
                }),
                OnAfterSubDamage =
                    new OnAfterSubDamageEventInfo((battle, damage, target, source, effect) =>
                    {
                        battle.Debug($"effect: {effect.Id}");
                        if (effect.EffectType == EffectType.Move)
                        {
                            battle.Add("-enditem", target, "Air Balloon");
                            target.Item = ItemId.None;
                            var itemState = target.ItemState;
                            battle.ClearEffectState(ref itemState);
                            battle.RunEvent(EventId.AfterUseItem, target, null, null,
                                _library.Items[ItemId.AirBalloon]);
                        }
                    }),
                Num = 541,
                Gen = 5,
            },
            [ItemId.ApicotBerry] = new()
            {
                Id = ItemId.ApicotBerry,
                Name = "Apicot Berry",
                SpriteNum = 10,
                IsBerry = true,
                NaturalGift = (100, "Ground"),
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
                    battle.Boost(new SparseBoostsTable { SpD = 1 });
                })),
                Num = 205,
                Gen = 3,
            },
            [ItemId.AspearBerry] = new()
            {
                Id = ItemId.AspearBerry,
                Name = "Aspear Berry",
                SpriteNum = 13,
                IsBerry = true,
                NaturalGift = (80, "Ice"),
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.Freeze)
                    {
                        pokemon.EatItem();
                    }
                }),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.Freeze)
                    {
                        pokemon.CureStatus();
                    }
                })),
                Num = 153,
                Gen = 3,
            },
            [ItemId.AuspiciousArmor] = new()
            {
                Id = ItemId.AuspiciousArmor,
                Name = "Auspicious Armor",
                SpriteNum = 753,
                Fling = new FlingData { BasePower = 30 },
                Num = 2344,
                Gen = 9,
            },
            [ItemId.AssaultVest] = new()
            {
                Id = ItemId.AssaultVest,
                Name = "Assault Vest",
                SpriteNum = 581,
                Fling = new FlingData { BasePower = 80 },
                //OnModifySpDPriority = 1,
                OnModifySpD = new OnModifySpDEventInfo((battle, spd, _, _, _) =>
                {
                    battle.ChainModify(1.5);
                    return battle.FinalModify(spd);
                }, 1),
                OnDisableMove = new OnDisableMoveEventInfo((_, pokemon) =>
                {
                    foreach (MoveSlot moveSlot in from moveSlot in pokemon.MoveSlots
                             let move = _library.Moves[moveSlot.Move]
                             where move.Category == MoveCategory.Status && move.Id != MoveId.MeFirst
                             select moveSlot)
                    {
                        pokemon.DisableMove(moveSlot.Id);
                    }
                }),
                Num = 640,
                Gen = 6,
            },

            // B items
            [ItemId.BabiriBerry] = new()
            {
                Id = ItemId.BabiriBerry,
                Name = "Babiri Berry",
                SpriteNum = 17,
                IsBerry = true,
                NaturalGift = (80, "Steel"),
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, source, target, move) =>
                    {
                        if (move.Type == MoveType.Steel && target.GetMoveHitData(move).TypeMod > 0)
                        {
                            var hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                         move.Flags.BypassSub != true &&
                                         !(move.Infiltrates == true && battle.Gen >= 6);
                            if (hitSub) return damage;

                            if (target.EatItem())
                            {
                                battle.Debug("-50% reduction");
                                battle.Add("-enditem", target, "item: Babiri Berry", "[weaken]");
                                battle.ChainModify(0.5);
                                return battle.FinalModify(damage);
                            }
                        }

                        return damage;
                    }),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((_, _) => { })),
                Num = 199,
                Gen = 4,
            },
            [ItemId.BeastBall] = new()
            {
                Id = ItemId.BeastBall,
                Name = "Beast Ball",
                SpriteNum = 661,
                IsPokeball = true,
                Num = 851,
                Gen = 7,
            },
            [ItemId.BerrySweet] = new()
            {
                Id = ItemId.BerrySweet,
                Name = "Berry Sweet",
                SpriteNum = 706,
                Fling = new FlingData { BasePower = 10 },
                Num = 1111,
                Gen = 8,
            },
            [ItemId.BigNugget] = new()
            {
                Id = ItemId.BigNugget,
                Name = "Big Nugget",
                SpriteNum = 27,
                Fling = new FlingData { BasePower = 130 },
                Num = 581,
                Gen = 5,
            },
            [ItemId.BigRoot] = new()
            {
                Id = ItemId.BigRoot,
                Name = "Big Root",
                SpriteNum = 29,
                Fling = new FlingData { BasePower = 10 },
                OnTryHeal = new OnTryHealEventInfo(
                    (Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>)((battle, damage,
                        target, source, effect) =>
                    {
                        var heals = new[]
                            { "drain", "leechseed", "ingrain", "aquaring", "strengthsap" };
                        var effectName = effect.Name.ToLower().Replace(" ", "");
                        if (heals.Contains(effectName))
                        {
                            battle.ChainModify([5324, 4096]);
                            return IntBoolUnion.FromInt(battle.FinalModify(damage));
                        }

                        return IntBoolUnion.FromInt(damage);
                    }), 1),
                Num = 296,
                Gen = 4,
            },
            [ItemId.BindingBand] = new()
            {
                Id = ItemId.BindingBand,
                Name = "Binding Band",
                SpriteNum = 31,
                Fling = new FlingData { BasePower = 30 },
                // TODO: implemented in statuses
                Num = 544,
                Gen = 5,
            },
            [ItemId.BlackBelt] = new()
            {
                Id = ItemId.BlackBelt,
                Name = "Black Belt",
                SpriteNum = 32,
                Fling = new FlingData { BasePower = 30 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Type == MoveType.Fighting)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 241,
                Gen = 2,
            },
            [ItemId.BlackGlasses] = new()
            {
                Id = ItemId.BlackGlasses,
                Name = "Black Glasses",
                SpriteNum = 35,
                Fling = new FlingData { BasePower = 30 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Type == MoveType.Dark)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 240,
                Gen = 2,
            },
            [ItemId.BlackSludge] = new()
            {
                Id = ItemId.BlackSludge,
                Name = "Black Sludge",
                SpriteNum = 34,
                Fling = new FlingData { BasePower = 30 },
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    if (pokemon.HasType(PokemonType.Poison))
                    {
                        battle.Heal(pokemon.BaseMaxHp / 16);
                    }
                    else
                    {
                        battle.Damage(pokemon.BaseMaxHp / 8);
                    }
                }, order: 5, subOrder: 4),
                Num = 281,
                Gen = 4,
            },
            [ItemId.BlueOrb] = new()
            {
                Id = ItemId.BlueOrb,
                Name = "Blue Orb",
                SpriteNum = 41,
                OnSwitchIn = new OnSwitchInEventInfo((battle, pokemon) =>
                {
                    if (pokemon.IsActive && pokemon.BaseSpecies.Name == "Kyogre" &&
                        !pokemon.Transformed)
                    {
                        // TODO: pokemon.FormeChange("Kyogre-Primal", battle.Effect, true);
                    }
                }, -1),
                // TODO: OnTakeItem - Kyogre can't have this item removed
                IsPrimalOrb = true,
                Num = 535,
                Gen = 6,
            },
            [ItemId.BlunderPolicy] = new()
            {
                Id = ItemId.BlunderPolicy,
                Name = "Blunder Policy",
                SpriteNum = 716,
                Fling = new FlingData { BasePower = 80 },
                // TODO: Item activation located in scripts - activates when move misses
                // Boosts Speed by 2 stages when the holder's move misses due to accuracy
                Num = 1121,
                Gen = 8,
            },
            [ItemId.BoosterEnergy] = new()
            {
                Id = ItemId.BoosterEnergy,
                Name = "Booster Energy",
                SpriteNum = 745,
                Fling = new FlingData { BasePower = 30 },
                OnSwitchIn = new OnSwitchInEventInfo((battle, pokemon) =>
                {
                    // Check conditions on switch-in
                    if (pokemon.Transformed) return;

                    if (pokemon.HasAbility(AbilityId.Protosynthesis) &&
                        !battle.Field.IsWeather(ConditionId.SunnyDay) &&
                        pokemon.UseItem())
                    {
                        pokemon.AddVolatile(ConditionId.Protosynthesis);
                    }
                    else if (pokemon.HasAbility(AbilityId.QuarkDrive) &&
                             !battle.Field.IsTerrain(ConditionId.ElectricTerrain, null) &&
                             pokemon.UseItem())
                    {
                        pokemon.AddVolatile(ConditionId.QuarkDrive);
                    }
                }, -2),
                // TODO: OnUpdate for turn-by-turn checks
                // TODO: OnTakeItem - only Paradox Pokemon can have this removed
                Num = 1880,
                Gen = 9,
            },
            [ItemId.BottleCap] = new()
            {
                Id = ItemId.BottleCap,
                Name = "Bottle Cap",
                SpriteNum = 696,
                Fling = new FlingData { BasePower = 30 },
                Num = 795,
                Gen = 7,
            },
            [ItemId.BrightPowder] = new()
            {
                Id = ItemId.BrightPowder,
                Name = "Bright Powder",
                SpriteNum = 51,
                Fling = new FlingData { BasePower = 10 },
                OnModifyAccuracy = new OnModifyAccuracyEventInfo((battle, accuracy, _, _, _) =>
                {
                    battle.Debug("brightpowder - decreasing accuracy");
                    battle.ChainModify([3686, 4096]);
                    return battle.FinalModify(accuracy);
                }, -2),
                Num = 213,
                Gen = 2,
            },

            // C items
            [ItemId.CellBattery] = new()
            {
                Id = ItemId.CellBattery,
                Name = "Cell Battery",
                SpriteNum = 60,
                Fling = new FlingData { BasePower = 30 },
                OnDamagingHit = new OnDamagingHitEventInfo((battle, damage, target, source, move) =>
                {
                    if (move.Type == MoveType.Electric)
                    {
                        target.UseItem();
                    }
                }),
                Boosts = new SparseBoostsTable { Atk = 1 },
                Num = 546,
                Gen = 5,
            },
            [ItemId.Charcoal] = new()
            {
                Id = ItemId.Charcoal,
                Name = "Charcoal",
                SpriteNum = 61,
                Fling = new FlingData { BasePower = 30 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Type == MoveType.Fire)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 249,
                Gen = 2,
            },
            [ItemId.ChartiBerry] = new()
            {
                Id = ItemId.ChartiBerry,
                Name = "Charti Berry",
                SpriteNum = 62,
                IsBerry = true,
                NaturalGift = (80, "Rock"),
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, source, target, move) =>
                    {
                        if (move.Type == MoveType.Rock && target.GetMoveHitData(move).TypeMod > 0)
                        {
                            var hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                         move.Flags.BypassSub != true &&
                                         !(move.Infiltrates == true && battle.Gen >= 6);
                            if (hitSub) return damage;

                            if (target.EatItem())
                            {
                                battle.Debug("-50% reduction");
                                battle.Add("-enditem", target, "item: Charti Berry", "[weaken]");
                                battle.ChainModify(0.5);
                                return battle.FinalModify(damage);
                            }
                        }

                        return damage;
                    }),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((_, _) => { })),
                Num = 195,
                Gen = 4,
            },
            [ItemId.CheriBerry] = new()
            {
                Id = ItemId.CheriBerry,
                Name = "Cheri Berry",
                SpriteNum = 63,
                IsBerry = true,
                NaturalGift = (80, "Fire"),
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.Paralysis)
                    {
                        pokemon.EatItem();
                    }
                }),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.Paralysis)
                    {
                        pokemon.CureStatus();
                    }
                })),
                Num = 149,
                Gen = 3,
            },
            [ItemId.CherishBall] = new()
            {
                Id = ItemId.CherishBall,
                Name = "Cherish Ball",
                SpriteNum = 64,
                IsPokeball = true,
                Num = 16,
                Gen = 4,
                // IsNonstandard = "Unobtainable", // Event-only but valid in Gen 9
            },
            [ItemId.ChestoBerry] = new()
            {
                Id = ItemId.ChestoBerry,
                Name = "Chesto Berry",
                SpriteNum = 65,
                IsBerry = true,
                NaturalGift = (80, "Water"),
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.Sleep)
                    {
                        pokemon.EatItem();
                    }
                }),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.Sleep)
                    {
                        pokemon.CureStatus();
                    }
                })),
                Num = 150,
                Gen = 3,
            },
            [ItemId.ChilanBerry] = new()
            {
                Id = ItemId.ChilanBerry,
                Name = "Chilan Berry",
                SpriteNum = 66,
                IsBerry = true,
                NaturalGift = (80, "Normal"),
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, source, target, move) =>
                    {
                        if (move.Type == MoveType.Normal &&
                            (!target.Volatiles.ContainsKey(ConditionId.Substitute) ||
                             move.Flags.BypassSub == true ||
                             (move.Infiltrates == true && battle.Gen >= 6)))
                        {
                            if (target.EatItem())
                            {
                                battle.Debug("-50% reduction");
                                battle.Add("-enditem", target, "item: Chilan Berry", "[weaken]");
                                battle.ChainModify(0.5);
                                return battle.FinalModify(damage);
                            }
                        }

                        return damage;
                    }),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((_, _) => { })),
                Num = 200,
                Gen = 4,
            },
            [ItemId.ChippedPot] = new()
            {
                Id = ItemId.ChippedPot,
                Name = "Chipped Pot",
                SpriteNum = 720,
                Fling = new FlingData { BasePower = 80 },
                Num = 1254,
                Gen = 8,
            },
            [ItemId.ChoiceBand] = new()
            {
                Id = ItemId.ChoiceBand,
                Name = "Choice Band",
                SpriteNum = 68,
                Fling = new FlingData { BasePower = 10 },
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Volatiles.ContainsKey(ConditionId.ChoiceLock))
                    {
                        battle.Debug("ChoiceBand: Removing existing choicelock on switch-in");
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.ChoiceLock]);
                    }
                }),
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, pokemon, _) =>
                {
                    pokemon.AddVolatile(ConditionId.ChoiceLock);
                    if (pokemon.Volatiles.ContainsKey(ConditionId.ChoiceLock) &&
                        pokemon.Volatiles[ConditionId.ChoiceLock].Move == null)
                    {
                        battle.Debug(
                            $"[ChoiceBand.OnModifyMove] {pokemon.Name}: Setting locked move to {move.Id}");
                        pokemon.Volatiles[ConditionId.ChoiceLock].Move = move.Id;
                    }
                }),
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, _, pokemon, _) =>
                {
                    // TODO: Check if pokemon has dynamax volatile
                    // if (pokemon.Volatiles.ContainsKey(ConditionId.Dynamax)) return atk;
                    battle.ChainModify(1.5);
                    return battle.FinalModify(atk);
                }, 1),
                IsChoice = true,
                Num = 220,
                Gen = 3,
            },
            [ItemId.ChoiceScarf] = new()
            {
                Id = ItemId.ChoiceScarf,
                Name = "Choice Scarf",
                SpriteNum = 69,
                Fling = new FlingData { BasePower = 10 },
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Volatiles.ContainsKey(ConditionId.ChoiceLock))
                    {
                        battle.Debug("ChoiceScarf: Removing existing choicelock on switch-in");
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.ChoiceLock]);
                    }
                }),
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, pokemon, _) =>
                {
                    pokemon.AddVolatile(ConditionId.ChoiceLock);
                    if (pokemon.Volatiles.ContainsKey(ConditionId.ChoiceLock) &&
                        pokemon.Volatiles[ConditionId.ChoiceLock].Move == null)
                    {
                        battle.Debug(
                            $"[ChoiceScarf.OnModifyMove] {pokemon.Name}: Setting locked move to {move.Id}");
                        pokemon.Volatiles[ConditionId.ChoiceLock].Move = move.Id;
                    }
                }),
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
                {
                    // TODO: Check if pokemon has dynamax volatile
                    // if (pokemon.Volatiles.ContainsKey(ConditionId.Dynamax)) return IntVoidUnion.FromInt(spe);
                    battle.ChainModify(1.5);
                    return IntVoidUnion.FromInt(battle.FinalModify(spe));
                }),
                IsChoice = true,
                Num = 287,
                Gen = 4,
            },
            [ItemId.ChoiceSpecs] = new()
            {
                Id = ItemId.ChoiceSpecs,
                Name = "Choice Specs",
                SpriteNum = 70,
                Fling = new FlingData { BasePower = 10 },
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    // Remove any existing choice lock when this Pokemon enters battle
                    // This allows switching to reset the choice lock
                    if (pokemon.Volatiles.ContainsKey(ConditionId.ChoiceLock))
                    {
                        battle.Debug("ChoiceSpecs: Removing existing choicelock on switch-in");

                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.ChoiceLock]);
                    }
                }),
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, pokemon, _) =>
                {
                    pokemon.AddVolatile(ConditionId.ChoiceLock);

                    // Set the locked move immediately after adding the volatile
                    if (pokemon.Volatiles.ContainsKey(ConditionId.ChoiceLock) &&
                        pokemon.Volatiles[ConditionId.ChoiceLock].Move == null)
                    {
                        battle.Debug(
                            $"[ChoiceSpecs.OnModifyMove] {pokemon.Name}: Setting locked move to {move.Id}");

                        pokemon.Volatiles[ConditionId.ChoiceLock].Move = move.Id;
                    }
                }),
                //OnModifySpAPriority = 1,
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, _, _, _) =>
                {
                    battle.ChainModify(1.5);
                    return battle.FinalModify(spa);
                }, 1),
                IsChoice = true,
                Num = 297,
                Gen = 4,
            },
            [ItemId.ChopleBerry] = new()
            {
                Id = ItemId.ChopleBerry,
                Name = "Chople Berry",
                SpriteNum = 71,
                IsBerry = true,
                NaturalGift = (80, "Fighting"),
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, source, target, move) =>
                    {
                        if (move.Type == MoveType.Fighting &&
                            target.GetMoveHitData(move).TypeMod > 0)
                        {
                            var hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                         move.Flags.BypassSub != true &&
                                         !(move.Infiltrates == true && battle.Gen >= 6);
                            if (hitSub) return damage;

                            if (target.EatItem())
                            {
                                battle.Debug("-50% reduction");
                                battle.Add("-enditem", target, "item: Chople Berry", "[weaken]");
                                battle.ChainModify(0.5);
                                return battle.FinalModify(damage);
                            }
                        }

                        return damage;
                    }),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((_, _) => { })),
                Num = 189,
                Gen = 4,
            },
            [ItemId.ClearAmulet] = new()
            {
                Id = ItemId.ClearAmulet,
                Name = "Clear Amulet",
                SpriteNum = 747,
                Fling = new FlingData { BasePower = 30 },
                OnTryBoost = new OnTryBoostEventInfo((battle, boost, target, source, effect) =>
                {
                    if (source != null && target == source) return;
                    bool showMsg = false;

                    var boostsCopy = new SparseBoostsTable
                    {
                        Atk = boost.Atk,
                        Def = boost.Def,
                        SpA = boost.SpA,
                        SpD = boost.SpD,
                        Spe = boost.Spe,
                        Accuracy = boost.Accuracy,
                        Evasion = boost.Evasion
                    };

                    if (boost.Atk < 0)
                    {
                        boostsCopy.Atk = null;
                        showMsg = true;
                    }

                    if (boost.Def < 0)
                    {
                        boostsCopy.Def = null;
                        showMsg = true;
                    }

                    if (boost.SpA < 0)
                    {
                        boostsCopy.SpA = null;
                        showMsg = true;
                    }

                    if (boost.SpD < 0)
                    {
                        boostsCopy.SpD = null;
                        showMsg = true;
                    }

                    if (boost.Spe < 0)
                    {
                        boostsCopy.Spe = null;
                        showMsg = true;
                    }

                    if (boost.Accuracy < 0)
                    {
                        boostsCopy.Accuracy = null;
                        showMsg = true;
                    }

                    if (boost.Evasion < 0)
                    {
                        boostsCopy.Evasion = null;
                        showMsg = true;
                    }

                    if (showMsg && effect is Move move && move.Secondaries == null &&
                        effect.EffectStateId != ConditionId.Octolock)
                    {
                        battle.Add("-fail", target, "unboost", "[from] item: Clear Amulet",
                            $"[of] {target}");
                    }

                    // Return the modified boosts
                    boost.Atk = boostsCopy.Atk;
                    boost.Def = boostsCopy.Def;
                    boost.SpA = boostsCopy.SpA;
                    boost.SpD = boostsCopy.SpD;
                    boost.Spe = boostsCopy.Spe;
                    boost.Accuracy = boostsCopy.Accuracy;
                    boost.Evasion = boostsCopy.Evasion;
                }, 1),
                Num = 1882,
                Gen = 9,
            },
            [ItemId.CloverSweet] = new()
            {
                Id = ItemId.CloverSweet,
                Name = "Clover Sweet",
                SpriteNum = 707,
                Fling = new FlingData { BasePower = 10 },
                Num = 1112,
                Gen = 8,
            },
            [ItemId.CobaBerry] = new()
            {
                Id = ItemId.CobaBerry,
                Name = "Coba Berry",
                SpriteNum = 76,
                IsBerry = true,
                NaturalGift = (80, "Flying"),
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, source, target, move) =>
                    {
                        if (move.Type == MoveType.Flying && target.GetMoveHitData(move).TypeMod > 0)
                        {
                            var hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                         move.Flags.BypassSub != true &&
                                         !(move.Infiltrates == true && battle.Gen >= 6);
                            if (hitSub) return damage;

                            if (target.EatItem())
                            {
                                battle.Debug("-50% reduction");
                                battle.Add("-enditem", target, "item: Coba Berry", "[weaken]");
                                battle.ChainModify(0.5);
                                return battle.FinalModify(damage);
                            }
                        }

                        return damage;
                    }),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((_, _) => { })),
                Num = 192,
                Gen = 4,
            },
            [ItemId.ColburBerry] = new()
            {
                Id = ItemId.ColburBerry,
                Name = "Colbur Berry",
                SpriteNum = 78,
                IsBerry = true,
                NaturalGift = (80, "Dark"),
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, source, target, move) =>
                    {
                        if (move.Type == MoveType.Dark && target.GetMoveHitData(move).TypeMod > 0)
                        {
                            var hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                         move.Flags.BypassSub != true &&
                                         !(move.Infiltrates == true && battle.Gen >= 6);
                            if (hitSub) return damage;

                            if (target.EatItem())
                            {
                                battle.Debug("-50% reduction");
                                battle.Add("-enditem", target, "item: Colbur Berry", "[weaken]");
                                battle.ChainModify(0.5);
                                return battle.FinalModify(damage);
                            }
                        }

                        return damage;
                    }),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((_, _) => { })),
                Num = 198,
                Gen = 4,
            },
            [ItemId.CornerstoneMask] = new()
            {
                Id = ItemId.CornerstoneMask,
                Name = "Cornerstone Mask",
                SpriteNum = 758,
                Fling = new FlingData { BasePower = 60 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (user.BaseSpecies.Name.StartsWith("Ogerpon-Cornerstone"))
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                // TODO: OnTakeItem - Ogerpon can't have this item removed
                ForcedForme = "Ogerpon-Cornerstone",
                Num = 2406,
                Gen = 9,
            },
            [ItemId.CovertCloak] = new()
            {
                Id = ItemId.CovertCloak,
                Name = "Covert Cloak",
                SpriteNum = 750,
                Fling = new FlingData { BasePower = 30 },
                OnModifySecondaries =
                    new OnModifySecondariesEventInfo((battle, secondaries, _, _, _) =>
                    {
                        battle.Debug("Covert Cloak prevent secondary");
                        secondaries.RemoveAll(effect => effect.Self == null);
                    }),
                Num = 1885,
                Gen = 9,
            },
            [ItemId.CrackedPot] = new()
            {
                Id = ItemId.CrackedPot,
                Name = "Cracked Pot",
                SpriteNum = 719,
                Fling = new FlingData { BasePower = 80 },
                Num = 1253,
                Gen = 8,
            },
            [ItemId.CustapBerry] = new()
            {
                Id = ItemId.CustapBerry,
                Name = "Custap Berry",
                SpriteNum = 86,
                IsBerry = true,
                NaturalGift = (100, "Ghost"),
                OnFractionalPriority = new OnFractionalPriorityEventInfo(
                    (ModifierSourceMoveHandler)((battle, priority, source, target, move) =>
                    {
                        if (priority <= 0 &&
                            (source.Hp <= source.MaxHp / 4 ||
                             (source.Hp <= source.MaxHp / 2 &&
                              source.HasAbility(AbilityId.Gluttony) &&
                              source.AbilityState.Gluttony == true)))
                        {
                            if (source.EatItem())
                            {
                                battle.Add("-activate", source, "item: Custap Berry", "[consumed]");
                                return DoubleVoidUnion.FromDouble(0.1);
                            }
                        }

                        return DoubleVoidUnion.FromVoid();
                    }), -2),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((_, _) => { })),
                Num = 210,
                Gen = 4,
            },
        };
    }
}