using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyAfterFaint event (pokemon/ally-specific).
/// Triggered after ally faints.
/// Signature: Action<Battle, int, Pokemon, Pokemon, IEffect>
/// </summary>
public sealed record OnAllyAfterFaintEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, RelayVar (int damage), TargetPokemon, SourcePokemon, SourceEffect
    /// </summary>
    public OnAllyAfterFaintEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterFaint;
        Prefix = EventPrefix.Ally;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnAllyAfterFaintEventInfo Create(
        Action<Battle, int, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAllyAfterFaintEventInfo(
            context =>
            {
                handler(
                    context.Battle,
                    context.GetIntRelayVar(),
                    context.GetTargetOrSourcePokemon(),
                    context.GetSourceOrTargetPokemon(),
                    context.GetSourceEffect<IEffect>()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
