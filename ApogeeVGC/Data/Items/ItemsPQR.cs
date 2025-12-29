using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
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
    private partial Dictionary<ItemId, Item> CreateItemsPqr()
    {
        return new Dictionary<ItemId, Item>
        {
            // P items
            [ItemId.PamtreBerry] = new()
            {
                Id = ItemId.PamtreBerry,
                Name = "Pamtre Berry",
                SpriteNum = 323,
                IsBerry = true,
                NaturalGift = (90, "Steel"),
                // onEat: false - used for Poffin/Pokeblock creation, not relevant in Gen 9 battles
                Num = 180,
                Gen = 3,
                // IsNonstandard = "Past",
            },
            [ItemId.ParkBall] = new()
            {
                Id = ItemId.ParkBall,
                Name = "Park Ball",
                SpriteNum = 325,
                Num = 500,
                Gen = 4,
                IsPokeball = true,
                // IsNonstandard = "Unobtainable",
            },
            [ItemId.PasshoBerry] = new()
            {
                Id = ItemId.PasshoBerry,
                Name = "Passho Berry",
                SpriteNum = 329,
                IsBerry = true,
                NaturalGift = (80, "Water"),
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, source, target, move) =>
                    {
                        if (move.Type == MoveType.Water && target.GetMoveHitData(move).TypeMod > 0)
                        {
                            var hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                         move.Flags.BypassSub != true &&
                                         !(move.Infiltrates == true && battle.Gen >= 6);
                            if (hitSub) return damage;

                            if (target.EatItem())
                            {
                                battle.Debug("-50% reduction");
                                battle.Add("-enditem", target, "item: Passho Berry", "[weaken]");
                                battle.ChainModify(0.5);
                                return battle.FinalModify(damage);
                            }
                        }

                        return damage;
                    }),
                // OnEat: empty function
                Num = 185,
                Gen = 4,
            },
            [ItemId.PayapaBerry] = new()
            {
                Id = ItemId.PayapaBerry,
                Name = "Payapa Berry",
                SpriteNum = 330,
                IsBerry = true,
                NaturalGift = (80, "Psychic"),
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, source, target, move) =>
                    {
                        if (move.Type == MoveType.Psychic &&
                            target.GetMoveHitData(move).TypeMod > 0)
                        {
                            var hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                         move.Flags.BypassSub != true &&
                                         !(move.Infiltrates == true && battle.Gen >= 6);
                            if (hitSub) return damage;

                            if (target.EatItem())
                            {
                                battle.Debug("-50% reduction");
                                battle.Add("-enditem", target, "item: Payapa Berry", "[weaken]");
                                battle.ChainModify(0.5);
                                return battle.FinalModify(damage);
                            }
                        }

                        return damage;
                    }),
                // OnEat: empty function
                Num = 193,
                Gen = 4,
            },
            [ItemId.PechaBerry] = new()
            {
                Id = ItemId.PechaBerry,
                Name = "Pecha Berry",
                SpriteNum = 333,
                IsBerry = true,
                NaturalGift = (80, "Electric"),
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.Poison || pokemon.Status == ConditionId.Toxic)
                    {
                        pokemon.EatItem();
                    }
                }),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.Poison || pokemon.Status == ConditionId.Toxic)
                    {
                        pokemon.CureStatus();
                    }
                })),
                Num = 151,
                Gen = 3,
            },
            [ItemId.PersimBerry] = new()
            {
                Id = ItemId.PersimBerry,
                Name = "Persim Berry",
                SpriteNum = 334,
                IsBerry = true,
                NaturalGift = (80, "Ground"),
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Volatiles.ContainsKey(ConditionId.Confusion))
                    {
                        pokemon.EatItem();
                    }
                }),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((battle, pokemon) =>
                {
                    var confusionCondition = battle.Library.Conditions[ConditionId.Confusion];
                    pokemon.RemoveVolatile(confusionCondition);
                })),
                Num = 156,
                Gen = 3,
            },
            [ItemId.PetayaBerry] = new()
            {
                Id = ItemId.PetayaBerry,
                Name = "Petaya Berry",
                SpriteNum = 335,
                IsBerry = true,
                NaturalGift = (100, "Poison"),
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
                    battle.Boost(new SparseBoostsTable { SpA = 1 });
                })),
                Num = 204,
                Gen = 3,
            },
            // Skip pidgeotite - mega stone
            // Skip pikaniumz - z-move
            // Skip pikashuniumz - z-move
            [ItemId.PinapBerry] = new()
            {
                Id = ItemId.PinapBerry,
                Name = "Pinap Berry",
                SpriteNum = 337,
                IsBerry = true,
                NaturalGift = (90, "Grass"),
                // onEat: false - used for wild Pokemon mechanics, not relevant in Gen 9 battles
                Num = 168,
                Gen = 3,
                // IsNonstandard = "Past",
            },
            // Skip pinsirite - mega stone
            [ItemId.PixiePlate] = new()
            {
                Id = ItemId.PixiePlate,
                Name = "Pixie Plate",
                SpriteNum = 610,
                OnPlate = PokemonType.Fairy,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Type == MoveType.Fairy)
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
                ForcedForme = "Arceus-Fairy",
                Num = 644,
                Gen = 6,
            },
            [ItemId.PlumeFossil] = new()
            {
                Id = ItemId.PlumeFossil,
                Name = "Plume Fossil",
                SpriteNum = 339,
                Fling = new FlingData { BasePower = 100 },
                Num = 573,
                Gen = 5,
                // IsNonstandard = "Past",
            },
            [ItemId.PoisonBarb] = new()
            {
                Id = ItemId.PoisonBarb,
                Name = "Poison Barb",
                SpriteNum = 343,
                Fling = new FlingData
                {
                    BasePower = 70,
                    Status = ConditionId.Poison
                },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Type == MoveType.Poison)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 245,
                Gen = 2,
            },
            // Skip poisongem, poisonmemory, poisoniumz - not Gen 9
            [ItemId.PokeBall] = new()
            {
                Id = ItemId.PokeBall,
                Name = "Poke Ball",
                SpriteNum = 345,
                Num = 4,
                Gen = 1,
                IsPokeball = true,
            },
            [ItemId.PomegBerry] = new()
            {
                Id = ItemId.PomegBerry,
                Name = "Pomeg Berry",
                SpriteNum = 351,
                IsBerry = true,
                NaturalGift = (90, "Ice"),
                // onEat: false - used for EV reduction, not relevant in Gen 9 battles
                Num = 169,
                Gen = 3,
            },
            [ItemId.PowerAnklet] = new()
            {
                Id = ItemId.PowerAnklet,
                Name = "Power Anklet",
                SpriteNum = 354,
                IgnoreKlutz = true,
                Fling = new FlingData { BasePower = 70 },
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
                {
                    battle.ChainModify(0.5);
                    return battle.FinalModify(spe);
                }),
                Num = 293,
                Gen = 4,
            },
            [ItemId.PowerBand] = new()
            {
                Id = ItemId.PowerBand,
                Name = "Power Band",
                SpriteNum = 355,
                IgnoreKlutz = true,
                Fling = new FlingData { BasePower = 70 },
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
                {
                    battle.ChainModify(0.5);
                    return battle.FinalModify(spe);
                }),
                Num = 292,
                Gen = 4,
            },
            [ItemId.PowerBelt] = new()
            {
                Id = ItemId.PowerBelt,
                Name = "Power Belt",
                SpriteNum = 356,
                IgnoreKlutz = true,
                Fling = new FlingData { BasePower = 70 },
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
                {
                    battle.ChainModify(0.5);
                    return battle.FinalModify(spe);
                }),
                Num = 290,
                Gen = 4,
            },
            [ItemId.PowerBracer] = new()
            {
                Id = ItemId.PowerBracer,
                Name = "Power Bracer",
                SpriteNum = 357,
                IgnoreKlutz = true,
                Fling = new FlingData { BasePower = 70 },
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
                {
                    battle.ChainModify(0.5);
                    return battle.FinalModify(spe);
                }),
                Num = 289,
                Gen = 4,
            },
            [ItemId.PowerLens] = new()
            {
                Id = ItemId.PowerLens,
                Name = "Power Lens",
                SpriteNum = 359,
                IgnoreKlutz = true,
                Fling = new FlingData { BasePower = 70 },
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
                {
                    battle.ChainModify(0.5);
                    return battle.FinalModify(spe);
                }),
                Num = 291,
                Gen = 4,
            },
            [ItemId.PowerWeight] = new()
            {
                Id = ItemId.PowerWeight,
                Name = "Power Weight",
                SpriteNum = 360,
                IgnoreKlutz = true,
                Fling = new FlingData { BasePower = 70 },
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
                {
                    battle.ChainModify(0.5);
                    return battle.FinalModify(spe);
                }),
                Num = 294,
                Gen = 4,
            },
            [ItemId.PremierBall] = new()
            {
                Id = ItemId.PremierBall,
                Name = "Premier Ball",
                SpriteNum = 363,
                Num = 12,
                Gen = 3,
                IsPokeball = true,
            },
            [ItemId.PrettyFeather] = new()
            {
                Id = ItemId.PrettyFeather,
                Name = "Pretty Feather",
                SpriteNum = 1,
                Fling = new FlingData { BasePower = 20 },
                Num = 571,
                Gen = 5,
            },
            // Skip primariumz - z-move
            [ItemId.PowerHerb] = new()
            {
                Id = ItemId.PowerHerb,
                Name = "Power Herb",
                SpriteNum = 358,
                Fling = new FlingData { BasePower = 10 },
                OnChargeMove = new OnChargeMoveEventInfo((battle, pokemon, target, move) =>
                {
                    if (pokemon.UseItem())
                    {
                        battle.Debug($"power herb - remove charge turn for {move.Id}");
                        battle.AttrLastMove("[still]");
                        return BoolVoidUnion.FromBool(false); // skip charge turn
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                Num = 271,
                Gen = 4,
            },
            [ItemId.PrismScale] = new()
            {
                Id = ItemId.PrismScale,
                Name = "Prism Scale",
                SpriteNum = 365,
                Fling = new FlingData { BasePower = 30 },
                Num = 537,
                Gen = 5,
            },
            [ItemId.Protector] = new()
            {
                Id = ItemId.Protector,
                Name = "Protector",
                SpriteNum = 367,
                Fling = new FlingData { BasePower = 80 },
                Num = 321,
                Gen = 4,
            },
            [ItemId.ProtectivePads] = new()
            {
                Id = ItemId.ProtectivePads,
                Name = "Protective Pads",
                SpriteNum = 663,
                Fling = new FlingData { BasePower = 30 },
                // Protective effect handled in Battle.CheckMoveMakesContact
                // Prevents the holder from making contact with targets
                // TODO: Implement in CheckMoveMakesContact method
                Num = 880,
                Gen = 7,
            },
            // Skip psychicgem, psychicmemory - not Gen 9
            [ItemId.PsychicSeed] = new()
            {
                Id = ItemId.PsychicSeed,
                Name = "Psychic Seed",
                SpriteNum = 665,
                Fling = new FlingData { BasePower = 10 },
                OnSwitchIn = new OnSwitchInEventInfo((battle, pokemon) =>
                {
                    if (!pokemon.IgnoringItem() &&
                        battle.Field.IsTerrain(ConditionId.PsychicTerrain, null))
                    {
                        pokemon.UseItem();
                    }
                }, -1),
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    if (!pokemon.IgnoringItem() &&
                        battle.Field.IsTerrain(ConditionId.PsychicTerrain, null))
                    {
                        pokemon.UseItem();
                    }
                }),
                OnTerrainChange = new OnTerrainChangeEventInfo((battle, pokemon, source, effect) =>
                {
                    if (battle.Field.IsTerrain(ConditionId.PsychicTerrain, null))
                    {
                        pokemon.UseItem();
                    }
                }),
                Boosts = new SparseBoostsTable { SpD = 1 },
                Num = 882,
                Gen = 7,
            },
            // Skip psychiumz - z-move
            [ItemId.PunchingGlove] = new()
            {
                Id = ItemId.PunchingGlove,
                Name = "Punching Glove",
                SpriteNum = 749,
                Fling = new FlingData { BasePower = 30 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Flags.Punch == true)
                    {
                        battle.Debug("Punching Glove boost");
                        battle.ChainModify([4506, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 23),
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, pokemon, target) =>
                {
                    if (move.Flags.Punch == true)
                    {
                        move.Flags = move.Flags with { Contact = null };
                    }
                }, 1),
                Num = 1884,
                Gen = 9,
            },
            // Skip pyroarite - mega stone

            // Q items
            [ItemId.QualotBerry] = new()
            {
                Id = ItemId.QualotBerry,
                Name = "Qualot Berry",
                SpriteNum = 371,
                IsBerry = true,
                NaturalGift = (90, "Poison"),
                // onEat: false - used for EV reduction, not relevant in Gen 9 battles
                Num = 171,
                Gen = 3,
            },
            [ItemId.QuickBall] = new()
            {
                Id = ItemId.QuickBall,
                Name = "Quick Ball",
                SpriteNum = 372,
                Num = 15,
                Gen = 4,
                IsPokeball = true,
            },
            [ItemId.QuickClaw] = new()
            {
                Id = ItemId.QuickClaw,
                Name = "Quick Claw",
                SpriteNum = 373,
                Fling = new FlingData { BasePower = 80 },
                OnFractionalPriority = new OnFractionalPriorityEventInfo(
                    (ModifierSourceMoveHandler)((battle, priority, pokemon, target, move) =>
                    {
                        if (move.Category == MoveCategory.Status &&
                            pokemon.HasAbility(AbilityId.MyceliumMight))
                            return DoubleVoidUnion.FromVoid();
                        if (priority <= 0 && battle.RandomChance(1, 5))
                        {
                            battle.Add("-activate", pokemon, "item: Quick Claw");
                            return DoubleVoidUnion.FromDouble(0.1);
                        }

                        return DoubleVoidUnion.FromVoid();
                    }), -2),
                Num = 217,
                Gen = 2,
            },
            [ItemId.QuickPowder] = new()
            {
                Id = ItemId.QuickPowder,
                Name = "Quick Powder",
                SpriteNum = 374,
                Fling = new FlingData { BasePower = 10 },
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
                {
                    if (pokemon.Species.Name == "Ditto" && !pokemon.Transformed)
                    {
                        battle.ChainModify(2);
                        return battle.FinalModify(spe);
                    }

                    return spe;
                }),
                Num = 274,
                Gen = 4,
                // IsNonstandard = "Past",
            },

            // R items
            [ItemId.RabutaBerry] = new()
            {
                Id = ItemId.RabutaBerry,
                Name = "Rabuta Berry",
                SpriteNum = 375,
                IsBerry = true,
                NaturalGift = (90, "Ghost"),
                // onEat: false - used for Poffin/Pokeblock creation, not relevant in Gen 9 battles
                Num = 177,
                Gen = 3,
                // IsNonstandard = "Past",
            },
            [ItemId.RareBone] = new()
            {
                Id = ItemId.RareBone,
                Name = "Rare Bone",
                SpriteNum = 379,
                Fling = new FlingData { BasePower = 100 },
                Num = 106,
                Gen = 4,
            },
            [ItemId.RawstBerry] = new()
            {
                Id = ItemId.RawstBerry,
                Name = "Rawst Berry",
                SpriteNum = 381,
                IsBerry = true,
                NaturalGift = (80, "Grass"),
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.Burn)
                    {
                        pokemon.EatItem();
                    }
                }),
                OnEat = new OnEatEventInfo((Action<Battle, Pokemon>)((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.Burn)
                    {
                        pokemon.CureStatus();
                    }
                })),
                Num = 152,
                Gen = 3,
            },
            [ItemId.RazorClaw] = new()
            {
                Id = ItemId.RazorClaw,
                Name = "Razor Claw",
                SpriteNum = 382,
                Fling = new FlingData { BasePower = 80 },
                OnModifyCritRatio =
                    new OnModifyCritRatioEventInfo((battle, critRatio, source, target, move) =>
                    {
                        return DoubleVoidUnion.FromDouble(critRatio + 1);
                    }),
                Num = 326,
                Gen = 4,
            },
            [ItemId.RockyHelmet] = new()
            {
                Id = ItemId.RockyHelmet,
                Name = "Rocky Helmet",
                SpriteNum = 417,
                Fling = new FlingData { BasePower = 60 },
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, source, move) =>
                {
                    if (battle.CheckMoveMakesContact(move, source, target))
                    {
                        battle.Damage(source.BaseMaxHp / 6, source, target);
                    }
                })
                {
                    Order = 2,
                },
                Num = 540,
                Gen = 5,
            },
            [ItemId.RazorFang] = new()
            {
                Id = ItemId.RazorFang,
                Name = "Razor Fang",
                SpriteNum = 383,
                Fling = new FlingData
                {
                    BasePower = 30,
                },
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, pokemon, target) =>
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
                Num = 327,
                Gen = 4,
            },
            [ItemId.ReaperCloth] = new()
            {
                Id = ItemId.ReaperCloth,
                Name = "Reaper Cloth",
                SpriteNum = 385,
                Fling = new FlingData { BasePower = 10 },
                Num = 325,
                Gen = 4,
            },
            [ItemId.ReBattle] = new()
            {
                Id = ItemId.ReBattle,
                Name = "ReBattle",
                SpriteNum = 0,
                Num = 0,
                Gen = 9,
            },
            [ItemId.RedCard] = new()
            {
                Id = ItemId.RedCard,
                Name = "Red Card",
                SpriteNum = 387,
                Fling = new FlingData { BasePower = 10 },
                OnAfterMoveSecondary = new OnAfterMoveSecondaryEventInfo((battle, target, source, move) =>
                {
                    if (source != null && source != target && source.Hp > 0 && target.Hp > 0 &&
                        move != null && move.Category != MoveCategory.Status)
                    {
                        if (!source.IsActive || battle.CanSwitch(source.Side) == 0 ||
                            source.ForceSwitchFlag || target.ForceSwitchFlag)
                        {
                            return;
                        }
                        // The item is used up even against a pokemon with Ingrain or that otherwise can't be forced out
                        if (target.UseItem(source))
                        {
                            var dragOutResult = battle.RunEvent(EventId.DragOut, source, target, move,
                                new BoolRelayVar(true));
                            if (dragOutResult is BoolRelayVar boolVar && boolVar.Value)
                            {
                                source.ForceSwitchFlag = true;
                            }
                        }
                    }
                }),
                Num = 542,
                Gen = 5,
            },
            [ItemId.RepeatBall] = new()
            {
                Id = ItemId.RepeatBall,
                Name = "Repeat Ball",
                SpriteNum = 401,
                Num = 9,
                Gen = 3,
                IsPokeball = true,
            },
            [ItemId.RibbonSweet] = new()
            {
                Id = ItemId.RibbonSweet,
                Name = "Ribbon Sweet",
                SpriteNum = 710,
                Fling = new FlingData { BasePower = 10 },
                Num = 1115,
                Gen = 8,
            },
            [ItemId.RindoBerry] = new()
            {
                Id = ItemId.RindoBerry,
                Name = "Rindo Berry",
                SpriteNum = 409,
                IsBerry = true,
                NaturalGift = (80, "Grass"),
                OnSourceModifyDamage = new OnSourceModifyDamageEventInfo((battle, damage, source, target, move) =>
                {
                    if (move.Type == MoveType.Grass && target.GetMoveHitData(move).TypeMod > 0)
                    {
                        var hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                     move.Flags.BypassSub != true &&
                                     !(move.Infiltrates == true && battle.Gen >= 6);
                        if (hitSub) return damage;

                        if (target.EatItem())
                        {
                            battle.Debug("-50% reduction");
                            battle.Add("-enditem", target, "item: Rindo Berry", "[weaken]");
                            battle.ChainModify(0.5);
                            return battle.FinalModify(damage);
                        }
                    }

                    return damage;
                }),
                // OnEat: empty function
                Num = 187,
                Gen = 4,
            },
            [ItemId.RingTarget] = new()
            {
                Id = ItemId.RingTarget,
                Name = "Ring Target",
                SpriteNum = 410,
                Fling = new FlingData { BasePower = 10 },
                OnNegateImmunity = new OnNegateImmunityEventInfo(false),
                Num = 543,
                Gen = 5,
            },
            [ItemId.RoomService] = new()
            {
                Id = ItemId.RoomService,
                Name = "Room Service",
                SpriteNum = 717,
                Fling = new FlingData { BasePower = 100 },
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    if (!pokemon.IgnoringItem() &&
                        battle.Field.GetPseudoWeather(ConditionId.TrickRoom) != null)
                    {
                        pokemon.UseItem();
                    }
                }),
                OnAnyPseudoWeatherChange = new OnAnyPseudoWeatherChangeEventInfo((battle, pokemon, source, condition) =>
                {
                    if (battle.Field.GetPseudoWeather(ConditionId.TrickRoom) != null)
                    {
                        pokemon.UseItem(pokemon);
                    }
                }),
                Boosts = new SparseBoostsTable { Spe = -1 },
                Num = 1122,
                Gen = 8,
            },
            [ItemId.RootFossil] = new()
            {
                Id = ItemId.RootFossil,
                Name = "Root Fossil",
                SpriteNum = 418,
                Fling = new FlingData { BasePower = 100 },
                Num = 99,
                Gen = 3,
                // isNonstandard: "Past"
            },
            [ItemId.RoseIncense] = new()
            {
                Id = ItemId.RoseIncense,
                Name = "Rose Incense",
                SpriteNum = 419,
                Fling = new FlingData { BasePower = 10 },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Type == MoveType.Grass)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                Num = 318,
                Gen = 4,
                // isNonstandard: "Past"
            },
            [ItemId.RoseliaBerry] = new()
            {
                Id = ItemId.RoseliaBerry,
                Name = "Roseli Berry",
                SpriteNum = 603,
                IsBerry = true,
                NaturalGift = (80, "Fairy"),
                OnSourceModifyDamage = new OnSourceModifyDamageEventInfo((battle, damage, source, target, move) =>
                {
                    if (move.Type == MoveType.Fairy && target.GetMoveHitData(move).TypeMod > 0)
                    {
                        var hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                     move.Flags.BypassSub != true &&
                                     !(move.Infiltrates == true && battle.Gen >= 6);
                        if (hitSub) return damage;

                        if (target.EatItem())
                        {
                            battle.Debug("-50% reduction");
                            battle.Add("-enditem", target, "item: Roseli Berry", "[weaken]");
                            battle.ChainModify(0.5);
                            return battle.FinalModify(damage);
                        }
                    }

                    return damage;
                }),
                // OnEat: empty function
                Num = 686,
                Gen = 6,
            },
            [ItemId.RowapBerry] = new()
            {
                Id = ItemId.RowapBerry,
                Name = "Rowap Berry",
                SpriteNum = 420,
                IsBerry = true,
                NaturalGift = (100, "Dark"),
                OnDamagingHit = new OnDamagingHitEventInfo((battle, damage, target, source, move) =>
                {
                    if (move.Category == MoveCategory.Special && source.Hp > 0 && source.IsActive &&
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
                Num = 212,
                Gen = 4,
            },
            [ItemId.RustedShield] = new()
            {
                Id = ItemId.RustedShield,
                Name = "Rusted Shield",
                SpriteNum = 699,
                // OnTakeItem - Zamazenta can't have this item removed
                // itemUser: ["Zamazenta-Crowned"],
                Num = 1104,
                Gen = 8,
            },
            [ItemId.RustedSword] = new()
            {
                Id = ItemId.RustedSword,
                Name = "Rusted Sword",
                SpriteNum = 698,
                // OnTakeItem - Zacian can't have this item removed
                // itemUser: ["Zacian-Crowned"],
                Num = 1103,
                Gen = 8,
            },
        };
    }
}