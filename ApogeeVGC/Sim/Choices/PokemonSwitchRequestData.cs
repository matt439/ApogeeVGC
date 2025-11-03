using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Choices;

public record PokemonSwitchRequestData
{
    //public required string Ident { get; init; }
    //public required string Details { get; init; }
    public required string Condition { get; init; }
    public bool Active { get; init; }
    public required StatsTable Stats { get; init; }
    public required IReadOnlyList<Move> Moves { get; init; }
    public IReadOnlyList<string> MoveNames => Moves.Select(m => m.Name).ToList();
    public required Ability BaseAbility { get; init; }
    public string BaseAbilityName => BaseAbility.Name;
    public required Item Item { get; init; }
    public string ItemName => Item.Name;
    public PokeballId Pokeball { get; init; }
    public required Ability Ability { get; init; }
    public string AbilityName => Ability.Name;
    public bool Commanding { get; init; }
    public bool Reviving { get; init; }
    public MoveType TeraType { get; init; }
    public bool Terastallized { get; init; }
}
