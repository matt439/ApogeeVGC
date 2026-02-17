using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnBasePower event.
/// Modifies the base power of a move.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, int, Pokemon, Pokemon, ActiveMove) => DoubleVoidUnion
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnBasePowerEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnBasePower event handler using the legacy strongly-typed pattern.
    /// </summary>
    /// <param name="handler">The event handler delegate (ModifierSourceMoveHandler)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnBasePowerEventInfo(
        ModifierSourceMoveHandler handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.BasePower;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
            typeof(Battle),
            typeof(int), // relayVar
            typeof(Pokemon), // source
            typeof(Pokemon), // target
            typeof(ActiveMove),
        ];
        ExpectedReturnType = typeof(DoubleVoidUnion);

        // Nullability: All parameters are non-nullable
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false; // DoubleVoidUnion is a struct

        // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, RelayVar (int base power), SourcePokemon, TargetPokemon, Move
    /// </summary>
    public OnBasePowerEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.BasePower;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnBasePowerEventInfo Create(
        ModifierSourceMoveHandler handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnBasePowerEventInfo(
            context =>
            {
                var result = handler(
                    context.Battle,
                    context.GetIntRelayVar(),
                    context.GetSourceOrTargetPokemon(),
                    context.GetTargetOrSourcePokemon(),
                    context.GetMove()
                );
                // Pattern match DoubleVoidUnion
                return result switch
                {
                    DoubleDoubleVoidUnion d => new DecimalRelayVar((decimal)d.Value),
                    VoidDoubleVoidUnion => null,
                    _ => null,
                };
            },
            priority,
            usesSpeed
        );
    }
}
