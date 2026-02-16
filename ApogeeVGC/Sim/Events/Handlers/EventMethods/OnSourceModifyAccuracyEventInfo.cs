using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceModifyAccuracy event.
/// Modifies move accuracy when this Pokemon is the source.
/// Signature: (Battle battle, int? relayVar, Pokemon target, Pokemon source, ActiveMove move) => DoubleVoidUnion
/// Note: accuracy is nullable because moves with true accuracy (always hit) pass null/true instead of a number
/// </summary>
public sealed record OnSourceModifyAccuracyEventInfo : EventHandlerInfo
{
  /// <summary>
  /// Creates a new OnSourceModifyAccuracy event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnSourceModifyAccuracyEventInfo(
    Func<Battle, int?, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
  int? priority = null,
  bool usesSpeed = true)
    {
        Id = EventId.ModifyAccuracy;
    Prefix = EventPrefix.Source;
Handler = handler;
Priority = priority;
      UsesSpeed = usesSpeed;
    ExpectedParameterTypes =
    [
       typeof(Battle),
   typeof(int?),
   typeof(Pokemon),
  typeof(Pokemon),
      typeof(ActiveMove),
  ];
  ExpectedReturnType = typeof(DoubleVoidUnion);
        
    // Nullability: accuracy parameter is nullable (position 1)
        ParameterNullability = [false, true, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnSourceModifyAccuracyEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyAccuracy;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourceModifyAccuracyEventInfo Create(
        Func<Battle, int?, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourceModifyAccuracyEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetNullableIntRelayVar(),
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
