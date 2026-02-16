using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for BeforeTurnCallback event (move-specific).
/// Callback executed before a turn begins.
/// Signature: Action&lt;Battle, Pokemon, Pokemon&gt;
/// </summary>
public sealed record BeforeTurnCallbackEventInfo : EventHandlerInfo
{
    public BeforeTurnCallbackEventInfo(
        Action<Battle, Pokemon, Pokemon> handler,
  int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.BeforeTurnCallback;
        Prefix = EventPrefix.None;
      Handler = handler;
        Priority = priority;
      UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    public BeforeTurnCallbackEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.BeforeTurnCallback;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    public static BeforeTurnCallbackEventInfo Create(
        Action<Battle, Pokemon, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new BeforeTurnCallbackEventInfo(
            context =>
            {
                handler(
                    context.Battle,
                    context.GetTargetOrSourcePokemon(),
                    context.GetSourceOrTargetPokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
