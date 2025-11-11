using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceAfterBoost event.
/// Signature: Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>
/// </summary>
public sealed record OnSourceAfterBoostEventInfo : EventHandlerInfo
{
    public OnSourceAfterBoostEventInfo(
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect> handler,
    int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.AfterBoost;
        Prefix = EventPrefix.Source;
 Handler = handler;
 Priority = priority;
UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(SparseBoostsTable), typeof(Pokemon), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(void);
    }
}