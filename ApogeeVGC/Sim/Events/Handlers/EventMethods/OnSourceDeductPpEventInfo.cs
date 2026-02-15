using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceDeductPp event.
/// Signature: Func<Battle, Pokemon, Pokemon, int>
/// </summary>
public sealed record OnSourceDeductPpEventInfo : EventHandlerInfo
{
 public OnSourceDeductPpEventInfo(
      Func<Battle, Pokemon, Pokemon, int> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.DeductPp;
   Prefix = EventPrefix.Source;
        Handler = handler;
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
    public OnSourceDeductPpEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.DeductPp;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourceDeductPpEventInfo Create(
        Func<Battle, Pokemon, Pokemon, int> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourceDeductPpEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetTargetPokemon(),
                context.GetSourcePokemon()
                );
                return new IntRelayVar(result);
            },
            priority,
            usesSpeed
        );
    }
}