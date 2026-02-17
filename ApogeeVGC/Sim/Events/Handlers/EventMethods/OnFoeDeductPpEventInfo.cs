using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeDeductPp event.
/// Signature: Func<Battle, Pokemon, Pokemon, int>
/// </summary>
public sealed record OnFoeDeductPpEventInfo : EventHandlerInfo
{
 [Obsolete("Use Create factory method instead.")]
 public OnFoeDeductPpEventInfo(
      Func<Battle, Pokemon, Pokemon, int> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.DeductPp;
   Prefix = EventPrefix.Foe;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
    Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon)];
        ExpectedReturnType = typeof(int);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnFoeDeductPpEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.DeductPp;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeDeductPpEventInfo Create(
        Func<Battle, Pokemon, Pokemon, int> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeDeductPpEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon()
                );
                return new IntRelayVar(result);
            },
            priority,
            usesSpeed
        );
    }
}
