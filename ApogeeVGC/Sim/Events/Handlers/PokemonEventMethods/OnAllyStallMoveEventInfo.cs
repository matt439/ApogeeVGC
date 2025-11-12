using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyStallMove event (pokemon/ally-specific).
/// Triggered for ally stall move.
/// Signature: Func<Battle, Pokemon, PokemonVoidUnion>
/// </summary>
public sealed record OnAllyStallMoveEventInfo : EventHandlerInfo
{
    public OnAllyStallMoveEventInfo(
    Func<Battle, Pokemon, PokemonVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.StallMove;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(PokemonVoidUnion);
  }
}