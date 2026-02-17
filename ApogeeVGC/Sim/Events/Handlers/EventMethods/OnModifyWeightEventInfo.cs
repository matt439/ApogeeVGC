using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifyWeight event.
/// Modifies a Pokemon's weight.
/// Signature: (Battle battle, int weighthg, Pokemon pokemon) => IntVoidUnion
/// </summary>
public sealed record OnModifyWeightEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnModifyWeight event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnModifyWeightEventInfo(
        Func<Battle, int, Pokemon, IntVoidUnion> handler,
  int? priority = null,
        bool usesSpeed = true)
    {
Id = EventId.ModifyWeight;
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
     ];
 ExpectedReturnType = typeof(IntVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnModifyWeightEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyWeight;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnModifyWeightEventInfo Create(
        Func<Battle, int, Pokemon, IntVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnModifyWeightEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetIntRelayVar(),
                context.GetTargetOrSourcePokemon()
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
