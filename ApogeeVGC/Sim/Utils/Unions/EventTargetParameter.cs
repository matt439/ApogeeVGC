using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Pokemon | Side | Field | Battle
/// Represents the resolved target parameter type for event callbacks.
/// </summary>
public abstract record EventTargetParameter
{
    public static EventTargetParameter? FromSingleEventTarget(SingleEventTarget? target, Type expectedType)
    {
        if (target == null) return null;

     return target switch
        {
        PokemonSingleEventTarget p when expectedType.IsAssignableFrom(typeof(Pokemon)) =>
     new PokemonEventTargetParameter(p.Pokemon),
   SideSingleEventTarget s when expectedType.IsAssignableFrom(typeof(Side)) =>
         new SideEventTargetParameter(s.Side),
       FieldSingleEventTarget f when expectedType.IsAssignableFrom(typeof(Field)) =>
            new FieldEventTargetParameter(f.Field),
   BattleSingleEventTarget b when expectedType.IsAssignableFrom(typeof(Battle)) =>
        new BattleEventTargetParameter(b.Battle),
 _ => null,
        };
    }

    public object? ToObject()
    {
        return this switch
    {
     PokemonEventTargetParameter p => p.Pokemon,
      SideEventTargetParameter s => s.Side,
            FieldEventTargetParameter f => f.Field,
      BattleEventTargetParameter b => b.Battle,
  _ => null,
  };
    }
}

public record PokemonEventTargetParameter(Pokemon Pokemon) : EventTargetParameter;
public record SideEventTargetParameter(Side Side) : EventTargetParameter;
public record FieldEventTargetParameter(Field Field) : EventTargetParameter;
public record BattleEventTargetParameter(Battle Battle) : EventTargetParameter;
