using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSetAbility event.
/// Triggered when an ability is set on a Pokemon.
/// Signature: (Battle battle, Ability ability, Pokemon target, Pokemon source, IEffect effect) => VoidReturn?
/// Returns null to block the ability change, or VoidReturn to allow it.
/// </summary>
public sealed record OnSetAbilityEventInfo : EventHandlerInfo
{
    public OnSetAbilityEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SetAbility;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSetAbilityEventInfo Create(
        Func<Battle, Ability, Pokemon, Pokemon, IEffect, VoidReturn?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSetAbilityEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetEffectParam<Ability>(),
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetSourceEffect<IEffect>()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
