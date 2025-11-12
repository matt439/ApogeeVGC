using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyResidual event (pokemon/ally-specific).
/// Triggered for ally residual effects.
/// Signature: Action<Battle, PokemonSideUnion, Pokemon, IEffect>
/// </summary>
public sealed record OnAllyResidualEventInfo : EventHandlerInfo
{
    public OnAllyResidualEventInfo(
    Action<Battle, PokemonSideUnion, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Residual;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(PokemonSideUnion), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(void);
  }
}