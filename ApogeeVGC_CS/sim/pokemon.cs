using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC_CS.sim
{
    public class MoveSlot
    {
        public string Id { get; set; } = string.Empty;
        public string Move { get; set; } = string.Empty;
        public int Pp { get; set; }
        public int MaxPp { get; set; }
        public string? Target { get; set; }
        public object Disabled { get; set; } = false; // bool or string
        public string? DisabledSource { get; set; }
        public bool Used { get; set; }
        public bool? Virtual { get; set; }
    }

    public class Attacker
    {
        public Pokemon Source { get; set; }
        public int Damage { get; set; }
        public bool ThisTurn { get; set; }
        public string? Move { get; set; }
        public PokemonSlot Slot { get; set; }
        public object? DamageValue { get; set; } // int, bool, or null
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
        public static readonly HashSet<string> RestorativeBerries = new()
        {
            "leppaberry", "aguavberry", "enigmaberry", "figyberry", "iapapaberry",
            "magoberry", "sitrusberry", "wikiberry", "oranberry"
        };
    }

    public class Pokemon
    {
        public Side Side { get; }
        public Battle Battle { get; }

        public PokemonSet Set { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public int Level { get; set; }
        public string Gender { get; set; } = string.Empty;
        public int Happiness { get; set; }
        public string Pokeball { get; set; } = string.Empty;
        public int DynamaxLevel { get; set; }
        public bool Gigantamax { get; set; }

        public string BaseHpType { get; set; } = string.Empty;
        public int BaseHpPower { get; set; }

        public List<MoveSlot> BaseMoveSlots { get; set; } = new();
        public List<MoveSlot> MoveSlots { get; set; } = new();

        public string HpType { get; set; } = string.Empty;
        public int HpPower { get; set; }

        public int Position { get; set; }
        public string Details { get; set; } = string.Empty;

        public Species BaseSpecies { get; set; } = new();
        public Species Species { get; set; } = new();
        public EffectState SpeciesState { get; set; } = new();

        public string Status { get; set; } = string.Empty;
        public EffectState StatusState { get; set; } = new();
        public Dictionary<string, EffectState> Volatiles { get; set; } = new();
        public bool? ShowCure { get; set; }

        public StatsTable BaseStoredStats { get; set; } = new();
        public StatsTable StoredStats { get; set; } = new();
        public BoostsTable Boosts { get; set; } = new();

        public string BaseAbility { get; set; } = string.Empty;
        public string Ability { get; set; } = string.Empty;
        public EffectState AbilityState { get; set; } = new();

        public string Item { get; set; } = string.Empty;
        public EffectState ItemState { get; set; } = new();
        public string LastItem { get; set; } = string.Empty;
        public bool UsedItemThisTurn { get; set; }
        public bool AteBerry { get; set; }

        public object Trapped { get; set; } = false; // bool or "hidden"
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

        public List<string> Types { get; set; } = new();
        public string AddedType { get; set; } = string.Empty;
        public bool KnownType { get; set; }
        public string ApparentType { get; set; } = string.Empty;

        public object SwitchFlag { get; set; } = false; // string or bool
        public bool ForceSwitchFlag { get; set; }
        public bool SkipBeforeSwitchOutEventFlag { get; set; }
        public int? DraggedIn { get; set; }
        public bool NewlySwitched { get; set; }
        public bool BeingCalledBack { get; set; }

        public ActiveMove? LastMove { get; set; }
        public ActiveMove? LastMoveEncore { get; set; } // Gen 2 only
        public ActiveMove? LastMoveUsed { get; set; }
        public int? LastMoveTargetLoc { get; set; }
        public object MoveThisTurn { get; set; } = string.Empty; // string or bool
        public bool StatsRaisedThisTurn { get; set; }
        public bool StatsLoweredThisTurn { get; set; }
        public bool? MoveLastTurnResult { get; set; }
        public bool? MoveThisTurnResult { get; set; }
        public int? HurtThisTurn { get; set; }
        public int LastDamage { get; set; }
        public List<Attacker> AttackedBy { get; set; } = new();
        public int TimesAttacked { get; set; }

        public bool IsActive { get; set; }
        public int ActiveTurns { get; set; }
        public int ActiveMoveActions { get; set; }
        public int PreviouslySwitchedIn { get; set; }
        public bool TruantTurn { get; set; }
        public bool BondTriggered { get; set; }
        public bool SwordBoost { get; set; } // Gen 9 only
        public bool ShieldBoost { get; set; } // Gen 9 only
        public bool SyrupTriggered { get; set; } // Gen 9 only
        public List<string> StellarBoostedTypes { get; set; } = new(); // Gen 9 only

        public bool IsStarted { get; set; }
        public bool DuringMove { get; set; }

        public double WeightHg { get; set; }
        public int Speed { get; set; }

        public string? CanMegaEvo { get; set; }
        public string? CanMegaEvoX { get; set; }
        public string? CanMegaEvoY { get; set; }
        public string? CanUltraBurst { get; set; }
        public string? CanGigantamax { get; }
        public object? CanTerastallize { get; set; } // string, false, or null
        public string TeraType { get; set; } = string.Empty;
        public List<string> BaseTypes { get; set; } = new();
        public string? Terastallized { get; set; }

        public string? Staleness { get; set; } // "internal" or "external"
        public string? PendingStaleness { get; set; } // "internal" or "external"
        public string? VolatileStaleness { get; set; } // "external"

        // Gen 1 only
        public StatsTable? ModifiedStats { get; set; }
        public Action<string, double>? ModifyStat { get; set; }
        // Stadium only
        public Action? RecalculateStats { get; set; }

        public Dictionary<string, object> M { get; set; } = new(); // Mod data

        public Pokemon(object set, Side side)
        {
            Side = side;
            Battle = side.Battle;
            M = new Dictionary<string, object>();

            // Handle Pokemon scripts from format/dex if needed

            if (set is string setName)
                set = new { name = setName };

            var setData = (dynamic)set;
            BaseSpecies = Battle.Dex.Species.Get(setData.species ?? setData.name);
            if (!BaseSpecies.Exists)
                throw new ArgumentException($"Unidentified species: {BaseSpecies.Name}");

            Set = (PokemonSet)set;
            Species = BaseSpecies;

            if (setData.name == setData.species || string.IsNullOrEmpty(setData.name))
                setData.name = BaseSpecies.BaseSpecies;

            SpeciesState = Battle.InitEffectState(new { id = Species.Id });

            Name = setData.name?.ToString()?.Substring(0, Math.Min(20, setData.name.ToString().Length)) ?? string.Empty;
            Fullname = $"{Side.Id}: {Name}";

            Level = Battle.ClampIntRange(setData.adjustLevel ?? setData.level ?? 100, 1, 9999);

            var genders = new Dictionary<string, string> { { "M", "M" }, { "F", "F" }, { "N", "N" } };
            Gender = genders.GetValueOrDefault(setData.gender) ?? Species.Gender ?? Battle.Sample(new[] { "M", "F" });
            if (Gender == "N") Gender = string.Empty;

            Happiness = setData.happiness is int h ? Battle.ClampIntRange(h, 0, 255) : 255;
            Pokeball = setData.pokeball ?? "pokeball";
            DynamaxLevel = setData.dynamaxLevel is int dl ? Battle.ClampIntRange(dl, 0, 10) : 10;
            Gigantamax = setData.gigantamax ?? false;

            BaseMoveSlots = new List<MoveSlot>();
            MoveSlots = new List<MoveSlot>();

            if (Set.Moves?.Length == 0)
                throw new ArgumentException($"Set {Name} has no moves");

            foreach (var moveid in Set.Moves)
            {
                var move = Battle.Dex.Moves.Get(moveid);
                if (string.IsNullOrEmpty(move.Id)) continue;

                if (move.Id == "hiddenpower" && move.Type != "Normal")
                {
                    if (string.IsNullOrEmpty(Set.HpType)) Set.HpType = move.Type;
                    move = Battle.Dex.Moves.Get("hiddenpower");
                }

                int basepp = move.NoPpBoosts ? move.Pp : move.Pp * 8 / 5;
                if (Battle.Gen < 3) basepp = Math.Min(61, basepp);

                BaseMoveSlots.Add(new MoveSlot
                {
                    Move = move.Name,
                    Id = move.Id,
                    Pp = basepp,
                    MaxPp = basepp,
                    Target = move.Target.ToString(),
                    Disabled = false,
                    DisabledSource = string.Empty,
                    Used = false
                });
            }

            Position = 0;
            Details = GetUpdatedDetails();

            Status = string.Empty;
            StatusState = Battle.InitEffectState(new { });
            Volatiles = new Dictionary<string, EffectState>();

            // Initialize EVs and IVs
            Set.Evs ??= new StatsTable();
            Set.Ivs ??= new StatsTable();

            // Clamp EV and IV values
            foreach (var stat in new[] { "hp", "atk", "def", "spa", "spd", "spe" })
            {
                Set.Evs[stat] = Battle.ClampIntRange(Set.Evs[stat], 0, 255);
                Set.Ivs[stat] = Battle.ClampIntRange(Set.Ivs[stat], 0, 31);
            }

            // Gen 1-2 DV handling
            if (Battle.Gen <= 2)
            {
                foreach (var stat in new[] { "hp", "atk", "def", "spa", "spd", "spe" })
                {
                    Set.Ivs[stat] &= 30; // Ensure even values for DVs
                }
            }

            var hpData = Battle.Dex.GetHiddenPower(Set.Ivs);
            HpType = Set.HpType ?? hpData.Type;
            HpPower = hpData.Power;
            BaseHpType = HpType;
            BaseHpPower = HpPower;

            BaseStoredStats = new StatsTable();
            StoredStats = new StatsTable();
            Boosts = new BoostsTable();

            BaseAbility = Set.Ability ?? string.Empty;
            Ability = BaseAbility;
            AbilityState = Battle.InitEffectState(new { id = Ability, target = this });

            Item = Set.Item ?? string.Empty;
            ItemState = Battle.InitEffectState(new { id = Item, target = this });
            LastItem = string.Empty;

            Types = BaseSpecies.Types.ToList();
            BaseTypes = Types.ToList();
            ApparentType = string.Join("/", BaseSpecies.Types);
            TeraType = Set.TeraType ?? Types.FirstOrDefault() ?? string.Empty;

            // Initialize various flags and counters
            AttackedBy = new List<Attacker>();
            StellarBoostedTypes = new List<string>();

            // Generation-specific initialization
            if (Battle.Gen == 2) LastMoveEncore = null;
            if (Battle.Gen == 1) ModifiedStats = new StatsTable();

            // Initialize mega/special abilities
            CanMegaEvo = Battle.Actions.CanMegaEvo(this);
            CanMegaEvoX = Battle.Actions.CanMegaEvoX?.Invoke(this);
            CanMegaEvoY = Battle.Actions.CanMegaEvoY?.Invoke(this);
            CanUltraBurst = Battle.Actions.CanUltraBurst(this);
            CanGigantamax = BaseSpecies.CanGigantamax;
            CanTerastallize = Battle.Actions.CanTerastallize(this);

            // Final initialization
            ClearVolatile();
            Hp = MaxHp;
        }

        private string GetUpdatedDetails()
        {
            // TODO: Implement details calculation
            return string.Empty;
        }

        private void ClearVolatile()
        {
            // TODO: Implement volatile clearing
        }
    }
}