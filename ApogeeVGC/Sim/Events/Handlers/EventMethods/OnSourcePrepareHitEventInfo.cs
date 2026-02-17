using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourcePrepareHit event.
/// Triggered before a move hits when this Pokemon is the source.
/// Signature: (Battle battle, Pokemon source, Pokemon target, ActiveMove move) => BoolEmptyVoidUnion?
/// </summary>
public sealed record OnSourcePrepareHitEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSourcePrepareHit event handler.
    /// </summary>
/// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnSourcePrepareHitEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?> handler,
int? priority = null,
bool usesSpeed = true)
    {
 Id = EventId.PrepareHit;
  Prefix = EventPrefix.Source;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
  Priority = priority;
 UsesSpeed = usesSpeed;
    ExpectedParameterTypes =
        [
     typeof(Battle),
  typeof(Pokemon),
  typeof(Pokemon),
    typeof(ActiveMove),
 ];
        ExpectedReturnType = typeof(BoolEmptyVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnSourcePrepareHitEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.PrepareHit;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourcePrepareHitEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourcePrepareHitEventInfo(
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
                    BoolBoolEmptyVoidUnion b => new BoolRelayVar(b.Value),
                    EmptyBoolEmptyVoidUnion => new BoolRelayVar(false),
                    VoidUnionBoolEmptyVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
