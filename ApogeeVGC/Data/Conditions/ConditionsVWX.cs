using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.FieldEventMethods;
using ApogeeVGC.Sim.Events.Handlers.SideEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsVwx()
    {
        return new Dictionary<ConditionId, Condition>
        {
            // ===== U CONDITIONS =====

            [ConditionId.Uproar] = new()
            {
                Id = ConditionId.Uproar,
                Name = "Uproar",
                EffectType = EffectType.Condition,
                Duration = 3,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "Uproar");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    // TODO: Check if pokemon has throatchop volatile - if so, remove uproar
                    // TODO: Check if last move was struggle - if so, don't lock
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "Uproar", "[upkeep]");
                    }
                }, 28, 1),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", pokemon, "Uproar");
                    }
                }),
                // TODO: OnLockMove = "uproar" - lock the pokemon into using Uproar
                // TODO: OnAnySetStatus - prevent sleep on all pokemon while Uproar is active
            },

            // ===== V CONDITIONS =====
            // No conditions needed for V moves

            // ===== W CONDITIONS =====

            [ConditionId.WaterPledge] = new()
            {
                Id = ConditionId.WaterPledge,
                Name = "Water Pledge",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.WaterPledge,
                Duration = 4,
                OnSideStart = new OnSideStartEventInfo((battle, targetSide, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", targetSide, "Water Pledge");
                    }
                }),
                OnSideResidual = new OnSideResidualEventInfo((_, _, _, _) => { })
                {
                    Order = 26,
                    SubOrder = 7,
                },
                OnSideEnd = new OnSideEndEventInfo((battle, targetSide) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sideend", targetSide, "Water Pledge");
                    }
                }),
                // TODO: OnModifyMove - double secondary effect chances for moves (except Secret Power)
                // Needs to skip Serene Grace + Flinch interaction
            },
            [ConditionId.WideGuard] = new()
            {
                Id = ConditionId.WideGuard,
                Name = "Wide Guard",
                EffectType = EffectType.Condition,
                Duration = 1,
                AssociatedMove = MoveId.WideGuard,
                // TODO: OnSideStart - display wide guard message
                // TODO: OnTryHitPriority = 4
                // TODO: OnTryHit - block spread moves (allAdjacent, allAdjacentFoes)
            },
            [ConditionId.WonderRoom] = new()
            {
                Id = ConditionId.WonderRoom,
                Name = "Wonder Room",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.WonderRoom,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((battle, _, source, _) =>
                {
                    if (source != null && source.HasAbility(AbilityId.Persistent))
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", source, "ability: Persistent", "[move] Wonder Room");
                        }
                        return 7;
                    }
                    return 5;
                }),
                // TODO: OnModifyMove - swap offensive stats for moves using def/spd
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        if (source != null && source.HasAbility(AbilityId.Persistent))
                        {
                            battle.Add("-fieldstart", "move: Wonder Room", $"[of] {source}", "[persistent]");
                        }
                        else
                        {
                            battle.Add("-fieldstart", "move: Wonder Room", $"[of] {source}");
                        }
                    }
                }),
                OnFieldRestart = new OnFieldRestartEventInfo((battle, _, _, _) =>
                {
                    battle.Field.RemovePseudoWeather(ConditionId.WonderRoom);
                }),
                // Swapping defenses partially implemented in sim/pokemon.js:Pokemon#calculateStat and Pokemon#getStat
                OnFieldResidual = new OnFieldResidualEventInfo((_, _, _, _) => { })
                {
                    Order = 27,
                    SubOrder = 5,
                },
                OnFieldEnd = new OnFieldEndEventInfo((battle, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-fieldend", "move: Wonder Room");
                    }
                }),
            },

            // ===== X CONDITIONS =====
            // No conditions needed for X moves
        };
    }
}
