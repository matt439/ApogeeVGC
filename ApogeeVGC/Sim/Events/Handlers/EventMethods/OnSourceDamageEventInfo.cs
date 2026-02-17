using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceDamage event.
/// Modifies or prevents damage when this Pokemon is the source.
/// Signature: (Battle battle, int damage, Pokemon target, Pokemon source, IEffect effect) => IntBoolVoidUnion?
/// </summary>
public sealed record OnSourceDamageEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSourceDamage event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
 /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnSourceDamageEventInfo(
  Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?> handler,
 int? priority = null,
      bool usesSpeed = true)
    {
    Id = EventId.Damage;
 Prefix = EventPrefix.Source;
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
            typeof(Pokemon),
   typeof(IEffect),
  ];
  ExpectedReturnType = typeof(IntBoolVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnSourceDamageEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Damage;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourceDamageEventInfo Create(
        Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourceDamageEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetIntRelayVar(),
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetSourceEffect<IEffect>()
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
