using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.GameObjects;

public enum ItemId
{
    Leftovers,
    ChoiceSpecs,
    FlameOrb,
    RockyHelmet,
    LightClay,
    AssaultVest,

    BoosterEnergy,
    ToxicOrb,
    TerrainExtender,
    AbilityShield,
    IronBall,
    AirBalloon,
    ProtectivePads,

    LeppaBerry,
    AguavBerry,
    EnigmaBerry,
    FigyBerry,
    IapapaBerry,
    MagoBerry,
    SitrusBerry,
    RustedSword,
    RustedShield,
    PowerHerb,
    JabocaBerry,
    RowapBerry,
    RedCard,
    UtilityUmbrella,
    CustapBerry,
    BlunderPolicy,
    LoadedDice,

    None,
}

public record FlingData
{
    public int BasePower { get; init; }
    public ConditionId? Status { get; init; }
}

public record Item : IPokemonEventMethods, IEffect, IBasicEffect
{
    public ItemId Id { get; init; }
    public EffectStateId EffectStateId => Id;
    public EffectType EffectType => EffectType.Item;
    public required string Name { get; init; }
    public string FullName => $"Item: {Name}";
    public int SpriteNum { get; init; }

    public FlingData? Fling
    {
        get
        {
            if (IsBerry)
            {
                return new FlingData { BasePower = 10 };
            }

            if (Id.ToString().EndsWith("Plate"))
            {
                return new FlingData { BasePower = 90 };
            }

            if (OnDrive != null)
            {
                return new FlingData { BasePower = 70 };
            }

            if (OnMemory != null)
            {
                return new FlingData { BasePower = 50 };
            }

            return field;
        }
        init;
    }

    public MoveType? OnDrive { get; init; }
    public int Num { get; init; }

    public int Gen
    {
        get
        {
            if (field is >= 1 and <= 9) return field;
            return Num switch
            {
                >= 1124 => 9,
                >= 927 => 8,
                >= 689 => 7,
                >= 577 => 6,
                >= 537 => 5,
                >= 377 => 4,
                >= 1 => 3,
                _ => field,
            };
        }
        init;
    }

    public MoveType? OnMemory { get; init; }
    public bool IsBerry { get; init; }
    public bool IgnoreKlutz { get; init; }
    public string? OnPlate { get; init; }
    public bool IsGem { get; init; }
    public bool IsPokeball { get; init; }
    public bool IsPrimalOrb { get; init; }
    public Condition? Condition { get; init; }
    public string? ForcedForme { get; init; }
    public bool? IsChoice { get; init; }
    public (int BasePower, string Type)? NaturalGift { get; init; }
    public ItemBoosts? Boosts { get; init; }

    public OnItemEatUse? OnEat { get; init; }
    public OnItemEatUse? OnUse { get; init; }
    public Action<IBattle, Pokemon>? OnStart { get; init; }
    public Action<IBattle, Pokemon>? OnEnd { get; init; }

    public bool AffectsFainted { get; init; }

    public Item Copy()
    {
        return this with { };
    }

    #region PokemonMoveEventMethods Implementation

