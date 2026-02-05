using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFlinch event.
/// Triggered when a Pokemon flinches.
/// Signature: (Battle battle, Pokemon pokemon) => BoolVoidUnion | bool
/// Note: TypeScript signature shows source/move params but they're always undefined in runEvent('Flinch', pokemon)
/// </summary>
public sealed record OnFlinchEventInfo : UnionEventHandlerInfo<OnFlinch>
{
    /// <summary>
    /// Creates a new OnFlinch event handler.
    /// </summary>
    /// <param name="unionValue">The union value (delegate or bool constant)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFlinchEventInfo(
        OnFlinch unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Flinch;
        UnionValue = unionValue;
        Handler = ExtractDelegate();
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
            typeof(Battle),
            typeof(Pokemon),
        ];
        ExpectedReturnType = typeof(BoolVoidUnion);

        // Nullability: All parameters non-nullable
        ParameterNullability = [false, false];
        ReturnTypeNullable = false;

        // Validate configuration
        ValidateConfiguration();
    }
}
