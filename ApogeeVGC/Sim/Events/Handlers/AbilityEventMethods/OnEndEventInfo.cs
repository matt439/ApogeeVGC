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
    /// <summary>
    /// Creates a new OnEnd event handler using the legacy strongly-typed pattern.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
 public OnEndEventInfo(
 Action<Battle, PokemonSideFieldUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.End;
        Prefix = EventPrefix.None;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(PokemonSideFieldUnion)];
        ExpectedReturnType = typeof(void);

    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false };
        ReturnTypeNullable = false;

    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, PokemonSideFieldUnion (from TargetPokemon, TargetSide, or TargetField)
    /// </summary>
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
