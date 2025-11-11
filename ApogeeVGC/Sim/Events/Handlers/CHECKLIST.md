# Event Classes Creation Checklist

Based on `EventEnums.cs`, here are all the event classes to create:

## Completed (8/156)
- [x] OnDamagingHitEventInfo
- [x] OnBasePowerEventInfo
- [x] OnResidualEventInfo
- [x] OnBeforeMoveEventInfo
- [x] OnAfterSetStatusEventInfo
- [x] OnSetStatusEventInfo
- [x] OnDamageEventInfo
- [x] OnEmergencyExitEventInfo

## To Create (148 remaining)

### A
- [ ] OnAccuracyEventInfo
- [ ] OnAfterBoostEventInfo
- [ ] OnAfterEachBoostEventInfo
- [ ] OnAfterFaintEventInfo
- [ ] OnAfterHitEventInfo
- [ ] OnAfterMegaEventInfo
- [ ] OnAfterMoveEventInfo
- [ ] OnAfterMoveSecondaryEventInfo
- [ ] OnAfterMoveSecondarySelfEventInfo
- [ ] OnAfterMoveSelfEventInfo
- [ ] OnAfterSubDamageEventInfo
- [ ] OnAfterSwitchInSelfEventInfo
- [ ] OnAfterTakeItemEventInfo
- [ ] OnAfterTerastallizationEventInfo
- [ ] OnAfterUseItemEventInfo
- [ ] OnAllyBasePowerEventInfo
- [ ] OnAllyModifyAtkEventInfo
- [ ] OnAllyModifySpAEventInfo
- [ ] OnAllyModifySpDEventInfo
- [ ] OnAllyTryHitSideEventInfo
- [ ] OnAnyBasePowerEventInfo
- [ ] OnAnyFaintEventInfo
- [ ] OnAnyInvulnerabilityEventInfo
- [ ] OnAnyModifyAccuracyEventInfo
- [ ] OnAnyPrepareHitEventInfo
- [ ] OnAnySwitchInEventInfo
- [ ] OnAttractEventInfo

### B
- [ ] OnBasePowerCallbackEventInfo
- [ ] OnBeforeFaintEventInfo
- [ ] OnBeforeMoveCallbackEventInfo
- [ ] OnBeforeSwitchInEventInfo
- [ ] OnBeforeSwitchOutEventInfo
- [ ] OnBeforeTurnEventInfo
- [ ] OnBeforeTurnCallbackEventInfo
- [ ] OnBoostEventInfo

### C
- [ ] OnChangeBoostEventInfo
- [ ] OnChargeMoveEventInfo
- [ ] OnCheckShowEventInfo
- [ ] OnCopyEventInfo
- [ ] OnCriticalHitEventInfo

### D
- [ ] OnDamageCallbackEventInfo
- [ ] OnDeductPpEventInfo
- [ ] OnDisableMoveEventInfo
- [ ] OnDragOutEventInfo

### E
- [ ] OnEatEventInfo
- [ ] OnEatItemEventInfo
- [ ] OnEffectivenessEventInfo
- [ ] OnEndEventInfo
- [ ] OnEntryHazardEventInfo

### F
- [ ] OnFaintEventInfo
- [ ] OnFieldEndEventInfo
- [ ] OnFieldResidualEventInfo
- [ ] OnFieldRestartEventInfo
- [ ] OnFieldStartEventInfo
- [ ] OnFlinchEventInfo
- [ ] OnFoeBasePowerEventInfo
- [ ] OnFoeBeforeMoveEventInfo
- [ ] OnFoeMaybeTrapPokemonEventInfo
- [ ] OnFoeModifyDefEventInfo
- [ ] OnFoeModifySpDEventInfo
- [ ] OnFoeRedirectTargetEventInfo
- [ ] OnFoeTrapPokemonEventInfo
- [ ] OnFractionalPriorityEventInfo

### H
- [ ] OnHealEventInfo
- [ ] OnHitEventInfo
- [ ] OnHitFieldEventInfo
- [ ] OnHitSideEventInfo

### I
- [ ] OnImmunityEventInfo
- [ ] OnInvulnerabilityEventInfo

### L
- [ ] OnLockMoveEventInfo

