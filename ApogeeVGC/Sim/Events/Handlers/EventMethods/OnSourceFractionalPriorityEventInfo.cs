using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceFractionalPriority event.
/// Signature: Func<Battle, int, Pokemon, ActiveMove, double>
/// </summary>
public sealed record OnSourceFractionalPriorityEventInfo : EventHandlerInfo
{
 public OnSourceFractionalPriorityEventInfo(
      Func<Battle, int, Pokemon, ActiveMove, double> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.FractionalPriority;
   Prefix = EventPrefix.Source;
        Handler = handler;
    Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(double);
    }
}