using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Ui;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.PokemonClasses;

public class Pokemon
{
    public Side Side { get; }
    public IBattle Battle => Side.Battle;
    public PokemonSet Set { get; }
    public string Name => Set.Name[..20];

    public string Fullname => $"{Side.Id.GetSideIdName()}: {Name}";
    public int Level => Set.Level;
    public GenderId Gender => Set.Gender;
    public int Happiness => Set.Happiness;
    public PokeballId Pokeball => Set.Pokeball;

    public IReadOnlyList<MoveSlot> BaseMoveSlots { get; }
    public StatsTable Evs => Set.Evs;
    public StatsTable Ivs => Set.Ivs;
    public List<MoveSlot> MoveSlots { get; set; }

    public PokemonSlotId Position { get; set; }

    public Species BaseSpecies { get; set; }
    public Species Species { get; set; }

    public EffectState SpeciesState { get; set; }

    public ConditionId? Status { get; set; }
    public EffectState StatusState { get; set; }

    public Dictionary<ConditionId, EffectState> Volatiles { get; set; }
    public bool? ShowCure { get; set; }

    public StatsTable BaseStoredStats { get; set; }
    public StatsExceptHpTable StoredStats { get; set; }
    public BoostsTable Boosts { get; set; }

    public AbilityId BaseAbility { get; set; }
    public AbilityId Ability { get; set; }
    public EffectState AbilityState { get; set; }

    public ItemId Item { get; set; }
    public EffectState ItemState { get; set; }
    public ItemId LastItem { get; set; }
    public bool UsedItemThisTurn { get; set; }

    public bool AteBerry { get; set; }
    public PokemonTrapped Trapped { get; set; }
    public bool MaybeTrapped { get; set; }
    public bool MaybeDisabled { get; set; }
    public bool? MaybeLocked { get; set; }

    public Pokemon? Illusion { get; set; }
    public bool Transformed { get; set; }

    public int MaxHp { get; set; }
    public int BaseMaxHp { get; set; }
    public int Hp { get; set; }
    public bool Fainted { get; set; }
    public bool FaintQueued { get; set; }
    public bool? SubFainted { get; set; }

    public bool FormeRegression { get; set; }

    public List<PokemonType> Types { get; set; }
    public PokemonType? AddedType { get; set; }
    public bool KnownType { get; set; }
    public List<PokemonType> ApparentType { get; set; }

    public MoveIdBoolUnion SwitchFlag { get; set; }
    public bool ForceSwitchFlag { get; set; }
    public bool SkipBeforeSwitchOutEventFlag { get; set; }
    public int? DraggedIn { get; set; }
    public bool NewlySwitched { get; set; }
    public bool BeingCalledBack { get; set; }

    public Move? LastMove { get; set; }
    public Move? LastMoveEncore { get; set; } // Gen 2 only
    public Move? LastMoveUsed { get; set; }
    public int? LastMoveTargetLoc { get; set; }
    public MoveIdBoolUnion MoveThisTurn { get; set; }
    public bool StatsRaisedThisTurn { get; set; }
    public bool StatsLoweredThisTurn { get; set; }
    public bool? MoveLastTurnResult { get; set; }
    public bool? MoveThisTurnResult { get; set; }
    public int? HurtThisTurn { get; set; }
    public int LastDamage { get; set; }
    public List<Attacker> AttackedBy { get; set; }
    public int TimesAttacked { get; set; }

    public bool IsActive { get; set; }
    public int ActiveTurns { get; set; }
    public int ActiveMoveActions { get; set; }
    public int PreviouslySwitchedIn { get; set; }
    public bool TruantTurn { get; set; }
    public bool BondTriggered { get; set; }
    public bool SwordBoost { get; set; } // Gen 9 only
    public bool ShieldBoost { get; init; } // Gen 9 only
    public bool SyrupTriggered { get; set; } // Gen 9 only
    public List<MoveType> StellarBoostedTypes { get; set; } // Gen 9 only

    public bool IsStarted { get; set; }
    public bool DuringMove { get; set; }

    public double WeightKg { get; set; }
    public int WeightHg { get; set; }
    public int Speed { get; set; }

