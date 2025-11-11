using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnStallMove event.
/// Triggered to check if a stall move (Protect, Detect, etc.) succeeds.
/// Signature: (Battle battle, Pokemon pokemon) => BoolVoidUnion
/// </summary>
public sealed record OnStallMoveEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnStallMove event handler.
    /// </summary>
/// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnStallMoveEventInfo(
        Func<Battle, Pokemon, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.StallMove;
 Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
   typeof(Battle),
         typeof(Pokemon),
        ];
        ExpectedReturnType = typeof(BoolVoidUnion);
    }
}
