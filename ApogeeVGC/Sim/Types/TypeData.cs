using ApogeeVGC.Data;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Types;

public record TypeData
{
    public required IReadOnlyDictionary<MoveType, TypeEffectiveness> DamageTaken { get; init; }
    // public required IReadOnlyDictionary<SpecialImmunityId, TypeEffectiveness> SpecialDamageTaken { get; init; }
    public IReadOnlyDictionary<StatId, int>? HpDvs { get; init; }
    public IReadOnlyDictionary<StatId, int>? HpIvs { get; init; }
    public Nonstandard? IsNonstandard { get; init; }
}