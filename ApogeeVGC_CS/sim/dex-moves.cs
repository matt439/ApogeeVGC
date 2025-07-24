using ApogeeVGC_CS.sim;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC_CS.sim
{
    /**
     * Describes the acceptable target(s) of a move.
     * adjacentAlly - Only relevant to Doubles or Triples, the move only targets an ally of the user.
     * adjacentAllyOrSelf - The move can target the user or its ally.
     * adjacentFoe - The move can target a foe, but not (in Triples) a distant foe.
     * all - The move targets the field or all Pokémon at once.
     * allAdjacent - The move is a spread move that also hits the user's ally.
     * allAdjacentFoes - The move is a spread move.
     * allies - The move affects all active Pokémon on the user's team.
     * allySide - The move adds a side condition on the user's side.
     * allyTeam - The move affects all unfainted Pokémon on the user's team.
     * any - The move can hit any other active Pokémon, not just those adjacent.
     * foeSide - The move adds a side condition on the foe's side.
     * normal - The move can hit one adjacent Pokémon of your choice.
     * randomNormal - The move targets an adjacent foe at random.
     * scripted - The move targets the foe that damaged the user.
     * self - The move affects the user of the move.
     */
    public enum MoveTarget
    {
        AdjacentAlly,
        AdjacentAllyOrSelf,
        AdjacentFoe,
        All,
        AllAdjacent,
        AllAdjacentFoes,
        Allies,
        AllySide,
        AllyTeam,
        Any,
        FoeSide,
        Normal,
        RandomNormal,
        Scripted,
        Self
    }

    public class MoveFlags
    {
        public bool AllyAnim { get; set; }
        public bool BypassSub { get; set; }
        public bool Bite { get; set; }
        public bool Bullet { get; set; }
        public bool CantUseTwice { get; set; }
        public bool Charge { get; set; }
        public bool Contact { get; set; }
        public bool Dance { get; set; }
        public bool Defrost { get; set; }
        public bool Distance { get; set; }
        public bool FailCopycat { get; set; }
        public bool FailEncore { get; set; }
        public bool FailInstruct { get; set; }
        public bool FailMeFirst { get; set; }
        public bool FailMimic { get; set; }
        public bool FutureMove { get; set; }
        public bool Gravity { get; set; }
        public bool Heal { get; set; }
        public bool Metronome { get; set; }
        public bool Mirror { get; set; }
        public bool MustPressure { get; set; }
        public bool NoAssist { get; set; }
        public bool NonSky { get; set; }
        public bool NoParentalBond { get; set; }
        public bool NoSketch { get; set; }
        public bool NoSleepTalk { get; set; }
        public bool PledgeCombo { get; set; }
        public bool Powder { get; set; }
        public bool Protect { get; set; }
        public bool Pulse { get; set; }
        public bool Punch { get; set; }
        public bool Recharge { get; set; }
        public bool Reflectable { get; set; }
        public bool Slicing { get; set; }
        public bool Snatch { get; set; }
        public bool Sound { get; set; }
        public bool Wind { get; set; }
    }

    public class HitEffect
    {
        public Delegate? OnHit { get; set; } // Replace with specific delegate type if needed

        // Set Pokémon conditions
        public object? Boosts { get; set; } // Replace with SparseBoostsTable if available
        public string? Status { get; set; }
        public string? VolatileStatus { get; set; }

        // Set side/slot conditions
        public string? SideCondition { get; set; }
        public string? SlotCondition { get; set; }

        // Set field conditions
        public string? PseudoWeather { get; set; }
        public string? Terrain { get; set; }
        public string? Weather { get; set; }
    }

    public class SecondaryEffect : HitEffect
    {
        public int? Chance { get; set; }
        public Ability? Ability { get; set; }
        public bool? KingsRock { get; set; }
        public HitEffect? Self { get; set; }
    }

    public interface IMoveEventMethods
    {

    }

    public class MoveData : EffectData, IMoveEventMethods
    {
        public string Name { get; set; } = string.Empty;
        public int? Num { get; set; }
        public ConditionData? Condition { get; set; }
        public int BasePower { get; set; }
        public object Accuracy { get; set; } = true; // true or int
        public int Pp { get; set; }
        public string Category { get; set; } = string.Empty; // "Physical", "Special", "Status"
        public string Type { get; set; } = string.Empty;
        public int Priority { get; set; }
        public MoveTarget Target { get; set; }
        public MoveFlags Flags { get; set; } = new();
        public string? RealMove { get; set; }

        public object? Damage { get; set; } // int, "level", false, or null
        public string? ContestType { get; set; }
        public bool? NoPpBoosts { get; set; }

        // Z-move data
        public object? IsZ { get; set; } // bool or string
        public ZMoveData? ZMove { get; set; }

        // Max move data
        public object? IsMax { get; set; } // bool or string
        public MaxMoveData? MaxMove { get; set; }

        // Hit effects
        public object? Ohko { get; set; } // bool or "Ice"
        public bool? ThawsTarget { get; set; }
        public int[]? Heal { get; set; }
        public bool? ForceSwitch { get; set; }
        public object? SelfSwitch { get; set; } // "copyvolatile", "shedtail", or bool
        public SelfBoostData? SelfBoost { get; set; }
        public object? Selfdestruct { get; set; } // "always", "ifHit", or bool
        public bool? BreaksProtect { get; set; }
        public int[]? Recoil { get; set; }
        public int[]? Drain { get; set; }
        public bool? MindBlownRecoil { get; set; }
        public bool? StealsBoosts { get; set; }
        public bool? StruggleRecoil { get; set; }
        public SecondaryEffect? Secondary { get; set; }
        public List<SecondaryEffect>? Secondaries { get; set; }
        public SecondaryEffect? Self { get; set; }
        public bool? HasSheerForce { get; set; }

        // Hit effect modifiers
        public bool? AlwaysHit { get; set; }
        public string? BaseMoveType { get; set; }
        public int? BasePowerModifier { get; set; }
        public int? CritModifier { get; set; }
        public int? CritRatio { get; set; }
        public string? OverrideOffensivePokemon { get; set; } // "target" or "source"
        public string? OverrideOffensiveStat { get; set; }
        public string? OverrideDefensivePokemon { get; set; } // "target" or "source"
        public string? OverrideDefensiveStat { get; set; }
        public bool? ForceSTAB { get; set; }
        public bool? IgnoreAbility { get; set; }
        public bool? IgnoreAccuracy { get; set; }
        public bool? IgnoreDefensive { get; set; }
        public bool? IgnoreEvasion { get; set; }
        public object? IgnoreImmunity { get; set; } // bool or Dictionary<string, bool>
        public bool? IgnoreNegativeOffensive { get; set; }
        public bool? IgnoreOffensive { get; set; }
        public bool? IgnorePositiveDefensive { get; set; }
        public bool? IgnorePositiveEvasion { get; set; }
        public bool? MultiAccuracy { get; set; }
        public object? MultiHit { get; set; } // int or int[]
        public string? MultiHitType { get; set; } // "parentalbond"
        public bool? NoDamageVariance { get; set; }
        public MoveTarget? NonGhostTarget { get; set; }
        public double? SpreadModifier { get; set; }
        public bool? SleepUsable { get; set; }
        public bool? SmartTarget { get; set; }
        public bool? TracksTarget { get; set; }
        public bool? WillCrit { get; set; }
        public bool? CallsMove { get; set; }

        // Mechanics flags
        public bool? HasCrashDamage { get; set; }
        public bool? IsConfusionSelfHit { get; set; }
        public bool? StallingMove { get; set; }
        public string? BaseMove { get; set; }
    }

    public class ZMoveData
    {
        public int? BasePower { get; set; }
        public string? Effect { get; set; }
        public object? Boost { get; set; } // Replace with SparseBoostsTable if available
    }

    public class MaxMoveData
    {
        public int BasePower { get; set; }
    }

    public class SelfBoostData
    {
        public object? Boosts { get; set; } // Replace with SparseBoostsTable if available
    }

    public class ModdedMoveData : MoveData
    {
        public bool Inherit { get; set; }
        public bool? IgniteBoosted { get; set; }
        public bool? SettleBoosted { get; set; }
        public bool? BodyOfWaterBoosted { get; set; }
        public bool? LongWhipBoost { get; set; }
        public int? Gen { get; set; }
    }

    // MoveDataTable and ModdedMoveDataTable
    public class MoveDataTable : Dictionary<string, MoveData> { }
    public class ModdedMoveDataTable : Dictionary<string, ModdedMoveData> { }

    // Move class (readonly effectType)
    public class Move : BasicEffect
    {
        public override EffectType EffectType => EffectType.Move;
        // Inherit all properties from BasicEffect and MoveData as needed
    }

    // MoveHitData structure
    public class MoveHitResult
    {
        public bool Crit { get; set; }
        public int TypeMod { get; set; }
        public bool ZBrokeProtect { get; set; }
    }

    public class MoveHitData : Dictionary<string, MoveHitResult> { }

    // ActiveMove structure
    public class ActiveMove : MoveData, IActiveMove
    {
        public new string Name { get; set; } = string.Empty;
        public override EffectType EffectType => EffectType.Move;
        public string Id { get; set; } = string.Empty;
        public int Num { get; set; }
        public string? Weather { get; set; }
        public string? Status { get; set; }
        public int Hit { get; set; }
        public MoveHitData? MoveHitData { get; set; }
        public List<Pokemon>? HitTargets { get; set; }
        public Ability? Ability { get; set; }
        public List<Pokemon>? Allies { get; set; }
        public Pokemon? AuraBooster { get; set; }
        public bool? CausedCrashDamage { get; set; }
        public string? ForceStatus { get; set; }
        public bool? HasAuraBreak { get; set; }
        public bool? HasBounced { get; set; }
        public bool? HasSheerForce { get; set; }
        public bool? IsExternal { get; set; }
        public bool? LastHit { get; set; }
        public int? Magnitude { get; set; }
        public bool? PranksterBoosted { get; set; }
        public bool? SelfDropped { get; set; }
        public object? SelfSwitch { get; set; } // "copyvolatile", "shedtail", or bool
        public bool? SpreadHit { get; set; }
        public string? StatusRoll { get; set; }
        public bool? StellarBoosted { get; set; }
        public object? TotalDamage { get; set; } // int or false
        public Effect? TypeChangerBoosted { get; set; }
        public bool? WillChangeForme { get; set; }
        public bool? Infiltrates { get; set; }
        public Pokemon? RuinedAtk { get; set; }
        public Pokemon? RuinedDef { get; set; }
        public Pokemon? RuinedSpA { get; set; }
        public Pokemon? RuinedSpD { get; set; }
        public bool? IsZOrMaxPowered { get; set; }
    }

    // MoveCategory enum
    public enum MoveCategory
    {
        Physical,
        Special,
        Status
    }

    public class DataMove : BasicEffect
    {
        public override EffectType EffectType => EffectType.Move;
        public string Type { get; set; } = string.Empty;
        public MoveTarget Target { get; set; }
        public int BasePower { get; set; }
        public object Accuracy { get; set; } = true; // true or int
        public int CritRatio { get; set; } = 1;
        public bool? WillCrit { get; set; }
        public object? Ohko { get; set; } // bool or "Ice"
        public string BaseMoveType { get; set; } = string.Empty;
        public SecondaryEffect? Secondary { get; set; }
        public List<SecondaryEffect>? Secondaries { get; set; }
        public bool HasSheerForce { get; set; }
        public int Priority { get; set; } = 0;
        public MoveCategory Category { get; set; }
        public string? OverrideOffensivePokemon { get; set; }
        public string? OverrideOffensiveStat { get; set; }
        public string? OverrideDefensivePokemon { get; set; }
        public string? OverrideDefensiveStat { get; set; }
        public bool IgnoreNegativeOffensive { get; set; }
        public bool IgnorePositiveDefensive { get; set; }
        public bool IgnoreOffensive { get; set; }
        public bool IgnoreDefensive { get; set; }
        public object IgnoreImmunity { get; set; } = false; // bool or Dictionary<string, bool>
        public int Pp { get; set; }
        public bool NoPpBoosts { get; set; }
        public object? MultiHit { get; set; } // int or int[]
        public object? IsZ { get; set; } // bool or string
        public ZMoveData? ZMove { get; set; }
        public object? IsMax { get; set; } // bool or string
        public MaxMoveData? MaxMove { get; set; }
        public MoveFlags Flags { get; set; } = new();
        public object? SelfSwitch { get; set; } // "copyvolatile", "shedtail", or bool
        public MoveTarget? NonGhostTarget { get; set; }
        public bool IgnoreAbility { get; set; }
        public object? Damage { get; set; } // int, "level", false, or null
        public bool SpreadHit { get; set; }
        public double? SpreadModifier { get; set; }
        public int? CritModifier { get; set; }
        public bool ForceSTAB { get; set; }
        public string? VolatileStatus { get; set; }

        public int Gen { get; set; }
        public string Id { get; set; } = string.Empty;

        public DataMove(IAnyObject data) : base(data)
        {
            Name = BasicEffect.GetString(data, "name");
            Type = BasicEffect.GetString(data, "type");
            Target = (MoveTarget)Enum.Parse(typeof(MoveTarget), BasicEffect.GetString(data, "target") ?? "Self", true);
            BasePower = BasicEffect.GetInt(data, "basePower") ?? 0;
            Accuracy = data.ContainsKey("accuracy") ? data["accuracy"] : true;
            CritRatio = BasicEffect.GetInt(data, "critRatio") ?? 1;
            BaseMoveType = BasicEffect.GetString(data, "baseMoveType") ?? Type;
            Secondary = data.ContainsKey("secondary") ? (SecondaryEffect?)data["secondary"] : null;
            Secondaries = data.ContainsKey("secondaries") ? (List<SecondaryEffect>?)data["secondaries"] : (Secondary != null ? new List<SecondaryEffect> { Secondary } : null);
            HasSheerForce = data.ContainsKey("hasSheerForce") && Secondaries == null;
            Priority = BasicEffect.GetInt(data, "priority") ?? 0;
            Category = Enum.TryParse(BasicEffect.GetString(data, "category"), out MoveCategory cat) ? cat : MoveCategory.Status;
            OverrideOffensiveStat = data.ContainsKey("overrideOffensiveStat") ? data["overrideOffensiveStat"]?.ToString() : null;
            OverrideOffensivePokemon = data.ContainsKey("overrideOffensivePokemon") ? data["overrideOffensivePokemon"]?.ToString() : null;
            OverrideDefensiveStat = data.ContainsKey("overrideDefensiveStat") ? data["overrideDefensiveStat"]?.ToString() : null;
            OverrideDefensivePokemon = data.ContainsKey("overrideDefensivePokemon") ? data["overrideDefensivePokemon"]?.ToString() : null;
            IgnoreNegativeOffensive = BasicEffect.GetBool(data, "ignoreNegativeOffensive") ?? false;
            IgnorePositiveDefensive = BasicEffect.GetBool(data, "ignorePositiveDefensive") ?? false;
            IgnoreOffensive = BasicEffect.GetBool(data, "ignoreOffensive") ?? false;
            IgnoreDefensive = BasicEffect.GetBool(data, "ignoreDefensive") ?? false;
            IgnoreImmunity = data.ContainsKey("ignoreImmunity") ? data["ignoreImmunity"] : (Category == MoveCategory.Status);
            Pp = BasicEffect.GetInt(data, "pp") ?? 0;
            NoPpBoosts = BasicEffect.GetBool(data, "noPPBoosts") ?? (BasicEffect.GetBool(data, "isZ") ?? false);
            IsZ = data.ContainsKey("isZ") ? data["isZ"] : false;
            IsMax = data.ContainsKey("isMax") ? data["isMax"] : false;
            Flags = data.ContainsKey("flags") ? (MoveFlags)data["flags"] : new MoveFlags();
            SelfSwitch = data.ContainsKey("selfSwitch") ? data["selfSwitch"] : null;
            NonGhostTarget = data.ContainsKey("nonGhostTarget") ? (MoveTarget?)data["nonGhostTarget"] : null;
            IgnoreAbility = BasicEffect.GetBool(data, "ignoreAbility") ?? false;
            Damage = data.ContainsKey("damage") ? data["damage"] : null;
            SpreadHit = BasicEffect.GetBool(data, "spreadHit") ?? false;
            ForceSTAB = BasicEffect.GetBool(data, "forceSTAB") ?? false;
            VolatileStatus = data.ContainsKey("volatileStatus") ? data["volatileStatus"]?.ToString() : null;

            // Max Move base power calculation
            if (Category != MoveCategory.Status && !data.ContainsKey("maxMove") && Id != "struggle")
            {
                MaxMove = new MaxMoveData { BasePower = 1 };
                if ((IsMax is bool b && b) || (IsZ is bool z && z))
                {
                    // already initialized to 1
                }
                else if (BasePower == 0)
                {
                    MaxMove.BasePower = 100;
                }
                else if (Type == "Fighting" || Type == "Poison")
                {
                    if (BasePower >= 150) MaxMove.BasePower = 100;
                    else if (BasePower >= 110) MaxMove.BasePower = 95;
                    else if (BasePower >= 75) MaxMove.BasePower = 90;
                    else if (BasePower >= 65) MaxMove.BasePower = 85;
                    else if (BasePower >= 55) MaxMove.BasePower = 80;
                    else if (BasePower >= 45) MaxMove.BasePower = 75;
                    else MaxMove.BasePower = 70;
                }
                else
                {
                    if (BasePower >= 150) MaxMove.BasePower = 150;
                    else if (BasePower >= 110) MaxMove.BasePower = 140;
                    else if (BasePower >= 75) MaxMove.BasePower = 130;
                    else if (BasePower >= 65) MaxMove.BasePower = 120;
                    else if (BasePower >= 55) MaxMove.BasePower = 110;
                    else if (BasePower >= 45) MaxMove.BasePower = 100;
                    else MaxMove.BasePower = 90;
                }
            }

            // Z-Move base power calculation
            if (Category != MoveCategory.Status && !data.ContainsKey("zMove") && !(IsZ is bool z2 && z2) && !(IsMax is bool m2 && m2) && Id != "struggle")
            {
                int basePower = BasePower;
                ZMove = new ZMoveData();
                if (data.ContainsKey("multihit") && data["multihit"] is Array) basePower *= 3;
                if (basePower == 0) ZMove.BasePower = 100;
                else if (basePower >= 140) ZMove.BasePower = 200;
                else if (basePower >= 130) ZMove.BasePower = 195;
                else if (basePower >= 120) ZMove.BasePower = 190;
                else if (basePower >= 110) ZMove.BasePower = 185;
                else if (basePower >= 100) ZMove.BasePower = 180;
                else if (basePower >= 90) ZMove.BasePower = 175;
                else if (basePower >= 80) ZMove.BasePower = 160;
                else if (basePower >= 70) ZMove.BasePower = 140;
                else if (basePower >= 60) ZMove.BasePower = 120;
                else ZMove.BasePower = 100;
            }

            // Generation assignment
            if (Gen == 0)
            {
                if (BasicEffect.GetInt(data, "num") >= 827 && !(IsMax is bool m3 && m3))
                {
                    Gen = 9;
                }
                else if (BasicEffect.GetInt(data, "num") >= 743)
                {
                    Gen = 8;
                }
                else if (BasicEffect.GetInt(data, "num") >= 622)
                {
                    Gen = 7;
                }
                else if (BasicEffect.GetInt(data, "num") >= 560)
                {
                    Gen = 6;
                }
                else if (BasicEffect.GetInt(data, "num") >= 468)
                {
                    Gen = 5;
                }
                else if (BasicEffect.GetInt(data, "num") >= 355)
                {
                    Gen = 4;
                }
                else if (BasicEffect.GetInt(data, "num") >= 252)
                {
                    Gen = 3;
                }
                else if (BasicEffect.GetInt(data, "num") >= 166)
                {
                    Gen = 2;
                }
                else if (BasicEffect.GetInt(data, "num") >= 1)
                {
                    Gen = 1;
                }
            }

            // TODO: assignMissingFields logic
        }
    }

    public class DexMoves
    {
        public ModdedDex Dex { get; }
        public Dictionary<string, Move> MoveCache { get; } = new();
        public IReadOnlyList<Move>? AllCache { get; set; } = null;

        public DexMoves(ModdedDex dex)
        {
            Dex = dex;
        }
    }
}