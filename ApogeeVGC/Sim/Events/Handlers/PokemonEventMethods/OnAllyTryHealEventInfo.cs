using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyTryHeal event (pokemon/ally-specific).
/// Triggered when trying to heal ally.
/// Signature: Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>
/// </summary>
public sealed record OnAllyTryHealEventInfo : EventHandlerInfo
{
    public OnAllyTryHealEventInfo(
    Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TryHeal;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(IntBoolVoidUnion);
  }
}