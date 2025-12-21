using ApogeeVGC.Sim.Abilities;

namespace ApogeeVGC.Data.Abilities;

public partial record Abilities
{
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesMNO()
    {
        return new Dictionary<AbilityId, Ability>
        {
            [AbilityId.NaturalCure] = new()
            {
                Id = AbilityId.NaturalCure,
                Name = "Natural Cure",
                Rating = 2.5,
                Num = 30,
                // TODO: Implement OnSwitchOut and OnCheckShow
            },
        };
    }
}
