using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for OnTryHitField event (move-specific).
/// Triggered to try hitting the field.
/// Signature: Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>
/// </summary>
public sealed record OnTryHitFieldEventInfo : EventHandlerInfo
{
[Obsolete("Use Create factory method instead.")]
public OnTryHitFieldEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?> handler,
        int? priority = null,
   bool usesSpeed = true)
    {
    Id = EventId.TryHitField;
        Prefix = EventPrefix.None;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
        Priority = priority;
      UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(BoolEmptyVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    public OnTryHitFieldEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TryHitField;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    public static OnTryHitFieldEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnTryHitFieldEventInfo(
            context =>
            {
                var result = handler(
                    context.Battle,
                    context.GetTargetOrSourcePokemon(),
                    context.GetSourceOrTargetPokemon(),
                    context.GetMove()
                );
                if (result == null) return null;
                return result switch
                {
                    BoolBoolEmptyVoidUnion b => new BoolRelayVar(b.Value),
                    EmptyBoolEmptyVoidUnion => null,
                    VoidUnionBoolEmptyVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
