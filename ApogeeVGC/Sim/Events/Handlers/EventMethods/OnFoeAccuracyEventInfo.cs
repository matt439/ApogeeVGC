using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeAccuracy event.
/// Modifies accuracy of moves against foes.
/// Signature: (Battle battle, int accuracy, Pokemon target, Pokemon source, ActiveMove move) => IntBoolVoidUnion?
/// </summary>
public sealed record OnFoeAccuracyEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnFoeAccuracy event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnFoeAccuracyEventInfo(
        Func<Battle, int?, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?> handler,
  int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Accuracy;
      Prefix = EventPrefix.Foe;
#pragma warning disable CS0618
Handler = handler;
#pragma warning restore CS0618
        Priority = priority;
        UsesSpeed = usesSpeed;
  ExpectedParameterTypes =
  [
     typeof(Battle),
typeof(int?),
   typeof(Pokemon),
       typeof(Pokemon),
            typeof(ActiveMove),
        ];
        ExpectedReturnType = typeof(IntBoolVoidUnion);
        
    // Nullability: accuracy can be null for always-hit moves (TypeScript: number | true)
        ParameterNullability = [false, true, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnFoeAccuracyEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Accuracy;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeAccuracyEventInfo Create(
        Func<Battle, int?, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeAccuracyEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetNullableIntRelayVar(),
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetMove()
                );
                return result switch
                {
                    IntIntBoolVoidUnion i => new IntRelayVar(i.Value),
                    BoolIntBoolVoidUnion b => new BoolRelayVar(b.Value),
                    VoidIntBoolVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
