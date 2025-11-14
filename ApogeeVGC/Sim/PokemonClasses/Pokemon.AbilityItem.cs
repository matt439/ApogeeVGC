using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.PokemonClasses;

public partial class Pokemon
{
    public bool EatItem(bool force = false, Pokemon? source = null, IEffect? sourceEffect = null)
    {
        // Early validation checks
        if (Item == ItemId.None || ItemState.KnockedOff == true)
            return false;

        // Check HP unless it's a retaliatory berry
        if ((Hp <= 0 && Item != ItemId.JabocaBerry && Item != ItemId.RowapBerry) || !IsActive)
            return false;

        // Resolve source and sourceEffect from battle event if not provided
        if (sourceEffect == null && Battle.Event is not null)
        {
            sourceEffect = Battle.Event.Effect;
        }

        if (source == null && Battle.Event?.Target is PokemonSingleEventTarget pset)
        {
            source = pset.Pokemon;
        }

        // Get the actual item object
        Item item = GetItem();

        // Prevent eating wrong item when triggered by another item
        if (sourceEffect?.EffectType == EffectType.Item &&
            Item != ((Item)sourceEffect).Id &&
            source == this)
        {
            // If an item is telling us to eat it but we aren't holding it, 
            // we probably shouldn't eat what we are holding
            return false;
        }

        // Run UseItem and TryEatItem events to check if eating should proceed
        RelayVar? useItemEvent = Battle.RunEvent(EventId.UseItem, this, null,
            null, item);
        bool canUseItem = useItemEvent is not BoolRelayVar { Value: false };

        bool canEatItem = force;
        if (!force)
        {
            RelayVar? tryEatEvent = Battle.RunEvent(EventId.TryEatItem, this, null,
                null, item);
            canEatItem = tryEatEvent is not BoolRelayVar { Value: false };
        }

        if (!canUseItem || !canEatItem) return false;

        // Display item consumption message
        if (Battle.DisplayUi)
        {
            Battle.Add("-enditem", this, item, "[eat]");
        }

        // Trigger the Eat event on the item
        Battle.SingleEvent(EventId.Eat, item, ItemState, this,
            SingleEventSource.FromNullablePokemon(source), sourceEffect);

        // Run EatItem event for other effects to respond
        Battle.RunEvent(EventId.EatItem, this,
            RunEventSource.FromNullablePokemon(source), sourceEffect, item);

        // Handle staleness tracking for restorative berries
        if (RestorativeBerries.Contains(item.Id))
        {
            switch (PendingStaleness)
            {
                case StalenessId.Internal:
                    // Only set to internal if not already external
                    if (Staleness != StalenessId.External)
                    {
                        Staleness = StalenessId.Internal;
                    }
                    break;

                case StalenessId.External:
                    Staleness = StalenessId.External;
                    break;
            }
            PendingStaleness = null;
        }

        // Clean up: move item to lastItem and clear current item
        LastItem = Item;
        Item = ItemId.None;
        ItemState = Battle.InitEffectState();

        // Set consumption flags
        UsedItemThisTurn = true;
        AteBerry = true;

        // Trigger AfterUseItem event
        Battle.RunEvent(EventId.AfterUseItem, this, null, null, item);

        return true;
    }

