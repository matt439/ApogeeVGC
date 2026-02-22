using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for OnHitSide event (move-specific).
/// Triggered when a move hits a side.
/// Signature: Func<Battle, Side, Pokemon, ActiveMove, BoolEmptyVoidUnion?>
/// </summary>
public sealed record OnHitSideEventInfo : EventHandlerInfo
{
    public OnHitSideEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.HitSide;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    public static OnHitSideEventInfo Create(
        Func<Battle, Side, Pokemon, ActiveMove, BoolEmptyVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnHitSideEventInfo(
            context =>
            {
                var result = handler(
                    context.Battle,
                    context.GetTargetSide(),
                    context.GetSourceOrTargetPokemon(),
                    context.GetMove()
                );
                // null = TS null (silent fail), VoidReturn = TS undefined (passthrough)
                if (result == null) return new NullRelayVar();
                return result switch
                {
                    BoolBoolEmptyVoidUnion b => new BoolRelayVar(b.Value),
                    EmptyBoolEmptyVoidUnion => new NullRelayVar(),
                    VoidUnionBoolEmptyVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
