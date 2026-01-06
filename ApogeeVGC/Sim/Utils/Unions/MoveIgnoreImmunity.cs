using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// boolean | { [PokemonType: string]: boolean }
/// </summary>
public abstract record MoveIgnoreImmunity
{
    public static implicit operator MoveIgnoreImmunity(bool value) =>
     new BoolMoveDataIgnoreImmunity(value);

    public static implicit operator MoveIgnoreImmunity(Dictionary<PokemonType, bool> typeImmunities) =>
       new TypeMoveDataIgnoreImmunity(typeImmunities);
}

public record BoolMoveDataIgnoreImmunity(bool Value) : MoveIgnoreImmunity;
public record TypeMoveDataIgnoreImmunity(Dictionary<PokemonType, bool> TypeImmunities) :
    MoveIgnoreImmunity;