    public bool UseItem(Pokemon? source = null, IEffect? sourceEffect = null)
    {
        // Get item to check if it's a gem
        Item item = GetItem();

        // Early validation checks
        // Allow gems to be used when fainted, but not other items
        if ((Hp <= 0 && !item.IsGem) || !IsActive)
            return false;

        // Check that item exists and hasn't been knocked off
        if (Item == ItemId.None || ItemState.KnockedOff == true)
            return false;

        // Resolve source and sourceEffect from battle event if not provided
        if (sourceEffect == null && Battle.Event is not null)
        {
            sourceEffect = Battle.Event.Effect;
        }

        if (source == null && Battle.Event?.Target is PokemonSingleEventTarget pset)
        {
            source = pset.Pokemon;
        }

        // Prevent using wrong item when triggered by another item
        if (sourceEffect?.EffectType == EffectType.Item &&
            Item != ((Item)sourceEffect).Id &&
            source == this)
        {
            // If an item is telling us to use it but we aren't holding it, 
            // we probably shouldn't use what we are holding
            return false;
        }

        // Run UseItem event for validation
        RelayVar? useItemEvent = Battle.RunEvent(EventId.UseItem, this, null,
            null, item);
        if (useItemEvent is BoolRelayVar { Value: false })
        {
            return false;
        }

        // Display appropriate end item message based on item type
        if (Battle.DisplayUi)
        {
            switch (item.Id)
            {
                case ItemId.RedCard:
                    // Red Card shows the source that triggered it
                    Battle.Add("-enditem", this, item, $"[of] {source}");
                    break;

                default:
                    if (item.IsGem)
                    {
                        // Gems show "[from] gem" tag
                        Battle.Add("-enditem", this, item, "[from] gem");
                    }
                    else
                    {
                        // Standard item consumption message
                        Battle.Add("-enditem", this, item);
                    }
                    break;
            }
        }

        // Apply stat boosts if the item provides them
        if (item.Boosts is SparseBoostsTableItemBoosts boosts)
        {
            Battle.Boost(boosts.Table, this, source, item);
        }

        // Trigger the Use event on the item
        Battle.SingleEvent(EventId.Use, item, ItemState, this,
            SingleEventSource.FromNullablePokemon(source), sourceEffect);

        // Clean up: move item to lastItem and clear current item
        LastItem = Item;
        Item = ItemId.None;
        ItemState = Battle.InitEffectState();

        // Set usage flag
        UsedItemThisTurn = true;

        // Trigger AfterUseItem event
        Battle.RunEvent(EventId.AfterUseItem, this, null, null, item);

        return true;
    }

    public ItemFalseUnion TakeItem(Pokemon? source = null)
    {
        // Check if item exists and hasn't been knocked off
        if (Item == ItemId.None || ItemState.KnockedOff == true)
            return ItemFalseUnion.FromFalse();

        // Default source to this Pokemon if not provided
        source ??= this;

        // Generation 4 special rule: Multitype ability (Arceus) protection
        if (Battle.Gen == 4)
        {
            // Arceus (Multitype) cannot have items taken
            if (Ability == AbilityId.Multitype)
                return ItemFalseUnion.FromFalse();

            // Cannot take items from Arceus (Multitype)
            if (source.Ability == AbilityId.Multitype)
                return ItemFalseUnion.FromFalse();
        }

        // Get the actual item object
        Item item = GetItem();

        // Run TakeItem event to check if taking should proceed
        RelayVar? takeItemEvent = Battle.RunEvent(EventId.TakeItem, this, source,
            null, item);

        if (takeItemEvent is BoolRelayVar { Value: false })
        {
            return ItemFalseUnion.FromFalse();
        }

        // Store old item state for End event
        EffectState oldItemState = ItemState;

        // Clear the item
        Item = ItemId.None;

        // Clear the item state
        ItemState = Battle.InitEffectState();

        // Clear pending staleness
        PendingStaleness = null;

        // Trigger End event on the item
        Battle.SingleEvent(EventId.End, item, oldItemState, new PokemonSingleEventTarget(this));

        // Trigger AfterTakeItem event
        Battle.RunEvent(EventId.AfterTakeItem, this, null, null, item);

        // Return the taken item
        return item;
    }

