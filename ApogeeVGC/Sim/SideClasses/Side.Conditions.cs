using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.SideClasses;

public partial class Side
{
    public bool AddSideCondition(ConditionId status, Pokemon? source = null, IEffect? sourceEffect = null)
    {
        Condition condition = Battle.Library.Conditions[status];
        return AddSideCondition(condition, source, sourceEffect);
    }

    public bool AddSideCondition(Condition status, Pokemon? source = null, IEffect? sourceEffect = null)
    {
        // Step 1: Source resolution
        if (source == null && Battle is { Event.Target: PokemonSingleEventTarget eventTarget })
        {
            source = eventTarget.Pokemon;
        }
        if (source == null)
            throw new InvalidOperationException("Setting side condition without a source");

        // Step 3: Restart handling
        if (SideConditions.TryGetValue(status.Id, out EffectState? condition))
        {
            // If no onSideRestart handler, return false
            if (status.OnRestart == null)
                return false;

            // Call the restart handler
            RelayVar? restartResult = Battle.SingleEvent(EventId.SideRestart, status, condition, this, source, sourceEffect);
            return restartResult is BoolRelayVar { Value: true };
        }

        // Step 4: Create EffectState
        EffectState effectState = Battle.InitEffectState(status.Id, source, source.GetSlot(), status.Duration);

        // Step 5: Duration callback
        if (status.DurationCallback != null)
        {
            Pokemon? firstActive = Active.FirstOrDefault(p => p != null);
            if (firstActive is null)
            {
                throw new InvalidOperationException("Side.Active has no non-null Pokemon.");
            }

            var durationHandler =
                (Func<Battle, Pokemon, Pokemon, IEffect?, int>)status.DurationCallback
                    .GetDelegateOrThrow();
            effectState.Duration = durationHandler(Battle, firstActive, source, sourceEffect);
        }

        SideConditions[status.Id] = effectState;

        // Step 6: SideStart event
        RelayVar? sideStartResult = Battle.SingleEvent(EventId.SideStart, status, effectState, this, source, sourceEffect);
        if (sideStartResult is not BoolRelayVar { Value: true })
        {
            SideConditions.Remove(status.Id);
            return false;
        }

        // Step 7: Run SideConditionStart event
        Battle.RunEvent(EventId.SideConditionStart, this, source, status);

        // Step 8: Success
        return true;
    }

    public Condition? GetSideCondition(ConditionId status)
    {
        Condition condition = Battle.Library.Conditions[status];
        return !SideConditions.ContainsKey(condition.Id) ? null : condition;
    }

    public Condition? GetSideCondition(Condition status)
    {
        return !SideConditions.ContainsKey(status.Id) ? null : status;
    }

    public EffectState? GetSideConditionData(ConditionId status)
    {
        Condition condition = Battle.Library.Conditions[status];
        return SideConditions.GetValueOrDefault(condition.Id);
    }

    public EffectState? GetSideConditionData(Condition status)
    {
        return SideConditions.GetValueOrDefault(status.Id);
    }

    public bool RemoveSideCondition(ConditionId status)
    {
        Condition condition = Battle.Library.Conditions[status];
        if (!SideConditions.TryGetValue(condition.Id, out EffectState? sideCondition)) return false;
        Battle.SingleEvent(EventId.SideEnd, condition, sideCondition, this);
        SideConditions.Remove(condition.Id);
        return true;
    }

    public bool RemoveSideCondition(Condition status)
    {
        return RemoveSideCondition(status.Id);
    }

    public bool AddSlotCondition(PokemonIntUnion target, ConditionId status, Pokemon? source = null,
        IEffect? sourceEffect = null)
    {
        Condition condition = Battle.Library.Conditions[status];
        return AddSlotCondition(target, condition, source, sourceEffect);
    }

