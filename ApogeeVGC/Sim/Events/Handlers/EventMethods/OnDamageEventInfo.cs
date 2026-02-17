using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnDamage event.
/// Modifies or prevents damage to a Pokemon.
/// Signature: (Battle battle, int damage, Pokemon target, Pokemon source, IEffect effect) => IntBoolVoidUnion?
/// </summary>
public sealed record OnDamageEventInfo : EventHandlerInfo
{
    public OnDamageEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Damage;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnDamageEventInfo Create(
        Func<Battle, int, Pokemon, Pokemon, IEffect, RelayVar?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnDamageEventInfo(
            context => handler(
                context.Battle,
                context.GetIntRelayVar(),
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetSourceEffect<IEffect>()
            ),
            priority,
            usesSpeed
        );
    }
}
