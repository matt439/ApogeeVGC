using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyNegateImmunity event (pokemon/ally-specific).
/// Triggered to negate ally immunity.
/// Signature: Func<Battle, Pokemon, PokemonType?, BoolVoidUnion>
/// </summary>
public sealed record OnAllyNegateImmunityEventInfo : EventHandlerInfo
{
    public OnAllyNegateImmunityEventInfo(
    Func<Battle, Pokemon, PokemonType?, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.NegateImmunity;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(PokemonType)];
        ExpectedReturnType = typeof(BoolVoidUnion);
  }
}