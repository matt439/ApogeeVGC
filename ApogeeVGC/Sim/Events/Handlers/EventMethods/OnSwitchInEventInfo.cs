using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSwitchIn event.
/// Triggered when a Pokemon switches in.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Pokemon) => void
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnSwitchInEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSwitchIn event handler using the legacy strongly-typed pattern.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnSwitchInEventInfo(
     Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SwitchIn;
    #pragma warning disable CS0618
    Handler = handler;
    #pragma warning restore CS0618
        Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
 [
            typeof(Battle),
            typeof(Pokemon),
        ];
        ExpectedReturnType = typeof(void);

        // Nullability: Battle (non-null), Pokemon (non-null)
        ParameterNullability = [false, false];
  ReturnTypeNullable = false; // void

        // Validate configuration
    ValidateConfiguration();
    }
    
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, TargetPokemon
    /// </summary>
    public OnSwitchInEventInfo(
     EventHandlerDelegate contextHandler,
    int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SwitchIn;
        ContextHandler = contextHandler;
        Priority = priority;
    UsesSpeed = usesSpeed;
    }
 
    /// <summary>
  /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
  public static OnSwitchInEventInfo Create(
        Action<Battle, Pokemon> handler,
  int? priority = null,
     bool usesSpeed = true)
    {
        return new OnSwitchInEventInfo(
            context =>
    {
     handler(
context.Battle,
         context.GetTargetOrSourcePokemon()
  );
         return null;
            },
  priority,
      usesSpeed
        );
    }
}
