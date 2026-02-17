using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyEffectiveness event (pokemon/ally-specific).
/// Triggered to modify type effectiveness against ally.
/// Signature: Func<Battle, int, Pokemon?, PokemonType, ActiveMove, IntVoidUnion>
/// </summary>
public sealed record OnAllyEffectivenessEventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public OnAllyEffectivenessEventInfo(
        Func<Battle, int, Pokemon?, PokemonType, ActiveMove, IntVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Effectiveness;
        Prefix = EventPrefix.Ally;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(PokemonType), typeof(ActiveMove)];
        ExpectedReturnType = typeof(IntVoidUnion);

        // Nullability: Battle (non-null), int (non-null), Pokemon (nullable), PokemonType (non-null), ActiveMove (non-null)
        ParameterNullability = new[] { false, false, true, false, false };
        ReturnTypeNullable = false; // IntVoidUnion is a struct

        // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAllyEffectivenessEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Effectiveness;
        Prefix = EventPrefix.Ally;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAllyEffectivenessEventInfo Create(
        Func<Battle, int, Pokemon?, PokemonType, ActiveMove, IntVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAllyEffectivenessEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetIntRelayVar(),
                context.GetTargetOrSourcePokemon(),
                context.SourceType!.Value,
                context.GetMove()
                );
                return result switch
                {
                    IntIntVoidUnion i => new IntRelayVar(i.Value),
                    VoidIntVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
