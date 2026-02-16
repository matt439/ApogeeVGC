using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.SideEventMethods;

/// <summary>
/// Event handler info for OnSideStart event (side-specific).
/// Triggered when a side condition starts.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Side, Pokemon, IEffect) => void
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnSideStartEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSideStart event handler using the legacy strongly-typed pattern.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnSideStartEventInfo(
        Action<Battle, Side, Pokemon, IEffect> handler,
        int? priority = null,
   bool usesSpeed = true)
    {
        Id = EventId.SideStart;
        Prefix = EventPrefix.None;
      Handler = handler;
     Priority = priority;
        UsesSpeed = usesSpeed;
 ExpectedParameterTypes = [typeof(Battle), typeof(Side), typeof(Pokemon), typeof(IEffect)];
 ExpectedReturnType = typeof(void);
        
        // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false;

 // Validate configuration
        ValidateConfiguration();
    }
    
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, TargetSide, TargetPokemon, SourceEffect
  /// </summary>
    public OnSideStartEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.SideStart;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnSideStartEventInfo Create(
   Action<Battle, Side, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
      return new OnSideStartEventInfo(
        context =>
    {
      handler(
        context.Battle,
   context.GetTargetSide(),
       context.GetTargetOrSourcePokemon(),
      context.GetSourceEffect<IEffect>()
);
      return null;
            },
          priority,
            usesSpeed
     );
    }
}
