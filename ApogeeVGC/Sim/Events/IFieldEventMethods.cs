using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events;

public interface IFieldEventMethods : IEventMethods
{
    /// <summary>
    /// battle, target, source, sourceEffect
    /// </summary>
    Action<IBattle, Field, Pokemon, IEffect>? OnFieldStart { get; }

    /// <summary>
    /// battle, target, source, sourceEffect
    /// </summary>
    Action<IBattle, Field, Pokemon, IEffect>? OnFieldRestart { get; }

    /// <summary>
    /// battle, target, source, effect
    /// </summary>
    Action<IBattle, Field, Pokemon, IEffect>? OnFieldResidual { get; }

    /// <summary>
    /// battle, target
    /// </summary>
    Action<IBattle, Field>? OnFieldEnd { get; }
    int? OnFieldResidualOrder { get; }
    int? OnFieldResidualPriority { get; }
    int? OnFieldResidualSubOrder { get; }
}