using System.Runtime.CompilerServices;

namespace ApogeeVGC_CS.sim
{
    public class FormatData : Format, IEventMethods, IFormatListEntry
    {
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnDamagingHit { get; init; }
        public Action<Battle, Pokemon>? OnEmergencyExit { get; init; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterEachBoost { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterHit { get; init; }
        public Action<Battle, Pokemon>? OnAfterMega { get; init; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAfterSetStatus { get; init; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAfterSubDamage { get; init; }
        public Action<Battle, Pokemon>? OnAfterSwitchInSelf { get; init; }
        public Action<Battle, Pokemon>? OnAfterTerastallization { get; init; }
        public Action<Battle, Item, Pokemon>? OnAfterUseItem { get; init; }
        public Action<Battle, Item, Pokemon>? OnAfterTakeItem { get; init; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterBoost { get; init; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAfterFaint { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondarySelf { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondary { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMove { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSelf { get; init; }
        public Action<Battle, Pokemon, Pokemon>? OnAttract { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAccuracy { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnBasePower { get; init; }
        public Action<Battle, Pokemon, IEffect>? OnBeforeFaint { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnBeforeMove { get; init; }
        public Action<Battle, Pokemon>? OnBeforeSwitchIn { get; init; }
        public Action<Battle, Pokemon>? OnBeforeSwitchOut { get; init; }
        public Action<Battle, Pokemon>? OnBeforeTurn { get; init; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnChangeBoost { get; init; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnTryBoost { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnChargeMove { get; init; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnCriticalHit { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnDamage { get; init; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnDeductPp { get; init; }
        public Action<Battle, Pokemon>? OnDisableMove { get; init; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnDragOut { get; init; }
        public Action<Battle, Item, Pokemon>? OnEatItem { get; init; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnEffectiveness { get; init; }
        public Action<Battle, Pokemon>? OnEntryHazard { get; init; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnFaint { get; init; }
        public Func<Battle, Pokemon, bool?>? OnFlinch { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, double?>? OnFractionalPriority { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; init; }
        public Action<Battle, string, Pokemon>? OnImmunity { get; init; }
        public Func<Battle, Pokemon, string?>? OnLockMove { get; init; }
        public Action<Battle, Pokemon>? OnMaybeTrapPokemon { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifyAccuracy { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyAtk { get; init; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnModifyBoost { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyCritRatio { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifyDef { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnModifyMove { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyPriority { get; init; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnModifySecondaries { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnModifyType { get; init; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnModifyTarget { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifySpA { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifySpD { get; init; }
        public Func<Battle, int, Pokemon, int?>? OnModifySpe { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyStab { get; init; }
        public Func<Battle, int, Pokemon, int?>? OnModifyWeight { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnMoveAborted { get; init; }
        public Func<Battle, Pokemon, string, bool?>? OnNegateImmunity { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnOverrideAction { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnPrepareHit { get; init; }
        public Action<Battle, Pokemon, Pokemon, Condition>? OnPseudoWeatherChange { get; init; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnRedirectTarget { get; init; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnResidual { get; init; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, object?>? OnSetAbility { get; init; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSetStatus { get; init; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnSetWeather { get; init; }
        public Action<Battle, Side, Pokemon, Condition>? OnSideConditionStart { get; init; }
        public Func<Battle, Pokemon, bool?>? OnStallMove { get; init; }
        public Action<Battle, Pokemon>? OnSwitchIn { get; init; }
        public Action<Battle, Pokemon>? OnSwitchOut { get; init; }
        public Action<Battle, Pokemon, Pokemon>? OnSwap { get; init; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnTakeItem { get; init; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnWeatherChange { get; init; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnTerrainChange { get; init; }
        public Action<Battle, Pokemon>? OnTrapPokemon { get; init; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnTryAddVolatile { get; init; }
        public Func<Battle, Item, Pokemon, bool?>? OnTryEatItem { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnTryHeal { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnTryHit { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitField { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitSide { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnInvulnerability { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryMove { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnTryPrimaryHit { get; init; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnType { get; init; }
        public Action<Battle, Item, Pokemon>? OnUseItem { get; init; }
        public Action<Battle, Pokemon>? OnUpdate { get; init; }
        public Action<Battle, Pokemon, object?, Condition>? OnWeather { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnWeatherModifyDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamagePhase1 { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamagePhase2 { get; init; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnFoeDamagingHit { get; init; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnFoeAfterEachBoost { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterHit { get; init; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnFoeAfterSetStatus { get; init; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnFoeAfterSubDamage { get; init; }
        public Action<Battle, Pokemon>? OnFoeAfterSwitchInSelf { get; init; }
        public Action<Battle, Item, Pokemon>? OnFoeAfterUseItem { get; init; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeAfterBoost { get; init; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnFoeAfterFaint { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSecondarySelf { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSecondary { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMove { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSelf { get; init; }
        public Action<Battle, Pokemon, Pokemon>? OnFoeAttract { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnFoeAccuracy { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeBasePower { get; init; }
        public Action<Battle, Pokemon, IEffect>? OnFoeBeforeFaint { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeBeforeMove { get; init; }
        public Action<Battle, Pokemon>? OnFoeBeforeSwitchIn { get; init; }
        public Action<Battle, Pokemon>? OnFoeBeforeSwitchOut { get; init; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeTryBoost { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeChargeMove { get; init; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnFoeCriticalHit { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnFoeDamage { get; init; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnFoeDeductPp { get; init; }
        public Action<Battle, Pokemon>? OnFoeDisableMove { get; init; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnFoeDragOut { get; init; }
        public Action<Battle, Item, Pokemon>? OnFoeEatItem { get; init; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnFoeEffectiveness { get; init; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnFoeFaint { get; init; }
        public Func<Battle, Pokemon, bool?>? OnFoeFlinch { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeHit { get; init; }
        public Action<Battle, string, Pokemon>? OnFoeImmunity { get; init; }
        public Func<Battle, Pokemon, string?>? OnFoeLockMove { get; init; }
        public Action<Battle, Pokemon, Pokemon?>? OnFoeMaybeTrapPokemon { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifyAccuracy { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyAtk { get; init; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnFoeModifyBoost { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyCritRatio { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifyDef { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnFoeModifyMove { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyPriority { get; init; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnFoeModifySecondaries { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifySpA { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifySpD { get; init; }
        public Func<Battle, int, Pokemon, int?>? OnFoeModifySpe { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyStab { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnFoeModifyType { get; init; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnFoeModifyTarget { get; init; }
        public Func<Battle, int, Pokemon, int?>? OnFoeModifyWeight { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeMoveAborted { get; init; }
        public Func<Battle, Pokemon, string, bool?>? OnFoeNegateImmunity { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnFoeOverrideAction { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoePrepareHit { get; init; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnFoeRedirectTarget { get; init; }
        public Action<Battle, object, Pokemon, IEffect>? OnFoeResidual { get; init; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetAbility { get; init; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetStatus { get; init; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnFoeSetWeather { get; init; }
        public Func<Battle, Pokemon, bool?>? OnFoeStallMove { get; init; }
        public Action<Battle, Pokemon>? OnFoeSwitchOut { get; init; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnFoeTakeItem { get; init; }
        public Action<Battle, Pokemon>? OnFoeTerrain { get; init; }
        public Action<Battle, Pokemon>? OnFoeTrapPokemon { get; init; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeTryAddVolatile { get; init; }
        public Func<Battle, Item, Pokemon, bool?>? OnFoeTryEatItem { get; init; }
        public Func<Battle, object, object?, object?, object?, object?>? OnFoeTryHeal { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnFoeTryHit { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryHitField { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryHitSide { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnFoeInvulnerability { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryMove { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnFoeTryPrimaryHit { get; init; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnFoeType { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeWeatherModifyDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamagePhase1 { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamagePhase2 { get; init; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnSourceDamagingHit { get; init; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnSourceAfterEachBoost { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterHit { get; init; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnSourceAfterSetStatus { get; init; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnSourceAfterSubDamage { get; init; }
        public Action<Battle, Pokemon>? OnSourceAfterSwitchInSelf { get; init; }
        public Action<Battle, Item, Pokemon>? OnSourceAfterUseItem { get; init; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceAfterBoost { get; init; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnSourceAfterFaint { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSecondarySelf { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSecondary { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMove { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSelf { get; init; }
        public Action<Battle, Pokemon, Pokemon>? OnSourceAttract { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnSourceAccuracy { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceBasePower { get; init; }
        public Action<Battle, Pokemon, IEffect>? OnSourceBeforeFaint { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceBeforeMove { get; init; }
        public Action<Battle, Pokemon>? OnSourceBeforeSwitchIn { get; init; }
        public Action<Battle, Pokemon>? OnSourceBeforeSwitchOut { get; init; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceTryBoost { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceChargeMove { get; init; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnSourceCriticalHit { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnSourceDamage { get; init; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnSourceDeductPp { get; init; }
        public Action<Battle, Pokemon>? OnSourceDisableMove { get; init; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnSourceDragOut { get; init; }
        public Action<Battle, Item, Pokemon>? OnSourceEatItem { get; init; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnSourceEffectiveness { get; init; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnSourceFaint { get; init; }
        public Func<Battle, Pokemon, bool?>? OnSourceFlinch { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceHit { get; init; }
        public Action<Battle, string, Pokemon>? OnSourceImmunity { get; init; }
        public Func<Battle, Pokemon, string?>? OnSourceLockMove { get; init; }
        public Action<Battle, Pokemon>? OnSourceMaybeTrapPokemon { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifyAccuracy { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyAtk { get; init; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnSourceModifyBoost { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyCritRatio { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifyDef { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnSourceModifyMove { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyPriority { get; init; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnSourceModifySecondaries { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifySpA { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifySpD { get; init; }
        public Func<Battle, int, Pokemon, int?>? OnSourceModifySpe { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyStab { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnSourceModifyType { get; init; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnSourceModifyTarget { get; init; }
        public Func<Battle, int, Pokemon, int?>? OnSourceModifyWeight { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceMoveAborted { get; init; }
        public Func<Battle, Pokemon, string, bool?>? OnSourceNegateImmunity { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnSourceOverrideAction { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourcePrepareHit { get; init; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnSourceRedirectTarget { get; init; }
        public Action<Battle, object, Pokemon, IEffect>? OnSourceResidual { get; init; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetAbility { get; init; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetStatus { get; init; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnSourceSetWeather { get; init; }
        public Func<Battle, Pokemon, bool?>? OnSourceStallMove { get; init; }
        public Action<Battle, Pokemon>? OnSourceSwitchOut { get; init; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnSourceTakeItem { get; init; }
        public Action<Battle, Pokemon>? OnSourceTerrain { get; init; }
        public Action<Battle, Pokemon>? OnSourceTrapPokemon { get; init; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceTryAddVolatile { get; init; }
        public Func<Battle, Item, Pokemon, bool?>? OnSourceTryEatItem { get; init; }
        public Func<Battle, object, object?, object?, object?, object?>? OnSourceTryHeal { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnSourceTryHit { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryHitField { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryHitSide { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnSourceInvulnerability { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryMove { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnSourceTryPrimaryHit { get; init; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnSourceType { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceWeatherModifyDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamagePhase1 { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamagePhase2 { get; init; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAnyDamagingHit { get; init; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnAnyAfterEachBoost { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterHit { get; init; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAnyAfterSetStatus { get; init; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAnyAfterSubDamage { get; init; }
        public Action<Battle, Pokemon>? OnAnyAfterSwitchInSelf { get; init; }
        public Action<Battle, Item, Pokemon>? OnAnyAfterUseItem { get; init; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyAfterBoost { get; init; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAnyAfterFaint { get; init; }
        public Action<Battle, Pokemon>? OnAnyAfterMega { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSecondarySelf { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSecondary { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMove { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSelf { get; init; }
        public Action<Battle, Pokemon>? OnAnyAfterTerastallization { get; init; }
        public Action<Battle, Pokemon, Pokemon>? OnAnyAttract { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAnyAccuracy { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyBasePower { get; init; }
        public Action<Battle, Pokemon, IEffect>? OnAnyBeforeFaint { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyBeforeMove { get; init; }
        public Action<Battle, Pokemon>? OnAnyBeforeSwitchIn { get; init; }
        public Action<Battle, Pokemon>? OnAnyBeforeSwitchOut { get; init; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyTryBoost { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyChargeMove { get; init; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnAnyCriticalHit { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnAnyDamage { get; init; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnAnyDeductPp { get; init; }
        public Action<Battle, Pokemon>? OnAnyDisableMove { get; init; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnAnyDragOut { get; init; }
        public Action<Battle, Item, Pokemon>? OnAnyEatItem { get; init; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnAnyEffectiveness { get; init; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnAnyFaint { get; init; }
        public Func<Battle, Pokemon, bool?>? OnAnyFlinch { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyHit { get; init; }
        public Action<Battle, string, Pokemon>? OnAnyImmunity { get; init; }
        public Func<Battle, Pokemon, string?>? OnAnyLockMove { get; init; }
        public Action<Battle, Pokemon, Pokemon?>? OnAnyMaybeTrapPokemon { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifyAccuracy { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyAtk { get; init; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAnyModifyBoost { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyCritRatio { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifyDef { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnAnyModifyMove { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyPriority { get; init; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAnyModifySecondaries { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifySpA { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifySpD { get; init; }
        public Func<Battle, int, Pokemon, int?>? OnAnyModifySpe { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyStab { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnAnyModifyType { get; init; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnAnyModifyTarget { get; init; }
        public Func<Battle, int, Pokemon, int?>? OnAnyModifyWeight { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyMoveAborted { get; init; }
        public Func<Battle, Pokemon, string, bool?>? OnAnyNegateImmunity { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnAnyOverrideAction { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyPrepareHit { get; init; }
        public Action<Battle, Pokemon, Pokemon, Condition>? OnAnyPseudoWeatherChange { get; init; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnAnyRedirectTarget { get; init; }
        public Action<Battle, object, Pokemon, IEffect>? OnAnyResidual { get; init; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnAnySetAbility { get; init; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnySetStatus { get; init; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnAnySetWeather { get; init; }
        public Func<Battle, Pokemon, bool?>? OnAnyStallMove { get; init; }
        public Action<Battle, Pokemon>? OnAnySwitchIn { get; init; }
        public Action<Battle, Pokemon>? OnAnySwitchOut { get; init; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnAnyTakeItem { get; init; }
        public Action<Battle, Pokemon>? OnAnyTerrain { get; init; }
        public Action<Battle, Pokemon>? OnAnyTrapPokemon { get; init; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnyTryAddVolatile { get; init; }
        public Func<Battle, Item, Pokemon, bool?>? OnAnyTryEatItem { get; init; }
        public Func<Battle, object, object?, object?, object?, object?>? OnAnyTryHeal { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAnyTryHit { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryHitField { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryHitSide { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAnyInvulnerability { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryMove { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnAnyTryPrimaryHit { get; init; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnAnyType { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyWeatherModifyDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamagePhase1 { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamagePhase2 { get; init; }
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
    }

    // Represents the element of a "FormatList" type
    public interface IFormatListEntry;

    public class SectionEntry : IFormatListEntry
    {
        public required string Section { get; init; }
        public int? Column { get; init; }
    }

    public class FormatList : List<IFormatListEntry>;

    public class ModdedFormatData : FormatData
    {
        public static bool Inherit => true;
    }

    public class FormatDataTable : Dictionary<IdEntry, FormatData>;

    public class ModdedFormatDataTable : Dictionary<IdEntry, ModdedFormatData>;

    public enum FormatEffectType
    {
        Format,
        Ruleset,
        Rule,
        ValidatorRule
    }

    public class ComplexBan
    {
        public required string Rule { get; init; }
        public required string Source { get; init; }
        public required int Limit { get; init; }
        public required List<string> Bans { get; init; }
    }

    public interface IGameTimerSettings
    {
        public bool DcTimer { get; }
        public bool DcTimerBank { get; }
        public int Starting { get; }
        public int Grace { get; }
        public int AddPerTurn { get; }
        public int MaxPerTurn { get; }
        public int MaxFirstTurn { get; }
        public bool TimeoutAutoChoose { get; }
        public bool Accelerate { get; }
    }

    /**
     * A RuleTable keeps track of the rules that a format has. The key can be:
     * - '[ruleid]' the ID of a rule in effect
     * - '-[thing]' or '-[category]:[thing]' ban a thing
     * - '+[thing]' or '+[category]:[thing]' allow a thing (override a ban)
     * [category] is one of: item, move, ability, species, basespecies
     *
     * The value is the name of the parent rule (blank for the active format).
     */
    public class RuleTable : Dictionary<string, string>
    {
        public required List<ComplexBan> ComplexBans { get; init; }
        public required List<ComplexBan> ComplexTeamBans { get; init; }
        public (Func<object, bool> CheckCanLearn, string)? CheckCanLearn { get; init; }
        public (IGameTimerSettings Timer, string)? Timer { get; init; }
        public required List<string> TagRules { get; init; }
        public required Dictionary<string, string> ValueRules { get; init; }

        public required int MinTeamSize { get; init; }
        public required int MaxTeamSize { get; init; }
        public int? PickedTeamSize { get; init; }
        public int? MaxTotalLevel { get; init; }
        public required int MaxMoveCount { get; init; }
        public required int MinSourceGen { get; init; }
        public required int MinLevel { get; init; }
        public required int MaxLevel { get; init; }
        public required int DefaultLevel { get; init; }
        public int? AdjustLevel { get; init; }
        public int? AdjustLevelDown { get; init; }
        public int? EvLimit { get; init; }

        public bool IsBanned(string thing)
        {
            throw new NotImplementedException("IsBanned method is not implemented yet.");
        }

        public bool IsBannedSpecies(string species)
        {
            throw new NotImplementedException("IsBannedSpecies method is not implemented yet.");
        }

        public bool IsRestricted(string thing)
        {
            throw new NotImplementedException("IsRestricted method is not implemented yet.");
        }

        public bool IsRestrictedSpecies(string species)
        {
            throw new NotImplementedException("IsRestrictedSpecies method is not implemented yet.");
        }

        public List<string> GetTagRules()
        {
            throw new NotImplementedException("GetTagRules method is not implemented yet.");
        }

        public string? Check(string thing, Dictionary<string, bool>? setHas = null)
        {
            throw new NotImplementedException("Check method is not implemented yet.");
        }

        public string? GetReason(string key)
        {
            throw new NotImplementedException("GetReason method is not implemented yet.");
        }

        public string Blame(string key)
        {
            throw new NotImplementedException("Blame method is not implemented yet.");
        }

        public int GetComplexBanIndex(List<ComplexBan> complexBans, string rule)
        {
            throw new NotImplementedException("GetComplexBanIndex method is not implemented yet.");
        }

        public void AddComplexBan(string rule, string source, int limit, List<string> bans)
        {
            throw new NotImplementedException("AddComplexBan method is not implemented yet.");
        }

        public void AddComplexTeamBan(string rule, string source, int limit, List<string> bans)
        {
            throw new NotImplementedException("AddComplexTeamBan method is not implemented yet.");
        }

        public void ResolveNumbers(Format format, DexFormats dex)
        {
            throw new NotImplementedException("ResolveNumbers method is not implemented yet.");
        }

        public bool HasComplexBans()
        {
            throw new NotImplementedException("HasComplexBans method is not implemented yet.");
        }
    }

    public class Format : BasicEffect, IBasicEffect, IEffect
    {
        public required string Mod { get; init; }

        /// <summary>
        /// Name of the team generator algorithm, if this format uses
        /// random/fixed teams. null if players can bring teams.
        /// </summary>
        public string? Team { get; init; }
        public required FormatEffectType FormatEffectType { get; init; }
        public required bool Debug { get; init; }

        /// <summary>
        /// Whether or not a format will update ladder points if searched
        /// for using the "Battle!" button.
        /// (Challenge and tournament games will never update ladder points.)
        /// (Defaults to true.)
        /// </summary>
        public required object Rated
        {
            get;
            init
            {
                if (value is bool or string)
                {
                    field = value;
                }
                else
                {
                    throw new ArgumentException("Rated must be a bool or string.");
                }
            }
        }

        /// <summary>Game type.</summary>
        public required GameType GameType { get; init; }

        /// <summary>Number of players, based on game type, for convenience</summary>
        public int PlayerCount => GameType is GameType.Multi or GameType.FreeForAll ? 4 : 2;

        /// <summary>List of rule names.</summary>
        public required List<string> Ruleset { get; init; }

        /// <summary>
        /// Base list of rule names as specified in "./config/formats.ts".
        /// Used in a custom format to correctly display the altered ruleset.
        /// </summary>
        public required List<string> BaseRuleset { get; init; }

        /// <summary>List of banned effects.</summary>
        public required List<string> Banlist { get; init; }

        /// <summary>List of effects that aren't completely banned.</summary>
        public required List<string> Restricted { get; init; }

        /// <summary>List of inherited banned effects to override.</summary>
        public required List<string> Unbanlist { get; init; }

        /// <summary>List of ruleset and banlist changes in a custom format.</summary>
        public List<string>? CustomRules { get; init; }

        /// <summary>Table of rule names and banned effects.</summary>
        public RuleTable? RuleTable { get; init; }

        /// <summary>An optional function that runs at the start of a battle.</summary>
        public Action<Battle>? OnBegin { get; init; } // deault is undefined

        public required bool NoLog { get; init; }

        // Rule-specific properties (only apply to rules, not formats)
        public object? HasValue // Can be bool, "integer", or "positive-integer"
        {
            get;
            init
            {
                field = value switch
                {
                    bool or string or int => value,
                    null => null,
                    _ => throw new ArgumentException("HasValue must be a bool, string, int, or null.")
                };
            }
        } 
        public Func<ValidationContext, string, string?>? OnValidateRule { get; init; }

        /// <summary>ID of rule that can't be combined with this rule</summary>
        public string? MutuallyExclusiveWith { get; init; }

        // Battle module properties
        public IModdedBattleScriptsData? Battle { get; init; }
        public IModdedBattlePokemon? Pokemon { get; init; }
        public IModdedBattleQueue? Queue { get; init; }
        public IModdedField? Field { get; init; }
        public IModdedBattleActions? Actions { get; init; }
        public IModdedBattleSide? Side { get; init; }

        // Display and tournament properties
        public bool? ChallengeShow { get; init; }
        public bool? SearchShow { get; init; }
        public bool? BestOfDefault { get; init; }
        public bool? TeraPreviewDefault { get; init; }
        public List<string>? Threads { get; init; }
        public bool? TournamentShow { get; init; }

        // Validation functions
        public Func<TeamValidator, Move, Species, PokemonSources, PokemonSet, string?>? CheckCanLearn { get; init; }
        public Func<Format, string, Id>? GetEvoFamily { get; init; }
        public Func<Format, Pokemon, HashSet<string>>? GetSharedPower { get; init; }
        public Func<Format, Pokemon, HashSet<string>>? GetSharedItems { get; init; }
        public Func<TeamValidator, PokemonSet, Format, object?, object?, List<string>?>? OnChangeSet { get; init; }

        // Battle event handlers
        public int? OnModifySpeciesPriority { get; init; }
        public Func<Battle, Species, Pokemon?, Pokemon?, IEffect?, Species?>? OnModifySpecies { get; init; }
        public Action<Battle>? OnBattleStart { get; init; }
        public Action<Battle>? OnTeamPreview { get; init; }
        public Func<TeamValidator, PokemonSet, Format, object, object, List<string>?>? OnValidateSet { get; init; }
        public Func<TeamValidator, List<PokemonSet>, Format, object, List<string>?>? OnValidateTeam { get; init; }
        public Func<TeamValidator, PokemonSet, object, List<string>?>? ValidateSet { get; init; }
        public Func<TeamValidator, List<PokemonSet>, ValidationOptions?, List<string>?>? ValidateTeam { get; init; }

        // Layout properties
        public string? Section { get; init; }
        public int? Column { get; init; }
    }

    // Helper class for Format
    public class ValidationContext
    {
        public Format Format { get; set; } = null!;
        public RuleTable RuleTable { get; set; } = null!;
        public ModdedDex Dex { get; set; } = null!;
    }

    // Helper class for Format
    public class ValidationOptions
    {
        public bool? RemoveNicknames { get; set; }
        public Dictionary<string, Dictionary<string, bool>>? SkipSets { get; set; }
    }

    public static class FormatUtils
    {
        public static FormatList MergeFormatLists(FormatList main, FormatList? custom)
        {
            throw new NotImplementedException("MergeFormatLists method is not implemented yet.");
        }
    }

    public class DexFormats(ModdedDex dex)
    {
        private ModdedDex Dex { get; } = dex;
        private Dictionary<Id, Format> RulesetCache { get; }= [];
        private Format[]? FormatListCache { get; } = null;

        public DexFormats Load()
        {
            throw new NotImplementedException("load method is not implemented yet.");
        }

        public string Validate(string name)
        {
            throw new NotImplementedException("Validate method is not implemented yet.");
        }

        public Format Get(string? name = null, bool isTrusted = false)
        {
            throw new NotImplementedException();
        }

        public Format[] All()
        {
            throw new NotImplementedException("All method is not implemented yet.");
        }

        public bool IsPokemonRule(string ruleSpec)
        {
            throw new NotImplementedException("IsPokemonRule method is not implemented yet.");
        }

        public RuleTable GetRuleTable(Format format, int depth = 1, Dictionary<string, int>? repeals = null)
        {
            throw new NotImplementedException("GetRuleTable method is not implemented yet.");
        }

        public object ValidateRule(string rule, Format? format = null)
        {
            throw new NotImplementedException("ValidateRule method is not implemented yet.");
        }

        public bool ValidPokemonTag(Id tagId)
        {
            throw new NotImplementedException("ValidPokemonTag method is not implemented yet.");
        }

        public string ValidateBanRule(string rule)
        {
            throw new NotImplementedException("ValidateBanRule method is not implemented yet.");
        }
    }
}