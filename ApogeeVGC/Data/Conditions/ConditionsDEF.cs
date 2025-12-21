using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.FieldEventMethods;
using ApogeeVGC.Sim.Events.Handlers.SideEventMethods;
using ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsDEF()
    {
        return new Dictionary<ConditionId, Condition>
        {
            [ConditionId.ElectricTerrain] = new()
            {
                Id = ConditionId.ElectricTerrain,
                Name = "Electric Terrain",
                EffectType = EffectType.Terrain,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, source, _, _) =>
                    source.HasItem(ItemId.TerrainExtender) ? 8 : 5),
                OnSetStatus = new OnSetStatusEventInfo((battle, status, target, _, effect) =>
                {
                    if (status.Id == ConditionId.Sleep &&
                        (target.IsGrounded() ?? false) &&
                        !target.IsSemiInvulnerable())
                    {
                        if (battle.DisplayUi && effect is Condition { Id: ConditionId.Yawn } or
                                Move { Id: MoveId.Yawn } or
                                ActiveMove { Secondaries: not null })
                        {
                            battle.Add("-activate", target, "move: Electric Terrain");
                        }

                        return false;
                    }

                    return new VoidReturn();
                }),
                OnTryAddVolatile = new OnTryAddVolatileEventInfo((battle, status, target, _, _) =>
                {
                    if (!(target.IsGrounded() ?? false) || target.IsSemiInvulnerable())
                        return new VoidReturn();
                    if (status.Id == ConditionId.Yawn)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", target, "move: Electric Terrain");
                        }

                        return null;
                    }

                    return new VoidReturn();
                }),
                //OnBasePowerPriority = 6,
                OnBasePower = new OnBasePowerEventInfo((battle, _, attacker, _, move) =>
                    {
                        if (move.Type == MoveType.Electric &&
                            (attacker.IsGrounded() ?? false) &&
                            !attacker.IsSemiInvulnerable())
                        {
                            if (battle.DisplayUi)
                            {
                                battle.Debug("electric terrain boost");
                            }

                            return battle.ChainModify([5325, 4096]);
                        }

                        return new VoidReturn();
                    },
                    6),
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, effect) =>
                {
                    if (!battle.DisplayUi) return;

                    if (effect is Ability)
                    {
                        battle.Add("-fieldstart", "move: Electric Terrain", "[from] ability: " +
                            effect.Name, $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-fieldstart", "move: Electric Terrain");
                    }
                }),
                //OnFieldResidualOrder = 27,
                //OnFieldResidualSubOrder = 7,
                OnSideResidual = new OnSideResidualEventInfo((_, _, _, _) => { })
                {
                    Order = 27,
                    SubOrder = 7,
                },
                OnFieldEnd = new OnFieldEndEventInfo((battle, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-fieldend", "move: Electric Terrain");
                    }
                }),
            },
            [ConditionId.Flinch] = new()
            {
                Id = ConditionId.Flinch,
                Name = "Flinch",
                EffectType = EffectType.Condition,
                Duration = 1,
                //OnBeforeMovePriority = 8,
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, _) =>
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("cant", pokemon, "flinch");
                        }

                        battle.RunEvent(EventId.Flinch, pokemon);
                        return false;
                    },
                    8),
            },
            [ConditionId.Freeze] = new()
            {
                Id = ConditionId.Freeze,
                Name = "Freeze",
                EffectType = EffectType.Status,
                ImmuneTypes = [PokemonType.Ice],
                OnStart = new OnStartEventInfo((battle, target, source, sourceEffect) =>
                {
                    if (battle.DisplayUi)
                    {
                        switch (sourceEffect)
                        {
                            case Ability:
                                battle.Add("-status", target, "frz", "[from] ability: " +
                                    sourceEffect.Name, $"[of] {source}");
                                break;
                            default:
                                battle.Add("-status", target, "frz");
                                break;
                        }
                    }

                    if (target.Species.Id == SpecieId.ShayminSky &&
                        target.BaseSpecies.BaseSpecies == SpecieId.Shaymin)
                    {
                        target.FormeChange(SpecieId.Shaymin, battle.Effect, true);
                    }

                    return new VoidReturn();
                }),
                //OnBeforeMovePriority = 10,
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, move) =>
                    {
                        if ((move.Flags.Defrost ?? false) &&
                            !(move.Id == MoveId.BurnUp && !pokemon.HasType(PokemonType.Fire)))
                        {
                            return new VoidReturn();
                        }

                        if (battle.RandomChance(1, 5))
                        {
                            pokemon.CureStatus();
                            return new VoidReturn();
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Add("cant", pokemon, "frz");
                        }

                        return false;
                    },
                    10),
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, pokemon, _) =>
                {
                    if (!(move.Flags.Defrost ?? false)) return;
                    if (battle.DisplayUi)
                    {
                        battle.Add("-curestatus", pokemon, "frz", $"[from] move: {move}");
                    }

                    pokemon.ClearStatus();
                }),
                OnAfterMoveSecondary = new OnAfterMoveSecondaryEventInfo((_, target, _, move) =>
                {
                    if (move.ThawsTarget ?? false)
                    {
                        target.CureStatus();
                    }
                }),
                OnDamagingHit = new OnDamagingHitEventInfo((_, _, target, _, move) =>
                {
                    if (move.Type == MoveType.Fire && move.Category != MoveCategory.Status &&
                        move.Id != MoveId.PolarFlare)
                    {
                        target.CureStatus();
                    }
                }),
            },
        };
    }
}
