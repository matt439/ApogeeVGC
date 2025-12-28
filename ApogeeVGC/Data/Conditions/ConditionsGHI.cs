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
            [ConditionId.GastroAcid] = new()
            {
                Id = ConditionId.GastroAcid,
                Name = "Gastro Acid",
                EffectType = EffectType.Condition,
                // TODO: onStart - end the pokemon's ability
                // TODO: onCopy - remove if pokemon has cantsuppress ability flag
                // Ability suppression should be implemented in Pokemon.IgnoringAbility()
            },
            [ConditionId.IceBall] = new()
            {
                Id = ConditionId.IceBall,
                Name = "Ice Ball",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.IceBall,
                // Ice Ball uses LockedMove for the locking behavior and RolloutStorage for damage scaling
                // This is just a marker condition
            },
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
            [ConditionId.GlaiveRush] = new()
            {
                Id = ConditionId.GlaiveRush,
                Name = "Glaive Rush",
                EffectType = EffectType.Condition,
                NoCopy = true,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singlemove", pokemon, "Glaive Rush", "[silent]");
                    }

                    return new VoidReturn();
                }),
                // TODO: onAccuracy - always hit (return true)
                // TODO: onSourceModifyDamage - double damage (chainModify 2)
                // TODO: onBeforeMovePriority 100, onBeforeMove - remove this condition before attack
            },
            [ConditionId.GrassPledge] = new()
            {
                Id = ConditionId.GrassPledge,
                Name = "Grass Pledge",
                EffectType = EffectType.Condition,
                Duration = 4,
                // TODO: onSideStart - display message
                // TODO: onModifySpe - quarter speed (chainModify 0.25)
                // TODO: onSideResidualOrder 26, onSideResidualSubOrder 9
                // TODO: onSideEnd - display message
            },
            [ConditionId.Gravity] = new()
            {
                Id = ConditionId.Gravity,
                Name = "Gravity",
                EffectType = EffectType.Condition,
                Duration = 5,
                // TODO: durationCallback - 7 if source has Persistent ability
                // TODO: onFieldStart - remove Bounce, Fly, Sky Drop, Magnet Rise, Telekinesis
                // TODO: onModifyAccuracy - increase accuracy (chainModify [6840, 4096])
                // TODO: onDisableMove - disable moves with gravity flag
                // TODO: onBeforeMovePriority 6, onBeforeMove - prevent gravity-affected moves
                // TODO: onModifyMove - prevent gravity-affected moves
                // TODO: onFieldResidualOrder 27, onFieldResidualSubOrder 2
                // TODO: onFieldEnd - display message
            },
            [ConditionId.Grudge] = new()
            {
                Id = ConditionId.Grudge,
                Name = "Grudge",
                EffectType = EffectType.Condition,
                // TODO: onStart - display singlemove message
                // TODO: onFaint - if fainted by a move, set that move's PP to 0 for the attacker
                // TODO: onBeforeMovePriority 100, onBeforeMove - remove this condition before attack
            },
            [ConditionId.GuardSplit] = new()
            {
                Id = ConditionId.GuardSplit,
                Name = "Guard Split",
                EffectType = EffectType.Condition,
                // This is handled in the move's onHit, not as a persistent condition
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