using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Ui;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC.Sim.PokemonClasses;

public class Pokemon : IPriorityComparison
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

    public int Position { get; set; }

    public Species BaseSpecies { get; set; }
    public Species Species { get; set; }

    public EffectState SpeciesState { get; set; }

    public ConditionId Status { get; set; }
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
    public IntFalseUnion Order =>int.MaxValue;
    public int Priority => 0;
    public int Speed { get; set; }
    public int SubOrder => 0;
    public int EffectOrder => 0;

    public MoveTypeFalseUnion? CanTerastallize { get; set; }
    public MoveType TeraType { get; set; }
    public List<PokemonType> BaseTypes { get; set; }
    public MoveType? Terastallized { get; set; }

    public StalenessId? Staleness { get; set; }
    public StalenessId? PendingStaleness { get; set; }

    public StalenessId? VolatileStaleness
    {
        get;
        set // must be 'external' or null
        {
            if (value is PokemonClasses.StalenessId.External or null)
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

        SpeciesState = battle.InitEffectState(Species.Id);

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

        // Position = 0;
        StatusState = battle.InitEffectState();
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

        var canTerastallize = Battle.Actions.CanTerastallize(battle, this);
        CanTerastallize = canTerastallize ?? null;

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
        return SetStatus(Status == ConditionId.None ? status : ConditionId.None, source, sourceEffect);
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
                    // Battle.Debug($"set status [{status.Choice}] interrupted");
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
                    // Battle.Debug($"status start [{status.Choice}] interrupted");
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
            //source ??= Battle.Event.Source;
            sourceEffect ??= Battle.Event.Effect;
            if (source == null && Battle.Event.Source is PokemonSingleEventSource pses)
            {
                source = pses.Pokemon;
            }
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

    public Condition GetStatus()
    {
        return Battle.Library.Conditions[Status];
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
            stat = (int)Math.Floor(stat * boosts.GetBoostMultiplier(statName.ConvertToBoostId()));
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
        if (Hp <= 0) return false;

        // Add cure status message to battle log
        UiGenerator.PrintCureStatusEvent(this, Battle.Library.Conditions[Status]);

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
        if (Hp <= 0) return false;

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
        Status = ConditionId.None;
        StatusState = Battle.InitEffectState();

        return true;
    }

    //setItem(item: string | Item, source?: Pokemon, effect?: Effect)
    //{
    //    if (!this.hp || !this.isActive) return false;
    //    if (this.itemState.knockedOff && !(effect?.id === 'recycle')) return false;
    //    delete this.itemState.knockedOff;
    //    if (typeof item === 'string') item = this.battle.dex.items.get(item);

    //    const effectid = this.battle.effect ? this.battle.effect.id : '';
    //    if (RESTORATIVE_BERRIES.has('leppaberry' as ID))
    //    {
    //        const inflicted = ['trick', 'switcheroo'].includes(effectid);
    //        const external = inflicted && source && !source.isAlly(this);
    //        this.pendingStaleness = external ? 'external' : 'internal';
    //    }
    //    else
    //    {
    //        this.pendingStaleness = undefined;
    //    }
    //    const oldItem = this.getItem();
    //    const oldItemState = this.itemState;
    //    this.item = item.id;
    //    this.itemState = this.battle.initEffectState({ id: item.id, target: this });
    //    if (oldItem.exists) this.battle.singleEvent('End', oldItem, oldItemState, this);
    //    if (item.id)
    //    {
    //        this.battle.singleEvent('Start', item, this.itemState, this, source, effect);
    //    }
    //    return true;
    //}

    private static readonly HashSet<ItemId> RestorativeBerries =
    [
        ItemId.LeppaBerry,
        ItemId.AguavBerry,
        ItemId.EnigmaBerry,
        ItemId.FigyBerry,
        ItemId.IapapaBerry,
        ItemId.MagoBerry,
        ItemId.SitrusBerry,
    ];

    public bool SetItem(ItemId item, Pokemon? source = null, IEffect? effect = null)
    {
        // Early exit if Pokemon is fainted or not active
        if (Hp <= 0 || !IsActive) return false;

        // Check if item was knocked off (except for Recycle move)
        if (ItemState.KnockedOff == true && effect is ActiveMove { Id: MoveId.Recycle })
        {
            return false;
        }

        // Clear knocked off flag
        ItemState.KnockedOff = null;

        // Determine current effect ID
        EffectStateId effectId = Battle.Effect.EffectStateId;

        // Check if this is a restorative berry (like Leppa Berry)
        // Note: You'll need to define RESTORATIVE_BERRIES set/list somewhere
        if (RestorativeBerries.Contains(ItemId.LeppaBerry))
        {
            // Check if item was inflicted by Trick or Switcheroo
            bool inflicted = effectId is MoveEffectStateId { MoveId: MoveId.Trick or MoveId.Switcheroo};

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
            Battle.SingleEvent(EventId.End, oldItem, oldItemState, this);
        }

        // Trigger Start event on new item if it exists
        if (item != ItemId.None)
        {
            Battle.SingleEvent(EventId.Start, Battle.Library.Items[item], ItemState,
                this, SingleEventSource.FromNullablePokemon(source), effect);
        }

        return true;
    }

    public bool ClearItem()
    {
        return SetItem(ItemId.None);
    }

    public bool IsAlly(Pokemon? pokemon = null)
    {
        return pokemon != null && Side == pokemon.Side;
    }

    public List<Pokemon> AlliesAndSelf()
    {
        return Side.Allies();
    }

    public List<Pokemon> Allies()
    {
        return Side.Allies().Where(p => p != this).ToList();
    }

    public List<Pokemon> Foes(bool all = false)
    {
        return Side.Foes(all);
    }


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

    public AbilityIdFalseUnion? ClearAbility()
    {
        return SetAbility(AbilityId.None);
    }

    public void UpdateMaxHp()
    {
        int newBaseMaxHp = Battle.StatModify(Species.BaseStats, Set, StatId.Hp);
        if (newBaseMaxHp == BaseMaxHp) return;
        BaseMaxHp = newBaseMaxHp;
        int newMaxHp = BaseMaxHp;
        Hp = Hp <= 0 ? 0 : Math.Max(1, newMaxHp - (MaxHp - Hp));
        MaxHp = newMaxHp;
        if (Hp > 0)
        {
            UiGenerator.PrintHealEvent(this, Hp);
        }
    }

    public bool HasType(PokemonType type)
    {
        return GetTypes().Contains(type);
    }

    public bool HasType(PokemonType[] types)
    {
        return types.Any(t => GetTypes().Contains(t));
    }

    /// <summary>
    /// Sets a type (except on Arceus/Silvally, who resist type changes)
    /// </summary>
    public bool SetType(PokemonType type, bool enforce = false)
    {
        return SetType([type], enforce);
    }

    /// <summary>
    /// Sets types (except on Arceus/Silvally, who resist type changes)
    /// </summary>
    public bool SetType(PokemonType[] types, bool enforce = false)
    {
        if (!enforce)
        {
            // First type of Arceus, Silvally cannot be normally changed
            if ((Battle.Gen >= 5 && Species.Num is 493 or 773) || 
                (Battle.Gen == 4 && HasAbility(AbilityId.Multitype)))
            {
                return false;
            }

            // Terastallized Pokemon cannot have their base type changed except via forme change
            if (Terastallized != null)
            {
                return false;
            }
        }

        // Validate that types array is not empty
        if (types.Length == 0)
        {
            throw new ArgumentException("Must pass type to SetType");
        }

        // Set the new types
        Types = types.ToList();

        // Clear any added type
        AddedType = null;

        // Mark type as known
        KnownType = true;

        // Update apparent type for display (join types with '/')
        ApparentType = Types.ToList();

        return true;
    }

    public PokemonType[] GetTypes(bool? excludeAdded = null, bool? preterastallized = null)
    {
        // Handle Terastallization - match TypeScript's !preterastallized logic
        if (preterastallized != true && Terastallized is not null && Terastallized != MoveType.Stellar)
        {
            return [Terastallized.Value.ConvertToPokemonType()];
        }

        // Run Type event to allow abilities/items/conditions to modify types
        RelayVar? rv = Battle.RunEvent(EventId.Type, this, null, null, Types);

        List<PokemonType> resultTypes;
        if (rv is TypesRelayVar typesRelayVar)
        {
            resultTypes = typesRelayVar.Types.ToList();
        }
        else
        {
            // Fallback to current types if event doesn't return expected type
            // This matches TypeScript behavior where unexpected event results are ignored
            resultTypes = Types.ToList();
        }

        // Add fallback type if no types exist
        if (resultTypes.Count == 0)
        {
            resultTypes.Add(Battle.Gen >= 5 ? PokemonType.Normal : PokemonType.Unknown);
        }

        // Add the added type if it exists and not excluded
        if (excludeAdded != true && AddedType is not null)
        {
            resultTypes.Add(AddedType.Value);
        }

        return resultTypes.ToArray();
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

    public Item GetItem()
    {
        return Battle.Library.Items[Item];
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

    public bool HasMove(MoveId move)
    {
        return MoveSlots.Any(ms => ms.Id == move);
    }

    public void DisableMove(MoveId moveId, bool isHidden = false, IEffect? sourceEffect = null)
    {
        if (sourceEffect is not null && Battle.Event is not null)
        {
            sourceEffect = Battle.Event.Effect;
        }

        foreach (MoveSlot moveSlot in MoveSlots.Where(moveSlot =>
                     moveSlot.Id != moveId && moveSlot.Disabled != true))
        {
            moveSlot.Disabled = isHidden ? BoolHiddenUnion.FromHidden() : true;
            moveSlot.DisabledSource = sourceEffect ?? Battle.Library.Moves[moveSlot.Move].ToActiveMove();
        }
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

    /// <summary>
    /// Gets the full slot identifier combining side ID and position letter.
    /// Simplified version for standard battle formats.
    /// </summary>
    public PokemonSlot GetSlot()
    {
        int poistionOffset = (int)Math.Floor(Side.N / 2.0) * Side.Active.Count;
        return new PokemonSlot(Side.Id, poistionOffset);
    }

    /// <summary>
    /// Gets or creates the move hit data for this Pokemon's slot.
    /// Tracks per-target information like critical hits and type effectiveness.
    /// </summary>
    public MoveHitResult GetMoveHitData(ActiveMove move)
    {
        // Lazy initialization of the moveHitData dictionary if it doesn't exist
        move.MoveHitData ??= new MoveHitData();

        // Get this Pokemon's slot identifier
        PokemonSlot slot = GetSlot();

        // Try to get existing hit data for this slot
        if (!move.MoveHitData.TryGetValue(slot, out MoveHitResult? hitResult))
        {
            // Create default hit data if it doesn't exist
            hitResult = new MoveHitResult
            {
                Crit = false,
                TypeMod = 0,
                ZBrokeProtect = false,
            };

            // Store it in the dictionary
            move.MoveHitData[slot] = hitResult;
        }

        return hitResult;
    }

    /// <summary>
    /// Checks if the Pokemon is grounded (affected by Ground-type moves and terrain).
    /// Returns true if grounded, false if not grounded, null if Levitate provides immunity.
    /// </summary>
    /// <param name="negateImmunity">If true, ignore type-based immunity (for moves like Thousand Arrows)</param>
    /// <returns>True if grounded, false if not grounded, null if Levitate ability</returns>
    public bool? IsGrounded(bool negateImmunity = false)
    {
        // Gravity forces all Pokemon to be grounded
        if (Battle.Field.PseudoWeather.ContainsKey(ConditionId.Gravity))
        {
            return true;
        }

        // Ingrain grounds the Pokemon (Gen 4+)
        if (Volatiles.ContainsKey(ConditionId.Ingrain) && Battle.Gen >= 4)
        {
            return true;
        }

        // Smackdown grounds the Pokemon
        if (Volatiles.ContainsKey(ConditionId.SmackDown))
        {
            return true;
        }

        // Get effective item (empty if ignoring item)
        ItemId effectiveItem = IgnoringItem() ? ItemId.None : Item;

        // Iron Ball grounds the Pokemon
        if (effectiveItem == ItemId.IronBall)
        {
            return true;
        }

        // Flying-type immunity check (unless negated)
        // Special case: Fire/Flying using Burn Up + Roost becomes ???/Flying but is still grounded
        if (!negateImmunity && HasType(PokemonType.Flying))
        {
            // Exception: ???-type + Roost active means it's still grounded
            bool roosting = HasType(PokemonType.Unknown) && Volatiles.ContainsKey(ConditionId.Roost);
            if (!roosting)
            {
                return false;
            }
        }

        // Levitate ability provides immunity (unless ability is being suppressed)
        if (HasAbility(AbilityId.Levitate))
        {
            return null; // Special return value indicating Levitate immunity
        }

        // Magnet Rise makes Pokemon airborne
        if (Volatiles.ContainsKey(ConditionId.MagnetRise))
        {
            return false;
        }

        // Telekinesis makes Pokemon airborne
        if (Volatiles.ContainsKey(ConditionId.Telekinesis))
        {
            return false;
        }

        // Air Balloon makes Pokemon airborne (unless popped)
        return effectiveItem != ItemId.AirBalloon;
    }

    /// <summary>
    /// Checks if this Pokemon is affected by Sky Drop (either as target or source).
    /// </summary>
    public bool IsSkyDropped()
    {
        // Check if this Pokemon is the target of Sky Drop
        if (Volatiles.ContainsKey(ConditionId.SkyDrop))
        {
            return true;
        }

        // Check if this Pokemon is the source of Sky Drop on any opponent
        return Side.Foe.Active.Any(foeActive =>
            foeActive.Volatiles.TryGetValue(ConditionId.SkyDrop, out EffectState? state) &&
            state.Source == this);
    }

    /// <summary>
    /// Checks if the Pokemon is semi-invulnerable (untargetable due to two-turn moves).
    /// </summary>
    public bool IsSemiInvulnerable()
    {
        // List of all semi-invulnerable conditions
        ConditionId[] semiInvulnerableConditions =
        [
            ConditionId.Fly,
            ConditionId.Bounce,
            ConditionId.Dive,
            ConditionId.Dig,
            ConditionId.PhantomForce,
            ConditionId.ShadowForce,
        ];

        return semiInvulnerableConditions.Any(Volatiles.ContainsKey) || IsSkyDropped();
    }

    /// <summary>
    /// Calculates the type effectiveness for a move against this Pokemon.
    /// Returns a MoveEffectiveness enum value representing the combined effectiveness.
    /// </summary>
    /// <param name="move">The move to check effectiveness for</param>
    /// <returns>MoveEffectiveness enum value (Normal, SuperEffective2X, NotVeryEffective05X, etc.)</returns>
    public MoveEffectiveness RunEffectiveness(ActiveMove move)
    {
        int totalTypeMod = 0;

        // Special case: Stellar-type moves against Terastallized Pokemon are always neutral
        if (Terastallized != null && move.Type == MoveType.Stellar)
        {
            totalTypeMod = 1;
        }
        else
        {
            // Calculate effectiveness against each of the Pokemon's types
            foreach (PokemonType type in GetTypes())
            {
                // Get base effectiveness from ModdedDex (returns MoveEffectiveness enum)
                MoveEffectiveness effectiveness = Battle.Dex.GetEffectiveness(move.Type, type);

                // Convert MoveEffectiveness enum to integer modifier for event system
                int typeMod = effectiveness.ToModifier();

                // Allow SingleEvent to modify effectiveness (e.g., Scrappy ability)
                RelayVar? singleEventResult = Battle.SingleEvent(
                    EventId.Effectiveness,
                    move,
                    null,
                    this,
                    type,
                    move,
                    typeMod
                );

                if (singleEventResult is IntRelayVar singleIrv)
                {
                    typeMod = singleIrv.Value;
                }

                // Allow RunEvent to further modify effectiveness
                RelayVar? runEventResult = Battle.RunEvent(
                    EventId.Effectiveness,
                    this,
                    type,
                    move,
                    typeMod
                );

                if (runEventResult is IntRelayVar runIrv)
                {
                    totalTypeMod += runIrv.Value;
                }
                else
                {
                    totalTypeMod += typeMod;
                }
            }
        }

        // Special handling for Terapagos-Terastal with Tera Shell ability
        // ✅ FIXED: Changed != false to !
        if (Species.Id == SpecieId.TerapagosTerastal && HasAbility(AbilityId.TeraShell) &&
            !Battle.SuppressingAbility(this))
        {
            // If ability already triggered, keep effectiveness at not very effective
            if (AbilityState.Resisted == true)
            {
                return MoveEffectiveness.NotVeryEffective05X; // All hits of multi-hit move should be not very effective
            }

            // Tera Shell only activates for damaging moves at full HP
            // It doesn't activate if:
            // - Move is Status category
            // - Move is Struggle
            // - Pokemon is immune to the move
            // - Effectiveness is already negative (resisted)
            // - Pokemon is not at full HP
            if (move.Category == MoveCategory.Status ||
                move.Id == MoveId.Struggle ||
                !RunImmunity(move) ||
                totalTypeMod < 0 ||
                Hp < MaxHp)
            {
                return totalTypeMod.ToMoveEffectiveness();
            }

            // Activate Tera Shell - make move not very effective
            UiGenerator.PrintActivateEvent(this, Battle.Library.Abilities[AbilityId.TeraShell]);
            AbilityState.Resisted = true;
            return MoveEffectiveness.NotVeryEffective05X;
        }

        // Convert accumulated integer modifier back to MoveEffectiveness enum
        return totalTypeMod.ToMoveEffectiveness();
    }

    /// <summary>
    /// Checks if the Pokemon is immune to a specific move type.
    /// </summary>
    /// <param name="source">The move or type string to check immunity against</param>
    /// <param name="message">If true/non-empty, display immunity messages</param>
    /// <returns>True if not immune (move can hit), false if immune</returns>
    public bool RunImmunity(ActiveMove source, bool message = false)
    {
        return RunImmunity(source.Type, message);
    }

    /// <summary>
    /// Checks if the Pokemon is immune to a specific type.
    /// </summary>
    /// <param name="source">The type to check immunity against</param>
    /// <param name="message">If true, display immunity messages</param>
    /// <returns>True if not immune, false if immune</returns>
    public bool RunImmunity(MoveType source, bool message = false)
    {
        // Null or unknown type is never immune
        if (source == MoveType.Unknown)
        {
            return true;
        }

        // Run NegateImmunity event
        RelayVar? negateEvent = Battle.RunEvent(EventId.NegateImmunity, this, null, null,
            source);
        bool negateImmunity = negateEvent is not BoolRelayVar { Value: true };

        // Special handling for Ground-type
        bool? notImmune;
        if (source == MoveType.Ground)
        {
            notImmune = IsGrounded(negateImmunity);
        }
        else
        {
            // Check type immunity using the dex
            notImmune = negateImmunity || Battle.Dex.GetImmunity(source, this);
        }

        // If not immune, return true
        if (notImmune == true)
        {
            return true;
        }

        // If no message requested, just return false
        if (!message)
        {
            return false;
        }

        // Display appropriate immunity message
        if (notImmune == null)
        {
            // Levitate ability immunity
            UiGenerator.PrintImmuneEvent(this, Battle.Library.Abilities[AbilityId.Levitate]);
        }
        else
        {
            // General immunity
            UiGenerator.PrintImmuneEvent(this);
        }

        return false;
    }



    /// <summary>
    /// Gets the Pokemon's current weight in hectograms, accounting for modifying effects.
    /// Weight is used for moves like Heavy Slam, Grass Knot, and Low Kick.
    /// Minimum weight is 1 hectogram (0.1 kg).
    /// </summary>
    /// <returns>Weight in hectograms (1 hg = 0.1 kg)</returns>
    public int GetWeight()
    {
        // Run ModifyWeight event to allow abilities/items/conditions to modify weight
        RelayVar? weightEvent = Battle.RunEvent(EventId.ModifyWeight, this, null, null,
            WeightHg);

        int modifiedWeight;
        if (weightEvent is IntRelayVar irv)
        {
            modifiedWeight = irv.Value;
        }
        else
        {
            // Fallback to base weight if event doesn't return an integer
            modifiedWeight = WeightHg;
        }

        // Ensure minimum weight of 1 hectogram
        return Math.Max(1, modifiedWeight);
    }

    /// <summary>
    /// Calculates a Pokemon's stat value with boosts and modifiers applied.
    /// This is used during battle for damage calculations and speed comparisons.
    /// </summary>
    /// <param name="statName">The stat to calculate (Atk, Def, SpA, SpD, Spe)</param>
    /// <param name="boost">Boost stage from -6 to +6</param>
    /// <param name="modifier">Optional modifier to apply (e.g., 1.5 for 50% increase)</param>
    /// <param name="statUser">Pokemon whose stats are being calculated (defaults to this)</param>
    /// <returns>The calculated stat value</returns>
    public int CalculateStat(StatIdExceptHp statName, int boost, int? modifier = null, Pokemon? statUser = null)
    {
        // Get base stat from stored stats
        int stat = StoredStats[statName];

        // Wonder Room swaps Defense and Special Defense BEFORE any other calculations
        if (Battle.Field.PseudoWeather.ContainsKey(ConditionId.WonderRoom))
        {
            stat = statName switch
            {
                StatIdExceptHp.Def => StoredStats[StatIdExceptHp.SpD],
                StatIdExceptHp.SpD => StoredStats[StatIdExceptHp.Def],
                _ => stat,
            };
        }

        // Create sparse boosts table with only the stat we're calculating
        var boosts = new SparseBoostsTable();
        BoostId boostName = statName.ConvertToBoostId();
        boosts.SetBoost(boostName, boost);

        // Run ModifyBoost event to allow abilities/items/conditions to modify boosts
        // Example: Simple ability doubles boost stages
        RelayVar? boostEvent = Battle.RunEvent(EventId.ModifyBoost, statUser ?? this, null,
            null, boosts);

        if (boostEvent is SparseBoostsTableRelayVar brv)
        {
            boosts = brv.Table;
            boost = boosts.GetBoost(boostName) ?? 0;
        }

        // Clamp boost to valid range
        boost = BoostsTable.ClampBoost(boost);

        // Apply boost multiplier
        if (boost >= 0)
        {
            // Positive boosts multiply the stat
            stat = (int)Math.Floor(stat * BoostsTable.CalculateRegularStatMultiplier(boost));
        }
        else
        {
            // Negative boosts divide the stat
            stat = (int)Math.Floor(stat / BoostsTable.CalculateRegularStatMultiplier(-boost));
        }

        // Apply modifier using battle's modify function
        return Battle.Modify(stat, modifier ?? 1);
    }

    public SparseBoostsTable GetCappedBoost(SparseBoostsTable boosts)
    {
        SparseBoostsTable cappedBoost = new();
        foreach ((BoostId boostId, int value) in boosts.GetNonNullBoosts())
        {
            // Get current boost value for this stat
            int currentBoost = Boosts.GetBoost(boostId);
            
            // Calculate capped boost: clamp(current + incoming) - current
            // This gives us the actual amount we can boost (respecting -6 to +6 limits)
            int cappedValue = Battle.ClampIntRange(currentBoost + value, -6, 6) - currentBoost;
            cappedBoost.SetBoost(boostId, cappedValue);
        }
        return cappedBoost;
    }

    /// <summary>
    /// Applies stat boosts to this Pokemon, respecting the -6 to +6 boost limits.
    /// Returns the delta (change amount) of the last boost that was applied.
    /// </summary>
    /// <param name="boosts">The boosts to apply (will be capped to valid ranges)</param>
    /// <returns>The change amount of the last stat that was boosted (0 if no boosts were applied)</returns>
    public int BoostBy(SparseBoostsTable boosts)
    {
        // Cap all boosts to respect -6 to +6 limits
        boosts = GetCappedBoost(boosts);
        
        int delta = 0;
        
        // Apply each boost to the Pokemon's current boosts
        foreach ((BoostId boostId, int value) in boosts.GetNonNullBoosts())
        {
            delta = value;
            Boosts.SetBoost(boostId, Boosts.GetBoost(boostId) + delta);
        }
        
        // Return the last delta (change amount of the last stat modified)
        return delta;
    }

    public Pokemon GetAtLoc(int targetLoc)
    {
        // Determine which side based on targetLoc sign
        Side side = Battle.Sides[targetLoc < 0 ? Side.N % 2 : (Side.N + 1) % 2];

        // Use absolute value for position calculation
        targetLoc = Math.Abs(targetLoc);

        // Handle wrap-around for multi-battle formats (e.g., if position exceeds active Pokemon count)
        if (targetLoc > side.Active.Count)
        {
            targetLoc -= side.Active.Count;
            side = Battle.Sides[side.N + 2];
        }

        // Return the Pokemon at the calculated position (adjust for 0-based indexing)
        return side.Active[targetLoc - 1];
    }

    public Pokemon Copy()
    {
        throw new NotImplementedException();
    }
}