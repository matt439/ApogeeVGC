using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Abilities;

public partial record Abilities
{
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesStu()
    {
        return new Dictionary<AbilityId, Ability>
        {
            [AbilityId.Unnerve] = new()
            {
                Id = AbilityId.Unnerve,
                Name = "Unnerve",
                Rating = 1.0,
                Num = 127,
                OnSwitchIn = new OnSwitchInEventInfo((_, _) => { }, 1),
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    if (battle.EffectState.Unnerved ?? false) return;
                    battle.Add("-ability", pokemon, "Unnerve");
                    battle.EffectState.Unnerved = true;
                }),
                OnEnd = new OnEndEventInfo((battle, _) => { battle.EffectState.Unnerved = false; }),
                OnFoeTryEatItem = new OnFoeTryEatItemEventInfo(
                    OnTryEatItem.FromFunc((battle, _, _) =>
                        !(battle.EffectState.Unnerved ?? false))),
            },
        };
    }
}
