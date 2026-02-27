using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for OnTryHitSide event (move-specific).
/// Triggered to try hitting a side.
/// Signature: Func<Battle, Side, Pokemon, ActiveMove, BoolEmptyVoidUnion?>
/// </summary>
public sealed record OnTryHitSideEventInfo : EventHandlerInfo
{
    public OnTryHitSideEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TryHitSide;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    public static OnTryHitSideEventInfo Create(
        Func<Battle, Side, Pokemon, ActiveMove, BoolEmptyVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnTryHitSideEventInfo(
            context =>
            {
                var result = handler(
                    context.Battle,
                    context.GetTargetSide(),
                    context.GetSourceOrTargetPokemon(),
                    context.GetMove()
                );
                if (result == null) return new NullRelayVar();
                return result switch
                {
                    BoolBoolEmptyVoidUnion b => (b.Value ? BoolRelayVar.True : BoolRelayVar.False),
                    EmptyBoolEmptyVoidUnion => new NullRelayVar(),
                    VoidUnionBoolEmptyVoidUnion => new VoidReturnRelayVar(),
                    _ => new VoidReturnRelayVar()
                };
            },
            priority,
            usesSpeed
        );
    }
}
