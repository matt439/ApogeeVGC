using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAllyFractionalPriority event.
/// Signature: Func<Battle, int, Pokemon, ActiveMove, double>
/// </summary>
public sealed record OnAllyFractionalPriorityEventInfo : EventHandlerInfo
{
 public OnAllyFractionalPriorityEventInfo(
      Func<Battle, int, Pokemon, ActiveMove, double> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.FractionalPriority;
   Prefix = EventPrefix.Ally;
        Handler = handler;
    Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(double);
    }
}