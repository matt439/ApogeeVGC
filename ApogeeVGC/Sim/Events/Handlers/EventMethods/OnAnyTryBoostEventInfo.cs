using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyTryBoost event.
/// Signature: Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>
/// </summary>
public sealed record OnAnyTryBoostEventInfo : EventHandlerInfo
{
    public OnAnyTryBoostEventInfo(
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
     bool usesSpeed = true)
    {
        Id = EventId.TryBoost;
Prefix = EventPrefix.Any;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(SparseBoostsTable), typeof(Pokemon), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(void);

        // Nullability: All parameters non-nullable
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false; // void

        // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAnyTryBoostEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TryBoost;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAnyTryBoostEventInfo Create(
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAnyTryBoostEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetRelayVar<SparseBoostsTableRelayVar>().Table,
                context.GetTargetPokemon(),
                context.GetSourcePokemon(),
                context.GetSourceEffect<IEffect>()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}