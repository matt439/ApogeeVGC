using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceFractionalPriority event.
/// Modifies move priority with fractional adjustment.
/// Signature: (Battle battle, int priority, Pokemon pokemon, ActiveMove move) => double
/// </summary>
public sealed record OnSourceFractionalPriorityEventInfo : UnionEventHandlerInfo<OnFractionalPriority>
{
    public OnSourceFractionalPriorityEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.FractionalPriority;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourceFractionalPriorityEventInfo Create(
        Func<Battle, int, Pokemon, ActiveMove, double> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourceFractionalPriorityEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetIntRelayVar(),
                context.GetTargetOrSourcePokemon(),
                context.GetMove()
                );
                return new DecimalRelayVar((decimal)result);
            },
            priority,
            usesSpeed
        );
    }
}
