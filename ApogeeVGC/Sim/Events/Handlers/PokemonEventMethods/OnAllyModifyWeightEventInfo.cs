using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyModifyWeight event (pokemon/ally-specific).
/// Triggered to modify ally's weight.
/// Signature: Func<Battle, int, Pokemon, IntVoidUnion>
/// </summary>
public sealed record OnAllyModifyWeightEventInfo : EventHandlerInfo
{
    public OnAllyModifyWeightEventInfo(
    Func<Battle, int, Pokemon, IntVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyWeight;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon)];
        ExpectedReturnType = typeof(IntVoidUnion);
  }
}