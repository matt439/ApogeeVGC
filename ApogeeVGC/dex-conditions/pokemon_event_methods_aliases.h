#pragma once

#include "../global-types/Effect.h"
#include "../global-types/SparseBoostsTable.h"
//#include "../battle/Battle.h"
//#include "../pokemon/Pokemon.h"
//#include "../dex-moves/ActiveMove.h"
//#include "../dex-items/Item.h"
//#include "../dex-conditions/Condition.h"
//#include "../side/Side.h"
#include <functional>
#include <string>
#include <variant>
#include <vector>

class Battle;
class Pokemon;
class Item;
class ActiveMove;
class Condition;
class Side;

using OnAllyDamagingHitFunc = std::function<void(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;
using OnAllyAfterEachBoostFunc = std::function<void(Battle*, SparseBoostsTable*, Pokemon*, Pokemon*)>;
// using OnAllyAfterHitFunc = OnAfterHitFunc; // from MoveEventMethods
using OnAllyAfterSetStatusFunc = std::function<void(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
// using OnAllyAfterSubDamageFunc = OnAfterSubDamageFunc; // from MoveEventMethods
using OnAllyAfterSwitchInSelfFunc = std::function<void(Battle*, Pokemon*)>;
using OnAllyAfterUseItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
using OnAllyAfterBoostFunc = std::function<void(Battle*, SparseBoostsTable*, Pokemon*, Pokemon*, Effect)>;
using OnAllyAfterFaintFunc = std::function<void(Battle*, int, Pokemon*, Pokemon*, Effect)>;
// using OnAllyAfterMoveSecondarySelfFunc = OnAfterMoveSecondarySelfFunc; // from MoveEventMethods
// using OnAllyAfterMoveSecondaryFunc = OnAfterMoveSecondaryFunc; // from MoveEventMethods
// using OnAllyAfterMoveFunc = OnAfterMoveFunc; // from MoveEventMethods
// using OnAllyAfterMoveSelfFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnAllyAttractFunc = std::function<void(Battle*, Pokemon*, Pokemon*)>;
using OnAllyAccuracyFunc = std::function<std::variant<int, bool, std::nullptr_t, void>(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;

// using OnAllyBasePowerFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnAllyBeforeFaintFunc = std::function<void(Battle*, Pokemon*, Effect)>;
// using OnAllyBeforeMoveFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnAllyBeforeSwitchInFunc = std::function<void(Battle*, Pokemon*)>;
using OnAllyBeforeSwitchOutFunc = std::function<void(Battle*, Pokemon*)>;
using OnAllyTryBoostFunc = std::function<void(Battle*, SparseBoostsTable*, Pokemon*, Pokemon*, Effect)>;
// using OnAllyChargeMoveFunc = VoidSourceMoveFunc; // from CommonHandlers
using OnAllyCriticalHitFunc = std::function<std::variant<bool, std::nullptr_t>(Battle*, Pokemon*, std::nullptr_t, ActiveMove*)>;
using OnAllyDamageFunc = std::function<std::variant<int, bool, std::nullptr_t, void>(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using OnAllyDeductPPFunc = std::function<std::variant<int, void>(Battle*, Pokemon*, Pokemon*)>;
using OnAllyDisableMoveFunc = std::function<void(Battle*, Pokemon*)>;
using OnAllyDragOutFunc = std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnAllyEatItemFunc = std::function<void(Battle*, Item*, Pokemon*)>;
// using OnAllyEffectivenessFunc = OnEffectivenessFunc; // from MoveEventMethods

// using OnAllyFaintFunc = VoidEffectFunc; // from CommonHandlers
using OnAllyFlinchFunc = std::function<std::variant<bool, void>(Battle*, Pokemon*)>;
// using OnAllyHitFunc = OnHitFunc; // from MoveEventMethods
using OnAllyImmunityFunc = std::function<void(Battle*, std::string, Pokemon*)>;
using OnAllyLockMoveFunc = std::variant<std::string, std::function<std::variant<void, std::string>(Battle*, Pokemon*)>>;
using OnAllyMaybeTrapPokemonFunc = std::function<void(Battle*, Pokemon*)>;
// using OnAllyModifyAccuracyFunc = ModifierMoveFunc; // from CommonHandlers
// using OnAllyModifyAtkFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnAllyModifyBoostFunc = std::function<std::variant<SparseBoostsTable*, void>(Battle*, SparseBoostsTable*, Pokemon*)>;
// using OnAllyModifyCritRatioFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnAllyModifyDamageFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnAllyModifyDefFunc = ModifierMoveFunc; // from CommonHandlers
// using OnAllyModifyMoveFunc = OnModifyMoveFunc; // from MoveEventMethods
// using OnAllyModifyPriorityFunc = ModifierSourceMoveFunc; // from CommonHandlers
using OnAllyModifySecondariesFunc = std::function<void(Battle*, std::vector<SecondaryEffect>&, Pokemon*, Pokemon*, ActiveMove*)>;

// using OnAllyModifySpAFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnAllyModifySpDFunc = ModifierMoveFunc; // from CommonHandlers
using OnAllyModifySpeFunc = std::function<std::variant<int, void>(Battle*, int, Pokemon*)>;
// using OnAllyModifySTABFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnAllyModifyTypeFunc = OnModifyTypeFunc; // from MoveEventMethods
// using OnAllyModifyTargetFunc = OnModifyTargetFunc; // from MoveEventMethods
using OnAllyModifyWeightFunc = std::function<std::variant<int, void>(Battle*, int, Pokemon*)>;
// using OnAllyMoveAbortedFunc = VoidMoveFunc; // from CommonHandlers
using OnAllyNegateImmunityFunc = std::variant<std::function<std::variant<bool, void>(Battle*, Pokemon*, std::string)>, bool>;
using OnAllyOverrideActionFunc = std::function<std::variant<std::string, void>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
// using OnAllyPrepareHitFunc = ResultSourceMoveFunc; // from CommonHandlers
using OnAllyRedirectTargetFunc = std::function<std::variant<Pokemon*, void>(Battle*, Pokemon*, Pokemon*, Effect, ActiveMove*)>;
using OnAllyResidualFunc = std::function<void(Battle*, Pokemon&, Pokemon*, Effect)>;
using OnAllySetAbilityFunc = std::function<std::variant<bool, void>(Battle*, std::string, Pokemon*, Pokemon*, Effect)>;

using OnAllySetStatusFunc = std::function<std::variant<bool, std::nullptr_t, void>(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
using OnAllySetWeatherFunc = std::function<std::variant<bool, void>(Battle*, Pokemon*, Pokemon*, Condition*)>;
using OnAllyStallMoveFunc = std::function<std::variant<bool, void>(Battle*, Pokemon*)>;
using OnAllySwitchOutFunc = std::function<void(Battle*, Pokemon*)>;
using OnAllyTakeItemFunc = std::variant<std::function<std::variant<bool, void>(Battle*, Item*, Pokemon*, Pokemon*, ActiveMove*)>, bool>;
using OnAllyTerrainFunc = std::function<void(Battle*, Pokemon*)>;
using OnAllyTrapPokemonFunc = std::function<void(Battle*, Pokemon*)>;
using OnAllyTryAddVolatileFunc = std::function<std::variant<bool, std::nullptr_t, void>(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>;
using OnAllyTryEatItemFunc = std::variant<bool, std::function<std::variant<bool, void>(Battle*, Item*, Pokemon*)>>;
using OnAllyTryHealFunc = std::variant<
	std::function<std::variant<int, bool, void>(Battle*, int, Pokemon*, Pokemon*, Effect)>,
	std::function<std::variant<bool, void>(Battle*, Pokemon*)>,
	bool>;

// using OnAllyTryHitFunc = OnTryHitFunc; // from MoveEventMethods
// using OnAllyTryHitFieldFunc = OnTryHitFieldFunc; // from MoveEventMethods
// using OnAllyTryHitSideFunc = ResultMoveFunc; // from CommonHandlers
// using OnAllyInvulnerabilityFunc = ExtResultMoveFunc; // from CommonHandlers
// using OnAllyTryMoveFunc = OnTryMoveFunc; // from MoveEventMethods
using OnAllyTryPrimaryHitFunc = std::function<std::variant<bool, std::nullptr_t, int, void>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
// using OnAllyTypeFunc = OnTypeFunc; // from MoveEventMethods
// using OnAllyWeatherModifyDamageFunc = ModifierSourceMoveFunc; // from CommonHandlers
// using OnAllyModifyDamagePhase1Func = ModifierSourceMoveFunc; // from CommonHandlers
// using OnAllyModifyDamagePhase2Func = ModifierSourceMoveFunc; // from CommonHandlers