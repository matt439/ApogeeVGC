#pragma once

#include "global-types.h"
#include "dex-moves.h"
#include <functional>
#include <optional>
#include <variant>
#include <string>
#include <vector>

using OnDamagingHitFunc = std::function<void(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;
using OnEmergencyExitFunc = std::function<void(Battle*, Pokemon*)>;
using OnAfterEachBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*, Effect)>;
using OnAfterMegaFunc = std::function<void(Battle*, Pokemon*)>;
using OnAfterSetStatusFunc = std::function<void(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
using OnAfterSwitchInSelfFunc = std::function<void(Battle*, Pokemon*)>;
using OnAfterTerastallizationFunc = std::function<void(Battle*, Pokemon*)>;
using OnAfterUseItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
using OnAfterTakeItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
using OnAfterBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*, Effect)>;
using OnAfterFaintFunc = std::function<void(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using OnAttractFunc = std::function<void(Battle*, Pokemon*, Pokemon*)>;
using OnAccuracyFunc = std::function<std::variant<int, bool, std::monostate>(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;
using OnBasePowerFunc = ModifierSourceMoveFunc; // Already defined elsewhere
using OnBeforeFaintFunc = std::function<void(Battle*, Pokemon*, Effect)>;
using OnBeforeSwitchInFunc = std::function<void(Battle*, Pokemon*)>;
using OnBeforeSwitchOutFunc = std::function<void(Battle*, Pokemon*)>;
using OnBeforeTurnFunc = std::function<void(Battle*, Pokemon*)>;
using OnChangeBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*, Effect)>;
using OnTryBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*, Effect)>;
using OnCriticalHitFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, std::nullptr_t, ActiveMove*)>>;
using OnDamageFunc = std::function<std::variant<int, bool, std::monostate>(Battle*, int, Pokemon*, Pokemon*, Effect)>;

using OnDeductPPFunc = std::function<std::variant<int, std::monostate>(Battle*, Pokemon*, Pokemon*)>;
using OnDisableMoveFunc = std::function<void(Battle*, Pokemon*)>;
using OnDragOutFunc = std::function<void(Battle*, Pokemon*, std::optional<Pokemon*>, std::optional<ActiveMove*>)>;
using OnEatItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
using OnEffectivenessFunc = OnEffectivenessFunc; // from MoveEventMethods
using OnEntryHazardFunc = std::function<void(Battle*, Pokemon*)>;
using OnFaintFunc = VoidEffectFunc; // from CommonHandlers
using OnFlinchFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*)>>;
using OnFractionalPriorityFunc = std::variant<ModifierSourceMoveFunc, double>; // -0.1 or function
using OnHitFunc = OnHitFunc; // from MoveEventMethods
using OnImmunityFunc = std::function<void(Battle*, std::string, Pokemon*)>;
using OnLockMoveFunc = std::variant<std::string, std::function<std::variant<void, std::string>(Battle*, Pokemon*)>>;
using OnMaybeTrapPokemonFunc = std::function<void(Battle*, Pokemon*)>;
using OnModifyAccuracyFunc = ModifierMoveFunc; // from CommonHandlers
using OnModifyAtkFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnModifyBoostFunc = std::function<std::variant<SparseBoostsTable, std::monostate>(Battle*, const SparseBoostsTable&, Pokemon*)>;
using OnModifyCritRatioFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnModifyDamageFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnModifyDefFunc = ModifierMoveFunc; // from CommonHandlers
using OnModifyMoveFunc = OnModifyMoveFunc; // from MoveEventMethods
using OnModifyPriorityFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnModifySecondariesFunc = std::function<void(Battle*, std::vector<SecondaryEffect>, Pokemon*, Pokemon*, ActiveMove*)>;
using OnModifyTypeFunc = OnModifyTypeFunc; // from MoveEventMethods
using OnModifyTargetFunc = OnModifyTargetFunc; // from MoveEventMethods
using OnModifySpAFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnModifySpDFunc = ModifierMoveFunc; // from CommonHandlers
using OnModifySpeFunc = std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*)>;
using OnModifySTABFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnModifyWeightFunc = std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*)>;
using OnMoveAbortedFunc = VoidMoveFunc; // from CommonHandlers
using OnNegateImmunityFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, std::string)>>;
using OnOverrideActionFunc = std::function<std::variant<std::string, std::monostate>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnPrepareHitFunc = ResultSourceMoveFunc; // from CommonHandlers
using OnPseudoWeatherChangeFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Condition*)>;
using OnRedirectTargetFunc = std::function<std::variant<Pokemon*, std::monostate>(Battle*, Pokemon*, Pokemon*, Effect, ActiveMove*)>;
using OnResidualFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using OnSetAbilityFunc = std::function<std::variant<std::monostate, void>(Battle*, std::string, Pokemon*, Pokemon*, Effect)>;
using OnSetStatusFunc = std::function<std::variant<bool, std::monostate>(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;

using OnSetWeatherFunc = std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, Pokemon*, Condition*)>;
using OnSideConditionStartFunc = std::function<void(Battle*, Side*, Pokemon*, Condition*)>;
using OnStallMoveFunc = std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*)>;
using OnSwitchInFunc = std::function<void(Battle*, Pokemon*)>;
using OnSwitchOutFunc = std::function<void(Battle*, Pokemon*)>;
using OnSwapFunc = std::function<void(Battle*, Pokemon*, Pokemon*)>;
using OnTakeItemFunc = std::variant<std::function<std::variant<bool, std::monostate>(Battle*, Item*, Pokemon*, Pokemon*, std::optional<ActiveMove*>)>, bool>;
using OnWeatherChangeFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using OnTerrainChangeFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using OnTrapPokemonFunc = std::function<void(Battle*, Pokemon*)>;
using OnTryAddVolatileFunc = std::function<std::variant<bool, std::nullptr_t, std::monostate>(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
using OnTryEatItemFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Item*, Pokemon*)>>;
using OnTryHealFunc = std::function<std::variant<int, bool, std::nullptr_t, std::monostate>(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using OnTryPrimaryHitFunc = std::function<std::variant<bool, std::nullptr_t, int, std::monostate>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnTypeFunc = std::function<std::variant<std::vector<std::string>, std::monostate>(Battle*, std::vector<std::string>, Pokemon*)>;
using OnUseItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
using OnUpdateFunc = std::function<void(Battle*, Pokemon*)>;
using OnWeatherFunc = std::function<void(Battle*, Pokemon*, std::nullptr_t, Condition*)>;

using OnWeatherModifyDamageFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnModifyDamagePhase1Func = ModifierSourceMoveFunc; // from CommonHandlers
using OnModifyDamagePhase2Func = ModifierSourceMoveFunc; // from CommonHandlers
using OnFoeDamagingHitFunc = std::function<void(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;
using OnFoeAfterEachBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*)>;
using OnFoeAfterHitFunc = OnAfterHitFunc; // from MoveEventMethods
using OnFoeAfterSetStatusFunc = std::function<void(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
using OnFoeAfterSubDamageFunc = OnAfterSubDamageFunc; // from MoveEventMethods
using OnFoeAfterSwitchInSelfFunc = std::function<void(Battle*, Pokemon*)>;
using OnFoeAfterUseItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
using OnFoeAfterBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*, Effect)>;
using OnFoeAfterFaintFunc = std::function<void(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using OnFoeAfterMoveSecondarySelfFunc = OnAfterMoveSecondarySelfFunc; // from MoveEventMethods
using OnFoeAfterMoveSecondaryFunc = OnAfterMoveSecondaryFunc; // from MoveEventMethods
using OnFoeAfterMoveFunc = OnAfterMoveFunc; // from MoveEventMethods
using OnFoeAfterMoveSelfFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnFoeAttractFunc = std::function<void(Battle*, Pokemon*, Pokemon*)>;
using OnFoeAccuracyFunc = std::function<std::variant<int, bool, std::monostate>(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;

using OnFoeBasePowerFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnFoeBeforeFaintFunc = std::function<void(Battle*, Pokemon*, Effect)>;
using OnFoeBeforeMoveFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnFoeBeforeSwitchInFunc = std::function<void(Battle*, Pokemon*)>;
using OnFoeBeforeSwitchOutFunc = std::function<void(Battle*, Pokemon*)>;
using OnFoeTryBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*, Effect)>;
using OnFoeChargeMoveFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnFoeCriticalHitFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, std::nullptr_t, ActiveMove*)>>;
using OnFoeDamageFunc = std::function<std::variant<int, bool, std::monostate>(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using OnFoeDeductPPFunc = std::function<std::variant<int, std::monostate>(Battle*, Pokemon*, Pokemon*)>;
using OnFoeDisableMoveFunc = std::function<void(Battle*, Pokemon*)>;
using OnFoeDragOutFunc = std::function<void(Battle*, Pokemon*, std::optional<Pokemon*>, std::optional<ActiveMove*>)>;
using OnFoeEatItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
using OnFoeEffectivenessFunc = OnEffectivenessFunc; // from MoveEventMethods
using OnFoeFaintFunc = VoidEffectFunc; // from CommonHandlers
using OnFoeFlinchFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*)>>;
using OnFoeHitFunc = OnHitFunc; // from MoveEventMethods
using OnFoeImmunityFunc = std::function<void(Battle*, std::string, Pokemon*)>;
using OnFoeLockMoveFunc = std::variant<std::string, std::function<std::variant<void, std::string>(Battle*, Pokemon*)>>;
using OnFoeMaybeTrapPokemonFunc = std::function<void(Battle*, Pokemon*)>;
using OnFoeModifyAccuracyFunc = ModifierMoveFunc; // from CommonHandlers
using OnFoeModifyAtkFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnFoeModifyBoostFunc = std::function<std::variant<SparseBoostsTable, std::monostate>(Battle*, const SparseBoostsTable&, Pokemon*)>;
using OnFoeModifyCritRatioFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnFoeModifyDamageFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnFoeModifyDefFunc = ModifierMoveFunc; // from CommonHandlers
using OnFoeModifyMoveFunc = OnModifyMoveFunc; // from MoveEventMethods
using OnFoeModifyPriorityFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnFoeModifySecondariesFunc = std::function<void(Battle*, std::vector<SecondaryEffect>, Pokemon*, Pokemon*, ActiveMove*)>;

using OnFoeModifySpAFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnFoeModifySpDFunc = ModifierMoveFunc; // from CommonHandlers
using OnFoeModifySpeFunc = std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*)>;
using OnFoeModifySTABFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnFoeModifyTypeFunc = OnModifyTypeFunc; // from MoveEventMethods
using OnFoeModifyTargetFunc = OnModifyTargetFunc; // from MoveEventMethods
using OnFoeModifyWeightFunc = std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*)>;
using OnFoeMoveAbortedFunc = VoidMoveFunc; // from CommonHandlers
using OnFoeNegateImmunityFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, std::string)>>;
using OnFoeOverrideActionFunc = std::function<std::variant<std::string, std::monostate>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnFoePrepareHitFunc = ResultSourceMoveFunc; // from CommonHandlers
using OnFoeRedirectTargetFunc = std::function<std::variant<Pokemon*, std::monostate>(Battle*, Pokemon*, Pokemon*, Effect, ActiveMove*)>;
using OnFoeResidualFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using OnFoeSetAbilityFunc = std::function<std::variant<std::monostate, void>(Battle*, std::string, Pokemon*, Pokemon*, Effect)>;
using OnFoeSetStatusFunc = std::function<std::variant<bool, std::monostate>(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
using OnFoeSetWeatherFunc = std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, Pokemon*, Condition*)>;
using OnFoeSideConditionStartFunc = std::function<void(Battle*, Side*, Pokemon*, Condition*)>;
using OnFoeStallMoveFunc = std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*)>;
using OnFoeSwitchOutFunc = std::function<void(Battle*, Pokemon*)>;
using OnFoeSwapFunc = std::function<void(Battle*, Pokemon*, Pokemon*)>;
using OnFoeTakeItemFunc = std::variant<std::function<std::variant<bool, std::monostate>(Battle*, Item*, Pokemon*, Pokemon*, std::optional<ActiveMove*>)>, bool>;
using OnFoeWeatherChangeFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using OnFoeTerrainChangeFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using OnFoeTrapPokemonFunc = std::function<void(Battle*, Pokemon*)>;
using OnFoeTryAddVolatileFunc = std::function<std::variant<bool, std::nullptr_t, std::monostate>(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
using OnFoeTryEatItemFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Item*, Pokemon*)>>;
using OnFoeTryHealFunc = std::function<std::variant<int, bool, std::nullptr_t, std::monostate>(Battle*, int, Pokemon*, Pokemon*, Effect)>;

