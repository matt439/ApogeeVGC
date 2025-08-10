namespace ApogeeVGC_CS.sim
{
    public class FlingData
    {
        public required int BasePower { get; init; }
        public string? Status { get; init; }
        public string? VolatileStatus { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? Effect { get; init; }
    }

    public class ItemData : Item, IPokemonEventMethods, IEffect
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
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnBeforeMove { get; init; }
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
        public OnFractionalPriority? OnFractionalPriority { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; init; }
        public Action<Battle, string, Pokemon>? OnImmunity { get; init; }
        public Func<Battle, Pokemon, string?>? OnLockMove { get; init; }
        public Action<Battle, Pokemon>? OnMaybeTrapPokemon { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnModifyAccuracy { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnModifyAtk { get; init; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnModifyBoost { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnModifyCritRatio { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnModifyDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnModifyDef { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnModifyMove { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnModifyPriority { get; init; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnModifySecondaries { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnModifyType { get; init; }
        public Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove>? OnModifyTarget { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnModifySpA { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnModifySpD { get; init; }
        public Func<Battle, int, Pokemon, int?>? OnModifySpe { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnModifyStab { get; init; }
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
        public Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolUnion?>? OnInvulnerability { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryMove { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnTryPrimaryHit { get; init; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnType { get; init; }
        public Action<Battle, Item, Pokemon>? OnUseItem { get; init; }
        public Action<Battle, Pokemon>? OnUpdate { get; init; }
        public Action<Battle, Pokemon, object?, Condition>? OnWeather { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnWeatherModifyDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnModifyDamagePhase1 { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnModifyDamagePhase2 { get; init; }
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
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnFoeBasePower { get; init; }
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
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnFoeModifyAccuracy { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnFoeModifyAtk { get; init; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnFoeModifyBoost { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnFoeModifyCritRatio { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnFoeModifyDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnFoeModifyDef { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnFoeModifyMove { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnFoeModifyPriority { get; init; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnFoeModifySecondaries { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnFoeModifySpA { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnFoeModifySpD { get; init; }
        public Func<Battle, int, Pokemon, int?>? OnFoeModifySpe { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnFoeModifyStab { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnFoeModifyType { get; init; }
        public Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove>? OnFoeModifyTarget { get; init; }
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
        public Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolUnion?>? OnFoeInvulnerability { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryMove { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnFoeTryPrimaryHit { get; init; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnFoeType { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnFoeWeatherModifyDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnFoeModifyDamagePhase1 { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnFoeModifyDamagePhase2 { get; init; }
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
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnSourceBasePower { get; init; }
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
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnSourceModifyAccuracy { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnSourceModifyAtk { get; init; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnSourceModifyBoost { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnSourceModifyCritRatio { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnSourceModifyDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnSourceModifyDef { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnSourceModifyMove { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnSourceModifyPriority { get; init; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnSourceModifySecondaries { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnSourceModifySpA { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnSourceModifySpD { get; init; }
        public Func<Battle, int, Pokemon, int?>? OnSourceModifySpe { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnSourceModifyStab { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnSourceModifyType { get; init; }
        public Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove>? OnSourceModifyTarget { get; init; }
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
        public Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolUnion?>? OnSourceInvulnerability { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryMove { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnSourceTryPrimaryHit { get; init; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnSourceType { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnSourceWeatherModifyDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnSourceModifyDamagePhase1 { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnSourceModifyDamagePhase2 { get; init; }
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
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAnyBasePower { get; init; }
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
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAnyModifyAccuracy { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAnyModifyAtk { get; init; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAnyModifyBoost { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAnyModifyCritRatio { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAnyModifyDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAnyModifyDef { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnAnyModifyMove { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAnyModifyPriority { get; init; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAnyModifySecondaries { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAnyModifySpA { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAnyModifySpD { get; init; }
        public Func<Battle, int, Pokemon, int?>? OnAnyModifySpe { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAnyModifyStab { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnAnyModifyType { get; init; }
        public Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove>? OnAnyModifyTarget { get; init; }
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
        public Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolUnion?>? OnAnyInvulnerability { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryMove { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnAnyTryPrimaryHit { get; init; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnAnyType { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAnyWeatherModifyDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAnyModifyDamagePhase1 { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAnyModifyDamagePhase2 { get; init; }
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
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAllyDamagingHit { get; init; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnAllyAfterEachBoost { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterHit { get; init; }
        public Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAllyAfterSetStatus { get; init; }
        public Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAllyAfterSubDamage { get; init; }
        public Action<Battle, Pokemon>? OnAllyAfterSwitchInSelf { get; init; }
        public Action<Battle, Item, Pokemon>? OnAllyAfterUseItem { get; init; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyAfterBoost { get; init; }
        public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAllyAfterFaint { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSecondarySelf { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSecondary { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMove { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSelf { get; init; }
        public Action<Battle, Pokemon, Pokemon>? OnAllyAttract { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, IntBoolUnion?>? OnAllyAccuracy { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAllyBasePower { get; init; }
        public Action<Battle, Pokemon, IEffect>? OnAllyBeforeFaint { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyBeforeMove { get; init; }
        public Action<Battle, Pokemon>? OnAllyBeforeSwitchIn { get; init; }
        public Action<Battle, Pokemon>? OnAllyBeforeSwitchOut { get; init; }
        public Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyTryBoost { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyChargeMove { get; init; }
        public Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnAllyCriticalHit { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnAllyDamage { get; init; }
        public Func<Battle, Pokemon, Pokemon, int?>? OnAllyDeductPp { get; init; }
        public Action<Battle, Pokemon>? OnAllyDisableMove { get; init; }
        public Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnAllyDragOut { get; init; }
        public Action<Battle, Item, Pokemon>? OnAllyEatItem { get; init; }
        public Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnAllyEffectiveness { get; init; }
        public Action<Battle, Pokemon, Pokemon, IEffect>? OnAllyFaint { get; init; }
        public Func<Battle, Pokemon, bool?>? OnAllyFlinch { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyHit { get; init; }
        public Action<Battle, string, Pokemon>? OnAllyImmunity { get; init; }
        public Func<Battle, Pokemon, string?>? OnAllyLockMove { get; init; }
        public Action<Battle, Pokemon>? OnAllyMaybeTrapPokemon { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAllyModifyAccuracy { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAllyModifyAtk { get; init; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAllyModifyBoost { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAllyModifyCritRatio { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAllyModifyDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAllyModifyDef { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnAllyModifyMove { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAllyModifyPriority { get; init; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAllyModifySecondaries { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAllyModifySpA { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAllyModifySpD { get; init; }
        public Func<Battle, int, Pokemon, int?>? OnAllyModifySpe { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int?>? OnAllyModifyStab { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnAllyModifyType { get; init; }
        public Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove>? OnAllyModifyTarget { get; init; }
        public Func<Battle, int, Pokemon, int?>? OnAllyModifyWeight { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyMoveAborted { get; init; }
        public Func<Battle, Pokemon, string, bool?>? OnAllyNegateImmunity { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnAllyOverrideAction { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyPrepareHit { get; init; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnAllyRedirectTarget { get; init; }
        public Action<Battle, OnAllyResidualTarget, Pokemon, IEffect>? OnAllyResidual { get; init; }
        public Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnAllySetAbility { get; init; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllySetStatus { get; init; }
        public Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnAllySetWeather { get; init; }
        public Func<Battle, Pokemon, bool?>? OnAllyStallMove { get; init; }
        public Action<Battle, Pokemon>? OnAllySwitchOut { get; init; }
        public Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnAllyTakeItem { get; init; }
        public Action<Battle, Pokemon>? OnAllyTerrain { get; init; }
        public Action<Battle, Pokemon>? OnAllyTrapPokemon { get; init; }
        public Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllyTryAddVolatile { get; init; }
        public Func<Battle, Item, Pokemon, bool?>? OnAllyTryEatItem { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>? OnAllyTryHeal { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAllyTryHit { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryHitField { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryHitSide { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolUnion?>? OnAllyInvulnerability { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryMove { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnAllyTryPrimaryHit { get; init; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnAllyType { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyWeatherModifyDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamagePhase1 { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamagePhase2 { get; init; }
        public Dictionary<string, object> ExtraData { get; set; } = [];
        public Func<Battle, Pokemon, Pokemon, IEffect, bool?>? OnStart { get; init; }
    }

    public class ModdedItemData : ItemData
    {
        public static bool Inherit => true;
        public Action<Battle, Pokemon>? OnCustap { get; init; }
    }

    public class ItemDataTable : Dictionary<IdEntry, ItemData>;

    public class ModdedItemDataTable : Dictionary<IdEntry, ModdedItemData>;

    public class Item : BasicEffect, IBasicEffect
    {
        public FlingData? Fling
        {
            get
            {
                if (IsBerry)
                {
                    return new FlingData { BasePower = 10 };
                }

                if (Id.EndsWith("plate"))
                {
                    return new FlingData { BasePower = 90 };
                }

                if (OnDrive != null)
                {
                    return new FlingData { BasePower = 70 };
                }

                if (MegaStone != null)
                {
                    return new FlingData { BasePower = 80 };
                }

                if (OnMemory != null)
                {
                    return new FlingData { BasePower = 50 };
                }

                return field;
            }
            init;
        }

        public string? OnDrive { get; init; }
        public string? OnMemory { get; init; }
        public string? MegaStone { get; init; }
        public string? MegaEvolves { get; init; }
        public StringTrueUnion? ZMove { get; init; }

        public string? ZMoveType { get; init; }
        public string? ZMoveFrom { get; init; }
        public List<string>? ItemUser { get; init; }
        public required bool IsBerry { get; init; }
        public required bool IgnoreKlutz { get; init; }
        public string? OnPlate { get; init; }
        public required bool IsGem { get; init; }
        public required bool IsPokeball { get; init; }
        public required bool IsPrimalOrb { get; init; }
        public IConditionData? Condition { get; init; }
        public string? ForcedForme { get; init; }
        public bool? IsChoice { get; init; }
        public (int BasePower, string Type)? NaturalGift { get; init; }
        public int? SpriteNum { get; init; }

        public ItemBoosts? Boosts { get; init; }

        // Event handlers
        public Delegate? OnEat { get; init; }
        public Delegate? OnUse { get; init; }
        public Action<Battle, Pokemon>? OnStart { get; init; }
        public Action<Battle, Pokemon>? OnEnd { get; init; }

        public override required string Fullname
        {
            get => "item: " + Name;
            init { }
        }

        public override required EffectType EffectType
        {
            get => EffectType.Item;
            init { }
        }

        public override required int Gen
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
                    _ => field
                };
            }
            init;
        }
    }

    public static class ItemConstants
    {
        public static readonly Item EmptyItem = new()
        {
            IsBerry = false,
            IgnoreKlutz = false,
            IsGem = false,
            IsPokeball = false,
            IsPrimalOrb = false,
            Fullname = string.Empty,
            EffectType = EffectType.Condition,
            Gen = 0,
            Name = string.Empty,
            Exists = false,
            Num = 0,
            NoCopy = false,
            AffectsFainted = false,
            SourceEffect = string.Empty
        };
    }

    public class DexItems(ModdedDex dex)
    {
        private ModdedDex Dex { get; } = dex;
        private Dictionary<string, Item> ItemCache { get; } = [];
        private List<Item>? AllCache { get; } = null;

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