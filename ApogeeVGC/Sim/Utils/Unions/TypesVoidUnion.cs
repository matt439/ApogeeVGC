using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// PokemonType[] | void
/// </summary>
public abstract record TypesVoidUnion
{
    public static implicit operator TypesVoidUnion(PokemonType[] types) => new TypesTypesVoidUnion(types);
  public static implicit operator TypesVoidUnion(VoidReturn value) => new VoidTypesVoidUnion(value);
    public static TypesVoidUnion FromVoid() => new VoidTypesVoidUnion(new VoidReturn());
}

public record TypesTypesVoidUnion(PokemonType[] Types) : TypesVoidUnion;
public record VoidTypesVoidUnion(VoidReturn Value) : TypesVoidUnion;
