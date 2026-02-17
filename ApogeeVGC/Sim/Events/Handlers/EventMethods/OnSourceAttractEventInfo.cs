using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceAttract event.
/// Signature: Action<Battle, Pokemon, Pokemon>
/// </summary>
public sealed record OnSourceAttractEventInfo : EventHandlerInfo
{
 [Obsolete("Use Create factory method instead.")]
 public OnSourceAttractEventInfo(
      Action<Battle, Pokemon, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.Attract;
   Prefix = EventPrefix.Source;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
    Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnSourceAttractEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Attract;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourceAttractEventInfo Create(
        Action<Battle, Pokemon, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourceAttractEventInfo(
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
