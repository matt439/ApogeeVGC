using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events;

public interface IFieldEventMethods : IEventMethods
{
    /// <summary>
    /// battle, target, source, sourceEffect
    /// </summary>
    Action<BattleContext, Field, Pokemon, IEffect>? OnFieldStart { get; }

    /// <summary>
    /// battle, target, source, sourceEffect
    /// </summary>
    Action<BattleContext, Field, Pokemon, IEffect>? OnFieldRestart { get; }

    /// <summary>
    /// battle, target, source, effect
    /// </summary>
    Action<BattleContext, Field, Pokemon, IEffect>? OnFieldResidual { get; }

    /// <summary>
    /// battle, target
    /// </summary>
    Action<BattleContext, Field>? OnFieldEnd { get; }
    int? OnFieldResidualOrder { get; }
    int? OnFieldResidualPriority { get; }
    int? OnFieldResidualSubOrder { get; }
}