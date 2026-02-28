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
                    return IntRelayVar.Get(result);
                },
            priority,
            usesSpeed
        );
    }

    /// <summary>
    /// Invokes the duration callback via the context-based handler.
    /// </summary>
    public int InvokeDuration(Battle battle, Pokemon target, Pokemon source, IEffect? sourceEffect)
    {
        if (ContextHandler is null)
        {
            throw new InvalidOperationException("DurationCallback has no ContextHandler set.");
        }

        var context = new EventContext
        {
            Battle = battle,
            EventId = EventId.DurationCallback,
            TargetPokemon = target,
            SourcePokemon = source,
            SourceEffect = sourceEffect
        };
        var result = ContextHandler(context);
        return result is IntRelayVar irv
            ? irv.Value
            : throw new InvalidOperationException("DurationCallback ContextHandler did not return IntRelayVar.");
    }
}
