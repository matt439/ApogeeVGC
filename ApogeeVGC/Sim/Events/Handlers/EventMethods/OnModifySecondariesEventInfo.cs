using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifySecondaries event.
/// Modifies secondary effects of a move.
/// Signature: (Battle battle, SecondaryEffect[] secondaries, Pokemon target, Pokemon source, ActiveMove move) => SecondaryEffect[]
/// </summary>
public sealed record OnModifySecondariesEventInfo : EventHandlerInfo
{
    public OnModifySecondariesEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifySecondaries;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnModifySecondariesEventInfo Create(
        Func<Battle, SecondaryEffect[], Pokemon, Pokemon, ActiveMove, SecondaryEffect[]> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnModifySecondariesEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetRelayVar<SecondaryEffectArrayRelayVar>().Effects,
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetMove()
                );
                return new SecondaryEffectArrayRelayVar(result);
            },
            priority,
            usesSpeed
        );
    }
}
