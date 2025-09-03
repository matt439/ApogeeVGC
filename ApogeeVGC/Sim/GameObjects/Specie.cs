using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.GameObjects;

public record SpeciesAbility
{
    public AbilityId Slot0 { get; init; }
    public AbilityId? Slot1 { get; init; }
    public AbilityId? Hidden { get; init; }
    public AbilityId? Special { get; init; }
}

public record Specie : IEffect
{
    public required SpecieId Id { get; init; }
    public EffectType EffectType => EffectType.Specie;
    public int Num { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? BaseSpecies { get; init; }
    public string? Forme { get; init; }
    public List<PokemonType> Types { get; init; } = [];
    public GenderId Gender { get; init; }
    public StatsTable BaseStats { get; init; } = new();
    public SpeciesAbility Abilities { get; init; } = new();
    public double Height { get; init; } // in meters
    public double Weight { get; init; } // in kilograms
    public string Color { get; init; } = string.Empty;
    public int Gen { get; init; }
    // Egg groups
    // Changes from
    // Gender ratio
    // Prevo
    // EvoType
    // EboCondition
    // Other forms
    // FormeOrder

    public Specie Copy()
    {
        return this with
        {
            Types = [..Types],
            BaseStats = BaseStats.Copy(),
            Abilities = new SpeciesAbility
            {
                Slot0 = Abilities.Slot0,
                Slot1 = Abilities.Slot1,
                Hidden = Abilities.Hidden,
                Special = Abilities.Special,
            },
        };
    }
}