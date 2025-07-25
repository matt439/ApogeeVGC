#pragma once

#include "../global-types/Effect.h"
#include "../global-types/SparseBoostsTable.h"
#include "../global-types/function_type_aliases.h"
//#include "../dex-moves/SecondaryEffect.h"
//#include "../battle/Battle.h"
//#include "../pokemon/Pokemon.h"
//#include "../dex-items/Item.h"
//#include "../dex-moves/ActiveMove.h"
//#include "../side/Side.h"
//#include "Condition.h"
#include <functional>
#include <variant>
#include <string>
#include <vector>

// forward declarations
class Battle;
class Pokemon;
class Item;
class ActiveMove;
class Condition;
class Side;
struct SecondaryEffect;

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
// using OnBasePowerFunc = ModifierSourceMoveFunc; // Already defined elsewhere
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
using OnDragOutFunc = std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnEatItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
// using OnEffectivenessFunc = OnEffectivenessFunc; // from MoveEventMethods
using OnEntryHazardFunc = std::function<void(Battle*, Pokemon*)>;
// using OnFaintFunc = VoidEffectFunc; // from CommonHandlers
using OnFlinchFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*)>>;
using OnFractionalPriorityFunc = std::variant<ModifierSourceMoveFunc, double>; // -0.1 or function
// using OnHitFunc = OnHitFunc; // from MoveEventMethods
using OnImmunityFunc = std::function<void(Battle*, std::string, Pokemon*)>;
using OnLockMoveFunc = std::variant<std::string, std::function<std::variant<std::monostate, std::string>(Battle*, Pokemon*)>>;
using OnMaybeTrapPokemonFunc = std::function<void(Battle*, Pokemon*)>;
// using OnModifyAccuracyFunc = ModifierMoveFunc; // from CommonHandlers
// using OnModifyAtkFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnModifyBoostFunc = std::function<std::variant<SparseBoostsTable, std::monostate>(Battle*, const SparseBoostsTable&, Pokemon*)>;
// using OnModifyCritRatioFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnModifyDamageFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnModifyDefFunc = ModifierMoveFunc; // from CommonHandlers
// using OnModifyMoveFunc = OnModifyMoveFunc; // from MoveEventMethods
// using OnModifyPriorityFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnModifySecondariesFunc = std::function<void(Battle*, std::vector<SecondaryEffect>, Pokemon*, Pokemon*, ActiveMove*)>;
// using OnModifyTypeFunc = OnModifyTypeFunc; // from MoveEventMethods
// using OnModifyTargetFunc = OnModifyTargetFunc; // from MoveEventMethods
// using OnModifySpAFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnModifySpDFunc = ModifierMoveFunc; // from CommonHandlers
using OnModifySpeFunc = std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*)>;
// using OnModifySTABFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnModifyWeightFunc = std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*)>;
// using OnMoveAbortedFunc = VoidMoveFunc; // from CommonHandlers
using OnNegateImmunityFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, std::string)>>;
using OnOverrideActionFunc = std::function<std::variant<std::string, std::monostate>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
// using OnPrepareHitFunc = ResultSourceMoveFunc; // from CommonHandlers
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
using OnTakeItemFunc = std::variant<std::function<std::variant<bool, std::monostate>(Battle*, Item*, Pokemon*, Pokemon*, ActiveMove*)>, bool>;
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

// using OnWeatherModifyDamageFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnModifyDamagePhase1Func = ModifierSourceMoveFunc; // from CommonHandlers
using OnModifyDamagePhase2Func = ModifierSourceMoveFunc; // from CommonHandlers
using OnFoeDamagingHitFunc = std::function<void(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;
using OnFoeAfterEachBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*)>;
// using OnFoeAfterHitFunc = OnAfterHitFunc; // from MoveEventMethods
using OnFoeAfterSetStatusFunc = std::function<void(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
// using OnFoeAfterSubDamageFunc = OnAfterSubDamageFunc; // from MoveEventMethods
using OnFoeAfterSwitchInSelfFunc = std::function<void(Battle*, Pokemon*)>;
using OnFoeAfterUseItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
using OnFoeAfterBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*, Effect)>;
using OnFoeAfterFaintFunc = std::function<void(Battle*, int, Pokemon*, Pokemon*, Effect)>;
// using OnFoeAfterMoveSecondarySelfFunc = OnAfterMoveSecondarySelfFunc; // from MoveEventMethods
// using OnFoeAfterMoveSecondaryFunc = OnAfterMoveSecondaryFunc; // from MoveEventMethods
// using OnFoeAfterMoveFunc = OnAfterMoveFunc; // from MoveEventMethods
// using OnFoeAfterMoveSelfFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnFoeAttractFunc = std::function<void(Battle*, Pokemon*, Pokemon*)>;
using OnFoeAccuracyFunc = std::function<std::variant<int, bool, std::monostate>(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;

// using OnFoeBasePowerFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnFoeBeforeFaintFunc = std::function<void(Battle*, Pokemon*, Effect)>;
// using OnFoeBeforeMoveFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnFoeBeforeSwitchInFunc = std::function<void(Battle*, Pokemon*)>;
using OnFoeBeforeSwitchOutFunc = std::function<void(Battle*, Pokemon*)>;
using OnFoeTryBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*, Effect)>;
// using OnFoeChargeMoveFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnFoeCriticalHitFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, std::nullptr_t, ActiveMove*)>>;
using OnFoeDamageFunc = std::function<std::variant<int, bool, std::monostate>(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using OnFoeDeductPPFunc = std::function<std::variant<int, std::monostate>(Battle*, Pokemon*, Pokemon*)>;
using OnFoeDisableMoveFunc = std::function<void(Battle*, Pokemon*)>;
using OnFoeDragOutFunc = std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnFoeEatItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
// using OnFoeEffectivenessFunc = OnEffectivenessFunc; // from MoveEventMethods
using OnFoeFaintFunc = VoidEffectFunc; // from CommonHandlers
using OnFoeFlinchFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*)>>;
// using OnFoeHitFunc = OnHitFunc; // from MoveEventMethods
using OnFoeImmunityFunc = std::function<void(Battle*, std::string, Pokemon*)>;
using OnFoeLockMoveFunc = std::variant<std::string, std::function<std::variant<std::monostate, std::string>(Battle*, Pokemon*)>>;
using OnFoeMaybeTrapPokemonFunc = std::function<void(Battle*, Pokemon*)>;
// using OnFoeModifyAccuracyFunc = ModifierMoveFunc; // from CommonHandlers
// using OnFoeModifyAtkFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnFoeModifyBoostFunc = std::function<std::variant<SparseBoostsTable, std::monostate>(Battle*, const SparseBoostsTable&, Pokemon*)>;
// using OnFoeModifyCritRatioFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnFoeModifyDamageFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnFoeModifyDefFunc = ModifierMoveFunc; // from CommonHandlers
// using OnFoeModifyMoveFunc = OnModifyMoveFunc; // from MoveEventMethods
// using OnFoeModifyPriorityFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnFoeModifySecondariesFunc = std::function<void(Battle*, std::vector<SecondaryEffect>, Pokemon*, Pokemon*, ActiveMove*)>;

