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

namespace ApogeeVGC.Data.Items;

public partial record Items
{
    private partial Dictionary<ItemId, Item> CreateItemsMno()
    {
        return new Dictionary<ItemId, Item>
        {
            // M items
            // MachoBrace removed - isNonstandard: "Past"
            [ItemId.Magmarizer] = new()
            {
                Id = ItemId.Magmarizer,
                Name = "Magmarizer",
                SpriteNum = 272,
                Fling = new FlingData { BasePower = 80 },
                Num = 323,
                Gen = 4,
            },
            [ItemId.Magnet] = new()
            {
                Id = ItemId.Magnet,
                Name = "Magnet",
                SpriteNum = 273,
                Fling = new FlingData { BasePower = 30 },
                OnBasePower = OnBasePowerEventInfo.Create((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Electric)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 242,
                Gen = 2,
            },
            [ItemId.MagoBerry] = new()
            {
                Id = ItemId.MagoBerry,
                Name = "Mago Berry",
                SpriteNum = 274,
                IsBerry = true,
                NaturalGift = (80, "Ghost"),
                OnUpdate = OnUpdateEventInfo.Create((_, pokemon) =>
                {
                    if (pokemon.Hp <= pokemon.MaxHp / 4 || (pokemon.Hp <= pokemon.MaxHp / 2 &&
                                                            pokemon.HasAbility(AbilityId
                                                                .Gluttony) &&
                                                            pokemon.AbilityState.Gluttony == true))
                    {
                        pokemon.EatItem();
                    }
                }),
                OnTryEatItem = OnTryEatItemEventInfo.Create((battle, _, pokemon) =>
                    {
                        RelayVar? canHeal = battle.RunEvent(EventId.TryHeal, pokemon, null,
                            battle.Effect,
                            pokemon.BaseMaxHp / 3);
                        if (canHeal is BoolRelayVar { Value: false })
                        {
                            return BoolVoidUnion.FromBool(false);
                        }

                        return BoolVoidUnion.FromVoid();
                    }),
                OnEat = OnEatEventInfo.Create((Action<Battle, Pokemon>)((battle, pokemon) =>
                {
                    battle.Heal(pokemon.BaseMaxHp / 3);
                    if (pokemon.Set.Nature.Minus == StatIdExceptHp.Spe)
                    {
                        pokemon.AddVolatile(ConditionId.Confusion);
                    }
                })),
                Num = 161,
                Gen = 3,
            },
            [ItemId.MaliciousArmor] = new()
            {
                Id = ItemId.MaliciousArmor,
                Name = "Malicious Armor",
                SpriteNum = 744,
                Fling = new FlingData { BasePower = 30 },
                Num = 1861,
                Gen = 9,
            },
            [ItemId.MarangaBerry] = new()
            {
                Id = ItemId.MarangaBerry,
                Name = "Maranga Berry",
                SpriteNum = 597,
                IsBerry = true,
                NaturalGift = (100, "Dark"),
                OnAfterMoveSecondary =
                    OnAfterMoveSecondaryEventInfo.Create((_, target, _, move) =>
                    {
                        if (move.Category == MoveCategory.Special)
                        {
                            target.EatItem();
                        }
                    }),
                OnEat = OnEatEventInfo.Create((Action<Battle, Pokemon>)((battle, _) =>
                {
                    battle.Boost(new SparseBoostsTable { SpD = 1 });
                })),
                Num = 688,
                Gen = 6,
            },
            [ItemId.MasterBall] = new()
            {
                Id = ItemId.MasterBall,
                Name = "Master Ball",
                SpriteNum = 276,
                Num = 1,
                Gen = 1,
                IsPokeball = true,
            },
            [ItemId.MasterpieceTeacup] = new()
            {
                Id = ItemId.MasterpieceTeacup,
                Name = "Masterpiece Teacup",
                SpriteNum = 757,
                Fling = new FlingData { BasePower = 80 },
                Num = 2404,
                Gen = 9,
            },
            [ItemId.MeadowPlate] = new()
            {
                Id = ItemId.MeadowPlate,
                Name = "Meadow Plate",
                SpriteNum = 282,
                OnPlate = PokemonType.Grass,
                OnBasePower = OnBasePowerEventInfo.Create((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Grass)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                OnTakeItem = OnTakeItemEventInfo.Create(
                    (
                        (_, _, pokemon, source, _) =>
                        {
                            if (source?.BaseSpecies.Num == 493 || pokemon.BaseSpecies.Num == 493)
                            {
                                return new BoolRelayVar(false);
                            }

                            return new BoolRelayVar(true);
                        })),
                ForcedForme = "Arceus-Grass",
                Num = 301,
                Gen = 4,
            },
            [ItemId.MentalHerb] = new()
            {
                Id = ItemId.MentalHerb,
                Name = "Mental Herb",
                SpriteNum = 285,
                Fling = new FlingData
                {
                    BasePower = 10,
                    Effect = (battle, target, _, _) =>
                    {
                        var conditions = new[]
                        {
                            ConditionId.Attract, ConditionId.Taunt, ConditionId.Encore,
                            ConditionId.Torment, ConditionId.Disable, ConditionId.PsychicNoise
                        };
                        foreach (ConditionId firstCondition in conditions)
                        {
                            if (target.Volatiles.ContainsKey(firstCondition))
                            {
                                foreach (ConditionId secondCondition in conditions)
                                {
                                    target.RemoveVolatile(_library.Conditions[secondCondition]);
                                    if (firstCondition == ConditionId.Attract &&
                                        secondCondition == ConditionId.Attract)
                                    {
                                        battle.Add("-end", target, "move: Attract",
                                            "[from] item: Mental Herb");
                                    }
                                }

                                return null;
                            }
                        }

                        return null;
                    }
                },
                OnUpdate = OnUpdateEventInfo.Create((battle, pokemon) =>
                {
                    var conditions = new[]
                    {
                        ConditionId.Attract, ConditionId.Taunt, ConditionId.Encore,
                        ConditionId.Torment, ConditionId.Disable, ConditionId.PsychicNoise
                    };
                    foreach (ConditionId firstCondition in conditions)
                    {
                        if (pokemon.Volatiles.ContainsKey(firstCondition))
                        {
                            if (!pokemon.UseItem()) return;
                            foreach (ConditionId secondCondition in conditions)
                            {
                                pokemon.RemoveVolatile(_library.Conditions[secondCondition]);
                                if (firstCondition == ConditionId.Attract &&
                                    secondCondition == ConditionId.Attract)
                                {
                                    battle.Add("-end", pokemon, "move: Attract",
                                        "[from] item: Mental Herb");
                                }
                            }

                            return;
                        }
                    }
                }),
                Num = 219,
                Gen = 3,
            },
            [ItemId.MetalAlloy] = new()
            {
                Id = ItemId.MetalAlloy,
                Name = "Metal Alloy",
                SpriteNum = 761,
                Num = 2482,
                Gen = 9,
            },
            [ItemId.MetalCoat] = new()
            {
                Id = ItemId.MetalCoat,
                Name = "Metal Coat",
                SpriteNum = 286,
                Fling = new FlingData { BasePower = 30 },
                OnBasePower = OnBasePowerEventInfo.Create((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Steel)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 233,
                Gen = 2,
            },
            // MetalPowder removed - isNonstandard: "Past"
            [ItemId.Metronome] = new()
            {
                Id = ItemId.Metronome,
                Name = "Metronome",
                SpriteNum = 289,
                Fling = new FlingData { BasePower = 30 },
                OnStart = OnStartEventInfo.Create((_, pokemon) => { pokemon.AddVolatile(ConditionId.Metronome); }),
                Condition = _library.Conditions[ConditionId.Metronome],
                Num = 277,
                Gen = 4,
            },
            [ItemId.MicleBerry] = new()
            {
                Id = ItemId.MicleBerry,
                Name = "Micle Berry",
                SpriteNum = 290,
                IsBerry = true,
                NaturalGift = (100, "Rock"),
                OnResidual = OnResidualEventInfo.Create((_, pokemon, _, _) =>
                {
                    if (pokemon.Hp <= pokemon.MaxHp / 4 || (pokemon.Hp <= pokemon.MaxHp / 2 &&
                                                            pokemon.HasAbility(AbilityId
                                                                .Gluttony) &&
                                                            pokemon.AbilityState.Gluttony == true))
                    {
                        pokemon.EatItem();
                    }
                }),
                OnEat = OnEatEventInfo.Create((Action<Battle, Pokemon>)((_, pokemon) =>
                {
                    pokemon.AddVolatile(ConditionId.MicleBerry);
                })),
                Condition = _library.Conditions[ConditionId.MicleBerry],
                Num = 209,
                Gen = 4,
            },
            [ItemId.MindPlate] = new()
            {
                Id = ItemId.MindPlate,
                Name = "Mind Plate",
                SpriteNum = 291,
                OnPlate = PokemonType.Psychic,
                OnBasePower = OnBasePowerEventInfo.Create((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Psychic)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                OnTakeItem = OnTakeItemEventInfo.Create(
                    (
                        (_, _, pokemon, source, _) =>
                        {
                            if (source?.BaseSpecies.Num == 493 || pokemon.BaseSpecies.Num == 493)
                            {
                                return new BoolRelayVar(false);
                            }

                            return new BoolRelayVar(true);
                        })),
                ForcedForme = "Arceus-Psychic",
                Num = 307,
                Gen = 4,
            },
            [ItemId.MiracleSeed] = new()
            {
                Id = ItemId.MiracleSeed,
                Name = "Miracle Seed",
                SpriteNum = 292,
                Fling = new FlingData { BasePower = 30 },
                OnBasePower = OnBasePowerEventInfo.Create((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Grass)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 239,
                Gen = 2,
            },
            [ItemId.MirrorHerb] = new()
            {
                Id = ItemId.MirrorHerb,
                Name = "Mirror Herb",
                SpriteNum = 748,
                Fling = new FlingData { BasePower = 30 },
                OnFoeAfterBoost = OnFoeAfterBoostEventInfo.Create((battle, boost, _, _, effect) =>
                {
                    // Don't trigger from Opportunist or Mirror Herb
                    if (effect?.EffectStateId == AbilityId.Opportunist ||
                        effect?.EffectStateId == ItemId.MirrorHerb) return;

                    battle.EffectState.Boosts ??= new SparseBoostsTable();
                    SparseBoostsTable? boostPlus = battle.EffectState.Boosts;

                    foreach (BoostId stat in Enum.GetValues<BoostId>())
                    {
                        int? boostValue = boost.GetBoost(stat);
                        if (boostValue is > 0)
                        {
                            int existing = boostPlus.GetBoost(stat) ?? 0;
                            boostPlus.SetBoost(stat, existing + boostValue.Value);
                            battle.EffectState.Ready = true;
                        }
                    }
                }),
                OnAnySwitchIn = OnAnySwitchInEventInfo.Create((battle, _) =>
                {
                    if (battle.EffectState.Ready != true) return;
                    if (battle.EffectState.Target is not PokemonEffectStateTarget target) return;
                    target.Pokemon.UseItem();
                }, -3),
                OnAnyAfterMega = OnAnyAfterMegaEventInfo.Create((battle, _) =>
                {
                    if (battle.EffectState.Ready != true) return;
                    if (battle.EffectState.Target is not PokemonEffectStateTarget target) return;
                    target.Pokemon.UseItem();
                }),
                OnAnyAfterTerastallization = OnAnyAfterTerastallizationEventInfo.Create((battle, _) =>
                {
                    if (battle.EffectState.Ready != true) return;
                    if (battle.EffectState.Target is not PokemonEffectStateTarget target) return;
                    target.Pokemon.UseItem();
                }),
                OnAnyAfterMove = OnAnyAfterMoveEventInfo.Create((battle, _, _, _) =>
                {
                    if (battle.EffectState.Ready != true) return null;
                    if (battle.EffectState.Target is not PokemonEffectStateTarget target)
                        return null;
                    target.Pokemon.UseItem();
                    return null;
                }),
                OnResidual = OnResidualEventInfo.Create((battle, _, _, _) =>
                {
                    if (battle.EffectState.Ready != true) return;
                    if (battle.EffectState.Target is not PokemonEffectStateTarget target) return;
                    target.Pokemon.UseItem();
                }, order: 29),
                OnUse = OnUseEventInfo.Create((battle, pokemon) =>
                {
                    if (battle.EffectState.Boosts != null)
                    {
                        battle.Boost(battle.EffectState.Boosts, pokemon);
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                OnEnd = OnEndEventInfo.Create((battle, _) =>
                {
                    battle.EffectState.Boosts = null;
                    battle.EffectState.Ready = null;
                }),
                Num = 1883,
                Gen = 9,
            },
            [ItemId.MistySeed] = new()
            {
                Id = ItemId.MistySeed,
                Name = "Misty Seed",
                SpriteNum = 666,
                Fling = new FlingData { BasePower = 10 },
                OnSwitchIn = OnSwitchInEventInfo.Create((battle, pokemon) =>
                {
                    if (!pokemon.IgnoringItem() &&
                        battle.Field.IsTerrain(ConditionId.MistyTerrain, null))
                    {
                        pokemon.UseItem();
                    }
                }, -1),
                OnTerrainChange = OnTerrainChangeEventInfo.Create((battle, pokemon, _, _) =>
                {
                    if (battle.Field.IsTerrain(ConditionId.MistyTerrain, null))
                    {
                        pokemon.UseItem();
                    }
                }),
                Boosts = new SparseBoostsTable { SpD = 1 },
                Num = 883,
                Gen = 7,
            },
            [ItemId.MoonBall] = new()
            {
                Id = ItemId.MoonBall,
                Name = "Moon Ball",
                SpriteNum = 294,
                Num = 498,
                Gen = 2,
                IsPokeball = true,
            },
            [ItemId.MoonStone] = new()
            {
                Id = ItemId.MoonStone,
                Name = "Moon Stone",
                SpriteNum = 295,
                Fling = new FlingData { BasePower = 30 },
                Num = 81,
                Gen = 1,
            },
            [ItemId.MuscleBand] = new()
            {
                Id = ItemId.MuscleBand,
                Name = "Muscle Band",
                SpriteNum = 297,
                Fling = new FlingData { BasePower = 10 },
                OnBasePower = OnBasePowerEventInfo.Create((battle, basePower, _, _, move) =>
                {
                    if (move.Category == MoveCategory.Physical)
                    {
                        battle.ChainModify([4505, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 16),
                Num = 266,
                Gen = 4,
            },
            [ItemId.MysticWater] = new()
            {
                Id = ItemId.MysticWater,
                Name = "Mystic Water",
                SpriteNum = 300,
                Fling = new FlingData { BasePower = 30 },
                OnBasePower = OnBasePowerEventInfo.Create((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Water)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 243,
                Gen = 2,
            },

            // N items
            // NanabBerry removed - isNonstandard: "Past"
            [ItemId.NestBall] = new()
            {
                Id = ItemId.NestBall,
                Name = "Nest Ball",
                SpriteNum = 303,
                Num = 8,
                Gen = 3,
                IsPokeball = true,
            },
            [ItemId.NetBall] = new()
            {
                Id = ItemId.NetBall,
                Name = "Net Ball",
                SpriteNum = 304,
                Num = 6,
                Gen = 3,
                IsPokeball = true,
            },
            [ItemId.NeverMeltIce] = new()
            {
                Id = ItemId.NeverMeltIce,
                Name = "Never-Melt Ice",
                SpriteNum = 305,
                Fling = new FlingData { BasePower = 30 },
                OnBasePower = OnBasePowerEventInfo.Create((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Ice)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 246,
                Gen = 2,
            },
            // NomelBerry removed - isNonstandard: "Past"
            [ItemId.None] = new()
            {
                Id = ItemId.None,
                Name = "None",
                SpriteNum = 0,
                Num = 0,
                Gen = 0,
            },
            [ItemId.NormalGem] = new()
            {
                Id = ItemId.NormalGem,
                Name = "Normal Gem",
                SpriteNum = 307,
                IsGem = true,
                OnSourceTryPrimaryHit =
                    OnSourceTryPrimaryHitEventInfo.Create((_, target, source, move) =>
                    {
                        if (target == source || move.Category == MoveCategory.Status ||
                            move.Flags.PledgeCombo == true)
                            return IntBoolVoidUnion.FromVoid();
                        if (move.Type == MoveType.Normal && source.UseItem())
                        {
                            source.AddVolatile(ConditionId.Gem);
                        }

                        return IntBoolVoidUnion.FromVoid();
                    }),
                Num = 564,
                Gen = 5,
            },

            // O items
            [ItemId.OccaBerry] = new()
            {
                Id = ItemId.OccaBerry,
                Name = "Occa Berry",
                SpriteNum = 311,
                IsBerry = true,
                NaturalGift = (80, "Fire"),
                OnSourceModifyDamage =
                    OnSourceModifyDamageEventInfo.Create((battle, damage, _, target, move) =>
                    {
                        if (move.Type == MoveType.Fire && target.GetMoveHitData(move).TypeMod > 0)
                        {
                            bool hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                          move.Flags.BypassSub != true &&
                                          !(move.Infiltrates == true && battle.Gen >= 6);
                            if (hitSub) return damage;

                            if (target.EatItem())
                            {
                                battle.Debug("-50% reduction");
                                battle.Add("-enditem", target, "item: Occa Berry", "[weaken]");
                                battle.ChainModify(0.5);
                                return battle.FinalModify(damage);
                            }
                        }

                        return damage;
                    }),
                OnEat = OnEatEventInfo.Create((Action<Battle, Pokemon>)((_, _) => { })),
                Num = 184,
                Gen = 4,
            },
            // OddIncense removed - isNonstandard: "Past"
            [ItemId.OranBerry] = new()
            {
                Id = ItemId.OranBerry,
                Name = "Oran Berry",
                SpriteNum = 319,
                IsBerry = true,
                NaturalGift = (80, "Poison"),
                OnUpdate = OnUpdateEventInfo.Create((_, pokemon) =>
                {
                    if (pokemon.Hp <= pokemon.MaxHp / 2)
                    {
                        pokemon.EatItem();
                    }
                }),
                OnTryEatItem = OnTryEatItemEventInfo.Create((battle, _, pokemon) =>
                    {
                        RelayVar? canHeal = battle.RunEvent(EventId.TryHeal, pokemon, null,
                            battle.Effect,
                            10);
                        if (canHeal is BoolRelayVar { Value: false })
                        {
                            return BoolVoidUnion.FromBool(false);
                        }

                        return BoolVoidUnion.FromVoid();
                    }),
                OnEat = OnEatEventInfo.Create((Action<Battle, Pokemon>)((battle, _) => { battle.Heal(10); })),
                Num = 155,
                Gen = 3,
            },
            [ItemId.OvalStone] = new()
            {
                Id = ItemId.OvalStone,
                Name = "Oval Stone",
                SpriteNum = 321,
                Fling = new FlingData { BasePower = 80 },
                Num = 110,
                Gen = 4,
            },
        };
    }
}