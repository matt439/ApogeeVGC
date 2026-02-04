using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.SideEventMethods;

/// <summary>
/// Event handler info for OnSideRestart event (side-specific).
/// Triggered when a side condition restarts/reactivates.
/// Signature: Func&lt;Battle, Side, Pokemon, IEffect, VoidFalseUnion&gt;
/// </summary>
public sealed record OnSideRestartEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSideRestart event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnSideRestartEventInfo(
    Func<Battle, Side, Pokemon, IEffect, VoidFalseUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SideRestart;
        Prefix = EventPrefix.None;
      Handler = handler;
        Priority = priority;
   UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Side), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(VoidFalseUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