    public Action<IBattle, int, Pokemon, Pokemon, ActiveMove>? OnDamagingHit { get; init; }
    public Action<IBattle, Pokemon>? OnEmergencyExit { get; init; }
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterEachBoost { get; init; }
    public VoidSourceMoveHandler? OnAfterHit { get; init; }
    public Action<IBattle, Pokemon>? OnAfterMega { get; init; }
    public Action<IBattle, Condition, Pokemon, Pokemon, IEffect>? OnAfterSetStatus { get; init; }
    public OnAfterSubDamageHandler? OnAfterSubDamage { get; init; }
    public Action<IBattle, Pokemon>? OnAfterSwitchInSelf { get; init; }
    public Action<IBattle, Pokemon>? OnAfterTerastallization { get; init; }
    public Action<IBattle, Item, Pokemon>? OnAfterUseItem { get; init; }
    public Action<IBattle, Item, Pokemon>? OnAfterTakeItem { get; init; }
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterBoost { get; init; }
    public Action<IBattle, int, Pokemon, Pokemon, IEffect>? OnAfterFaint { get; init; }
    public VoidSourceMoveHandler? OnAfterMoveSecondarySelf { get; init; }
    public VoidMoveHandler? OnAfterMoveSecondary { get; init; }
    public VoidSourceMoveHandler? OnAfterMove { get; init; }
    public VoidSourceMoveHandler? OnAfterMoveSelf { get; init; }
    public Action<IBattle, Pokemon, Pokemon>? OnAttract { get; init; }
    public Func<IBattle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnAccuracy { get; init; }
    public ModifierSourceMoveHandler? OnBasePower { get; init; }
    public Action<IBattle, Pokemon, IEffect>? OnBeforeFaint { get; init; }
    public VoidSourceMoveHandler? OnBeforeMove { get; init; }
    public Action<IBattle, Pokemon>? OnBeforeSwitchIn { get; init; }
    public Action<IBattle, Pokemon>? OnBeforeSwitchOut { get; init; }
    public Action<IBattle, Pokemon>? OnBeforeTurn { get; init; }
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnChangeBoost { get; init; }
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnTryBoost { get; init; }
    public VoidSourceMoveHandler? OnChargeMove { get; init; }
    public OnCriticalHit? OnCriticalHit { get; init; }
    public Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>? OnDamage { get; init; }
    public Func<IBattle, Pokemon, Pokemon, IntVoidUnion>? OnDeductPp { get; init; }
    public Action<IBattle, Pokemon>? OnDisableMove { get; init; }
    public Action<IBattle, Pokemon, Pokemon?, ActiveMove?>? OnDragOut { get; init; }
    public Action<IBattle, Item, Pokemon>? OnEatItem { get; init; }
    public OnEffectivenessHandler? OnEffectiveness { get; init; }
    public Action<IBattle, Pokemon>? OnEntryHazard { get; init; }
    public VoidEffectHandler? OnFaint { get; init; }
    public OnFlinch? OnFlinch { get; init; }
    public OnFractionalPriority? OnFractionalPriority { get; init; }
    public ResultMoveHandler? OnHit { get; init; }
    public Action<IBattle, PokemonType, Pokemon>? OnImmunity { get; init; }
    public OnLockMove? OnLockMove { get; init; }
    public Action<IBattle, Pokemon>? OnMaybeTrapPokemon { get; init; }
    public ModifierMoveHandler? OnModifyAccuracy { get; init; }
    public ModifierSourceMoveHandler? OnModifyAtk { get; init; }
    public Func<IBattle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>? OnModifyBoost { get; init; }
    public ModifierSourceMoveHandler? OnModifyCritRatio { get; init; }
    public ModifierSourceMoveHandler? OnModifyDamage { get; init; }
    public ModifierMoveHandler? OnModifyDef { get; init; }
    public OnModifyMoveHandler? OnModifyMove { get; init; }
    public ModifierSourceMoveHandler? OnModifyPriority { get; init; }
    public Action<IBattle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnModifySecondaries { get; init; }
    public OnModifyTypeHandler? OnModifyType { get; init; }
    public OnModifyTargetHandler? OnModifyTarget { get; init; }
    public ModifierSourceMoveHandler? OnModifySpA { get; init; }
    public ModifierMoveHandler? OnModifySpD { get; init; }
    public Func<IBattle, int, Pokemon, IntVoidUnion>? OnModifySpe { get; init; }
    public ModifierSourceMoveHandler? OnModifyStab { get; init; }
    public Func<IBattle, int, Pokemon, IntVoidUnion>? OnModifyWeight { get; init; }
    public VoidMoveHandler? OnMoveAborted { get; init; }
    public OnNegateImmunity? OnNegateImmunity { get; init; }
    public Func<IBattle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>? OnOverrideAction { get; init; }
    public ResultSourceMoveHandler? OnPrepareHit { get; init; }
    public Action<IBattle, Pokemon, Pokemon, Condition>? OnPseudoWeatherChange { get; init; }
    public Func<IBattle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>? OnRedirectTarget { get; init; }
    public Action<IBattle, Pokemon, Pokemon, IEffect>? OnResidual { get; init; }
    public Action<IBattle, Ability, Pokemon, Pokemon, IEffect>? OnSetAbility { get; init; }
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnSetStatus { get; init; }
    public Func<IBattle, Pokemon, Pokemon, Condition, BoolVoidUnion>? OnSetWeather { get; init; }
    public Action<IBattle, Side, Pokemon, Condition>? OnSideConditionStart { get; init; }
    public Func<IBattle, Pokemon, BoolVoidUnion>? OnStallMove { get; init; }
    public Action<IBattle, Pokemon>? OnSwitchIn { get; init; }
    public Action<IBattle, Pokemon>? OnSwitchOut { get; init; }
    public Action<IBattle, Pokemon, Pokemon>? OnSwap { get; init; }
    public OnTakeItem? OnTakeItem { get; init; }
    public Action<IBattle, Pokemon, Pokemon, IEffect>? OnWeatherChange { get; init; }
    public Action<IBattle, Pokemon, Pokemon, IEffect>? OnTerrainChange { get; init; }
    public Action<IBattle, Pokemon>? OnTrapPokemon { get; init; }
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnTryAddVolatile { get; init; }
    public OnTryEatItem? OnTryEatItem { get; init; }
    public OnTryHeal? OnTryHeal { get; init; }
    public ExtResultSourceMoveHandler? OnTryHit { get; init; }
    public ResultMoveHandler? OnTryHitField { get; init; }
    public ResultMoveHandler? OnTryHitSide { get; init; }
    public ExtResultMoveHandler? OnInvulnerability { get; init; }
    public ResultSourceMoveHandler? OnTryMove { get; init; }
    public Func<IBattle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnTryPrimaryHit { get; init; }
    public Func<IBattle, PokemonType[], Pokemon, TypesVoidUnion>? OnType { get; init; }
    public Action<IBattle, Item, Pokemon>? OnUseItem { get; init; }
    public Action<IBattle, Pokemon>? OnUpdate { get; init; }
    public Action<IBattle, Pokemon, object?, Condition>? OnWeather { get; init; }
    public ModifierSourceMoveHandler? OnWeatherModifyDamage { get; init; }
    public ModifierSourceMoveHandler? OnModifyDamagePhase1 { get; init; }
    public ModifierSourceMoveHandler? OnModifyDamagePhase2 { get; init; }
    public Action<IBattle, int, Pokemon, Pokemon, ActiveMove>? OnFoeDamagingHit { get; init; }
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon>? OnFoeAfterEachBoost { get; init; }
    public VoidSourceMoveHandler? OnFoeAfterHit { get; init; }
    public Action<IBattle, Condition, Pokemon, Pokemon, IEffect>? OnFoeAfterSetStatus { get; init; }
    public OnAfterSubDamageHandler? OnFoeAfterSubDamage { get; init; }
    public Action<IBattle, Pokemon>? OnFoeAfterSwitchInSelf { get; init; }
    public Action<IBattle, Item, Pokemon>? OnFoeAfterUseItem { get; init; }
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeAfterBoost { get; init; }
    public Action<IBattle, int, Pokemon, Pokemon, IEffect>? OnFoeAfterFaint { get; init; }
    public VoidSourceMoveHandler? OnFoeAfterMoveSecondarySelf { get; init; }
    public VoidMoveHandler? OnFoeAfterMoveSecondary { get; init; }
    public VoidSourceMoveHandler? OnFoeAfterMove { get; init; }
    public VoidSourceMoveHandler? OnFoeAfterMoveSelf { get; init; }
    public Action<IBattle, Pokemon, Pokemon>? OnFoeAttract { get; init; }
    public Func<IBattle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnFoeAccuracy { get; init; }
    public ModifierSourceMoveHandler? OnFoeBasePower { get; init; }
    public Action<IBattle, Pokemon, IEffect>? OnFoeBeforeFaint { get; init; }
    public VoidSourceMoveHandler? OnFoeBeforeMove { get; init; }
    public Action<IBattle, Pokemon>? OnFoeBeforeSwitchIn { get; init; }
    public Action<IBattle, Pokemon>? OnFoeBeforeSwitchOut { get; init; }
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeTryBoost { get; init; }
    public VoidSourceMoveHandler? OnFoeChargeMove { get; init; }
    public OnCriticalHit? OnFoeCriticalHit { get; init; }
    public Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>? OnFoeDamage { get; init; }
    public Func<IBattle, Pokemon, Pokemon, IntVoidUnion>? OnFoeDeductPp { get; init; }
    public Action<IBattle, Pokemon>? OnFoeDisableMove { get; init; }
    public Action<IBattle, Pokemon, Pokemon?, ActiveMove?>? OnFoeDragOut { get; init; }
    public Action<IBattle, Item, Pokemon>? OnFoeEatItem { get; init; }
    public OnEffectivenessHandler? OnFoeEffectiveness { get; init; }
    public VoidEffectHandler? OnFoeFaint { get; init; }
    public OnFlinch? OnFoeFlinch { get; init; }
    public ResultMoveHandler? OnFoeHit { get; init; }
    public Action<IBattle, PokemonType, Pokemon>? OnFoeImmunity { get; init; }
    public OnLockMove? OnFoeLockMove { get; init; }
    public Action<IBattle, Pokemon, Pokemon?>? OnFoeMaybeTrapPokemon { get; init; }
    public ModifierMoveHandler? OnFoeModifyAccuracy { get; init; }
    public ModifierSourceMoveHandler? OnFoeModifyAtk { get; init; }
    public Func<IBattle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>? OnFoeModifyBoost { get; init; }
    public ModifierSourceMoveHandler? OnFoeModifyCritRatio { get; init; }
    public ModifierSourceMoveHandler? OnFoeModifyDamage { get; init; }
    public ModifierMoveHandler? OnFoeModifyDef { get; init; }
    public OnModifyMoveHandler? OnFoeModifyMove { get; init; }
    public ModifierSourceMoveHandler? OnFoeModifyPriority { get; init; }
    public Action<IBattle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnFoeModifySecondaries { get; init; }
    public ModifierSourceMoveHandler? OnFoeModifySpA { get; init; }
    public ModifierMoveHandler? OnFoeModifySpD { get; init; }
    public Func<IBattle, int, Pokemon, IntVoidUnion>? OnFoeModifySpe { get; init; }
    public ModifierSourceMoveHandler? OnFoeModifyStab { get; init; }
    public OnModifyTypeHandler? OnFoeModifyType { get; init; }
    public OnModifyTargetHandler? OnFoeModifyTarget { get; init; }
    public Func<IBattle, int, Pokemon, IntVoidUnion>? OnFoeModifyWeight { get; init; }
    public VoidMoveHandler? OnFoeMoveAborted { get; init; }
    public OnNegateImmunity? OnFoeNegateImmunity { get; init; }
    public Func<IBattle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>? OnFoeOverrideAction { get; init; }
    public ResultSourceMoveHandler? OnFoePrepareHit { get; init; }
    public Func<IBattle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>? OnFoeRedirectTarget { get; init; }
    public Action<IBattle, PokemonSideUnion, Pokemon, IEffect>? OnFoeResidual { get; init; }
    public Func<IBattle, Ability, Pokemon, Pokemon, IEffect, BoolVoidUnion>? OnFoeSetAbility { get; init; }
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnFoeSetStatus { get; init; }
    public Func<IBattle, Pokemon, Pokemon, Condition, BoolVoidUnion>? OnFoeSetWeather { get; init; }
    public Func<IBattle, Pokemon, BoolVoidUnion>? OnFoeStallMove { get; init; }
    public Action<IBattle, Pokemon>? OnFoeSwitchOut { get; init; }
    public OnTakeItem? OnFoeTakeItem { get; init; }
    public Action<IBattle, Pokemon>? OnFoeTerrain { get; init; }
    public Action<IBattle, Pokemon>? OnFoeTrapPokemon { get; init; }
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnFoeTryAddVolatile { get; init; }
    public OnTryEatItem? OnFoeTryEatItem { get; init; }
    public OnTryHeal? OnFoeTryHeal { get; init; }
    public ExtResultSourceMoveHandler? OnFoeTryHit { get; init; }
    public ResultMoveHandler? OnFoeTryHitField { get; init; }
    public ResultMoveHandler? OnFoeTryHitSide { get; init; }
    public ExtResultMoveHandler? OnFoeInvulnerability { get; init; }
    public ResultSourceMoveHandler? OnFoeTryMove { get; init; }
    public Func<IBattle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnFoeTryPrimaryHit { get; init; }
    public Func<IBattle, PokemonType[], Pokemon, TypesVoidUnion>? OnFoeType { get; init; }
    public ModifierSourceMoveHandler? OnFoeWeatherModifyDamage { get; init; }
    public ModifierSourceMoveHandler? OnFoeModifyDamagePhase1 { get; init; }
    public ModifierSourceMoveHandler? OnFoeModifyDamagePhase2 { get; init; }
    public Action<IBattle, int, Pokemon, Pokemon, ActiveMove>? OnSourceDamagingHit { get; init; }
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon>? OnSourceAfterEachBoost { get; init; }
    public VoidSourceMoveHandler? OnSourceAfterHit { get; init; }
    public Action<IBattle, Condition, Pokemon, Pokemon, IEffect>? OnSourceAfterSetStatus { get; init; }
    public OnAfterSubDamageHandler? OnSourceAfterSubDamage { get; init; }
    public Action<IBattle, Pokemon>? OnSourceAfterSwitchInSelf { get; init; }
    public Action<IBattle, Item, Pokemon>? OnSourceAfterUseItem { get; init; }
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceAfterBoost { get; init; }
    public Action<IBattle, int, Pokemon, Pokemon, IEffect>? OnSourceAfterFaint { get; init; }
    public VoidSourceMoveHandler? OnSourceAfterMoveSecondarySelf { get; init; }
    public VoidMoveHandler? OnSourceAfterMoveSecondary { get; init; }
    public VoidSourceMoveHandler? OnSourceAfterMove { get; init; }
    public VoidSourceMoveHandler? OnSourceAfterMoveSelf { get; init; }
    public Action<IBattle, Pokemon, Pokemon>? OnSourceAttract { get; init; }
    public Func<IBattle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnSourceAccuracy { get; init; }
    public ModifierSourceMoveHandler? OnSourceBasePower { get; init; }
    public Action<IBattle, Pokemon, IEffect>? OnSourceBeforeFaint { get; init; }
    public VoidSourceMoveHandler? OnSourceBeforeMove { get; init; }
    public Action<IBattle, Pokemon>? OnSourceBeforeSwitchIn { get; init; }
    public Action<IBattle, Pokemon>? OnSourceBeforeSwitchOut { get; init; }
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceTryBoost { get; init; }
    public VoidSourceMoveHandler? OnSourceChargeMove { get; init; }
    public OnCriticalHit? OnSourceCriticalHit { get; init; }
    public Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>? OnSourceDamage { get; init; }
    public Func<IBattle, Pokemon, Pokemon, IntVoidUnion>? OnSourceDeductPp { get; init; }
    public Action<IBattle, Pokemon>? OnSourceDisableMove { get; init; }
    public Action<IBattle, Pokemon, Pokemon?, ActiveMove?>? OnSourceDragOut { get; init; }
    public Action<IBattle, Item, Pokemon>? OnSourceEatItem { get; init; }
    public OnEffectivenessHandler? OnSourceEffectiveness { get; init; }
    public VoidEffectHandler? OnSourceFaint { get; init; }
    public OnFlinch? OnSourceFlinch { get; init; }
    public ResultMoveHandler? OnSourceHit { get; init; }
    public Action<IBattle, PokemonType, Pokemon>? OnSourceImmunity { get; init; }
    public OnLockMove? OnSourceLockMove { get; init; }
    public Action<IBattle, Pokemon>? OnSourceMaybeTrapPokemon { get; init; }
    public ModifierMoveHandler? OnSourceModifyAccuracy { get; init; }
    public ModifierSourceMoveHandler? OnSourceModifyAtk { get; init; }
    public Func<IBattle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>? OnSourceModifyBoost { get; init; }
    public ModifierSourceMoveHandler? OnSourceModifyCritRatio { get; init; }
    public ModifierSourceMoveHandler? OnSourceModifyDamage { get; init; }
    public ModifierMoveHandler? OnSourceModifyDef { get; init; }
    public OnModifyMoveHandler? OnSourceModifyMove { get; init; }
    public ModifierSourceMoveHandler? OnSourceModifyPriority { get; init; }

