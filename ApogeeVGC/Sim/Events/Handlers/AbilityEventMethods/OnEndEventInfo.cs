using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;

/// <summary>
/// Event handler info for OnEnd event (ability-specific).
/// Triggered when an ability effect ends.
/// Signature: Action&lt;Battle, PokemonSideFieldUnion&gt;
/// </summary>
public sealed record OnEndEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnEnd event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
 public OnEndEventInfo(
 Action<Battle, PokemonSideFieldUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.End;
        Prefix = EventPrefix.None;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(PokemonSideFieldUnion)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
