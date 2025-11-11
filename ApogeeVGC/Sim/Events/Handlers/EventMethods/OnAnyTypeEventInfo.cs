using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyType event.
/// Signature: Func<Battle, PokemonType[], Pokemon, TypesVoidUnion>
/// </summary>
public sealed record OnAnyTypeEventInfo : EventHandlerInfo
{
    public OnAnyTypeEventInfo(
        Func<Battle, PokemonType[], Pokemon, TypesVoidUnion> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.Type;
Prefix = EventPrefix.Any;
        Handler = handler;
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(PokemonType[]), typeof(Pokemon)];
        ExpectedReturnType = typeof(TypesVoidUnion);
    }
}