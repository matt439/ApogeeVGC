using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for OnMoveFail event (move-specific).
/// Triggered when a move fails.
/// Signature: Action<Battle, Pokemon, Pokemon, ActiveMove>
/// </summary>
public sealed record OnMoveFailEventInfo : EventHandlerInfo
{
[Obsolete("Use Create factory method instead.")]
public OnMoveFailEventInfo(
        Action<Battle, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
   bool usesSpeed = true)
    {
    Id = EventId.MoveFail;
        Prefix = EventPrefix.None;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
        Priority = priority;
      UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    public OnMoveFailEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.MoveFail;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    public static OnMoveFailEventInfo Create(
        Action<Battle, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnMoveFailEventInfo(
            context =>
            {
                handler(
                    context.Battle,
                    context.GetTargetOrSourcePokemon(),
                    context.GetSourceOrTargetPokemon(),
                    context.GetMove()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
