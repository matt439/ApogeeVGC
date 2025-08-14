using ApogeeVGC_CS.lib;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC_CS.sim
{
    public record MoveSlot
    {
        public required Id Id { get; init; }
        public required string Move { get; init; }
        public required int Pp { get; init; }
        public required int MaxPp { get; init; }
        public MoveTarget? Target { get; init; }
        public required BoolStringUnion Disabled { get; init; }
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
        public IntBoolUnion? DamageValue { get; init; }
    }

    public class EffectState
    {
        public required Id Id { get; init; }
        public int EffectOrder { get; init; }
        public int? Duration { get; set; }
        public Dictionary<string, object> ExtraData { get; set; } = [];
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

        public PokemonSet Set { get; init; }
        public string Name { get; init; }
        public string Fullname => $"{Side.Id}: {Name}";
        public int Level { get; init; }
        public GenderName Gender { get; init; }
        public int Happiness { get; init; }
        public Id Pokeball { get; init; }
        public int DynamaxLevel { get; init; }
        public bool Gigantamax { get; init; }

        public PokemonType BaseHpType { get; init; }
        public int BaseHpPower { get; init; }

        public List<MoveSlot> BaseMoveSlots { get; init; }
        public List<MoveSlot> MoveSlots { get; set; }

        public PokemonType HpType { get; set; }
        public int HpPower { get; set; }

        public int Position { get; set; }
        public string Details { get; set; }

        public Species BaseSpecies { get; set; }
        public Species Species { get; set; }
        public EffectState SpeciesState { get; init; }

        public Id Status { get; init; }
        public EffectState StatusState { get; init; }
        public Dictionary<string, EffectState> Volatiles { get; init; }
        public bool? ShowCure { get; init; }

        public StatsTable BaseStoredStats { get; set; }
        public StatsTable StoredStats { get; init; }
        public BoostsTable Boosts { get; set; }

        public Id BaseAbility { get; set; }
        public Id Ability { get; set; }
        public EffectState AbilityState { get; init; }

        public Id Item { get; init; }
        public EffectState ItemState { get; init; }
        public Id LastItem { get; init; }
        public bool UsedItemThisTurn { get; init; }
        public bool AteBerry { get; init; }
        public PokemonTrapped Trapped { get; init; }
        public bool MaybeTrapped { get; init; }
        public bool MaybeDisabled { get; init; }
        public bool? MaybeLocked { get; init; }

        public Pokemon? Illusion { get; set; }
        public bool Transformed { get; set; }
        public int MaxHp { get; set; }
        public int BaseMaxHp { get; set; }
        public int Hp { get; set; }
        public bool Fainted { get; set; }
        public bool FaintQueued { get; init; }
        public bool? SubFainted { get; init; }
        public bool FormeRegression { get; set; }
        public List<PokemonType> Types { get; init; }
        public string AddedType { get; set; }
        public bool KnownType { get; set; }
        public string ApparentType { get; set; }
        public IdBoolUnion SwitchFlag { get; set; }
        public bool ForceSwitchFlag { get; set; }
        public bool SkipBeforeSwitchOutEventFlag { get; init; }
        public int? DraggedIn { get; init; }
        public bool NewlySwitched { get; set; }
        public bool BeingCalledBack { get; set; }

        public ActiveMove? LastMove { get; set; }
        public ActiveMove? LastMoveEncore { get; set; } // Gen 2 only
        public ActiveMove? LastMoveUsed { get; set; }
        public int? LastMoveTargetLoc { get; init; }
        public BoolStringUnion MoveThisTurn { get; set; }
        public bool StatsRaisedThisTurn { get; init; }
        public bool StatsLoweredThisTurn { get; init; }
        public bool? MoveLastTurnResult { get; set; }
        public bool? MoveThisTurnResult { get; set; }
        public int? HurtThisTurn { get; set; }
        public int LastDamage { get; set; }
        public List<Attacker> AttackedBy { get; init; }
        public int TimesAttacked { get; init; }

        public bool IsActive { get; set; }
        public int ActiveTurns { get; init; }
        public int ActiveMoveActions { get; init; }
        public int PreviouslySwitchedIn { get; init; }
        public bool TruantTurn { get; init; }
        public bool BondTriggered { get; init; }
        public bool SwordBoost { get; init; } // Gen 9 only
        public bool ShieldBoost { get; init; } // Gen 9 only
        public bool SyrupTriggered { get; init; } // Gen 9 only
        public List<string> StellarBoostedTypes { get; init; } // Gen 9 only

        public bool IsStarted { get; set; }
        public bool DuringMove { get; init; }

        public double WeightHg { get; set; }
        public int Speed { get; set; }

        public string? CanMegaEvo { get; init; }
        public string? CanMegaEvoX { get; init; }
        public string? CanMegaEvoY { get; init; }
        public string? CanUltraBurst { get; init; }
        public string? CanGigantamax { get; }
        public StringFalseUnion? CanTerastallize { get; set; }
        public PokemonType TeraType { get; init; }
        public List<PokemonType> BaseTypes { get; init; }
        public string? Terastallized { get; set; }

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
            set // "external"
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

        public Action<StatIdExceptHp, double>? ModifyStat { get; init; }

        // Stadium only
        public Action? RecalculateStats { get; init; }

        public Dictionary<string, object> M { get; init; } = [];


    //public Pokemon(string set, Side side) : this(new PokemonSet
    //{
    //    Species = set,
    //    Moves = [],
    //    Name = set,
    //    Item = string.Empty,
    //    Ability = string.Empty,
    //    Nature = string.Empty,
    //    Gender = GenderName.Empty,
    //    Evs = new StatsTable(),
    //    Ivs = new StatsTable(),
    //    Level = 0
    //},
    //    side) { }

    public Pokemon(PokemonSet set, Side side)
        {
            Side = side;
            Battle = side.Battle;

            BaseSpecies = Battle.Dex.Species.Get(set.Species);
            if (!BaseSpecies.Exists)
                throw new ArgumentException($"Unidentified species: {BaseSpecies.Name}");

            Set = set;

            Species = BaseSpecies;

            // Name setup
            if (string.IsNullOrEmpty(set.Name) || set.Name == set.Species)
            {
                set.Name = BaseSpecies.BaseSpecies;
            }
            SpeciesState = Battle.InitEffectState(new EffectState
            {
                Id = Species.Id,
                EffectOrder = 0
            });

            Name = set.Name.Length > 20 ? set.Name[..20] : set.Name;

            // Level setup
            Level = Utilities.ClampIntRange(set.AdjustLevel ?? set.Level, 1, 9999);

            Gender = set.Gender;

            // Happiness, Pokeball, Dynamax
            Happiness = set.Happiness is { } h ? Utilities.ClampIntRange(h, 0, 255) : 255;
            Pokeball = new Id(set.Pokeball ?? "pokeball");
            DynamaxLevel = set.DynamaxLevel is { } dl ? Utilities.ClampIntRange(dl, 0, 10) : 10;
            Gigantamax = set.Gigantamax ?? false;

            // Move slots initialization
            BaseMoveSlots = [];
            MoveSlots = [];

            if (set.Moves.Count == 0)
            {
                throw new InvalidOperationException($"Set {Name} has no moves");
            }

            foreach (string moveId in set.Moves)
            {
                Move move = Battle.Dex.Moves.Get(moveId);
                if (move.Id.IsEmpty) continue;

                if (move.Id.ToString() == "hiddenpower" && move.Type != PokemonType.Normal)
                {
                    if (set.HpType == PokemonType.Unknown)
                        set.HpType = move.Type;
                    move = Battle.Dex.Moves.Get("hiddenpower");
                }

                int basePp = (move.NoPpBoosts ?? false) ? move.Pp : move.Pp * 8 / 5;
                if (Battle.Gen < 3) basePp = Math.Min(61, basePp);

                BaseMoveSlots.Add(new MoveSlot
                {
                    Move = move.Name,
                    Id = move.Id,
                    Pp = basePp,
                    MaxPp = basePp,
                    Target = move.Target,
                    Disabled = false,
                    DisabledSource = string.Empty,
                    Used = false
                });
            }

            Position = 0;
            Details = GetUpdatedDetails();

            // Status initialization
            Status = new Id("");
            StatusState = Battle.InitEffectState(new EffectState
            {
                Id = new Id(),
                EffectOrder = 0
            });
            
            
            Volatiles = [];
            ShowCure = null;

            // EV/IV setup
            Set!.Evs = new StatsTable { Hp = 0, Atk = 0, Def = 0, Spa = 0, Spd = 0, Spe = 0 };
            Set.Ivs = new StatsTable { Hp = 31, Atk = 31, Def = 31, Spa = 31, Spd = 31, Spe = 31 };

            foreach (StatId stat in Enum.GetValues<StatId>())
            {
                if (Set.Evs.GetStat(stat) == 0) Set.Evs.SetStat(stat, 0);
                if (Set.Ivs.GetStat(stat) == 0) Set.Ivs.SetStat(stat, 31);
            }

            foreach (StatId stat in Enum.GetValues<StatId>())
            {
                Set.Evs.SetStat(stat, Utilities.ClampIntRange(Set.Evs.GetStat(stat), 0, 255));
                Set.Ivs.SetStat(stat, Utilities.ClampIntRange(Set.Ivs.GetStat(stat), 0, 31));
            }

            // Gen 1-2 DV handling
            if (Battle.Gen <= 2)
            {
                foreach (StatId stat in Enum.GetValues<StatId>())
                {
                    Set.Ivs.SetStat(stat, Set.Ivs.GetStat(stat) & 30); // Ensure even values
                }
            }

            // Hidden Power calculation
            HiddenPower hpData = Battle.Dex.GetHiddenPower(Set.Ivs);
            HpType = set.HpType ?? hpData.Type;
            HpPower = hpData.Power;

            BaseHpType = HpType;
            BaseHpPower = HpPower;

            // Stats initialization
            BaseStoredStats = new StatsTable(); // Will be initialized in SetSpecies
            StoredStats = new StatsTable { Atk = 0, Def = 0, Spa = 0, Spd = 0, Spe = 0 };
            Boosts = new BoostsTable
            {
                [BoostId.Atk] = 0,
                [BoostId.Def] = 0,
                [BoostId.Spa] = 0,
                [BoostId.Spd] = 0,
                [BoostId.Spe] = 0,
                [BoostId.Accuracy] = 0,
                [BoostId.Evasion] = 0
            };

            // Ability setup
            BaseAbility = new Id(set.Ability);
            Ability = BaseAbility;
            AbilityState = Battle.InitEffectState(new EffectState
            {
                Id = Ability,
                ExtraData = new Dictionary<string, object>()
                {
                    ["target"] = this
                },
                EffectOrder = 0
            });

            // Item setup
            Item = new Id(set.Item);
            ItemState = Battle.InitEffectState(new EffectState
            {
                Id = Item,
                ExtraData = new Dictionary<string, object>()
                {
                    ["target"] = this
                },
                EffectOrder = 0
            });
            LastItem = new Id("");
            UsedItemThisTurn = false;
            AteBerry = false;

            // Trap states
            Trapped = false;
            MaybeTrapped = false;
            MaybeDisabled = false;
            MaybeLocked = false;

            // Transform states
            Illusion = null;
            Transformed = false;

            // HP and fainting
            Fainted = false;
            FaintQueued = false;
            SubFainted = null;

            // Forme
            FormeRegression = false;

            // Types
            Types = BaseSpecies.Types;
            BaseTypes = Types;
            AddedType = string.Empty;
            KnownType = true;
            ApparentType = string.Join("/", BaseSpecies.Types);
            TeraType = set.TeraType ?? Types[0];

            // Switch flags
            SwitchFlag = false;
            ForceSwitchFlag = false;
            SkipBeforeSwitchOutEventFlag = false;
            DraggedIn = null;
            NewlySwitched = false;
            BeingCalledBack = false;

            // Move tracking
            LastMove = null;
            if (Battle.Gen == 2) LastMoveEncore = null;
            LastMoveUsed = null;
            MoveThisTurn = string.Empty;
            StatsRaisedThisTurn = false;
            StatsLoweredThisTurn = false;
            HurtThisTurn = null;
            LastDamage = 0;
            AttackedBy = [];
            TimesAttacked = 0;

            // Activity states
            IsActive = false;
            ActiveTurns = 0;
            ActiveMoveActions = 0;
            PreviouslySwitchedIn = 0;
            TruantTurn = false;
            BondTriggered = false;
            SwordBoost = false;
            ShieldBoost = false;
            SyrupTriggered = false;
            StellarBoostedTypes = [];
            IsStarted = false;
            DuringMove = false;

            // Physical properties
            WeightHg = 1;
            Speed = 0;

            // Special abilities
            CanMegaEvo = Battle.Actions.CanMegaEvo(this);
            CanMegaEvoX = Battle.Actions.CanMegaEvoX?.Invoke(Battle.Actions, this);
            CanMegaEvoY = Battle.Actions.CanMegaEvoY?.Invoke(Battle.Actions, this);
            CanUltraBurst = Battle.Actions.CanUltraBurst(this);
            CanGigantamax = BaseSpecies.CanGigantamax;
            CanTerastallize = Battle.Actions.CanTerastallize(this);

            // Gen-specific stats
            if (Battle.Gen == 1)
            {
                ModifiedStats = new StatsTable { Atk = 0, Def = 0, Spa = 0, Spd = 0, Spe = 0 };
            }

            // HP initialization
            MaxHp = 0;
            BaseMaxHp = 0;
            Hp = 0;
            ClearVolatile();
            Hp = MaxHp;
        }

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
            string name = Species.Name;

            // Handle special forms that should use base species name
            if (name is "Greninja-Bond" or "Rockruff-Dusk")
            {
                name = Species.BaseSpecies;
            }

            // Use provided level or fall back to Pokemon's level
            int displayLevel = level ?? Level;

            // Build the details string
            var details = new List<string> { name };

            // Add level if not 100 (standard level)
            if (displayLevel != 100)
            {
                details.Add($"L{displayLevel}");
            }

            // Add gender if not empty
            if (Gender != GenderName.Empty)
            {
                details.Add(Gender.ToString());
            }

            // Add shiny status if applicable
            if (Set.Shiny == true)
            {
                details.Add("shiny");
            }

            return string.Join(", ", details);
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
            // In Gen 5+, inactive Pokemon's abilities are ignored.
            if (Battle.Gen >= 5 && !IsActive) return true;

            Ability ability = GetAbility();

            // Certain abilities don't work while transformed.
            if (ability.Flags.NoTransform ?? false && Transformed) return true;
            // Abilities that can't be suppressed are never ignored.
            if (ability.Flags.CantSuppress ?? false) return false;
            // Gastro Acid suppresses abilities.
            if (Volatiles.ContainsKey("gastroacid")) return true;

            // Neutralizing Gas check
            // Ability Shield prevents suppression by Neutralizing Gas.
            // The Neutralizing Gas user itself is not affected by its own ability.
            if (HasItem("Ability Shield") || this.Ability == "neutralizinggas") return false;

            foreach (Pokemon pokemon in Battle.GetAllActive())
            {
                // A different active Pokemon has Neutralizing Gas.
                // We check its ability directly to avoid infinite recursion from `pokemon.HasAbility()`.
                if (pokemon.Ability == "neutralizinggas" &&
                    // Its ability must not be suppressed by Gastro Acid.
                    !pokemon.Volatiles.ContainsKey("gastroacid") &&
                    // It must not be transformed.
                    !pokemon.Transformed &&
                    // Its ability must not be in the process of ending.
                    !(pokemon.AbilityState.ExtraData.TryGetValue("ending",
                        out object? ending) && (bool)ending) &&
                        // The target Pokemon must not be under the effect of Commander.
                        !Volatiles.ContainsKey("commanding"))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IgnoringItem(bool isFling = false)
        {
            Item item = GetItem();

            // Primal Orbs are never ignored
            if (item.IsPrimalOrb) return false;

            // Gen 3-4: knocked off items are ignored
            if (ItemState.ExtraData?.ContainsKey("knockedOff") == true &&
                (bool)(ItemState.ExtraData["knockedOff"])) return true;

            // Gen 5+: inactive Pokemon ignore their items
            if (Battle.Gen >= 5 && !IsActive) return true;

            // Embargo condition or Magic Room field condition
            if (Volatiles.ContainsKey("embargo") ||
                Battle.Field.PseudoWeather.ContainsKey("magicroom")) return true;

            // Check Fling first to avoid infinite recursion
            if (isFling) return Battle.Gen >= 5 && HasAbility("klutz");

            // Klutz ability makes Pokemon ignore items (unless item ignores Klutz)
            return !(item.IgnoreKlutz) && HasAbility("klutz");
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

        /// <summary>
        /// Changes this Pokemon's species to the given species.
        /// This function only handles changes to stats and type.
        /// Use FormeChange to handle changes to ability and sending client messages.
        /// </summary>
        public Species? SetSpecies(Species rawSpecies, IEffect? source = null, bool isTransform = false)
        {
            try
            {
                // Use battle effect as default source
                source ??= Battle.Effect;

                // Run ModifySpecies event to allow modifications
                var modifiedSpecies = Battle.RunEvent("ModifySpecies", this, null, source, rawSpecies);

                // Handle different return types from the event
                var species = modifiedSpecies switch
                {
                    Species s => s,
                    null => null,
                    _ => rawSpecies // Fallback to original if unexpected type
                };

                if (species == null) return null;

                // Update species
                Species = species;

                // Update type information safely
                UpdateTypeInformation(species, rawSpecies);

                // Update weight
                WeightHg = species.WeightHg;

                // Calculate and apply new stats
                var stats = CalculateNewStats(species);

                // Initialize HP if needed
                InitializeHpIfNeeded(stats);

                // Update stored stats
                UpdateStoredStatsForSpeciesChange(stats, isTransform);

                // Apply generation-specific modifications
                ApplyGenerationSpecificModifications();

                // Update speed
                Speed = StoredStats.Spe;

                return species;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SetSpecies for {Name}: {ex.Message}");
                return null;
            }
        }

        private void UpdateTypeInformation(Species species, Species rawSpecies)
        {
            try
            {
                SetType(species.Types, enforce: true);
                ApparentType = string.Join("/", rawSpecies.Types.Select(t => t.ToString()));
                AddedType = species.AddedType?.ToString() ?? "";
                KnownType = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating type information: {ex.Message}");
                // Continue with defaults
                ApparentType = string.Join("/", species.Types.Select(t => t.ToString()));
                AddedType = "";
                KnownType = true;
            }
        }

        private StatsTable CalculateNewStats(Species species)
        {
            var stats = Battle.SpreadModify(species.BaseStats, Set);

            // Handle special maxHP override
            if (species.MaxHp.HasValue)
            {
                var newStats = new StatsTable();
                // Copy all stats from the original
                if (species.MaxHp.HasValue)
                {
                    stats = new StatsTable(stats)
                    {
                        Hp = species.MaxHp.Value // Direct property assignment
                    };
                }
                // Override the HP value
                newStats.SetStat(StatId.Hp, species.MaxHp.Value);
                stats = newStats;
            }

            return stats;
        }

        private void InitializeHpIfNeeded(StatsTable stats)
        {
            if (MaxHp != 0) return;
            BaseMaxHp = stats.Hp;
            MaxHp = stats.Hp;
            Hp = stats.Hp;
        }

        private void UpdateStoredStatsForSpeciesChange(StatsTable stats, bool isTransform)
        {
            // Update base stored stats (not for transforms)
            if (!isTransform)
            {
                BaseStoredStats = stats;
            }

            // Update all stored stats except HP
            foreach (var statId in Enum.GetValues<StatIdExceptHp>())
            {
                int statValue = stats.GetStat(StatIdTools.ConvertToStatId(statId));
                StoredStats.SetStat(StatIdTools.ConvertToStatId(statId), statValue);

                // Gen 1: Reset modified stats
                ModifiedStats?.SetStat(StatIdTools.ConvertToStatId(statId), statValue);
            }
        }

        private void ApplyGenerationSpecificModifications()
        {
            if (Battle.Gen <= 1)
            {
                ApplyGen1StatusModifications();
            }
        }

        private void ApplyGen1StatusModifications()
        {
            if (ModifyStat == null) return;

            try
            {
                // Re-apply burn and paralysis drops in Gen 1
                string statusValue = Status.Value.ToLowerInvariant();

                switch (statusValue)
                {
                    case "par":
                        ModifyStat(StatIdExceptHp.Spe, 0.25);
                        break;
                    case "brn":
                        ModifyStat(StatIdExceptHp.Atk, 0.5);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying Gen 1 status modifications: {ex.Message}");
            }
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
            try
            {
                // Reset all boosts to 0
                Boosts ??= new BoostsTable();
                foreach (var boostType in Enum.GetValues<BoostId>())
                {
                    Boosts[boostType] = 0;
                }

                // Handle move slots restoration
                RestoreMoveSlots();

                // Reset transformation state
                ResetTransformationState();

                // Handle linked volatiles cleanup
                CleanupLinkedVolatiles();

                // Handle volatile effects
                HandleVolatileEffects();

                // Clear switch flags if requested
                if (includeSwitchFlags)
                {
                    SwitchFlag = false;
                    ForceSwitchFlag = false;
                }

                // Reset move and damage tracking
                ResetMoveTracking();
                ResetDamageTracking();

                // Clear staleness and state flags
                ClearStateFlags();

                // Reset to base species
                SetSpecies(BaseSpecies);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ClearVolatile for {Name}: {ex.Message}");
                // Continue with partial cleanup rather than throwing
            }
        }

        private void RestoreMoveSlots()
        {
            if (Battle.Gen == 1 && HasBaseMoveId(new Id("mimic")) && !Transformed)
            {
                int mimicSlotIndex = GetBaseMoveSlotIndex(new Id("mimic"));
                if (mimicSlotIndex >= 0 && mimicSlotIndex < MoveSlots.Count)
                {
                    int preservedMimicPp = MoveSlots[mimicSlotIndex].Pp;
                    MoveSlots = CloneMoveSlots(BaseMoveSlots);

                    if (mimicSlotIndex < MoveSlots.Count)
                    {
                        MoveSlots[mimicSlotIndex] = MoveSlots[mimicSlotIndex] with { Pp = preservedMimicPp };
                    }
                }
                else
                {
                    MoveSlots = CloneMoveSlots(BaseMoveSlots);
                }
            }
            else
            {
                MoveSlots = CloneMoveSlots(BaseMoveSlots);
            }
        }

        private void ResetTransformationState()
        {
            Transformed = false;
            Ability = BaseAbility;
            HpType = BaseHpType;
            HpPower = BaseHpPower;

            if (CanTerastallize is FalseStringFalseUnion)
            {
                CanTerastallize = TeraType.ToString();
            }
        }

        private void CleanupLinkedVolatiles()
        {
            var volatilesToProcess = Volatiles.ToList(); // Create a copy to avoid modification during iteration

            foreach (var (statusId, volatileData) in volatilesToProcess)
            {
                if (volatileData?.ExtraData != null &&
                    volatileData.ExtraData.TryGetValue("linkedStatus", out object? linkedStatusObj) &&
                    volatileData.ExtraData.TryGetValue("linkedPokemon", out object? linkedPokemonObj) &&
                    linkedStatusObj is string linkedStatus &&
                    linkedPokemonObj is List<Pokemon> linkedPokemon)
                {
                    RemoveLinkedVolatiles(linkedStatus, linkedPokemon);
                }
            }
        }

        private void HandleVolatileEffects()
        {
            if (Species.Name == "Eternatus-Eternamax" && Volatiles.TryGetValue("dynamax", out EffectState? dynamaxVolatile))
            {
                Volatiles.Clear();
                Volatiles["dynamax"] = dynamaxVolatile;
            }
            else
            {
                Volatiles.Clear();
            }
        }

        private void ResetMoveTracking()
        {
            LastMove = null;
            if (Battle.Gen == 2) LastMoveEncore = null;
            LastMoveUsed = null;
            MoveThisTurn = "";
            MoveLastTurnResult = null;
            MoveThisTurnResult = null;
        }

        private void ResetDamageTracking()
        {
            LastDamage = 0;
            AttackedBy?.Clear();
            HurtThisTurn = null;
            NewlySwitched = true;
            BeingCalledBack = false;
        }

        private void ClearStateFlags()
        {
            VolatileStaleness = null;

            AbilityState?.ExtraData?.Remove("started");
            ItemState?.ExtraData?.Remove("started");
        }

        private static List<MoveSlot> CloneMoveSlots(List<MoveSlot> original)
        {
            return original.Select(slot => new MoveSlot
            {
                Id = slot.Id,
                Move = slot.Move,
                Pp = slot.Pp,
                MaxPp = slot.MaxPp,
                Target = slot.Target,
                Disabled = slot.Disabled,
                DisabledSource = slot.DisabledSource,
                Used = slot.Used,
                Virtual = slot.Virtual
            }).ToList();
        }

        private bool HasBaseMoveId(Id moveId)
        {
            return BaseMoveSlots.Any(slot => slot.Id == moveId);
        }

        private int GetBaseMoveSlotIndex(Id moveId)
        {
            for (int i = 0; i < BaseMoveSlots.Count; i++)
            {
                if (BaseMoveSlots[i].Id == moveId)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool HasType(string type)
        {
            throw new NotImplementedException();
        }

        public bool HasType(List<string> types)
        {
            throw new NotImplementedException();
        }

        public bool HasType(PokemonType type)
        {
            throw new NotImplementedException();
        }

        public bool HasType(List<PokemonType> types)
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

        public Id? HasMove(Id moveId)
        {
            throw new NotImplementedException();
        }

        public void DisableMove(Id moveId, bool isHidden, IEffect? sourceEffect = null)
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

        public object AddVolatile(string status)
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
            foreach (Pokemon linkedPoke in linkedPokemon)
            {
                linkedPoke.RemovePokemonFromVolatileLinks(linkedStatus, this);
            }
        }

        public void RemoveLinkedVolatiles(IEffect linkedStatus, List<Pokemon> linkedPokemon)
        {
            RemoveLinkedVolatiles(linkedStatus.ToString(), linkedPokemon);
        }

        // Helper method
        private void RemovePokemonFromVolatileLinks(string statusId, Pokemon pokemonToRemove)
        {
            if (!Volatiles.TryGetValue(statusId, out EffectState? volatileData))
                return;

            if (volatileData.ExtraData.TryGetValue("linkedPokemon", out object? linkedObj) &&
                linkedObj is List<Pokemon> linkedList)
            {
                linkedList.Remove(pokemonToRemove);

                if (linkedList.Count == 0)
                {
                    RemoveVolatile(statusId);
                }
            }
        }

        public FullDetails GetHealth()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets a type using PokemonType enum (except on Arceus, who resists type changes)
        /// </summary>
        public bool SetType(PokemonType newType, bool enforce = false)
        {
            return SetType(new List<PokemonType> { newType }, enforce);
        }

        public bool SetType(List<PokemonType> newTypes, bool enforce = false)
        {
            if (newTypes == null || newTypes.Count == 0)
                throw new ArgumentException("Must pass type to SetType");

            if (!enforce)
            {
                // First type of Arceus, Silvally cannot be normally changed
                if (IsTypeChangeResistantPokemon())
                    return false;

                // Terastallized Pokemon cannot have their base type changed except via forme change
                if (IsTerastallized())
                    return false;
            }

            // Update the Pokemon's types
            Types.Clear();
            Types.AddRange(newTypes);

            AddedType = "";
            KnownType = true;
            ApparentType = string.Join("/", newTypes.Select(t => t.ToString()));

            return true;
        }

        // Helper method to check if Pokemon resists type changes
        private bool IsTypeChangeResistantPokemon()
        {
            return (Battle.Gen >= 5 && Species.Num is 493 or 773) ||
                   (Battle.Gen == 4 && HasAbility("multitype"));
        }

        // Helper method to check if Pokemon is Terastallized
        private bool IsTerastallized()
        {
            return !string.IsNullOrEmpty(Terastallized);
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