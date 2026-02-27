using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnDragOut event.
/// Triggered when a Pokemon is forcibly switched out.
/// Signature: (Battle battle, Pokemon pokemon, Pokemon? source, ActiveMove? move) => BoolVoidUnion?
/// Return values:
/// - null: Prevent drag-out silently (e.g., Suction Cups)
/// - false: Prevent drag-out with fail message for status moves
/// - true/void: Allow drag-out to proceed
/// </summary>
public sealed record OnDragOutEventInfo : EventHandlerInfo
{
    public OnDragOutEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.DragOut;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnDragOutEventInfo Create(
        Func<Battle, Pokemon, Pokemon?, ActiveMove?, BoolVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnDragOutEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon(),
                context.SourcePokemon,
                context.Move
                );
                return result switch
                {
                    BoolBoolVoidUnion b => (b.Value ? BoolRelayVar.True : BoolRelayVar.False),
                    VoidBoolVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
