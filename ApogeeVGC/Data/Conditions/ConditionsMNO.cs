using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.SideEventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsMno()
    {
        return new Dictionary<ConditionId, Condition>
        {
            [ConditionId.MagicRoom] = new()
            {
                Id = ConditionId.MagicRoom,
                Name = "Magic Room",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.MagicRoom,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, _, source, _) =>
                {
                    // TODO: Check for Persistent ability
                    return 5;
                }),
                // TODO: onFieldStart - trigger item End events for all active pokemon
                // TODO: onFieldRestart - remove Magic Room if used again
                // TODO: onFieldResidualOrder = 27
                // TODO: onFieldResidualSubOrder = 6
                // TODO: onFieldEnd - display end message
            },
            [ConditionId.MagmaStorm] = new()
            {
                Id = ConditionId.MagmaStorm,
                Name = "Magma Storm",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.MagmaStorm,
                // Uses PartiallyTrapped condition with MagmaStorm as source
            },
            [ConditionId.MagnetRise] = new()
            {
                Id = ConditionId.MagnetRise,
                Name = "Magnet Rise",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.MagnetRise,
                Duration = 5,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, "Magnet Rise");
                    }
                    return new VoidReturn();
                }),
                // TODO: onImmunity - if type is Ground, return false
                // TODO: onResidualOrder = 18
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", target, "Magnet Rise");
                    }
                }),
            },
            [ConditionId.MeanLook] = new()
            {
                Id = ConditionId.MeanLook,
                Name = "Mean Look",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.MeanLook,
                // Uses Trapped condition
            },
            [ConditionId.Mimic] = new()
            {
                Id = ConditionId.Mimic,
                Name = "Mimic",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Mimic,
                // Mimic doesn't create a persistent condition - it modifies move slots directly
            },
            [ConditionId.Minimize] = new()
            {
                Id = ConditionId.Minimize,
                Name = "Minimize",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Minimize,
                NoCopy = true,
                // TODO: onRestart - return null
                // TODO: onSourceModifyDamage - double damage from stomping moves
                // TODO: onAccuracy - guarantee hit from stomping moves
            },
            [ConditionId.MirrorCoat] = new()
            {
                Id = ConditionId.MirrorCoat,
                Name = "Mirror Coat",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.MirrorCoat,
                Duration = 1,
                NoCopy = true,
                // TODO: onStart - initialize slot and damage to track
                // TODO: onRedirectTargetPriority = -1
                // TODO: onRedirectTarget - redirect to pokemon that damaged source
                // TODO: onDamagingHit - track special damage and attacker slot
            },
            [ConditionId.Mist] = new()
            {
                Id = ConditionId.Mist,
                Name = "Mist",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Mist,
                Duration = 5,
                // TODO: onTryBoost - prevent negative stat changes from opponents
                OnSideStart = new OnSideStartEventInfo((battle, side, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", side, "Mist");
                    }
                }),
                //OnSideResidualOrder = 26,
                //OnSideResidualSubOrder = 4,
                OnSideResidual = new OnSideResidualEventInfo((_, _, _, _) => { })
                {
                    Order = 26,
                    SubOrder = 4,
                },
                OnSideEnd = new OnSideEndEventInfo((battle, side) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sideend", side, "Mist");
                    }
                }),
            },
            [ConditionId.MustRecharge] = new()
            {
                Id = ConditionId.MustRecharge,
                Name = "MustRecharge",
                EffectType = EffectType.Condition,
                Duration = 2,
                //OnBeforeMovePriority = 11,
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, _) =>
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("cant", pokemon, "recharge");
                        }

                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.MustRecharge]);
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.Truant]);
                        return null;
                    },
                    11),
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-mustrecharge", pokemon);
                    }

                    return new VoidReturn();
                }),
                OnLockMove = new OnLockMoveEventInfo(MoveId.Recharge),
            },
            [ConditionId.NoRetreat] = new()
            {
                Id = ConditionId.NoRetreat,
                Name = "No Retreat",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.NoRetreat,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "move: No Retreat");
                    }
                    return new VoidReturn();
                }),
                // TODO: onTrapPokemon - trap the pokemon
            },
            [ConditionId.None] = new()
            {
                Id = ConditionId.None,
                Name = "None",
                EffectType = EffectType.Condition,
            },
        };
    }
}
