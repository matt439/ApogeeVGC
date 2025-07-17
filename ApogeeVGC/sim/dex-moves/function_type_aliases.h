#pragma once

#include "../global-types/Effect.h"
//#include "../battle/Battle.h"
//#include "../pokemon/Pokemon.h"
//#include "../side/Side.h"
#include "type_aliases.h"
//#include "ActiveMove.h"
#include <rapidjson/document.h>
#include <functional>
#include <variant>

class Battle;
class Pokemon;
class Side;
class ActiveMove;
class Item;
class Condition;

// Callback function type aliases for MoveEventMethods
using BasePowerCallbackFunc = std::function<std::variant<int, bool, std::monostate>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using BeforeMoveCallbackFunc = std::function<BoolOrVoid(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using BeforeTurnCallbackFunc = std::function<void(Battle*, Pokemon*, Pokemon*)>;
using DamageCallbackFunc = std::function<std::variant<int, bool>(Battle*, Pokemon*, Pokemon*)>;
using PriorityChargeCallbackFunc = std::function<void(Battle*, Pokemon*)>;
using OnDisableMoveFunc = std::function<void(Battle*, Pokemon*)>;
using OnAfterHitFunc = std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnAfterSubDamageFunc = std::function<void(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;
using OnAfterMoveSecondarySelfFunc = std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnAfterMoveSecondaryFunc = std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnAfterMoveFunc = std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnDamageFunc = std::function<NumberOrBoolOrNullOrVoid(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using OnBasePowerFunc = std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;
using OnEffectivenessFunc = std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*, Pokemon*, std::string, ActiveMove*)>;
using OnHitFunc = std::function<BoolOrNullOrStringOrVoid(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnHitFieldFunc = std::function<BoolOrNullOrStringOrVoid(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnHitSideFunc = std::function<BoolOrNullOrStringOrVoid(Battle*, Side*, Pokemon*, ActiveMove*)>;
using OnModifyMoveFunc = std::function<void(Battle*, ActiveMove*, Pokemon*, Pokemon*)>;
using OnModifyPriorityFunc = std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;
using OnMoveFailFunc = std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnModifyTypeFunc = std::function<void(Battle*, ActiveMove*, Pokemon*, Pokemon*)>;
using OnModifyTargetFunc = std::function<void(Battle*, const rapidjson::Value&, Pokemon*, Pokemon*, ActiveMove*)>;
using OnPrepareHitFunc = std::function<BoolOrNullOrStringOrVoid(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnTryFunc = std::function<BoolOrNullOrStringOrVoid(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnTryHitFunc = std::function<BoolOrNullOrNumberOrStringOrVoid(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnTryHitFieldFunc = std::function<BoolOrNullOrStringOrVoid(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnTryHitSideFunc = std::function<BoolOrNullOrStringOrVoid(Battle*, Side*, Pokemon*, ActiveMove*)>;
using OnTryImmunityFunc = std::function<BoolOrNullOrStringOrVoid(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnTryMoveFunc = std::function<BoolOrNullOrStringOrVoid(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using OnUseMoveMessageFunc = std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;