using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for OnHitSide event (move-specific).
/// Triggered when a move hits a side.
/// Signature: Func<Battle, Side, Pokemon, ActiveMove, BoolEmptyVoidUnion?>
/// </summary>
public sealed record OnHitSideEventInfo : EventHandlerInfo
{
[Obsolete("Use Create factory method instead.")]
public OnHitSideEventInfo(
        Func<Battle, Side, Pokemon, ActiveMove, BoolEmptyVoidUnion?> handler,
        int? priority = null,
   bool usesSpeed = true)
    {
    Id = EventId.HitSide;
        Prefix = EventPrefix.None;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
        Priority = priority;
      UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Side), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(BoolEmptyVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    public OnHitSideEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.HitSide;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    public static OnHitSideEventInfo Create(
        Func<Battle, Side, Pokemon, ActiveMove, BoolEmptyVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnHitSideEventInfo(
            context =>
            {
                var result = handler(
                    context.Battle,
                    context.GetTargetSide(),
                    context.GetSourceOrTargetPokemon(),
                    context.GetMove()
                );
                if (result == null) return null;
                return result switch
                {
                    BoolBoolEmptyVoidUnion b => new BoolRelayVar(b.Value),
                    EmptyBoolEmptyVoidUnion => null,
                    VoidUnionBoolEmptyVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
