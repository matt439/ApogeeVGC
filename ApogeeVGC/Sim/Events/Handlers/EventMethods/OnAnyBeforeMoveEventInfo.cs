using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyBeforeMove event.
/// Triggered before any Pokemon uses a move.
/// Signature: (Battle battle, Pokemon source, Pokemon target, ActiveMove move) => BoolVoidUnion
/// </summary>
public sealed record OnAnyBeforeMoveEventInfo : EventHandlerInfo
{
  public OnAnyBeforeMoveEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion> handler,
      int? priority = null,
        bool usesSpeed = true)
  {
        Id = EventId.BeforeMove;
  Prefix = EventPrefix.Any;
     Handler = handler;
  Priority = priority;
      UsesSpeed = usesSpeed;
   ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
   ExpectedReturnType = typeof(BoolVoidUnion);
    }
}
