using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeModifySpe event.
/// Signature: Func<Battle, int, Pokemon, IntVoidUnion>
/// </summary>
public sealed record OnFoeModifySpeEventInfo : EventHandlerInfo
{
 [Obsolete("Use Create factory method instead.")]
 public OnFoeModifySpeEventInfo(
      Func<Battle, int, Pokemon, IntVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.ModifySpe;
   Prefix = EventPrefix.Foe;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
    Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon)];
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
    public OnFoeModifySpeEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifySpe;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeModifySpeEventInfo Create(
        Func<Battle, int, Pokemon, IntVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeModifySpeEventInfo(
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
