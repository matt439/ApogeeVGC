using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifySpD event.
/// Modifies the Special Defense stat.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, int, Pokemon, Pokemon, ActiveMove) => DoubleVoidUnion
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnModifySpDEventInfo : EventHandlerInfo
{
    /// <summary>
 /// Creates a new OnModifySpD event handler using the legacy strongly-typed pattern.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnModifySpDEventInfo(
  Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
        int? priority = null,
  bool usesSpeed = true)
    {
        Id = EventId.ModifySpD;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
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
      
        // Nullability: Battle, int, and target Pokemon are non-nullable
        // Source Pokemon and Move can be null (e.g., when calculating base stats)
   ParameterNullability = [false, false, false, true, true];
        ReturnTypeNullable = false;
    
        // Validate configuration
   ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, RelayVar (int value), TargetPokemon, SourcePokemon, Move
    /// </summary>
    public OnModifySpDEventInfo(
   EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
     Id = EventId.ModifySpD;
        ContextHandler = contextHandler;
     Priority = priority;
     UsesSpeed = usesSpeed;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnModifySpDEventInfo Create(
 Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
    int? priority = null,
bool usesSpeed = true)
    {
 return new OnModifySpDEventInfo(
   context =>
{
  var result = handler(
   context.Battle,
   context.GetIntRelayVar(),
 context.GetTargetOrSourcePokemon(),
       context.GetSourceOrTargetPokemon(),
   context.GetMove()
 );
     return result switch
       {
    DoubleDoubleVoidUnion d => new DecimalRelayVar((decimal)d.Value),
       VoidDoubleVoidUnion => null,
  _ => null
       };
 },
            priority,
usesSpeed
    );
 }
}
