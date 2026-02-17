using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;

/// <summary>
/// Event handler info for OnEnd event (ability-specific).
/// Triggered when an ability effect ends.
/// Signature: Action&lt;Battle, PokemonSideFieldUnion&gt;
/// </summary>
public sealed record OnEndEventInfo : EventHandlerInfo
{
    public OnEndEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.End;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnEndEventInfo Create(
        Action<Battle, PokemonSideFieldUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnEndEventInfo(
            context =>
            {
                PokemonSideFieldUnion target;
                if (context.TargetPokemon != null)
                    target = new PokemonSideFieldPokemon(context.TargetPokemon);
                else if (context.TargetSide != null)
                    target = new PokemonSideFieldSide(context.TargetSide);
                else if (context.TargetField != null)
                    target = new PokemonSideFieldField(context.TargetField);
                else
                    throw new InvalidOperationException(
                        $"Event {EventId.End}: No target Pokemon, Side, or Field available for PokemonSideFieldUnion");

                handler(context.Battle, target);
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