    public Action<IBattle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnSourceModifySecondaries
    {
        get;
        init;
    }

    public ModifierSourceMoveHandler? OnSourceModifySpA { get; init; }
    public ModifierMoveHandler? OnSourceModifySpD { get; init; }
    public Func<IBattle, int, Pokemon, IntVoidUnion>? OnSourceModifySpe { get; init; }
    public ModifierSourceMoveHandler? OnSourceModifyStab { get; init; }
    public OnModifyTypeHandler? OnSourceModifyType { get; init; }
    public OnModifyTargetHandler? OnSourceModifyTarget { get; init; }
    public Func<IBattle, int, Pokemon, IntVoidUnion>? OnSourceModifyWeight { get; init; }
    public VoidMoveHandler? OnSourceMoveAborted { get; init; }
    public OnNegateImmunity? OnSourceNegateImmunity { get; init; }
    public Func<IBattle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>? OnSourceOverrideAction { get; init; }
    public ResultSourceMoveHandler? OnSourcePrepareHit { get; init; }
    public Func<IBattle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>? OnSourceRedirectTarget { get; init; }
    public Action<IBattle, PokemonSideUnion, Pokemon, IEffect>? OnSourceResidual { get; init; }
    public Func<IBattle, Ability, Pokemon, Pokemon, IEffect, BoolVoidUnion>? OnSourceSetAbility { get; init; }
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnSourceSetStatus { get; init; }
    public Func<IBattle, Pokemon, Pokemon, Condition, BoolVoidUnion>? OnSourceSetWeather { get; init; }
    public Func<IBattle, Pokemon, BoolVoidUnion>? OnSourceStallMove { get; init; }
    public Action<IBattle, Pokemon>? OnSourceSwitchOut { get; init; }
    public OnTakeItem? OnSourceTakeItem { get; init; }
    public Action<IBattle, Pokemon>? OnSourceTerrain { get; init; }
    public Action<IBattle, Pokemon>? OnSourceTrapPokemon { get; init; }
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnSourceTryAddVolatile { get; init; }
    public OnTryEatItem? OnSourceTryEatItem { get; init; }
    public OnTryHeal? OnSourceTryHeal { get; init; }
    public ExtResultSourceMoveHandler? OnSourceTryHit { get; init; }
    public ResultMoveHandler? OnSourceTryHitField { get; init; }
    public ResultMoveHandler? OnSourceTryHitSide { get; init; }
    public ExtResultMoveHandler? OnSourceInvulnerability { get; init; }
    public ResultSourceMoveHandler? OnSourceTryMove { get; init; }
    public Func<IBattle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnSourceTryPrimaryHit { get; init; }
    public Func<IBattle, PokemonType[], Pokemon, TypesVoidUnion>? OnSourceType { get; init; }
    public ModifierSourceMoveHandler? OnSourceWeatherModifyDamage { get; init; }
    public ModifierSourceMoveHandler? OnSourceModifyDamagePhase1 { get; init; }
    public ModifierSourceMoveHandler? OnSourceModifyDamagePhase2 { get; init; }
    public Action<IBattle, int, Pokemon, Pokemon, ActiveMove>? OnAnyDamagingHit { get; init; }
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon>? OnAnyAfterEachBoost { get; init; }
    public VoidSourceMoveHandler? OnAnyAfterHit { get; init; }
    public Action<IBattle, Condition, Pokemon, Pokemon, IEffect>? OnAnyAfterSetStatus { get; init; }
    public OnAfterSubDamageHandler? OnAnyAfterSubDamage { get; init; }
    public Action<IBattle, Pokemon>? OnAnyAfterSwitchInSelf { get; init; }
    public Action<IBattle, Item, Pokemon>? OnAnyAfterUseItem { get; init; }
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyAfterBoost { get; init; }
    public Action<IBattle, int, Pokemon, Pokemon, IEffect>? OnAnyAfterFaint { get; init; }
    public Action<IBattle, Pokemon>? OnAnyAfterMega { get; init; }
    public VoidSourceMoveHandler? OnAnyAfterMoveSecondarySelf { get; init; }
    public VoidMoveHandler? OnAnyAfterMoveSecondary { get; init; }
    public VoidSourceMoveHandler? OnAnyAfterMove { get; init; }
    public VoidSourceMoveHandler? OnAnyAfterMoveSelf { get; init; }
    public Action<IBattle, Pokemon>? OnAnyAfterTerastallization { get; init; }
    public Action<IBattle, Pokemon, Pokemon>? OnAnyAttract { get; init; }
    public Func<IBattle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnAnyAccuracy { get; init; }
    public ModifierSourceMoveHandler? OnAnyBasePower { get; init; }
    public Action<IBattle, Pokemon, IEffect>? OnAnyBeforeFaint { get; init; }
    public VoidSourceMoveHandler? OnAnyBeforeMove { get; init; }
    public Action<IBattle, Pokemon>? OnAnyBeforeSwitchIn { get; init; }
    public Action<IBattle, Pokemon>? OnAnyBeforeSwitchOut { get; init; }
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyTryBoost { get; init; }
    public VoidSourceMoveHandler? OnAnyChargeMove { get; init; }
    public OnCriticalHit? OnAnyCriticalHit { get; init; }
    public Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>? OnAnyDamage { get; init; }
    public Func<IBattle, Pokemon, Pokemon, IntVoidUnion>? OnAnyDeductPp { get; init; }
    public Action<IBattle, Pokemon>? OnAnyDisableMove { get; init; }
    public Action<IBattle, Pokemon, Pokemon?, ActiveMove?>? OnAnyDragOut { get; init; }
    public Action<IBattle, Item, Pokemon>? OnAnyEatItem { get; init; }
    public OnEffectivenessHandler? OnAnyEffectiveness { get; init; }
    public VoidEffectHandler? OnAnyFaint { get; init; }
    public OnFlinch? OnAnyFlinch { get; init; }
    public ResultMoveHandler? OnAnyHit { get; init; }
    public Action<IBattle, PokemonType, Pokemon>? OnAnyImmunity { get; init; }
    public OnLockMove? OnAnyLockMove { get; init; }
    public Action<IBattle, Pokemon, Pokemon?>? OnAnyMaybeTrapPokemon { get; init; }
    public ModifierMoveHandler? OnAnyModifyAccuracy { get; init; }
    public ModifierSourceMoveHandler? OnAnyModifyAtk { get; init; }
    public Func<IBattle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>? OnAnyModifyBoost { get; init; }
    public ModifierSourceMoveHandler? OnAnyModifyCritRatio { get; init; }
    public ModifierSourceMoveHandler? OnAnyModifyDamage { get; init; }
    public ModifierMoveHandler? OnAnyModifyDef { get; init; }
    public OnModifyMoveHandler? OnAnyModifyMove { get; init; }
    public ModifierSourceMoveHandler? OnAnyModifyPriority { get; init; }
    public Action<IBattle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAnyModifySecondaries { get; init; }
    public ModifierSourceMoveHandler? OnAnyModifySpA { get; init; }
    public ModifierMoveHandler? OnAnyModifySpD { get; init; }
    public Func<IBattle, int, Pokemon, IntVoidUnion>? OnAnyModifySpe { get; init; }
    public ModifierSourceMoveHandler? OnAnyModifyStab { get; init; }
    public OnModifyTypeHandler? OnAnyModifyType { get; init; }
    public OnModifyTargetHandler? OnAnyModifyTarget { get; init; }
    public Func<IBattle, int, Pokemon, IntVoidUnion>? OnAnyModifyWeight { get; init; }
    public Action<IBattle, Pokemon, Pokemon, ActiveMove>? OnAnyMoveAborted { get; init; }
    public OnNegateImmunity? OnAnyNegateImmunity { get; init; }
    public Func<IBattle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>? OnAnyOverrideAction { get; init; }
    public ResultSourceMoveHandler? OnAnyPrepareHit { get; init; }
    public Action<IBattle, Pokemon, Pokemon, Condition>? OnAnyPseudoWeatherChange { get; init; }
    public Func<IBattle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>? OnAnyRedirectTarget { get; init; }
    public Action<IBattle, PokemonSideUnion, Pokemon, IEffect>? OnAnyResidual { get; init; }
    public Func<IBattle, Ability, Pokemon, Pokemon, IEffect, bool?>? OnAnySetAbility { get; init; }
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion>? OnAnySetStatus { get; init; }
    public Func<IBattle, Pokemon, Pokemon, Condition, BoolVoidUnion>? OnAnySetWeather { get; init; }
    public Func<IBattle, Pokemon, BoolVoidUnion>? OnAnyStallMove { get; init; }
    public Action<IBattle, Pokemon>? OnAnySwitchIn { get; init; }
    public Action<IBattle, Pokemon>? OnAnySwitchOut { get; init; }
    public OnTakeItem? OnAnyTakeItem { get; init; }
    public Action<IBattle, Pokemon>? OnAnyTerrain { get; init; }
    public Action<IBattle, Pokemon>? OnAnyTrapPokemon { get; init; }
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnAnyTryAddVolatile { get; init; }
    public OnTryEatItem? OnAnyTryEatItem { get; init; }
    public OnTryHeal? OnAnyTryHeal { get; init; }
    public ExtResultSourceMoveHandler? OnAnyTryHit { get; init; }
    public ResultMoveHandler? OnAnyTryHitField { get; init; }
    public ResultMoveHandler? OnAnyTryHitSide { get; init; }
    public ExtResultMoveHandler? OnAnyInvulnerability { get; init; }
    public ResultSourceMoveHandler? OnAnyTryMove { get; init; }
    public Func<IBattle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnAnyTryPrimaryHit { get; init; }
    public Func<IBattle, PokemonType[], Pokemon, TypesVoidUnion>? OnAnyType { get; init; }
    public ModifierSourceMoveHandler? OnAnyWeatherModifyDamage { get; init; }
    public ModifierSourceMoveHandler? OnAnyModifyDamagePhase1 { get; init; }
    public ModifierSourceMoveHandler? OnAnyModifyDamagePhase2 { get; init; }
    public int? OnAccuracyPriority { get; init; }
    public int? OnDamagingHitOrder { get; init; }
    public int? OnAfterMoveSecondaryPriority { get; init; }
    public int? OnAfterMoveSecondarySelfPriority { get; init; }
    public int? OnAfterMoveSelfPriority { get; init; }
    public int? OnAfterSetStatusPriority { get; init; }
    public int? OnAnyBasePowerPriority { get; init; }
    public int? OnAnyInvulnerabilityPriority { get; init; }
    public int? OnAnyModifyAccuracyPriority { get; init; }
    public int? OnAnyFaintPriority { get; init; }
    public int? OnAnyPrepareHitPriority { get; init; }
    public int? OnAnySwitchInPriority { get; init; }
    public int? OnAnySwitchInSubOrder { get; init; }
    public int? OnAllyBasePowerPriority { get; init; }
    public int? OnAllyModifyAtkPriority { get; init; }
    public int? OnAllyModifySpAPriority { get; init; }
    public int? OnAllyModifySpDPriority { get; init; }
    public int? OnAttractPriority { get; init; }
    public int? OnBasePowerPriority { get; init; }
    public int? OnBeforeMovePriority { get; init; }
    public int? OnBeforeSwitchOutPriority { get; init; }
    public int? OnChangeBoostPriority { get; init; }
    public int? OnDamagePriority { get; init; }
    public int? OnDragOutPriority { get; init; }
    public int? OnEffectivenessPriority { get; init; }
    public int? OnFoeBasePowerPriority { get; init; }
    public int? OnFoeBeforeMovePriority { get; init; }
    public int? OnFoeModifyDefPriority { get; init; }
    public int? OnFoeModifySpDPriority { get; init; }
    public int? OnFoeRedirectTargetPriority { get; init; }
    public int? OnFoeTrapPokemonPriority { get; init; }
    public int? OnFractionalPriorityPriority { get; init; }
    public int? OnHitPriority { get; init; }
    public int? OnInvulnerabilityPriority { get; init; }
    public int? OnModifyAccuracyPriority { get; init; }
    public int? OnModifyAtkPriority { get; init; }
    public int? OnModifyCritRatioPriority { get; init; }
    public int? OnModifyDefPriority { get; init; }
    public int? OnModifyMovePriority { get; init; }
    public int? OnModifyPriorityPriority { get; init; }
    public int? OnModifySpAPriority { get; init; }
    public int? OnModifySpDPriority { get; init; }
    public int? OnModifySpePriority { get; init; }
    public int? OnModifyStabPriority { get; init; }
    public int? OnModifyTypePriority { get; init; }
    public int? OnModifyWeightPriority { get; init; }
    public int? OnRedirectTargetPriority { get; init; }
    public int? OnResidualOrder { get; init; }
    public int? OnResidualPriority { get; init; }
    public int? OnResidualSubOrder { get; init; }
    public int? OnSourceBasePowerPriority { get; init; }
    public int? OnSourceInvulnerabilityPriority { get; init; }
    public int? OnSourceModifyAccuracyPriority { get; init; }
    public int? OnSourceModifyAtkPriority { get; init; }
    public int? OnSourceModifyDamagePriority { get; init; }
    public int? OnSourceModifySpAPriority { get; init; }
    public int? OnSwitchInPriority { get; init; }
    public int? OnSwitchInSubOrder { get; init; }
    public int? OnTrapPokemonPriority { get; init; }
    public int? OnTryBoostPriority { get; init; }
    public int? OnTryEatItemPriority { get; init; }
    public int? OnTryHealPriority { get; init; }
    public int? OnTryHitPriority { get; init; }
    public int? OnTryMovePriority { get; init; }
    public int? OnTryPrimaryHitPriority { get; init; }
    public int? OnTypePriority { get; init; }
    public Action<IBattle, int, Pokemon, Pokemon, ActiveMove>? OnAllyDamagingHit { get; init; }
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon>? OnAllyAfterEachBoost { get; init; }
    public VoidSourceMoveHandler? OnAllyAfterHit { get; init; }
    public Action<IBattle, Condition, Pokemon, Pokemon, IEffect>? OnAllyAfterSetStatus { get; init; }
    public OnAfterSubDamageHandler? OnAllyAfterSubDamage { get; init; }
    public Action<IBattle, Pokemon>? OnAllyAfterSwitchInSelf { get; init; }
    public Action<IBattle, Item, Pokemon>? OnAllyAfterUseItem { get; init; }
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyAfterBoost { get; init; }
    public Action<IBattle, int, Pokemon, Pokemon, IEffect>? OnAllyAfterFaint { get; init; }
    public VoidSourceMoveHandler? OnAllyAfterMoveSecondarySelf { get; init; }
    public VoidMoveHandler? OnAllyAfterMoveSecondary { get; init; }
    public VoidSourceMoveHandler? OnAllyAfterMove { get; init; }
    public VoidSourceMoveHandler? OnAllyAfterMoveSelf { get; init; }
    public Action<IBattle, Pokemon, Pokemon>? OnAllyAttract { get; init; }
    public Func<IBattle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnAllyAccuracy { get; init; }
    public ModifierSourceMoveHandler? OnAllyBasePower { get; init; }
    public Action<IBattle, Pokemon, IEffect>? OnAllyBeforeFaint { get; init; }
    public VoidSourceMoveHandler? OnAllyBeforeMove { get; init; }
    public Action<IBattle, Pokemon>? OnAllyBeforeSwitchIn { get; init; }
    public Action<IBattle, Pokemon>? OnAllyBeforeSwitchOut { get; init; }
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyTryBoost { get; init; }
    public VoidSourceMoveHandler? OnAllyChargeMove { get; init; }
    public OnCriticalHit? OnAllyCriticalHit { get; init; }
    public Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>? OnAllyDamage { get; init; }
    public Func<IBattle, Pokemon, Pokemon, IntVoidUnion>? OnAllyDeductPp { get; init; }
    public Action<IBattle, Pokemon>? OnAllyDisableMove { get; init; }
    public Action<IBattle, Pokemon, Pokemon?, ActiveMove?>? OnAllyDragOut { get; init; }
    public Action<IBattle, Item, Pokemon>? OnAllyEatItem { get; init; }
    public OnEffectivenessHandler? OnAllyEffectiveness { get; init; }
    public VoidEffectHandler? OnAllyFaint { get; init; }
    public OnFlinch? OnAllyFlinch { get; init; }
    public ResultMoveHandler? OnAllyHit { get; init; }
    public Action<IBattle, PokemonType, Pokemon>? OnAllyImmunity { get; init; }
    public OnLockMove? OnAllyLockMove { get; init; }
    public Action<IBattle, Pokemon>? OnAllyMaybeTrapPokemon { get; init; }
    public ModifierMoveHandler? OnAllyModifyAccuracy { get; init; }
    public ModifierSourceMoveHandler? OnAllyModifyAtk { get; init; }
    public Func<IBattle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>? OnAllyModifyBoost { get; init; }
    public ModifierSourceMoveHandler? OnAllyModifyCritRatio { get; init; }
    public ModifierSourceMoveHandler? OnAllyModifyDamage { get; init; }
    public ModifierMoveHandler? OnAllyModifyDef { get; init; }
    public OnModifyMoveHandler? OnAllyModifyMove { get; init; }
    public ModifierSourceMoveHandler? OnAllyModifyPriority { get; init; }
    public Action<IBattle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAllyModifySecondaries { get; init; }
    public ModifierSourceMoveHandler? OnAllyModifySpA { get; init; }
    public ModifierMoveHandler? OnAllyModifySpD { get; init; }
    public Func<IBattle, int, Pokemon, IntVoidUnion>? OnAllyModifySpe { get; init; }
    public ModifierSourceMoveHandler? OnAllyModifyStab { get; init; }
    public OnModifyTypeHandler? OnAllyModifyType { get; init; }
    public OnModifyTargetHandler? OnAllyModifyTarget { get; init; }
    public Func<IBattle, int, Pokemon, IntVoidUnion>? OnAllyModifyWeight { get; init; }
    public VoidMoveHandler? OnAllyMoveAborted { get; init; }
    public OnNegateImmunity? OnAllyNegateImmunity { get; init; }
    public Func<IBattle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>? OnAllyOverrideAction { get; init; }
    public ResultSourceMoveHandler? OnAllyPrepareHit { get; init; }
    public Func<IBattle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>? OnAllyRedirectTarget { get; init; }
    public Action<IBattle, PokemonSideUnion, Pokemon, IEffect>? OnAllyResidual { get; init; }
    public Func<IBattle, Ability, Pokemon, Pokemon, IEffect, BoolVoidUnion>? OnAllySetAbility { get; init; }
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, PokemonVoidUnion?>? OnAllySetStatus { get; init; }
    public Func<IBattle, Pokemon, Pokemon, Condition, PokemonVoidUnion>? OnAllySetWeather { get; init; }
    public Func<IBattle, Pokemon, PokemonVoidUnion>? OnAllyStallMove { get; init; }
    public Action<IBattle, Pokemon>? OnAllySwitchOut { get; init; }
    public OnTakeItem? OnAllyTakeItem { get; init; }
    public Action<IBattle, Pokemon>? OnAllyTerrain { get; init; }
    public Action<IBattle, Pokemon>? OnAllyTrapPokemon { get; init; }
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnAllyTryAddVolatile { get; init; }
    public Func<IBattle, Item, Pokemon, BoolVoidUnion>? OnAllyTryEatItem { get; init; }
    public OnTryHeal? OnAllyTryHeal { get; init; }
    public ExtResultSourceMoveHandler? OnAllyTryHit { get; init; }
    public ExtResultSourceMoveHandler? OnAllyTryHitField { get; init; }
    public ResultMoveHandler? OnAllyTryHitSide { get; init; }
    public ExtResultMoveHandler? OnAllyInvulnerability { get; init; }
    public ResultSourceMoveHandler? OnAllyTryMove { get; init; }
    public Func<IBattle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnAllyTryPrimaryHit { get; init; }
    public Func<IBattle, PokemonType[], Pokemon, TypesVoidUnion>? OnAllyType { get; init; }
    public ModifierSourceMoveHandler? OnAllyWeatherModifyDamage { get; init; }
    public ModifierSourceMoveHandler? OnAllyModifyDamagePhase1 { get; init; }
    public ModifierSourceMoveHandler? OnAllyModifyDamagePhase2 { get; init; }

