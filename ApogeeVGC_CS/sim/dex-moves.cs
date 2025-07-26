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

    public interface IMoveHitEffect
    {
        public object? Boosts { get; set; }
        
        public string? VolatileStatus { get; set; }
        public string? Terrain { get; set; }

        public string? SideCondition { get; set; }
        public string? SlotCondition { get; set; }

        public string? PseudoWeather { get; set; }
    }

    public interface IHitEffect : IMoveHitEffect
    {
        public object? OnHit { get; set; }
        
        public string? Status { get; set; }
        public string? Weather { get; set; }
    }

    public class HitEffect : IHitEffect
    {
        public object? OnHit { get; set; }
        public object? Boosts { get; set; }
        public string? Status { get; set; }
        public string? VolatileStatus { get; set; }
        public string? SideCondition { get; set; }
        public string? SlotCondition { get; set; }
        public string? PseudoWeather { get; set; }
        public string? Terrain { get; set; }
        public string? Weather { get; set; }
    }

    public interface ISecondaryEffect : IHitEffect
    {
        public int? Chance { get; set; }
        public Ability? Ability { get; set; }
        public bool? KingsRock { get; set; }
        public IHitEffect? Self { get; set; }
    }

    public class SecondaryEffect : ISecondaryEffect
    {
        public int? Chance { get; set; }
        public Ability? Ability { get; set; }
        public bool? KingsRock { get; set; }
        public IHitEffect? Self { get; set; }
        public object? OnHit { get; set; }
        public object? Boosts { get; set; }
        public string? Status { get; set; }
        public string? VolatileStatus { get; set; }
        public string? SideCondition { get; set; }
        public string? SlotCondition { get; set; }
        public string? PseudoWeather { get; set; }
        public string? Terrain { get; set; }
        public string? Weather { get; set; }
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

    // Using IMoveHitEffect to avoid duplication on Weather, Status and OnHit
    public interface IMoveData : IEffectData, IMoveEventMethods, IMoveHitEffect, IMove { }

    public class MoveData : IMoveData
    {
        public string Name { get; set; } = string.Empty;
        public string? Desc { get; set; }
        public int? Duration { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; set; }
        public string? EffectTypeString { get; set; }
        public bool? Infiltrates { get; set; }
        public Nonstandard? IsNonstandard { get; set; }
        public string? ShortDesc { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, int?>? BasePowerCallback { get; set; }
        public Func<Battle, Pokemon, Pokemon?, ActiveMove, bool?>? BeforeMoveCallback { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? BeforeTurnCallback { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, int?>? DamageCallback { get; set; }
        public Action<Battle, Pokemon>? PriorityChargeCallback { get; set; }
        public Action<Battle, Pokemon>? OnDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterHit { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAfterSubDamage { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMove { get; set; }
        public int? OnDamagePriority { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnBasePower { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnEffectiveness { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHitField { get; set; }
        public Func<Battle, Side, Pokemon, ActiveMove, object?>? OnHitSide { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyPriority { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnMoveFail { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnModifyTarget { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnPrepareHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTry { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitField { get; set; }
        public Func<Battle, Side, Pokemon, ActiveMove, object?>? OnTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnUseMoveMessage { get; set; }
        public object? Boosts { get; set; }
        public string? Status { get; set; }
        public string? VolatileStatus { get; set; }
        public string? SideCondition { get; set; }
        public string? SlotCondition { get; set; }
        public string? PseudoWeather { get; set; }
        public string? Terrain { get; set; }
        public string? Weather { get; set; }
        public Id Id { get; set; }
        public string Fullname { get; set; }
        public EffectType EffectType { get; set; }
        public bool Exists { get; set; }
        public int Num { get; set; }
        public int Gen { get; set; }
        public bool NoCopy { get; set; }
        public bool AffectsFainted { get; set; }
        public string SourceEffect { get; set; }
        public string? RealMove { get; set; }
        Id? IBasicEffect.Status { get; set; }
        Id? IBasicEffect.Weather { get; set; }
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

    public interface IMove : IBasicEffect { }


    public class Move(MoveData data) : BasicEffect(data), IMove { }

    // Element of MoveHitData
    public class MoveHitResult
    {
        public bool Crit { get; set; }
        public int TypeMod { get; set; }
        public bool ZBrokeProtect { get; set; }
    }

    public class MoveHitData : Dictionary<string, MoveHitResult> { }

    public interface IMutableMove : IMoveData
    {
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
        public IEffect? TypeChangerBoosted { get; set; }
        public bool? WillChangeForme { get; set; }
        public Pokemon? RuinedAtk { get; set; }
        public Pokemon? RuinedDef { get; set; }
        public Pokemon? RuinedSpA { get; set; }
        public Pokemon? RuinedSpD { get; set; }
        public bool? IsZOrMaxPowered { get; set; }
    }

    public class ActiveMove : IMutableMove
    {
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
        public object? SelfSwitch { get; set; }
        public bool? SpreadHit { get; set; }
        public string? StatusRoll { get; set; }
        public bool? StellarBoosted { get; set; }
        public object? TotalDamage { get; set; }
        public IEffect? TypeChangerBoosted { get; set; }
        public bool? WillChangeForme { get; set; }
        public Pokemon? RuinedAtk { get; set; }
        public Pokemon? RuinedDef { get; set; }
        public Pokemon? RuinedSpA { get; set; }
        public Pokemon? RuinedSpD { get; set; }
        public bool? IsZOrMaxPowered { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, int?>? BasePowerCallback { get; set; }
        public Func<Battle, Pokemon, Pokemon?, ActiveMove, bool?>? BeforeMoveCallback { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? BeforeTurnCallback { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, int?>? DamageCallback { get; set; }
        public Action<Battle, Pokemon>? PriorityChargeCallback { get; set; }
        public Action<Battle, Pokemon>? OnDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterHit { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAfterSubDamage { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMove { get; set; }
        public int? OnDamagePriority { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnBasePower { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnEffectiveness { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHitField { get; set; }
        public Func<Battle, Side, Pokemon, ActiveMove, object?>? OnHitSide { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyPriority { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnMoveFail { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnModifyTarget { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnPrepareHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTry { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitField { get; set; }
        public Func<Battle, Side, Pokemon, ActiveMove, object?>? OnTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnUseMoveMessage { get; set; }
        public object? Boosts { get; set; }
        public string? Status { get; set; }
        public string? VolatileStatus { get; set; }
        public string? SideCondition { get; set; }
        public string? SlotCondition { get; set; }
        public string? PseudoWeather { get; set; }
        public string? Terrain { get; set; }
        public string? Weather { get; set; }
        public Id Id { get; set; }
        public string Name { get; set; }
        public string Fullname { get; set; }
        public EffectType EffectType { get; set; }
        public bool Exists { get; set; }
        public int Num { get; set; }
        public int Gen { get; set; }
        public string? ShortDesc { get; set; }
        public string? Desc { get; set; }
        public Nonstandard? IsNonstandard { get; set; }
        public int? Duration { get; set; }
        public bool NoCopy { get; set; }
        public bool AffectsFainted { get; set; }
        public string SourceEffect { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; set; }
        public string? EffectTypeString { get; set; }
        public bool? Infiltrates { get; set; }
        public string? RealMove { get; set; }
        Id? IBasicEffect.Status { get; set; }
        Id? IBasicEffect.Weather { get; set; }
    }

    public enum MoveCategory
    {
        Physical,
        Special,
        Status
    }

    public interface IDataMove : IMoveData
    {
        public int CritRatio { get; set; }
        public bool HasSheerForce { get; set; }
        public MoveCategory Category { get; set; }
        public string? OverrideOffensivePokemon { get; set; }
        public bool IgnoreNegativeOffensive { get; set; }
        public bool IgnorePositiveDefensive { get; set; }
        public bool IgnoreOffensive { get; set; }
        public bool IgnoreDefensive { get; set; }
        public bool NoPpBoosts { get; set; }
        public object? IsZ { get; set; } // bool or string
        public ZMoveData? ZMove { get; set; }
        public object? IsMax { get; set; } // bool or string
        public MaxMoveData? MaxMove { get; set; }
        public MoveFlags Flags { get; set; }
        public object? SelfSwitch { get; set; } // "copyvolatile", "shedtail", or bool
        public MoveTarget? NonGhostTarget { get; set; }
        public bool IgnoreAbility { get; set; }
        public object? Damage { get; set; } // int, "level", false, or null
        public bool SpreadHit { get; set; }
        public double? SpreadModifier { get; set; }
        public int? CritModifier { get; set; }
        public bool ForceSTAB { get; set; }
        public string? VolatileStatus { get; set; }
    }

    public class DataMove(IDataMove data) : BasicEffect(data), IDataMove
    {
        // Initialize all IMoveEventMethods properties
        public Func<Battle, Pokemon, Pokemon, ActiveMove, int?>? BasePowerCallback { get; set; } = data.BasePowerCallback;
        public Func<Battle, Pokemon, Pokemon?, ActiveMove, bool?>? BeforeMoveCallback { get; set; } = data.BeforeMoveCallback;
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? BeforeTurnCallback { get; set; } = data.BeforeTurnCallback;
        public Func<Battle, Pokemon, Pokemon, ActiveMove, int?>? DamageCallback { get; set; } = data.DamageCallback;
        public Action<Battle, Pokemon>? PriorityChargeCallback { get; set; } = data.PriorityChargeCallback;
        public Action<Battle, Pokemon>? OnDisableMove { get; set; } = data.OnDisableMove;
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterHit { get; set; } = data.OnAfterHit;
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAfterSubDamage { get; set; } = data.OnAfterSubDamage;
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondarySelf { get; set; } = data.OnAfterMoveSecondarySelf;
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondary { get; set; } = data.OnAfterMoveSecondary;
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMove { get; set; } = data.OnAfterMove;
        public int? OnDamagePriority { get; set; } = data.OnDamagePriority;
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnDamage { get; set; } = data.OnDamage;
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnBasePower { get; set; } = data.OnBasePower;
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnEffectiveness { get; set; } = data.OnEffectiveness;
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; set; } = data.OnHit;
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHitField { get; set; } = data.OnHitField;
        public Func<Battle, Side, Pokemon, ActiveMove, object?>? OnHitSide { get; set; } = data.OnHitSide;
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnModifyMove { get; set; } = data.OnModifyMove;
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyPriority { get; set; } = data.OnModifyPriority;
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnMoveFail { get; set; } = data.OnMoveFail;
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnModifyType { get; set; } = data.OnModifyType;
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnModifyTarget { get; set; } = data.OnModifyTarget;
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnPrepareHit { get; set; } = data.OnPrepareHit;
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTry { get; set; } = data.OnTry;
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnTryHit { get; set; } = data.OnTryHit;
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitField { get; set; } = data.OnTryHitField;
        public Func<Battle, Side, Pokemon, ActiveMove, object?>? OnTryHitSide { get; set; } = data.OnTryHitSide;
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryImmunity { get; set; } = data.OnTryImmunity;
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryMove { get; set; } = data.OnTryMove;
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnUseMoveMessage { get; set; } = data.OnUseMoveMessage;

        // Initialize all IHitEffect properties
        public object? Boosts { get; set; } = data.Boosts;
        public string? VolatileStatus { get; set; } = data.VolatileStatus;
        public string? SideCondition { get; set; } = data.SideCondition;
        public string? SlotCondition { get; set; } = data.SlotCondition;
        public string? PseudoWeather { get; set; } = data.PseudoWeather;
        public string? Terrain { get; set; } = data.Terrain;

        // IDataMove properties - these need to be initialized from data or set to defaults
        public int CritRatio { get; set; } = data.CritRatio;
        public bool HasSheerForce { get; set; } = data.HasSheerForce;
        public MoveCategory Category { get; set; } = data.Category;
        public string? OverrideOffensivePokemon { get; set; } = data.OverrideOffensivePokemon;
        public bool IgnoreNegativeOffensive { get; set; } = data.IgnoreNegativeOffensive;
        public bool IgnorePositiveDefensive { get; set; } = data.IgnorePositiveDefensive;
        public bool IgnoreOffensive { get; set; } = data.IgnoreOffensive;
        public bool IgnoreDefensive { get; set; } = data.IgnoreDefensive;
        public bool NoPpBoosts { get; set; } = data.NoPpBoosts;
        public object? IsZ { get; set; } = data.IsZ; // bool or string
        public ZMoveData? ZMove { get; set; } = data.ZMove;
        public object? IsMax { get; set; } = data.IsMax; // bool or string
        public MaxMoveData? MaxMove { get; set; } = data.MaxMove;
        public MoveFlags Flags { get; set; } = data.Flags;
        public object? SelfSwitch { get; set; } = data.SelfSwitch; // "copyvolatile", "shedtail", or bool
        public MoveTarget? NonGhostTarget { get; set; } = data.NonGhostTarget;
        public bool IgnoreAbility { get; set; } = data.IgnoreAbility;
        public object? Damage { get; set; } = data.Damage; // int, "level", false, or null
        public bool SpreadHit { get; set; } = data.SpreadHit;
        public double? SpreadModifier { get; set; } = data.SpreadModifier;
        public int? CritModifier { get; set; } = data.CritModifier;
        public bool ForceSTAB { get; set; } = data.ForceSTAB;
        MoveFlags IDataMove.Flags { get; set; } = data.Flags;

        public void Init()
        {
            InitBasicEffect();

            Fullname = $"move: {Name}";
            EffectType = EffectType.Move;

        }
    }

    public class DexMoves(ModdedDex dex)
    {
        public ModdedDex Dex { get; } = dex;
        public Dictionary<string, Move> MoveCache { get; } = new();
        public IReadOnlyList<Move>? AllCache { get; set; } = null;
    }
}