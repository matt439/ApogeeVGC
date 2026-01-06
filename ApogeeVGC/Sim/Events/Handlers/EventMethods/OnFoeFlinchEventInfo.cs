using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeFlinch event.
/// Signature: Func<Battle, Pokemon, BoolVoidUnion> | bool
/// </summary>
public sealed record OnFoeFlinchEventInfo : UnionEventHandlerInfo<OnFlinch>
{
    public OnFoeFlinchEventInfo(
       OnFlinch unionValue,
     int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Flinch;
        Prefix = EventPrefix.Foe;
        UnionValue = unionValue;
     Handler = ExtractDelegate();
        Priority = priority;
  UsesSpeed = usesSpeed;
   ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
   ExpectedReturnType = typeof(BoolVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}