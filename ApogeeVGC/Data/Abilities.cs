using System.Collections.ObjectModel;
using ApogeeVGC.Sim;

namespace ApogeeVGC.Data;

public record Abilities
{
    public IReadOnlyDictionary<AbilityId, Ability> AbilitiesData { get; }

    public Abilities()
    {
        AbilitiesData = new ReadOnlyDictionary<AbilityId, Ability>(_abilities);
    }

    private readonly Dictionary<AbilityId, Ability> _abilities = new()
    {
        [AbilityId.AsOneGlastrier] = new Ability
        {
            Id = AbilityId.AsOneGlastrier,
            Name = "As One (Glastrier)",
            Num = 266,
            Rating = 3.5,
            OnSwitchInPriority = 1,
        },
        [AbilityId.HadronEngine] = new Ability
        {
            Id = AbilityId.HadronEngine,
            Name = "Hadron Engine",
            Num = 289,
            Rating = 4.5,
            OnSwitchInPriority = 1,
        },
        [AbilityId.Guts] = new Ability
        {
            Id = AbilityId.Guts,
            Name = "Guts",
            Num = 62,
            Rating = 3.5,
        },
        [AbilityId.FlameBody] = new Ability
        {
            Id = AbilityId.FlameBody,
            Name = "Flame Body",
            Num = 49,
            Rating = 2.0,
        },
        [AbilityId.Prankster] = new Ability
        {
            Id = AbilityId.Prankster,
            Name = "Prankster",
            Num = 158,
            Rating = 4.0,
        },
        [AbilityId.QuarkDrive] = new Ability
        {
            Id = AbilityId.QuarkDrive,
            Name = "Quark Drive",
            Num = 282,
            Rating = 3.0,
            OnSwitchInPriority = -2,
        },
    };
}