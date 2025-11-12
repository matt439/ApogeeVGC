using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyDeductPp event (pokemon/ally-specific).
/// Triggered when deducting ally's PP.
/// Signature: Func<Battle, Pokemon, Pokemon, IntVoidUnion>
/// </summary>
public sealed record OnAllyDeductPpEventInfo : EventHandlerInfo
{
    public OnAllyDeductPpEventInfo(
    Func<Battle, Pokemon, Pokemon, IntVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.DeductPp;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon)];
        ExpectedReturnType = typeof(IntVoidUnion);
  }
}