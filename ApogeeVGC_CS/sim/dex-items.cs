using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ApogeeVGC_CS.sim
{
    public class FlingData
    {
        public int BasePower { get; set; }
        public string? Status { get; set; }
        public string? VolatileStatus { get; set; }
        public object? Effect { get; set; } // Replace with delegate or specific type if needed
    }

    public interface IItemData : IPokemonEventMethods, IItem { }

    public class ItemData : IItemData
    {
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAllyDamagingHit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnAllyAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterHit { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAllyAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAllyAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnAllyAfterSwitchInSelf { get; set; }
        public Action<Battle, Item, Pokemon>? OnAllyAfterUseItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAllyAfterFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnAllyAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAllyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnAllyBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnAllyBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnAllyBeforeSwitchOut { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnAllyCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnAllyDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnAllyDeductPp { get; set; }
        public Action<Battle, Pokemon>? OnAllyDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnAllyDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnAllyEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnAllyEffectiveness { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnAllyFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnAllyFlinch { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyHit { get; set; }
        public Action<Battle, string, Pokemon>? OnAllyImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnAllyLockMove { get; set; }
        public Action<Battle, Pokemon>? OnAllyMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAllyModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnAllyModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAllyModifySecondaries { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnAllyModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyStab { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnAllyModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnAllyModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnAllyModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnAllyNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnAllyOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyPrepareHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnAllyRedirectTarget { get; set; }
        public Action<Battle, object, Pokemon, IEffect>? OnAllyResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnAllySetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllySetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnAllySetWeather { get; set; }
        public Func<Battle, Pokemon, bool?>? OnAllyStallMove { get; set; }
        public Action<Battle, Pokemon>? OnAllySwitchOut { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnAllyTakeItem { get; set; }
        public Action<Battle, Pokemon>? OnAllyTerrain { get; set; }
        public Action<Battle, Pokemon>? OnAllyTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllyTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnAllyTryEatItem { get; set; }
        public Func<Battle, object, object?, object?, object?, object?>? OnAllyTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAllyTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAllyInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnAllyTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnAllyType { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamagePhase2 { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnDamagingHit { get; set; }
        public Action<Battle, Pokemon>? OnEmergencyExit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterHit { get; set; }
        public Action<Battle, Pokemon>? OnAfterMega { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnAfterSwitchInSelf { get; set; }
        public Action<Battle, Pokemon>? OnAfterTerastallization { get; set; }
        public Action<Battle, Item, Pokemon>? OnAfterUseItem { get; set; }
        public Action<Battle, Item, Pokemon>? OnAfterTakeItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAfterFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnBeforeSwitchOut { get; set; }
        public Action<Battle, Pokemon>? OnBeforeTurn { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnChangeBoost { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnDeductPp { get; set; }
        public Action<Battle, Pokemon>? OnDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnEffectiveness { get; set; }
        public Action<Battle, Pokemon>? OnEntryHazard { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnFlinch { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, double?>? OnFractionalPriority { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; set; }
        public Action<Battle, string, Pokemon>? OnImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnLockMove { get; set; }
        public Action<Battle, Pokemon>? OnMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnModifySecondaries { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyStab { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnPrepareHit { get; set; }
        public Action<Battle, Pokemon, Pokemon, Condition>? OnPseudoWeatherChange { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnRedirectTarget { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, object?>? OnSetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnSetWeather { get; set; }
        public Action<Battle, Side, Pokemon, Condition>? OnSideConditionStart { get; set; }
        public Func<Battle, Pokemon, bool?>? OnStallMove { get; set; }
        public Action<Battle, Pokemon>? OnSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnSwitchOut { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnSwap { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnTakeItem { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnWeatherChange { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnTerrainChange { get; set; }
        public Action<Battle, Pokemon>? OnTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnTryEatItem { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnType { get; set; }
        public Action<Battle, Item, Pokemon>? OnUseItem { get; set; }
        public Action<Battle, Pokemon>? OnUpdate { get; set; }
        public Action<Battle, Pokemon, object?, Condition>? OnWeather { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamagePhase2 { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnFoeDamagingHit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnFoeAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterHit { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnFoeAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnFoeAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnFoeAfterSwitchInSelf { get; set; }
        public Action<Battle, Item, Pokemon>? OnFoeAfterUseItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnFoeAfterFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnFoeAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnFoeAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnFoeBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnFoeBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnFoeBeforeSwitchOut { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnFoeCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnFoeDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnFoeDeductPp { get; set; }
        public Action<Battle, Pokemon>? OnFoeDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnFoeDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnFoeEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnFoeEffectiveness { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnFoeFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnFoeFlinch { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeHit { get; set; }
        public Action<Battle, string, Pokemon>? OnFoeImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnFoeLockMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?>? OnFoeMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnFoeModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnFoeModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnFoeModifySecondaries { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnFoeModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyStab { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnFoeModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnFoeModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnFoeModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnFoeNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnFoeOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoePrepareHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnFoeRedirectTarget { get; set; }
        public Action<Battle, object, Pokemon, IEffect>? OnFoeResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnFoeSetWeather { get; set; }
        public Func<Battle, Pokemon, bool?>? OnFoeStallMove { get; set; }
        public Action<Battle, Pokemon>? OnFoeSwitchOut { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnFoeTakeItem { get; set; }
        public Action<Battle, Pokemon>? OnFoeTerrain { get; set; }
        public Action<Battle, Pokemon>? OnFoeTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnFoeTryEatItem { get; set; }
        public Func<Battle, object, object?, object?, object?, object?>? OnFoeTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnFoeTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnFoeInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnFoeTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnFoeType { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamagePhase2 { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnSourceDamagingHit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnSourceAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterHit { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnSourceAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnSourceAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnSourceAfterSwitchInSelf { get; set; }
        public Action<Battle, Item, Pokemon>? OnSourceAfterUseItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnSourceAfterFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnSourceAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnSourceAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnSourceBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnSourceBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnSourceBeforeSwitchOut { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnSourceCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnSourceDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnSourceDeductPp { get; set; }
        public Action<Battle, Pokemon>? OnSourceDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnSourceDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnSourceEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnSourceEffectiveness { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnSourceFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnSourceFlinch { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceHit { get; set; }
        public Action<Battle, string, Pokemon>? OnSourceImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnSourceLockMove { get; set; }
        public Action<Battle, Pokemon>? OnSourceMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnSourceModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnSourceModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnSourceModifySecondaries { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnSourceModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyStab { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnSourceModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnSourceModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnSourceModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnSourceNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnSourceOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourcePrepareHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnSourceRedirectTarget { get; set; }
        public Action<Battle, object, Pokemon, IEffect>? OnSourceResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnSourceSetWeather { get; set; }
        public Func<Battle, Pokemon, bool?>? OnSourceStallMove { get; set; }
        public Action<Battle, Pokemon>? OnSourceSwitchOut { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnSourceTakeItem { get; set; }
        public Action<Battle, Pokemon>? OnSourceTerrain { get; set; }
        public Action<Battle, Pokemon>? OnSourceTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnSourceTryEatItem { get; set; }
        public Func<Battle, object, object?, object?, object?, object?>? OnSourceTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnSourceTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnSourceInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnSourceTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnSourceType { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamagePhase2 { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAnyDamagingHit { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnAnyAfterEachBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterHit { get; set; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAnyAfterSetStatus { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAnyAfterSubDamage { get; set; }
        public Action<Battle, Pokemon>? OnAnyAfterSwitchInSelf { get; set; }
        public Action<Battle, Item, Pokemon>? OnAnyAfterUseItem { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyAfterBoost { get; set; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAnyAfterFaint { get; set; }
        public Action<Battle, Pokemon>? OnAnyAfterMega { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSecondarySelf { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSecondary { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMove { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSelf { get; set; }
        public Action<Battle, Pokemon>? OnAnyAfterTerastallization { get; set; }
        public Action<Battle, Pokemon, Pokemon>? OnAnyAttract { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAnyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyBasePower { get; set; }
        public Action<Battle, Pokemon, IEffect>? OnAnyBeforeFaint { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyBeforeMove { get; set; }
        public Action<Battle, Pokemon>? OnAnyBeforeSwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnAnyBeforeSwitchOut { get; set; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyTryBoost { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyChargeMove { get; set; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnAnyCriticalHit { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnAnyDamage { get; set; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnAnyDeductPp { get; set; }
        public Action<Battle, Pokemon>? OnAnyDisableMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnAnyDragOut { get; set; }
        public Action<Battle, Item, Pokemon>? OnAnyEatItem { get; set; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnAnyEffectiveness { get; set; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnAnyFaint { get; set; }
        public Func<Battle, Pokemon, bool?>? OnAnyFlinch { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyHit { get; set; }
        public Action<Battle, string, Pokemon>? OnAnyImmunity { get; set; }
        public Func<Battle, Pokemon, string?>? OnAnyLockMove { get; set; }
        public Action<Battle, Pokemon, Pokemon?>? OnAnyMaybeTrapPokemon { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifyAccuracy { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyAtk { get; set; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAnyModifyBoost { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyCritRatio { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifyDef { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnAnyModifyMove { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyPriority { get; set; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAnyModifySecondaries { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifySpA { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifySpD { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnAnyModifySpe { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyStab { get; set; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnAnyModifyType { get; set; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnAnyModifyTarget { get; set; }
        public Func<Battle, int, Pokemon, int?>? OnAnyModifyWeight { get; set; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyMoveAborted { get; set; }
        public Func<Battle, Pokemon, string, bool?>? OnAnyNegateImmunity { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnAnyOverrideAction { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyPrepareHit { get; set; }
        public Action<Battle, Pokemon, Pokemon, Condition>? OnAnyPseudoWeatherChange { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnAnyRedirectTarget { get; set; }
        public Action<Battle, object, Pokemon, IEffect>? OnAnyResidual { get; set; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnAnySetAbility { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnySetStatus { get; set; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnAnySetWeather { get; set; }
        public Func<Battle, Pokemon, bool?>? OnAnyStallMove { get; set; }
        public Action<Battle, Pokemon>? OnAnySwitchIn { get; set; }
        public Action<Battle, Pokemon>? OnAnySwitchOut { get; set; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnAnyTakeItem { get; set; }
        public Action<Battle, Pokemon>? OnAnyTerrain { get; set; }
        public Action<Battle, Pokemon>? OnAnyTrapPokemon { get; set; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnyTryAddVolatile { get; set; }
        public Func<Battle, Item, Pokemon, bool?>? OnAnyTryEatItem { get; set; }
        public Func<Battle, object, object?, object?, object?, object?>? OnAnyTryHeal { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAnyTryHit { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryHitField { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryHitSide { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAnyInvulnerability { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryMove { get; set; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnAnyTryPrimaryHit { get; set; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnAnyType { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyWeatherModifyDamage { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamagePhase1 { get; set; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamagePhase2 { get; set; }
        public int? OnAccuracyPriority { get; set; }
        public int? OnDamagingHitOrder { get; set; }
        public int? OnAfterMoveSecondaryPriority { get; set; }
        public int? OnAfterMoveSecondarySelfPriority { get; set; }
        public int? OnAfterMoveSelfPriority { get; set; }
        public int? OnAfterSetStatusPriority { get; set; }
        public int? OnAnyBasePowerPriority { get; set; }
        public int? OnAnyInvulnerabilityPriority { get; set; }
        public int? OnAnyModifyAccuracyPriority { get; set; }
        public int? OnAnyFaintPriority { get; set; }
        public int? OnAnyPrepareHitPriority { get; set; }
        public int? OnAnySwitchInPriority { get; set; }
        public int? OnAnySwitchInSubOrder { get; set; }
        public int? OnAllyBasePowerPriority { get; set; }
        public int? OnAllyModifyAtkPriority { get; set; }
        public int? OnAllyModifySpAPriority { get; set; }
        public int? OnAllyModifySpDPriority { get; set; }
        public int? OnAttractPriority { get; set; }
        public int? OnBasePowerPriority { get; set; }
        public int? OnBeforeMovePriority { get; set; }
        public int? OnBeforeSwitchOutPriority { get; set; }
        public int? OnChangeBoostPriority { get; set; }
        public int? OnDamagePriority { get; set; }
        public int? OnDragOutPriority { get; set; }
        public int? OnEffectivenessPriority { get; set; }
        public int? OnFoeBasePowerPriority { get; set; }
        public int? OnFoeBeforeMovePriority { get; set; }
        public int? OnFoeModifyDefPriority { get; set; }
        public int? OnFoeModifySpDPriority { get; set; }
        public int? OnFoeRedirectTargetPriority { get; set; }
        public int? OnFoeTrapPokemonPriority { get; set; }
        public int? OnFractionalPriorityPriority { get; set; }
        public int? OnHitPriority { get; set; }
        public int? OnInvulnerabilityPriority { get; set; }
        public int? OnModifyAccuracyPriority { get; set; }
        public int? OnModifyAtkPriority { get; set; }
        public int? OnModifyCritRatioPriority { get; set; }
        public int? OnModifyDefPriority { get; set; }
        public int? OnModifyMovePriority { get; set; }
        public int? OnModifyPriorityPriority { get; set; }
        public int? OnModifySpAPriority { get; set; }
        public int? OnModifySpDPriority { get; set; }
        public int? OnModifySpePriority { get; set; }
        public int? OnModifyStabPriority { get; set; }
        public int? OnModifyTypePriority { get; set; }
        public int? OnModifyWeightPriority { get; set; }
        public int? OnRedirectTargetPriority { get; set; }
        public int? OnResidualOrder { get; set; }
        public int? OnResidualPriority { get; set; }
        public int? OnResidualSubOrder { get; set; }
        public int? OnSourceBasePowerPriority { get; set; }
        public int? OnSourceInvulnerabilityPriority { get; set; }
        public int? OnSourceModifyAccuracyPriority { get; set; }
        public int? OnSourceModifyAtkPriority { get; set; }
        public int? OnSourceModifyDamagePriority { get; set; }
        public int? OnSourceModifySpAPriority { get; set; }
        public int? OnSwitchInPriority { get; set; }
        public int? OnSwitchInSubOrder { get; set; }
        public int? OnTrapPokemonPriority { get; set; }
        public int? OnTryBoostPriority { get; set; }
        public int? OnTryEatItemPriority { get; set; }
        public int? OnTryHealPriority { get; set; }
        public int? OnTryHitPriority { get; set; }
        public int? OnTryMovePriority { get; set; }
        public int? OnTryPrimaryHitPriority { get; set; }
        public int? OnTypePriority { get; set; }
        public FlingData? Fling { get; set; }
        public string? OnDrive { get; set; }
        public string? OnMemory { get; set; }
        public string? MegaStone { get; set; }
        public string? MegaEvolves { get; set; }
        public object? ZMove { get; set; }
        public string? ZMoveType { get; set; }
        public string? ZMoveFrom { get; set; }
        public List<string>? ItemUser { get; set; }
        public bool IsBerry { get; set; }
        public bool IgnoreKlutz { get; set; }
        public string? OnPlate { get; set; }
        public bool IsGem { get; set; }
        public bool IsPokeball { get; set; }
        public bool IsPrimalOrb { get; set; }
        public IConditionData? Condition { get; set; }
        public string? ForcedForme { get; set; }
        public bool? IsChoice { get; set; }
        public (int BasePower, string Type)? NaturalGift { get; set; }
        public int? SpriteNum { get; set; }
        public object? Boosts { get; set; }
        public Delegate? OnEat { get; set; }
        public Delegate? OnUse { get; set; }
        public Action<Battle, Pokemon>? OnStart { get; set; }
        public Action<Battle, Pokemon>? OnEnd { get; set; }
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
        public Id? Status { get; set; }
        public Id? Weather { get; set; }
        public string SourceEffect { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; set; }
        //public string? EffectTypeString { get; set; }
        public bool? Infiltrates { get; set; }
        public string? RealMove { get; set; }
    }

    public interface IModdedItemData : IItemData
    {
        public bool Inherit { get; set; }
        public Action<Battle, Pokemon>? OnCustap { get; set; }
    }

    public class ItemDataTable : Dictionary<IdEntry, IItemData> { }
    public class ModdedItemDataTable : Dictionary<IdEntry, IModdedItemData> { }


    /// <summary>
    /// Represents an item effect.
    /// </summary>
    public interface IItem : IBasicEffect, IEffect
    {
        public FlingData? Fling { get; set; }
        public string? OnDrive { get; set; }
        public string? OnMemory { get; set; }
        public string? MegaStone { get; set; }
        public string? MegaEvolves { get; set; }
        public object? ZMove { get; set; } // true or string
        public string? ZMoveType { get; set; }
        public string? ZMoveFrom { get; set; }
        public List<string>? ItemUser { get; set; }
        public bool IsBerry { get; set; }
        public bool IgnoreKlutz { get; set; }
        public string? OnPlate { get; set; }
        public bool IsGem { get; set; }
        public bool IsPokeball { get; set; }
        public bool IsPrimalOrb { get; set; }
        public IConditionData? Condition { get; set; }
        public string? ForcedForme { get; set; }
        public bool? IsChoice { get; set; }
        public (int BasePower, string Type)? NaturalGift { get; set; }
        public int? SpriteNum { get; set; }
        public object? Boosts { get; set; } // Replace with SparseBoostsTable or bool

        // Event handlers
        public Delegate? OnEat { get; set; }
        public Delegate? OnUse { get; set; }
        public Action<Battle, Pokemon>? OnStart { get; set; }
        public Action<Battle, Pokemon>? OnEnd { get; set; }
    }

    public class Item : BasicEffect, IItem
    {
        public FlingData? Fling { get; set; }
        public string? OnDrive { get; set; }
        public string? OnMemory { get; set; }
        public string? MegaStone { get; set; }
        public string? MegaEvolves { get; set; }
        public object? ZMove { get; set; }
        public string? ZMoveType { get; set; }
        public string? ZMoveFrom { get; set; }
        public List<string>? ItemUser { get; set; }
        public bool IsBerry { get; set; }
        public bool IgnoreKlutz { get; set; }
        public string? OnPlate { get; set; }
        public bool IsGem { get; set; }
        public bool IsPokeball { get; set; }
        public bool IsPrimalOrb { get; set; }
        public IConditionData? Condition { get; set; }
        public string? ForcedForme { get; set; }
        public bool? IsChoice { get; set; }
        public (int BasePower, string Type)? NaturalGift { get; set; }
        public int? SpriteNum { get; set; }
        public object? Boosts { get; set; }
        public Delegate? OnEat { get; set; }
        public Delegate? OnUse { get; set; }
        public Action<Battle, Pokemon>? OnStart { get; set; }
        public Action<Battle, Pokemon>? OnEnd { get; set; }

        public Item(IItemData data) : base(data)
        {
            Fling = data.Fling;
            OnDrive = data.OnDrive;
            OnMemory = data.OnMemory;
            MegaStone = data.MegaStone;
            MegaEvolves = data.MegaEvolves;
            ZMove = data.ZMove;
            ZMoveType = data.ZMoveType;
            ZMoveFrom = data.ZMoveFrom;
            ItemUser = data.ItemUser;
            IsBerry = data.IsBerry;
            IgnoreKlutz = data.IgnoreKlutz;
            OnPlate = data.OnPlate;
            IsGem = data.IsGem;
            IsPokeball = data.IsPokeball;
            IsPrimalOrb = data.IsPrimalOrb;
            Condition = data.Condition;
            ForcedForme = data.ForcedForme;
            IsChoice = data.IsChoice;
            NaturalGift = data.NaturalGift;
            SpriteNum = data.SpriteNum;
            Boosts = data.Boosts;
            OnEat = data.OnEat;
            OnUse = data.OnUse;
            OnStart = data.OnStart;
            OnEnd = data.OnEnd;
        }

        public void Init()
        {
            InitBasicEffect();
            
            Fullname = $"item: {Name}";
            EffectType = EffectType.Item;
            
            if (Gen == 0)
            {
                // Auto-assign generation based on item number
                if (Num >= 1124)
                    Gen = 9;
                else if (Num >= 927)
                    Gen = 8;
                else if (Num >= 689)
                    Gen = 7;
                else if (Num >= 577)
                    Gen = 6;
                else if (Num >= 537)
                    Gen = 5;
                else if (Num >= 377)
                    Gen = 4;
                else
                    Gen = 3; // Default to Gen 3 for older items
            }

            if (IsBerry)
            {
                Fling ??= new FlingData { BasePower = 10 };
            }
            if (Id.EndsWith("plate"))
            {
                Fling ??= new FlingData { BasePower = 90 };
            }
            if (OnDrive != null)
            {
                Fling ??= new FlingData { BasePower = 70 };
            }
            if (MegaStone != null)
            {
                Fling ??= new FlingData { BasePower = 80 };
            }
            if (OnMemory != null)
            {
                Fling ??= new FlingData { BasePower = 50 };
            }
        }

        //private void CopyPokemonEventMethods(IPokemonEventMethods eventMethods)
        //{
        //    // Ally event handlers (triggered by ally Pokémon actions)
        //    OnAllyDamagingHit = eventMethods.OnAllyDamagingHit;
        //    OnAllyAfterEachBoost = eventMethods.OnAllyAfterEachBoost;
        //    OnAllyAfterHit = eventMethods.OnAllyAfterHit;
        //    OnAllyAfterSetStatus = eventMethods.OnAllyAfterSetStatus;
        //    OnAllyAfterSubDamage = eventMethods.OnAllyAfterSubDamage;
        //    OnAllyAfterSwitchInSelf = eventMethods.OnAllyAfterSwitchInSelf;
        //    OnAllyAfterUseItem = eventMethods.OnAllyAfterUseItem;
        //    OnAllyAfterBoost = eventMethods.OnAllyAfterBoost;
        //    OnAllyAfterFaint = eventMethods.OnAllyAfterFaint;
        //    OnAllyAfterMoveSecondarySelf = eventMethods.OnAllyAfterMoveSecondarySelf;
        //    OnAllyAfterMoveSecondary = eventMethods.OnAllyAfterMoveSecondary;
        //    OnAllyAfterMove = eventMethods.OnAllyAfterMove;
        //    OnAllyAfterMoveSelf = eventMethods.OnAllyAfterMoveSelf;
        //    OnAllyAttract = eventMethods.OnAllyAttract;
        //    OnAllyAccuracy = eventMethods.OnAllyAccuracy;
        //    OnAllyBasePower = eventMethods.OnAllyBasePower;
        //    OnAllyBeforeFaint = eventMethods.OnAllyBeforeFaint;
        //    OnAllyBeforeMove = eventMethods.OnAllyBeforeMove;
        //    OnAllyBeforeSwitchIn = eventMethods.OnAllyBeforeSwitchIn;
        //    OnAllyBeforeSwitchOut = eventMethods.OnAllyBeforeSwitchOut;
        //    OnAllyTryBoost = eventMethods.OnAllyTryBoost;
        //    OnAllyChargeMove = eventMethods.OnAllyChargeMove;
        //    OnAllyCriticalHit = eventMethods.OnAllyCriticalHit;
        //    OnAllyDamage = eventMethods.OnAllyDamage;
        //    OnAllyDeductPP = eventMethods.OnAllyDeductPP;
        //    OnAllyDisableMove = eventMethods.OnAllyDisableMove;
        //    OnAllyDragOut = eventMethods.OnAllyDragOut;
        //    OnAllyEatItem = eventMethods.OnAllyEatItem;
        //    OnAllyEffectiveness = eventMethods.OnAllyEffectiveness;
        //    OnAllyFaint = eventMethods.OnAllyFaint;
        //    OnAllyFlinch = eventMethods.OnAllyFlinch;
        //    OnAllyHit = eventMethods.OnAllyHit;
        //    OnAllyImmunity = eventMethods.OnAllyImmunity;
        //    OnAllyLockMove = eventMethods.OnAllyLockMove;
        //    OnAllyMaybeTrapPokemon = eventMethods.OnAllyMaybeTrapPokemon;
        //    OnAllyModifyAccuracy = eventMethods.OnAllyModifyAccuracy;
        //    OnAllyModifyAtk = eventMethods.OnAllyModifyAtk;
        //    OnAllyModifyBoost = eventMethods.OnAllyModifyBoost;
        //    OnAllyModifyCritRatio = eventMethods.OnAllyModifyCritRatio;
        //    OnAllyModifyDamage = eventMethods.OnAllyModifyDamage;
        //    OnAllyModifyDef = eventMethods.OnAllyModifyDef;
        //    OnAllyModifyMove = eventMethods.OnAllyModifyMove;
        //    OnAllyModifyPriority = eventMethods.OnAllyModifyPriority;
        //    OnAllyModifySecondaries = eventMethods.OnAllyModifySecondaries;
        //    OnAllyModifySpA = eventMethods.OnAllyModifySpA;
        //    OnAllyModifySpD = eventMethods.OnAllyModifySpD;
        //    OnAllyModifySpe = eventMethods.OnAllyModifySpe;
        //    OnAllyModifySTAB = eventMethods.OnAllyModifySTAB;
        //    OnAllyModifyType = eventMethods.OnAllyModifyType;
        //    OnAllyModifyTarget = eventMethods.OnAllyModifyTarget;
        //    OnAllyModifyWeight = eventMethods.OnAllyModifyWeight;
        //    OnAllyMoveAborted = eventMethods.OnAllyMoveAborted;
        //    OnAllyNegateImmunity = eventMethods.OnAllyNegateImmunity;
        //    OnAllyOverrideAction = eventMethods.OnAllyOverrideAction;
        //    OnAllyPrepareHit = eventMethods.OnAllyPrepareHit;
        //    OnAllyRedirectTarget = eventMethods.OnAllyRedirectTarget;
        //    OnAllyResidual = eventMethods.OnAllyResidual;
        //    OnAllySetAbility = eventMethods.OnAllySetAbility;
        //    OnAllySetStatus = eventMethods.OnAllySetStatus;
        //    OnAllySetWeather = eventMethods.OnAllySetWeather;
        //    OnAllyStallMove = eventMethods.OnAllyStallMove;
        //    OnAllySwitchOut = eventMethods.OnAllySwitchOut;
        //    OnAllyTakeItem = eventMethods.OnAllyTakeItem;
        //    OnAllyTerrain = eventMethods.OnAllyTerrain;
        //    OnAllyTrapPokemon = eventMethods.OnAllyTrapPokemon;
        //    OnAllyTryAddVolatile = eventMethods.OnAllyTryAddVolatile;
        //    OnAllyTryEatItem = eventMethods.OnAllyTryEatItem;
        //    OnAllyTryHeal = eventMethods.OnAllyTryHeal;
        //    OnAllyTryHit = eventMethods.OnAllyTryHit;
        //    OnAllyTryHitField = eventMethods.OnAllyTryHitField;
        //    OnAllyTryHitSide = eventMethods.OnAllyTryHitSide;
        //    OnAllyInvulnerability = eventMethods.OnAllyInvulnerability;
        //    OnAllyTryMove = eventMethods.OnAllyTryMove;
        //    OnAllyTryPrimaryHit = eventMethods.OnAllyTryPrimaryHit;
        //    OnAllyType = eventMethods.OnAllyType;
        //    OnAllyWeatherModifyDamage = eventMethods.OnAllyWeatherModifyDamage;
        //    OnAllyModifyDamagePhase1 = eventMethods.OnAllyModifyDamagePhase1;
        //    OnAllyModifyDamagePhase2 = eventMethods.OnAllyModifyDamagePhase2;
        //}
    }

    public static class ItemConstants
    {
        public static readonly Item EmptyItem = new (new ItemData
        {
            Name = string.Empty,
            Exists = false
        });
    }

    public class DexItems(ModdedDex dex)
    {
        private readonly ModdedDex _dex = dex;
        private readonly Dictionary<string, Item> _itemCache = [];
        private readonly List<Item>? _allCache = null;

        public Item Get(string name)
        {
            throw new NotImplementedException("Get method is not implemented yet.");
        }

        public Item Get(Item item)
        {
            throw new NotImplementedException("Get method is not implemented yet.");
        }

        public Item GetById(Id id)
        {
            throw new NotImplementedException("GetByID method is not implemented yet.");
        }

        public List<Item> All()
        {
            throw new NotImplementedException("All method is not implemented yet.");
        }
    }
}