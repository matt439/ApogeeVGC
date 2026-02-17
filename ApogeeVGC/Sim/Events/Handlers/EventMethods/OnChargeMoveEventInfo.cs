using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnChargeMove event.
/// Triggered when a Pokemon is charging a move (e.g., Solar Beam).
/// Signature: (Battle battle, Pokemon source, Pokemon target, ActiveMove move) => BoolVoidUnion
/// </summary>
public sealed record OnChargeMoveEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnChargeMove event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnChargeMoveEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion> handler,
        int? priority = null,
  bool usesSpeed = true)
    {
   Id = EventId.ChargeMove;
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
      typeof(ActiveMove),
     ];
     ExpectedReturnType = typeof(BoolVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
  }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnChargeMoveEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ChargeMove;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnChargeMoveEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnChargeMoveEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetSourceOrTargetPokemon(),
                context.GetTargetOrSourcePokemon(),
                context.GetMove()
                );
                return result switch
                {
                    BoolBoolVoidUnion b => new BoolRelayVar(b.Value),
                    VoidBoolVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
