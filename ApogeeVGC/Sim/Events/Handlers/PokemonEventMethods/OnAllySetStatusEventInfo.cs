using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllySetStatus event (pokemon/ally-specific).
/// Triggered when setting ally status.
/// Signature: Func<Battle, Condition, Pokemon, Pokemon, IEffect, PokemonFalseVoidUnion?>
/// </summary>
public sealed record OnAllySetStatusEventInfo : EventHandlerInfo
{
    public OnAllySetStatusEventInfo(
    Func<Battle, Condition, Pokemon, Pokemon, IEffect, PokemonFalseVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SetStatus;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Condition), typeof(Pokemon), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(PokemonFalseVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false, false };
        ReturnTypeNullable = true;
    
    // Validate configuration
        ValidateConfiguration();
  }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAllySetStatusEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SetStatus;
        Prefix = EventPrefix.Ally;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAllySetStatusEventInfo Create(
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, PokemonFalseVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAllySetStatusEventInfo(
            context =>
            {
                var result = handler(
                    context.Battle,
                    context.GetRelayVarEffect<Condition>(),
                    context.GetTargetOrSourcePokemon(),
                    context.GetSourceOrTargetPokemon(),
                    context.GetSourceEffect<IEffect>()
                );
                return result switch
                {
                    PokemonPokemonFalseVoidUnion p => new PokemonRelayVar(p.Pokemon),
                    FalsePokemonFalseVoidUnion => new BoolRelayVar(false),
                    VoidPokemonFalseVoidUnion => null,
                    null => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}