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

namespace ApogeeVGC.Data.Items;

public partial record Items
{
    private partial Dictionary<ItemId, Item> CreateItemsVwx()
    {
        return new Dictionary<ItemId, Item>
        {
            // V items - all are mega stones or non-standard, skipped

            // W items
            [ItemId.WacanBerry] = new()
            {
                Id = ItemId.WacanBerry,
                Name = "Wacan Berry",
                SpriteNum = 526,
                IsBerry = true,
                NaturalGift = (80, "Electric"),
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, _, target, move) =>
                    {
                        if (move.Type == MoveType.Electric &&
                            target.GetMoveHitData(move).TypeMod > 0)
                        {
                            bool hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                          move.Flags.BypassSub != true &&
                                          !(move.Infiltrates == true && battle.Gen >= 6);
                            if (hitSub) return damage;

                            if (target.EatItem())
                            {
                                battle.Debug("-50% reduction");
                                battle.Add("-enditem", target, "item: Wacan Berry", "[weaken]");
                                battle.ChainModify(0.5);
                                return battle.FinalModify(damage);
                            }
                        }

                        return damage;
                    }),
                // OnEat: empty function
                Num = 186,
                Gen = 4,
            },
            // Skip watergem, watermemory, wateriumz - not Gen 9 standard
            [ItemId.WaterStone] = new()
            {
                Id = ItemId.WaterStone,
                Name = "Water Stone",
                SpriteNum = 529,
                Fling = new FlingData { BasePower = 30 },
                Num = 84,
                Gen = 1,
            },
            // Skip watmelberry, waveincense - not Gen 9 standard
            [ItemId.WeaknessPolicy] = new()
            {
                Id = ItemId.WeaknessPolicy,
                Name = "Weakness Policy",
                SpriteNum = 609,
                Fling = new FlingData { BasePower = 80 },
                OnDamagingHit = new OnDamagingHitEventInfo((_, _, target, _, move) =>
                {
                    // Only trigger if the move doesn't have fixed damage (like Seismic Toss)
                    // TypeScript: !move.damage && !move.damageCallback
                    if (move.Damage == null && move.DamageCallback == null &&
                        target.GetMoveHitData(move).TypeMod > 0)
                    {
                        target.UseItem();
                    }
                }),
                Boosts = new SparseBoostsTable { Atk = 2, SpA = 2 },
                Num = 639,
                Gen = 6,
            },
            [ItemId.WellspringMask] = new()
            {
                Id = ItemId.WellspringMask,
                Name = "Wellspring Mask",
                SpriteNum = 759,
                Fling = new FlingData { BasePower = 60 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, _, _) =>
                {
                    // TS checks: user.baseSpecies.name.startsWith('Ogerpon-Wellspring')
                    if (user.BaseSpecies.Id == SpecieId.OgerponWellspring ||
                        user.BaseSpecies.Id == SpecieId.OgerponWellspringTera)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                OnTakeItem = new OnTakeItemEventInfo(
                    (Func<Battle, Item, Pokemon, Pokemon?, Move?, BoolVoidUnion>)(
                        (_, _, pokemon, _, _) =>
                        {
                            // Ogerpon cannot have its mask removed
                            // TypeScript only checks the holder (pokemon), not the source
                            if (pokemon.BaseSpecies.BaseSpecies == SpecieId.Ogerpon)
                            {
                                return BoolVoidUnion.FromBool(false);
                            }

                            return BoolVoidUnion.FromBool(true);
                        })),
                ForcedForme = "Ogerpon-Wellspring",
                // itemUser: ["Ogerpon-Wellspring"],
                Num = 2407,
                Gen = 9,
            },
            // Skip wepearberry, whippeddream - not Gen 9 standard
            [ItemId.WhiteHerb] = new()
            {
                Id = ItemId.WhiteHerb,
                Name = "White Herb",
                SpriteNum = 535,
                Fling = new FlingData
                {
                    BasePower = 10,
                    Effect = (battle, target, _, _) =>
                    {
                        bool activate = false;
                        var boosts = new SparseBoostsTable();
                        foreach (BoostId stat in Enum.GetValues<BoostId>())
                        {
                            if (target.Boosts.GetBoost(stat) < 0)
                            {
                                activate = true;
                                boosts.SetBoost(stat, 0);
                            }
                        }

                        if (activate)
                        {
                            target.SetBoost(boosts);
                            battle.Add("-clearnegativeboost", target, "[silent]");
                        }

                        return BoolEmptyVoidUnion.FromVoid();
                    }
                },
                OnStart = new OnStartEventInfo(TryUseWhiteHerb),
                OnAnySwitchIn = new OnAnySwitchInEventInfo(TryUseWhiteHerb, priority: -2),
                OnAnyAfterMega = new OnAnyAfterMegaEventInfo(TryUseWhiteHerb),
                OnAnyAfterTerastallization =
                    new OnAnyAfterTerastallizationEventInfo(TryUseWhiteHerb),
                OnAnyAfterMove = new OnAnyAfterMoveEventInfo((battle, pokemon, _, _) =>
                {
                    TryUseWhiteHerb(battle, pokemon);
                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo(
                    (battle, pokemon, _, _) => { TryUseWhiteHerb(battle, pokemon); }, order: 29),
                OnUse = new OnUseEventInfo((Action<Battle, Pokemon>)((battle, pokemon) =>
                {
                    if (pokemon.ItemState.Boosts != null)
                    {
                        pokemon.SetBoost(pokemon.ItemState.Boosts);
                        if (battle.DisplayUi)
                        {
                            battle.Add("-clearnegativeboost", pokemon, "[silent]");
                        }
                    }
                })),
                OnEnd = new OnEndEventInfo((_, pokemon) => { pokemon.ItemState.Boosts = null; }),
                Num = 214,
                Gen = 3,
            },
            [ItemId.WideLens] = new()
            {
                Id = ItemId.WideLens,
                Name = "Wide Lens",
                SpriteNum = 537,
                Fling = new FlingData { BasePower = 10 },
                OnSourceModifyAccuracy = new OnSourceModifyAccuracyEventInfo(
                    (battle, accuracy, _, _, move) =>
                    {
                        // Only modify numeric accuracy (TypeScript: typeof accuracy === 'number')
                        // Moves with true accuracy (always hit) have accuracy == null
                        if (accuracy.HasValue)
                        {
                            battle.ChainModify([4505, 4096]);
                            int result = battle.FinalModify(accuracy.Value);
                            return DoubleVoidUnion.FromDouble(result);
                        }

                        return DoubleVoidUnion.FromVoid();
                    }, -2),
                Num = 265,
                Gen = 4,
            },
            [ItemId.WikiBerry] = new()
            {
                Id = ItemId.WikiBerry,
                Name = "Wiki Berry",
                SpriteNum = 538,
                IsBerry = true,
                NaturalGift = (80, "Rock"),
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
                    if (pokemon.GetNature().Minus == StatIdExceptHp.SpA)
                    {
                        pokemon.AddVolatile(ConditionId.Confusion);
                    }
                })),
                Num = 160,
                Gen = 3,
            },
            [ItemId.WiseGlasses] = new()
            {
                Id = ItemId.WiseGlasses,
                Name = "Wise Glasses",
                SpriteNum = 539,
                Fling = new FlingData { BasePower = 10 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Category == MoveCategory.Special)
                    {
                        battle.ChainModify([4505, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 16),
                Num = 267,
                Gen = 4,
            },

            // X items - none that start with X in the standard game
        };
    }

    private static void TryUseWhiteHerb(Battle battle, Pokemon pokemon)
    {
        var boosts = new SparseBoostsTable();
        bool ready = false;
        foreach (BoostId stat in Enum.GetValues<BoostId>())
        {
            if (pokemon.Boosts.GetBoost(stat) < 0)
            {
                ready = true;
                boosts.SetBoost(stat, 0);
            }
        }

        if (ready)
        {
            pokemon.ItemState.Boosts = boosts;
            pokemon.UseItem();
        }
    }
}