    public bool SetItem(ItemId item, Pokemon? source = null, IEffect? effect = null)
    {
        // Early exit if Pokemon is fainted or not active
  if (Hp <= 0 || !IsActive) return false;

    // Check if item was knocked off (except for Recycle move)
 if (ItemState.KnockedOff == true && 
            !(effect is ActiveMove { Id: MoveId.Recycle }))
   {
            return false;
        }

        // Clear knocked off flag
        ItemState.KnockedOff = null;

   // Default sourceEffect if not provided
        effect ??= Battle.Effect;

        // Determine current effect ID
EffectStateId effectId = effect.EffectStateId;

        // Check if this is a restorative berry (like Leppa Berry)
        // Note: You'll need to define RESTORATIVE_BERRIES set/list somewhere
        if (RestorativeBerries.Contains(ItemId.LeppaBerry))
        {
            // Check if item was inflicted by Trick or Switcheroo
            bool inflicted = effectId is MoveEffectStateId { MoveId: MoveId.Trick or MoveId.Switcheroo };

            // Check if it's external (from opponent)
            bool external = inflicted && source != null && !source.IsAlly(this);

            // Set pending staleness
            PendingStaleness = external ? StalenessId.External : StalenessId.Internal;
        }
        else
        {
            PendingStaleness = null;
        }

        // Store old item and state
        Item oldItem = GetItem();
        EffectState oldItemState = ItemState;

        // Set new item
        Item = item;
        ItemState = Battle.InitEffectState(item, null, this);

        // Trigger End event on old item if it existed
        if (oldItem.Id != ItemId.None)
        {
            Battle.SingleEvent(EventId.End, oldItem, oldItemState, new PokemonSingleEventTarget(this));
        }

        // Trigger Start event on new item if it exists
        if (item != ItemId.None)
        {
            Battle.SingleEvent(EventId.Start, Battle.Library.Items[item], ItemState,
                this, SingleEventSource.FromNullablePokemon(source), effect);
        }

        return true;
    }

    public Item GetItem()
    {
        return Battle.Library.Items[Item];
    }

    /// <summary>
    /// Checks if the Pokemon has a specific item and is not ignoring it
    /// </summary>
    public bool HasItem(ItemId item)
    {
        // Check if Pokemon has the specified item
        if (Item != item) return false;

        // Check if Pokemon is ignoring its item
        return !IgnoringItem();
    }

    /// <summary>
    /// Checks if the Pokemon has any of the specified items and is not ignoring it
    /// </summary>
    public bool HasItem(ItemId[] items)
    {
        // Check if Pokemon's current item is in the array
        if (!items.Contains(Item)) return false;

        // Check if Pokemon is ignoring its item
        return !IgnoringItem();
    }

    public bool ClearItem()
    {
        return SetItem(ItemId.None);
    }

    /// <summary>
    /// Checks if the Pokemon is ignoring its held item due to various effects
    /// </summary>
    /// <param name="isFling">If true, this check is for Fling move (prevents infinite recursion)</param>
    /// <returns>True if the Pokemon is ignoring its item</returns>
    public bool IgnoringItem(bool isFling = false)
    {
        // Get the actual item object to check its properties
        Item item = GetItem();

        // Primal Orbs are never ignored
        if (item.IsPrimalOrb) return false;

        // Items that were knocked off are ignored (Gen 3-4 mechanic)
        if (ItemState.KnockedOff == true) return true;

        // In Gen 5+, inactive Pokemon ignore their items
        if (Battle.Gen >= 5 && !IsActive) return true;

        // Embargo volatile condition causes item ignoring
        if (Volatiles.ContainsKey(ConditionId.Embargo)) return true;

        // Magic Room pseudo-weather causes item ignoring
        if (Battle.Field.PseudoWeather.ContainsKey(ConditionId.MagicRoom)) return true;

        // Check Fling first to avoid infinite recursion
        if (isFling)
        {
            return Battle.Gen >= 5 && HasAbility(AbilityId.Klutz);
        }

        // Regular Klutz check - ignores item unless item specifically ignores Klutz
        if (HasAbility(AbilityId.Klutz))
        {
            return item.IgnoreKlutz != true;
        }

        return false;
    }

