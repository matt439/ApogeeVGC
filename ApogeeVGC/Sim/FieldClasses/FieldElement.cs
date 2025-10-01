using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.FieldClasses;

/// <summary>
/// Base class for field elements like Weather, Terrain, and Pseudo-Weather.
/// </summary>
public abstract class FieldElement : ICondition
{
    public required string Name { get; init; }
    public required int Duration
    {
        get;
        set
        {
            if (value < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "BaseDuration must be at least 1.");
            }
            field = value;
        }
    }
    public int RemainingTurns => Duration - ElapsedTurns;
    public bool IsExpired => RemainingTurns <= 0;
    public bool PrintDebug { get; init; }
    protected int ElapsedTurns { get; set; }


    public void IncrementTurnCounter()
    {
        //if (RemainingTurns > 0)
        //{
        //    ElapsedTurns++;
        //}
        //else
        //{
        //    throw new InvalidOperationException("Cannot increment turn counter beyond duration.");
        //}
        ElapsedTurns++;
    }

    public void ResetTurnCounter()
    {
        ElapsedTurns = 0;
    }

    public EffectType EffectType => EffectType.Condition;
    public abstract ConditionEffectType ConditionEffectType { get; }

    #region ICondition Implementation

    public Func<IBattle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; init; }
    public Action<IBattle, Pokemon>? OnCopy { get; init; }
    public Action<IBattle, Pokemon>? OnEnd { get; init; }
    public Func<IBattle, Pokemon, Pokemon, IEffect, bool?>? OnRestart { get; init; }
    public Func<IBattle, Pokemon, Pokemon, IEffect, bool?>? OnStart { get; init; }

    public Action<IBattle, int, Pokemon, Pokemon, Move>? OnDamagingHit { get; init; }
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
    public Func<IBattle, int, Pokemon, Pokemon, Move, IntBoolUnion?>? OnAccuracy { get; init; }
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
    public Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>? OnDamage { get; init; }
    public Func<IBattle, Pokemon, Pokemon, int?>? OnDeductPp { get; init; }
    public Action<IBattle, Pokemon>? OnDisableMove { get; init; }
    public Action<IBattle, Pokemon, Pokemon?, Move?>? OnDragOut { get; init; }
    public Action<IBattle, Item, Pokemon>? OnEatItem { get; init; }
    public OnEffectivenessHandler? OnEffectiveness { get; init; }
    public Action<IBattle, Pokemon>? OnEntryHazard { get; init; }
    public VoidEffectHandler? OnFaint { get; init; }
    public OnFlinch? OnFlinch { get; init; }
    public OnFractionalPriority? OnFractionalPriority { get; init; }
    public ResultMoveHandler? OnHit { get; init; }
    public Action<IBattle, PokemonType, Pokemon>? OnImmunity { get; init; }
    public Func<IBattle, Pokemon, Move?>? OnLockMove { get; init; }
    public Action<IBattle, Pokemon>? OnMaybeTrapPokemon { get; init; }
    public ModifierMoveHandler? OnModifyAccuracy { get; init; }
    public ModifierSourceMoveHandler? OnModifyAtk { get; init; }
    public Func<IBattle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnModifyBoost { get; init; }
    public ModifierSourceMoveHandler? OnModifyCritRatio { get; init; }
    public ModifierSourceMoveHandler? OnModifyDamage { get; init; }
    public ModifierMoveHandler? OnModifyDef { get; init; }
    public OnModifyMoveHandler? OnModifyMove { get; init; }
    public ModifierSourceMoveHandler? OnModifyPriority { get; init; }
    public Action<IBattle, List<SecondaryEffect>, Pokemon, Pokemon, Move>? OnModifySecondaries { get; init; }
    public OnModifyTypeHandler? OnModifyType { get; init; }
    public OnModifyTargetHandler? OnModifyTarget { get; init; }
    public ModifierSourceMoveHandler? OnModifySpA { get; init; }
    public ModifierMoveHandler? OnModifySpD { get; init; }
    public Func<IBattle, int, Pokemon, int?>? OnModifySpe { get; init; }
    public ModifierSourceMoveHandler? OnModifyStab { get; init; }
    public Func<IBattle, int, Pokemon, int?>? OnModifyWeight { get; init; }
    public VoidMoveHandler? OnMoveAborted { get; init; }
    public OnNegateImmunity? OnNegateImmunity { get; init; }
    public Func<IBattle, Pokemon, Pokemon, Move, Delegate?>? OnOverrideAction { get; init; }
    public ResultSourceMoveHandler? OnPrepareHit { get; init; }
    public Action<IBattle, Pokemon, Pokemon, Condition>? OnPseudoWeatherChange { get; init; }
    public Func<IBattle, Pokemon, Pokemon, IEffect, Move, Pokemon?>? OnRedirectTarget { get; init; }
    public Action<IBattle, Pokemon, Pokemon, IEffect>? OnResidual { get; init; }
    public Action<IBattle, Ability, Pokemon, Pokemon, IEffect>? OnSetAbility { get; init; }
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSetStatus { get; init; }
    public Func<IBattle, Pokemon, Pokemon, Condition, bool?>? OnSetWeather { get; init; }
    public Action<IBattle, Side, Pokemon, Condition>? OnSideConditionStart { get; init; }
    public Func<IBattle, Pokemon, bool?>? OnStallMove { get; init; }
    public Action<IBattle, Pokemon>? OnSwitchIn { get; init; }
    public Action<IBattle, Pokemon>? OnSwitchOut { get; init; }
    public Action<IBattle, Pokemon, Pokemon>? OnSwap { get; init; }
    public OnTakeItem? OnTakeItem { get; init; }
    public Action<IBattle, Pokemon, Pokemon, IEffect>? OnWeatherChange { get; init; }
    public Action<IBattle, Pokemon, Pokemon, IEffect>? OnTerrainChange { get; init; }
    public Action<IBattle, Pokemon>? OnTrapPokemon { get; init; }
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnTryAddVolatile { get; init; }
    public Func<IBattle, Item, Pokemon, bool?>? OnTryEatItem { get; init; }
    public Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>? OnTryHeal { get; init; }
    public ExtResultSourceMoveHandler? OnTryHit { get; init; }
    public ResultMoveHandler? OnTryHitField { get; init; }
    public ResultMoveHandler? OnTryHitSide { get; init; }
    public ExtResultMoveHandler? OnInvulnerability { get; init; }
    public ResultSourceMoveHandler? OnTryMove { get; init; }
    public Func<IBattle, Pokemon, Pokemon, Move, IntBoolUnion?>? OnTryPrimaryHit { get; init; }
    public Func<IBattle, PokemonType[], Pokemon, List<PokemonType>?>? OnType { get; init; }
    public Action<IBattle, Item, Pokemon>? OnUseItem { get; init; }
    public Action<IBattle, Pokemon>? OnUpdate { get; init; }
    public Action<IBattle, Pokemon, object?, Condition>? OnWeather { get; init; }
    public ModifierSourceMoveHandler? OnWeatherModifyDamage { get; init; }
    public ModifierSourceMoveHandler? OnModifyDamagePhase1 { get; init; }
    public ModifierSourceMoveHandler? OnModifyDamagePhase2 { get; init; }
    public Action<IBattle, int, Pokemon, Pokemon, Move>? OnFoeDamagingHit { get; init; }
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
    public Func<IBattle, int, Pokemon, Pokemon, Move, IntBoolUnion?>? OnFoeAccuracy { get; init; }
    public ModifierSourceMoveHandler? OnFoeBasePower { get; init; }
    public Action<IBattle, Pokemon, IEffect>? OnFoeBeforeFaint { get; init; }
    public VoidSourceMoveHandler? OnFoeBeforeMove { get; init; }
    public Action<IBattle, Pokemon>? OnFoeBeforeSwitchIn { get; init; }
    public Action<IBattle, Pokemon>? OnFoeBeforeSwitchOut { get; init; }
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeTryBoost { get; init; }
    public VoidSourceMoveHandler? OnFoeChargeMove { get; init; }
    public OnCriticalHit? OnFoeCriticalHit { get; init; }
    public Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>? OnFoeDamage { get; init; }
    public Func<IBattle, Pokemon, Pokemon, int?>? OnFoeDeductPp { get; init; }
    public Action<IBattle, Pokemon>? OnFoeDisableMove { get; init; }
    public Action<IBattle, Pokemon, Pokemon?, Move?>? OnFoeDragOut { get; init; }
    public Action<IBattle, Item, Pokemon>? OnFoeEatItem { get; init; }
    public OnEffectivenessHandler? OnFoeEffectiveness { get; init; }
    public VoidEffectHandler? OnFoeFaint { get; init; }
    public OnFlinch? OnFoeFlinch { get; init; }
    public ResultMoveHandler? OnFoeHit { get; init; }
    public Action<IBattle, PokemonType, Pokemon>? OnFoeImmunity { get; init; }
    public Func<IBattle, Pokemon, Move?>? OnFoeLockMove { get; init; }
    public Action<IBattle, Pokemon, Pokemon?>? OnFoeMaybeTrapPokemon { get; init; }
    public ModifierMoveHandler? OnFoeModifyAccuracy { get; init; }
    public ModifierSourceMoveHandler? OnFoeModifyAtk { get; init; }
    public Func<IBattle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnFoeModifyBoost { get; init; }
    public ModifierSourceMoveHandler? OnFoeModifyCritRatio { get; init; }
    public ModifierSourceMoveHandler? OnFoeModifyDamage { get; init; }
    public ModifierMoveHandler? OnFoeModifyDef { get; init; }
    public OnModifyMoveHandler? OnFoeModifyMove { get; init; }
    public ModifierSourceMoveHandler? OnFoeModifyPriority { get; init; }
    public Action<IBattle, List<SecondaryEffect>, Pokemon, Pokemon, Move>? OnFoeModifySecondaries { get; init; }
    public ModifierSourceMoveHandler? OnFoeModifySpA { get; init; }
    public ModifierMoveHandler? OnFoeModifySpD { get; init; }
    public Func<IBattle, int, Pokemon, int?>? OnFoeModifySpe { get; init; }
    public ModifierSourceMoveHandler? OnFoeModifyStab { get; init; }
    public OnModifyTypeHandler? OnFoeModifyType { get; init; }
    public OnModifyTargetHandler? OnFoeModifyTarget { get; init; }
    public Func<IBattle, int, Pokemon, int?>? OnFoeModifyWeight { get; init; }
    public VoidMoveHandler? OnFoeMoveAborted { get; init; }
    public OnNegateImmunity? OnFoeNegateImmunity { get; init; }
    public Func<IBattle, Pokemon, Pokemon, Move, Delegate?>? OnFoeOverrideAction { get; init; }
    public ResultSourceMoveHandler? OnFoePrepareHit { get; init; }
    public Func<IBattle, Pokemon, Pokemon, IEffect, Move, Pokemon?>? OnFoeRedirectTarget { get; init; }
    public Action<IBattle, PokemonSideUnion, Pokemon, IEffect>? OnFoeResidual { get; init; }
    public Func<IBattle, Ability, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetAbility { get; init; }
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetStatus { get; init; }
    public Func<IBattle, Pokemon, Pokemon, Condition, bool?>? OnFoeSetWeather { get; init; }
    public Func<IBattle, Pokemon, bool?>? OnFoeStallMove { get; init; }
    public Action<IBattle, Pokemon>? OnFoeSwitchOut { get; init; }
    public OnTakeItem? OnFoeTakeItem { get; init; }
    public Action<IBattle, Pokemon>? OnFoeTerrain { get; init; }
    public Action<IBattle, Pokemon>? OnFoeTrapPokemon { get; init; }
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeTryAddVolatile { get; init; }
    public Func<IBattle, Item, Pokemon, bool?>? OnFoeTryEatItem { get; init; }
    public OnTryHeal? OnFoeTryHeal { get; init; }
    public ExtResultSourceMoveHandler? OnFoeTryHit { get; init; }
    public ResultMoveHandler? OnFoeTryHitField { get; init; }
    public ResultMoveHandler? OnFoeTryHitSide { get; init; }
    public ExtResultMoveHandler? OnFoeInvulnerability { get; init; }
    public ResultSourceMoveHandler? OnFoeTryMove { get; init; }
    public Func<IBattle, Pokemon, Pokemon, Move, IntBoolUnion?>? OnFoeTryPrimaryHit { get; init; }
    public Func<IBattle, PokemonType[], Pokemon, PokemonType[]?>? OnFoeType { get; init; }
    public ModifierSourceMoveHandler? OnFoeWeatherModifyDamage { get; init; }
    public ModifierSourceMoveHandler? OnFoeModifyDamagePhase1 { get; init; }
    public ModifierSourceMoveHandler? OnFoeModifyDamagePhase2 { get; init; }
    public Action<IBattle, int, Pokemon, Pokemon, Move>? OnSourceDamagingHit { get; init; }
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
    public Func<IBattle, int, Pokemon, Pokemon, Move, IntBoolUnion?>? OnSourceAccuracy { get; init; }
    public ModifierSourceMoveHandler? OnSourceBasePower { get; init; }
    public Action<IBattle, Pokemon, IEffect>? OnSourceBeforeFaint { get; init; }
    public VoidSourceMoveHandler? OnSourceBeforeMove { get; init; }
    public Action<IBattle, Pokemon>? OnSourceBeforeSwitchIn { get; init; }
    public Action<IBattle, Pokemon>? OnSourceBeforeSwitchOut { get; init; }
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceTryBoost { get; init; }
    public VoidSourceMoveHandler? OnSourceChargeMove { get; init; }
    public OnCriticalHit? OnSourceCriticalHit { get; init; }
    public Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>? OnSourceDamage { get; init; }
    public Func<IBattle, Pokemon, Pokemon, int?>? OnSourceDeductPp { get; init; }
    public Action<IBattle, Pokemon>? OnSourceDisableMove { get; init; }
    public Action<IBattle, Pokemon, Pokemon?, Move?>? OnSourceDragOut { get; init; }
    public Action<IBattle, Item, Pokemon>? OnSourceEatItem { get; init; }
    public OnEffectivenessHandler? OnSourceEffectiveness { get; init; }
    public VoidEffectHandler? OnSourceFaint { get; init; }
    public OnFlinch? OnSourceFlinch { get; init; }
    public ResultMoveHandler? OnSourceHit { get; init; }
    public Action<IBattle, PokemonType, Pokemon>? OnSourceImmunity { get; init; }
    public Func<IBattle, Pokemon, Move?>? OnSourceLockMove { get; init; }
    public Action<IBattle, Pokemon>? OnSourceMaybeTrapPokemon { get; init; }
    public ModifierMoveHandler? OnSourceModifyAccuracy { get; init; }
    public ModifierSourceMoveHandler? OnSourceModifyAtk { get; init; }
    public Func<IBattle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnSourceModifyBoost { get; init; }
    public ModifierSourceMoveHandler? OnSourceModifyCritRatio { get; init; }
    public ModifierSourceMoveHandler? OnSourceModifyDamage { get; init; }
    public ModifierMoveHandler? OnSourceModifyDef { get; init; }
    public OnModifyMoveHandler? OnSourceModifyMove { get; init; }
    public ModifierSourceMoveHandler? OnSourceModifyPriority { get; init; }
    public Action<IBattle, List<SecondaryEffect>, Pokemon, Pokemon, Move>? OnSourceModifySecondaries { get; init; }
    public ModifierSourceMoveHandler? OnSourceModifySpA { get; init; }
    public ModifierMoveHandler? OnSourceModifySpD { get; init; }
    public Func<IBattle, int, Pokemon, int?>? OnSourceModifySpe { get; init; }
    public ModifierSourceMoveHandler? OnSourceModifyStab { get; init; }
    public OnModifyTypeHandler? OnSourceModifyType { get; init; }
    public OnModifyTargetHandler? OnSourceModifyTarget { get; init; }
    public Func<IBattle, int, Pokemon, int?>? OnSourceModifyWeight { get; init; }
    public VoidMoveHandler? OnSourceMoveAborted { get; init; }
    public OnNegateImmunity? OnSourceNegateImmunity { get; init; }
    public Func<IBattle, Pokemon, Pokemon, Move, Delegate?>? OnSourceOverrideAction { get; init; }
    public ResultSourceMoveHandler? OnSourcePrepareHit { get; init; }
    public Func<IBattle, Pokemon, Pokemon, IEffect, Move, Pokemon?>? OnSourceRedirectTarget { get; init; }
    public Action<IBattle, PokemonSideUnion, Pokemon, IEffect>? OnSourceResidual { get; init; }
    public Func<IBattle, Ability, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetAbility { get; init; }
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetStatus { get; init; }
    public Func<IBattle, Pokemon, Pokemon, Condition, bool?>? OnSourceSetWeather { get; init; }
    public Func<IBattle, Pokemon, bool?>? OnSourceStallMove { get; init; }
    public Action<IBattle, Pokemon>? OnSourceSwitchOut { get; init; }
    public OnTakeItem? OnSourceTakeItem { get; init; }
    public Action<IBattle, Pokemon>? OnSourceTerrain { get; init; }
    public Action<IBattle, Pokemon>? OnSourceTrapPokemon { get; init; }
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceTryAddVolatile { get; init; }
    public Func<IBattle, Item, Pokemon, bool?>? OnSourceTryEatItem { get; init; }
    public OnTryHeal? OnSourceTryHeal { get; init; }
    public ExtResultSourceMoveHandler? OnSourceTryHit { get; init; }
    public ResultMoveHandler? OnSourceTryHitField { get; init; }
    public ResultMoveHandler? OnSourceTryHitSide { get; init; }
    public ExtResultMoveHandler? OnSourceInvulnerability { get; init; }
    public ResultSourceMoveHandler? OnSourceTryMove { get; init; }
    public Func<IBattle, Pokemon, Pokemon, Move, IntBoolUnion?>? OnSourceTryPrimaryHit { get; init; }
    public Func<IBattle, PokemonType[], Pokemon, PokemonType[]?>? OnSourceType { get; init; }
    public ModifierSourceMoveHandler? OnSourceWeatherModifyDamage { get; init; }
    public ModifierSourceMoveHandler? OnSourceModifyDamagePhase1 { get; init; }
    public ModifierSourceMoveHandler? OnSourceModifyDamagePhase2 { get; init; }
    public Action<IBattle, int, Pokemon, Pokemon, Move>? OnAnyDamagingHit { get; init; }
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
    public Func<IBattle, int, Pokemon, Pokemon, Move, IntBoolUnion?>? OnAnyAccuracy { get; init; }
    public ModifierSourceMoveHandler? OnAnyBasePower { get; init; }
    public Action<IBattle, Pokemon, IEffect>? OnAnyBeforeFaint { get; init; }
    public VoidSourceMoveHandler? OnAnyBeforeMove { get; init; }
    public Action<IBattle, Pokemon>? OnAnyBeforeSwitchIn { get; init; }
    public Action<IBattle, Pokemon>? OnAnyBeforeSwitchOut { get; init; }
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyTryBoost { get; init; }
    public VoidSourceMoveHandler? OnAnyChargeMove { get; init; }
    public OnCriticalHit? OnAnyCriticalHit { get; init; }
    public Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>? OnAnyDamage { get; init; }
    public Func<IBattle, Pokemon, Pokemon, int?>? OnAnyDeductPp { get; init; }
    public Action<IBattle, Pokemon>? OnAnyDisableMove { get; init; }
    public Action<IBattle, Pokemon, Pokemon?, Move?>? OnAnyDragOut { get; init; }
    public Action<IBattle, Item, Pokemon>? OnAnyEatItem { get; init; }
    public OnEffectivenessHandler? OnAnyEffectiveness { get; init; }
    public VoidEffectHandler? OnAnyFaint { get; init; }
    public OnFlinch? OnAnyFlinch { get; init; }
    public ResultMoveHandler? OnAnyHit { get; init; }
    public Action<IBattle, PokemonType, Pokemon>? OnAnyImmunity { get; init; }
    public Func<IBattle, Pokemon, Move?>? OnAnyLockMove { get; init; }
    public Action<IBattle, Pokemon, Pokemon?>? OnAnyMaybeTrapPokemon { get; init; }
    public ModifierMoveHandler? OnAnyModifyAccuracy { get; init; }
    public ModifierSourceMoveHandler? OnAnyModifyAtk { get; init; }
    public Func<IBattle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAnyModifyBoost { get; init; }
    public ModifierSourceMoveHandler? OnAnyModifyCritRatio { get; init; }
    public ModifierSourceMoveHandler? OnAnyModifyDamage { get; init; }
    public ModifierMoveHandler? OnAnyModifyDef { get; init; }
    public OnModifyMoveHandler? OnAnyModifyMove { get; init; }
    public ModifierSourceMoveHandler? OnAnyModifyPriority { get; init; }
    public Action<IBattle, List<SecondaryEffect>, Pokemon, Pokemon, Move>? OnAnyModifySecondaries { get; init; }
    public ModifierSourceMoveHandler? OnAnyModifySpA { get; init; }
    public ModifierMoveHandler? OnAnyModifySpD { get; init; }
    public Func<IBattle, int, Pokemon, int?>? OnAnyModifySpe { get; init; }
    public ModifierSourceMoveHandler? OnAnyModifyStab { get; init; }
    public OnModifyTypeHandler? OnAnyModifyType { get; init; }
    public OnModifyTargetHandler? OnAnyModifyTarget { get; init; }
    public Func<IBattle, int, Pokemon, int?>? OnAnyModifyWeight { get; init; }
    public Action<IBattle, Pokemon, Pokemon, Move>? OnAnyMoveAborted { get; init; }
    public OnNegateImmunity? OnAnyNegateImmunity { get; init; }
    public Func<IBattle, Pokemon, Pokemon, Move, Delegate?>? OnAnyOverrideAction { get; init; }
    public ResultSourceMoveHandler? OnAnyPrepareHit { get; init; }
    public Action<IBattle, Pokemon, Pokemon, Condition>? OnAnyPseudoWeatherChange { get; init; }
    public Func<IBattle, Pokemon, Pokemon, IEffect, Move, Pokemon?>? OnAnyRedirectTarget { get; init; }
    public Action<IBattle, PokemonSideUnion, Pokemon, IEffect>? OnAnyResidual { get; init; }
    public Func<IBattle, Ability, Pokemon, Pokemon, IEffect, bool?>? OnAnySetAbility { get; init; }
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnySetStatus { get; init; }
    public Func<IBattle, Pokemon, Pokemon, Condition, bool?>? OnAnySetWeather { get; init; }
    public Func<IBattle, Pokemon, bool?>? OnAnyStallMove { get; init; }
    public Action<IBattle, Pokemon>? OnAnySwitchIn { get; init; }
    public Action<IBattle, Pokemon>? OnAnySwitchOut { get; init; }
    public OnTakeItem? OnAnyTakeItem { get; init; }
    public Action<IBattle, Pokemon>? OnAnyTerrain { get; init; }
    public Action<IBattle, Pokemon>? OnAnyTrapPokemon { get; init; }
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnyTryAddVolatile { get; init; }
    public Func<IBattle, Item, Pokemon, bool?>? OnAnyTryEatItem { get; init; }
    public OnTryHeal? OnAnyTryHeal { get; init; }
    public ExtResultSourceMoveHandler? OnAnyTryHit { get; init; }
    public ResultMoveHandler? OnAnyTryHitField { get; init; }
    public ResultMoveHandler? OnAnyTryHitSide { get; init; }
    public ExtResultMoveHandler? OnAnyInvulnerability { get; init; }
    public ResultSourceMoveHandler? OnAnyTryMove { get; init; }
    public Func<IBattle, Pokemon, Pokemon, Move, IntBoolUnion?>? OnAnyTryPrimaryHit { get; init; }
    public Func<IBattle, PokemonType[], Pokemon, PokemonType[]?>? OnAnyType { get; init; }
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
    public Action<IBattle, Side, Pokemon, IEffect>? OnSideStart { get; init; }
    public Action<IBattle, Side, Pokemon, IEffect>? OnSideRestart { get; init; }
    public Action<IBattle, Side, Pokemon, IEffect>? OnSideResidual { get; init; }
    public Action<IBattle, Side>? OnSideEnd { get; init; }
    public int? OnSideResidualOrder { get; init; }
    public int? OnSideResidualPriority { get; init; }
    public int? OnSideResidualSubOrder { get; init; }
    public Action<IBattle, Field, Pokemon, IEffect>? OnFieldStart { get; init; }
    public Action<IBattle, Field, Pokemon, IEffect>? OnFieldRestart { get; init; }
    public Action<IBattle, Field, Pokemon, IEffect>? OnFieldResidual { get; init; }
    public Action<IBattle, Field>? OnFieldEnd { get; init; }
    public int? OnFieldResidualOrder { get; init; }
    public int? OnFieldResidualPriority { get; init; }
    public int? OnFieldResidualSubOrder { get; init; }
    public Action<IBattle, int, Pokemon, Pokemon, Move>? OnAllyDamagingHit { get; init; }
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
    public Func<IBattle, int, Pokemon, Pokemon, Move, IntBoolUnion?>? OnAllyAccuracy { get; init; }
    public ModifierSourceMoveHandler? OnAllyBasePower { get; init; }
    public Action<IBattle, Pokemon, IEffect>? OnAllyBeforeFaint { get; init; }
    public VoidSourceMoveHandler? OnAllyBeforeMove { get; init; }
    public Action<IBattle, Pokemon>? OnAllyBeforeSwitchIn { get; init; }
    public Action<IBattle, Pokemon>? OnAllyBeforeSwitchOut { get; init; }
    public Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyTryBoost { get; init; }
    public VoidSourceMoveHandler? OnAllyChargeMove { get; init; }
    public OnCriticalHit? OnAllyCriticalHit { get; init; }
    public Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>? OnAllyDamage { get; init; }
    public Func<IBattle, Pokemon, Pokemon, int?>? OnAllyDeductPp { get; init; }
    public Action<IBattle, Pokemon>? OnAllyDisableMove { get; init; }
    public Action<IBattle, Pokemon, Pokemon?, Move?>? OnAllyDragOut { get; init; }
    public Action<IBattle, Item, Pokemon>? OnAllyEatItem { get; init; }
    public OnEffectivenessHandler? OnAllyEffectiveness { get; init; }
    public VoidEffectHandler? OnAllyFaint { get; init; }
    public OnFlinch? OnAllyFlinch { get; init; }
    public ResultMoveHandler? OnAllyHit { get; init; }
    public Action<IBattle, PokemonType, Pokemon>? OnAllyImmunity { get; init; }
    public Func<IBattle, Pokemon, Move?>? OnAllyLockMove { get; init; }
    public Action<IBattle, Pokemon>? OnAllyMaybeTrapPokemon { get; init; }
    public ModifierMoveHandler? OnAllyModifyAccuracy { get; init; }
    public ModifierSourceMoveHandler? OnAllyModifyAtk { get; init; }
    public Func<IBattle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAllyModifyBoost { get; init; }
    public ModifierSourceMoveHandler? OnAllyModifyCritRatio { get; init; }
    public ModifierSourceMoveHandler? OnAllyModifyDamage { get; init; }
    public ModifierMoveHandler? OnAllyModifyDef { get; init; }
    public OnModifyMoveHandler? OnAllyModifyMove { get; init; }
    public ModifierSourceMoveHandler? OnAllyModifyPriority { get; init; }
    public Action<IBattle, List<SecondaryEffect>, Pokemon, Pokemon, Move>? OnAllyModifySecondaries { get; init; }
    public ModifierSourceMoveHandler? OnAllyModifySpA { get; init; }
    public ModifierMoveHandler? OnAllyModifySpD { get; init; }
    public Func<IBattle, int, Pokemon, int?>? OnAllyModifySpe { get; init; }
    public ModifierSourceMoveHandler? OnAllyModifyStab { get; init; }
    public OnModifyTypeHandler? OnAllyModifyType { get; init; }
    public OnModifyTargetHandler? OnAllyModifyTarget { get; init; }
    public Func<IBattle, int, Pokemon, int?>? OnAllyModifyWeight { get; init; }
    public VoidMoveHandler? OnAllyMoveAborted { get; init; }
    public OnNegateImmunity? OnAllyNegateImmunity { get; init; }
    public Func<IBattle, Pokemon, Pokemon, Move, Delegate?>? OnAllyOverrideAction { get; init; }
    public ResultSourceMoveHandler? OnAllyPrepareHit { get; init; }
    public Func<IBattle, Pokemon, Pokemon, IEffect, Move, Pokemon?>? OnAllyRedirectTarget { get; init; }
    public Action<IBattle, PokemonSideUnion, Pokemon, IEffect>? OnAllyResidual { get; init; }
    public Func<IBattle, Ability, Pokemon, Pokemon, IEffect, bool?>? OnAllySetAbility { get; init; }
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllySetStatus { get; init; }
    public Func<IBattle, Pokemon, Pokemon, Condition, bool?>? OnAllySetWeather { get; init; }
    public Func<IBattle, Pokemon, bool?>? OnAllyStallMove { get; init; }
    public Action<IBattle, Pokemon>? OnAllySwitchOut { get; init; }
    public OnTakeItem? OnAllyTakeItem { get; init; }
    public Action<IBattle, Pokemon>? OnAllyTerrain { get; init; }
    public Action<IBattle, Pokemon>? OnAllyTrapPokemon { get; init; }
    public Func<IBattle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllyTryAddVolatile { get; init; }
    public Func<IBattle, Item, Pokemon, bool?>? OnAllyTryEatItem { get; init; }
    public OnTryHeal? OnAllyTryHeal { get; init; }
    public ExtResultSourceMoveHandler? OnAllyTryHit { get; init; }
    public ExtResultSourceMoveHandler? OnAllyTryHitField { get; init; }
    public ResultMoveHandler? OnAllyTryHitSide { get; init; }
    public ExtResultMoveHandler? OnAllyInvulnerability { get; init; }
    public ResultSourceMoveHandler? OnAllyTryMove { get; init; }
    public Func<IBattle, Pokemon, Pokemon, Move, IntBoolUnion?>? OnAllyTryPrimaryHit { get; init; }
    public Func<IBattle, PokemonType[], Pokemon, PokemonType[]?>? OnAllyType { get; init; }
    public ModifierSourceMoveHandler? OnAllyWeatherModifyDamage { get; init; }
    public ModifierSourceMoveHandler? OnAllyModifyDamagePhase1 { get; init; }
    public ModifierSourceMoveHandler? OnAllyModifyDamagePhase2 { get; init; }

    #endregion
}