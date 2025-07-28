namespace ApogeeVGC_CS.sim
{
    public interface IEventMethods
    {
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnDamagingHit { get; }
        Action<Battle, Pokemon>? OnEmergencyExit { get; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterEachBoost { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterHit { get; }
        Action<Battle, Pokemon>? OnAfterMega { get; }
        Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAfterSetStatus { get; }
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAfterSubDamage { get; }
        Action<Battle, Pokemon>? OnAfterSwitchInSelf { get; }
        Action<Battle, Pokemon>? OnAfterTerastallization { get; }
        Action<Battle, Item, Pokemon>? OnAfterUseItem { get; }
        Action<Battle, Item, Pokemon>? OnAfterTakeItem { get; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterBoost { get; }
        Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAfterFaint { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondarySelf { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSecondary { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMove { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAfterMoveSelf { get; }
        Action<Battle, Pokemon, Pokemon>? OnAttract { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAccuracy { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnBasePower { get; }
        Action<Battle, Pokemon, IEffect>? OnBeforeFaint { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnBeforeMove { get; }
        Action<Battle, Pokemon>? OnBeforeSwitchIn { get; }
        Action<Battle, Pokemon>? OnBeforeSwitchOut { get; }
        Action<Battle, Pokemon>? OnBeforeTurn { get; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnChangeBoost { get; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnTryBoost { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnChargeMove { get; }
        Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnCriticalHit { get; }
        Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnDamage { get; }
        Func<Battle, Pokemon, Pokemon, int?>? OnDeductPp { get; }
        Action<Battle, Pokemon>? OnDisableMove { get; }
        Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnDragOut { get; }
        Action<Battle, Item, Pokemon>? OnEatItem { get; }
        Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnEffectiveness { get; }
        Action<Battle, Pokemon>? OnEntryHazard { get; }
        Action<Battle, Pokemon, Pokemon, IEffect>? OnFaint { get; }
        Func<Battle, Pokemon, bool?>? OnFlinch { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, double?>? OnFractionalPriority { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; }
        Action<Battle, string, Pokemon>? OnImmunity { get; }
        Func<Battle, Pokemon, string?>? OnLockMove { get; }
        Action<Battle, Pokemon>? OnMaybeTrapPokemon { get; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifyAccuracy { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyAtk { get; }
        Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnModifyBoost { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyCritRatio { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamage { get; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifyDef { get; }
        Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnModifyMove { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyPriority { get; }
        Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnModifySecondaries { get; }
        Action<Battle, ActiveMove, Pokemon, Pokemon>? OnModifyType { get; }
        Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnModifyTarget { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifySpA { get; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnModifySpD { get; }
        Func<Battle, int, Pokemon, int?>? OnModifySpe { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyStab { get; }
        Func<Battle, int, Pokemon, int?>? OnModifyWeight { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnMoveAborted { get; }
        Func<Battle, Pokemon, string, bool?>? OnNegateImmunity { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnOverrideAction { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnPrepareHit { get; }
        Action<Battle, Pokemon, Pokemon, Condition>? OnPseudoWeatherChange { get; }
        Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnRedirectTarget { get; }
        Action<Battle, Pokemon, Pokemon, IEffect>? OnResidual { get; }
        Func<Battle, string, Pokemon, Pokemon, IEffect, object?>? OnSetAbility { get; }
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSetStatus { get; }
        Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnSetWeather { get; }
        Action<Battle, Side, Pokemon, Condition>? OnSideConditionStart { get; }
        Func<Battle, Pokemon, bool?>? OnStallMove { get; }
        Action<Battle, Pokemon>? OnSwitchIn { get; }
        Action<Battle, Pokemon>? OnSwitchOut { get; }
        Action<Battle, Pokemon, Pokemon>? OnSwap { get; }
        Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnTakeItem { get; }
        Action<Battle, Pokemon, Pokemon, IEffect>? OnWeatherChange { get; }
        Action<Battle, Pokemon, Pokemon, IEffect>? OnTerrainChange { get; }
        Action<Battle, Pokemon>? OnTrapPokemon { get; }
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnTryAddVolatile { get; }
        Func<Battle, Item, Pokemon, bool?>? OnTryEatItem { get; }
        Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnTryHeal { get; }



        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnTryHit { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitField { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryHitSide { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnInvulnerability { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnTryMove { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnTryPrimaryHit { get; }
        Func<Battle, List<string>, Pokemon, List<string>?>? OnType { get; }
        Action<Battle, Item, Pokemon>? OnUseItem { get; }
        Action<Battle, Pokemon>? OnUpdate { get; }
        Action<Battle, Pokemon, object?, Condition>? OnWeather { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnWeatherModifyDamage { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamagePhase1 { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnModifyDamagePhase2 { get; }

        // Foe event handlers (triggered by opponent actions)
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnFoeDamagingHit { get; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnFoeAfterEachBoost { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterHit { get; }
        Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnFoeAfterSetStatus { get; }
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnFoeAfterSubDamage { get; }
        Action<Battle, Pokemon>? OnFoeAfterSwitchInSelf { get; }
        Action<Battle, Item, Pokemon>? OnFoeAfterUseItem { get; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeAfterBoost { get; }
        Action<Battle, int, Pokemon, Pokemon, IEffect>? OnFoeAfterFaint { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSecondarySelf { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSecondary { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMove { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeAfterMoveSelf { get; }
        Action<Battle, Pokemon, Pokemon>? OnFoeAttract { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnFoeAccuracy { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeBasePower { get; }
        Action<Battle, Pokemon, IEffect>? OnFoeBeforeFaint { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeBeforeMove { get; }
        Action<Battle, Pokemon>? OnFoeBeforeSwitchIn { get; }
        Action<Battle, Pokemon>? OnFoeBeforeSwitchOut { get; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeTryBoost { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeChargeMove { get; }
        Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnFoeCriticalHit { get; }
        Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnFoeDamage { get; }
        Func<Battle, Pokemon, Pokemon, int?>? OnFoeDeductPp { get; }
        Action<Battle, Pokemon>? OnFoeDisableMove { get; }
        Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnFoeDragOut { get; }
        Action<Battle, Item, Pokemon>? OnFoeEatItem { get; }
        Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnFoeEffectiveness { get; }
        Action<Battle, Pokemon, Pokemon, IEffect>? OnFoeFaint { get; }
        Func<Battle, Pokemon, bool?>? OnFoeFlinch { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeHit { get; }
        Action<Battle, string, Pokemon>? OnFoeImmunity { get; }
        Func<Battle, Pokemon, string?>? OnFoeLockMove { get; }
        Action<Battle, Pokemon, Pokemon?>? OnFoeMaybeTrapPokemon { get; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifyAccuracy { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyAtk { get; }
        Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnFoeModifyBoost { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyCritRatio { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamage { get; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifyDef { get; }
        Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnFoeModifyMove { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyPriority { get; }
        Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnFoeModifySecondaries { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifySpA { get; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnFoeModifySpD { get; }
        Func<Battle, int, Pokemon, int?>? OnFoeModifySpe { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyStab { get; }
        Action<Battle, ActiveMove, Pokemon, Pokemon>? OnFoeModifyType { get; }
        Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnFoeModifyTarget { get; }
        Func<Battle, int, Pokemon, int?>? OnFoeModifyWeight { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnFoeMoveAborted { get; }
        Func<Battle, Pokemon, string, bool?>? OnFoeNegateImmunity { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnFoeOverrideAction { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoePrepareHit { get; }
        Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnFoeRedirectTarget { get; }
        Action<Battle, object, Pokemon, IEffect>? OnFoeResidual { get; }
        Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetAbility { get; }
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetStatus { get; }
        Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnFoeSetWeather { get; }
        Func<Battle, Pokemon, bool?>? OnFoeStallMove { get; }
        Action<Battle, Pokemon>? OnFoeSwitchOut { get; }
        Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnFoeTakeItem { get; }
        Action<Battle, Pokemon>? OnFoeTerrain { get; }
        Action<Battle, Pokemon>? OnFoeTrapPokemon { get; }
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeTryAddVolatile { get; }
        Func<Battle, Item, Pokemon, bool?>? OnFoeTryEatItem { get; }
        Func<Battle, object, object?, object?, object?, object?>? OnFoeTryHeal { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnFoeTryHit { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryHitField { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryHitSide { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnFoeInvulnerability { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnFoeTryMove { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnFoeTryPrimaryHit { get; }


        Func<Battle, List<string>, Pokemon, List<string>?>? OnFoeType { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeWeatherModifyDamage { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamagePhase1 { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnFoeModifyDamagePhase2 { get; }

        // Source event handlers (triggered when this Pokémon is the source)
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnSourceDamagingHit { get; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnSourceAfterEachBoost { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterHit { get; }
        Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnSourceAfterSetStatus { get; }
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnSourceAfterSubDamage { get; }
        Action<Battle, Pokemon>? OnSourceAfterSwitchInSelf { get; }
        Action<Battle, Item, Pokemon>? OnSourceAfterUseItem { get; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceAfterBoost { get; }
        Action<Battle, int, Pokemon, Pokemon, IEffect>? OnSourceAfterFaint { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSecondarySelf { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSecondary { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMove { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceAfterMoveSelf { get; }
        Action<Battle, Pokemon, Pokemon>? OnSourceAttract { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnSourceAccuracy { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceBasePower { get; }
        Action<Battle, Pokemon, IEffect>? OnSourceBeforeFaint { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceBeforeMove { get; }
        Action<Battle, Pokemon>? OnSourceBeforeSwitchIn { get; }
        Action<Battle, Pokemon>? OnSourceBeforeSwitchOut { get; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceTryBoost { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceChargeMove { get; }
        Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnSourceCriticalHit { get; }
        Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnSourceDamage { get; }
        Func<Battle, Pokemon, Pokemon, int?>? OnSourceDeductPp { get; }
        Action<Battle, Pokemon>? OnSourceDisableMove { get; }
        Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnSourceDragOut { get; }
        Action<Battle, Item, Pokemon>? OnSourceEatItem { get; }
        Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnSourceEffectiveness { get; }
        Action<Battle, Pokemon, Pokemon, IEffect>? OnSourceFaint { get; }
        Func<Battle, Pokemon, bool?>? OnSourceFlinch { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceHit { get; }
        Action<Battle, string, Pokemon>? OnSourceImmunity { get; }
        Func<Battle, Pokemon, string?>? OnSourceLockMove { get; }
        Action<Battle, Pokemon>? OnSourceMaybeTrapPokemon { get; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifyAccuracy { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyAtk { get; }
        Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnSourceModifyBoost { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyCritRatio { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamage { get; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifyDef { get; }
        Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnSourceModifyMove { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyPriority { get; }
        Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnSourceModifySecondaries { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifySpA { get; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnSourceModifySpD { get; }
        Func<Battle, int, Pokemon, int?>? OnSourceModifySpe { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyStab { get; }
        Action<Battle, ActiveMove, Pokemon, Pokemon>? OnSourceModifyType { get; }
        Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnSourceModifyTarget { get; }
        Func<Battle, int, Pokemon, int?>? OnSourceModifyWeight { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnSourceMoveAborted { get; }
        Func<Battle, Pokemon, string, bool?>? OnSourceNegateImmunity { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnSourceOverrideAction { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourcePrepareHit { get; }
        Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnSourceRedirectTarget { get; }
        Action<Battle, object, Pokemon, IEffect>? OnSourceResidual { get; }
        Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetAbility { get; }
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetStatus { get; }
        Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnSourceSetWeather { get; }
        Func<Battle, Pokemon, bool?>? OnSourceStallMove { get; }
        Action<Battle, Pokemon>? OnSourceSwitchOut { get; }
        Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnSourceTakeItem { get; }
        Action<Battle, Pokemon>? OnSourceTerrain { get; }
        Action<Battle, Pokemon>? OnSourceTrapPokemon { get; }
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceTryAddVolatile { get; }
        Func<Battle, Item, Pokemon, bool?>? OnSourceTryEatItem { get; }
        Func<Battle, object, object?, object?, object?, object?>? OnSourceTryHeal { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnSourceTryHit { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryHitField { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryHitSide { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnSourceInvulnerability { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnSourceTryMove { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnSourceTryPrimaryHit { get; }
        Func<Battle, List<string>, Pokemon, List<string>?>? OnSourceType { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceWeatherModifyDamage { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamagePhase1 { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnSourceModifyDamagePhase2 { get; }

        // Any event handlers (triggered for any Pokémon action in battle)
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAnyDamagingHit { get; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnAnyAfterEachBoost { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterHit { get; }
        Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAnyAfterSetStatus { get; }
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAnyAfterSubDamage { get; }
        Action<Battle, Pokemon>? OnAnyAfterSwitchInSelf { get; }
        Action<Battle, Item, Pokemon>? OnAnyAfterUseItem { get; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyAfterBoost { get; }
        Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAnyAfterFaint { get; }
        Action<Battle, Pokemon>? OnAnyAfterMega { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSecondarySelf { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSecondary { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMove { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyAfterMoveSelf { get; }
        Action<Battle, Pokemon>? OnAnyAfterTerastallization { get; }
        Action<Battle, Pokemon, Pokemon>? OnAnyAttract { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAnyAccuracy { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyBasePower { get; }
        Action<Battle, Pokemon, IEffect>? OnAnyBeforeFaint { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyBeforeMove { get; }
        Action<Battle, Pokemon>? OnAnyBeforeSwitchIn { get; }
        Action<Battle, Pokemon>? OnAnyBeforeSwitchOut { get; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyTryBoost { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyChargeMove { get; }
        Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnAnyCriticalHit { get; }
        Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnAnyDamage { get; }
        Func<Battle, Pokemon, Pokemon, int?>? OnAnyDeductPp { get; }
        Action<Battle, Pokemon>? OnAnyDisableMove { get; }
        Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnAnyDragOut { get; }
        Action<Battle, Item, Pokemon>? OnAnyEatItem { get; }
        Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnAnyEffectiveness { get; }
        Action<Battle, Pokemon, Pokemon, IEffect>? OnAnyFaint { get; }
        Func<Battle, Pokemon, bool?>? OnAnyFlinch { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyHit { get; }
        Action<Battle, string, Pokemon>? OnAnyImmunity { get; }
        Func<Battle, Pokemon, string?>? OnAnyLockMove { get; }
        Action<Battle, Pokemon, Pokemon?>? OnAnyMaybeTrapPokemon { get; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifyAccuracy { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyAtk { get; }
        Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAnyModifyBoost { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyCritRatio { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamage { get; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifyDef { get; }
        Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnAnyModifyMove { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyPriority { get; }
        Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAnyModifySecondaries { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifySpA { get; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAnyModifySpD { get; }
        Func<Battle, int, Pokemon, int?>? OnAnyModifySpe { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyStab { get; }
        Action<Battle, ActiveMove, Pokemon, Pokemon>? OnAnyModifyType { get; }
        Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnAnyModifyTarget { get; }
        Func<Battle, int, Pokemon, int?>? OnAnyModifyWeight { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAnyMoveAborted { get; }
        Func<Battle, Pokemon, string, bool?>? OnAnyNegateImmunity { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnAnyOverrideAction { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyPrepareHit { get; }
        Action<Battle, Pokemon, Pokemon, Condition>? OnAnyPseudoWeatherChange { get; }
        Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnAnyRedirectTarget { get; }
        Action<Battle, object, Pokemon, IEffect>? OnAnyResidual { get; }
        Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnAnySetAbility { get; }
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnySetStatus { get; }
        Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnAnySetWeather { get; }
        Func<Battle, Pokemon, bool?>? OnAnyStallMove { get; }
        Action<Battle, Pokemon>? OnAnySwitchIn { get; }
        Action<Battle, Pokemon>? OnAnySwitchOut { get; }
        Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnAnyTakeItem { get; }
        Action<Battle, Pokemon>? OnAnyTerrain { get; }
        Action<Battle, Pokemon>? OnAnyTrapPokemon { get; }
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnyTryAddVolatile { get; }
        Func<Battle, Item, Pokemon, bool?>? OnAnyTryEatItem { get; }
        Func<Battle, object, object?, object?, object?, object?>? OnAnyTryHeal { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAnyTryHit { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryHitField { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryHitSide { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAnyInvulnerability { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAnyTryMove { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnAnyTryPrimaryHit { get; }
        Func<Battle, List<string>, Pokemon, List<string>?>? OnAnyType { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyWeatherModifyDamage { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamagePhase1 { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAnyModifyDamagePhase2 { get; }

        // Priorities (incomplete list)
        int? OnAccuracyPriority { get; }
        int? OnDamagingHitOrder { get; }
        int? OnAfterMoveSecondaryPriority { get; }
        int? OnAfterMoveSecondarySelfPriority { get; }
        int? OnAfterMoveSelfPriority { get; }
        int? OnAfterSetStatusPriority { get; }
        int? OnAnyBasePowerPriority { get; }
        int? OnAnyInvulnerabilityPriority { get; }
        int? OnAnyModifyAccuracyPriority { get; }
        int? OnAnyFaintPriority { get; }
        int? OnAnyPrepareHitPriority { get; }
        int? OnAnySwitchInPriority { get; }
        int? OnAnySwitchInSubOrder { get; }
        int? OnAllyBasePowerPriority { get; }
        int? OnAllyModifyAtkPriority { get; }
        int? OnAllyModifySpAPriority { get; }
        int? OnAllyModifySpDPriority { get; }
        int? OnAttractPriority { get; }
        int? OnBasePowerPriority { get; }
        int? OnBeforeMovePriority { get; }
        int? OnBeforeSwitchOutPriority { get; }
        int? OnChangeBoostPriority { get; }
        int? OnDamagePriority { get; }
        int? OnDragOutPriority { get; }
        int? OnEffectivenessPriority { get; }
        int? OnFoeBasePowerPriority { get; }
        int? OnFoeBeforeMovePriority { get; }
        int? OnFoeModifyDefPriority { get; }
        int? OnFoeModifySpDPriority { get; }
        int? OnFoeRedirectTargetPriority { get; }
        int? OnFoeTrapPokemonPriority { get; }
        int? OnFractionalPriorityPriority { get; }
        int? OnHitPriority { get; }
        int? OnInvulnerabilityPriority { get; }
        int? OnModifyAccuracyPriority { get; }
        int? OnModifyAtkPriority { get; }
        int? OnModifyCritRatioPriority { get; }
        int? OnModifyDefPriority { get; }
        int? OnModifyMovePriority { get; }
        int? OnModifyPriorityPriority { get; }
        int? OnModifySpAPriority { get; }
        int? OnModifySpDPriority { get; }
        int? OnModifySpePriority { get; }
        int? OnModifyStabPriority { get; }
        int? OnModifyTypePriority { get; }
        int? OnModifyWeightPriority { get; }
        int? OnRedirectTargetPriority { get; }
        int? OnResidualOrder { get; }
        int? OnResidualPriority { get; }
        int? OnResidualSubOrder { get; }
        int? OnSourceBasePowerPriority { get; }
        int? OnSourceInvulnerabilityPriority { get; }
        int? OnSourceModifyAccuracyPriority { get; }
        int? OnSourceModifyAtkPriority { get; }
        int? OnSourceModifyDamagePriority { get; }
        int? OnSourceModifySpAPriority { get; }
        int? OnSwitchInPriority { get; }
        int? OnSwitchInSubOrder { get; }
        int? OnTrapPokemonPriority { get; }
        int? OnTryBoostPriority { get; }
        int? OnTryEatItemPriority { get; }
        int? OnTryHealPriority { get; }
        int? OnTryHitPriority { get; }
        int? OnTryMovePriority { get; }
        int? OnTryPrimaryHitPriority { get; }
        int? OnTypePriority { get; }
    }

    public interface IPokemonEventMethods : IEventMethods
    {
        // Ally event handlers (triggered by ally Pokémon actions)
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAllyDamagingHit { get; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnAllyAfterEachBoost { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterHit { get; }
        Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAllyAfterSetStatus { get; }
        Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAllyAfterSubDamage { get; }
        Action<Battle, Pokemon>? OnAllyAfterSwitchInSelf { get; }
        Action<Battle, Item, Pokemon>? OnAllyAfterUseItem { get; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyAfterBoost { get; }
        Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAllyAfterFaint { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSecondarySelf { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSecondary { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMove { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyAfterMoveSelf { get; }
        Action<Battle, Pokemon, Pokemon>? OnAllyAttract { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAllyAccuracy { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyBasePower { get; }
        Action<Battle, Pokemon, IEffect>? OnAllyBeforeFaint { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyBeforeMove { get; }
        Action<Battle, Pokemon>? OnAllyBeforeSwitchIn { get; }
        Action<Battle, Pokemon>? OnAllyBeforeSwitchOut { get; }
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyTryBoost { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyChargeMove { get; }
        Func<Battle, Pokemon, object?, ActiveMove, bool?>? OnAllyCriticalHit { get; }
        Func<Battle, int, Pokemon, Pokemon, IEffect, object?>? OnAllyDamage { get; }
        Func<Battle, Pokemon, Pokemon, int?>? OnAllyDeductPp { get; }
        Action<Battle, Pokemon>? OnAllyDisableMove { get; }
        Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnAllyDragOut { get; }
        Action<Battle, Item, Pokemon>? OnAllyEatItem { get; }
        Func<Battle, int, Pokemon?, string, ActiveMove, int?>? OnAllyEffectiveness { get; }
        Action<Battle, Pokemon, Pokemon, IEffect>? OnAllyFaint { get; }
        Func<Battle, Pokemon, bool?>? OnAllyFlinch { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyHit { get; }
        Action<Battle, string, Pokemon>? OnAllyImmunity { get; }
        Func<Battle, Pokemon, string?>? OnAllyLockMove { get; }
        Action<Battle, Pokemon>? OnAllyMaybeTrapPokemon { get; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifyAccuracy { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyAtk { get; }
        Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAllyModifyBoost { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyCritRatio { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamage { get; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifyDef { get; }
        Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnAllyModifyMove { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyPriority { get; }
        Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAllyModifySecondaries { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifySpA { get; }
        Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifySpD { get; }
        Func<Battle, int, Pokemon, int?>? OnAllyModifySpe { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyStab { get; }
        Action<Battle, ActiveMove, Pokemon, Pokemon>? OnAllyModifyType { get; }
        Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnAllyModifyTarget { get; }
        Func<Battle, int, Pokemon, int?>? OnAllyModifyWeight { get; }
        Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyMoveAborted { get; }
        Func<Battle, Pokemon, string, bool?>? OnAllyNegateImmunity { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnAllyOverrideAction { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyPrepareHit { get; }
        Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnAllyRedirectTarget { get; }
        Action<Battle, object, Pokemon, IEffect>? OnAllyResidual { get; }
        Func<Battle, string, Pokemon, Pokemon, IEffect, bool?>? OnAllySetAbility { get; }
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllySetStatus { get; }
        Func<Battle, Pokemon, Pokemon, Condition, bool?>? OnAllySetWeather { get; }
        Func<Battle, Pokemon, bool?>? OnAllyStallMove { get; }
        Action<Battle, Pokemon>? OnAllySwitchOut { get; }
        Func<Battle, Item, Pokemon, Pokemon, ActiveMove?, bool?>? OnAllyTakeItem { get; }
        Action<Battle, Pokemon>? OnAllyTerrain { get; }
        Action<Battle, Pokemon>? OnAllyTrapPokemon { get; }
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllyTryAddVolatile { get; }
        Func<Battle, Item, Pokemon, bool?>? OnAllyTryEatItem { get; }
        Func<Battle, object, object?, object?, object?, object?>? OnAllyTryHeal { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAllyTryHit { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryHitField { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryHitSide { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAllyInvulnerability { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryMove { get; }
        Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnAllyTryPrimaryHit { get; }
        Func<Battle, List<string>, Pokemon, List<string>?>? OnAllyType { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyWeatherModifyDamage { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamagePhase1 { get; }
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamagePhase2 { get; }
    }

    public interface ISideEventMethods : IEventMethods
    {
        // Side condition lifecycle events
        Action<Battle, Side, Pokemon, IEffect>? OnSideStart { get; }
        Action<Battle, Side, Pokemon, IEffect>? OnSideRestart { get; }
        Action<Battle, Side, Pokemon, IEffect>? OnSideResidual { get; }
        Action<Battle, Side>? OnSideEnd { get; }

        // Side event priorities and ordering
        int? OnSideResidualOrder { get; }
        int? OnSideResidualPriority { get; }
        int? OnSideResidualSubOrder { get; }
    }

    public interface IFieldEventMethods : IEventMethods
    {
        // Field condition lifecycle events
        Action<Battle, Field, Pokemon, IEffect>? OnFieldStart { get; }
        Action<Battle, Field, Pokemon, IEffect>? OnFieldRestart { get; }
        Action<Battle, Field, Pokemon, IEffect>? OnFieldResidual { get; }
        Action<Battle, Field>? OnFieldEnd { get; }

        // Field event priorities and ordering
        int? OnFieldResidualOrder { get; }
        int? OnFieldResidualPriority { get; }
        int? OnFieldResidualSubOrder { get; }
    }

    public class PokemonConditionData : Condition, IConditionData; //, IPokemonEventMethods

    public class SideConditionData : Condition, IConditionData; //, ISideEventMethods

    public class FieldConditionData : Condition, IConditionData; //, IFieldEventMethods

    /// <summary>
    /// PokemonConditionData, SideConditionData, and FieldConditionData
    /// inherit from this interface.
    /// </summary>
    public interface IConditionData;

    public interface IModdedConditionData : IConditionData
    {
        public static bool Inherit => true;
    }

    public class ConditionDataTable : Dictionary<IdEntry, IConditionData>;

    public class ModdedConditionDataTable : Dictionary<IdEntry, IModdedConditionData>;


    public class Condition : BasicEffect, IBasicEffect, ISideEventMethods,
        IFieldEventMethods, IPokemonEventMethods, IEffect
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
        public Action<Battle, Side, Pokemon, IEffect>? OnSideStart { get; init; }
        public Action<Battle, Side, Pokemon, IEffect>? OnSideRestart { get; init; }
        public Action<Battle, Side, Pokemon, IEffect>? OnSideResidual { get; init; }
        public Action<Battle, Side>? OnSideEnd { get; init; }
        public int? OnSideResidualOrder { get; init; }
        public int? OnSideResidualPriority { get; init; }
        public int? OnSideResidualSubOrder { get; init; }
        public Action<Battle, Field, Pokemon, IEffect>? OnFieldStart { get; init; }
        public Action<Battle, Field, Pokemon, IEffect>? OnFieldRestart { get; init; }
        public Action<Battle, Field, Pokemon, IEffect>? OnFieldResidual { get; init; }
        public Action<Battle, Field>? OnFieldEnd { get; init; }
        public int? OnFieldResidualOrder { get; init; }
        public int? OnFieldResidualPriority { get; init; }
        public int? OnFieldResidualSubOrder { get; init; }
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
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, object?>? OnAllyAccuracy { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyBasePower { get; init; }
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
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifyAccuracy { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyAtk { get; init; }
        public Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAllyModifyBoost { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyCritRatio { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifyDef { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon?>? OnAllyModifyMove { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyPriority { get; init; }
        public Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAllyModifySecondaries { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifySpA { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, IActiveMove, int>? OnAllyModifySpD { get; init; }
        public Func<Battle, int, Pokemon, int?>? OnAllyModifySpe { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyStab { get; init; }
        public Action<Battle, ActiveMove, Pokemon, Pokemon>? OnAllyModifyType { get; init; }
        public Action<Battle, object, Pokemon, Pokemon, ActiveMove>? OnAllyModifyTarget { get; init; }
        public Func<Battle, int, Pokemon, int?>? OnAllyModifyWeight { get; init; }
        public Action<Battle, Pokemon, Pokemon, ActiveMove>? OnAllyMoveAborted { get; init; }
        public Func<Battle, Pokemon, string, bool?>? OnAllyNegateImmunity { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, string?>? OnAllyOverrideAction { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyPrepareHit { get; init; }
        public Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, Pokemon?>? OnAllyRedirectTarget { get; init; }
        public Action<Battle, object, Pokemon, IEffect>? OnAllyResidual { get; init; }
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
        public Func<Battle, object, object?, object?, object?, object?>? OnAllyTryHeal { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAllyTryHit { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryHitField { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryHitSide { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object>? OnAllyInvulnerability { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnAllyTryMove { get; init; }
        public Func<Battle, Pokemon, Pokemon, ActiveMove, object?>? OnAllyTryPrimaryHit { get; init; }
        public Func<Battle, List<string>, Pokemon, List<string>?>? OnAllyType { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyWeatherModifyDamage { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamagePhase1 { get; init; }
        public Func<Battle, int, Pokemon, Pokemon, ActiveMove, int>? OnAllyModifyDamagePhase2 { get; init; }

        public override required EffectType EffectType
        {
            get;
            init
            {
                if (value != EffectType.Condition &&
                    value != EffectType.Weather &
                    value != EffectType.Status &&
                    value != EffectType.Terrain)
                {
                    throw new ArgumentException("EffectType must be Condition, Weather, Status, or Terrain.",
                        nameof(value));
                }

                field = value;
            }
        }
    }

    public static class  ConditionConstants
    {
        public static readonly Condition EmptyCondition = new()
        {
            Name = "empty",
            EffectType = EffectType.Condition,
            Duration = 0,
            Fullname = string.Empty,
            Exists = false,
            Num = 0,
            Gen = 0,
            NoCopy = false,
            AffectsFainted = false,
            SourceEffect = string.Empty
        };
    }

    public class DexConditions(ModdedDex dex)
    {
        private ModdedDex Dex { get; } = dex;
        private readonly Dictionary<Id, Condition> _conditionCache = [];

        public Condition Get(string? name)
        {
            throw new NotImplementedException("Get method is not implemented yet.");
        }

        public Condition Get(IEffect? effect)
        {
            throw new NotImplementedException("Get method is not implemented yet.");
        }

        public Condition GetById(Id id)
        {
            throw new NotImplementedException("GetById method is not implemented yet.");
        }
    }
}
