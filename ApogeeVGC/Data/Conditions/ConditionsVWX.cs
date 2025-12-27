using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
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
            // No conditions needed for W moves
        };
    }
}
