using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnySetAbility event.
/// Signature: Func<Battle, Ability, Pokemon, Pokemon, IEffect, bool?>
/// </summary>
public sealed record OnAnySetAbilityEventInfo : EventHandlerInfo
{
    public OnAnySetAbilityEventInfo(
        Func<Battle, Ability, Pokemon, Pokemon, IEffect, bool?> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.SetAbility;
Prefix = EventPrefix.Any;
        Handler = handler;
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(Ability), typeof(Pokemon), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(bool?);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = true;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAnySetAbilityEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SetAbility;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAnySetAbilityEventInfo Create(
        Func<Battle, Ability, Pokemon, Pokemon, IEffect, bool?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAnySetAbilityEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetRelayVarEffect<Ability>(),
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetSourceEffect<IEffect>()
                );
                if (result == null) return null;
                return new BoolRelayVar(result.Value);
            },
            priority,
            usesSpeed
        );
    }
}