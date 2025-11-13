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
public OnHitSideEventInfo(
        Func<Battle, Side, Pokemon, ActiveMove, BoolEmptyVoidUnion?> handler,
        int? priority = null,
   bool usesSpeed = true)
    {
    Id = EventId.HitSide;
        Prefix = EventPrefix.None;
        Handler = handler;
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
}