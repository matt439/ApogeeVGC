using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;

namespace ApogeeVGC.Data.Abilities;

public partial record Abilities
{
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesDef()
    {
        return new Dictionary<AbilityId, Ability>
        {
            [AbilityId.FlameBody] = new()
            {
                Id = AbilityId.FlameBody,
                Name = "Flame Body",
                Num = 49,
                Rating = 2.0,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, source, move) =>
                {
                    if (!battle.CheckMoveMakesContact(move, source, target)) return;

                    if (battle.RandomChance(3, 10))
                    {
                        source.TrySetStatus(ConditionId.Burn, target);
                    }
                }),
            },
        };
    }
}
