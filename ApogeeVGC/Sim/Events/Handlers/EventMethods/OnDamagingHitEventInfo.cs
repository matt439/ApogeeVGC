using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnDamagingHit event.
/// Triggered when a Pokemon is hit by a damaging move.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, int, Pokemon, Pokemon, ActiveMove) => void
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnDamagingHitEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnDamagingHit event handler using the legacy strongly-typed pattern.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="order">Execution order (lower executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnDamagingHitEventInfo(
        Action<Battle, int, Pokemon, Pokemon, ActiveMove> handler,
        int? order = null,
        bool usesSpeed = true)
    {
        Id = EventId.DamagingHit;
        Handler = handler;
        Order = order;
    UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
            typeof(Battle), 
            typeof(int), 
            typeof(Pokemon), 
  typeof(Pokemon), 
            typeof(ActiveMove),
  ];
  ExpectedReturnType = typeof(void);
        
     // Nullability: All parameters are non-nullable
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false; // void
   
 // Validate configuration
ValidateConfiguration();
    }
    
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, RelayVar (int damage), TargetPokemon, SourcePokemon, Move
    /// </summary>
    public OnDamagingHitEventInfo(
        EventHandlerDelegate contextHandler,
 int? order = null,
        bool usesSpeed = true)
    {
        Id = EventId.DamagingHit;
ContextHandler = contextHandler;
     Order = order;
        UsesSpeed = usesSpeed;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnDamagingHitEventInfo Create(
        Action<Battle, int, Pokemon, Pokemon, ActiveMove> handler,
        int? order = null,
      bool usesSpeed = true)
    {
        return new OnDamagingHitEventInfo(
     context =>
          {
  handler(
         context.Battle,
     context.GetIntRelayVar(),
              context.GetTargetOrSourcePokemon(),
     context.GetSourceOrTargetPokemon(),
        context.GetMove()
        );
 return null;
          },
            order,
       usesSpeed
        );
    }
}
