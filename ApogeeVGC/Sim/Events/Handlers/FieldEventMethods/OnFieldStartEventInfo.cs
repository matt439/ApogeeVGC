using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.FieldEventMethods;

/// <summary>
/// Event handler info for OnFieldStart event (field-specific).
/// Triggered when a field condition starts.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Field, Pokemon, IEffect) => void
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnFieldStartEventInfo : EventHandlerInfo
{
    /// <summary>
  /// Creates a new OnFieldStart event handler using the legacy strongly-typed pattern.
    /// </summary>
 /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFieldStartEventInfo(
        Action<Battle, Field, Pokemon, IEffect> handler,
  int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.FieldStart;
        Prefix = EventPrefix.None;
   Handler = handler;
        Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Field), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(void);
        
        // Nullability: All parameters non-nullable by default (adjust as needed)
   ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
 ValidateConfiguration();
    }
    
    /// <summary>
  /// Creates event handler using context-based pattern.
    /// Context provides: Battle, Field, TargetPokemon, SourceEffect
    /// </summary>
    public OnFieldStartEventInfo(
        EventHandlerDelegate contextHandler,
  int? priority = null,
bool usesSpeed = true)
    {
      Id = EventId.FieldStart;
   Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
UsesSpeed = usesSpeed;
    }
    
    /// <summary>
  /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnFieldStartEventInfo Create(
        Action<Battle, Field, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   return new OnFieldStartEventInfo(
     context =>
          {
       handler(
   context.Battle,
  context.Battle.Field,
 context.GetTargetPokemon(),
     context.GetSourceEffect<IEffect>()
     );
      return null;
   },
        priority,
    usesSpeed
        );
    }
}
