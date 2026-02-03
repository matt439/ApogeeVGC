using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.ItemSpecific;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace ApogeeVGC.Data.Items;

public partial record Items
{
    private partial Dictionary<ItemId, Item> CreateItemsStu()
    {
        return new Dictionary<ItemId, Item>
        {
            // S items
            [ItemId.SafariBall] = new()
            {
                Id = ItemId.SafariBall,
                Name = "Safari Ball",
                SpriteNum = 425,
                Num = 5,
                Gen = 1,
                IsPokeball = true,
            },
            [ItemId.SafetyGoggles] = new()
            {
                Id = ItemId.SafetyGoggles,
                Name = "Safety Goggles",
                SpriteNum = 604,
                Fling = new FlingData { BasePower = 80 },
                OnImmunity = new OnImmunityEventInfo((_, type, _) =>
                {
                    if (type == ConditionId.Sandstorm ||
                        type == ConditionId.Powder)
                    {
                        return BoolVoidUnion.FromBool(false);
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    if (move.Flags.Powder == true && target != source &&
                        battle.Dex.GetImmunity(ConditionId.Powder, target.Types))
                    {
                        battle.Add("-activate", target, "item: Safety Goggles", move.Name);
                        return null;
                    }

                    return BoolIntEmptyVoidUnion.FromVoid();
                }),
                Num = 650,
                Gen = 6,
            },
            // SailFossil skipped - isNonstandard: "Past"
            [ItemId.SalacBerry] = new()
            {
                Id = ItemId.SalacBerry,
                Name = "Salac Berry",
                SpriteNum = 426,
                IsBerry = true,
                NaturalGift = (100, "Fighting"),
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
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((battle, _) =>
                {
                    battle.Boost(new SparseBoostsTable { Spe = 1 });
                })),
                Num = 203,
                Gen = 3,
            },
            // Skip salamencite, sceptilite, scizorite, scolipite, scraftinite - mega stones
            [ItemId.ScopeLens] = new()
            {
                Id = ItemId.ScopeLens,
                Name = "Scope Lens",
                SpriteNum = 429,
                Fling = new FlingData { BasePower = 30 },
                OnModifyCritRatio =
                    new OnModifyCritRatioEventInfo((_, critRatio, _, _, _) =>
                        DoubleVoidUnion.FromDouble(critRatio + 1)),
                Num = 232,
                Gen = 2,
            },
            // SeaIncense skipped - isNonstandard: "Past"
            [ItemId.SharpBeak] = new()
            {
                Id = ItemId.SharpBeak,
                Name = "Sharp Beak",
                SpriteNum = 436,
                Fling = new FlingData { BasePower = 50 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Flying)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 244,
                Gen = 2,
            },
            // Skip sharpedonite - mega stone
            [ItemId.ShedShell] = new()
            {
                Id = ItemId.ShedShell,
                Name = "Shed Shell",
                SpriteNum = 437,
                Fling = new FlingData { BasePower = 10 },
                OnTrapPokemon =
                    new OnTrapPokemonEventInfo(
                        (_, pokemon) => { pokemon.Trapped = PokemonTrapped.False; }, -10),
                OnMaybeTrapPokemon =
                    new OnMaybeTrapPokemonEventInfo(
                        (_, pokemon) => { pokemon.MaybeTrapped = false; }, -10),
                Num = 295,
                Gen = 4,
            },
            [ItemId.ShellBell] = new()
            {
                Id = ItemId.ShellBell,
                Name = "Shell Bell",
                SpriteNum = 438,
                Fling = new FlingData { BasePower = 30 },
                OnAfterMoveSecondarySelf = new OnAfterMoveSecondarySelfEventInfo(
                    (battle, pokemon, _, move) =>
                    {
                        if (move.TotalDamage != null && move.TotalDamage.ToInt() > 0 &&
                            !pokemon.ForceSwitchFlag)
                        {
                            battle.Heal(move.TotalDamage.ToInt() / 8, pokemon);
                        }
                    }, -1),
                Num = 253,
                Gen = 3,
            },
            [ItemId.ShinyStone] = new()
            {
                Id = ItemId.ShinyStone,
                Name = "Shiny Stone",
                SpriteNum = 439,
                Fling = new FlingData { BasePower = 80 },
                Num = 107,
                Gen = 4,
            },
            // Skip shockdrive - not Gen 9
            [ItemId.ShucaBerry] = new()
            {
                Id = ItemId.ShucaBerry,
                Name = "Shuca Berry",
                SpriteNum = 443,
                IsBerry = true,
                NaturalGift = (80, "Ground"),
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, _, target, move) =>
                    {
                        if (move.Type == MoveType.Ground && target.GetMoveHitData(move).TypeMod > 0)
                        {
                            bool hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                          move.Flags.BypassSub != true &&
                                          !(move.Infiltrates == true && battle.Gen >= 6);
                            if (hitSub) return damage;

                            if (target.EatItem())
                            {
                                battle.Debug("-50% reduction");
                                battle.Add("-enditem", target, "item: Shuca Berry", "[weaken]");
                                battle.ChainModify(0.5);
                                return battle.FinalModify(damage);
                            }
                        }

                        return damage;
                    }),
                // OnEat: empty function
                Num = 191,
                Gen = 4,
            },
            [ItemId.SilkScarf] = new()
            {
                Id = ItemId.SilkScarf,
                Name = "Silk Scarf",
                SpriteNum = 444,
                Fling = new FlingData { BasePower = 10 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Normal)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 251,
                Gen = 3,
            },
            [ItemId.SilverPowder] = new()
            {
                Id = ItemId.SilverPowder,
                Name = "Silver Powder",
                SpriteNum = 447,
                Fling = new FlingData { BasePower = 10 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Bug)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 222,
                Gen = 2,
            },
            [ItemId.SitrusBerry] = new()
            {
                Id = ItemId.SitrusBerry,
                Name = "Sitrus Berry",
                SpriteNum = 448,
                IsBerry = true,
                NaturalGift = (80, "Psychic"),
                OnUpdate = new OnUpdateEventInfo((_, pokemon) =>
                {
                    if (pokemon.Hp <= pokemon.MaxHp / 2)
                    {
                        pokemon.EatItem();
                    }
                }),
                OnTryEatItem = new OnTryEatItemEventInfo(
                    (Func<Battle, Item, Pokemon, BoolVoidUnion>)((battle, _, pokemon) =>
                    {
                        RelayVar? result = battle.RunEvent(EventId.TryHeal, pokemon, null,
                            battle.Effect,
                            pokemon.BaseMaxHp / 4);
                        if (result is BoolRelayVar { Value: false })
                        {
                            return BoolVoidUnion.FromBool(false);
                        }

                        return BoolVoidUnion.FromVoid();
                    })),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((battle, pokemon) =>
                {
                    battle.Heal(pokemon.BaseMaxHp / 4);
                })),
                Num = 158,
                Gen = 3,
            },
            // Skip skarmorite - mega stone
            // Skip skullfossil - not Gen 9
            [ItemId.SkyPlate] = new()
            {
                Id = ItemId.SkyPlate,
                Name = "Sky Plate",
                SpriteNum = 450,
                OnPlate = PokemonType.Flying,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Flying)
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
                ForcedForme = "Arceus-Flying",
                Num = 306,
                Gen = 4,
            },
            // Skip slowbronite - mega stone
            [ItemId.SmoothRock] = new()
            {
                Id = ItemId.SmoothRock,
                Name = "Smooth Rock",
                SpriteNum = 453,
                Fling = new FlingData { BasePower = 10 },
                Num = 283,
                Gen = 4,
            },
            // Skip snorliumz - z-move
            [ItemId.Snowball] = new()
            {
                Id = ItemId.Snowball,
                Name = "Snowball",
                SpriteNum = 606,
                Fling = new FlingData { BasePower = 30 },
                OnDamagingHit = new OnDamagingHitEventInfo((_, _, target, _, move) =>
                {
                    if (move.Type == MoveType.Ice)
                    {
                        target.UseItem();
                    }
                }),
                Boosts = new SparseBoostsTable { Atk = 1 },
                Num = 649,
                Gen = 6,
            },
            [ItemId.SoftSand] = new()
            {
                Id = ItemId.SoftSand,
                Name = "Soft Sand",
                SpriteNum = 456,
                Fling = new FlingData { BasePower = 10 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Ground)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 237,
                Gen = 2,
            },
            // Skip solganiumz - z-move
            [ItemId.SoulDew] = new()
            {
                Id = ItemId.SoulDew,
                Name = "Soul Dew",
                SpriteNum = 459,
                Fling = new FlingData { BasePower = 30 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, _, move) =>
                {
                    // Latios (#380) or Latias (#381)
                    if (user.BaseSpecies.Num is 380 or 381 &&
                        move.Type is MoveType.Psychic or MoveType.Dragon)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                // itemUser: ["Latios", "Latias"],
                Num = 225,
                Gen = 3,
            },
            [ItemId.SpellTag] = new()
            {
                Id = ItemId.SpellTag,
                Name = "Spell Tag",
                SpriteNum = 461,
                Fling = new FlingData { BasePower = 30 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Ghost)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 247,
                Gen = 2,
            },
            // SpelonBerry skipped - isNonstandard: "Past"
            [ItemId.SplashPlate] = new()
            {
                Id = ItemId.SplashPlate,
                Name = "Splash Plate",
                SpriteNum = 463,
                OnPlate = PokemonType.Water,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Water)
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
                ForcedForme = "Arceus-Water",
                Num = 299,
                Gen = 4,
            },
            [ItemId.SpookyPlate] = new()
            {
                Id = ItemId.SpookyPlate,
                Name = "Spooky Plate",
                SpriteNum = 464,
                OnPlate = PokemonType.Ghost,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Ghost)
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
                ForcedForme = "Arceus-Ghost",
                Num = 310,
                Gen = 4,
            },
            [ItemId.SportBall] = new()
            {
                Id = ItemId.SportBall,
                Name = "Sport Ball",
                SpriteNum = 465,
                Num = 499,
                Gen = 2,
                IsPokeball = true,
            },
            [ItemId.StarfBerry] = new()
            {
                Id = ItemId.StarfBerry,
                Name = "Starf Berry",
                SpriteNum = 472,
                IsBerry = true,
                NaturalGift = (100, "Psychic"),
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
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((battle, pokemon) =>
                {
                    var stats = new List<StatIdExceptHp>();
                    foreach (StatIdExceptHp stat in new[]
                             {
                                 StatIdExceptHp.Atk, StatIdExceptHp.Def, StatIdExceptHp.SpA,
                                 StatIdExceptHp.SpD, StatIdExceptHp.Spe,
                             })
                    {
                        int currentBoost = pokemon.Boosts.GetBoost((BoostId)stat);
                        if (currentBoost < 6)
                        {
                            stats.Add(stat);
                        }
                    }

                    if (stats.Count > 0)
                    {
                        StatIdExceptHp randomStat = battle.Sample(stats);
                        var boost = new SparseBoostsTable();
                        boost.SetBoost((BoostId)randomStat, 2);
                        battle.Boost(boost);
                    }
                })),
                Num = 207,
                Gen = 3,
            },
            // Skip starminite - mega stone
            [ItemId.StarSweet] = new()
            {
                Id = ItemId.StarSweet,
                Name = "Star Sweet",
                SpriteNum = 709,
                Fling = new FlingData { BasePower = 10 },
                Num = 1114,
                Gen = 8,
            },
            // Skip steelixite, steelgem, steelmemory, steeliumz, stick - isNonstandard: "Past"
            [ItemId.StickyBarb] = new()
            {
                Id = ItemId.StickyBarb,
                Name = "Sticky Barb",
                SpriteNum = 476,
                Fling = new FlingData { BasePower = 80 },
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    battle.Damage(pokemon.BaseMaxHp / 8, pokemon);
                })
                {
                    Order = 28,
                    SubOrder = 3,
                },
                OnHit = new OnHitEventInfo((battle, target, source, move) =>
                {
                    if (source != null && source != target && source.Item == ItemId.None &&
                        battle.CheckMoveMakesContact(move, source, target))
                    {
                        ItemFalseUnion barb = target.TakeItem();
                        if (barb is not ItemItemFalseUnion itemUnion)
                            return null; // Gen 4 Multitype
                        source.SetItem(itemUnion.Item.Id);
                        // no message for Sticky Barb changing hands
                    }

                    return null;
                }),
                Num = 288,
                Gen = 4,
            },
            [ItemId.StonePlate] = new()
            {
                Id = ItemId.StonePlate,
                Name = "Stone Plate",
                SpriteNum = 477,
                OnPlate = PokemonType.Rock,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Rock)
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
                ForcedForme = "Arceus-Rock",
                Num = 309,
                Gen = 4,
            },
            [ItemId.StrawberrySweet] = new()
            {
                Id = ItemId.StrawberrySweet,
                Name = "Strawberry Sweet",
                SpriteNum = 704,
                Fling = new FlingData { BasePower = 10 },
                Num = 1109,
                Gen = 8,
            },
            [ItemId.SunStone] = new()
            {
                Id = ItemId.SunStone,
                Name = "Sun Stone",
                SpriteNum = 480,
                Fling = new FlingData { BasePower = 30 },
                Num = 80,
                Gen = 2,
            },
            // Skip swampertite - mega stone
            [ItemId.SweetApple] = new()
            {
                Id = ItemId.SweetApple,
                Name = "Sweet Apple",
                SpriteNum = 711,
                Fling = new FlingData { BasePower = 30 },
                Num = 1116,
                Gen = 8,
            },
            [ItemId.SyrupyApple] = new()
            {
                Id = ItemId.SyrupyApple,
                Name = "Syrupy Apple",
                SpriteNum = 755,
                Fling = new FlingData { BasePower = 30 },
                Num = 2402,
                Gen = 9,
            },

            // T items
            [ItemId.TamatoBerry] = new()
            {
                Id = ItemId.TamatoBerry,
                Name = "Tamato Berry",
                SpriteNum = 486,
                IsBerry = true,
                NaturalGift = (90, "Psychic"),
                // onEat: false - used for EV reduction, not relevant in Gen 9 battles
                Num = 174,
                Gen = 3,
            },
            [ItemId.TangaBerry] = new()
            {
                Id = ItemId.TangaBerry,
                Name = "Tanga Berry",
                SpriteNum = 487,
                IsBerry = true,
                NaturalGift = (80, "Bug"),
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, _, target, move) =>
                    {
                        if (move.Type == MoveType.Bug && target.GetMoveHitData(move).TypeMod > 0)
                        {
                            bool hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                          move.Flags.BypassSub != true &&
                                          !(move.Infiltrates == true && battle.Gen >= 6);
                            if (hitSub) return damage;

                            if (target.EatItem())
                            {
                                battle.Debug("-50% reduction");
                                battle.Add("-enditem", target, "item: Tanga Berry", "[weaken]");
                                battle.ChainModify(0.5);
                                return battle.FinalModify(damage);
                            }
                        }

                        return damage;
                    }),
                // OnEat: empty function
                Num = 194,
                Gen = 4,
            },
            // Skip tapuniumz - z-move
            [ItemId.TartApple] = new()
            {
                Id = ItemId.TartApple,
                Name = "Tart Apple",
                SpriteNum = 712,
                Fling = new FlingData { BasePower = 30 },
                Num = 1117,
                Gen = 8,
            },
            [ItemId.TerrainExtender] = new()
            {
                Id = ItemId.TerrainExtender,
                Name = "Terrain Extender",
                SpriteNum = 662,
                Fling = new FlingData { BasePower = 60 },
                Num = 879,
                Gen = 7,
            },
            // ThickClub skipped - isNonstandard: "Past"
            [ItemId.ThroatSpray] = new()
            {
                Id = ItemId.ThroatSpray,
                Name = "Throat Spray",
                SpriteNum = 713,
                Fling = new FlingData { BasePower = 30 },
                OnAfterMoveSecondarySelf =
                    new OnAfterMoveSecondarySelfEventInfo((_, pokemon, _, move) =>
                    {
                        if (move.Flags.Sound == true)
                        {
                            pokemon.UseItem();
                        }
                    }),
                Boosts = new SparseBoostsTable { SpA = 1 },
                Num = 1118,
                Gen = 8,
            },
            [ItemId.ThunderStone] = new()
            {
                Id = ItemId.ThunderStone,
                Name = "Thunder Stone",
                SpriteNum = 492,
                Fling = new FlingData { BasePower = 30 },
                Num = 83,
                Gen = 1,
            },
            [ItemId.TimerBall] = new()
            {
                Id = ItemId.TimerBall,
                Name = "Timer Ball",
                SpriteNum = 494,
                Num = 10,
                Gen = 3,
                IsPokeball = true,
            },
            [ItemId.ToxicOrb] = new()
            {
                Id = ItemId.ToxicOrb,
                Name = "Toxic Orb",
                SpriteNum = 515,
                Fling = new FlingData
                {
                    BasePower = 30,
                    Status = ConditionId.Toxic,
                },
                OnResidual = new OnResidualEventInfo((_, pokemon, _, _) =>
                {
                    pokemon.TrySetStatus(ConditionId.Toxic, pokemon);
                })
                {
                    Order = 28,
                    SubOrder = 3,
                },
                Num = 272,
                Gen = 4,
            },
            [ItemId.ToxicPlate] = new()
            {
                Id = ItemId.ToxicPlate,
                Name = "Toxic Plate",
                SpriteNum = 516,
                OnPlate = PokemonType.Poison,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Poison)
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
                ForcedForme = "Arceus-Poison",
                Num = 304,
                Gen = 4,
            },
            // Skip TR items - not relevant for Gen 9 battles
            [ItemId.TwistedSpoon] = new()
            {
                Id = ItemId.TwistedSpoon,
                Name = "Twisted Spoon",
                SpriteNum = 520,
                Fling = new FlingData { BasePower = 30 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Psychic)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 248,
                Gen = 2,
            },

            // U items
            [ItemId.UltraBall] = new()
            {
                Id = ItemId.UltraBall,
                Name = "Ultra Ball",
                SpriteNum = 521,
                Num = 2,
                Gen = 1,
                IsPokeball = true,
            },
            // Skip ultranecroziumz - z-move
            [ItemId.UnremarkableTeacup] = new()
            {
                Id = ItemId.UnremarkableTeacup,
                Name = "Unremarkable Teacup",
                SpriteNum = 756,
                Fling = new FlingData { BasePower = 80 },
                Num = 2403,
                Gen = 9,
            },
            [ItemId.UpGrade] = new()
            {
                Id = ItemId.UpGrade,
                Name = "Up-Grade",
                SpriteNum = 523,
                Fling = new FlingData { BasePower = 30 },
                Num = 252,
                Gen = 2,
            },
            [ItemId.UtilityUmbrella] = new()
            {
                Id = ItemId.UtilityUmbrella,
                Name = "Utility Umbrella",
                SpriteNum = 718,
                Fling = new FlingData { BasePower = 60 },
                // Weather immunity partially implemented in Pokemon.EffectiveWeather()
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    // TS: if (!pokemon.ignoringItem()) return; - return early if NOT ignoring item
                    if (!pokemon.IgnoringItem()) return;
                    ConditionId weather = battle.Field.EffectiveWeather();
                    if (weather is ConditionId.SunnyDay or ConditionId.RainDance
                        or ConditionId.DesolateLand or ConditionId.PrimordialSea)
                    {
                        battle.RunEvent(EventId.WeatherChange, pokemon, pokemon, battle.Effect);
                    }
                }),
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.ItemState.Inactive != true) return;
                    pokemon.ItemState.Inactive = false;
                    ConditionId weather = battle.Field.EffectiveWeather();
                    if (weather is ConditionId.SunnyDay or ConditionId.RainDance
                        or ConditionId.DesolateLand or ConditionId.PrimordialSea)
                    {
                        battle.RunEvent(EventId.WeatherChange, pokemon, pokemon, battle.Effect);
                    }
                }),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    ConditionId weather = battle.Field.EffectiveWeather();
                    if (weather is ConditionId.SunnyDay or ConditionId.RainDance
                        or ConditionId.DesolateLand or ConditionId.PrimordialSea)
                    {
                        battle.RunEvent(EventId.WeatherChange, pokemon, pokemon, battle.Effect);
                    }

                    pokemon.ItemState.Inactive = true;
                }),
                Num = 1123,
                Gen = 8,
            },
        };
    }
}