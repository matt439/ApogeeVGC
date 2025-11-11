using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnDeductPp event.
/// Triggered when PP is deducted from a move.
/// Signature: (Battle battle, Pokemon target, Pokemon source) => IntVoidUnion
/// </summary>
public sealed record OnDeductPpEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnDeductPp event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnDeductPpEventInfo(
     Func<Battle, Pokemon, Pokemon, int> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.DeductPp;
        Handler = handler;
     Priority = priority;
     UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
            typeof(Battle),
            typeof(Pokemon),
   typeof(Pokemon),
        ];
   ExpectedReturnType = typeof(int);
    }
}
