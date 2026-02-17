using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceModifyAtk event.
/// Modifies the Attack stat when this Pokemon is the source.
/// Signature: (Battle battle, int relayVar, Pokemon source, Pokemon target, ActiveMove move) => DoubleVoidUnion
/// </summary>
public sealed record OnSourceModifyAtkEventInfo : EventHandlerInfo
{
    /// <summary>
/// Creates a new OnSourceModifyAtk event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnSourceModifyAtkEventInfo(
     Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
 int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.ModifyAtk;
  Prefix = EventPrefix.Source;
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
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnSourceModifyAtkEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyAtk;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourceModifyAtkEventInfo Create(
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourceModifyAtkEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetIntRelayVar(),
                context.GetSourceOrTargetPokemon(),
                context.GetTargetOrSourcePokemon(),
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
