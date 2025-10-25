using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Actions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleAsync
{
    /// <summary>
    /// Sort a list, resolving speed ties the way the games do.
    /// 
    /// This uses a Selection Sort algorithm - not the fastest sort in general, but
    /// actually faster than QuickSort for small arrays like the ones SpeedSort is used for.
    /// More importantly, it makes it easiest to resolve speed ties properly through
    /// randomization via Prng.Shuffle().
    /// </summary>
    /// <typeparam name="T">Value that implements IPriorityComparison for sorting</typeparam>
    /// <param name="list">List to sort in-place</param>
    /// <param name="comparator">Comparison function (defaults to ComparePriority)</param>
    public void SpeedSort<T>(List<T> list, Func<T, T, int>? comparator = null)
        where T : IPriorityComparison
    {
        // Default to ComparePriority if no comparator provided
        comparator ??= (a, b) => ComparePriority(a, b);

        // Nothing to sort for lists with less than 2 elements
        if (list.Count < 2) return;

        int sorted = 0;

        // Selection Sort with speed tie resolution
        while (sorted + 1 < list.Count)
        {
            // Start with the first unsorted element
            List<int> nextIndexes = [sorted];

            // Find all elements that should come next (including ties)
            for (int i = sorted + 1; i < list.Count; i++)
            {
                int delta = comparator(list[nextIndexes[0]], list[i]);

                switch (delta)
                {
                    case < 0:
                        // Current element is already better, skip
                        continue;
                    case > 0:
                        // Found a better element, start new list
                        nextIndexes = [i];
                        break;
                    // delta == 0
                    default:
                        // Speed tie - add to list of tied elements
                        nextIndexes.Add(i);
                        break;
                }
            }

            // Place the next elements in their sorted positions
            for (int i = 0; i < nextIndexes.Count; i++)
            {
                int index = nextIndexes[i];
                if (index != sorted + i)
                {
                    // Swap elements into place
                    // nextIndexes is guaranteed to be in order, so it will never have
                    // been disturbed by an earlier swap
                    (list[sorted + i], list[index]) = (list[index], list[sorted + i]);
                }
            }

            // If there are multiple elements with the same priority (speed ties),
            // shuffle them randomly to fairly resolve the tie
            if (nextIndexes.Count > 1)
            {
                Prng.Shuffle(list, sorted, sorted + nextIndexes.Count);
            }

            sorted += nextIndexes.Count;
        }
    }

    public EventListener ResolvePriority(EventListenerWithoutPriority h, EventId callbackName)
    {
        // Get event metadata from Library
        EventIdInfo eventInfo = Library.Events[callbackName];

        // Look up order/priority/subOrder from the effect using the delegate system
        // These would need to be added to IEffect interface or accessed via reflection once
        IntFalseUnion? order = h.Effect.GetOrder(callbackName);
        int? priority = h.Effect.GetPriority(callbackName);
        int? subOrder = h.Effect.GetSubOrder(callbackName);

        // Calculate default subOrder if not set
        if (subOrder == 0)
        {
            subOrder = CalculateDefaultSubOrder(h);
        }

        // Determine effectOrder based on event type
        int effectOrder = eventInfo.UsesEffectOrder
            ? (h.State?.EffectOrder ?? 0)
            : 0;

        // Calculate speed if needed
        int speed = 0;
        if (eventInfo.UsesSpeed && h.EffectHolder is PokemonEffectHolder pokemonEffectHolder)
        {
            Pokemon pokemon = pokemonEffectHolder.Pokemon;
            speed = pokemon.Speed;

            // Special case for Magic Bounce
            if (h.Effect.EffectType == EffectType.Ability &&
                h.Effect is Ability { Id: AbilityId.MagicBounce } &&
                callbackName == EventId.AllyTryHitSide)
            {
                speed = pokemon.GetStat(StatIdExceptHp.Spe, unmodified: true, unboosted: true);
            }

            // Apply fractional speed adjustment for switch-in events
            if (eventInfo.UsesFractionalSpeed)
            {
                int fieldPositionValue = pokemon.Side.N * Sides.Count + pokemon.Position;
                speed -= SpeedOrder.IndexOf(fieldPositionValue) / (ActivePerHalf * 2);
            }
        }

        return new EventListener
        {
            Effect = h.Effect,
            Target = h.Target,
            Index = h.Index,
            Callback = h.Callback,
            State = h.State,
            End = h.End,
            EndCallArgs = h.EndCallArgs,
            EffectHolder = h.EffectHolder,
            Order = order ?? IntFalseUnion.FromFalse(),
            Priority = priority ?? 0,
            Speed = speed,
            SubOrder = subOrder ?? 0,
            EffectOrder = effectOrder,
        };
    }

    public void GetActionSpeed(IAction action)
    {
        // Only process move actions for priority calculation
        if (action is MoveAction moveAction)
        {
            var move = moveAction.Move.ToActiveMove();

            // Get base priority from the original move data (not the active move)
            // This ensures abilities like Prankster only apply once, not repeatedly
            int priority = Library.Moves[move.Id].Priority;

            // Run ModifyPriority events to allow effects to change priority
            // SingleEvent: for move-specific priority changes (e.g., Grassy Glide in Grassy Terrain)
            RelayVar? singleEventResult = SingleEvent(
                EventId.ModifyPriority,
                move,
                null,
                moveAction.Pokemon,
                null,
                null,
                priority
            );

            if (singleEventResult is IntRelayVar singlePriority)
            {
                priority = singlePriority.Value;
            }

            // RunEvent: for Pokemon-based priority changes (e.g., Prankster ability)
            RelayVar? runEventResult = RunEvent(
                EventId.ModifyPriority,
                moveAction.Pokemon,
                null,
                move,
                priority
            );

            if (runEventResult is IntRelayVar modifiedPriority)
            {
                priority = modifiedPriority.Value;
            }

            // Set the action's priority (includes fractional priority for tie-breaking)
            moveAction.Priority = priority + moveAction.FractionalPriority;

            // In Gen 6+, update the move's priority property
            // This is used by Quick Guard to block moves with artificially enhanced priority
            if (Gen > 5)
            {
                moveAction.Move.Priority = priority;
            }
        }

        // Get the Pokemon's action speed (factors in speed stat, paralysis, etc.)
        // The other Action types have constant speed values so do not need to be set.
        switch (action)
        {
            case SwitchAction switchAction:
                switchAction.Speed = switchAction.Pokemon.GetActionSpeed();
                break;
            case PokemonAction pokemonAction:
                pokemonAction.Speed = pokemonAction.Pokemon.GetActionSpeed();
                break;
        }
    }

    /// <summary>
    /// The default sort order for actions, but also event listeners.
    /// 
    /// 1. Order, low to high (default last)
    /// 2. Priority, high to low (default 0)
    /// 3. Speed, high to low (default 0)
    /// 4. SubOrder, low to high (default 0)
    /// 5. EffectOrder, low to high (default 0)
    /// 
    /// This is a static comparison function that doesn't reference battle state.
    /// </summary>
    public static int ComparePriority(IPriorityComparison a, IPriorityComparison b)
    {
        // 1. Order comparison (lower values first, Max = last)
        // ActionOrder.Max represents items without explicit order
        int orderResult = a.Order.CompareTo(b.Order);
        if (orderResult != 0) return orderResult;

        // 2. Priority comparison (higher values first)
        int priorityResult = b.Priority.CompareTo(a.Priority); // Reversed for descending
        if (priorityResult != 0) return priorityResult;

        // 3. Speed comparison (higher values first)
        int speedResult = b.Speed.CompareTo(a.Speed); // Reversed for descending
        if (speedResult != 0) return speedResult;

        // 4. SubOrder comparison (lower values first)
        int subOrderResult = a.SubOrder.CompareTo(b.SubOrder);
        if (subOrderResult != 0) return subOrderResult;

        // 5. EffectOrder comparison (lower values first)
        return a.EffectOrder.CompareTo(b.EffectOrder);
    }

    /// <summary>
    /// Compares two event handlers for redirect order.
    /// Used to determine which redirect effect triggers first.
    /// 
    /// Order:
    /// 1. Priority (higher first)
    /// 2. Speed (higher first)
    /// 3. Ability state effect order (lower first, only if both are abilities)
    /// </summary>
    public static int CompareRedirectOrder(IPriorityComparison a, IPriorityComparison b)
    {
        // 1. Priority comparison (higher values first)
        int priorityResult = b.Priority.CompareTo(a.Priority); // Reversed for descending
        if (priorityResult != 0) return priorityResult;

        // 2. Speed comparison (higher values first)
        int speedResult = b.Speed.CompareTo(a.Speed); // Reversed for descending
        if (speedResult != 0) return speedResult;

        // 3. Ability state effect order comparison (lower values first, only if both have ability states)
        // This is used to break ties between abilities that redirect moves
        if (a is EventListener aListener && b is EventListener bListener)
        {
            // Check if both handlers are from abilities
            bool aHasAbilityState = aListener.Effect.EffectType == EffectType.Ability &&
                                    aListener is { EffectHolder: PokemonEffectHolder, State: not null };
            bool bHasAbilityState = bListener.Effect.EffectType == EffectType.Ability &&
                                    bListener is { EffectHolder: PokemonEffectHolder, State: not null };

            if (aHasAbilityState && bHasAbilityState)
            {
                // Negative sign to reverse the order (lower effectOrder comes first)
                return -(bListener.State!.EffectOrder.CompareTo(aListener.State!.EffectOrder));
            }
        }

        return 0;
    }

    /// <summary>
    /// Compares two event handlers for left-to-right order.
    /// Used for processing actions in field position order (left to right).
    /// 
    /// Order:
    /// 1. Order (higher first) - reversed comparison, with false treated as maximum value
    /// 2. Priority (higher first)
    /// 3. Index (higher first) - reversed comparison for left-to-right processing
    /// </summary>
    public static int CompareLeftToRightOrder(IPriorityComparison a, IPriorityComparison b)
    {
        // 1. Order comparison (higher values first, but treating false as lowest priority)
        // The negative sign reverses the comparison: higher order values come first
        // IntFalseUnion.CompareTo handles this: false > int values (false has lower priority)
        int orderResult = -(b.Order.CompareTo(a.Order));
        if (orderResult != 0) return orderResult;

        // 2. Priority comparison (higher values first)
        int priorityResult = b.Priority.CompareTo(a.Priority); // Reversed for descending
        if (priorityResult != 0) return priorityResult;

        // 3. Index comparison (higher values first for left-to-right)
        // This processes Pokemon from left to right on the field
        // Both handlers need to be EventListeners with valid indices
        if (a is EventListener aListener && b is EventListener bListener)
        {
            int aIndex = aListener.Index ?? 0;
            int bIndex = bListener.Index ?? 0;
            int indexResult = -(bIndex.CompareTo(aIndex));
            if (indexResult != 0) return indexResult;
        }

        return 0;
    }

    private int CalculateDefaultSubOrder(EventListenerWithoutPriority listener)
    {
        // Effect type hierarchy for subOrder
        int subOrder = listener.Effect.EffectType switch
        {
            EffectType.Condition => 2,
            EffectType.Weather => 5,
            EffectType.Format => 5,
            EffectType.Rule => 5,
            EffectType.Ruleset => 5,
            EffectType.Ability => 7,
            EffectType.Item => 8,
            _ => 0,
        };

        // Refine for conditions
        if (listener.Effect.EffectType == EffectType.Condition && listener.State?.Target != null)
        {
            subOrder = listener.State.Target switch
            {
                SideEffectStateTarget when listener.State.IsSlotCondition == true => 3,  // Slot condition
                SideEffectStateTarget => 4,                                       // Side condition
                FieldEffectStateTarget => 5,                                      // Field condition
                _ => subOrder,
            };
        }

        // Special abilities
        if (listener.Effect is Ability ability)
        {
            subOrder = ability.Id switch
            {
                AbilityId.PoisonTouch => 6,
                AbilityId.PerishBody => 6,
                AbilityId.Stall => 9,
                _ => subOrder,
            };
        }

        return subOrder;
    }
}