using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyEffectiveness event.
/// Signature: Func<Battle, int, Pokemon?, PokemonType, ActiveMove, IntVoidUnion>
/// </summary>
public sealed record OnAnyEffectivenessEventInfo : EventHandlerInfo
{
    public OnAnyEffectivenessEventInfo(
        Func<Battle, int, Pokemon?, PokemonType, ActiveMove, IntVoidUnion> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.Effectiveness;
Prefix = EventPrefix.Any;
        Handler = handler;
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(PokemonType), typeof(ActiveMove)];
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
    public OnAnyEffectivenessEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Effectiveness;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAnyEffectivenessEventInfo Create(
        Func<Battle, int, Pokemon?, PokemonType, ActiveMove, IntVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAnyEffectivenessEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetRelayVar<IntRelayVar>().Value,
                context.GetTargetPokemon(),
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