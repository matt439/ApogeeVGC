using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

public class TargetRelay
{
    public required Pokemon Target { get; set; }
}

/// <summary>
/// Event handler info for OnModifyTarget event (move-specific).
/// Triggered to modify move target.
/// Signature: Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove>
/// </summary>
public sealed record OnModifyTargetEventInfo : EventHandlerInfo
{
    public OnModifyTargetEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyTarget;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    public static OnModifyTargetEventInfo Create(
        Action<Battle, TargetRelay, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnModifyTargetEventInfo(
            context =>
            {
                var relayPokemon = context.RelayVar is PokemonRelayVar prv ? prv.Pokemon : context.GetTargetOrSourcePokemon();
                var targetRelay = new TargetRelay { Target = relayPokemon };
                handler(
                    context.Battle,
                    targetRelay,
                    context.GetSourceOrTargetPokemon(),
                    context.GetTargetOrSourcePokemon(),
                    context.GetMove()
                );
                return new PokemonRelayVar(targetRelay.Target);
            },
            priority,
            usesSpeed
        );
    }
}
