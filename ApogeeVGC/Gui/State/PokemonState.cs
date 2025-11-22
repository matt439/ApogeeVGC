using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.Gui.State;

/// <summary>
/// Represents the GUI's view of a Pokemon's state
/// This is updated incrementally based on messages
/// </summary>
public class PokemonState
{
    // Identity
    public required string Name { get; set; }
    public required SpecieId Species { get; set; }
    public required GenderId Gender { get; set; }
    public required int Level { get; set; }
    
    // Combat state
    public required int Hp { get; set; }
    public required int MaxHp { get; set; }
    public required ConditionId Status { get; set; }
    
    // Position
    public required int Slot { get; set; }
    public required SideId Side { get; set; }
    
    // Status flags
    public bool IsFainted { get; set; }
    public bool IsActive { get; set; }
    
    // Item
    public ItemId Item { get; set; }
    
    // For rendering
    public int Position { get; set; } // 0 or 1 for doubles
    
    /// <summary>
    /// Create from perspective data (initial state)
    /// </summary>
    public static PokemonState FromPerspective(PokemonPerspective perspective, int slot, SideId side)
    {
        return new PokemonState
        {
            Name = perspective.Name,
            Species = perspective.Species,
            Gender = perspective.Gender,
            Level = perspective.Level,
            Hp = perspective.Hp,
            MaxHp = perspective.MaxHp,
            Status = perspective.Status,
            Slot = slot,
            Side = side,
            IsFainted = perspective.Hp <= 0,
            IsActive = true,
            Item = perspective.Item,
            Position = perspective.Position
        };
    }
}
