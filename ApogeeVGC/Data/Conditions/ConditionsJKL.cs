using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.SideEventMethods;
using ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsJKL()
    {
        return new Dictionary<ConditionId, Condition>
        {
            [ConditionId.LeechSeed] = new()
            {
                Id = ConditionId.LeechSeed,
                Name = "Leech Seed",
                EffectType = EffectType.Condition,
                ImmuneTypes = [PokemonType.Grass],
                AssociatedMove = MoveId.LeechSeed,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, "move: Leech Seed");
                    }

                    return new VoidReturn();
                }),
                //OnResidualOrder = 8,
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                    {
                        Pokemon? target =
                            battle.GetAtSlot(pokemon.Volatiles[ConditionId.LeechSeed].SourceSlot);
                        if (target is null || target.Fainted || target.Hp <= 0)
                        {
                            if (battle.DisplayUi)
                            {
                                battle.Debug("Nothing to leech into");
                            }

                            return;
                        }

                        IntFalseUndefinedUnion damage =
                            battle.Damage(pokemon.BaseMaxHp / 8, pokemon, target);

                        if (damage is IntIntFalseUndefined d)
                        {
                            battle.Heal(d.Value, target, pokemon);
                        }
                    },
                    8),
            },
            [ConditionId.LightScreen] = new()
            {
                Id = ConditionId.LightScreen,
                Name = "Light Screen",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.LightScreen,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, _, source, _) =>
                    source.HasItem(ItemId.LightClay) ? 8 : 5),
                OnAnyModifyDamage =
                    new OnAnyModifyDamageEventInfo((battle, _, source, target, move) =>
                    {
                        if (target != source &&
                            battle.EffectState.Target is SideEffectStateTarget side &&
                            side.Side.HasAlly(target) &&
                            battle.GetCategory(move) == MoveCategory.Special)
                        {
                            if (!target.GetMoveHitData(move).Crit && !(move.Infiltrates ?? false))
                            {
                                if (battle.DisplayUi)
                                {
                                    battle.Debug("Light Screen weaken");
                                }

                                return battle.ActivePerHalf > 1
                                    ? battle.ChainModify([2732, 4096])
                                    : battle.ChainModify(0.5);
                            }
                        }

                        return new VoidReturn();
                    }),
                OnSideStart = new OnSideStartEventInfo((battle, side, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", side, "move: Light Screen");
                    }
                }),
                //OnSideResidualOrder = 26,
                //OnSideResidualSubOrder = 2,
                OnSideResidual = new OnSideResidualEventInfo((_, _, _, _) => { })
                {
                    Order = 26,
                    SubOrder = 2,
                },
                OnSideEnd = new OnSideEndEventInfo((battle, side) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sideend", side, "move: Light Screen");
                    }
                }),
            },
        };
    }
}