// using OnFoeModifySpAFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnFoeModifySpDFunc = ModifierMoveFunc; // from CommonHandlers
using OnFoeModifySpeFunc = std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*)>;
// using OnFoeModifySTABFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnFoeModifyTypeFunc = OnModifyTypeFunc; // from MoveEventMethods
// using OnFoeModifyTargetFunc = OnModifyTargetFunc; // from MoveEventMethods
using OnFoeModifyWeightFunc = std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*)>;
// using OnFoeMoveAbortedFunc = VoidMoveFunc; // from CommonHandlers
using OnFoeNegateImmunityFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, std::string)>>;
using OnFoeOverrideActionFunc = std::function<std::variant<std::string, std::monostate>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
// using OnFoePrepareHitFunc = ResultSourceMoveFunc; // from CommonHandlers
using OnFoeRedirectTargetFunc = std::function<std::variant<Pokemon*, std::monostate>(Battle*, Pokemon*, Pokemon*, Effect, ActiveMove*)>;
using OnFoeResidualFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using OnFoeSetAbilityFunc = std::function<std::variant<std::monostate, void>(Battle*, std::string, Pokemon*, Pokemon*, Effect)>;
using OnFoeSetStatusFunc = std::function<std::variant<bool, std::monostate>(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
using OnFoeSetWeatherFunc = std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, Pokemon*, Condition*)>;
using OnFoeSideConditionStartFunc = std::function<void(Battle*, Side*, Pokemon*, Condition*)>;
using OnFoeStallMoveFunc = std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*)>;
using OnFoeSwitchOutFunc = std::function<void(Battle*, Pokemon*)>;
using OnFoeSwapFunc = std::function<void(Battle*, Pokemon*, Pokemon*)>;
using OnFoeTakeItemFunc = std::variant<std::function<std::variant<bool, std::monostate>(Battle*, Item*, Pokemon*, Pokemon*, ActiveMove*)>, bool>;
using OnFoeWeatherChangeFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using OnFoeTerrainChangeFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using OnFoeTrapPokemonFunc = std::function<void(Battle*, Pokemon*)>;
using OnFoeTryAddVolatileFunc = std::function<std::variant<bool, std::nullptr_t, std::monostate>(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
using OnFoeTryEatItemFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Item*, Pokemon*)>>;
using OnFoeTryHealFunc = std::function<std::variant<int, bool, std::nullptr_t, std::monostate>(Battle*, int, Pokemon*, Pokemon*, Effect)>;

// using OnFoeTryHitFunc = OnTryHitFunc; // from MoveEventMethods
// using OnFoeTryHitFieldFunc = OnTryHitFieldFunc; // from MoveEventMethods
// using OnFoeTryHitSideFunc = ResultMoveFunc; // from CommonHandlers
// using OnFoeInvulnerabilityFunc = ExtResultMoveFunc; // from CommonHandlers
// using OnFoeTryMoveFunc = OnTryMoveFunc; // from MoveEventMethods
using OnFoeTryPrimaryHitFunc = std::function<std::variant<bool, std::nullptr_t, int, std::monostate>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnFoeTypeFunc = std::function<std::variant<std::vector<std::string>, std::monostate>(Battle*, std::vector<std::string>, Pokemon*)>;
// using OnFoeWeatherModifyDamageFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnFoeModifyDamagePhase1Func = ModifierSourceMoveFunc; // from CommonHandlers
// using OnFoeModifyDamagePhase2Func = ModifierSourceMoveFunc; // from CommonHandlers
using OnSourceDamagingHitFunc = std::function<void(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;
using OnSourceAfterEachBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*)>;
// using OnSourceAfterHitFunc = OnAfterHitFunc; // from MoveEventMethods
using OnSourceAfterSetStatusFunc = std::function<void(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
// using OnSourceAfterSubDamageFunc = OnAfterSubDamageFunc; // from MoveEventMethods
using OnSourceAfterSwitchInSelfFunc = std::function<void(Battle*, Pokemon*)>;
using OnSourceAfterUseItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
using OnSourceAfterBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*, Effect)>;
using OnSourceAfterFaintFunc = std::function<void(Battle*, int, Pokemon*, Pokemon*, Effect)>;
// using OnSourceAfterMoveSecondarySelfFunc = OnAfterMoveSecondarySelfFunc; // from MoveEventMethods
// using OnSourceAfterMoveSecondaryFunc = OnAfterMoveSecondaryFunc; // from MoveEventMethods
// using OnSourceAfterMoveFunc = OnAfterMoveFunc; // from MoveEventMethods
// using OnSourceAfterMoveSelfFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnSourceAttractFunc = std::function<void(Battle*, Pokemon*, Pokemon*)>;
using OnSourceAccuracyFunc = std::function<std::variant<int, bool, std::monostate>(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;

// using OnSourceBasePowerFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnSourceBeforeFaintFunc = std::function<void(Battle*, Pokemon*, Effect)>;
// using OnSourceBeforeMoveFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnSourceBeforeSwitchInFunc = std::function<void(Battle*, Pokemon*)>;
using OnSourceBeforeSwitchOutFunc = std::function<void(Battle*, Pokemon*)>;
using OnSourceTryBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*, Effect)>;
// using OnSourceChargeMoveFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnSourceCriticalHitFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, std::nullptr_t, ActiveMove*)>>;
using OnSourceDamageFunc = std::function<std::variant<int, bool, std::monostate>(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using OnSourceDeductPPFunc = std::function<std::variant<int, std::monostate>(Battle*, Pokemon*, Pokemon*)>;
using OnSourceDisableMoveFunc = std::function<void(Battle*, Pokemon*)>;
using OnSourceDragOutFunc = std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnSourceEatItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
// using OnSourceEffectivenessFunc = OnEffectivenessFunc; // from MoveEventMethods
// using OnSourceFaintFunc = VoidEffectFunc; // from CommonHandlers

using OnSourceFlinchFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*)>>;
// using OnSourceHitFunc = OnHitFunc; // from MoveEventMethods
using OnSourceImmunityFunc = std::function<void(Battle*, std::string, Pokemon*)>;
using OnSourceLockMoveFunc = std::variant<std::string, std::function<std::variant<std::monostate, std::string>(Battle*, Pokemon*)>>;
using OnSourceMaybeTrapPokemonFunc = std::function<void(Battle*, Pokemon*)>;
// using OnSourceModifyAccuracyFunc = ModifierMoveFunc; // from CommonHandlers
// using OnSourceModifyAtkFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnSourceModifyBoostFunc = std::function<std::variant<SparseBoostsTable, std::monostate>(Battle*, const SparseBoostsTable&, Pokemon*)>;
// using OnSourceModifyCritRatioFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnSourceModifyDamageFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnSourceModifyDefFunc = ModifierMoveFunc; // from CommonHandlers
// using OnSourceModifyMoveFunc = OnModifyMoveFunc; // from MoveEventMethods
// using OnSourceModifyPriorityFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnSourceModifySecondariesFunc = std::function<void(Battle*, std::vector<SecondaryEffect>, Pokemon*, Pokemon*, ActiveMove*)>;

