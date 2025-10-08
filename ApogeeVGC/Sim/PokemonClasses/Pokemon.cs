using ApogeeVGC.Data;
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


    // setStatus(
    // 	status: string | Condition,
    // 	source: Pokemon | null = null,
    // 	sourceEffect: Effect | null = null,
    // 	ignoreImmunities = false
    // ) {
    // 	if (!this.hp) return false;
    // 	status = this.battle.dex.conditions.get(status);
    // 	if (this.battle.event) {
    // 		if (!source) source = this.battle.event.source;
    // 		if (!sourceEffect) sourceEffect = this.battle.effect;
    // 	}
    // 	if (!source) source = this;

    // 	if (this.status === status.id) {
    // 		if ((sourceEffect as Move)?.status === this.status) {
    // 			this.battle.add('-fail', this, this.status);
    // 		} else if ((sourceEffect as Move)?.status) {
    // 			this.battle.add('-fail', source);
    // 			this.battle.attrLastMove('[still]');
    // 		}
    // 		return false;
    // 	}

    // 	if (
    // 		!ignoreImmunities && status.id && !(source?.hasAbility('corrosion') && ['tox', 'psn'].includes(status.id))
    // 	) {
    // 		// the game currently never ignores immunities
    // 		if (!this.runStatusImmunity(status.id === 'tox' ? 'psn' : status.id)) {
    // 			this.battle.debug('immune to status');
    // 			if ((sourceEffect as Move)?.status) {
    // 				this.battle.add('-immune', this);
    // 			}
    // 			return false;
    // 		}
    // 	}
    // 	const prevStatus = this.status;
    // 	const prevStatusState = this.statusState;
    // 	if (status.id) {
    // 		const result: boolean = this.battle.runEvent('SetStatus', this, source, sourceEffect, status);
    // 		if (!result) {
    // 			this.battle.debug('set status [' + status.id + '] interrupted');
    // 			return result;
    // 		}
    // 	}

    // 	this.status = status.id;
    // 	this.statusState = this.battle.initEffectState({ id: status.id, target: this });
    // 	if (source) this.statusState.source = source;
    // 	if (status.duration) this.statusState.duration = status.duration;
    // 	if (status.durationCallback) {
    // 		this.statusState.duration = status.durationCallback.call(this.battle, this, source, sourceEffect);
    // 	}

    // 	if (status.id && !this.battle.singleEvent('Start', status, this.statusState, this, source, sourceEffect)) {
    // 		this.battle.debug('status start [' + status.id + '] interrupted');
    // 		// cancel the setstatus
    // 		this.status = prevStatus;
    // 		this.statusState = prevStatusState;
    // 		return false;
    // 	}
    // 	if (status.id && !this.battle.runEvent('AfterSetStatus', this, source, sourceEffect, status)) {
    // 		return false;
    // 	}
    // 	return true;
    // }

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
        source ??= this;

        // Check for duplicate status
        if (Status is not null && Status == status.Id)
        {
            if (sourceEffect is ActiveMove move && move.Status == Status)
            {
                UiGenerator.PrintFailEvent(this, Battle.Library.Conditions[(ConditionId)Status]);
            }
            else if (sourceEffect is ActiveMove { Status: not null } move2)
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
                // Convert toxic to poison for immunity check
                PokemonType? immunityType = status.Id == ConditionId.Toxic ? PokemonType.Poison : null;
                var specialImmunityId = status.SpecialImmunity;

                if (!RunStatusImmunity(immunityType) && !RunStatusImmunity(specialImmunityId))
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
            if (result is BoolRelayVar brv && !brv.Value)
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

        if (source is not null)
            StatusState.Source = source;

        if (status.Duration is not null)
            StatusState.Duration = status.Duration;

        if (status.DurationCallback is not null)
        {
            StatusState.Duration = status.DurationCallback(Battle, this, source, sourceEffect);
        }

        // Run Start event (with rollback on failure)
        if (status.Id != ConditionId.None)
        {
            RelayVar? startResult = Battle.SingleEvent(EventId.Start, status, StatusState, this,
                source, sourceEffect);
            if (startResult is BoolRelayVar && startResult == false)
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
            RelayVar? afterResult = Battle.RunEvent(EventId.AfterSetStatus, this, source,
                sourceEffect, status);
            if (afterResult is BoolRelayVar { Value: false })
            {
                return false;
            }
        }

        return true;
    }

    // runStatusImmunity(type: string, message?: string) {
    // 	if (this.fainted) return false;
    // 	if (!type) return true;

    // 	if (!this.battle.dex.getImmunity(type, this)) {
    // 		this.battle.debug('natural status immunity');
    // 		if (message) {
    // 			this.battle.add('-immune', this);
    // 		}
    // 		return false;
    // 	}
    // 	const immunity = this.battle.runEvent('Immunity', this, null, null, type);
    // 	if (!immunity) {
    // 		this.battle.debug('artificial status immunity');
    // 		if (message && immunity !== null) {
    // 			this.battle.add('-immune', this);
    // 		}
    // 		return false;
    // 	}
    // 	return true;
    // }


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
    /// Checks special immunity effects (e.g., Prankster immunity for Dark-types)
    /// </summary>
    public bool RunStatusImmunity(SpecialImmunityId? immunityId, string? message = null)
    {
        // Check if Pokemon is fainted
        if (Fainted) return false;

        // Equivalent to TypeScript: if (!type) return true;
        if (immunityId == null) return true;

        // Check special immunity using battle's dex
        if (!Battle.Dex.GetSpecialImmunity(immunityId.Value, this))
        {
            if (Battle.PrintDebug)
            {
                // Battle.Debug("natural special immunity");
            }

            if (!string.IsNullOrEmpty(message))
            {
                UiGenerator.PrintImmuneEvent(this);
            }
            return false;
        }

        // Check artificial immunity (abilities, items, etc.) for special effects
        RelayVar? immunity = Battle.RunEvent(EventId.Immunity, this, null, null,
            immunityId);

        // TypeScript logic: if (!immunity) - means if immunity is falsy/false
        if (immunity is BoolRelayVar { Value: false } or null)
        {
            if (Battle.PrintDebug)
            {
                // Battle.Debug("artificial special immunity");
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

    public RelayVar AddVolatile(IBattle battle, Condition status, Pokemon? source = null,
        IEffect? sourceEffect = null, Condition? linkedStatus = null)
    {
        throw new NotImplementedException();
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

    public bool IgnoringAbility()
    {
        throw new NotImplementedException();
    }

    public StatIdExceptHp GetBestStat(bool? unboosted, bool? unmodified)
    {
        throw new NotImplementedException();
    }

    public bool HasAbility(AbilityId ability)
    {
        throw new NotImplementedException();
    }

    public bool HasAbility(AbilityId[] abilities)
    {
        throw new NotImplementedException();
    }

    public bool CureStatus(bool silent = false)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Unlike CureStatus, this does not give any cure messages.
    /// </summary>
    public bool ClearStatus()
    {
        throw new NotImplementedException();
    }

    public bool FormeChange(SpecieId specieId, IEffect? source, bool? isPermanent, int abilitySlot = 0,
        string? message = null)
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

    public bool IgnoringItem(bool isFling = false)
    {
        throw new NotImplementedException();
    }

    public bool HasMove(MoveId move)
    {
        throw new NotImplementedException();
    }

    public void DisableMove(MoveId moveId, bool? isHidden = null, IEffect? sourceEffect = null)
    {
        throw new NotImplementedException();
    }

    public bool HasItem(ItemId item)
    {
        throw new NotImplementedException();
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