using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllySetStatus event (pokemon/ally-specific).
/// Triggered when setting ally status.
/// Signature: Func<Battle, Condition, Pokemon, Pokemon, IEffect, PokemonFalseVoidUnion?>
/// </summary>
public sealed record OnAllySetStatusEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAllySetStatusEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SetStatus;
        Prefix = EventPrefix.Ally;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAllySetStatusEventInfo Create(
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, PokemonFalseVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAllySetStatusEventInfo(
            context =>
            {
                var result = handler(
                    context.Battle,
                    context.GetEffectParam<Condition>(),
                    context.GetTargetOrSourcePokemon(),
                    context.GetSourceOrTargetPokemon(),
                    context.GetSourceEffect<IEffect>()
                );
                return result switch
                {
                    PokemonPokemonFalseVoidUnion p => new PokemonRelayVar(p.Pokemon),
                    FalsePokemonFalseVoidUnion => BoolRelayVar.False,
                    VoidPokemonFalseVoidUnion => null,
                    null => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
