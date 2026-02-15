using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeAfterMove event.
/// Signature: Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>
/// </summary>
public sealed record OnFoeAfterMoveEventInfo : EventHandlerInfo
{
    public OnFoeAfterMoveEventInfo(
     Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion> handler,
  int? priority = null,
    bool usesSpeed = true)
    {
      Id = EventId.AfterMove;
     Prefix = EventPrefix.Foe;
  Handler = handler;
    Priority = priority;
        UsesSpeed = usesSpeed;
 ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
     ExpectedReturnType = typeof(BoolVoidUnion);
        
     // Nullability: All parameters non-nullable
    ParameterNullability = [false, false, false, false];
  ReturnTypeNullable = false; // BoolVoidUnion is a struct
        
   // Validate configuration
  ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnFoeAfterMoveEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterMove;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeAfterMoveEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeAfterMoveEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetTargetPokemon(),
                context.GetSourcePokemon(),
                context.GetMove()
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