    public MoveTypeFalseUnion? CanTerastallize { get; set; }
    public MoveType TeraType { get; set; }
    public List<PokemonType> BaseTypes { get; set; }
    public MoveType? Terastallized { get; set; }

    public Staleness? Staleness { get; set; }
    public Staleness? PendingStaleness { get; set; }

    public Staleness? VolatileStaleness
    {
        get;
        set // must be 'external' or null
        {
            if (value is PokemonClasses.Staleness.External or null)
            {
                field = value;
            }
            else
            {
                throw new ArgumentException("VolatileStaleness must be 'external' or null");
            }
        }
    }

    public PokemonDetails Details { get; set; }

    public Pokemon(IBattle battle, PokemonSet set, Side side)
    {
        Side = side;
        Set = set;
        BaseSpecies = battle.Library.Species[set.Species];
        Species = BaseSpecies;

        SpeciesState = battle.InitEffectState(Species.Id, null, null);

        if (set.Moves.Count == 0)
        {
            throw new InvalidOperationException($"Set {Name} has no moves");
        }

        MoveSlots = [];
        BaseMoveSlots = [];

        List<MoveSlot> baseMoveSlots = [];

        baseMoveSlots.AddRange(from moveId in Set.Moves
            let move = battle.Library.Moves[moveId]
            let basePp = move.NoPpBoosts ? move.BasePp : move.BasePp * 8 / 5
            select new MoveSlot
            {
                Id = moveId,
                Move = moveId,
                Pp = basePp,
                MaxPp = basePp,
                Target = move.Target,
                Disabled = false,
                DisabledSource = null,
                Used = false, 
                }
            );
        BaseMoveSlots = baseMoveSlots;

        Position = PokemonSlotId.Slot1;
        StatusState = battle.InitEffectState(null, null, null);
        Volatiles = [];

        BaseStoredStats = new StatsTable(); // Will be initialized in SetSpecies
        StoredStats = new StatsExceptHpTable { Atk = 0, Def = 0, SpA = 0, SpD = 0, Spe = 0 };
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

        Ability = BaseAbility;
        AbilityState = battle.InitEffectState(Ability, null, this);

        Item = set.Item;
        ItemState = battle.InitEffectState(Item, null, this);
        LastItem = ItemId.None;

        Trapped = PokemonTrapped.False;

        Types = BaseSpecies.Types.ToList();
        BaseTypes = Types;
        KnownType = true;
        ApparentType = BaseTypes;
        TeraType = Set.TeraType;

        SwitchFlag = false;

        LastMoveUsed = null;
        MoveThisTurn = false;
        AttackedBy = [];

        StellarBoostedTypes = [];

        WeightKg = 1.0;

        CanTerastallize = BattleActions.CanTerastallize(battle, this);

        ClearVolatile();
        Hp = MaxHp;
        Details = GetUpdatedDetails();
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

        MoveSlots = BaseMoveSlots.ToList();

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

        SetSpecie(BaseSpecies, Battle.Effect);
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

    public bool TrySetStatus(ConditionId status, Pokemon? source = null, IEffect? sourceEffect = null)
    {
        return SetStatus(Status ?? status, source, sourceEffect);
    }

    public bool SetStatus(ConditionId statusId, Pokemon? source = null, IEffect? sourceEffect = null,
    bool ignoreImmunities = false)
    {
        // Initial HP check
        if (Hp <= 0) return false;

        Condition status = Battle.Library.Conditions[statusId];

        // Resolve source and sourceEffect from battle event if not provided
        if (Battle.Event is not null)
        {
            source ??= Battle.Event.Source;
            sourceEffect ??= Battle.Event.Effect;
        }
        source ??= this; // This ensures source is never null after this point

        // Check for duplicate status
        if (Status is not null && Status == status.Id)
        {
            if (sourceEffect is ActiveMove move && move.Status == Status)
            {
                UiGenerator.PrintFailEvent(this, Battle.Library.Conditions[(ConditionId)Status]);
            }
            else if (sourceEffect is ActiveMove { Status: not null })
            {
                UiGenerator.PrintFailEvent(source);
                // Battle.AttrLastMove("[still]"); // Skipping visual effect
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
                    if (Battle.PrintDebug)
                    {
                        // Battle.Debug("immune to status");
                    }

                    if (sourceEffect is ActiveMove { Status: not null })
                    {
                        UiGenerator.PrintImmuneEvent(this);
                    }
                    return false;
                }
            }
        }

        // Store previous status for potential rollback
        var prevStatus = Status;
        EffectState prevStatusState = StatusState;

        // Run SetStatus event
        if (status.Id != ConditionId.None)
        {
            RelayVar? result = Battle.RunEvent(EventId.SetStatus, this, source, sourceEffect, status);
            if (result is BoolRelayVar { Value: false })
            {
                if (Battle.PrintDebug)
                {
                    // Battle.Debug($"set status [{status.Id}] interrupted");
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
            // FIXED: Proper boolean handling for SingleEvent
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
                if (Battle.PrintDebug)
                {
                    // Battle.Debug($"status start [{status.Id}] interrupted");
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
    /// Checks status immunity based on Pokemon type (e.g., Fire-types immune to Burn)
    /// </summary>
    public bool RunStatusImmunity(PokemonType? type, string? message = null)
    {
        // Check if Pokemon is fainted
        if (Fainted) return false;

        // Equivalent to TypeScript: if (!type) return true;
        if (type == null) return true;

        // Convert PokemonType to MoveType for immunity check
        MoveType moveType = type.Value.ConvertToMoveType();

        // Check natural type immunity using battle's dex
        if (!Battle.Dex.GetImmunity(moveType, this))
        {
            if (Battle.PrintDebug)
            {
                // Battle.Debug("natural status immunity");
            }

            if (!string.IsNullOrEmpty(message))
            {
                UiGenerator.PrintImmuneEvent(this);
            }
            return false;
        }

        // Check artificial immunity (abilities, items, etc.)
        RelayVar? immunity = Battle.RunEvent(EventId.Immunity, this, null, null, type);

        // TypeScript logic: if (!immunity) - means if immunity is falsy/false
        if (immunity is BoolRelayVar { Value: false } or null)
        {
            if (Battle.PrintDebug)
            {
                // Battle.Debug("artificial status immunity");
            }

            // TypeScript: if (message && immunity !== null)
            if (!string.IsNullOrEmpty(message) && immunity is not null)
            {
                UiGenerator.PrintImmuneEvent(this);
            }
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks immunity for specific conditions (e.g., Sleep immunity for certain Pokemon types)
    /// </summary>
    public bool RunStatusImmunity(ConditionId? conditionId, string? message = null)
    {
        // Check if Pokemon is fainted
        if (Fainted) return false;

        // Equivalent to TypeScript: if (!type) return true;
        if (conditionId == null) return true;

        // Get the condition from the library
        Condition condition = Battle.Library.Conditions[conditionId.Value];

        // Check natural condition immunity using ModdedDex static method
        if (!ModdedDex.GetImmunity(condition, Types))
        {
            if (Battle.PrintDebug)
            {
                // Battle.Debug("natural condition immunity");
            }

            if (!string.IsNullOrEmpty(message))
            {
                UiGenerator.PrintImmuneEvent(this);
            }
            return false;
        }

        // Check artificial immunity (abilities, items, etc.) for conditions
        RelayVar? immunity = Battle.RunEvent(EventId.Immunity, this, null, null,
            conditionId);

        // TypeScript logic: if (!immunity) - means if immunity is falsy/false
        if (immunity is BoolRelayVar { Value: false } or null)
        {
            if (Battle.PrintDebug)
            {
                // Battle.Debug("artificial condition immunity");
            }

            // TypeScript: if (message && immunity !== null)
            if (!string.IsNullOrEmpty(message) && immunity is not null)
            {
                UiGenerator.PrintImmuneEvent(this);
            }
            return false;
        }

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
            source ??= Battle.Event.Source;
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
            if (Battle.PrintDebug)
            {
                // Battle.Debug("immune to volatile status");
            }

            // Show immunity message if source effect is a move with status
            if (sourceEffect is ActiveMove { Status: not null })
            {
                UiGenerator.PrintImmuneEvent(this);
            }
            return new BoolRelayVar(false);
        }

        // Run TryAddVolatile event
        RelayVar? tryResult = Battle.RunEvent(EventId.TryAddVolatile, this, source, sourceEffect,
            condition);

        if (tryResult is BoolRelayVar { Value: false } or null)
        {
            if (Battle.PrintDebug)
            {
                // Battle.Debug($"add volatile [{status}] interrupted");
            }
            return tryResult ?? new BoolRelayVar(false);
        }

        // Create the volatile effect state
        Volatiles[status] = Battle.InitEffectState(status, null, this);
        EffectState volatileState = Volatiles[status];

        // Set source information
        volatileState.Source = source;
        volatileState.SourceSlot = source.Position; // Assuming getSlot() maps to Position

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
            _ => true, // Non-boolean RelayVar types treated as success
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

    /// <summary>
    /// Deletes a volatile condition without running the extra logic from RemoveVolatile
    /// </summary>
    public bool DeleteVolatile(ConditionId volatileId)
    {
        return Volatiles.Remove(volatileId);
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

    public Ability GetAbility()
    {
        return Battle.Library.Abilities[Ability];
    }

    public StatIdExceptHp GetBestStat(bool unboosted = false, bool unmodified = false)
    {
        int bestStatValue = int.MinValue;
        var bestStatName = StatIdExceptHp.Atk;

        // Iterate through all stat types except HP
        foreach (StatIdExceptHp statId in Enum.GetValues<StatIdExceptHp>())
        {
            int currentStatValue = GetStat(statId, unboosted, unmodified);
            if (currentStatValue <= bestStatValue) continue;
            bestStatValue = currentStatValue;
            bestStatName = statId;
        }
        return bestStatName;
    }

    public int GetStat(StatIdExceptHp statName, bool unboosted = false, bool unmodified = false)
    {
        int stat = StoredStats[statName];

        // Wonder Room swaps Def and SpD
        if (unmodified && Battle.Field.PseudoWeather.ContainsKey(ConditionId.WonderRoom))
        {
            statName = statName switch
            {
                StatIdExceptHp.Def => StatIdExceptHp.SpD,
                StatIdExceptHp.SpD => StatIdExceptHp.Def,
                _ => statName,
            };
        }

        // Stat boosts
        if (!unboosted)
        {
            BoostsTable boosts = Boosts;
            if (!unmodified)
            {
                RelayVar? relayVar = Battle.RunEvent(EventId.ModifyBoost, this, null,
                    null, boosts);

                if (relayVar is BoostsTableRelayVar brv)
                {
                    boosts = brv.Table;
                }
                else
                {
                    throw new InvalidOperationException("boosts must be a BoostsTableRelayVar" );
                }
            }
            stat = (int)Math.Floor(stat * boosts.GetBoostMultiplier(statName));
        }

        // Stat modifier effects
        if (!unmodified)
        {
            EventId eventId = statName switch
            {
                StatIdExceptHp.Atk => EventId.ModifyAtk,
                StatIdExceptHp.Def => EventId.ModifyDef,
                StatIdExceptHp.SpA => EventId.ModifySpA,
                StatIdExceptHp.SpD => EventId.ModifySpD,
                StatIdExceptHp.Spe => EventId.ModifySpe,
                _ => throw new ArgumentOutOfRangeException(nameof(statName), "Invalid stat name."),
            };
            RelayVar? relayVar = Battle.RunEvent(eventId, this, null, null, stat);
            if (relayVar is IntRelayVar irv)
            {
                stat = irv.Value;
            }
            else
            {
                throw new InvalidOperationException("stat must be an IntRelayVar" );
            }
        }

        if (statName == StatIdExceptHp.Spe && stat > 10000) stat = 10000;
        return stat;
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

    public bool CureStatus(bool silent = false)
    {
        // Early exit if Pokemon is fainted or has no status
        if (Hp <= 0 || Status == null) return false;

        // Add cure status message to battle log
        UiGenerator.PrintCureStatusEvent(this, Battle.Library.Conditions[Status.Value]);

        // Special case: If curing sleep, also remove Nightmare volatile
        if (Status == ConditionId.Sleep)
        {
            // Check if Pokemon has Nightmare volatile and remove it
            if (Volatiles.ContainsKey(ConditionId.Nightmare))
            {
                DeleteVolatile(ConditionId.Nightmare); // Use DeleteVolatile to avoid extra logic
                UiGenerator.PrintEndEvent(this, Battle.Library.Conditions[ConditionId.Nightmare]);
            }
        }

        // Clear the status (equivalent to setStatus(''))
        SetStatus(ConditionId.None);

        return true;
    }

    /// <summary>
    /// Unlike CureStatus, this does not give any cure messages.
    /// </summary>
    public bool ClearStatus()
    {
        // Early exit if Pokemon is fainted or has no status
        if (Hp <= 0 || Status == null) return false;

        // Special case: If clearing sleep, also remove Nightmare volatile (silent)
        if (Status == ConditionId.Sleep && Volatiles.ContainsKey(ConditionId.Nightmare))
        {
            // Remove Nightmare volatile and add silent end message
            if (RemoveVolatile(Battle.Library.Conditions[ConditionId.Nightmare]))
            {
                UiGenerator.PrintEndEvent(this, Battle.Library.Conditions[ConditionId.Nightmare]);
            }
        }

        // Clear the status directly (no events, no messages)
        Status = null;
        StatusState = Battle.InitEffectState(null, null, null);

        return true;
    }

 //   /**
	// * Changes this Pokemon's forme to match the given speciesId (or species).
	// * This function handles all changes to stats, ability, type, species, etc.
	// * as well as sending all relevant messages sent to the client.
	// */
	//formeChange(
	//	speciesId: string | Species, source: Effect | null = this.battle.effect,
	//	isPermanent?: boolean, abilitySlot = '0', message?: string
	//) {
	//	const rawSpecies = this.battle.dex.species.get(speciesId);

	//	const species = this.setSpecies(rawSpecies, source);
	//	if (!species) return false;

	//	if (this.battle.gen <= 2) return true;

	//	// The species the opponent sees
	//	const apparentSpecies =
	//		this.illusion ? this.illusion.species.name : species.baseSpecies;
	//	if (isPermanent) {
	//		this.baseSpecies = rawSpecies;
	//		this.details = this.getUpdatedDetails();
	//		let details = (this.illusion || this).details;
	//		if (this.terastallized) details += `, tera:${this.terastallized}`;
	//		this.battle.add('detailschange', this, details);
	//		this.updateMaxHp();
	//		if (!source) {
	//			// Tera forme
	//			// Ogerpon/Terapagos text goes here
	//			this.formeRegression = true;
	//		} else if (source.effectType === 'Item') {
	//			this.canTerastallize = null; // National Dex behavior
	//			if (source.zMove) {
	//				this.battle.add('-burst', this, apparentSpecies, species.requiredItem);
	//				this.moveThisTurnResult = true; // Ultra Burst counts as an action for Truant
	//			} else if (source.isPrimalOrb) {
	//				if (this.illusion) {
	//					this.ability = '';
	//					this.battle.add('-primal', this.illusion, species.requiredItem);
	//				} else {
	//					this.battle.add('-primal', this, species.requiredItem);
	//				}
	//			} else {
	//				this.battle.add('-mega', this, apparentSpecies, species.requiredItem);
	//				this.moveThisTurnResult = true; // Mega Evolution counts as an action for Truant
	//			}
	//			this.formeRegression = true;
	//		} else if (source.effectType === 'Status') {
	//			// Shaymin-Sky -> Shaymin
	//			this.battle.add('-formechange', this, species.name, message);
	//		}
	//	} else {
	//		if (source?.effectType === 'Ability') {
	//			this.battle.add('-formechange', this, species.name, message, `[from] ability: ${source.name}`);
	//		} else {
	//			this.battle.add('-formechange', this, this.illusion ? this.illusion.species.name : species.name, message);
	//		}
	//	}
	//	if (isPermanent && (!source || !['disguise', 'iceface'].includes(source.id))) {
	//		if (this.illusion && source) {
	//			// Tera forme by Ogerpon or Terapagos breaks the Illusion
	//			this.ability = ''; // Don't allow Illusion to wear off
	//		}
	//		const ability = species.abilities[abilitySlot] || species.abilities['0'];
	//		// Ogerpon's forme change doesn't override permanent abilities
	//		if (source || !this.getAbility().flags['cantsuppress']) this.setAbility(ability, null, null, true);
	//		// However, its ability does reset upon switching out
	//		this.baseAbility = toID(ability);
	//	}
	//	if (this.terastallized) {
	//		this.knownType = true;
	//		this.apparentType = this.terastallized;
	//	}
	//	return true;
	//}


    /// <summary>
    /// Changes this Pokemon's forme to match the given speciesId (or species).
    /// This function handles all changes to stats, ability, type, species, etc.
    /// as well as sending all relevant messages sent to the client.
    /// </summary>
    public bool FormeChange(SpecieId specieId, IEffect? source = null, bool? isPermanent = null,
        SpeciesAbilityType abilitySlot = SpeciesAbilityType.Slot0, string? message = null)
    {
        // Default source to battle effect if not provided
        source ??= Battle.Effect;

        // Get the raw species from the battle library
        Species rawSpecies = Battle.Library.Species[specieId];

        // Attempt to set the species
        Species? species = SetSpecie(rawSpecies, source);
        if (species == null) return false;

        // Early return for Gen 1-2 battles
        if (Battle.Gen <= 2) return true;

        // Determine the species the opponent sees (accounting for Illusion)
        //SpecieId apparentSpecies = Illusion?.Species.BaseSpecies ?? species.BaseSpecies;

        if (isPermanent == true)
        {
            // Update base species for permanent changes
            BaseSpecies = rawSpecies;

            // Update details and send to client
            Details = GetUpdatedDetails();
            PokemonDetails details = (Illusion ?? this).GetUpdatedDetails();

            // Add Tera type to details if Terastallized
            if (Terastallized != null)
            {
                details.TeraType = Terastallized.Value;
            }
            UiGenerator.PrintDetailsChangeEvent(this, details);

            // Update max HP based on new species
            UpdateMaxHp();

            // Handle different source types for permanent changes
            if (source.EffectType == EffectType.Condition)
            {
                // Status-based forme change (e.g., Shaymin-Sky -> Shaymin)
                UiGenerator.PrintFormeChangeEvent(this, species.Id, message);
            }
        }
        else
        {
            // Handle temporary forme changes
            if (source.EffectType == EffectType.Ability)
            {
                UiGenerator.PrintFormeChangeEvent(this, species.Id, message, source);
            }
            else
            {
                UiGenerator.PrintFormeChangeEvent(this, Illusion is not null ? Illusion.Species.Id :
                    species.Id, message);
            }
        }

        // Handle ability changes for permanent forme changes
        if (isPermanent == true &&
            source is Ability ability && ability.Id != AbilityId.Disguise && ability.Id != AbilityId.IceFace)
        {
            // Break Illusion for certain Tera forme changes
            if (Illusion != null)
            {
                // Tera forme by Ogerpon or Terapagos breaks the Illusion
                Ability = AbilityId.None; // Don't allow Illusion to wear off
            }

            // Get the new ability from the species
            AbilityId newAbility = species.Abilities.GetAbility(abilitySlot) ?? species.Abilities.Slot0;

            SetAbility(newAbility, isFromFormeChange: true);

            // Reset base ability (ability resets upon switching out)
            BaseAbility = newAbility;
        }

        // Update type visibility for Terastallized Pokemon
        if (Terastallized != null)
        {
            KnownType = true;
            ApparentType = [Terastallized.Value.ConvertToPokemonType()];
        }

        return true;
    }

    public class PokemonDetails
    {
        public SpecieId Id{ get; init; }
        public int Level { get; init; }
        public GenderId Gender { get; init; }
        public bool Shiny { get; init; }
        public MoveType? TeraType { get; set; }
    }

    public PokemonDetails GetUpdatedDetails(int? level = null)
    {
        SpecieId id = Species.Id;
        
        // Handle special forms that should use base species name
        if (id is SpecieId.GreninjaBond or SpecieId.RockruffDusk)
        {
            id = Species.BaseSpecies;
        }
        
        // Use provided level or fall back to Pokemon's level
        int displayLevel = level ?? Level;
        
        var details = new PokemonDetails
        {
            Id = id,
            Level = displayLevel,
            Gender = Gender,
            Shiny = Set.Shiny,
        };

        return details;
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
                PokemonFalseUnion.FromNullablePokemon(source), sourceEffect, newAbility);

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
        Battle.SingleEvent(EventId.End, oldAbility, AbilityState, this,
            SingleEventSource.FromNullablePokemon(source));

        // Set the new ability
        Ability = ability;
        AbilityState = Battle.InitEffectState(ability, null, this);

        // Send battle message ONLY if sourceEffect exists (matching TypeScript)
        if (!isFromFormeChange && !isTransform)
        {
            if (source != null)
            {
                UiGenerator.PrintAbilityChangeEvent(this, newAbility, oldAbility, sourceEffect, source);
            }
            else
            {
                UiGenerator.PrintAbilityChangeEvent(this, newAbility, oldAbility, sourceEffect);
            }
        }

        // Start the new ability's effects (Gen 4+ only)
        if (ability != AbilityId.None && Battle.Gen > 3 &&
            (!isTransform || oldAbility.Id != newAbility.Id || Battle.Gen <= 4))
        {
            Battle.SingleEvent(EventId.Start, newAbility, AbilityState, this,
                SingleEventSource.FromNullablePokemon(source));
        }

        return oldAbility.Id;
    }

    public void UpdateMaxHp()
    {
        throw new NotImplementedException();
    }

    public bool HasType(PokemonType type)
    {
        throw new NotImplementedException();
    }

    public bool HasType(PokemonType[] types)
    {
        throw new NotImplementedException();
    }

    public bool SetType(PokemonType type, bool enforce = false)
    {
        throw new NotImplementedException();
    }

    public bool SetType(PokemonType[] types, bool enforce = false)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Default is Battle.Effect for source.
    /// </summary>
    public Species? SetSpecie(Species rawSpecies, IEffect? source, bool isTransform = false)
    {
        RelayVar? rv = Battle.RunEvent(EventId.ModifySpecie, this, null, source, rawSpecies);
        if (rv is null) return null;

        if (rv is SpecieRelayVar srv)
        {
            Species = srv.Species;
        }
        else
        {
            throw new InvalidOperationException("species must be a SpecieRelayVar" );
        }
        Species species = srv.Species;

        SetType(species.Types.ToArray(), true);
        ApparentType = rawSpecies.Types.ToList();
        AddedType = species.AddedType;
        KnownType = true;
        WeightHg = species.WeightHg;

        StatsTable stats = Battle.SpreadModify(Species.BaseStats, Set);
        if (Species.MaxHp is not null)
        {
            stats.Hp = Species.MaxHp.Value;
        }

        if (MaxHp != 0)
        {
            BaseMaxHp = stats.Hp;
            MaxHp = stats.Hp;
            Hp = stats.Hp;
        }

        if (!isTransform) BaseStoredStats = stats;
        foreach (var statName in StoredStats)
        {
            StoredStats[statName.Key] = stats[statName.Key.ConvertToStatId()];
        }

        Speed = StoredStats.Spe;
        return species;
    }

    public Item? GetItem()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Checks if the Pokemon is ignoring its held item due to various effects
    /// </summary>
    /// <param name="isFling">If true, this check is for Fling move (prevents infinite recursion)</param>
    /// <returns>True if the Pokemon is ignoring its item</returns>
    public bool IgnoringItem(bool isFling = false)
    {
        // Get the actual item object to check its properties
        Item? item = GetItem();

        // Primal Orbs are never ignored
        if (item?.IsPrimalOrb == true) return false;

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
            return item?.IgnoreKlutz != true;
        }

        return false;
    }

    public bool HasMove(MoveId move)
    {
        throw new NotImplementedException();
    }

    public void DisableMove(MoveId moveId, bool? isHidden = null, IEffect? sourceEffect = null)
    {
        throw new NotImplementedException();
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

    public MoveHitData GetMoveHitData(ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public bool IsGrounded(bool negateImmunity = false)
    {
        throw new NotImplementedException();
    }

    public bool IsSemiInvulnerable()
    {
        throw new NotImplementedException();
    }

    public double RunEffectiveness(ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public int GetWeight()
    {
        throw new NotImplementedException();
    }

    public Pokemon Copy()
    {
        throw new NotImplementedException();
    }
}


public record MoveHitData
{
    public bool Crit { get; init; }
    public double TypeMod { get; init; }
}