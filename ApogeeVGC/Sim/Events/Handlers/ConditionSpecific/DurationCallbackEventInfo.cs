using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;

/// <summary>
/// Event handler info for DurationCallback event.
/// Used to calculate dynamic durations for conditions.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Pokemon, Pokemon, IEffect?) => int
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record DurationCallbackEventInfo : EventHandlerInfo
{
    public DurationCallbackEventInfo(
        Func<Battle, Pokemon, Pokemon, IEffect?, int> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.DurationCallback;
        Prefix = EventPrefix.None;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(int);
        
        // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false;
    
        // Validate configuration
        ValidateConfiguration();
    }
    
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, TargetPokemon, SourcePokemon, SourceEffect
    /// </summary>
    public DurationCallbackEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.DurationCallback;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static DurationCallbackEventInfo Create(
        Func<Battle, Pokemon, Pokemon, IEffect?, int> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new DurationCallbackEventInfo(
            context =>
                {
                    int result = handler(
                        context.Battle,
                        context.GetTargetOrSourcePokemon(),
                        context.GetSourceOrTargetPokemon(),
                        context.TryGetSourceEffect<IEffect>()
                        );
                    return new IntRelayVar(result);
                },
            priority,
            usesSpeed
        );
    }
}