// using OnSourceModifySpAFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnSourceModifySpDFunc = ModifierMoveFunc; // from CommonHandlers
using OnSourceModifySpeFunc = std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*)>;
// using OnSourceModifySTABFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnSourceModifyTypeFunc = OnModifyTypeFunc; // from MoveEventMethods
// using OnSourceModifyTargetFunc = OnModifyTargetFunc; // from MoveEventMethods
using OnSourceModifyWeightFunc = std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*)>;
// using OnSourceMoveAbortedFunc = VoidMoveFunc; // from CommonHandlers
using OnSourceNegateImmunityFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, std::string)>>;
using OnSourceOverrideActionFunc = std::function<std::variant<std::string, std::monostate>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
// using OnSourcePrepareHitFunc = ResultSourceMoveFunc; // from CommonHandlers
using OnSourceRedirectTargetFunc = std::function<std::variant<Pokemon*, std::monostate>(Battle*, Pokemon*, Pokemon*, Effect, ActiveMove*)>;
using OnSourceResidualFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using OnSourceSetAbilityFunc = std::function<std::variant<std::monostate, void>(Battle*, std::string, Pokemon*, Pokemon*, Effect)>;
using OnSourceSetStatusFunc = std::function<std::variant<bool, std::monostate>(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;

using OnSourceSetWeatherFunc = std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, Pokemon*, Condition*)>;
using OnSourceStallMoveFunc = std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*)>;
using OnSourceSwitchOutFunc = std::function<void(Battle*, Pokemon*)>;
using OnSourceTakeItemFunc = std::variant<std::function<std::variant<bool, std::monostate>(Battle*, Item*, Pokemon*, Pokemon*, ActiveMove*)>, bool>;
using OnSourceTerrainFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using OnSourceTrapPokemonFunc = std::function<void(Battle*, Pokemon*)>;
using OnSourceTryAddVolatileFunc = std::function<std::variant<bool, std::nullptr_t, std::monostate>(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
using OnSourceTryEatItemFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Item*, Pokemon*)>>;
using OnSourceTryHealFunc = std::function<std::variant<int, bool, std::nullptr_t, std::monostate>(Battle*, int, Pokemon*, Pokemon*, Effect)>;
// using OnSourceTryHitFunc = OnTryHitFunc; // from MoveEventMethods
// using OnSourceTryHitFieldFunc = OnTryHitFieldFunc; // from MoveEventMethods
// using OnSourceTryHitSideFunc = ResultMoveFunc; // from CommonHandlers
// using OnSourceInvulnerabilityFunc = ExtResultMoveFunc; // from CommonHandlers
// using OnSourceTryMoveFunc = OnTryMoveFunc; // from MoveEventMethods
using OnSourceTryPrimaryHitFunc = std::function<std::variant<bool, std::nullptr_t, int, std::monostate>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;

