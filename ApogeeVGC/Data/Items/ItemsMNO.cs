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
    private partial Dictionary<ItemId, Item> CreateItemsMno()
    {
        return new Dictionary<ItemId, Item>
        {
            [ItemId.Magnet] = new()
            {
                Id = ItemId.Magnet,
                Name = "Magnet",
                SpriteNum = 273,
                Fling = new FlingData { BasePower = 30 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
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
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Hp <= pokemon.MaxHp / 4 || (pokemon.Hp <= pokemon.MaxHp / 2 &&
                                                            pokemon.HasAbility(AbilityId
                                                                .Gluttony) &&
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
                    // TODO: Get nature from Pokemon and check if minus stat is Spe
                    // if (pokemon.GetNature().Minus == StatIdExceptHp.Spe)
                    // {
                    //     pokemon.AddVolatile(ConditionId.Confusion);
                    // }
                })),
                Num = 161,
                Gen = 3,
            },
            [ItemId.MarangaBerry] = new()
            {
                Id = ItemId.MarangaBerry,
                Name = "Maranga Berry",
                SpriteNum = 597,
                IsBerry = true,
                NaturalGift = (100, "Dark"),
                OnAfterMoveSecondary =
                    new OnAfterMoveSecondaryEventInfo((battle, target, source, move) =>
                    {
                        if (move.Category == MoveCategory.Special)
                        {
                            target.EatItem();
                        }
                    }),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((battle, pokemon) =>
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
                OnPlate = "Grass",
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Type == MoveType.Grass)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                // OnTakeItem - Arceus can't have plates removed
                // TODO: Implement proper OnTakeItem logic for Arceus plates
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
                    // TODO: OnEffect for fling - removes mental status conditions
                },
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    var conditions = new[]
                    {
                        ConditionId.Attract, ConditionId.Taunt, ConditionId.Encore,
                        ConditionId.Torment, ConditionId.Disable, ConditionId.HealBlock
                    };
                    foreach (var firstCondition in conditions)
                    {
                        if (pokemon.Volatiles.ContainsKey(firstCondition))
                        {
                            if (!pokemon.UseItem()) return;
                            foreach (var secondCondition in conditions)
                            {
                                //TODO: Fix logic
                                //if (pokemon.Volatiles.TryGetValue(secondCondition, out var cond))
                                //{
                                //    pokemon.RemoveVolatile(cond);
                                //    if (firstCondition == ConditionId.Attract &&
                                //        secondCondition == ConditionId.Attract)
                                //    {
                                //        battle.Add("-end", pokemon, "move: Attract",
                                //            "[from] item: Mental Herb");
                                //    }
                                //}
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
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
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
            [ItemId.MetalPowder] = new()
            {
                Id = ItemId.MetalPowder,
                Name = "Metal Powder",
                SpriteNum = 287,
                Fling = new FlingData { BasePower = 10 },
                OnModifyDef = new OnModifyDefEventInfo((battle, def, pokemon, _, _) =>
                {
                    // Ditto has species number 132
                    if (pokemon.Species.Name == "Ditto" && !pokemon.Transformed)
                    {
                        battle.ChainModify(2);
                        return battle.FinalModify(def);
                    }

                    return def;
                }, 2),
                Num = 257,
                Gen = 2,
            },
            [ItemId.Metronome] = new()
            {
                Id = ItemId.Metronome,
                Name = "Metronome",
                SpriteNum = 289,
                Fling = new FlingData { BasePower = 30 },
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    pokemon.AddVolatile(ConditionId.Metronome);
                }),
                // TODO: Implement Metronome condition that tracks consecutive move usage and boosts damage
                // The condition needs OnStart, OnTryMove priority -2, and OnModifyDamage handlers
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
                OnResidual = new OnResidualEventInfo((battle, pokemon, source, effect) =>
                {
                    if (pokemon.Hp <= pokemon.MaxHp / 4 || (pokemon.Hp <= pokemon.MaxHp / 2 &&
                                                            pokemon.HasAbility(AbilityId
                                                                .Gluttony) &&
                                                            pokemon.AbilityState.Gluttony == true))
                    {
                        pokemon.EatItem();
                    }
                }),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((battle, pokemon) =>
                {
                    pokemon.AddVolatile(ConditionId.MicleBerry);
                })),
                // TODO: Implement Micle Berry condition that boosts accuracy on next move
                Num = 209,
                Gen = 4,
            },
            [ItemId.MindPlate] = new()
            {
                Id = ItemId.MindPlate,
                Name = "Mind Plate",
                SpriteNum = 291,
                OnPlate = "Psychic",
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Type == MoveType.Psychic)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                // OnTakeItem - Arceus can't have plates removed
                // TODO: Implement proper OnTakeItem logic for Arceus plates
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
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
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
                // TODO: Implement Mirror Herb logic
                // OnFoeAfterBoost - track positive boosts from opponent
                // OnAnySwitchIn, OnAnyAfterMega, OnAnyAfterTerastallization, OnAnyAfterMove, OnResidual - check if ready to use
                // OnUse - copy boosts
                // OnEnd - cleanup
                Num = 1883,
                Gen = 9,
            },
            [ItemId.MistySeed] = new()
            {
                Id = ItemId.MistySeed,
                Name = "Misty Seed",
                SpriteNum = 666,
                Fling = new FlingData { BasePower = 10 },
                OnSwitchIn = new OnSwitchInEventInfo((battle, pokemon) =>
                {
                    if (!pokemon.IgnoringItem() &&
                        battle.Field.IsTerrain(ConditionId.MistyTerrain, null))
                    {
                        pokemon.UseItem();
                    }
                }, -1),
                OnTerrainChange = new OnTerrainChangeEventInfo((battle, pokemon, source, effect) =>
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
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
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
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
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
            [ItemId.None] = new()
            {
                Id = ItemId.None,
                Name = "None",
                SpriteNum = 0,
                Num = 0,
                Gen = 0,
            },
        };
    }
}