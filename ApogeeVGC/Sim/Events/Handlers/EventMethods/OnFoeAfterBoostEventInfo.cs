using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeAfterBoost event.
/// Triggered after stat boosts are applied to a foe Pokemon.
/// Signature: (Battle battle, SparseBoostsTable boost, Pokemon target, Pokemon source, IEffect effect) => void
/// </summary>
public sealed record OnFoeAfterBoostEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnFoeAfterBoost event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFoeAfterBoostEventInfo(
    Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterBoost;
        Prefix = EventPrefix.Foe;
     Handler = handler;
        Priority = priority;
   UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
    typeof(Battle),
typeof(SparseBoostsTable),
         typeof(Pokemon),
    typeof(Pokemon),
            typeof(IEffect),
    ];
    ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
