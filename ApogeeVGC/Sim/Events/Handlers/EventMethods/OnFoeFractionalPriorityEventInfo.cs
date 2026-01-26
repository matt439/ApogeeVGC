using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeFractionalPriority event.
/// Signature: (Battle, int, Pokemon, Pokemon?, ActiveMove) => double | decimal constant
/// </summary>
public sealed record OnFoeFractionalPriorityEventInfo : UnionEventHandlerInfo<OnFractionalPriority>
{
    public OnFoeFractionalPriorityEventInfo(
        OnFractionalPriority unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.FractionalPriority;
        Prefix = EventPrefix.Foe;
        UnionValue = unionValue;
        Handler = ExtractDelegate();
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(DoubleVoidUnion);
        
        // Nullability: target (4th param) is nullable since it's passed as null
        ParameterNullability = [false, false, false, true, false];
        ReturnTypeNullable = false;
    
        // Validate configuration
        ValidateConfiguration();
    }
}