using OnFoeTryHitFunc = OnTryHitFunc; // from MoveEventMethods
using OnFoeTryHitFieldFunc = OnTryHitFieldFunc; // from MoveEventMethods
using OnFoeTryHitSideFunc = ResultMoveFunc; // from CommonHandlers
using OnFoeInvulnerabilityFunc = ExtResultMoveFunc; // from CommonHandlers
using OnFoeTryMoveFunc = OnTryMoveFunc; // from MoveEventMethods
using OnFoeTryPrimaryHitFunc = std::function<std::variant<bool, std::nullptr_t, int, std::monostate>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnFoeTypeFunc = std::function<std::variant<std::vector<std::string>, std::monostate>(Battle*, std::vector<std::string>, Pokemon*)>;
using OnFoeWeatherModifyDamageFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnFoeModifyDamagePhase1Func = ModifierSourceMoveFunc; // from CommonHandlers
using OnFoeModifyDamagePhase2Func = ModifierSourceMoveFunc; // from CommonHandlers
using OnSourceDamagingHitFunc = std::function<void(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;
using OnSourceAfterEachBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*)>;
using OnSourceAfterHitFunc = OnAfterHitFunc; // from MoveEventMethods
using OnSourceAfterSetStatusFunc = std::function<void(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
using OnSourceAfterSubDamageFunc = OnAfterSubDamageFunc; // from MoveEventMethods
using OnSourceAfterSwitchInSelfFunc = std::function<void(Battle*, Pokemon*)>;
using OnSourceAfterUseItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
using OnSourceAfterBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*, Effect)>;
using OnSourceAfterFaintFunc = std::function<void(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using OnSourceAfterMoveSecondarySelfFunc = OnAfterMoveSecondarySelfFunc; // from MoveEventMethods
using OnSourceAfterMoveSecondaryFunc = OnAfterMoveSecondaryFunc; // from MoveEventMethods
using OnSourceAfterMoveFunc = OnAfterMoveFunc; // from MoveEventMethods
using OnSourceAfterMoveSelfFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnSourceAttractFunc = std::function<void(Battle*, Pokemon*, Pokemon*)>;
using OnSourceAccuracyFunc = std::function<std::variant<int, bool, std::monostate>(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;

using OnSourceBasePowerFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnSourceBeforeFaintFunc = std::function<void(Battle*, Pokemon*, Effect)>;
using OnSourceBeforeMoveFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnSourceBeforeSwitchInFunc = std::function<void(Battle*, Pokemon*)>;
using OnSourceBeforeSwitchOutFunc = std::function<void(Battle*, Pokemon*)>;
using OnSourceTryBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*, Effect)>;
using OnSourceChargeMoveFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnSourceCriticalHitFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, std::nullptr_t, ActiveMove*)>>;
using OnSourceDamageFunc = std::function<std::variant<int, bool, std::monostate>(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using OnSourceDeductPPFunc = std::function<std::variant<int, std::monostate>(Battle*, Pokemon*, Pokemon*)>;
using OnSourceDisableMoveFunc = std::function<void(Battle*, Pokemon*)>;
using OnSourceDragOutFunc = std::function<void(Battle*, Pokemon*, std::optional<Pokemon*>, std::optional<ActiveMove*>)>;
using OnSourceEatItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
using OnSourceEffectivenessFunc = OnEffectivenessFunc; // from MoveEventMethods
using OnSourceFaintFunc = VoidEffectFunc; // from CommonHandlers

using OnSourceFlinchFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*)>>;
using OnSourceHitFunc = OnHitFunc; // from MoveEventMethods
using OnSourceImmunityFunc = std::function<void(Battle*, std::string, Pokemon*)>;
using OnSourceLockMoveFunc = std::variant<std::string, std::function<std::variant<void, std::string>(Battle*, Pokemon*)>>;
using OnSourceMaybeTrapPokemonFunc = std::function<void(Battle*, Pokemon*)>;
using OnSourceModifyAccuracyFunc = ModifierMoveFunc; // from CommonHandlers
using OnSourceModifyAtkFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnSourceModifyBoostFunc = std::function<std::variant<SparseBoostsTable, std::monostate>(Battle*, const SparseBoostsTable&, Pokemon*)>;
using OnSourceModifyCritRatioFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnSourceModifyDamageFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnSourceModifyDefFunc = ModifierMoveFunc; // from CommonHandlers
using OnSourceModifyMoveFunc = OnModifyMoveFunc; // from MoveEventMethods
using OnSourceModifyPriorityFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnSourceModifySecondariesFunc = std::function<void(Battle*, std::vector<SecondaryEffect>, Pokemon*, Pokemon*, ActiveMove*)>;

using OnSourceModifySpAFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnSourceModifySpDFunc = ModifierMoveFunc; // from CommonHandlers
using OnSourceModifySpeFunc = std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*)>;
using OnSourceModifySTABFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnSourceModifyTypeFunc = OnModifyTypeFunc; // from MoveEventMethods
using OnSourceModifyTargetFunc = OnModifyTargetFunc; // from MoveEventMethods
using OnSourceModifyWeightFunc = std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*)>;
using OnSourceMoveAbortedFunc = VoidMoveFunc; // from CommonHandlers
using OnSourceNegateImmunityFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, std::string)>>;
using OnSourceOverrideActionFunc = std::function<std::variant<std::string, std::monostate>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnSourcePrepareHitFunc = ResultSourceMoveFunc; // from CommonHandlers
using OnSourceRedirectTargetFunc = std::function<std::variant<Pokemon*, std::monostate>(Battle*, Pokemon*, Pokemon*, Effect, ActiveMove*)>;
using OnSourceResidualFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using OnSourceSetAbilityFunc = std::function<std::variant<std::monostate, void>(Battle*, std::string, Pokemon*, Pokemon*, Effect)>;
using OnSourceSetStatusFunc = std::function<std::variant<bool, std::monostate>(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;

using OnSourceSetWeatherFunc = std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, Pokemon*, Condition*)>;
using OnSourceStallMoveFunc = std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*)>;
using OnSourceSwitchOutFunc = std::function<void(Battle*, Pokemon*)>;
using OnSourceTakeItemFunc = std::variant<std::function<std::variant<bool, std::monostate>(Battle*, Item*, Pokemon*, Pokemon*, std::optional<ActiveMove*>)>, bool>;
using OnSourceTerrainFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using OnSourceTrapPokemonFunc = std::function<void(Battle*, Pokemon*)>;
using OnSourceTryAddVolatileFunc = std::function<std::variant<bool, std::nullptr_t, std::monostate>(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
using OnSourceTryEatItemFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Item*, Pokemon*)>>;
using OnSourceTryHealFunc = std::function<std::variant<int, bool, std::nullptr_t, std::monostate>(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using OnSourceTryHitFunc = OnTryHitFunc; // from MoveEventMethods
using OnSourceTryHitFieldFunc = OnTryHitFieldFunc; // from MoveEventMethods
using OnSourceTryHitSideFunc = ResultMoveFunc; // from CommonHandlers
using OnSourceInvulnerabilityFunc = ExtResultMoveFunc; // from CommonHandlers
using OnSourceTryMoveFunc = OnTryMoveFunc; // from MoveEventMethods
using OnSourceTryPrimaryHitFunc = std::function<std::variant<bool, std::nullptr_t, int, std::monostate>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;

using OnSourceTypeFunc = std::function<std::variant<std::vector<std::string>, std::monostate>(Battle*, std::vector<std::string>, Pokemon*)>;
using OnSourceWeatherModifyDamageFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnSourceModifyDamagePhase1Func = ModifierSourceMoveFunc; // from CommonHandlers
using OnSourceModifyDamagePhase2Func = ModifierSourceMoveFunc; // from CommonHandlers
using OnAnyDamagingHitFunc = std::function<void(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;
using OnAnyAfterEachBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*)>;
using OnAnyAfterHitFunc = OnAfterHitFunc; // from MoveEventMethods
using OnAnyAfterSetStatusFunc = std::function<void(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
using OnAnyAfterSubDamageFunc = OnAfterSubDamageFunc; // from MoveEventMethods
using OnAnyAfterSwitchInSelfFunc = std::function<void(Battle*, Pokemon*)>;
using OnAnyAfterUseItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
using OnAnyAfterBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*, Effect)>;
using OnAnyAfterFaintFunc = std::function<void(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using OnAnyAfterMegaFunc = std::function<void(Battle*, Pokemon*)>;

using OnAnyAfterMoveSecondarySelfFunc = OnAfterMoveSecondarySelfFunc; // from MoveEventMethods
using OnAnyAfterMoveSecondaryFunc = OnAfterMoveSecondaryFunc; // from MoveEventMethods
using OnAnyAfterMoveFunc = OnAfterMoveFunc; // from MoveEventMethods
using OnAnyAfterMoveSelfFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnAnyAttractFunc = std::function<void(Battle*, Pokemon*, Pokemon*)>;
using OnAnyAccuracyFunc = std::function<std::variant<int, bool, std::monostate>(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;
using OnAnyBasePowerFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnAnyBeforeFaintFunc = std::function<void(Battle*, Pokemon*, Effect)>;
using OnAnyBeforeMoveFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnAnyBeforeSwitchInFunc = std::function<void(Battle*, Pokemon*)>;
using OnAnyBeforeSwitchOutFunc = std::function<void(Battle*, Pokemon*)>;
using OnAnyTryBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*, Effect)>;
using OnAnyChargeMoveFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnAnyCriticalHitFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, std::nullptr_t, ActiveMove*)>>;

using OnAnyDamageFunc = std::function<std::variant<int, bool, std::monostate>(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using OnAnyDeductPPFunc = std::function<std::variant<int, std::monostate>(Battle*, Pokemon*, Pokemon*)>;
using OnAnyDisableMoveFunc = std::function<void(Battle*, Pokemon*)>;
using OnAnyDragOutFunc = std::function<void(Battle*, Pokemon*, std::optional<Pokemon*>, std::optional<ActiveMove*>)>;
using OnAnyEatItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
using OnAnyEffectivenessFunc = OnEffectivenessFunc; // from MoveEventMethods
using OnAnyFaintFunc = VoidEffectFunc; // from CommonHandlers
using OnAnyFlinchFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*)>>;
using OnAnyHitFunc = OnHitFunc; // from MoveEventMethods
using OnAnyImmunityFunc = std::function<void(Battle*, std::string, Pokemon*)>;
using OnAnyLockMoveFunc = std::variant<std::string, std::function<std::variant<void, std::string>(Battle*, Pokemon*)>>;
using OnAnyMaybeTrapPokemonFunc = std::function<void(Battle*, Pokemon*)>;
using OnAnyModifyAccuracyFunc = ModifierMoveFunc; // from CommonHandlers
using OnAnyModifyAtkFunc = ModifierSourceMoveFunc; // from CommonHandlers

using OnAnyModifyBoostFunc = std::function<std::variant<SparseBoostsTable, std::monostate>(Battle*, const SparseBoostsTable&, Pokemon*)>;
using OnAnyModifyCritRatioFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnAnyModifyDamageFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnAnyModifyDefFunc = ModifierMoveFunc; // from CommonHandlers
using OnAnyModifyMoveFunc = OnModifyMoveFunc; // from MoveEventMethods
using OnAnyModifyPriorityFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnAnyModifySecondariesFunc = std::function<void(Battle*, std::vector<SecondaryEffect>, Pokemon*, Pokemon*, ActiveMove*)>;
using OnAnyModifySpAFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnAnyModifySpDFunc = ModifierMoveFunc; // from CommonHandlers
using OnAnyModifySpeFunc = std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*)>;
using OnAnyModifySTABFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnAnyModifyTypeFunc = OnModifyTypeFunc; // from MoveEventMethods
using OnAnyModifyTargetFunc = OnModifyTargetFunc; // from MoveEventMethods
using OnAnyModifyWeightFunc = std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*)>;
using OnAnyMoveAbortedFunc = VoidMoveFunc; // from CommonHandlers

using OnAnyNegateImmunityFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, std::string)>>;
using OnAnyOverrideActionFunc = std::function<std::variant<std::string, std::monostate>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnAnyPrepareHitFunc = ResultSourceMoveFunc; // from CommonHandlers
using OnAnyPseudoWeatherChangeFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Condition*)>;
using OnAnyRedirectTargetFunc = std::function<std::variant<Pokemon*, std::monostate>(Battle*, Pokemon*, Pokemon*, Effect, ActiveMove*)>;
using OnAnyResidualFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using OnAnySetAbilityFunc = std::function<std::variant<std::monostate, void>(Battle*, std::string, Pokemon*, Pokemon*, Effect)>;
using OnAnySetStatusFunc = std::function<std::variant<bool, std::monostate>(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
using OnAnySetWeatherFunc = std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, Pokemon*, Condition*)>;
using OnAnyStallMoveFunc = std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*)>;
using OnAnySwitchInFunc = std::function<void(Battle*, Pokemon*)>;
using OnAnySwitchOutFunc = std::function<void(Battle*, Pokemon*)>;
using OnAnyTakeItemFunc = std::variant<std::function<std::variant<bool, std::monostate>(Battle*, Item*, Pokemon*, Pokemon*, std::optional<ActiveMove*>)>, bool>;

using OnAnyTerrainFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using OnAnyTrapPokemonFunc = std::function<void(Battle*, Pokemon*)>;
using OnAnyTryAddVolatileFunc = std::function<std::variant<bool, std::nullptr_t, std::monostate>(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
using OnAnyTryEatItemFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Item*, Pokemon*)>>;
using OnAnyTryHealFunc = std::function<std::variant<int, bool, std::nullptr_t, std::monostate>(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using OnAnyTryHitFunc = OnTryHitFunc; // from MoveEventMethods
using OnAnyTryHitFieldFunc = OnTryHitFieldFunc; // from MoveEventMethods
using OnAnyTryHitSideFunc = ResultMoveFunc; // from CommonHandlers
using OnAnyInvulnerabilityFunc = ExtResultMoveFunc; // from CommonHandlers
using OnAnyTryMoveFunc = OnTryMoveFunc; // from MoveEventMethods
using OnAnyTryPrimaryHitFunc = std::function<std::variant<bool, std::nullptr_t, int, std::monostate>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnAnyTypeFunc = std::function<std::variant<std::vector<std::string>, std::monostate>(Battle*, std::vector<std::string>, Pokemon*)>;
using OnAnyWeatherModifyDamageFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnAnyModifyDamagePhase1Func = ModifierSourceMoveFunc; // from CommonHandlers
using OnAnyModifyDamagePhase2Func = ModifierSourceMoveFunc; // from CommonHandlers

struct EventMethods {
    std::optional<OnDamagingHitFunc> on_damaging_hit;
    std::optional<OnEmergencyExitFunc> on_emergency_exit;
    std::optional<OnAfterEachBoostFunc> on_after_each_boost;
    std::optional<OnAfterHitFunc> on_after_hit;
    std::optional<OnAfterMegaFunc> on_after_mega;
    std::optional<OnAfterSetStatusFunc> on_after_set_status;
    std::optional<OnAfterSubDamageFunc> on_after_sub_damage;
    std::optional<OnAfterSwitchInSelfFunc> on_after_switch_in_self;
    std::optional<OnAfterTerastallizationFunc> on_after_terastallization;
    std::optional<OnAfterUseItemFunc> on_after_use_item;
    std::optional<OnAfterTakeItemFunc> on_after_take_item;
    std::optional<OnAfterBoostFunc> on_after_boost;
    std::optional<OnAfterFaintFunc> on_after_faint;
    std::optional<OnAfterMoveSecondarySelfFunc> on_after_move_secondary_self;
    std::optional<OnAfterMoveSecondaryFunc> on_after_move_secondary;
    std::optional<OnAfterMoveFunc> on_after_move;
    std::optional<VoidSourceMoveFunc> on_after_move_self;
    std::optional<OnAttractFunc> on_attract;
    std::optional<OnAccuracyFunc> on_accuracy;
    std::optional<OnBasePowerFunc> on_base_power;
    std::optional<OnBeforeFaintFunc> on_before_faint;
    std::optional<VoidSourceMoveFunc> on_before_move;
    std::optional<OnBeforeSwitchInFunc> on_before_switch_in;
    std::optional<OnBeforeSwitchOutFunc> on_before_switch_out;
    std::optional<OnBeforeTurnFunc> on_before_turn;
    std::optional<OnChangeBoostFunc> on_change_boost;
    std::optional<OnTryBoostFunc> on_try_boost;
    std::optional<VoidSourceMoveFunc> on_charge_move;
    std::optional<OnCriticalHitFunc> on_critical_hit;
    std::optional<OnDamageFunc> on_damage;

    std::optional<OnDeductPPFunc> on_deduct_pp;
    std::optional<OnDisableMoveFunc> on_disable_move;
    std::optional<OnDragOutFunc> on_drag_out;
    std::optional<OnEatItemFunc> on_eat_item;
    std::optional<OnEffectivenessFunc> on_effectiveness;
    std::optional<OnEntryHazardFunc> on_entry_hazard;
    std::optional<OnFaintFunc> on_faint;
    std::optional<OnFlinchFunc> on_flinch;
    std::optional<OnFractionalPriorityFunc> on_fractional_priority;
    std::optional<OnHitFunc> on_hit;
    std::optional<OnImmunityFunc> on_immunity;
    std::optional<OnLockMoveFunc> on_lock_move;
    std::optional<OnMaybeTrapPokemonFunc> on_maybe_trap_pokemon;
    std::optional<OnModifyAccuracyFunc> on_modify_accuracy;
    std::optional<OnModifyAtkFunc> on_modify_atk;
    std::optional<OnModifyBoostFunc> on_modify_boost;
    std::optional<OnModifyCritRatioFunc> on_modify_crit_ratio;
    std::optional<OnModifyDamageFunc> on_modify_damage;
    std::optional<OnModifyDefFunc> on_modify_def;
    std::optional<OnModifyMoveFunc> on_modify_move;
    std::optional<OnModifyPriorityFunc> on_modify_priority;
    std::optional<OnModifySecondariesFunc> on_modify_secondaries;
    std::optional<OnModifyTypeFunc> on_modify_type;
    std::optional<OnModifyTargetFunc> on_modify_target;
    std::optional<OnModifySpAFunc> on_modify_spa;
    std::optional<OnModifySpDFunc> on_modify_spd;
    std::optional<OnModifySpeFunc> on_modify_spe;
    std::optional<OnModifySTABFunc> on_modify_stab;
    std::optional<OnModifyWeightFunc> on_modify_weight;
    std::optional<OnMoveAbortedFunc> on_move_aborted;
    std::optional<OnNegateImmunityFunc> on_negate_immunity;
    std::optional<OnOverrideActionFunc> on_override_action;
    std::optional<OnPrepareHitFunc> on_prepare_hit;
    std::optional<OnPseudoWeatherChangeFunc> on_pseudo_weather_change;
    std::optional<OnRedirectTargetFunc> on_redirect_target;
    std::optional<OnResidualFunc> on_residual;
    std::optional<OnSetAbilityFunc> on_set_ability;
    std::optional<OnSetStatusFunc> on_set_status;

	std::optional<OnSetWeatherFunc> on_set_weather;
	std::optional<OnSideConditionStartFunc> on_side_condition_start;
	std::optional<OnStallMoveFunc> on_stall_move;
	std::optional<OnSwitchInFunc> on_switch_in;
	std::optional<OnSwitchOutFunc> on_switch_out;
	std::optional<OnSwapFunc> on_swap;
	std::optional<OnTakeItemFunc> on_take_item;
	std::optional<OnWeatherChangeFunc> on_weather_change;
	std::optional<OnTerrainChangeFunc> on_terrain_change;
	std::optional<OnTrapPokemonFunc> on_trap_pokemon;
	std::optional<OnTryAddVolatileFunc> on_try_add_volatile;
	std::optional<OnTryEatItemFunc> on_try_eat_item;
	std::optional<OnTryHealFunc> on_try_heal;
	std::optional<OnTryPrimaryHitFunc> on_try_primary_hit;
	std::optional<OnTypeFunc> on_type;
	std::optional<OnUseItemFunc> on_use_item;
	std::optional<OnUpdateFunc> on_update;
	std::optional<OnWeatherFunc> on_weather;

	std::optional<OnWeatherModifyDamageFunc> on_weather_modify_damage;
	std::optional<OnModifyDamagePhase1Func> on_modify_damage_phase_1;
	std::optional<OnModifyDamagePhase2Func> on_modify_damage_phase_2;
	std::optional<OnFoeDamagingHitFunc> on_foe_damaging_hit;
	std::optional<OnFoeAfterEachBoostFunc> on_foe_after_each_boost;
	std::optional<OnFoeAfterHitFunc> on_foe_after_hit;
	std::optional<OnFoeAfterSetStatusFunc> on_foe_after_set_status;
	std::optional<OnFoeAfterSubDamageFunc> on_foe_after_sub_damage;
	std::optional<OnFoeAfterSwitchInSelfFunc> on_foe_after_switch_in_self;
	std::optional<OnFoeAfterUseItemFunc> on_foe_after_use_item;
	std::optional<OnFoeAfterBoostFunc> on_foe_after_boost;
	std::optional<OnFoeAfterFaintFunc> on_foe_after_faint;
	std::optional<OnFoeAfterMoveSecondarySelfFunc> on_foe_after_move_secondary_self;
	std::optional<OnFoeAfterMoveSecondaryFunc> on_foe_after_move_secondary;
	std::optional<OnFoeAfterMoveFunc> on_foe_after_move;
	std::optional<OnFoeAfterMoveSelfFunc> on_foe_after_move_self;
	std::optional<OnFoeAttractFunc> on_foe_attract;
	std::optional<OnFoeAccuracyFunc> on_foe_accuracy;

	std::optional<OnFoeBasePowerFunc> on_foe_base_power;
	std::optional<OnFoeBeforeFaintFunc> on_foe_before_faint;
	std::optional<OnFoeBeforeMoveFunc> on_foe_before_move;
	std::optional<OnFoeBeforeSwitchInFunc> on_foe_before_switch_in;
	std::optional<OnFoeBeforeSwitchOutFunc> on_foe_before_switch_out;
	std::optional<OnFoeTryBoostFunc> on_foe_try_boost;
	std::optional<OnFoeChargeMoveFunc> on_foe_charge_move;
	std::optional<OnFoeCriticalHitFunc> on_foe_critical_hit;
	std::optional<OnFoeDamageFunc> on_foe_damage;
	std::optional<OnFoeDeductPPFunc> on_foe_deduct_pp;
	std::optional<OnFoeDisableMoveFunc> on_foe_disable_move;
	std::optional<OnFoeDragOutFunc> on_foe_drag_out;
	std::optional<OnFoeEatItemFunc> on_foe_eat_item;
	std::optional<OnFoeEffectivenessFunc> on_foe_effectiveness;
	std::optional<OnFoeFaintFunc> on_foe_faint;
	std::optional<OnFoeFlinchFunc> on_foe_flinch;
	std::optional<OnFoeHitFunc> on_foe_hit;
	std::optional<OnFoeImmunityFunc> on_foe_immunity;
	std::optional<OnFoeLockMoveFunc> on_foe_lock_move;
	std::optional<OnFoeMaybeTrapPokemonFunc> on_foe_maybe_trap_pokemon;
	std::optional<OnFoeModifyAccuracyFunc> on_foe_modify_accuracy;
	std::optional<OnFoeModifyAtkFunc> on_foe_modify_atk;
	std::optional<OnFoeModifyBoostFunc> on_foe_modify_boost;
	std::optional<OnFoeModifyCritRatioFunc> on_foe_modify_crit_ratio;
	std::optional<OnFoeModifyDamageFunc> on_foe_modify_damage;
	std::optional<OnFoeModifyDefFunc> on_foe_modify_def;
	std::optional<OnFoeModifyMoveFunc> on_foe_modify_move;
	std::optional<OnFoeModifyPriorityFunc> on_foe_modify_priority;
	std::optional<OnFoeModifySecondariesFunc> on_foe_modify_secondaries;

	std::optional<OnFoeModifySpAFunc> on_foe_modify_spa;
	std::optional<OnFoeModifySpDFunc> on_foe_modify_spd;
	std::optional<OnFoeModifySpeFunc> on_foe_modify_spe;
	std::optional<OnFoeModifySTABFunc> on_foe_modify_stab;
	std::optional<OnFoeModifyTypeFunc> on_foe_modify_type;
	std::optional<OnFoeModifyTargetFunc> on_foe_modify_target;
	std::optional<OnFoeModifyWeightFunc> on_foe_modify_weight;
	std::optional<OnFoeMoveAbortedFunc> on_foe_move_aborted;
	std::optional<OnFoeNegateImmunityFunc> on_foe_negate_immunity;
	std::optional<OnFoeOverrideActionFunc> on_foe_override_action;
	std::optional<OnFoePrepareHitFunc> on_foe_prepare_hit;
	std::optional<OnFoeRedirectTargetFunc> on_foe_redirect_target;
	std::optional<OnFoeResidualFunc> on_foe_residual;
	std::optional<OnFoeSetAbilityFunc> on_foe_set_ability;
	std::optional<OnFoeSetStatusFunc> on_foe_set_status;
	std::optional<OnFoeSetWeatherFunc> on_foe_set_weather;
	std::optional<OnFoeSideConditionStartFunc> on_foe_side_condition_start;
	std::optional<OnFoeStallMoveFunc> on_foe_stall_move;
	std::optional<OnFoeSwitchOutFunc> on_foe_switch_out;
	std::optional<OnFoeSwapFunc> on_foe_swap;
	std::optional<OnFoeTakeItemFunc> on_foe_take_item;
	std::optional<OnFoeWeatherChangeFunc> on_foe_weather_change;
	std::optional<OnFoeTerrainChangeFunc> on_foe_terrain_change;
	std::optional<OnFoeTrapPokemonFunc> on_foe_trap_pokemon;
	std::optional<OnFoeTryAddVolatileFunc> on_foe_try_add_volatile;
	std::optional<OnFoeTryEatItemFunc> on_foe_try_eat_item;
	std::optional<OnFoeTryHealFunc> on_foe_try_heal;

	std::optional<OnFoeTryHitFunc> on_foe_try_hit;
	std::optional<OnFoeTryHitFieldFunc> on_foe_try_hit_field;
	std::optional<OnFoeTryHitSideFunc> on_foe_try_hit_side;
	std::optional<OnFoeInvulnerabilityFunc> on_foe_invulnerability;
	std::optional<OnFoeTryMoveFunc> on_foe_try_move;
	std::optional<OnFoeTryPrimaryHitFunc> on_foe_try_primary_hit;
	std::optional<OnFoeTypeFunc> on_foe_type;
	std::optional<OnFoeWeatherModifyDamageFunc> on_foe_weather_modify_damage;
	std::optional<OnFoeModifyDamagePhase1Func> on_foe_modify_damage_phase_1;
	std::optional<OnFoeModifyDamagePhase2Func> on_foe_modify_damage_phase_2;
	std::optional<OnSourceDamagingHitFunc> on_source_damaging_hit;
	std::optional<OnSourceAfterEachBoostFunc> on_source_after_each_boost;
	std::optional<OnSourceAfterHitFunc> on_source_after_hit;
	std::optional<OnSourceAfterSetStatusFunc> on_source_after_set_status;
	std::optional<OnSourceAfterSubDamageFunc> on_source_after_sub_damage;
	std::optional<OnSourceAfterSwitchInSelfFunc> on_source_after_switch_in_self;
	std::optional<OnSourceAfterUseItemFunc> on_source_after_use_item;
	std::optional<OnSourceAfterBoostFunc> on_source_after_boost;
	std::optional<OnSourceAfterFaintFunc> on_source_after_faint;
	std::optional<OnSourceAfterMoveSecondarySelfFunc> on_source_after_move_secondary_self;
	std::optional<OnSourceAfterMoveSecondaryFunc> on_source_after_move_secondary;
	std::optional<OnSourceAfterMoveFunc> on_source_after_move;
	std::optional<VoidSourceMoveFunc> on_source_after_move_self;
	std::optional<OnSourceAttractFunc> on_source_attract;
	std::optional<OnSourceAccuracyFunc> on_source_accuracy;

	std::optional<OnSourceBasePowerFunc> on_source_base_power;
	std::optional<OnSourceBeforeFaintFunc> on_source_before_faint;
	std::optional<VoidSourceMoveFunc> on_source_before_move;
	std::optional<OnSourceBeforeSwitchInFunc> on_source_before_switch_in;
	std::optional<OnSourceBeforeSwitchOutFunc> on_source_before_switch_out;
	std::optional<OnSourceTryBoostFunc> on_source_try_boost;
	std::optional<OnSourceChargeMoveFunc> on_source_charge_move;
	std::optional<OnSourceCriticalHitFunc> on_source_critical_hit;
	std::optional<OnSourceDamageFunc> on_source_damage;
	std::optional<OnSourceDeductPPFunc> on_source_deduct_pp;
	std::optional<OnSourceDisableMoveFunc> on_source_disable_move;
	std::optional<OnSourceDragOutFunc> on_source_drag_out;
	std::optional<OnSourceEatItemFunc> on_source_eat_item;
	std::optional<OnSourceEffectivenessFunc> on_source_effectiveness;
	std::optional<OnSourceFaintFunc> on_source_faint;

	std::optional<OnSourceFlinchFunc> on_source_flinch;
	std::optional<OnSourceHitFunc> on_source_hit;
	std::optional<OnSourceImmunityFunc> on_source_immunity;
	std::optional<OnSourceLockMoveFunc> on_source_lock_move;
	std::optional<OnSourceMaybeTrapPokemonFunc> on_source_maybe_trap_pokemon;
	std::optional<OnSourceModifyAccuracyFunc> on_source_modify_accuracy;
	std::optional<OnSourceModifyAtkFunc> on_source_modify_atk;
	std::optional<OnSourceModifyBoostFunc> on_source_modify_boost;
	std::optional<OnSourceModifyCritRatioFunc> on_source_modify_crit_ratio;
	std::optional<OnSourceModifyDamageFunc> on_source_modify_damage;
	std::optional<OnSourceModifyDefFunc> on_source_modify_def;
	std::optional<OnSourceModifyMoveFunc> on_source_modify_move;
	std::optional<OnSourceModifyPriorityFunc> on_source_modify_priority;
	std::optional<OnSourceModifySecondariesFunc> on_source_modify_secondaries;

	std::optional<OnSourceModifySpAFunc> on_source_modify_spa;
	std::optional<OnSourceModifySpDFunc> on_source_modify_spd;
	std::optional<OnSourceModifySpeFunc> on_source_modify_spe;
	std::optional<OnSourceModifySTABFunc> on_source_modify_stab;
	std::optional<OnSourceModifyTypeFunc> on_source_modify_type;
	std::optional<OnSourceModifyTargetFunc> on_source_modify_target;
	std::optional<OnSourceModifyWeightFunc> on_source_modify_weight;
	std::optional<OnSourceMoveAbortedFunc> on_source_move_aborted;
	std::optional<OnSourceNegateImmunityFunc> on_source_negate_immunity;
	std::optional<OnSourceOverrideActionFunc> on_source_override_action;
	std::optional<OnSourcePrepareHitFunc> on_source_prepare_hit;
	std::optional<OnSourceRedirectTargetFunc> on_source_redirect_target;
	std::optional<OnSourceResidualFunc> on_source_residual;
	std::optional<OnSourceSetAbilityFunc> on_source_set_ability;
	std::optional<OnSourceSetStatusFunc> on_source_set_status;

	std::optional<OnSourceSetWeatherFunc> on_source_set_weather;
	std::optional<OnSourceStallMoveFunc> on_source_stall_move;
	std::optional<OnSourceSwitchOutFunc> on_source_switch_out;
	std::optional<OnSourceTakeItemFunc> on_source_take_item;
	std::optional<OnSourceTerrainFunc> on_source_terrain_change;
	std::optional<OnSourceTrapPokemonFunc> on_source_trap_pokemon;
	std::optional<OnSourceTryAddVolatileFunc> on_source_try_add_volatile;
	std::optional<OnSourceTryEatItemFunc> on_source_try_eat_item;
	std::optional<OnSourceTryHealFunc> on_source_try_heal;
	std::optional<OnSourceTryHitFunc> on_source_try_hit;
	std::optional<OnSourceTryHitFieldFunc> on_source_try_hit_field;
	std::optional<OnSourceTryHitSideFunc> on_source_try_hit_side;
	std::optional<OnSourceInvulnerabilityFunc> on_source_invulnerability;
	std::optional<OnSourceTryMoveFunc> on_source_try_move;
	std::optional<OnSourceTryPrimaryHitFunc> on_source_try_primary_hit;

	std::optional<OnSourceTypeFunc> on_source_type;
	std::optional<OnSourceWeatherModifyDamageFunc> on_source_weather_modify_damage;
	std::optional<OnSourceModifyDamagePhase1Func> on_source_modify_damage_phase_1;
	std::optional<OnSourceModifyDamagePhase2Func> on_source_modify_damage_phase_2;
	std::optional<OnAnyDamagingHitFunc> on_any_damaging_hit;
	std::optional<OnAnyAfterEachBoostFunc> on_any_after_each_boost;
	std::optional<OnAnyAfterHitFunc> on_any_after_hit;
	std::optional<OnAnyAfterSetStatusFunc> on_any_after_set_status;
	std::optional<OnAnyAfterSubDamageFunc> on_any_after_sub_damage;
	std::optional<OnAnyAfterSwitchInSelfFunc> on_any_after_switch_in_self;
	std::optional<OnAnyAfterUseItemFunc> on_any_after_use_item;
	std::optional<OnAnyAfterBoostFunc> on_any_after_boost;
	std::optional<OnAnyAfterFaintFunc> on_any_after_faint;
	std::optional<OnAnyAfterMegaFunc> on_any_after_mega;
	
	std::optional<OnAnyAfterMoveSecondarySelfFunc> on_any_after_move_secondary_self;
	std::optional<OnAnyAfterMoveSecondaryFunc> on_any_after_move_secondary;
	std::optional<OnAnyAfterMoveFunc> on_any_after_move;
	std::optional<VoidSourceMoveFunc> on_any_after_move_self;
	std::optional<OnAnyAttractFunc> on_any_attract;
	std::optional<OnAnyAccuracyFunc> on_any_accuracy;
	std::optional<OnAnyBasePowerFunc> on_any_base_power;
	std::optional<OnAnyBeforeFaintFunc> on_any_before_faint;
	std::optional<VoidSourceMoveFunc> on_any_before_move;
	std::optional<OnAnyBeforeSwitchInFunc> on_any_before_switch_in;
	std::optional<OnAnyBeforeSwitchOutFunc> on_any_before_switch_out;
	std::optional<OnAnyTryBoostFunc> on_any_try_boost;
	std::optional<VoidSourceMoveFunc> on_any_charge_move;
	std::optional<OnAnyCriticalHitFunc> on_any_critical_hit;

	std::optional<OnAnyDamageFunc> on_any_damage;
	std::optional<OnAnyDeductPPFunc> on_any_deduct_pp;
	std::optional<OnAnyDisableMoveFunc> on_any_disable_move;
	std::optional<OnAnyDragOutFunc> on_any_drag_out;
	std::optional<OnAnyEatItemFunc> on_any_eat_item;
	std::optional<OnAnyEffectivenessFunc> on_any_effectiveness;
	std::optional<OnAnyFaintFunc> on_any_faint;
	std::optional<OnAnyFlinchFunc> on_any_flinch;
	std::optional<OnAnyHitFunc> on_any_hit;
	std::optional<OnAnyImmunityFunc> on_any_immunity;
	std::optional<OnAnyLockMoveFunc> on_any_lock_move;
	std::optional<OnAnyMaybeTrapPokemonFunc> on_any_maybe_trap_pokemon;
	std::optional<OnAnyModifyAccuracyFunc> on_any_modify_accuracy;
	std::optional<OnAnyModifyAtkFunc> on_any_modify_atk;

	std::optional<OnAnyModifyBoostFunc> on_any_modify_boost;
	std::optional<OnAnyModifyCritRatioFunc> on_any_modify_crit_ratio;
	std::optional<OnAnyModifyDamageFunc> on_any_modify_damage;
	std::optional<OnAnyModifyDefFunc> on_any_modify_def;
	std::optional<OnAnyModifyMoveFunc> on_any_modify_move;
	std::optional<OnAnyModifyPriorityFunc> on_any_modify_priority;
	std::optional<OnAnyModifySecondariesFunc> on_any_modify_secondaries;
	std::optional<OnAnyModifySpAFunc> on_any_modify_spa;
	std::optional<OnAnyModifySpDFunc> on_any_modify_spd;
	std::optional<OnAnyModifySpeFunc> on_any_modify_spe;
	std::optional<OnAnyModifySTABFunc> on_any_modify_stab;
	std::optional<OnAnyModifyTypeFunc> on_any_modify_type;
	std::optional<OnAnyModifyTargetFunc> on_any_modify_target;
	std::optional<OnAnyModifyWeightFunc> on_any_modify_weight;
	std::optional<VoidMoveFunc> on_any_move_aborted;

	std::optional<OnAnyNegateImmunityFunc> on_any_negate_immunity;
	std::optional<OnAnyOverrideActionFunc> on_any_override_action;
	std::optional<ResultSourceMoveFunc> on_any_prepare_hit;
	std::optional<OnAnyRedirectTargetFunc> on_any_redirect_target;
	std::optional<OnAnyResidualFunc> on_any_residual;
	std::optional<OnAnySetAbilityFunc> on_any_set_ability;
	std::optional<OnAnySetStatusFunc> on_any_set_status;
	std::optional<OnAnySetWeatherFunc> on_any_set_weather;
	std::optional<OnAnyStallMoveFunc> on_any_stall_move;
	std::optional<OnAnySwitchInFunc> on_any_switch_in;
	std::optional<OnAnySwitchOutFunc> on_any_switch_out;
	std::optional<OnAnyTakeItemFunc> on_any_take_item;

	std::optional<OnAnyTerrainFunc> on_any_terrain_change;
	std::optional<OnAnyTrapPokemonFunc> on_any_trap_pokemon;
	std::optional<OnAnyTryAddVolatileFunc> on_any_try_add_volatile;
	std::optional<OnAnyTryEatItemFunc> on_any_try_eat_item;
	std::optional<OnAnyTryHealFunc> on_any_try_heal;
	std::optional<OnAnyTryHitFunc> on_any_try_hit;
	std::optional<OnAnyTryHitFieldFunc> on_any_try_hit_field;
	std::optional<OnAnyTryHitSideFunc> on_any_try_hit_side;
	std::optional<OnAnyInvulnerabilityFunc> on_any_invulnerability;
	std::optional<OnAnyTryMoveFunc> on_any_try_move;
	std::optional<OnAnyTryPrimaryHitFunc> on_any_try_primary_hit;
	std::optional<OnAnyTypeFunc> on_any_type;
	std::optional<OnAnyWeatherModifyDamageFunc> on_any_weather_modify_damage;
	std::optional<OnAnyModifyDamagePhase1Func> on_any_modify_damage_phase_1;
	std::optional<OnAnyModifyDamagePhase2Func> on_any_modify_damage_phase_2;

	std::optional<int> on_accuracy_priority;
	std::optional<int> on_damaging_hit_order;
	std::optional<int> on_after_move_secondary_priority;
	std::optional<int> on_after_move_secondary_self_priority;
	std::optional<int> on_after_move_self_priority;
	std::optional<int> on_after_set_status_priority;
	std::optional<int> on_any_base_power_priority;
	std::optional<int> on_any_invulnerability_priority;
	std::optional<int> on_any_modify_accuracy_priority;
	std::optional<int> on_any_faint_priority;
	std::optional<int> on_any_prepare_hit_priority;
	std::optional<int> on_any_switch_in_priority;
	std::optional<int> on_any_switch_in_sub_order;
	std::optional<int> on_ally_base_power_priority;
	std::optional<int> on_ally_modify_atk_priority;
	std::optional<int> on_ally_modify_spa_priority;
	std::optional<int> on_ally_modify_spd_priority;
	std::optional<int> on_attract_priority;
	std::optional<int> on_base_power_priority;
	std::optional<int> on_before_move_priority;
	std::optional<int> on_before_switch_out_priority;
	std::optional<int> on_change_boost_priority;
	std::optional<int> on_damage_priority;
	std::optional<int> on_drag_out_priority;
	std::optional<int> on_effectiveness_priority;
	std::optional<int> on_foe_base_power_priority;
	std::optional<int> on_foe_before_move_priority;
	std::optional<int> on_foe_modify_def_priority;
	std::optional<int> on_foe_modify_spd_priority;
	std::optional<int> on_foe_redirect_target_priority;
	std::optional<int> on_foe_trap_pokemon_priority;
	std::optional<int> on_fractional_priority_priority;
	std::optional<int> on_hit_priority;
	std::optional<int> on_invulnerability_priority;
	std::optional<int> on_modify_accuracy_priority;
	std::optional<int> on_modify_atk_priority;
	std::optional<int> on_modify_crit_ratio_priority;
	std::optional<int> on_modify_def_priority;
	std::optional<int> on_modify_move_priority;
	std::optional<int> on_modify_priority_priority;
	std::optional<int> on_modify_spa_priority;
	std::optional<int> on_modify_spd_priority;
	std::optional<int> on_modify_spe_priority;
	std::optional<int> on_modify_stab_priority;
	std::optional<int> on_modify_type_priority;
	std::optional<int> on_modify_weight_priority;
	std::optional<int> on_redirect_target_priority;
	std::optional<int> on_residual_order;
	std::optional<int> on_residual_priority;
	std::optional<int> on_residual_sub_order;
	std::optional<int> on_source_base_power_priority;
	std::optional<int> on_source_invulnerability_priority;
	std::optional<int> on_source_modify_accuracy_priority;
	std::optional<int> on_source_modify_atk_priority;
	std::optional<int> on_source_modify_damage_priority;
	std::optional<int> on_source_modify_spa_priority;
	std::optional<int> on_switch_in_priority;
	std::optional<int> on_switch_in_sub_order;
	std::optional<int> on_trap_pokemon_priority;
	std::optional<int> on_try_boost_priority;
	std::optional<int> on_try_eat_item_priority;
	std::optional<int> on_try_heal_priority;
	std::optional<int> on_try_hit_priority;
	std::optional<int> on_try_move_priority;
	std::optional<int> on_try_primary_hit_priority;
	std::optional<int> on_type_priority;
};

using OnAllyDamagingHitFunc = std::function<void(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;
using OnAllyAfterEachBoostFunc = std::function<void(Battle*, SparseBoostsTable, Pokemon*, Pokemon*)>;
using OnAllyAfterHitFunc = OnAfterHitFunc; // from MoveEventMethods
using OnAllyAfterSetStatusFunc = std::function<void(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
using OnAllyAfterSubDamageFunc = OnAfterSubDamageFunc; // from MoveEventMethods
using OnAllyAfterSwitchInSelfFunc = std::function<void(Battle*, Pokemon*)>;
using OnAllyAfterUseItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
using OnAllyAfterBoostFunc = std::function<void(Battle*, SparseBoostsTable, Pokemon*, Pokemon*, Effect)>;
using OnAllyAfterFaintFunc = std::function<void(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using OnAllyAfterMoveSecondarySelfFunc = OnAfterMoveSecondarySelfFunc; // from MoveEventMethods
using OnAllyAfterMoveSecondaryFunc = OnAfterMoveSecondaryFunc; // from MoveEventMethods
using OnAllyAfterMoveFunc = OnAfterMoveFunc; // from MoveEventMethods
using OnAllyAfterMoveSelfFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnAllyAttractFunc = std::function<void(Battle*, Pokemon*, Pokemon*)>;
using OnAllyAccuracyFunc = std::function<std::variant<int, bool, std::nullptr_t, void>(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;

using OnAllyBasePowerFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnAllyBeforeFaintFunc = std::function<void(Battle*, Pokemon*, Effect)>;
using OnAllyBeforeMoveFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnAllyBeforeSwitchInFunc = std::function<void(Battle*, Pokemon*)>;
using OnAllyBeforeSwitchOutFunc = std::function<void(Battle*, Pokemon*)>;
using OnAllyTryBoostFunc = std::function<void(Battle*, SparseBoostsTable, Pokemon*, Pokemon*, Effect)>;
using OnAllyChargeMoveFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnAllyCriticalHitFunc = std::function<std::variant<bool, std::nullptr_t>(Battle*, Pokemon*, std::nullptr_t, ActiveMove*)>;
using OnAllyDamageFunc = std::function<std::variant<int, bool, std::nullptr_t, void>(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using OnAllyDeductPPFunc = std::function<std::variant<int, void>(Battle*, Pokemon*, Pokemon*)>;
using OnAllyDisableMoveFunc = std::function<void(Battle*, Pokemon*)>;
using OnAllyDragOutFunc = std::function<void(Battle*, Pokemon*, std::optional<Pokemon*>, std::optional<ActiveMove*>)>;
using OnAllyEatItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
using OnAllyEffectivenessFunc = OnEffectivenessFunc; // from MoveEventMethods

using OnAllyFaintFunc = VoidEffectFunc; // from CommonHandlers
using OnAllyFlinchFunc = std::function<std::variant<bool, void>(Battle*, Pokemon*)>;
using OnAllyHitFunc = OnHitFunc; // from MoveEventMethods
using OnAllyImmunityFunc = std::function<void(Battle*, std::string, Pokemon*)>;
using OnAllyLockMoveFunc = std::variant<std::string, std::function<std::variant<void, std::string>(Battle*, Pokemon*)>>;
using OnAllyMaybeTrapPokemonFunc = std::function<void(Battle*, Pokemon*)>;
using OnAllyModifyAccuracyFunc = ModifierMoveFunc; // from CommonHandlers
using OnAllyModifyAtkFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnAllyModifyBoostFunc = std::function<std::variant<SparseBoostsTable, void>(Battle*, SparseBoostsTable, Pokemon*)>;
using OnAllyModifyCritRatioFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnAllyModifyDamageFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnAllyModifyDefFunc = ModifierMoveFunc; // from CommonHandlers
using OnAllyModifyMoveFunc = OnModifyMoveFunc; // from MoveEventMethods
using OnAllyModifyPriorityFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnAllyModifySecondariesFunc = std::function<void(Battle*, std::vector<SecondaryEffect>&, Pokemon*, Pokemon*, ActiveMove*)>;

using OnAllyModifySpAFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnAllyModifySpDFunc = ModifierMoveFunc; // from CommonHandlers
using OnAllyModifySpeFunc = std::function<std::variant<int, void>(Battle*, int, Pokemon*)>;
using OnAllyModifySTABFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnAllyModifyTypeFunc = OnModifyTypeFunc; // from MoveEventMethods
using OnAllyModifyTargetFunc = OnModifyTargetFunc; // from MoveEventMethods
using OnAllyModifyWeightFunc = std::function<std::variant<int, void>(Battle*, int, Pokemon*)>;
using OnAllyMoveAbortedFunc = VoidMoveFunc; // from CommonHandlers
using OnAllyNegateImmunityFunc = std::variant<std::function<std::variant<bool, void>(Battle*, Pokemon*, std::string)>, bool>;
using OnAllyOverrideActionFunc = std::function<std::variant<std::string, void>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnAllyPrepareHitFunc = ResultSourceMoveFunc; // from CommonHandlers
using OnAllyRedirectTargetFunc = std::function<std::variant<Pokemon*, void>(Battle*, Pokemon*, Pokemon*, Effect, ActiveMove*)>;
using OnAllyResidualFunc = std::function<void(Battle*, Pokemon&, Pokemon*, Effect)>;
using OnAllySetAbilityFunc = std::function<std::variant<bool, void>(Battle*, std::string, Pokemon*, Pokemon*, Effect)>;

using OnAllySetStatusFunc = std::function<std::variant<bool, std::nullptr_t, void>(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
using OnAllySetWeatherFunc = std::function<std::variant<bool, void>(Battle*, Pokemon*, Pokemon*, Condition*)>;
using OnAllyStallMoveFunc = std::function<std::variant<bool, void>(Battle*, Pokemon*)>;
using OnAllySwitchOutFunc = std::function<void(Battle*, Pokemon*)>;
using OnAllyTakeItemFunc = std::variant<std::function<std::variant<bool, void>(Battle*, Item*, Pokemon*, Pokemon*, std::optional<ActiveMove>)>, bool>;
using OnAllyTerrainFunc = std::function<void(Battle*, Pokemon*)>;
using OnAllyTrapPokemonFunc = std::function<void(Battle*, Pokemon*)>;
using OnAllyTryAddVolatileFunc = std::function<std::variant<bool, std::nullptr_t, void>(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
using OnAllyTryEatItemFunc = std::variant<bool, std::function<std::variant<bool, void>(Battle*, Item*, Pokemon*)>>;
using OnAllyTryHealFunc = std::variant<
	std::function<std::variant<int, bool, void>(Battle*, int, Pokemon*, Pokemon*, Effect)>,
	std::function<std::variant<bool, void>(Battle*, Pokemon*)>,
	bool>;

using OnAllyTryHitFunc = OnTryHitFunc; // from MoveEventMethods
using OnAllyTryHitFieldFunc = OnTryHitFieldFunc; // from MoveEventMethods
using OnAllyTryHitSideFunc = ResultMoveFunc; // from CommonHandlers
using OnAllyInvulnerabilityFunc = ExtResultMoveFunc; // from CommonHandlers
using OnAllyTryMoveFunc = OnTryMoveFunc; // from MoveEventMethods
using OnAllyTryPrimaryHitFunc = std::function<std::variant<bool, std::nullptr_t, int, void>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnAllyTypeFunc = OnTypeFunc; // from MoveEventMethods
using OnAllyWeatherModifyDamageFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnAllyModifyDamagePhase1Func = ModifierSourceMoveFunc; // from CommonHandlers
using OnAllyModifyDamagePhase2Func = ModifierSourceMoveFunc; // from CommonHandlers

struct PokemonEventMethods : public EventMethods
{
	std::optional<OnAllyDamagingHitFunc> on_ally_damaging_hit;
	std::optional<OnAllyAfterEachBoostFunc> on_ally_after_each_boost;	
	std::optional<OnAllyAfterHitFunc> on_ally_after_hit;
	std::optional<OnAllyAfterSetStatusFunc> on_ally_after_set_status;
	std::optional<OnAllyAfterSubDamageFunc> on_ally_after_sub_damage;
	std::optional<OnAllyAfterSwitchInSelfFunc> on_ally_after_switch_in_self;
	std::optional<OnAllyAfterUseItemFunc> on_ally_after_use_item;
	std::optional<OnAllyAfterBoostFunc> on_ally_after_boost;
	std::optional<OnAllyAfterFaintFunc> on_ally_after_faint;
	std::optional<OnAllyAfterMoveSecondarySelfFunc> on_ally_after_move_secondary_self;
	std::optional<OnAllyAfterMoveSecondaryFunc> on_ally_after_move_secondary;
	std::optional<OnAllyAfterMoveFunc> on_ally_after_move;
	std::optional<OnAllyAfterMoveSelfFunc> on_ally_after_move_self;
	std::optional<OnAllyAttractFunc> on_ally_attract;
	std::optional<OnAllyAccuracyFunc> on_ally_accuracy;

	std::optional<OnAllyBasePowerFunc> on_ally_base_power;
	std::optional<OnAllyBeforeFaintFunc> on_ally_before_faint;
	std::optional<OnAllyBeforeMoveFunc> on_ally_before_move;
	std::optional<OnAllyBeforeSwitchInFunc> on_ally_before_switch_in;
	std::optional<OnAllyBeforeSwitchOutFunc> on_ally_before_switch_out;
	std::optional<OnAllyTryBoostFunc> on_ally_try_boost;
	std::optional<OnAllyChargeMoveFunc> on_ally_charge_move;
	std::optional<OnAllyCriticalHitFunc> on_ally_critical_hit;
	std::optional<OnAllyDamageFunc> on_ally_damage;
	std::optional<OnAllyDeductPPFunc> on_ally_deduct_pp;
	std::optional<OnAllyDisableMoveFunc> on_ally_disable_move;
	std::optional<OnAllyDragOutFunc> on_ally_drag_out;
	std::optional<OnAllyEatItemFunc> on_ally_eat_item;
	std::optional<OnAllyEffectivenessFunc> on_ally_effectiveness;

	std::optional<OnAllyFaintFunc> on_ally_faint;
	std::optional<OnAllyFlinchFunc> on_ally_flinch;
	std::optional<OnAllyHitFunc> on_ally_hit;
	std::optional<OnAllyImmunityFunc> on_ally_immunity;
	std::optional<OnAllyLockMoveFunc> on_ally_lock_move;
	std::optional<OnAllyMaybeTrapPokemonFunc> on_ally_maybe_trap_pokemon;
	std::optional<OnAllyModifyAccuracyFunc> on_ally_modify_accuracy;
	std::optional<OnAllyModifyAtkFunc> on_ally_modify_atk;
	std::optional<OnAllyModifyBoostFunc> on_ally_modify_boost;
	std::optional<OnAllyModifyCritRatioFunc> on_ally_modify_crit_ratio;
	std::optional<OnAllyModifyDamageFunc> on_ally_modify_damage;
	std::optional<OnAllyModifyDefFunc> on_ally_modify_def;
	std::optional<OnAllyModifyMoveFunc> on_ally_modify_move;
	std::optional<OnAllyModifyPriorityFunc> on_ally_modify_priority;
	std::optional<OnAllyModifySecondariesFunc> on_ally_modify_secondaries;

	std::optional<OnAllyModifySpAFunc> on_ally_modify_spa;
	std::optional<OnAllyModifySpDFunc> on_ally_modify_spd;
	std::optional<OnAllyModifySpeFunc> on_ally_modify_spe;
	std::optional<OnAllyModifySTABFunc> on_ally_modify_stab;
	std::optional<OnAllyModifyTypeFunc> on_ally_modify_type;
	std::optional<OnAllyModifyTargetFunc> on_ally_modify_target;
	std::optional<OnAllyModifyWeightFunc> on_ally_modify_weight;
	std::optional<OnAllyMoveAbortedFunc> on_ally_move_aborted;
	std::optional<OnAllyNegateImmunityFunc> on_ally_negate_immunity;
	std::optional<OnAllyOverrideActionFunc> on_ally_override_action;
	std::optional<OnAllyPrepareHitFunc> on_ally_prepare_hit;
	std::optional<OnAllyRedirectTargetFunc> on_ally_redirect_target;
	std::optional<OnAllyResidualFunc> on_ally_residual;
	std::optional<OnAllySetAbilityFunc> on_ally_set_ability;

	std::optional<OnAllySetStatusFunc> on_ally_set_status;
	std::optional<OnAllySetWeatherFunc> on_ally_set_weather;
	std::optional<OnAllyStallMoveFunc> on_ally_stall_move;
	std::optional<OnAllySwitchOutFunc> on_ally_switch_out;
	std::optional<OnAllyTakeItemFunc> on_ally_take_item;
	std::optional<OnAllyTerrainFunc> on_ally_terrain_change;
	std::optional<OnAllyTrapPokemonFunc> on_ally_trap_pokemon;
	std::optional<OnAllyTryAddVolatileFunc> on_ally_try_add_volatile;
	std::optional<OnAllyTryEatItemFunc> on_ally_try_eat_item;
	std::optional<OnAllyTryHealFunc> on_ally_try_heal;

	std::optional<OnAllyTryHitFunc> on_ally_try_hit;
	std::optional<OnAllyTryHitFieldFunc> on_ally_try_hit_field;
	std::optional<OnAllyTryHitSideFunc> on_ally_try_hit_side;
	std::optional<OnAllyInvulnerabilityFunc> on_ally_invulnerability;
	std::optional<OnAllyTryMoveFunc> on_ally_try_move;
	std::optional<OnAllyTryPrimaryHitFunc> on_ally_try_primary_hit;
	std::optional<OnAllyTypeFunc> on_ally_type;
	std::optional<OnAllyWeatherModifyDamageFunc> on_ally_weather_modify_damage;
	std::optional<OnAllyModifyDamagePhase1Func> on_ally_modify_damage_phase_1;
	std::optional<OnAllyModifyDamagePhase2Func> on_ally_modify_damage_phase_2;
};