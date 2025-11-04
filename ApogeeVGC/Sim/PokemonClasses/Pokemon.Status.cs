using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.PokemonClasses;

public partial class Pokemon
{
    public bool SetStatus(ConditionId statusId, Pokemon? source = null, IEffect? sourceEffect = null,
        bool ignoreImmunities = false)
    {
        // Initial HP check
        if (Hp <= 0) return false;

        Condition status = Battle.Library.Conditions[statusId];

        // Resolve source and sourceEffect from battle event if not provided
        if (Battle.Event is not null)
        {
            sourceEffect ??= Battle.Event.Effect;
            if (source == null && Battle.Event.Source is PokemonSingleEventSource pses)
            {
                source = pses.Pokemon;
            }
        }
        source ??= this; // This ensures source is never null after this point

        // Check for duplicate status
        if (Status == status.Id)
        {
            if (sourceEffect is ActiveMove move && move.Status == Status)
            {
                if (Battle.DisplayUi)
                {
                    Battle.Add("-fail", this, Battle.Library.Conditions[Status]);
                }
            }
            else if (sourceEffect is ActiveMove { Status: not null })
            {
                if (Battle.DisplayUi)
                {
                    Battle.Add("-fail", source);
                    Battle.AttrLastMove("[still]");
                }
            }
            return false;
        }

        // Immunity checks (unless ignored)
        if (!ignoreImmunities && status.Id != ConditionId.None)
        {
            // Special case for Corrosion ability bypassing poison immunity
            bool corrosionBypass = source.HasAbility(AbilityId.Corrosion) &&
                                   status.Id is ConditionId.Toxic or ConditionId.Poison;

            if (!corrosionBypass)
            {
                // Check condition-specific immunity using the new overload
                if (!RunStatusImmunity(status.Id))
                {
                    if (Battle.DisplayUi)
                    {
                        Battle.Debug("immune to status");

                        if (sourceEffect is ActiveMove { Status: not null })
                        {
                            Battle.Add("-immune", this);
                        }
                    }
                    return false;
                }
            }
        }

        // Store previous status for potential rollback
        ConditionId prevStatus = Status;
        EffectState prevStatusState = StatusState;

        // Run SetStatus event
        if (status.Id != ConditionId.None)
        {
            RelayVar? result = Battle.RunEvent(EventId.SetStatus, this, source, sourceEffect, status);
            if (result is BoolRelayVar { Value: false })
            {
                if (Battle.DisplayUi)
                {
                    Battle.Debug($"set status [{status.Id}] interrupted");
                }
                return false;
            }
        }

        // Apply the status
        Status = status.Id;
        StatusState = Battle.InitEffectState(status.Id, null, this);

        StatusState.Source = source;

        if (status.Duration is not null)
        {
            StatusState.Duration = status.Duration;
        }

        if (status.DurationCallback is not null)
        {
            StatusState.Duration = status.DurationCallback(Battle, this, source, sourceEffect);
        }

        // Run Start event (with rollback on failure)
        if (status.Id != ConditionId.None)
        {
            RelayVar? startResult = Battle.SingleEvent(EventId.Start, status, StatusState, this,
                source, sourceEffect);

            // Convert RelayVar to boolean - if it's a BoolRelayVar with false, or null, treat as failure
            bool startSucceeded = startResult switch
            {
                BoolRelayVar brv => brv.Value,
                null => false,
                _ => true, // Non-boolean RelayVar types are treated as success
            };

            if (!startSucceeded)
            {
                if (Battle.DisplayUi)
                {
                    Battle.Debug($"status start [{status.Id}] interrupted");
                }

                // Rollback the status change
                Status = prevStatus;
                StatusState = prevStatusState;
                return false;
            }
        }

        // Run AfterSetStatus event
        if (status.Id != ConditionId.None)
        {
            RelayVar? afterResult = Battle.RunEvent(EventId.AfterSetStatus, this, source, sourceEffect,
                status);
            if (afterResult is BoolRelayVar { Value: false })
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Unlike CureStatus, this does not give any cure messages.
    /// </summary>
    public bool ClearStatus()
    {
        // Early exit if Pokemon is fainted or has no status
        if (Hp <= 0 || Status == ConditionId.None) return false;

        // Special case: If clearing sleep, also remove Nightmare volatile (silent)
        if (Status == ConditionId.Sleep && Volatiles.ContainsKey(ConditionId.Nightmare))
        {
            // Remove Nightmare volatile and add silent end message
            if (RemoveVolatile(Battle.Library.Conditions[ConditionId.Nightmare]))
            {
                if (Battle.DisplayUi)
                {
                    Battle.Add("-end", this, Battle.Library.Conditions[ConditionId.Nightmare], "[silent]");
                }
            }
        }

        // Clear the status directly (no events, no messages)
        Status = ConditionId.None;
        StatusState = Battle.InitEffectState();

        return true;
    }

    public Condition GetStatus()
    {
        return Battle.Library.Conditions[Status];
    }

    public bool TrySetStatus(ConditionId status, Pokemon? source = null, IEffect? sourceEffect = null)
    {
        // Logic: if we already have a status, try to set that same status (which will fail duplicate check)
    // Otherwise, set the new status
        return SetStatus(Status != ConditionId.None ? Status : status, source, sourceEffect);
    }

    public bool CureStatus(bool silent = false)
    {
        // Early exit if Pokemon is fainted or has no status
        if (Hp <= 0 || Status == ConditionId.None) return false;

        // Add cure status message to battle log
        if (Battle.DisplayUi)
        {
            Battle.Add("-curestatus", this, Battle.Library.Conditions[Status], silent ? "[silent]" : "[msg]");
        }

        // Special case: If curing sleep, also remove Nightmare volatile
        if (Status == ConditionId.Sleep && Volatiles.ContainsKey(ConditionId.Nightmare))
        {
            if (RemoveVolatile(Battle.Library.Conditions[ConditionId.Nightmare]))
            {
                if (Battle.DisplayUi)
                {
                    Battle.Add("-end", this, Battle.Library.Conditions[ConditionId.Nightmare], "[silent]");
                }
            }
        }

        // Clear the status (equivalent to setStatus(''))
        SetStatus(ConditionId.None);

        return true;
    }

    public RelayVar AddVolatile(ConditionId status, Pokemon? source = null, IEffect? sourceEffect = null,
        ConditionId? linkedStatus = null)
    {
        // Get the condition from the battle library
        Condition condition = Battle.Library.Conditions[status];

        // Early exit if Pokemon is fainted and condition doesn't affect fainted Pokemon
        if (Hp <= 0 && condition.AffectsFainted != true)
            return new BoolRelayVar(false);

        // Early exit if linked status and source is fainted
        if (linkedStatus != null && source is { Hp: <= 0 })
            return new BoolRelayVar(false);

        // Resolve source and sourceEffect from battle event if not provided
        if (Battle.Event is not null)
        {
            if (source == null && Battle.Event.Source is PokemonSingleEventSource pses)
            {
                source = pses.Pokemon;
            }
            sourceEffect ??= Battle.Event.Effect;
        }
        source ??= this; // Default source to this Pokemon

        // Check if volatile already exists
        if (Volatiles.TryGetValue(status, out EffectState? existingState))
        {
            // If no restart callback, fail
            if (condition.OnRestart == null)
                return new BoolRelayVar(false);

            // Try to restart the existing volatile
            return Battle.SingleEvent(EventId.Restart, condition, existingState, this, source,
                       sourceEffect) ?? new BoolRelayVar(false);
        }

        // Check status immunity
        if (!RunStatusImmunity(status))
        {
            if (Battle.DisplayUi)
            {
                Battle.Debug("immune to volatile status");
            }

            // Show immunity message if source effect is a move with status
            if (sourceEffect is ActiveMove { Status: not null })
            {
                if (Battle.DisplayUi)
                {
                    Battle.Add("-immune", this);
                }
            }
            return new BoolRelayVar(false);
        }

        // Run TryAddVolatile event
        RelayVar? tryResult = Battle.RunEvent(EventId.TryAddVolatile, this, source, sourceEffect,
            condition);

        if (tryResult is BoolRelayVar { Value: false } or null)
        {
            if (Battle.DisplayUi)
            {
                Battle.Debug($"add volatile [{status}] interrupted");
            }
            return tryResult ?? new BoolRelayVar(false);
        }

        // Create the volatile effect state
        Volatiles[status] = Battle.InitEffectState(status, null, this);
        EffectState volatileState = Volatiles[status];

        // Set source information
        volatileState.Source = source;
        volatileState.SourceSlot = source.GetSlot();

        // Set source effect
        if (sourceEffect != null)
        {
            volatileState.SourceEffect = sourceEffect;
        }

        // Set duration from condition
        if (condition.Duration != null)
        {
            volatileState.Duration = condition.Duration;
        }

        // Set duration from callback
        if (condition.DurationCallback != null)
        {
            volatileState.Duration = condition.DurationCallback(Battle, this, source, sourceEffect);
        }

        // Run the Start event
        RelayVar? startResult = Battle.SingleEvent(EventId.Start, condition, volatileState,
            this, source, sourceEffect);

        // Check if start event failed
        bool startSucceeded = startResult switch
        {
            BoolRelayVar brv => brv.Value,
            null => false,
            _ => true, // Non-boolean RelayVar types are treated as success
        };

        if (!startSucceeded)
        {
            // Cancel - remove the volatile we just added
            Volatiles.Remove(status);
            return startResult ?? new BoolRelayVar(false);
        }

        // Handle linked status setup
        if (linkedStatus != null)
        {
            if (!source.Volatiles.TryGetValue(linkedStatus.Value, out EffectState? linkedState))
            {
                // Source doesn't have the linked status - add it
                source.AddVolatile(linkedStatus.Value, this, sourceEffect);
                linkedState = source.Volatiles[linkedStatus.Value];
                linkedState.LinkedPokemon = [this];
                linkedState.LinkedStatus = condition;
            }
            else
            {
                // Source already has linked status - add this Pokemon to the list
                linkedState.LinkedPokemon ??= [];
                linkedState.LinkedPokemon.Add(this);
            }

            // Set up reverse linking on this Pokemon's volatile
            volatileState.LinkedPokemon = [source];
            volatileState.LinkedStatus = Battle.Library.Conditions[linkedStatus.Value];
        }

        return new BoolRelayVar(true);
    }

    public EffectState? GetVolatile(ConditionId volatileId)
    {
        return Volatiles.GetValueOrDefault(volatileId);
    }

    public bool RemoveVolatile(Condition status)
    {
        // Check if Pokemon is fainted (equivalent to !this.hp)
        if (Hp <= 0) return false;

        // Check if the volatile exists
        if (!Volatiles.TryGetValue(status.Id, out EffectState? volatileData))
            return false;

        // Extract linked data (equivalent to destructuring)
        var linkedPokemon = volatileData.LinkedPokemon;
        Condition? linkedStatus = volatileData.LinkedStatus;

        // Trigger the End event
        Battle.SingleEvent(EventId.End, status, volatileData, this);

        // Remove the volatile (equivalent to delete this.volatiles[status.id])
        Volatiles.Remove(status.Id);

        // Handle linked Pokemon cleanup
        if (linkedPokemon is not null && linkedStatus is not null)
        {
            RemoveLinkedVolatiles(linkedStatus, linkedPokemon);
        }

        return true;
    }

    public void RemoveLinkedVolatiles(Condition linkedStatus, List<Pokemon> linkedPokemon)
    {
        foreach (Pokemon linkedPoke in linkedPokemon)
        {
            if (!linkedPoke.Volatiles.TryGetValue(linkedStatus.Id, out EffectState? volatileData) ||
                volatileData.LinkedPokemon is null)
            {
                continue;
            }
            // Remove this Pokemon from the linked Pokemon list
            volatileData.LinkedPokemon.Remove(this);

            // If no linked Pokemon remain, remove the volatile status
            if (volatileData.LinkedPokemon.Count == 0)
            {
                linkedPoke.RemoveVolatile(linkedStatus);
            }
        }
    }

    /// <summary>
    /// Deletes a volatile condition without running the extra logic from RemoveVolatile
    /// </summary>
    public bool DeleteVolatile(ConditionId volatileId)
    {
        return Volatiles.Remove(volatileId);
    }

    public void CopyVolatileFrom(Pokemon pokemon, ConditionIdBoolUnion? switchCause = null)
    {
        // Clear this Pokémon's current volatiles
        ClearVolatile();

        // Determine if switchCause is 'shedtail'
        bool isShedTail = switchCause switch
        {
            ConditionIdConditionIdBoolUnion { ConditionId: ConditionId.ShedTail } => true,
            _ => false,
        };

        // Copy boosts unless switch cause is 'shedtail'
        if (!isShedTail)
        {
            Boosts = new BoostsTable
            {
                Atk = pokemon.Boosts.Atk,
                Def = pokemon.Boosts.Def,
                SpA = pokemon.Boosts.SpA,
                SpD = pokemon.Boosts.SpD,
                Spe = pokemon.Boosts.Spe,
                Accuracy = pokemon.Boosts.Accuracy,
                Evasion = pokemon.Boosts.Evasion,
            };
        }

        // Copy each volatile condition
        foreach ((ConditionId conditionId, EffectState volatileState) in pokemon.Volatiles)
        {
            // Skip non-Substitute volatiles when using Shed Tail
            if (isShedTail && conditionId != ConditionId.Substitute)
            {
                continue;
            }

            // Get the condition definition
            Condition condition = Battle.Library.Conditions[conditionId];

            // Skip conditions marked as noCopy
            if (condition.NoCopy)
            {
                continue;
            }

            // Create a shallow clone of the volatile state with this Pokémon as the target
            EffectState clonedState = volatileState.ShallowClone();
            clonedState.Target = this;

            // Initialize the effect state for this Pokémon
            Volatiles[conditionId] = clonedState;

            // Handle linked Pokémon (for moves like Bind, Wrap, etc.)
            if (clonedState.LinkedPokemon is null || clonedState.LinkedStatus is null) continue;
            // Clear the source's linked references
            pokemon.Volatiles[conditionId].LinkedPokemon = null;
            pokemon.Volatiles[conditionId].LinkedStatus = null;

            // Update all linked Pokémon to point to this Pokémon instead of the source
            foreach (Pokemon linkedPoke in clonedState.LinkedPokemon)
            {
                // Get the linked Pokémon's volatile state for this condition
                if (!linkedPoke.Volatiles.TryGetValue(clonedState.LinkedStatus.Id, out EffectState? linkedState)
                    || linkedState.LinkedPokemon is null) continue;
                // Find and replace the source Pokémon with this Pokémon
                int sourceIndex = linkedState.LinkedPokemon.IndexOf(pokemon);
                if (sourceIndex >= 0)
                {
                    linkedState.LinkedPokemon[sourceIndex] = this;
                }
            }
        }

        // Clear the source Pokémon's volatiles after copying
        pokemon.ClearVolatile();

        // Trigger Copy event for each copied volatile
        foreach ((ConditionId conditionId, EffectState volatileState) in Volatiles)
        {
            Condition condition = Battle.Library.Conditions[conditionId];
            Battle.SingleEvent(EventId.Copy, condition, volatileState, this);
        }
    }

    public void ClearVolatile(bool includeSwitchFlags = true)
    {
        Boosts = new BoostsTable
        {
            Atk = 0,
            Def = 0,
            SpA = 0,
            SpD = 0,
            Spe = 0,
            Accuracy = 0,
            Evasion = 0,
        };

        MoveSlots = BaseMoveSlots.Select(moveSlot => new MoveSlot
        {
       Id = moveSlot.Id,
    Move = moveSlot.Move,
         Pp = moveSlot.Pp,
      MaxPp = moveSlot.MaxPp,
     Target = moveSlot.Target,
      Disabled = false, // Always start with moves enabled
     DisabledSource = null, // Clear disabled source on reset
   Used = false, // Reset used flag
  Virtual = moveSlot.Virtual,
      }).ToList();

      Transformed = false;
        Ability = BaseAbility;
        if (CanTerastallize is FalseMoveTypeFalseUnion)
        {
            CanTerastallize = TeraType;
        }

        var volatileKeys = Volatiles.Keys.ToList();
        foreach (ConditionId conditionId in volatileKeys)
        {
            if (Volatiles.TryGetValue(conditionId, out EffectState? effectState) &&
                effectState.LinkedStatus is not null)
            {
                RemoveLinkedVolatiles(effectState.LinkedStatus, effectState.LinkedPokemon ?? []);
            }
        }

        Volatiles.Clear();

        if (includeSwitchFlags)
        {
            SwitchFlag = false;
            ForceSwitchFlag = false;
        }

        LastMove = null;
        LastMoveUsed = null;
        MoveThisTurn = MoveId.None;
        MoveLastTurnResult = null;
        MoveThisTurnResult = null;

        LastDamage = 0;
        AttackedBy.Clear();
        HurtThisTurn = null;
        NewlySwitched = true;
        BeingCalledBack = false;

        VolatileStaleness = null;

        AbilityState.Started = null;
        ItemState.Started = null;

        SetSpecies(BaseSpecies, Battle.Effect);
    }
}