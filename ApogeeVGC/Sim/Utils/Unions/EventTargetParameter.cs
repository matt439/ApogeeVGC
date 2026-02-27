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
        if (target is null) return null;

        var t = target.Value;
        return t.Kind switch
        {
            SingleEventTargetKind.Pokemon when expectedType.IsAssignableFrom(typeof(Pokemon)) =>
                new PokemonEventTargetParameter(t.Pokemon),
            SingleEventTargetKind.Side when expectedType.IsAssignableFrom(typeof(Side)) =>
                new SideEventTargetParameter(t.Side),
            SingleEventTargetKind.Field when expectedType.IsAssignableFrom(typeof(Field)) =>
                new FieldEventTargetParameter(t.Field),
            SingleEventTargetKind.Battle when expectedType.IsAssignableFrom(typeof(Battle)) =>
                new BattleEventTargetParameter(t.Battle),
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
