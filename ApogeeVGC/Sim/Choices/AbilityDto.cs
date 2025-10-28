using ApogeeVGC.Sim.Abilities;

namespace ApogeeVGC.Sim.Choices;

/// <summary>
/// Data Transfer Object for Ability - contains only serializable data needed for requests.
/// </summary>
public record AbilityDto
{
    public required AbilityId Id { get; init; }
    public required string Name { get; init; }
    public int Num { get; init; }

    public static AbilityDto FromAbility(Ability ability)
    {
        return new AbilityDto
        {
          Id = ability.Id,
Name = ability.Name,
    Num = ability.Num,
      };
    }
}
