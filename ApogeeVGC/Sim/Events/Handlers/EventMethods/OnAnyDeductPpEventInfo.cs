using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyDeductPp event.
/// Signature: Func<Battle, Pokemon, Pokemon, int>
/// </summary>
public sealed record OnAnyDeductPpEventInfo : EventHandlerInfo
{
 public OnAnyDeductPpEventInfo(
      Func<Battle, Pokemon, Pokemon, int> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.DeductPp;
   Prefix = EventPrefix.Any;
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
    public OnAnyDeductPpEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.DeductPp;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAnyDeductPpEventInfo Create(
        Func<Battle, Pokemon, Pokemon, int> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAnyDeductPpEventInfo(
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