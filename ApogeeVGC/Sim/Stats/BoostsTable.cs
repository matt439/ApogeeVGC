namespace ApogeeVGC.Sim.Stats;

public record BoostsTable
{ 
    // Boosts are not restricted to -6 to +6 like StatModifiers
    // For example bellydrum can boost Atk by +12
    // The restriction is applied by StatModifiers
    public Dictionary<StatModifierId, int> Boosts { get; init; } = [];
}