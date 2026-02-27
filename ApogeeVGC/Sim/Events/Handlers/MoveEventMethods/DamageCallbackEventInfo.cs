using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for DamageCallback event (move-specific).
/// Callback for calculating damage dynamically.
/// Signature: Func&lt;Battle, Pokemon, Pokemon, ActiveMove, IntFalseUnion&gt;
/// </summary>
public sealed record DamageCallbackEventInfo : EventHandlerInfo
{
    public DamageCallbackEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.DamageCallback;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    public static DamageCallbackEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, IntFalseUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new DamageCallbackEventInfo(
            context =>
            {
                var result = handler(
                    context.Battle,
                    context.GetSourceOrTargetPokemon(),
                    context.GetTargetOrSourcePokemon(),
                    context.GetMove()
                );
                return result switch
                {
                    IntIntFalseUnion i => new IntRelayVar(i.Value),
                    FalseIntFalseUnion => BoolRelayVar.False,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
