using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.SideEventMethods;

/// <summary>
/// Event handler info for OnSideRestart event (side-specific).
/// Triggered when a side condition restarts/reactivates.
/// Signature: Func&lt;Battle, Side, Pokemon, IEffect, VoidFalseUnion&gt;
/// </summary>
public sealed record OnSideRestartEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSideRestart event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnSideRestartEventInfo(
    Func<Battle, Side, Pokemon, IEffect, VoidFalseUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SideRestart;
        Prefix = EventPrefix.None;
      #pragma warning disable CS0618
      Handler = handler;
      #pragma warning restore CS0618
        Priority = priority;
   UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Side), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(VoidFalseUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    public OnSideRestartEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SideRestart;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    public static OnSideRestartEventInfo Create(
        Func<Battle, Side, Pokemon, IEffect, VoidFalseUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSideRestartEventInfo(
            context =>
            {
                var result = handler(
                    context.Battle,
                    context.GetTargetSide(),
                    context.GetTargetOrSourcePokemon(),
                    context.GetSourceEffect<IEffect>()
                );
                if (result is FalseVoidFalseUnion)
                {
                    return new BoolRelayVar(false);
                }
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