    public AbilityIdFalseUnion? SetAbility(AbilityId ability, Pokemon? source = null, IEffect? sourceEffect = null,
        bool isFromFormeChange = false, bool isTransform = false)
    {
        // Early exit if Pokemon is fainted
        if (Hp <= 0) return AbilityIdFalseUnion.FromFalse();

        // Get the ability object from the battle library
        Ability newAbility = Battle.Library.Abilities[ability];

        // Default sourceEffect to battle effect if not provided
        sourceEffect ??= Battle.Effect;

        // Get the old ability for comparison and return value
        Ability oldAbility = GetAbility();

        // Check suppression flags (unless from forme change)
        if (!isFromFormeChange)
        {
            if (newAbility.Flags.CantSuppress == true || oldAbility.Flags.CantSuppress == true)
            {
                return AbilityIdFalseUnion.FromFalse();
            }
        }

        // Run SetAbility event for validation (unless from forme change or transform)
        if (!isFromFormeChange && !isTransform)
        {
            RelayVar? setAbilityEvent = Battle.RunEvent(EventId.SetAbility, this,
                RunEventSource.FromNullablePokemon(source), sourceEffect, newAbility);

            // Return the actual event result (matching TypeScript behavior)
            if (setAbilityEvent is BoolRelayVar { Value: false })
            {
                return AbilityIdFalseUnion.FromFalse();
            }
            if (setAbilityEvent is null)
            {
                return null;
            }
        }

        // End the old ability's effects
        Battle.SingleEvent(EventId.End, oldAbility, AbilityState, new PokemonSingleEventTarget(this),
   SingleEventSource.FromNullablePokemon(source));

        // Set the new ability
        Ability = ability;
        AbilityState = Battle.InitEffectState(ability, null, this);

        // Send battle message ONLY if sourceEffect exists and DisplayUi is enabled (matching TypeScript)
        if (Battle.DisplayUi && !isFromFormeChange && !isTransform)
        {
            if (source != null)
            {
                Battle.Add("-ability", this, newAbility.Name, oldAbility.Name,
                    $"[from] {sourceEffect.FullName}", $"[of] {source}");
            }
            else
            {
                Battle.Add("-ability", this, newAbility.Name, oldAbility.Name,
                    $"[from] {sourceEffect.FullName}");
            }
        }

        // Start the new ability's effects (Gen 4+ only, but since gen is always 9, we skip the gen check)
        if (ability != AbilityId.None &&
            (!isTransform || oldAbility.Id != newAbility.Id))
        {
            Battle.SingleEvent(EventId.Start, newAbility, AbilityState, this,
                SingleEventSource.FromNullablePokemon(source));
        }

        return oldAbility.Id;
    }

    public Ability GetAbility()
    {
        return Battle.Library.Abilities[Ability];
    }

    public bool HasAbility(AbilityId ability)
    {
        if (ability != Ability) return false;
        return !IgnoringAbility();
    }

    public bool HasAbility(AbilityId[] abilities)
    {
        return abilities.Contains(Ability) && !IgnoringAbility();
    }

    public AbilityIdFalseUnion? ClearAbility()
    {
        return SetAbility(AbilityId.None);
    }

    /// <summary>
    /// Checks if the Pokemon's ability is being ignored due to various effects
    /// </summary>
    /// <returns>True if the Pokemon's ability is being ignored</returns>
    public bool IgnoringAbility()
    {
        // In Gen 5+, inactive Pokemon have their abilities suppressed
        if (Battle.Gen >= 5 && !IsActive) return true;

        Ability ability = GetAbility();

        // Certain abilities won't activate while Transformed, even if they ordinarily couldn't be suppressed
        if (ability.Flags.NoTransform == true && Transformed) return true;

        // Some abilities can't be suppressed at all
        if (ability.Flags.CantSuppress == true) return false;

        // Gastro Acid suppresses abilities
        if (Volatiles.ContainsKey(ConditionId.GastroAcid)) return true;

        // Ability Shield protects from ability suppression, and Neutralizing Gas can't suppress itself
        if (HasItem(ItemId.AbilityShield) || Ability == AbilityId.NeutralizingGas) return false;

        // Check if any active Pokemon have Neutralizing Gas ability
        return Battle.GetAllActive().Any(pokemon => pokemon.Ability == AbilityId.NeutralizingGas &&
                                                    !pokemon.Volatiles.ContainsKey(ConditionId.GastroAcid) &&
                                                    !pokemon.Transformed && pokemon.AbilityState.Ending != true &&
                                                    !Volatiles.ContainsKey(ConditionId.Commanding));
    }
}