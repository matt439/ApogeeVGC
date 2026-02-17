using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for DamageCallback event (move-specific).
/// Callback for calculating damage dynamically.
/// Signature: Func&lt;Battle, Pokemon, Pokemon, ActiveMove, IntFalseUnion&gt;
/// </summary>
public sealed record DamageCallbackEventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public DamageCallbackEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, IntFalseUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.DamageCallback;
        Prefix = EventPrefix.None;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
            [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(IntFalseUnion);

        // Nullability: Battle (non-null), source (non-null), target (non-null), move (non-null)
        ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false; // IntFalseUnion is a struct

        // Validate configuration
        ValidateConfiguration();
    }

    public DamageCallbackEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.DamageCallback;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    public static DamageCallbackEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, IntFalseUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new DamageCallbackEventInfo(
            context =>
            {
                var result = handler(
                    context.Battle,
                    context.GetSourceOrTargetPokemon(),
                    context.GetTargetOrSourcePokemon(),
                    context.GetMove()
                );
                return result switch
                {
                    IntIntFalseUnion i => new IntRelayVar(i.Value),
                    FalseIntFalseUnion => new BoolRelayVar(false),
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
