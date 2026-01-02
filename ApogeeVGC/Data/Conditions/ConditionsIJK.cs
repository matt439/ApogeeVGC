using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsIjk()
    {
        return new Dictionary<ConditionId, Condition>
        {
            [ConditionId.Imprison] = new()
            {
                Id = ConditionId.Imprison,
                Name = "Imprison",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Imprison,
                NoCopy = true,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "Imprison");
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                // TODO: onFoeDisableMove - disable moves that the user also knows
                // TODO: onFoeBeforeMove - prevent using imprisoned moves
            },
            [ConditionId.Ingrain] = new()
            {
                Id = ConditionId.Ingrain,
                Name = "Ingrain",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Ingrain,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "Ingrain");
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                // TODO: OnTrapPokemon - set pokemon.TryTrap = true
                OnResidual = new OnResidualEventInfo(
                    (battle, pokemon, _, _) =>
                    {
                        battle.Heal(pokemon.BaseMaxHp / 16, pokemon, pokemon);
                    }, 7),
            },
        };
    }
}