using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.SideEventMethods;

/// <summary>
/// Event handler info for OnSideRestart event (side-specific).
/// Triggered when a side condition restarts/reactivates.
/// Signature: Func&lt;Battle, Side, Pokemon, IEffect, VoidFalseUnion&gt;
/// </summary>
public sealed record OnSideRestartEventInfo : EventHandlerInfo
{
    public OnSideRestartEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SideRestart;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    public static OnSideRestartEventInfo Create(
        Func<Battle, Side, Pokemon, IEffect, VoidFalseUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSideRestartEventInfo(
            context =>
            {
                var result = handler(
                    context.Battle,
                    context.GetTargetSide(),
                    context.GetTargetOrSourcePokemon(),
                    context.GetSourceEffect<IEffect>()
                );
                if (result is FalseVoidFalseUnion)
                {
                    return new BoolRelayVar(false);
                }
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
