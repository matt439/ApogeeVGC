using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;

public sealed record OnCopyEventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public OnCopyEventInfo(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Copy;
        Prefix = EventPrefix.None;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    public OnCopyEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Copy;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    public static OnCopyEventInfo Create(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnCopyEventInfo(
            context =>
            {
                handler(
                    context.Battle,
                    context.GetTargetOrSourcePokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
