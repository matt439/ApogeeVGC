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
    /// <summary>
    /// Creates a new OnDragOut event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnDragOutEventInfo(
        Func<Battle, Pokemon, Pokemon?, ActiveMove?, BoolVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.DragOut;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
            typeof(Battle),
            typeof(Pokemon),
            typeof(Pokemon),
            typeof(ActiveMove),
        ];
        ExpectedReturnType = typeof(BoolVoidUnion);

        // Nullability: source and move are nullable
        ParameterNullability = [false, false, true, true];
        ReturnTypeNullable = true;

        // Validate configuration
        ValidateConfiguration();
    }
}
