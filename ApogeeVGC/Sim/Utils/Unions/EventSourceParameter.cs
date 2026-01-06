using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Pokemon | IEffect | PokemonType | bool (false)
/// Represents the resolved source parameter type for event callbacks.
/// </summary>
public abstract record EventSourceParameter
{
    public static EventSourceParameter? FromSingleEventSource(SingleEventSource? source, Type expectedType)
    {
        if (source == null) return null;

        return source switch
  {
        PokemonSingleEventSource p when expectedType.IsAssignableFrom(typeof(Pokemon)) =>
      new PokemonEventSourceParameter(p.Pokemon),
   EffectSingleEventSource e when expectedType.IsAssignableFrom(typeof(IEffect)) =>
     new EffectEventSourceParameter(e.Effect),
      PokemonTypeSingleEventSource t when expectedType.IsAssignableFrom(typeof(PokemonType)) =>
    new PokemonTypeEventSourceParameter(t.Type),
   FalseSingleEventSource when expectedType == typeof(bool) =>
       new BoolEventSourceParameter(false),
       _ => null,
 };
    }

    public object? ToObject()
 {
   return this switch
   {
  PokemonEventSourceParameter p => p.Pokemon,
    EffectEventSourceParameter e => e.Effect,
            PokemonTypeEventSourceParameter t => t.Type,
          BoolEventSourceParameter b => b.Value,
            _ => null,
        };
    }
}

public record PokemonEventSourceParameter(Pokemon Pokemon) : EventSourceParameter;
public record EffectEventSourceParameter(IEffect Effect) : EventSourceParameter;
public record PokemonTypeEventSourceParameter(PokemonType Type) : EventSourceParameter;
public record BoolEventSourceParameter(bool Value) : EventSourceParameter;
