using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Choices;

public record PokemonSwitchRequestData
{
    //public required string Ident { get; init; }
    //public required string Details { get; init; }
    public ConditionId Condition { get; init; }
    public bool Active { get; init; }
    public required StatsTable Stats { get; init; }
    public required IReadOnlyList<MoveDto> Moves { get; init; }
    public required AbilityDto BaseAbility { get; init; }
    public required ItemDto Item { get; init; }
    public PokeballId Pokeball { get; init; }
    public required AbilityDto Ability { get; init; }
    public bool Commanding { get; init; }
    public bool Reviving { get; init; }
    public MoveType TeraType { get; init; }
    public bool Terastallized { get; init; }
}
