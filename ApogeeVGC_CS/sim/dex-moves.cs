using ApogeeVGC_CS.sim;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
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

    public interface IMoveFlags
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
        // Return int, false (null), or null
        Func<Battle, Pokemon, Pokemon, ActiveMove, int?>? BasePowerCallback { get; set; }

        // Return true to stop the move from being used
        Func<Battle, Pokemon, Pokemon?, ActiveMove, bool?>? BeforeMoveCallback { get; set; }

        Action<Battle, Pokemon, Pokemon, ActiveMove>? BeforeTurnCallback { get; set; }

        // Return int or false (null)
        Func<Battle, Pokemon, Pokemon, ActiveMove, int?>? DamageCallback { get; set; }

        Action<Battle, Pokemon>? PriorityChargeCallback { get; set; }

        Action<Battle, Pokemon>? OnDisableMove { get; set; }

        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterHit { get; set; }
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAfterSubDamage { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondarySelf { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondary { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMove { get; set; }

        int? OnDamagePriority { get; set; }

        // Return int, bool, or null
        Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnDamage { get; set; }

        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnBasePower { get; set; }

        // Return int or void (null)
        Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnEffectiveness { get; set; }

        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHitField { get; set; }

        // Return bool, null, empty string, or void
        Func<Battle, Side, Pokemon, ActiveMove, object?>? OnHitSide { get; set; }

        Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnModifyMove { get; set; }

        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyPriority { get; set; }

        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnMoveFail { get; set; }

        Action<Battle, ActiveMove, Pokemon, Pokemon>? OnModifyType { get; set; }

        Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnModifyTarget { get; set; }

        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnPrepareHit { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTry { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnTryHit { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitField { get; set; }
        Func<Battle, Side, Pokemon, ActiveMove, object?>? OnTryHitSide { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryImmunity { get; set; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryMove { get; set; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnUseMoveMessage { get; set; }
    }

    public interface IMoveData : IEffectData, IMoveEventMethods
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

    // helper class for MoveData
    public class ZMoveData
    {
        public int? BasePower { get; set; }
        public string? Effect { get; set; }
        public object? Boost { get; set; } // Replace with SparseBoostsTable if available
    }

    // Helper class for MoveData
    public class MaxMoveData
    {
        public int BasePower { get; set; }
    }

    // helper class for MoveData
    public class SelfBoostData
    {
        public SparseBoostsTable? Boosts { get; set; } // Replace with SparseBoostsTable if available
    }

    public interface IModdedMoveData : IMoveData
    {
        bool Inherit { get; set; }
        bool? IgniteBoosted { get; set; }
        bool? SettleBoosted { get; set; }
        bool? BodyOfWaterBoosted { get; set; }
        bool? LongWhipBoost { get; set; }
        int? Gen { get; set; }
    }

    // MoveDataTable and ModdedMoveDataTable
    public class MoveDataTable : Dictionary<IdEntry, IMoveData> { }
    public class ModdedMoveDataTable : Dictionary<IdEntry, IModdedMoveData> { }

    // Move class (readonly effectType)
    public class Move : BasicEffect, IMoveData
    {
        public override EffectType EffectType => EffectType.Move;
        // Inherit all properties from BasicEffect and MoveData as needed
    }

    // Element of MoveHitData
    public class MoveHitResult
    {
        public bool Crit { get; set; }
        public int TypeMod { get; set; }
        public bool ZBrokeProtect { get; set; }
    }

    public class MoveHitData : Dictionary<string, MoveHitResult> { }

    public class MutableMove : BasicEffect, IMoveData { }

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

    public enum MoveCategory
    {
        Physical,
        Special,
        Status
    }

    public class DataMove : BasicEffect, IMoveData
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

        //public int Gen { get; set; }
        //public string Id { get; set; } = string.Empty;

        public DataMove(IAnyObject data) : base(data)
        {
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