    #endregion

    public EffectDelegate? GetDelegate(EventId id)
    {
        return id switch
        {
            EventId.Start => EffectDelegate.FromNullableDelegate(OnStart),
            EventId.End => EffectDelegate.FromNullableDelegate(OnEnd),
            EventId.DamagingHit => EffectDelegate.FromNullableDelegate(OnDamagingHit),
            EventId.EmergencyExit => EffectDelegate.FromNullableDelegate(OnEmergencyExit),
            EventId.AfterEachBoost => EffectDelegate.FromNullableDelegate(OnAfterEachBoost),
            EventId.AfterHit => EffectDelegate.FromNullableDelegate(OnAfterHit),
            EventId.AfterMega => EffectDelegate.FromNullableDelegate(OnAfterMega),
            EventId.AfterSetStatus => EffectDelegate.FromNullableDelegate(OnAfterSetStatus),
            EventId.AfterSubDamage => EffectDelegate.FromNullableDelegate(OnAfterSubDamage),
            EventId.AfterSwitchInSelf => EffectDelegate.FromNullableDelegate(OnAfterSwitchInSelf),
            EventId.AfterTerastallization => EffectDelegate.FromNullableDelegate(OnAfterTerastallization),
            EventId.AfterUseItem => EffectDelegate.FromNullableDelegate(OnAfterUseItem),
            EventId.AfterTakeItem => EffectDelegate.FromNullableDelegate(OnAfterTakeItem),
            EventId.AfterBoost => EffectDelegate.FromNullableDelegate(OnAfterBoost),
            EventId.AfterFaint => EffectDelegate.FromNullableDelegate(OnAfterFaint),
            EventId.AfterMoveSecondarySelf => EffectDelegate.FromNullableDelegate(OnAfterMoveSecondarySelf),
            EventId.AfterMoveSecondary => EffectDelegate.FromNullableDelegate(OnAfterMoveSecondary),
            EventId.AfterMove => EffectDelegate.FromNullableDelegate(OnAfterMove),
            EventId.AfterMoveSelf => EffectDelegate.FromNullableDelegate(OnAfterMoveSelf),
            EventId.Attract => EffectDelegate.FromNullableDelegate(OnAttract),
            EventId.Accuracy => EffectDelegate.FromNullableDelegate(OnAccuracy),
            EventId.BasePower => EffectDelegate.FromNullableDelegate(OnBasePower),
            EventId.BeforeFaint => EffectDelegate.FromNullableDelegate(OnBeforeFaint),
            EventId.BeforeMove => EffectDelegate.FromNullableDelegate(OnBeforeMove),
            EventId.BeforeSwitchIn => EffectDelegate.FromNullableDelegate(OnBeforeSwitchIn),
            EventId.BeforeSwitchOut => EffectDelegate.FromNullableDelegate(OnBeforeSwitchOut),
            EventId.BeforeTurn => EffectDelegate.FromNullableDelegate(OnBeforeTurn),
            EventId.ChangeBoost => EffectDelegate.FromNullableDelegate(OnChangeBoost),
            EventId.TryBoost => EffectDelegate.FromNullableDelegate(OnTryBoost),
            EventId.ChargeMove => EffectDelegate.FromNullableDelegate(OnChargeMove),
            EventId.CriticalHit => EffectDelegate.FromNullableOnCriticalHit(OnCriticalHit),
            EventId.Damage => EffectDelegate.FromNullableDelegate(OnDamage),
            EventId.DeductPp => EffectDelegate.FromNullableDelegate(OnDeductPp),
            EventId.DisableMove => EffectDelegate.FromNullableDelegate(OnDisableMove),
            EventId.DragOut => EffectDelegate.FromNullableDelegate(OnDragOut),
            EventId.EatItem => EffectDelegate.FromNullableDelegate(OnEatItem),
            EventId.Effectiveness => EffectDelegate.FromNullableDelegate(OnEffectiveness),
            EventId.EntryHazard => EffectDelegate.FromNullableDelegate(OnEntryHazard),
            EventId.Faint => EffectDelegate.FromNullableDelegate(OnFaint),
            EventId.Flinch => EffectDelegate.FromNullableOnFlinch(OnFlinch),
            EventId.FractionalPriority => EffectDelegate.FromNullableOnFractionalPriority(OnFractionalPriority),
            EventId.Hit => EffectDelegate.FromNullableDelegate(OnHit),
            EventId.Immunity => EffectDelegate.FromNullableDelegate(OnImmunity),
            EventId.LockMove => EffectDelegate.FromNullableOnLockMove(OnLockMove),
            EventId.MaybeTrapPokemon => EffectDelegate.FromNullableDelegate(OnMaybeTrapPokemon),
            EventId.ModifyAccuracy => EffectDelegate.FromNullableDelegate(OnModifyAccuracy),
            EventId.ModifyAtk => EffectDelegate.FromNullableDelegate(OnModifyAtk),
            EventId.ModifyBoost => EffectDelegate.FromNullableDelegate(OnModifyBoost),
            EventId.ModifyCritRatio => EffectDelegate.FromNullableDelegate(OnModifyCritRatio),
            EventId.ModifyDamage => EffectDelegate.FromNullableDelegate(OnModifyDamage),
            EventId.ModifyDef => EffectDelegate.FromNullableDelegate(OnModifyDef),
            EventId.ModifyMove => EffectDelegate.FromNullableDelegate(OnModifyMove),
            EventId.ModifyPriority => EffectDelegate.FromNullableDelegate(OnModifyPriority),
            EventId.ModifySecondaries => EffectDelegate.FromNullableDelegate(OnModifySecondaries),
            EventId.ModifyType => EffectDelegate.FromNullableDelegate(OnModifyType),
            EventId.ModifyTarget => EffectDelegate.FromNullableDelegate(OnModifyTarget),
            EventId.ModifySpA => EffectDelegate.FromNullableDelegate(OnModifySpA),
            EventId.ModifySpD => EffectDelegate.FromNullableDelegate(OnModifySpD),
            EventId.ModifySpe => EffectDelegate.FromNullableDelegate(OnModifySpe),
            EventId.ModifyStab => EffectDelegate.FromNullableDelegate(OnModifyStab),
            EventId.ModifyWeight => EffectDelegate.FromNullableDelegate(OnModifyWeight),
            EventId.MoveAborted => EffectDelegate.FromNullableDelegate(OnMoveAborted),
            EventId.NegateImmunity => EffectDelegate.FromNullableOnNegateImmunity(OnNegateImmunity),
            EventId.OverrideAction => EffectDelegate.FromNullableDelegate(OnOverrideAction),
            EventId.PrepareHit => EffectDelegate.FromNullableDelegate(OnPrepareHit),
            EventId.PseudoWeatherChange => EffectDelegate.FromNullableDelegate(OnPseudoWeatherChange),
            EventId.RedirectTarget => EffectDelegate.FromNullableDelegate(OnRedirectTarget),
            EventId.Residual => EffectDelegate.FromNullableDelegate(OnResidual),
            EventId.SetAbility => EffectDelegate.FromNullableDelegate(OnSetAbility),
            EventId.SetStatus => EffectDelegate.FromNullableDelegate(OnSetStatus),
            EventId.SetWeather => EffectDelegate.FromNullableDelegate(OnSetWeather),
            EventId.SideConditionStart => EffectDelegate.FromNullableDelegate(OnSideConditionStart),
            EventId.StallMove => EffectDelegate.FromNullableDelegate(OnStallMove),
            EventId.SwitchIn => EffectDelegate.FromNullableDelegate(OnSwitchIn),
            EventId.SwitchOut => EffectDelegate.FromNullableDelegate(OnSwitchOut),
            EventId.Swap => EffectDelegate.FromNullableDelegate(OnSwap),
            EventId.TakeItem => EffectDelegate.FromNullableOnTakeItem(OnTakeItem),
            EventId.TerrainChange => EffectDelegate.FromNullableDelegate(OnTerrainChange),
            EventId.TrapPokemon => EffectDelegate.FromNullableDelegate(OnTrapPokemon),
            EventId.TryAddVolatile => EffectDelegate.FromNullableDelegate(OnTryAddVolatile),
            EventId.TryEatItem => EffectDelegate.FromNullableOnTryEatItem(OnTryEatItem),
            EventId.TryHeal => EffectDelegate.FromNullableOnTryHeal(OnTryHeal),
            EventId.TryHit => EffectDelegate.FromNullableDelegate(OnTryHit),
            EventId.TryHitField => EffectDelegate.FromNullableDelegate(OnTryHitField),
            EventId.TryHitSide => EffectDelegate.FromNullableDelegate(OnTryHitSide),
            EventId.Invulnerability => EffectDelegate.FromNullableDelegate(OnInvulnerability),
            EventId.TryMove => EffectDelegate.FromNullableDelegate(OnTryMove),
            EventId.TryPrimaryHit => EffectDelegate.FromNullableDelegate(OnTryPrimaryHit),
            EventId.Type => EffectDelegate.FromNullableDelegate(OnType),
            EventId.UseItem => EffectDelegate.FromNullableDelegate(OnUseItem),
            EventId.Update => EffectDelegate.FromNullableDelegate(OnUpdate),
            EventId.Weather => EffectDelegate.FromNullableDelegate(OnWeather),
            EventId.WeatherModifyDamage => EffectDelegate.FromNullableDelegate(OnWeatherModifyDamage),
            EventId.ModifyDamagePhase1 => EffectDelegate.FromNullableDelegate(OnModifyDamagePhase1),
            EventId.ModifyDamagePhase2 => EffectDelegate.FromNullableDelegate(OnModifyDamagePhase2),
            _ => null,
        };
    }

