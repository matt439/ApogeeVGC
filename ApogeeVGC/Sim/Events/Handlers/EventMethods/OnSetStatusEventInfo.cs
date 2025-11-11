using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSetStatus event.
/// Triggered when attempting to set a status condition, can prevent it.
/// Signature: (Battle battle, Condition status, Pokemon target, Pokemon source, IEffect effect) => BoolVoidUnion?
/// </summary>
public sealed record OnSetStatusEventInfo : EventHandlerInfo
{
    /// <summary>
  /// Creates a new OnSetStatus event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnSetStatusEventInfo(
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.SetStatus;
     Handler = handler;
  Priority = priority;
        UsesSpeed = usesSpeed;
   ExpectedParameterTypes = new[] 
        { 
   typeof(Battle), 
            typeof(Condition), 
         typeof(Pokemon), 
            typeof(Pokemon), 
          typeof(IEffect) 
    };
        ExpectedReturnType = typeof(BoolVoidUnion);
    }
}
