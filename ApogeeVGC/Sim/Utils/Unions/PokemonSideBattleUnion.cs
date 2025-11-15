using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Pokemon | Side | Battle | Pokemon?
/// </summary>
public abstract record PokemonSideBattleUnion
{
    public static implicit operator PokemonSideBattleUnion(Pokemon pokemon) =>
    new PokemonSideBattlePokemon(pokemon);
    public static implicit operator PokemonSideBattleUnion(Side side) => new PokemonSideBattleSide(side);
    public static PokemonSideBattleUnion FromBattle(Battle battle) => new PokemonSideBattleBattle(battle);
    public static PokemonSideBattleUnion? FromNullablePokemon(Pokemon? pokemon)
    {
        return pokemon is null ? null : new PokemonSideBattleNullablePokemon(pokemon);
    }

    public static PokemonSideBattleUnion? FromNullableSingleEventTarget(SingleEventTarget? target)
    {
        return target switch
        {
   null => null,
     PokemonSingleEventTarget pokemon => new PokemonSideBattlePokemon(pokemon.Pokemon),
      SideSingleEventTarget side => new PokemonSideBattleSide(side.Side),
    BattleSingleEventTarget battle => new PokemonSideBattleBattle(battle.Battle),
            _ => throw new InvalidOperationException("Cannot convert to PokemonSideBattleUnion"),
        };
    }
}

public record PokemonSideBattlePokemon(Pokemon Pokemon) : PokemonSideBattleUnion;
public record PokemonSideBattleSide(Side Side) : PokemonSideBattleUnion;
public record PokemonSideBattleBattle(Battle Battle) : PokemonSideBattleUnion;
public record PokemonSideBattleNullablePokemon(Pokemon? Pokemon) : PokemonSideBattleUnion;
