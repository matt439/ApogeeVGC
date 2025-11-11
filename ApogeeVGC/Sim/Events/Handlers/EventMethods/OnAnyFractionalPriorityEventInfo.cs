using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyFractionalPriority event.
/// Signature: Func<Battle, int, Pokemon, ActiveMove, double>
/// </summary>
public sealed record OnAnyFractionalPriorityEventInfo : EventHandlerInfo
{
 public OnAnyFractionalPriorityEventInfo(
      Func<Battle, int, Pokemon, ActiveMove, double> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.FractionalPriority;
   Prefix = EventPrefix.Any;
        Handler = handler;
    Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(double);
    }
}