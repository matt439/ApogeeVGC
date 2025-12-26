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
                    if (user.BaseSpecies.Num == 483 && (move.Type == MoveType.Steel || move.Type == MoveType.Dragon))
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
                    if (user.BaseSpecies.Num == 483 && (move.Type == MoveType.Steel || move.Type == MoveType.Dragon))
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
                    if (target.Boosts.GetBoost(BoostId.Spe) == 6 || boost.GetBoost(BoostId.Atk) == 0)
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
                        var canHeal = battle.RunEvent(EventId.TryHeal, pokemon, null, battle.Effect, pokemon.BaseMaxHp / 3);
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
                    if (!target.IgnoringItem() && battle.Field.GetPseudoWeather(ConditionId.Gravity) == null)
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
                    battle.RunEvent(EventId.AfterUseItem, target, null, null, _library.Items[ItemId.AirBalloon]);
                }),
                OnAfterSubDamage = new OnAfterSubDamageEventInfo((battle, damage, target, source, effect) =>
                {
                    battle.Debug($"effect: {effect.Id}");
                    if (effect.EffectType == EffectType.Move)
                    {
                        battle.Add("-enditem", target, "Air Balloon");
                        target.Item = ItemId.None;
                        var itemState = target.ItemState;
                        battle.ClearEffectState(ref itemState);
                        battle.RunEvent(EventId.AfterUseItem, target, null, null, _library.Items[ItemId.AirBalloon]);
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
        };
    }
}
