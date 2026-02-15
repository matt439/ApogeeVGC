using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyLockMove event (pokemon/ally-specific).
/// Forces a Pokémon to use a specific move.
/// Signature: (Battle battle, Pokemon pokemon) => MoveIdVoidUnion | MoveId?
/// </summary>
public sealed record OnAllyLockMoveEventInfo : UnionEventHandlerInfo<OnLockMove>
{
    /// <summary>
    /// Creates a new OnAllyLockMove event handler.
    /// </summary>
    /// <param name="unionValue">The union value (delegate or MoveId constant)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnAllyLockMoveEventInfo(
  OnLockMove unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
     Id = EventId.LockMove;
      Prefix = EventPrefix.Ally;
     UnionValue = unionValue;
Handler = ExtractDelegate();
   Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
     ExpectedReturnType = typeof(ActiveMove);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAllyLockMoveEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.LockMove;
        Prefix = EventPrefix.Ally;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAllyLockMoveEventInfo Create(
        Func<Battle, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAllyLockMoveEventInfo(
            context =>
            {
                var result = handler(
                    context.Battle,
                    context.GetTargetPokemon()
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