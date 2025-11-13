using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for BasePowerCallback event (move-specific).
/// Callback for calculating base power dynamically.
/// Signature: Func&lt;Battle, Pokemon, Pokemon, ActiveMove, IntFalseUnion?&gt;
/// </summary>
public sealed record BasePowerCallbackEventInfo : EventHandlerInfo
{
    public BasePowerCallbackEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, IntFalseUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.BasePowerCallback;
        Prefix = EventPrefix.None;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(IntFalseUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