### M
- [ ] OnMaybeTrapPokemonEventInfo
- [ ] OnModifyAccuracyEventInfo
- [ ] OnModifyAtkEventInfo
- [ ] OnModifyBoostEventInfo
- [ ] OnModifyCritRatioEventInfo
- [ ] OnModifyDamageEventInfo
- [ ] OnModifyDamagePhase1EventInfo
- [ ] OnModifyDamagePhase2EventInfo
- [ ] OnModifyDefEventInfo
- [ ] OnModifyMoveEventInfo
- [ ] OnModifyPriorityEventInfo
- [ ] OnModifySecondariesEventInfo
- [ ] OnModifySpAEventInfo
- [ ] OnModifySpDEventInfo
- [ ] OnModifySpecieEventInfo
- [ ] OnModifySpeEventInfo
- [ ] OnModifyStabEventInfo
- [ ] OnModifyTargetEventInfo
- [ ] OnModifyTypeEventInfo
- [ ] OnModifyWeightEventInfo
- [ ] OnMoveAbortedEventInfo
- [ ] OnMoveFailEventInfo

### N
- [ ] OnNegateImmunityEventInfo

### O
- [ ] OnOverrideActionEventInfo

### P
- [ ] OnPrepareHitEventInfo
- [ ] OnPriorityChargeCallbackEventInfo
- [ ] OnPseudoWeatherChangeEventInfo

### R
- [ ] OnRedirectTargetEventInfo
- [ ] OnRestartEventInfo

### S
- [ ] OnSetAbilityEventInfo
- [ ] OnSetWeatherEventInfo
- [ ] OnSideConditionStartEventInfo
- [ ] OnSideEndEventInfo
- [ ] OnSideResidualEventInfo
- [ ] OnSideRestartEventInfo
- [ ] OnSideStartEventInfo
- [ ] OnSourceBasePowerEventInfo
- [ ] OnSourceInvulnerabilityEventInfo
- [ ] OnSourceModifyAccuracyEventInfo
- [ ] OnSourceModifyAtkEventInfo
- [ ] OnSourceModifyDamageEventInfo
- [ ] OnSourceModifySpAEventInfo
- [ ] OnStallMoveEventInfo
- [ ] OnStartEventInfo
- [ ] OnSwapEventInfo
- [ ] OnSwitchInEventInfo
- [ ] OnSwitchOutEventInfo

### T
- [ ] OnTakeItemEventInfo
- [ ] OnTerrainEventInfo
- [ ] OnTerrainChangeEventInfo
- [ ] OnTrapPokemonEventInfo
- [ ] OnTryEventInfo
- [ ] OnTryAddVolatileEventInfo
- [ ] OnTryBoostEventInfo
- [ ] OnTryEatItemEventInfo
- [ ] OnTryHealEventInfo
- [ ] OnTryHitEventInfo
- [ ] OnTryHitFieldEventInfo
- [ ] OnTryHitSideEventInfo
- [ ] OnTryImmunityEventInfo
- [ ] OnTryMoveEventInfo
- [ ] OnTryPrimaryHitEventInfo
- [ ] OnTryTerrainEventInfo
- [ ] OnTypeEventInfo

### U
- [ ] OnUpdateEventInfo
- [ ] OnUseEventInfo
- [ ] OnUseItemEventInfo
- [ ] OnUseMoveMessageEventInfo

### W
- [ ] OnWeatherEventInfo
- [ ] OnWeatherChangeEventInfo
- [ ] OnWeatherModifyDamageEventInfo

## Priority by Usage

### High Priority (commonly used, create first)
1. OnStartEventInfo
2. OnEndEventInfo
3. OnSwitchInEventInfo
4. OnSwitchOutEventInfo
5. OnModifyAtkEventInfo
6. OnModifySpAEventInfo
7. OnModifyDefEventInfo
8. OnModifySpDEventInfo
9. OnModifySpeEventInfo
10. OnHitEventInfo
11. OnFaintEventInfo
12. OnTryHitEventInfo

### Medium Priority (moderately common)
- OnBoostEventInfo
- OnTryBoostEventInfo
- OnModifyMoveEventInfo
- OnWeatherChangeEventInfo
- OnTerrainChangeEventInfo
- OnCriticalHitEventInfo

### Lower Priority (specialized/rare)
- Callback events (OnBeforeTurnCallback, etc.)
- Field/Side events
- Foe/Source/Ally/Any variants
