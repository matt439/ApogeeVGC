using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifyType event.
/// Modifies the type of a move before it executes.
/// Signature: (Battle battle, ActiveMove move, Pokemon pokemon, Pokemon target) => void
/// </summary>
public sealed record OnModifyTypeEventInfo : EventHandlerInfo
{
    public OnModifyTypeEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyType;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnModifyTypeEventInfo Create(
        Action<Battle, ActiveMove, Pokemon, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnModifyTypeEventInfo(
            context =>
            {
                handler(
                    context.Battle,
                    context.GetMove(),
                    context.GetSourceOrTargetPokemon(),
                    context.GetTargetOrSourcePokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
