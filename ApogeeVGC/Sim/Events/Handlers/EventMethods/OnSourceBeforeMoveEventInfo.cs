using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceBeforeMove event.
/// Triggered before this Pokemon uses a move as the source.
/// Signature: (Battle battle, Pokemon source, Pokemon target, ActiveMove move) => BoolVoidUnion
/// </summary>
public sealed record OnSourceBeforeMoveEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSourceBeforeMove event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnSourceBeforeMoveEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion> handler,
        int? priority = null,
bool usesSpeed = true)
    {
      Id = EventId.BeforeMove;
  Prefix = EventPrefix.Source;
        Handler = handler;
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
    public OnSourceBeforeMoveEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.BeforeMove;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourceBeforeMoveEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourceBeforeMoveEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetSourcePokemon(),
                context.GetTargetPokemon(),
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
