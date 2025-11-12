using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyType event (pokemon/ally-specific).
/// Triggered to get ally's type.
/// Signature: Func<Battle, PokemonType[], Pokemon, TypesVoidUnion>
/// </summary>
public sealed record OnAllyTypeEventInfo : EventHandlerInfo
{
    public OnAllyTypeEventInfo(
    Func<Battle, PokemonType[], Pokemon, TypesVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Type;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(PokemonType[]), typeof(Pokemon)];
        ExpectedReturnType = typeof(TypesVoidUnion);
  }
}