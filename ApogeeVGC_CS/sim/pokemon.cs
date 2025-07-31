using System.Resources;

namespace ApogeeVGC_CS.sim
{
    public class MoveSlot
    {
        public required Id Id { get; init; }
        public required string Move { get; init; }
        public required int Pp { get; init; }
        public required int MaxPp { get; init; }
        public string? Target { get; init; }

        public required object Disabled
        {
            get;
            init // bool or string
            {
                if (value is bool or string)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("Disabled must be a bool or string");
                }
            }
        }

        public string? DisabledSource { get; init; }
        public required bool Used { get; init; }
        public bool? Virtual { get; init; }
    }

    public class Attacker
    {
        public required Pokemon Source { get; init; }
        public required int Damage { get; init; }
        public required bool ThisTurn { get; init; }
        public Id? Move { get; init; }
        public required PokemonSlot Slot { get; init; }

        public object? DamageValue
        {
            get;
            init // int, bool, or null
            {
                if (value is int or bool or null)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("DamageValue must be an int, bool, or null");
                }
            }
        }
    }

    public class EffectState
    {
        public required Id Id { get; init; }
        public required int EffectOrder { get; init; }
        public int? Duration { get; init; }
        public Dictionary<string, object>? ExtraData { get; init; }
    }

    public static class PokemonConstants
    {
        public static readonly HashSet<Id> RestorativeBerries =
        [
            new("leppaberry"), new("aguavberry"), new("enigmaberry"),
            new("figyberry"), new("iapapaberry"), new("magoberry"),
            new("sitrusberry"), new("wikiberry"), new("oranberry")
        ];
    }

    // helper struct for GetFullDetails()
    public struct FullDetails
    {
        public SideId Side { get; init; }
        public string Secret { get; init; }
        public string Shared { get; init; }
    }

    // helper struct for GetMoveTargets()
    public struct MoveTargets
    {
        public List<Pokemon> Targets { get; init; }
        public List<Pokemon> PressureTargets { get; init; }
    }

    public class Pokemon
    {
        public Side Side { get; }
        public Battle Battle { get; }

        public required PokemonSet Set { get; init; }
        public required string Name { get; init; }
        public string Fullname => $"{Side.Id}: {Name}";
        public required int Level { get; init; }
        public required GenderName Gender { get; init; }
        public required int Happiness { get; init; }
        public required Id Pokeball { get; init; }
        public required int DynamaxLevel { get; init; }
        public required bool Gigantamax { get; init; }

        public required string BaseHpType { get; init; }
        public required int BaseHpPower { get; init; }

        public required List<MoveSlot> BaseMoveSlots { get; init; }
        public required List<MoveSlot> MoveSlots { get; init; }

        public required string HpType { get; init; }
        public required int HpPower { get; init; }

        public required int Position { get; init; }
        public required string Details { get; init; }

        public required Species BaseSpecies { get; init; }
        public required Species Species { get; init; }
        public required EffectState SpeciesState { get; init; }

        public required Id Status { get; init; }
        public required EffectState StatusState { get; init; }
        public required Dictionary<string, EffectState> Volatiles { get; init; }
        public bool? ShowCure { get; init; }

        public required StatsTable BaseStoredStats { get; init; }
        public required StatsTable StoredStats { get; init; }
        public required BoostsTable Boosts { get; init; }

        public required Id BaseAbility { get; init; }
        public required Id Ability { get; init; }
        public required EffectState AbilityState { get; init; }

        public required Id Item { get; init; }
        public required EffectState ItemState { get; init; }
        public required Id LastItem { get; init; }
        public required bool UsedItemThisTurn { get; init; }
        public required bool AteBerry { get; init; }

        public required object Trapped
        {
            get;
            init // bool or "hidden"
            {
                if (value is bool or "hidden")
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("Trapped must be a bool or 'hidden'");
                }
            }
        }

        public required bool MaybeTrapped { get; init; }
        public required bool MaybeDisabled { get; init; }
        public bool? MaybeLocked { get; init; }

        public Pokemon? Illusion { get; init; }
        public required bool Transformed { get; init; }

        public required int MaxHp { get; init; }
        public required int BaseMaxHp { get; init; }
        public required int Hp { get; init; }
        public required bool Fainted { get; init; }
        public required bool FaintQueued { get; init; }
        public bool? SubFainted { get; init; }

        public required bool FormeRegression { get; init; }

        public required List<string> Types { get; init; }
        public required string AddedType { get; init; }
        public required bool KnownType { get; init; }
        public required string ApparentType { get; init; }

        public required object SwitchFlag
        {
            get;
            init // Id or bool
            {
                if (value is Id or bool)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("SwitchFlag must be an Id or bool");
                }
            }
        }

        public required bool ForceSwitchFlag { get; init; }
        public required bool SkipBeforeSwitchOutEventFlag { get; init; }
        public int? DraggedIn { get; init; }
        public required bool NewlySwitched { get; init; }
        public required bool BeingCalledBack { get; init; }

        public ActiveMove? LastMove { get; init; }
        public ActiveMove? LastMoveEncore { get; init; } // Gen 2 only
        public ActiveMove? LastMoveUsed { get; init; }
        public int? LastMoveTargetLoc { get; init; }

        public required object MoveThisTurn
        {
            get;
            init // string or bool
            {
                if (value is string or bool)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("MoveThisTurn must be a string or bool");
                }
            }
        }

        public required bool StatsRaisedThisTurn { get; init; }
        public required bool StatsLoweredThisTurn { get; init; }
        public bool? MoveLastTurnResult { get; init; }
        public bool? MoveThisTurnResult { get; init; }
        public int? HurtThisTurn { get; init; }
        public required int LastDamage { get; init; }
        public required List<Attacker> AttackedBy { get; init; }
        public required int TimesAttacked { get; init; }

        public required bool IsActive { get; init; }
        public required int ActiveTurns { get; init; }
        public required int ActiveMoveActions { get; init; }
        public required int PreviouslySwitchedIn { get; init; }
        public required bool TruantTurn { get; init; }
        public required bool BondTriggered { get; init; }
        public required bool SwordBoost { get; init; } // Gen 9 only
        public required bool ShieldBoost { get; init; } // Gen 9 only
        public required bool SyrupTriggered { get; init; } // Gen 9 only
        public required List<string> StellarBoostedTypes { get; init; } // Gen 9 only

        public required bool IsStarted { get; init; }
        public required bool DuringMove { get; init; }

        public required double WeightHg { get; init; }
        public required int Speed { get; init; }

        public string? CanMegaEvo { get; init; }
        public string? CanMegaEvoX { get; init; }
        public string? CanMegaEvoY { get; init; }
        public string? CanUltraBurst { get; init; }
        public string? CanGigantamax { get; }

        public object? CanTerastallize
        {
            get;
            init // string, false, or null
            {
                if (value is string or false or null)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("CanTerastallize must be a string, false, or null");
                }
            }
        }

        public required string TeraType { get; init; }
        public required List<string> BaseTypes { get; init; }
        public string? Terastallized { get; init; }

        public string? Staleness
        {
            get;
            init // "internal" or "external"
            {
                if (value is "internal" or "external" or null)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("Staleness must be 'internal', 'external', or null");
                }
            }
        }

        public string? PendingStaleness
        {
            get;
            init // "internal" or "external"
            {
                if (value is "internal" or "external" or null)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("PendingStaleness must be 'internal', 'external', or null");
                }
            }
        }

        public string? VolatileStaleness
        {
            get;
            init // "external"
            {
                if (value is "external" or null)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("VolatileStaleness must be 'external' or null");
                }
            }
        }

        // Gen 1 only
        public StatsTable? ModifiedStats { get; init; }

        public Action<StatIdExceptHp, int>? ModifyStat { get; init; }

        // Stadium only
        public Action? RecalculateStats { get; init; }

        public Dictionary<string, object> M { get; init; } = [];

        public Pokemon(string set, Side side)
        {
            Side = side;
            Battle = side.Battle;

            BaseSpecies = Battle.Dex.Species.Get(set);
            if (!BaseSpecies.Exists)
                throw new ArgumentException($"Unidentified species: {BaseSpecies.Name}");

            Set = new PokemonSet
            {
                Species = BaseSpecies.Name,
                Moves =
                    [],
                Name = set,
                Item = string.Empty,
                Ability = string.Empty,
                Nature = string.Empty,
                Gender = string.Empty,
                Evs = new StatsTable(),
                Ivs = new StatsTable(),
                Level = 0
            };

            Species = BaseSpecies;

        }

        //public Pokemon(PokemonSet set, Side side)
        //{
        //    Side = side;
        //    Battle = side.Battle;

        //    // Handle Pokemon scripts from format/dex if needed

        //    if (set is string setName)
        //        set = new { name = setName };

        //    var setData = (dynamic)set;
        //    BaseSpecies = Battle.Dex.Species.Get(setData.species ?? setData.name);
        //    if (!BaseSpecies.Exists)
        //        throw new ArgumentException($"Unidentified species: {BaseSpecies.Name}");

        //    Set = (PokemonSet)set;
        //    Species = BaseSpecies;

        //    if (setData.name == setData.species || string.IsNullOrEmpty(setData.name))
        //        setData.name = BaseSpecies.BaseSpecies;
        //}

        public object ToJson()
        {
            throw new NotImplementedException();
        }

        public List<string> GetMoves()
        {
            throw new NotImplementedException();
        }

        public List<string> GetBaseMoves()
        {
            throw new NotImplementedException();
        }

        public PokemonSlot GetSlot()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public string GetUpdatedDetails(int? level = null)
        {
            throw new NotImplementedException();
        }

        public FullDetails GetFullDetails()
        {
            throw new NotImplementedException();
        }

        public void UpdateSpeed()
        {
            throw new NotImplementedException();
        }

        public int CalculateStat(StatIdExceptHp statName, int boost, double? modifier = null,
            Pokemon? statUser = null)
        {
            throw new NotImplementedException();
        }

        public int GetStat(StatIdExceptHp statName, bool? unboosted = null, bool? unmodified = null)
        {
            throw new NotImplementedException();
        }

        public int GetActionSpeed()
        {
            throw new NotImplementedException();
        }

        public StatIdExceptHp GetBestStat(bool? unboosted = null, bool? unmodified = null)
        {
            throw new NotImplementedException();
        }

        public double GetWeight()
        {
            throw new NotImplementedException();
        }

        public MoveSlot? GetMoveSlot(string move)
        {
            throw new NotImplementedException();
        }

        public MoveSlot? GetMoveSlot(Move move)
        {
            throw new NotImplementedException();
        }

        public MoveHitResult GetMoveHitData(ActiveMove move)
        {
            throw new NotImplementedException();
        }

        public List<Pokemon> AlliesAndSelf()
        {
            throw new NotImplementedException();
        }

        public List<Pokemon> Allies()
        {
            throw new NotImplementedException();
        }
        public List<Pokemon> AdjacentAllies()
        {
            throw new NotImplementedException();
        }

        public List<Pokemon> Foes(bool? all = null)
        {
            throw new NotImplementedException();
        }

        public List<Pokemon> AdjacentFoes()
        {
            throw new NotImplementedException();
        }

        public bool IsAlly(Pokemon? pokemon = null)
        {
            throw new NotImplementedException();
        }

        public int GetUndynamaxedHp(int? amount = null)
        {
            throw new NotImplementedException();
        }

        public List<Pokemon> GetSmartTargets(Pokemon target, ActiveMove move)
        {
            throw new NotImplementedException();
        }

        public Pokemon GetAtLoc(int targetLoc)
        {
            throw new NotImplementedException();
        }

        public int GetLocOf(Pokemon target)
        {
            throw new NotImplementedException();
        }

        public MoveTargets GetMoveTargets(ActiveMove move, Pokemon target)
        {
            throw new NotImplementedException();
        }

        public bool IgnoringAbility()
        {
            throw new NotImplementedException();
        }

        public bool IgnoringItem(bool isFling = false)
        {
            throw new NotImplementedException();
        }

        public int DeductPp(string move, Pokemon target, int? amount = null)
        {
            throw new NotImplementedException();
        }

        public int DeductPp(Move move, Pokemon target, int? amount = null)
        {
            throw new NotImplementedException();
        }

        public int DeductPp(string move, bool target, int? amount = null)
        {
            throw new NotImplementedException();
        }

        public int DeductPp(Move move, bool target, int? amount = null)
        {
            throw new NotImplementedException();
        }

        public int DeductPp(string move, int? amount = null)
        {
            throw new NotImplementedException();
        }

        public int DeductPp(Move move, int? amount = null)
        {
            throw new NotImplementedException();
        }

        public void MoveUsed(ActiveMove move, int? targetLoc = null)
        {
            throw new NotImplementedException();
        }

        public void GotAttacked(string move, Pokemon source, int damage)
        {
            throw new NotImplementedException();
        }

        public void GotAttacked(Move move, Pokemon source, int damage)
        {
            throw new NotImplementedException();
        }

        public void GotAttacked(string move, Pokemon source, bool damage)
        {
            throw new NotImplementedException();
        }

        public void GotAttacked(Move move, Pokemon source, bool damage)
        {
            throw new NotImplementedException();
        }

        public void GotAttacked(string move, Pokemon source)
        {
            throw new NotImplementedException();
        }

        public void GotAttacked(Move move, Pokemon source)
        {
            throw new NotImplementedException();
        }

        public Attacker? GetLastAttackedBy()
        {
            throw new NotImplementedException();
        }

        public Attacker? GetLastDamagedBy(bool filterOutSameSide)
        {
            throw new NotImplementedException();
        }

        public Id? GetLockedMove()
        {
            throw new NotImplementedException();
        }

        public List<MoveSlot> GetMoves(Id? lockedMove = null, bool? restricData = null)
        {
            throw new NotImplementedException();
        }

        public bool MaxMoveDisabled(string baseMove)
        {
            throw new NotImplementedException();
        }

        public bool MaxMoveDisabled(Move baseMove)
        {
            throw new NotImplementedException();
        }

        public DynamaxOptions? GetDynamaxRequest(bool? skipChecks = null)
        {
            throw new NotImplementedException();
        }

        public PokemonMoveRequestData GetMoveRequestData()
        {
            throw new NotImplementedException();
        }

        public PokemonSwitchRequestData GetSwitchRequestData(bool? forAlly)
        {
            throw new NotImplementedException();
        }

        public bool IsLastActive()
        {
            throw new NotImplementedException();
        }

        public int PositiveBoosts()
        {
            throw new NotImplementedException();
        }

        public BoostsTable GetCappedBoost(SparseBoostsTable boosts)
        {
            throw new NotImplementedException();
        }

        public int BoostBy(SparseBoostsTable boosts)
        {
            throw new NotImplementedException();
        }

        public void ClearBoosts()
        {
            throw new NotImplementedException();
        }

        public void SetBoost(SparseBoostsTable boosts)
        {
            throw new NotImplementedException();
        }

        public void CopyVolatileFrom(Pokemon pokemon, string switchCause)
        {
            throw new NotImplementedException();
        }

        public void CopyVolatileFrom(Pokemon pokemon, bool switchCause)
        {
            throw new NotImplementedException();
        }

        public void CopyVolatileFrom(Pokemon pokemon)
        {
            throw new NotImplementedException();
        }

        public bool TransformInto(Pokemon pokemon, IEffect? effect = null)
        {
            throw new NotImplementedException();
        }

        public Species? SetSpecies(Species rawSpecies, IEffect? source = null, bool isTransform = false)
        {
            throw new NotImplementedException();
        }

        public bool FormeChange(Species speciesId, IEffect? source = null, bool? isPermanent = null,
            string abilitySlot = "0", string? message = null)
        {
            throw new NotImplementedException();
        }

        public bool FormeChange(string speciesId, IEffect? source = null, bool? isPermanent = null,
            string abilitySlot = "0", string? message = null)
        {
            throw new NotImplementedException();
        }

        public void ClearVolatile(bool includeSwitchFlags = true)
        {
            throw new NotImplementedException();
        }

        public bool HasType(string type)
        {
            throw new NotImplementedException();
        }

        public bool HasType(List<string> types)
        {
            throw new NotImplementedException();
        }

        public int Faint(Pokemon? pokemon = null, IEffect? effect = null)
        {
            throw new NotImplementedException();
        }

        public int Damage(int d, Pokemon? source = null, IEffect? effect = null)
        {
            throw new NotImplementedException();
        }

        public bool TryTrap(bool isHidden = false)
        {
            throw new NotImplementedException();
        }

        public string? HasMove(string moveId)
        {
            throw new NotImplementedException();
        }

        public void DisableMove(string moveId, bool isHidden, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException();
        }

        public void DisableMove(string moveId, string isHidden, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException();
        }

        public int? Heal(int d, Pokemon? source, IEffect? effect = null)
        {
            throw new NotImplementedException();
        }

        public int? SetHp(int d)
        {
            throw new NotImplementedException();
        }

        public bool TrySetStatus(string status, Pokemon? source = null, IEffect? effect = null)
        {
            throw new NotImplementedException();
        }

        public bool TrySetStatus(Condition status, Pokemon? source = null, IEffect? effect = null)
        {
            throw new NotImplementedException();
        }

        public bool CureStatus(bool silent = false)
        {
            throw new NotImplementedException();
        }

        public bool SetStatus(string status, Pokemon? source = null, IEffect? sourceEffect = null,
            bool ignoreImmunities = false)
        {
            throw new NotImplementedException();
        }

        public bool SetStatus(Condition status, Pokemon? source = null, IEffect? sourceEffect = null,
            bool ignoreImmunities = false)
        {
            throw new NotImplementedException();
        }

        public bool ClearStatus()
        {
            throw new NotImplementedException();
        }

        public Condition GetStatus()
        {
            throw new NotImplementedException();
        }

        public bool EatItem(bool? force = null, Pokemon? source = null, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException();
        }

        public bool UseItem(Pokemon? source = null, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException();
        }

        public Item? TakeItem(Pokemon? source = null)
        {
            throw new NotImplementedException();
        }

        public bool SetItem(string item, Pokemon? source = null, IEffect? effect = null)
        {
            throw new NotImplementedException();
        }

        public bool SetItem(Item item, Pokemon? source = null, IEffect? effect = null)
        {
            throw new NotImplementedException();
        }

        public Item GetItem()
        {
            throw new NotImplementedException();
        }

        public bool HasItem(string item)
        {
            throw new NotImplementedException();
        }

        public bool HasItem(List<string> items)
        {
            throw new NotImplementedException();
        }

        public bool ClearItem()
        {
            throw new NotImplementedException();
        }

        public Id? SetAbility(string ability, Pokemon? source = null, bool isFormeChange = false,
            bool isTransform = false)
        {
            throw new NotImplementedException();
        }

        public Id? SetAbility(Ability ability, Pokemon? source = null, bool isFormeChange = false,
            bool isTransform = false)
        {
            throw new NotImplementedException();
        }

        public Ability GetAbility()
        {
            throw new NotImplementedException();
        }

        public bool HasAbility(string ability)
        {
            throw new NotImplementedException();
        }

        public bool HasAbility(List<string> abilities)
        {
            throw new NotImplementedException();
        }

        public Id? ClearAbility()
        {
            throw new NotImplementedException();
        }

        public Nature GetNature()
        {
            throw new NotImplementedException();
        }

        public object AddVolatile(string status, Pokemon? source = null, IEffect? sourceEffect = null,
            string? linkedStatus = null)
        {
            throw new NotImplementedException();
        }

        public object AddVolatile(Condition status, Pokemon? source = null, IEffect? sourceEffect = null,
            string? linkedStatus = null)
        {
            throw new NotImplementedException();
        }

        public object AddVolatile(string status, Pokemon? source = null, IEffect? sourceEffect = null,
            Condition? linkedStatus = null)
        {
            throw new NotImplementedException();
        }

        public object AddVolatile(Condition status, Pokemon? source = null, IEffect? sourceEffect = null,
            Condition? linkedStatus = null)
        {
            throw new NotImplementedException();
        }

        public IEffect? GetVolatile(string status)
        {
            throw new NotImplementedException();
        }

        public IEffect? GetVolatile(IEffect status)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVolatile(string status)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVolatile(IEffect status)
        {
            throw new NotImplementedException();
        }

        public void RemoveLinkedVolatiles(string linkedStatus, List<Pokemon> linkedPokemon)
        {
            throw new NotImplementedException();
        }

        public void RemoveLinkedVolatiles(IEffect linkedStatus, List<Pokemon> linkedPokemon)
        {
            throw new NotImplementedException();
        }

        public FullDetails GetHealth()
        {
            throw new NotImplementedException();
        }

        public bool SetType(string newType, bool enforce = false)
        {
            throw new NotImplementedException();
        }

        public bool SetType(List<string> newTypes, bool enforce = false)
        {
            throw new NotImplementedException();
        }

        public bool AddType(string newType)
        {
            throw new NotImplementedException();
        }

        public List<string> GetTypes(bool? excludeAdded = null, bool? preTerastallized = null)
        {
            throw new NotImplementedException();
        }

        public bool? IsGrounded(bool negateImmunity = false)
        {
            throw new NotImplementedException();
        }

        public EffectState IsSemiInvulnerable()
        {
            throw new NotImplementedException();
        }

        public bool IsSkyDropped()
        {
            throw new NotImplementedException();
        }

        public bool IsProtected()
        {
            throw new NotImplementedException();
        }

        public Id EffectiveWeather()
        {
            throw new NotImplementedException();
        }

        public int RunEffectiveness(ActiveMove move)
        {
            throw new NotImplementedException();
        }

        public bool RunImmunity(ActiveMove source, string? message = null)
        {
            throw new NotImplementedException();
        }

        public bool RunImmunity(string source, string? message = null)
        {
            throw new NotImplementedException();
        }

        public bool RunImmunity(ActiveMove source, bool message)
        {
            throw new NotImplementedException();
        }

        public bool RunImmunity(string source, bool message)
        {
            throw new NotImplementedException();
        }

        public bool RunStatusImmunity(string type, string? message = null)
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }
    }
}