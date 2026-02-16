using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyLockMove event.
/// Forces a Pokémon to use a specific move.
/// Signature: (Battle battle, Pokemon pokemon) => MoveIdVoidUnion | MoveId?
/// </summary>
public sealed record OnAnyLockMoveEventInfo : UnionEventHandlerInfo<OnLockMove>
{
    /// <summary>
    /// Creates a new OnAnyLockMove event handler.
    /// </summary>
    /// <param name="unionValue">The union value (delegate or MoveId constant)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
  /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
 public OnAnyLockMoveEventInfo(
        OnLockMove unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
Id = EventId.LockMove;
    Prefix = EventPrefix.Any;
        UnionValue = unionValue;
        Handler = ExtractDelegate();
   Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(ActiveMove);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAnyLockMoveEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.LockMove;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAnyLockMoveEventInfo Create(
        Func<Battle, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAnyLockMoveEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon()
                );
                return result switch
                {
                    null => null,
                    _ => new EffectRelayVar(result)
                };
            },
            priority,
            usesSpeed
        );
    }
}