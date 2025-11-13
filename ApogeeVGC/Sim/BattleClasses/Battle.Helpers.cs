using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class Battle
{
    public static MoveCategory GetCategory(ActiveMove move)
    {
        return move.Category;
    }

    public int Randomizer(int baseDamage)
    {
        return Trunc(Trunc(baseDamage * (100 - Random(16))) / 100);
    }

    public int GetOverflowedTurnCount()
    {
        return Gen >= 8 ? (Turn - 1) % 256 : Turn - 1;
    }

    /// <summary>
    /// Checks if a relay variable is truthy (non-null, non-false, non-zero string).
    /// </summary>
    private static bool IsRelayVarTruthy(RelayVar? relayVar)
    {
        return relayVar switch
        {
            null => false,
            BoolRelayVar boolVar => boolVar.Value,
            IntRelayVar intVar => intVar.Value != 0,
            StringRelayVar strVar => !string.IsNullOrEmpty(strVar.Value),
            _ => true,
        };
    }

    /// <summary>
    /// Checks if an event is an "attacking event" that should be suppressed by Mold Breaker
    /// for custom abilities.
    /// </summary>
    private static bool IsAttackingEvent(EventId eventId, IEffect? sourceEffect)
    {
        // List of attacking events that should be suppressed
        HashSet<EventId> attackingEvents =
        [
            EventId.BeforeMove,
            EventId.BasePower,
            EventId.Immunity,
            EventId.RedirectTarget,
            EventId.Heal,
            EventId.SetStatus,
            EventId.CriticalHit,
            EventId.ModifyAtk,
            EventId.ModifyDef,
            EventId.ModifySpA,
            EventId.ModifySpD,
            EventId.ModifySpe,
            EventId.ModifyAccuracy,
            EventId.ModifyBoost,
            EventId.ModifyDamage,
            EventId.ModifySecondaries,
            EventId.ModifyWeight,
            EventId.TryAddVolatile,
            EventId.TryHit,
            EventId.TryHitSide,
            EventId.TryMove,
            EventId.Boost,
            EventId.DragOut,
            EventId.Effectiveness,
        ];

        if (attackingEvents.Contains(eventId))
        {
            return true;
        }

        // Damage from moves is also considered an attacking event
        if (eventId == EventId.Damage && sourceEffect?.EffectType == EffectType.Move)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the Field-prefixed variant of an event ID.
    /// For example, Residual becomes FieldResidual.
    /// Uses the pre-computed EventIdInfo metadata for fast lookup.
    /// </summary>
    private EventId GetFieldEventId(EventId baseEvent)
    {
        return Library.Events.TryGetValue(baseEvent, out EventIdInfo? eventInfo)
            ? eventInfo.FieldEventId
            : baseEvent;
    }

    /// <summary>
    /// Gets the Side-prefixed variant of an event ID.
    /// For example, Residual becomes SideResidual.
    /// Uses the pre-computed EventIdInfo metadata for fast lookup.
    /// </summary>
    private EventId GetSideEventId(EventId baseEvent)
    {
        return Library.Events.TryGetValue(baseEvent, out EventIdInfo? eventInfo)
            ? eventInfo.SideEventId
            : baseEvent;
    }
}