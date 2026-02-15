using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeFlinch event.
/// Signature: Func<Battle, Pokemon, BoolVoidUnion> | bool
/// </summary>
public sealed record OnFoeFlinchEventInfo : UnionEventHandlerInfo<OnFlinch>
{
    public OnFoeFlinchEventInfo(
       OnFlinch unionValue,
     int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Flinch;
        Prefix = EventPrefix.Foe;
        UnionValue = unionValue;
     Handler = ExtractDelegate();
        Priority = priority;
  UsesSpeed = usesSpeed;
   ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
   ExpectedReturnType = typeof(BoolVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnFoeFlinchEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Flinch;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeFlinchEventInfo Create(
        Func<Battle, Pokemon, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeFlinchEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetTargetPokemon()
                );
                return result switch
                {
                    BoolBoolVoidUnion b => new BoolRelayVar(b.Value),
                    VoidBoolVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}