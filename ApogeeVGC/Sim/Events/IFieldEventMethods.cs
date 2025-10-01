using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events;

public interface IFieldEventMethods : IEventMethods
{
    Action<BattleContext, Field, Pokemon, IEffect>? OnFieldStart { get; }
    Action<BattleContext, Field, Pokemon, IEffect>? OnFieldRestart { get; }
    Action<BattleContext, Field, Pokemon, IEffect>? OnFieldResidual { get; }
    Action<BattleContext, Field>? OnFieldEnd { get; }
    int? OnFieldResidualOrder { get; }
    int? OnFieldResidualPriority { get; }
    int? OnFieldResidualSubOrder { get; }
}