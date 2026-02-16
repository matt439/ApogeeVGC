using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSetAbility event.
/// Triggered when an ability is set on a Pokemon.
/// Signature: (Battle battle, Ability ability, Pokemon target, Pokemon source, IEffect effect) => VoidReturn?
/// Returns null to block the ability change, or VoidReturn to allow it.
/// </summary>
public sealed record OnSetAbilityEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSetAbility event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnSetAbilityEventInfo(
        Func<Battle, Ability, Pokemon, Pokemon, IEffect, VoidReturn?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SetAbility;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
            typeof(Battle),
            typeof(Ability),
            typeof(Pokemon),
            typeof(Pokemon),
            typeof(IEffect),
        ];
        ExpectedReturnType = typeof(VoidReturn);

        // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = true; // VoidReturn? is nullable to allow blocking ability changes

        // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnSetAbilityEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SetAbility;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSetAbilityEventInfo Create(
        Func<Battle, Ability, Pokemon, Pokemon, IEffect, VoidReturn?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSetAbilityEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetRelayVarEffect<Ability>(),
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
