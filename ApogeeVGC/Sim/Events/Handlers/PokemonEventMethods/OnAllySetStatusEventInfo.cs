using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllySetStatus event (pokemon/ally-specific).
/// Triggered when setting ally status.
/// Signature: Func<Battle, Condition, Pokemon, Pokemon, IEffect, PokemonFalseVoidUnion?>
/// </summary>
public sealed record OnAllySetStatusEventInfo : EventHandlerInfo
{
    public OnAllySetStatusEventInfo(
    Func<Battle, Condition, Pokemon, Pokemon, IEffect, PokemonFalseVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SetStatus;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Condition), typeof(Pokemon), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(PokemonFalseVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false, false };
        ReturnTypeNullable = true;
    
    // Validate configuration
        ValidateConfiguration();
  }
}