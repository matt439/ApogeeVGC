using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnRedirectTarget event.
/// Redirects move target to a different Pokemon.
/// Signature: (Battle battle, Pokemon target, Pokemon source, IEffect effect, ActiveMove move) => PokemonVoidUnion
/// </summary>
public sealed record OnRedirectTargetEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnRedirectTarget event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnRedirectTargetEventInfo(
     Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
     Id = EventId.RedirectTarget;
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
            typeof(IEffect),
            typeof(ActiveMove),
        ];
        ExpectedReturnType = typeof(PokemonVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnRedirectTargetEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.RedirectTarget;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnRedirectTargetEventInfo Create(
        Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnRedirectTargetEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetSourceEffect<IEffect>(),
                context.GetMove()
                );
                return result switch
                {
                    PokemonPokemonVoidUnion p => new PokemonRelayVar(p.Pokemon),
                    VoidPokemonVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