    public bool AddSlotCondition(PokemonIntUnion target, Condition status, Pokemon? source = null,
        IEffect? sourceEffect = null)
    {
        // Step 1: Source resolution
        if (source == null && Battle is { Event.Target: PokemonSingleEventTarget eventTarget })
        {
            source = eventTarget.Pokemon;
        }
        if (source == null)
            throw new InvalidOperationException("Setting slot condition without a source");

        // Step 2: Convert target to position if it's a Pokemon
        int targetSlot = target switch
        {
            PokemonPokemonIntUnion pokemon => pokemon.Pokemon.Position,
            IntPokemonIntUnion intValue => intValue.Value,
            _ => throw new InvalidOperationException("Invalid target type"),
        };

        // Step 3: Validate slot index
        if (targetSlot < 0 || targetSlot >= SlotConditions.Count)
        {
            throw new InvalidOperationException($"Invalid slot index: {targetSlot}");
        }

        // Step 4: Restart handling - if condition already exists
        if (SlotConditions[targetSlot].TryGetValue(status.Id, out EffectState? condition))
        {
            // If no onRestart handler, return false
            if (status.OnRestart == null)
                return false;

            // Call the restart handler
            RelayVar? restartResult = Battle.SingleEvent(EventId.Restart, status, condition, this, source, sourceEffect);
            return restartResult is BoolRelayVar { Value: true };
        }

        // Step 5: Create EffectState
        EffectState conditionState = Battle.InitEffectState(status.Id, source, source.GetSlot(), status.Duration);
        conditionState.IsSlotCondition = true;

        // Step 6: Duration callback
        if (status.DurationCallback != null)
        {
            Pokemon? firstActive = Active.FirstOrDefault(p => p != null);
            if (firstActive is null)
            {
                throw new InvalidOperationException("Side.Active has no non-null Pokemon.");
            }
            var durationHandler = (Func<Battle, Pokemon, Pokemon, IEffect?, int>)status.DurationCallback.
                GetDelegateOrThrow();
            conditionState.Duration = durationHandler(Battle, firstActive, source, sourceEffect);
        }

        SlotConditions[targetSlot][status.Id] = conditionState;

        // Step 7: Start event
        RelayVar? startResult = Battle.SingleEvent(EventId.Start, status, conditionState,
            GetActiveAt(targetSlot), source, sourceEffect);

        if (startResult is BoolRelayVar { Value: true }) return true;
        SlotConditions[targetSlot].Remove(status.Id);
        return false;
    }

    public IEffect? GetSlotCondition(PokemonIntUnion target, Condition status)
    {
        // Convert target to position if it's a Pokemon
        int targetSlot = target switch
        {
            PokemonPokemonIntUnion pokemon => pokemon.Pokemon.Position,
            IntPokemonIntUnion intValue => intValue.Value,
            _ => throw new InvalidOperationException("Invalid target type"),
        };

        // Validate slot index
        if (targetSlot < 0 || targetSlot >= SlotConditions.Count)
        {
            throw new InvalidOperationException($"Invalid slot index: {targetSlot}");
        }

        // Check if condition exists in the slot
        if (!SlotConditions[targetSlot].ContainsKey(status.Id))
            return null;

        return status;
    }

    public IEffect? GetSlotCondition(PokemonIntUnion target, ConditionId status)
    {
        Condition condition = Battle.Library.Conditions[status];
        return GetSlotCondition(target, condition);
    }

    public bool RemoveSlotCondition(PokemonIntUnion target, Condition status)
    {
        // Convert target to position if it's a Pokemon
        int targetSlot = target switch
        {
            PokemonPokemonIntUnion pokemon => pokemon.Pokemon.Position,
            IntPokemonIntUnion intValue => intValue.Value,
            _ => throw new InvalidOperationException("Invalid target type"),
        };

        // Validate slot index
        if (targetSlot < 0 || targetSlot >= SlotConditions.Count)
        {
            throw new InvalidOperationException($"Invalid slot index: {targetSlot}");
        }

        // Check if condition exists in the slot
        if (!SlotConditions[targetSlot].TryGetValue(status.Id, out EffectState? conditionState))
            return false;

        // Trigger End event
        Battle.SingleEvent(EventId.End, status, conditionState, GetActiveAt(targetSlot));

        // Remove the condition
        SlotConditions[targetSlot].Remove(status.Id);

        return true;
    }

    public bool RemoveSlotCondition(PokemonIntUnion target, ConditionId status)
    {
        Condition condition = Battle.Library.Conditions[status];
        return RemoveSlotCondition(target, condition);
    }
}