using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Pokemon | Side | Field | Battle
/// </summary>
public abstract record PokemonSideFieldBattleUnion
{
    public static implicit operator PokemonSideFieldBattleUnion(Pokemon pokemon) =>
        new PokemonSideFieldBattlePokemon(pokemon);
    public static implicit operator PokemonSideFieldBattleUnion(Side side) =>
  new PokemonSideFieldBattleSide(side);
    public static implicit operator PokemonSideFieldBattleUnion(Field field) =>
        new PokemonSideFieldBattleField(field);
    public static PokemonSideFieldBattleUnion FromBattle(Battle battle) =>
    new PokemonSideFieldBattleBattle(battle);
}

public record PokemonSideFieldBattlePokemon(Pokemon Pokemon) : PokemonSideFieldBattleUnion;
public record PokemonSideFieldBattleSide(Side Side) : PokemonSideFieldBattleUnion;
public record PokemonSideFieldBattleField(Field Field) : PokemonSideFieldBattleUnion;
public record PokemonSideFieldBattleBattle(Battle Battle) : PokemonSideFieldBattleUnion;
