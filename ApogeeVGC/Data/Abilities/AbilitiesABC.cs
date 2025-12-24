using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Abilities;

public partial record Abilities
{
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesAbc()
    {
        return new Dictionary<AbilityId, Ability>
        {
            [AbilityId.Adaptability] = new()
            {
                Id = AbilityId.Adaptability,
                Name = "Adaptability",
                Num = 91,
                Rating = 4.0,
                OnModifyStab = new OnModifyStabEventInfo((battle, stab, source, target, move) =>
                {
                    if ((move.ForceStab ?? false) ||
                        source.HasType(move.Type.ConvertToPokemonType()))
                    {
                        if (stab == 2)
                        {
                            return 2.25;
                        }

                        return 2.0;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.Aerilate] = new()
            {
                Id = AbilityId.Aerilate,
                Name = "Aerilate",
                Num = 184,
                Rating = 4.0,
                //OnModifyTypePriority = -1,
                OnModifyType = new OnModifyTypeEventInfo((battle, move, pokemon, _) =>
                    {
                        // Change Normal-type moves to Flying
                        // TODO: Add checks for specific moves like Judgment, Multi-Attack, etc.
                        if (move.Type == MoveType.Normal && move.Category != MoveCategory.Status)
                        {
                            move.Type = MoveType.Flying;
                            move.TypeChangerBoosted = battle.Effect;
                        }
                    },
                    -1),
                //OnBasePowerPriority = 23,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                    {
                        if (move.TypeChangerBoosted == battle.Effect)
                        {
                            battle.ChainModify([4915, 4096]);
                            return battle.FinalModify(basePower);
                        }

                        return basePower;
                    },
                    23),
            },
            [AbilityId.Aftermath] = new()
            {
                Id = AbilityId.Aftermath,
                Name = "Aftermath",
                Num = 106,
                Rating = 2.0,
                //OnDamagingHitOrder = 1,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, source, move) =>
                    {
                        if (target.Hp == 0 &&
                            battle.CheckMoveMakesContact(move, source, target, true))
                        {
                            battle.Damage(source.BaseMaxHp / 4, source, target);
                        }
                    },
                    1),
            },
            [AbilityId.AirLock] = new()
            {
                Id = AbilityId.AirLock,
                Name = "Air Lock",
                Num = 76,
                Rating = 1.5,
                OnSwitchIn = new OnSwitchInEventInfo((battle, pokemon) =>
                {
                    // Air Lock does not activate when Skill Swapped or when Neutralizing Gas leaves the field
                    battle.Add("-ability", pokemon, "Air Lock");
                    // Call onStart
                    battle.SingleEvent(EventId.Start, battle.Effect, battle.EffectState, pokemon);
                }),
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    pokemon.AbilityState.Ending = false; // Clear the ending flag
                    battle.EachEvent(EventId.WeatherChange, battle.Effect);
                }),
                OnEnd = new OnEndEventInfo((battle, pokemonUnion) =>
                {
                    if (pokemonUnion is not PokemonSideFieldPokemon psfp) return;
                    psfp.Pokemon.AbilityState.Ending = true;
                    battle.EachEvent(EventId.WeatherChange, battle.Effect);
                }),
                SuppressWeather = true,
            },
            [AbilityId.Analytic] = new()
            {
                Id = AbilityId.Analytic,
                Name = "Analytic",
                Num = 148,
                Rating = 2.5,
                //OnBasePowerPriority = 21,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, pokemon, _, _) =>
                    {
                        bool boosted = true;
                        foreach (var target in battle.GetAllActive())
                        {
                            if (target == pokemon) continue;
                            if (battle.Queue.WillMove(target) != null)
                            {
                                boosted = false;
                                break;
                            }
                        }

                        if (boosted)
                        {
                            battle.Debug("Analytic boost");
                            battle.ChainModify([5325, 4096]);
                            return battle.FinalModify(basePower);
                        }

                        return basePower;
                    },
                    21),
            },
            [AbilityId.AngerPoint] = new()
            {
                Id = AbilityId.AngerPoint,
                Name = "Anger Point",
                Num = 83,
                Rating = 2.0,
                OnHit = new OnHitEventInfo((battle, target, source, move) =>
                {
                    if (move is not ActiveMove) return new VoidReturn();
                    if (!target.GetMoveHitData(move).Crit) return new VoidReturn();

                    target.SetBoost(new SparseBoostsTable { Atk = 6 });
                    return new VoidReturn();
                }),
            },
            [AbilityId.AsOneGlastrier] = new()
            {
                Id = AbilityId.AsOneGlastrier,
                Name = "As One (Glastrier)",
                Num = 266,
                Rating = 3.5,
                //OnSwitchInPriority = 1,
                OnSwitchIn = new OnSwitchInEventInfo((_, _) => { },
                    1
                ),
                OnStart = new OnStartEventInfo(
                    (battle, pokemon) =>
                    {
                        if (battle.EffectState.Unnerved is true) return;
                        if (battle.DisplayUi)
                        {
                            //UiGenerator.PrintAbilityEvent(pokemon, "As One");
                            //UiGenerator.PrintAbilityEvent(pokemon, _library.Abilities[AbilityId.Unnerve]);

                            battle.Add("-ability", pokemon, "As One");
                            battle.Add("-ability", pokemon, "Unnerve");
                        }

                        battle.EffectState.Unnerved = true;
                    },
                    1
                ),
                OnEnd = new OnEndEventInfo((battle, _) => { battle.EffectState.Unnerved = false; }),
                OnFoeTryEatItem = new OnFoeTryEatItemEventInfo(
                    OnTryEatItem.FromFunc((battle, _, _) =>
                        BoolVoidUnion.FromBool(!(battle.EffectState.Unnerved ?? false)))),
                OnSourceAfterFaint =
                    new OnSourceAfterFaintEventInfo((battle, length, _, source, effect) =>
                    {
                        if (effect is null || effect.EffectType != EffectType.Move) return;

                        battle.Boost(new SparseBoostsTable { Atk = length }, source, source,
                            _library.Abilities[AbilityId.ChillingNeigh]);
                    }),
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                    CantSuppress = true,
                },
            },
            [AbilityId.Bulletproof] = new()
            {
                Id = AbilityId.Bulletproof,
                Name = "Bulletproof",
                Num = 171,
                Rating = 3.0,
                Flags = new AbilityFlags { Breakable = true },
                OnTryHit = new OnTryHitEventInfo((battle, pokemon, _, move) =>
                {
                    if (move.Flags.Bullet == true)
                    {
                        battle.Add("-immune", pokemon, "[from] ability: Bulletproof");
                        return null;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.ChillingNeigh] = new()
            {
                Id = AbilityId.ChillingNeigh,
                Name = "Chilling Neigh",
                Num = 264,
                Rating = 3.0,
                OnSourceAfterFaint =
                    new OnSourceAfterFaintEventInfo((battle, length, _, source, effect) =>
                    {
                        if (effect is null || effect.EffectType != EffectType.Move) return;
                        battle.Boost(new SparseBoostsTable { Atk = length }, source);
                    }),
            },
        };
    }
}
