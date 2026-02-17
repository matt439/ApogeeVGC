using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnTerrainChange event.
/// Triggered when terrain changes.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Pokemon, Pokemon, IEffect) => void
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnTerrainChangeEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnTerrainChange event handler using the legacy strongly-typed pattern.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnTerrainChangeEventInfo(
  Action<Battle, Pokemon, Pokemon, IEffect> handler,
  int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TerrainChange;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
        Priority = priority;
   UsesSpeed = usesSpeed;
 ExpectedParameterTypes =
        [
      typeof(Battle),
       typeof(Pokemon),
   typeof(Pokemon),
  typeof(IEffect),
        ];
  ExpectedReturnType = typeof(void);
        
// Nullability: All parameters non-nullable by default (adjust as needed)
    ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
     // Validate configuration
        ValidateConfiguration();
    }
    
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, TargetPokemon, SourcePokemon, SourceEffect
  /// </summary>
    public OnTerrainChangeEventInfo(
        EventHandlerDelegate contextHandler,
int? priority = null,
        bool usesSpeed = true)
    {
Id = EventId.TerrainChange;
        ContextHandler = contextHandler;
     Priority = priority;
   UsesSpeed = usesSpeed;
    }
    
    /// <summary>
/// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnTerrainChangeEventInfo Create(
   Action<Battle, Pokemon, Pokemon, IEffect> handler,
  int? priority = null,
        bool usesSpeed = true)
    {
        return new OnTerrainChangeEventInfo(
  context =>
  {
   handler(
context.Battle,
 context.GetTargetOrSourcePokemon(),
      context.GetSourceOrTargetPokemon(),
  context.GetSourceEffect<IEffect>()
  );
          return null;
            },
     priority,
   usesSpeed
     );
    }
}