    public int? GetPriority(EventId id)
    {
        return id switch
        {
            EventId.Accuracy => OnAccuracyPriority,
            EventId.DamagingHit => OnDamagingHitOrder,
            EventId.AfterMoveSecondary => OnAfterMoveSecondaryPriority,
            EventId.AfterMoveSecondarySelf => OnAfterMoveSecondarySelfPriority,
            EventId.AfterMoveSelf => OnAfterMoveSelfPriority,
            EventId.AfterSetStatus => OnAfterSetStatusPriority,
            EventId.AnyBasePower => OnAnyBasePowerPriority,
            EventId.AnyInvulnerability => OnAnyInvulnerabilityPriority,
            EventId.AnyModifyAccuracy => OnAnyModifyAccuracyPriority,
            EventId.AnyFaint => OnAnyFaintPriority,
            EventId.AnyPrepareHit => OnAnyPrepareHitPriority,
            EventId.AnySwitchIn => OnAnySwitchInPriority,
            EventId.AllyBasePower => OnAllyBasePowerPriority,
            EventId.AllyModifyAtk => OnAllyModifyAtkPriority,
            EventId.AllyModifySpA => OnAllyModifySpAPriority,
            EventId.AllyModifySpD => OnAllyModifySpDPriority,
            EventId.Attract => OnAttractPriority,
            EventId.BasePower => OnBasePowerPriority,
            EventId.BeforeMove => OnBeforeMovePriority,
            EventId.BeforeSwitchOut => OnBeforeSwitchOutPriority,
            EventId.ChangeBoost => OnChangeBoostPriority,
            EventId.Damage => OnDamagePriority,
            EventId.DragOut => OnDragOutPriority,
            EventId.Effectiveness => OnEffectivenessPriority,
            EventId.FoeBasePower => OnFoeBasePowerPriority,
            EventId.FoeBeforeMove => OnFoeBeforeMovePriority,
            EventId.FoeModifyDef => OnFoeModifyDefPriority,
            EventId.FoeModifySpD => OnFoeModifySpDPriority,
            EventId.FoeRedirectTarget => OnFoeRedirectTargetPriority,
            EventId.FoeTrapPokemon => OnFoeTrapPokemonPriority,
            EventId.FractionalPriority => OnFractionalPriorityPriority,
            EventId.Hit => OnHitPriority,
            EventId.Invulnerability => OnInvulnerabilityPriority,
            EventId.ModifyAccuracy => OnModifyAccuracyPriority,
            EventId.ModifyAtk => OnModifyAtkPriority,
            EventId.ModifyCritRatio => OnModifyCritRatioPriority,
            EventId.ModifyDef => OnModifyDefPriority,
            EventId.ModifyMove => OnModifyMovePriority,
            EventId.ModifyPriority => OnModifyPriorityPriority,
            EventId.ModifySpA => OnModifySpAPriority,
            EventId.ModifySpD => OnModifySpDPriority,
            EventId.ModifySpe => OnModifySpePriority,
            EventId.ModifyStab => OnModifyStabPriority,
            EventId.ModifyType => OnModifyTypePriority,
            EventId.ModifyWeight => OnModifyWeightPriority,
            EventId.RedirectTarget => OnRedirectTargetPriority,
            EventId.Residual => OnResidualPriority,
            EventId.SourceBasePower => OnSourceBasePowerPriority,
            EventId.SourceInvulnerability => OnSourceInvulnerabilityPriority,
            EventId.SourceModifyAccuracy => OnSourceModifyAccuracyPriority,
            EventId.SourceModifyAtk => OnSourceModifyAtkPriority,
            EventId.SourceModifyDamage => OnSourceModifyDamagePriority,
            EventId.SourceModifySpA => OnSourceModifySpAPriority,
            EventId.SwitchIn => OnSwitchInPriority,
            EventId.TrapPokemon => OnTrapPokemonPriority,
            EventId.TryBoost => OnTryBoostPriority,
            EventId.TryEatItem => OnTryEatItemPriority,
            EventId.TryHeal => OnTryHealPriority,
            EventId.TryHit => OnTryHitPriority,
            EventId.TryMove => OnTryMovePriority,
            EventId.TryPrimaryHit => OnTryPrimaryHitPriority,
            EventId.Type => OnTypePriority,
            _ => null,
        };
    }

    public IntFalseUnion? GetOrder(EventId id)
    {
        return id switch
        {
            EventId.DamagingHit => OnDamagingHitOrder is not null ?
                IntFalseUnion.FromInt(OnDamagingHitOrder.Value) : null,

            EventId.Residual => OnResidualOrder is not null ?
                IntFalseUnion.FromInt(OnResidualOrder.Value) : null,

            _ => null,
        };
    }

    public int? GetSubOrder(EventId id)
    {
        return id switch
        {
            EventId.AnySwitchIn => OnAnySwitchInSubOrder,
            EventId.Residual => OnResidualSubOrder,
            EventId.SwitchIn => OnSwitchInSubOrder,
            _ => null,
        };
    }
}