using OnSourceTypeFunc = std::function<std::variant<std::vector<std::string>, std::monostate>(Battle*, std::vector<std::string>, Pokemon*)>;
// using OnSourceWeatherModifyDamageFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnSourceModifyDamagePhase1Func = ModifierSourceMoveFunc; // from CommonHandlers
// using OnSourceModifyDamagePhase2Func = ModifierSourceMoveFunc; // from CommonHandlers
using OnAnyDamagingHitFunc = std::function<void(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;
using OnAnyAfterEachBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*)>;
// using OnAnyAfterHitFunc = OnAfterHitFunc; // from MoveEventMethods
using OnAnyAfterSetStatusFunc = std::function<void(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
// using OnAnyAfterSubDamageFunc = OnAfterSubDamageFunc; // from MoveEventMethods
using OnAnyAfterSwitchInSelfFunc = std::function<void(Battle*, Pokemon*)>;
using OnAnyAfterUseItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
using OnAnyAfterBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*, Effect)>;
using OnAnyAfterFaintFunc = std::function<void(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using OnAnyAfterMegaFunc = std::function<void(Battle*, Pokemon*)>;

// using OnAnyAfterMoveSecondarySelfFunc = OnAfterMoveSecondarySelfFunc; // from MoveEventMethods
// using OnAnyAfterMoveSecondaryFunc = OnAfterMoveSecondaryFunc; // from MoveEventMethods
// using OnAnyAfterMoveFunc = OnAfterMoveFunc; // from MoveEventMethods
// using OnAnyAfterMoveSelfFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnAnyAttractFunc = std::function<void(Battle*, Pokemon*, Pokemon*)>;
using OnAnyAccuracyFunc = std::function<std::variant<int, bool, std::monostate>(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;
// using OnAnyBasePowerFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnAnyBeforeFaintFunc = std::function<void(Battle*, Pokemon*, Effect)>;
// using OnAnyBeforeMoveFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnAnyBeforeSwitchInFunc = std::function<void(Battle*, Pokemon*)>;
using OnAnyBeforeSwitchOutFunc = std::function<void(Battle*, Pokemon*)>;
using OnAnyTryBoostFunc = std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*, Effect)>;
// using OnAnyChargeMoveFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnAnyCriticalHitFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, std::nullptr_t, ActiveMove*)>>;

using OnAnyDamageFunc = std::function<std::variant<int, bool, std::monostate>(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using OnAnyDeductPPFunc = std::function<std::variant<int, std::monostate>(Battle*, Pokemon*, Pokemon*)>;
using OnAnyDisableMoveFunc = std::function<void(Battle*, Pokemon*)>;
using OnAnyDragOutFunc = std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnAnyEatItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
// using OnAnyEffectivenessFunc = OnEffectivenessFunc; // from MoveEventMethods
// using OnAnyFaintFunc = VoidEffectFunc; // from CommonHandlers
using OnAnyFlinchFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*)>>;
// using OnAnyHitFunc = OnHitFunc; // from MoveEventMethods
using OnAnyImmunityFunc = std::function<void(Battle*, std::string, Pokemon*)>;
using OnAnyLockMoveFunc = std::variant<std::string, std::function<std::variant<std::monostate, std::string>(Battle*, Pokemon*)>>;
using OnAnyMaybeTrapPokemonFunc = std::function<void(Battle*, Pokemon*)>;
// using OnAnyModifyAccuracyFunc = ModifierMoveFunc; // from CommonHandlers
// using OnAnyModifyAtkFunc = ModifierSourceMoveFunc; // from CommonHandlers

using OnAnyModifyBoostFunc = std::function<std::variant<SparseBoostsTable, std::monostate>(Battle*, const SparseBoostsTable&, Pokemon*)>;
// using OnAnyModifyCritRatioFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnAnyModifyDamageFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnAnyModifyDefFunc = ModifierMoveFunc; // from CommonHandlers
// using OnAnyModifyMoveFunc = OnModifyMoveFunc; // from MoveEventMethods
// using OnAnyModifyPriorityFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnAnyModifySecondariesFunc = std::function<void(Battle*, std::vector<SecondaryEffect>, Pokemon*, Pokemon*, ActiveMove*)>;
// using OnAnyModifySpAFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnAnyModifySpDFunc = ModifierMoveFunc; // from CommonHandlers
using OnAnyModifySpeFunc = std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*)>;
// using OnAnyModifySTABFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnAnyModifyTypeFunc = OnModifyTypeFunc; // from MoveEventMethods
// using OnAnyModifyTargetFunc = OnModifyTargetFunc; // from MoveEventMethods
using OnAnyModifyWeightFunc = std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*)>;
// using OnAnyMoveAbortedFunc = VoidMoveFunc; // from CommonHandlers

using OnAnyNegateImmunityFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, std::string)>>;
using OnAnyOverrideActionFunc = std::function<std::variant<std::string, std::monostate>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
// using OnAnyPrepareHitFunc = ResultSourceMoveFunc; // from CommonHandlers
using OnAnyPseudoWeatherChangeFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Condition*)>;
using OnAnyRedirectTargetFunc = std::function<std::variant<Pokemon*, std::monostate>(Battle*, Pokemon*, Pokemon*, Effect, ActiveMove*)>;
using OnAnyResidualFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using OnAnySetAbilityFunc = std::function<std::variant<std::monostate, void>(Battle*, std::string, Pokemon*, Pokemon*, Effect)>;
using OnAnySetStatusFunc = std::function<std::variant<bool, std::monostate>(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
using OnAnySetWeatherFunc = std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, Pokemon*, Condition*)>;
using OnAnyStallMoveFunc = std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*)>;
using OnAnySwitchInFunc = std::function<void(Battle*, Pokemon*)>;
using OnAnySwitchOutFunc = std::function<void(Battle*, Pokemon*)>;
using OnAnyTakeItemFunc = std::variant<std::function<std::variant<bool, std::monostate>(Battle*, Item*, Pokemon*, Pokemon*, ActiveMove*)>, bool>;

using OnAnyTerrainFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using OnAnyTrapPokemonFunc = std::function<void(Battle*, Pokemon*)>;
using OnAnyTryAddVolatileFunc = std::function<std::variant<bool, std::nullptr_t, std::monostate>(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
using OnAnyTryEatItemFunc = std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Item*, Pokemon*)>>;
using OnAnyTryHealFunc = std::function<std::variant<int, bool, std::nullptr_t, std::monostate>(Battle*, int, Pokemon*, Pokemon*, Effect)>;
// using OnAnyTryHitFunc = OnTryHitFunc; // from MoveEventMethods
// using OnAnyTryHitFieldFunc = OnTryHitFieldFunc; // from MoveEventMethods
// using OnAnyTryHitSideFunc = ResultMoveFunc; // from CommonHandlers
// using OnAnyInvulnerabilityFunc = ExtResultMoveFunc; // from CommonHandlers
// using OnAnyTryMoveFunc = OnTryMoveFunc; // from MoveEventMethods
using OnAnyTryPrimaryHitFunc = std::function<std::variant<bool, std::nullptr_t, int, std::monostate>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnAnyTypeFunc = std::function<std::variant<std::vector<std::string>, std::monostate>(Battle*, std::vector<std::string>, Pokemon*)>;
// using OnAnyWeatherModifyDamageFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnAnyModifyDamagePhase1Func = ModifierSourceMoveFunc; // from CommonHandlers
// using OnAnyModifyDamagePhase2Func = ModifierSourceMoveFunc; // from CommonHandlers