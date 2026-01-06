using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.PokemonClasses;

/// <summary>
/// Represents a Pokemon with full information visible.
/// Used for both player and opponent Pokemon in full observability mode.
/// </summary>
public record PokemonPerspective
{
    // Basic Info
    public required string Name { get; init; }
    public required SpecieId Species { get; init; }
    public required int Level { get; init; }
    public required GenderId Gender { get; init; }
    public required bool Shiny { get; init; }
    
    // Battle Status
    public required int Hp { get; init; }
    public required int MaxHp { get; init; }
    public required bool Fainted { get; init; }
    public required ConditionId Status { get; init; }
    
    // Moves
    public required IReadOnlyList<MoveSlot> MoveSlots { get; init; }
    
    // Stats and Boosts
    public required BoostsTable Boosts { get; init; }
    public required StatsExceptHpTable StoredStats { get; init; }
    
    // Ability and Item
    public required AbilityId Ability { get; init; }
    public required ItemId Item { get; init; }

    // Types
    public required IReadOnlyList<PokemonType> Types { get; init; }
    public required MoveType? Terastallized { get; init; }
    public required MoveType TeraType { get; init; }
    public MoveTypeFalseUnion? CanTerastallize { get; init; }
    
    // Volatiles (temporary battle conditions like confusion, substitute, etc.)
    public required IReadOnlyList<ConditionId> Volatiles { get; init; }
  
    // Volatiles with duration tracking (includes conditions like Stall, Protect, etc.)
    // Key: ConditionId, Value: Duration remaining (null if no duration limit)
    public required IReadOnlyDictionary<ConditionId, int?> VolatilesWithDuration { get; init; }
    
    // Position
    public required int Position { get; init; }
    public required bool IsActive { get; init; }
}