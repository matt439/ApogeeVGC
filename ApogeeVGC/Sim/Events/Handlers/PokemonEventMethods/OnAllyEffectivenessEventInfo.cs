using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyEffectiveness event (pokemon/ally-specific).
/// Triggered to modify type effectiveness against ally.
/// Signature: Func<Battle, int, Pokemon?, PokemonType, ActiveMove, IntVoidUnion>
/// </summary>
public sealed record OnAllyEffectivenessEventInfo : EventHandlerInfo
{
    public OnAllyEffectivenessEventInfo(
        Func<Battle, int, Pokemon?, PokemonType, ActiveMove, IntVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Effectiveness;
        Prefix = EventPrefix.Ally;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(PokemonType), typeof(ActiveMove)];
        ExpectedReturnType = typeof(IntVoidUnion);

        // Nullability: Battle (non-null), int (non-null), Pokemon (nullable), PokemonType (non-null), ActiveMove (non-null)
        ParameterNullability = new[] { false, false, true, false, false };
        ReturnTypeNullable = false; // IntVoidUnion is a struct

        // Validate configuration
        ValidateConfiguration();
    }
}