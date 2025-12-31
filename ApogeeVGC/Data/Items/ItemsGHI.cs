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
using PokemonType = ApogeeVGC.Sim.PokemonClasses.PokemonType;

namespace ApogeeVGC.Data.Items;

public partial record Items
{
    private partial Dictionary<ItemId, Item> CreateItemsGhi()
    {
        return new Dictionary<ItemId, Item>
        {
            // G items
            [ItemId.GalaricaCuff] = new()
            {
                Id = ItemId.GalaricaCuff,
                Name = "Galarica Cuff",
                SpriteNum = 739,
                Fling = new FlingData { BasePower = 30 },
                Num = 1582,
                Gen = 8,
            },
            [ItemId.GalaricaWreath] = new()
            {
                Id = ItemId.GalaricaWreath,
                Name = "Galarica Wreath",
                SpriteNum = 740,
                Fling = new FlingData { BasePower = 30 },
                Num = 1592,
                Gen = 8,
            },
            [ItemId.GanlonBerry] = new()
            {
                Id = ItemId.GanlonBerry,
                Name = "Ganlon Berry",
                SpriteNum = 158,
                IsBerry = true,
                NaturalGift = (100, "Ice"),
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
                    battle.Boost(new SparseBoostsTable { Def = 1 });
                })),
                Num = 202,
                Gen = 3,
            },
            [ItemId.GoldBottleCap] = new()
            {
                Id = ItemId.GoldBottleCap,
                Name = "Gold Bottle Cap",
                SpriteNum = 697,
                Fling = new FlingData { BasePower = 30 },
                Num = 796,
                Gen = 7,
            },
            [ItemId.GrassySeed] = new()
            {
                Id = ItemId.GrassySeed,
                Name = "Grassy Seed",
                SpriteNum = 667,
                Fling = new FlingData { BasePower = 10 },
                OnSwitchIn = new OnSwitchInEventInfo((battle, pokemon) =>
                {
                    if (!pokemon.IgnoringItem() &&
                        battle.Field.IsTerrain(ConditionId.GrassyTerrain, null))
                    {
                        pokemon.UseItem();
                    }
                }, -1),
                OnTerrainChange = new OnTerrainChangeEventInfo((battle, pokemon, source, effect) =>
                {
                    if (battle.Field.IsTerrain(ConditionId.GrassyTerrain, null))
                    {
                        pokemon.UseItem();
                    }
                }),
                Boosts = new SparseBoostsTable { Def = 1 },
                Num = 884,
                Gen = 7,
            },
            [ItemId.GreatBall] = new()
            {
                Id = ItemId.GreatBall,
                Name = "Great Ball",
                SpriteNum = 174,
                Num = 3,
                Gen = 1,
                IsPokeball = true,
            },
            [ItemId.GrepaBerry] = new()
            {
                Id = ItemId.GrepaBerry,
                Name = "Grepa Berry",
                SpriteNum = 178,
                IsBerry = true,
                NaturalGift = (90, "Flying"),
                // onEat: false - used for EV reduction, not relevant in Gen 9 battles
                Num = 173,
                Gen = 3,
            },
            [ItemId.GripClaw] = new()
            {
                Id = ItemId.GripClaw,
                Name = "Grip Claw",
                SpriteNum = 179,
                Fling = new FlingData { BasePower = 90 },
                // TODO: Implemented in statuses - extends binding move duration
                Num = 286,
                Gen = 4,
            },
            [ItemId.GriseousCore] = new()
            {
                Id = ItemId.GriseousCore,
                Name = "Griseous Core",
                SpriteNum = 743,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    // Giratina has species number 487
                    if (user.BaseSpecies.Num == 487 &&
                        (move.Type == MoveType.Ghost || move.Type == MoveType.Dragon))
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                OnTakeItem = new OnTakeItemEventInfo((Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion>)(
                    (_, item, pokemon, source, _) =>
                    {
                        // Giratina (species number 487) can't have this item removed
                        if (source?.BaseSpecies.Num == 487 || pokemon.BaseSpecies.Num == 487)
                        {
                            return BoolVoidUnion.FromBool(false);
                        }
                        return BoolVoidUnion.FromBool(true);
                    })),
                ForcedForme = "Giratina-Origin",
                Num = 1779,
                Gen = 8,
            },
            [ItemId.GriseousOrb] = new()
            {
                Id = ItemId.GriseousOrb,
                Name = "Griseous Orb",
                SpriteNum = 180,
                Fling = new FlingData { BasePower = 60 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    // Giratina has species number 487
                    if (user.BaseSpecies.Num == 487 &&
                        (move.Type == MoveType.Ghost || move.Type == MoveType.Dragon))
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                // Note: Unlike GriseousCore, GriseousOrb does NOT prevent item removal
                Num = 112,
                Gen = 4,
            },

            // H items
            [ItemId.HabanBerry] = new()
            {
                Id = ItemId.HabanBerry,
                Name = "Haban Berry",
                SpriteNum = 185,
                IsBerry = true,
                NaturalGift = (80, "Dragon"),
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, source, target, move) =>
                    {
                        if (move.Type == MoveType.Dragon && target.GetMoveHitData(move).TypeMod > 0)
                        {
                            var hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                         move.Flags.BypassSub != true &&
                                         !(move.Infiltrates == true && battle.Gen >= 6);
                            if (hitSub) return damage;

                            if (target.EatItem())
                            {
                                battle.Debug("-50% reduction");
                                battle.Add("-enditem", target, "item: Haban Berry", "[weaken]");
                                battle.ChainModify(0.5);
                                return battle.FinalModify(damage);
                            }
                        }

                        return damage;
                    }),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((_, _) => { })),
                Num = 197,
                Gen = 4,
            },
            [ItemId.HardStone] = new()
            {
                Id = ItemId.HardStone,
                Name = "Hard Stone",
                SpriteNum = 187,
                Fling = new FlingData { BasePower = 100 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Type == MoveType.Rock)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 238,
                Gen = 2,
            },
            [ItemId.HealBall] = new()
            {
                Id = ItemId.HealBall,
                Name = "Heal Ball",
                SpriteNum = 188,
                Num = 14,
                Gen = 4,
                IsPokeball = true,
            },
            [ItemId.HearthflameMask] = new()
            {
                Id = ItemId.HearthflameMask,
                Name = "Hearthflame Mask",
                SpriteNum = 760,
                Fling = new FlingData { BasePower = 60 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (user.BaseSpecies.Name.StartsWith("Ogerpon-Hearthflame"))
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                OnTakeItem = new OnTakeItemEventInfo((Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion>)(
                    (_, item, pokemon, source, _) =>
                    {
                        if (source?.BaseSpecies.Name.StartsWith("Ogerpon") == true || 
                            pokemon.BaseSpecies.Name.StartsWith("Ogerpon"))
                        {
                            return BoolVoidUnion.FromBool(false);
                        }
                        return BoolVoidUnion.FromBool(true);
                    })),
                ForcedForme = "Ogerpon-Hearthflame",
                Num = 2408,
                Gen = 9,
            },
            [ItemId.HeatRock] = new()
            {
                Id = ItemId.HeatRock,
                Name = "Heat Rock",
                SpriteNum = 193,
                Fling = new FlingData { BasePower = 60 },
                Num = 284,
                Gen = 4,
            },
            [ItemId.HeavyBall] = new()
            {
                Id = ItemId.HeavyBall,
                Name = "Heavy Ball",
                SpriteNum = 194,
                Num = 495,
                Gen = 2,
                IsPokeball = true,
            },
            [ItemId.HeavyDutyBoots] = new()
            {
                Id = ItemId.HeavyDutyBoots,
                Name = "Heavy-Duty Boots",
                SpriteNum = 715,
                Fling = new FlingData { BasePower = 80 },
                // Hazard immunity implemented in moves.ts
                Num = 1120,
                Gen = 8,
            },
            [ItemId.HondewBerry] = new()
            {
                Id = ItemId.HondewBerry,
                Name = "Hondew Berry",
                SpriteNum = 213,
                IsBerry = true,
                NaturalGift = (90, "Ground"),
                // onEat: false - used for EV reduction, not relevant in Gen 9 battles
                Num = 172,
                Gen = 3,
            },

            // I items
            [ItemId.IapapaBerry] = new()
            {
                Id = ItemId.IapapaBerry,
                Name = "Iapapa Berry",
                SpriteNum = 217,
                IsBerry = true,
                NaturalGift = (80, "Dark"),
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
                // TODO: OnTryEatItem for TryHeal event
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((battle, pokemon) =>
                {
                    battle.Heal(pokemon.BaseMaxHp / 3);
                    if (pokemon.GetNature().Minus == StatIdExceptHp.Def)
                    {
                        pokemon.AddVolatile(ConditionId.Confusion);
                    }
                })),
                Num = 163,
                Gen = 3,
            },
            [ItemId.IceStone] = new()
            {
                Id = ItemId.IceStone,
                Name = "Ice Stone",
                SpriteNum = 693,
                Fling = new FlingData { BasePower = 30 },
                Num = 849,
                Gen = 7,
            },
            [ItemId.IciclePlate] = new()
            {
                Id = ItemId.IciclePlate,
                Name = "Icicle Plate",
                SpriteNum = 220,
                OnPlate = PokemonType.Ice,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Type == MoveType.Ice)
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
                ForcedForme = "Arceus-Ice",
                Num = 302,
                Gen = 4,
            },
            [ItemId.IcyRock] = new()
            {
                Id = ItemId.IcyRock,
                Name = "Icy Rock",
                SpriteNum = 221,
                Fling = new FlingData { BasePower = 40 },
                Num = 282,
                Gen = 4,
            },
            [ItemId.InsectPlate] = new()
            {
                Id = ItemId.InsectPlate,
                Name = "Insect Plate",
                SpriteNum = 223,
                OnPlate = PokemonType.Bug,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Type == MoveType.Bug)
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
                ForcedForme = "Arceus-Bug",
                Num = 308,
                Gen = 4,
            },
            [ItemId.IronBall] = new()
            {
                Id = ItemId.IronBall,
                Name = "Iron Ball",
                SpriteNum = 224,
                Fling = new FlingData { BasePower = 130 },
                OnEffectiveness =
                    new OnEffectivenessEventInfo((battle, typeMod, target, type, move) =>
                    {
                        if (target == null) return typeMod;
                        if (target.Volatiles.ContainsKey(ConditionId.Ingrain) ||
                            target.Volatiles.ContainsKey(ConditionId.SmackDown) ||
                            battle.Field.GetPseudoWeather(ConditionId.Gravity) != null)
                            return typeMod;
                        if (move.Type == MoveType.Ground && target.HasType(PokemonType.Flying))
                            return 0;
                        return typeMod;
                    }),
                // Airborneness negation implemented in Pokemon.IsGrounded()
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
                {
                    battle.ChainModify(0.5);
                    return IntVoidUnion.FromInt(battle.FinalModify(spe));
                }),
                Num = 278,
                Gen = 4,
            },
            [ItemId.IronPlate] = new()
            {
                Id = ItemId.IronPlate,
                Name = "Iron Plate",
                SpriteNum = 225,
                OnPlate = PokemonType.Steel,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Type == MoveType.Steel)
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
                ForcedForme = "Arceus-Steel",
                Num = 313,
                Gen = 4,
            },
        };
    }
}