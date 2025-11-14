using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifySpe event.
/// Modifies the Speed stat.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, int, Pokemon) => IntVoidUnion
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnModifySpeEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnModifySpe event handler using the legacy strongly-typed pattern.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnModifySpeEventInfo(
        Func<Battle, int, Pokemon, IntVoidUnion> handler,
     int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifySpe;
        Handler = handler;
        Priority = priority;
    UsesSpeed = usesSpeed;
   ExpectedParameterTypes =
     [
  typeof(Battle),
            typeof(int),
    typeof(Pokemon),
   ];
        ExpectedReturnType = typeof(IntVoidUnion);
 
    // Nullability: All parameters non-nullable by default (adjust as needed)
     ParameterNullability = [false, false, false];
        ReturnTypeNullable = false;
    
     // Validate configuration
        ValidateConfiguration();
    }
    
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, RelayVar (int value), TargetPokemon
    /// </summary>
    public OnModifySpeEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
 bool usesSpeed = true)
    {
        Id = EventId.ModifySpe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnModifySpeEventInfo Create(
Func<Battle, int, Pokemon, IntVoidUnion> handler,
      int? priority = null,
     bool usesSpeed = true)
    {
    return new OnModifySpeEventInfo(
context =>
    {
       var result = handler(
     context.Battle,
         context.GetRelayVar<IntRelayVar>().Value,
   context.GetTargetPokemon()
    );
    return result switch
     {
          IntIntVoidUnion i => new IntRelayVar(i.Value),
     VoidIntVoidUnion => null,
     _ => null
       };
  },
            priority,
     usesSpeed
    );
    }
}
