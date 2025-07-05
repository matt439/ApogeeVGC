#pragma once

#include "GlobalTypes.h"
#include "rapidjson/document.h"
#include <functional>
#include <optional>
#include <variant>
#include <string>

// Forward declarations
class Battle;
class Pokemon;
class Side;
class ActiveMove;

// Example union types for return values
using NumberOrBoolOrNull = std::variant<int, bool, std::monostate>;
using NumberOrBoolOrNullOrVoid = std::variant<int, bool, std::monostate>; // void can be represented by std::monostate
using BoolOrVoid = std::variant<bool, std::monostate>;
using BoolOrNullOrStringOrVoid = std::variant<bool, std::monostate, std::string>;
using BoolOrNullOrNumberOrStringOrVoid = std::variant<bool, std::monostate, int, std::string>;

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

struct MoveEventMethods
{
    BasePowerCallbackFunc base_power_callback;
    BeforeMoveCallbackFunc before_move_callback;
    BeforeTurnCallbackFunc before_turn_callback;
    DamageCallbackFunc damage_callback;
    PriorityChargeCallbackFunc priority_charge_callback;

    OnDisableMoveFunc on_disable_move;
    OnAfterHitFunc on_after_hit;
    OnAfterSubDamageFunc on_after_sub_damage;
    OnAfterMoveSecondarySelfFunc on_after_move_secondary_self;
    OnAfterMoveSecondaryFunc on_after_move_secondary;
    OnAfterMoveFunc on_after_move;

    std::optional<int> on_damage_priority;
    OnDamageFunc on_damage;

    OnBasePowerFunc on_base_power;
    OnEffectivenessFunc on_effectiveness;
    OnHitFunc on_hit;
    OnHitFieldFunc on_hit_field;
    OnHitSideFunc on_hit_side;
    OnModifyMoveFunc on_modify_move;
    OnModifyPriorityFunc on_modify_priority;
    OnMoveFailFunc on_move_fail;
    OnModifyTypeFunc on_modify_type;
    OnModifyTargetFunc on_modify_target;
    OnPrepareHitFunc on_prepare_hit;
    OnTryFunc on_try;
    OnTryHitFunc on_try_hit;
    OnTryHitFieldFunc on_try_hit_field;
    OnTryHitSideFunc on_try_hit_side;
    OnTryImmunityFunc on_try_immunity;
    OnTryMoveFunc on_try_move;
    OnUseMoveMessageFunc on_use_move_message;
};

