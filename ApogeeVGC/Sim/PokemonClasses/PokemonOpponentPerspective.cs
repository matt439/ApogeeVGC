using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.PokemonClasses;

/// <summary>
/// Represents the opponent's perspective of enemy Pokemon with obscured information.
/// HP is shown as percentage, and certain details may be hidden until revealed in battle.
/// </summary>
public record PokemonOpponentPerspective
{
    // Basic Info
    public required string Name { get; init; }
    public required SpecieId Species { get; init; }
    public required int Level { get; init; }
    public required GenderId Gender { get; init; }
    public required bool Shiny { get; init; }
    
    // Battle Status - HP as percentage for opponent
    public required double HpPercentage { get; init; }
    public HpColor? HpColor { get; init; }
    public required bool Fainted { get; init; }
    public required ConditionId Status { get; init; }
    
    // Revealed information (null if not yet revealed)
    public AbilityId? RevealedAbility { get; init; }
    public ItemId? RevealedItem { get; init; }
    public IReadOnlyList<MoveId>? RevealedMoves { get; init; }
    
    // Types (visible when Pokemon is seen)
    public required IReadOnlyList<PokemonType> Types { get; init; }
    public required MoveType? Terastallized { get; init; }
    
    // Volatiles (visible status conditions)
    public required IReadOnlyList<ConditionId> Volatiles { get; init; }
    
    // Position
    public required int Position { get; init; }
    public required bool IsActive { get; init; }
}