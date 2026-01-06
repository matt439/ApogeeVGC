using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeCriticalHit event.
/// Signature: Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion> | bool
/// </summary>
public sealed record OnFoeCriticalHitEventInfo : UnionEventHandlerInfo<OnCriticalHit>
{
    public OnFoeCriticalHitEventInfo(
    OnCriticalHit unionValue,
int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.CriticalHit;
        Prefix = EventPrefix.Foe;
     UnionValue = unionValue;
    Handler = ExtractDelegate();
     Priority = priority;
        UsesSpeed = usesSpeed;
  ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
   ExpectedReturnType = typeof(BoolVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}