using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnEffectiveness event.
/// Modifies type effectiveness calculation.
/// Signature: (Battle battle, int typeMod, Pokemon? target, PokemonType type, ActiveMove move) => IntVoidUnion
/// </summary>
public sealed record OnEffectivenessEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnEffectiveness event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
  /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnEffectivenessEventInfo(
  Func<Battle, int, Pokemon?, PokemonType, ActiveMove, IntVoidUnion> handler,
int? priority = null,
        bool usesSpeed = true)
    {
    Id = EventId.Effectiveness;
#pragma warning disable CS0618
Handler = handler;
#pragma warning restore CS0618
Priority = priority;
      UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
  [
  typeof(Battle),
       typeof(int),
     typeof(Pokemon),
     typeof(PokemonType),
  typeof(ActiveMove),
    ];
  ExpectedReturnType = typeof(IntVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnEffectivenessEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Effectiveness;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnEffectivenessEventInfo Create(
        Func<Battle, int, Pokemon?, PokemonType, ActiveMove, IntVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnEffectivenessEventInfo(
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
