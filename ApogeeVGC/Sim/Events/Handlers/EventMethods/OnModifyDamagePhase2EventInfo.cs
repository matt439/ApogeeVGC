using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifyDamagePhase2 event.
/// Modifies damage in phase 2 of damage calculation.
/// Signature: (Battle battle, int relayVar, Pokemon source, Pokemon target, ActiveMove move) => DoubleVoidUnion
/// </summary>
public sealed record OnModifyDamagePhase2EventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnModifyDamagePhase2 event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
/// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnModifyDamagePhase2EventInfo(
  Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
        int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.ModifyDamagePhase2;
 Handler = handler;
Priority = priority;
UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
    [
        typeof(Battle),
            typeof(int),
       typeof(Pokemon),
 typeof(Pokemon),
  typeof(ActiveMove),
];
ExpectedReturnType = typeof(DoubleVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnModifyDamagePhase2EventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyDamagePhase2;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnModifyDamagePhase2EventInfo Create(
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnModifyDamagePhase2EventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetRelayVar<IntRelayVar>().Value,
                context.GetSourcePokemon(),
                context.GetTargetPokemon(),
                context.GetMove()
                );
                return result switch
                {
                    DoubleDoubleVoidUnion d => new DecimalRelayVar((decimal)d.Value),
                    VoidDoubleVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
