# Missing EventHandlerInfo Records

The following EventHandlerInfo records are referenced in IEventMethodsV2.cs but do not exist yet:

## Foe Prefix - Missing Records (33)
- OnFoeBeforeSwitchOutEventInfo
- OnFoeDragOutEventInfo
- OnFoeEatItemEventInfo
- OnFoeEffectivenessEventInfo
- OnFoeFaintEventInfo
- OnFoeHitEventInfo
- OnFoeModifyAccuracyEventInfo
- OnFoeModifyAtkEventInfo
- OnFoeModifyBoostEventInfo
- OnFoeModifyDamageEventInfo
- OnFoeModifyMoveEventInfo
- OnFoeModifySecondariesEventInfo
- OnFoeModifySpAEventInfo
- OnFoeModifyTypeEventInfo
- OnFoeModifyTargetEventInfo
- OnFoeNegateImmunityEventInfo
- OnFoeOverrideActionEventInfo
- OnFoePrepareHitEventInfo
- OnFoeSetAbilityEventInfo
- OnFoeSetWeatherEventInfo
- OnFoeTakeItemEventInfo
- OnFoeTerrainEventInfo
- OnFoeTryAddVolatileEventInfo
- OnFoeTryEatItemEventInfo
- OnFoeTryHealEventInfo
- OnFoeTryHitEventInfo
- OnFoeTryHitSideEventInfo
- OnFoeInvulnerabilityEventInfo
- OnFoeTryMoveEventInfo
- OnFoeTryPrimaryHitEventInfo
- OnFoeTypeEventInfo
- OnFoeModifyDamagePhase1EventInfo
- OnFoeModifyDamagePhase2EventInfo

## Source Prefix - Missing Records (37)
- OnSourceAfterUseItemEventInfo
- OnSourceAfterBoostEventInfo
- OnSourceAfterFaintEventInfo
- OnSourceAfterMoveSecondarySelfEventInfo
- OnSourceBeforeFaintEventInfo
- OnSourceBeforeSwitchOutEventInfo
- OnSourceTryBoostEventInfo
- OnSourceDragOutEventInfo
- OnSourceEatItemEventInfo
- OnSourceEffectivenessEventInfo
- OnSourceFaintEventInfo
- OnSourceHitEventInfo
- OnSourceMaybeTrapPokemonEventInfo
- OnSourceModifyBoostEventInfo
- OnSourceModifyMoveEventInfo
- OnSourceModifySecondariesEventInfo
- OnSourceModifySpDEventInfo
- OnSourceModifyTypeEventInfo
- OnSourceModifyTargetEventInfo
- OnSourceNegateImmunityEventInfo
- OnSourceOverrideActionEventInfo
- OnSourceRedirectTargetEventInfo
- OnSourceSetAbilityEventInfo
- OnSourceSetStatusEventInfo
- OnSourceSetWeatherEventInfo
- OnSourceSwitchOutEventInfo
- OnSourceTakeItemEventInfo
- OnSourceTerrainEventInfo
- OnSourceTrapPokemonEventInfo
- OnSourceTryAddVolatileEventInfo
- OnSourceTryEatItemEventInfo
- OnSourceTryHealEventInfo
- OnSourceTryHitEventInfo
- OnSourceTryHitSideEventInfo
- OnSourceTryMoveEventInfo
- OnSourceTryPrimaryHitEventInfo
- OnSourceTypeEventInfo
- OnSourceModifyDamagePhase1EventInfo
- OnSourceModifyDamagePhase2EventInfo

## Any Prefix - Missing Records (49)
- OnAnyAfterUseItemEventInfo
- OnAnyAfterFaintEventInfo
- OnAnyAfterMoveSecondarySelfEventInfo
- OnAnyAccuracyEventInfo
- OnAnyBeforeFaintEventInfo
- OnAnyBeforeSwitchOutEventInfo
- OnAnyTryBoostEventInfo
- OnAnyDamageEventInfo
- OnAnyDragOutEventInfo
- OnAnyEatItemEventInfo
- OnAnyEffectivenessEventInfo
- OnAnyMaybeTrapPokemonEventInfo
- OnAnyModifyAtkEventInfo
- OnAnyModifyBoostEventInfo
- OnAnyModifyDamageEventInfo
- OnAnyModifyMoveEventInfo
- OnAnyModifySecondariesEventInfo
- OnAnyModifySpAEventInfo
- OnAnyModifySpDEventInfo
- OnAnyModifyTypeEventInfo
- OnAnyModifyTargetEventInfo
- OnAnyNegateImmunityEventInfo
- OnAnyOverrideActionEventInfo
- OnAnyPseudoWeatherChangeEventInfo
- OnAnyRedirectTargetEventInfo
- OnAnySetAbilityEventInfo
- OnAnySetStatusEventInfo
- OnAnySetWeatherEventInfo
- OnAnyTakeItemEventInfo
- OnAnyTerrainEventInfo
- OnAnyTrapPokemonEventInfo
- OnAnyTryAddVolatileEventInfo
- OnAnyTryEatItemEventInfo
- OnAnyTryHealEventInfo
- OnAnyTryHitSideEventInfo
- OnAnyTryPrimaryHitEventInfo
- OnAnyTypeEventInfo
- OnAnyModifyDamagePhase1EventInfo
- OnAnyModifyDamagePhase2EventInfo

## Total Missing: 119 records

These can be generated using the PowerShell scripts in the `Scripts\` directory.

To generate all missing records, run:
```powershell
.\Scripts\GenerateEventHandlerInfoRecords-Simple.ps1
```

This will create all missing EventHandlerInfo record files following the established pattern.
