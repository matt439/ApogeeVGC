using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.FieldEventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsGhi()
    {
        return new Dictionary<ConditionId, Condition>
        {
            [ConditionId.Gem] = new()
            {
                Id = ConditionId.Gem,
                Name = "Gem",
                EffectType = EffectType.Condition,
                Duration = 1,
                AffectsFainted = true,
                //OnBasePowerPriority = 14,
                OnBasePower = new OnBasePowerEventInfo((battle, _, _, _, _) =>
                    {
                        battle.Debug("Gem Boost");
                        return battle.ChainModify([5325, 4096]);
                    },
                    14),
            },
            [ConditionId.Gravity] = new()
            {
                Id = ConditionId.Gravity,
                Name = "Gravity",
                EffectType = EffectType.Condition,
            },
            [ConditionId.Hail] = new()
            {
                Id = ConditionId.Hail,
                Name = "Hail",
                EffectType = EffectType.Weather,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, source, _, _) =>
                    source.HasItem(ItemId.IcyRock) ? 8 : 5),
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, effect) =>
                {
                    if (!battle.DisplayUi) return;

                    if (effect is Ability)
                    {
                        if (battle.Gen <= 5) battle.EffectState.Duration = 0;
                        battle.Add("-weather", "Hail", "[from] ability: " + effect.Name,
                            $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-weather", "Hail");
                    }
                }),
                //OnFieldResidualOrder = 1,
                OnFieldResidual = new OnFieldResidualEventInfo((battle, _, _, _) =>
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-weather", "Hail", "[upkeep]");
                        }

                        if (battle.Field.IsWeather(ConditionId.Hail))
                        {
                            battle.EachEvent(EventId.Weather);
                        }
                    },
                    1),
                OnWeather = new OnWeatherEventInfo((battle, target, _, _) =>
                {
                    battle.Damage(target.BaseMaxHp / 16);
                }),
                OnFieldEnd = new OnFieldEndEventInfo((battle, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-weather", "none");
                    }
                }),
            },
            [ConditionId.HealReplacement] = new()
            {
                // This is a slot condition
                Id = ConditionId.HealReplacement,
                Name = "HealReplacement",
                EffectType = EffectType.Condition,
                OnStart = new OnStartEventInfo((battle, _, source, sourceEffect) =>
                {
                    battle.EffectState.SourceEffect = sourceEffect;
                    battle.Add("-activate", source, "healreplacement");
                    return new VoidReturn();
                }),
                OnSwitchIn = new OnSwitchInEventInfo((battle, target) =>
                {
                    if (!target.Fainted)
                    {
                        target.Heal(target.MaxHp);
                        if (battle.DisplayUi)
                        {
                            battle.Add("-heal", target, target.GetHealth,
                                "[from] move: " + battle.EffectState.SourceEffect?.Name);
                        }

                        target.Side.RemoveSlotCondition(target,
                            _library.Conditions[ConditionId.HealReplacement]);
                    }
                }),
            },
        };
    }
}