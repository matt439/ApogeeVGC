using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Pokemon | Pokemon[] | Side | Battle | PokemonSideBattleUnion? | PokemonSideBattleUnion|  Field | Pokemon?
/// </summary>
public abstract record RunEventTarget
{
    public static implicit operator RunEventTarget(Pokemon pokemon) =>
    new PokemonRunEventTarget(pokemon);
    public static RunEventTarget? FromNullablePokemon(Pokemon? pokemon)
    {
      return pokemon is null ? null : new PokemonRunEventTarget(pokemon);
    }

    public static implicit operator RunEventTarget(Pokemon[] pokemonList) =>
  new PokemonArrayRunEventTarget(pokemonList);

    public static implicit operator RunEventTarget(Side side) => new SideRunEventTarget(side);
    public static RunEventTarget FromBattle(Battle battle) => new BattleRunEventTarget(battle);
    public static RunEventTarget? FromNullablePokemonSideBattleUnion(PokemonSideBattleUnion? target)
    {
   return target switch
     {
     null => null,
     PokemonSideBattlePokemon pokemon => new PokemonRunEventTarget(pokemon.Pokemon),
     PokemonSideBattleNullablePokemon nullablePokemon => nullablePokemon.Pokemon is null 
         ? null 
         : new PokemonRunEventTarget(nullablePokemon.Pokemon),
 PokemonSideBattleSide side => new SideRunEventTarget(side.Side),
         PokemonSideBattleBattle battle => new BattleRunEventTarget(battle.Battle),
  _ => throw new InvalidOperationException("Cannot convert to RunEventTarget"),
    };
    }
    public static RunEventTarget FromPokemonSideBattleUnion(PokemonSideBattleUnion target)
    {
     return target switch
    {
     PokemonSideBattlePokemon pokemon => new PokemonRunEventTarget(pokemon.Pokemon),
       PokemonSideBattleSide side => new SideRunEventTarget(side.Side),
  PokemonSideBattleBattle battle => new BattleRunEventTarget(battle.Battle),
    _ => throw new InvalidOperationException("Cannot convert to RunEventTarget"),
   };
    }
    public static implicit operator RunEventTarget(Field field) => new FieldRunEventTarget(field);
}

public record PokemonRunEventTarget(Pokemon Pokemon) : RunEventTarget;
public record PokemonArrayRunEventTarget(Pokemon[] PokemonList) : RunEventTarget;
public record SideRunEventTarget(Side Side) : RunEventTarget;
public record BattleRunEventTarget(Battle Battle) : RunEventTarget;
public record FieldRunEventTarget(Field Field) : RunEventTarget;
