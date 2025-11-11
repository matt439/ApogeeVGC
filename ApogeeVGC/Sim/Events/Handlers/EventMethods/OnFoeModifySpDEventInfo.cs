using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeModifySpD event.
/// Modifies the Special Defense stat against foe attacks.
/// Signature: (Battle battle, int relayVar, Pokemon target, Pokemon source, ActiveMove move) => DoubleVoidUnion
/// </summary>
public sealed record OnFoeModifySpDEventInfo : EventHandlerInfo
{
    /// <summary>
/// Creates a new OnFoeModifySpD event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFoeModifySpDEventInfo(
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifySpD;
        Prefix = EventPrefix.Foe;
      Handler = handler;
     Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
   [
        typeof(Battle),
     typeof(int),
            typeof(Pokemon),
            typeof(Pokemon),
            typeof(ActiveMove),
        ];
        ExpectedReturnType = typeof(DoubleVoidUnion);
    }
}
