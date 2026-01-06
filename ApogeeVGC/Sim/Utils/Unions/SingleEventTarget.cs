using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Pokemon | Side | Field | Battle | Pokemon?
/// </summary>
public abstract record SingleEventTarget
{
public static implicit operator SingleEventTarget(Pokemon pokemon) =>
   new PokemonSingleEventTarget(pokemon);

  public static implicit operator SingleEventTarget(Side side) => new SideSingleEventTarget(side);
    public static implicit operator SingleEventTarget(Field field) => new FieldSingleEventTarget(field);

    public static SingleEventTarget FromBattle(Battle battle) => new BattleSingleEventTarget(battle);
    public static SingleEventTarget? FromNullablePokemon(Pokemon? pokemon)
{
  return pokemon is null ? null : new PokemonSingleEventTarget(pokemon);
    }
}

public record PokemonSingleEventTarget(Pokemon Pokemon) : SingleEventTarget;
public record SideSingleEventTarget(Side Side) : SingleEventTarget;
public record FieldSingleEventTarget(Field Field) : SingleEventTarget;
public record BattleSingleEventTarget(Battle Battle) : SingleEventTarget;
