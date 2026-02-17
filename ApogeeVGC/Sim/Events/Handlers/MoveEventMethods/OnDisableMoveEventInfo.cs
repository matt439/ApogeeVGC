using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for OnDisableMove event (move-specific).
/// Triggered to disable a move.
/// Signature: Action&lt;Battle, Pokemon&gt;
/// </summary>
public sealed record OnDisableMoveEventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public OnDisableMoveEventInfo(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.DisableMove;
        Prefix = EventPrefix.None;
#pragma warning disable CS0618
Handler = handler;
#pragma warning restore CS0618
        Priority = priority;
     UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
    ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    public OnDisableMoveEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.DisableMove;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    public static OnDisableMoveEventInfo Create(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnDisableMoveEventInfo(
            context =>
            {
                handler(
                    context.Battle,
                    context.GetTargetOrSourcePokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
