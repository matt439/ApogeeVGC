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

// The struct
struct MoveEventMethods {
    // Optional callbacks
    std::function<std::variant<int, bool, std::monostate>(Battle*, Pokemon*, Pokemon*, ActiveMove*)> base_power_callback;
    std::function<BoolOrVoid(Battle*, Pokemon*, Pokemon*, ActiveMove*)> before_move_callback;
    std::function<void(Battle*, Pokemon*, Pokemon*)> before_turn_callback;
    std::function<std::variant<int, bool>(Battle*, Pokemon*, Pokemon*)> damage_callback;
    std::function<void(Battle*, Pokemon*)> priority_charge_callback;

    std::function<void(Battle*, Pokemon*)> on_disable_move;

    // These types are defined in CommonHandlers, so use those typedefs if available
    std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)> on_after_hit;
    std::function<void(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)> on_after_sub_damage;
    std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)> on_after_move_secondary_self;
    std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)> on_after_move_secondary;
    std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)> on_after_move;

    std::optional<int> on_damage_priority;
    std::function<NumberOrBoolOrNullOrVoid(Battle*, int, Pokemon*, Pokemon*, Effect)> on_damage;

    std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)> on_base_power;

    std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*, Pokemon*, std::string, ActiveMove*)> on_effectiveness;
    std::function<BoolOrNullOrStringOrVoid(Battle*, Pokemon*, Pokemon*, ActiveMove*)> on_hit;
    std::function<BoolOrNullOrStringOrVoid(Battle*, Pokemon*, Pokemon*, ActiveMove*)> on_hit_field;
    std::function<BoolOrNullOrStringOrVoid(Battle*, Side*, Pokemon*, ActiveMove*)> on_hit_side;
    std::function<void(Battle*, ActiveMove*, Pokemon*, Pokemon*)> on_modify_move;
    std::function<std::variant<int, std::monostate>(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)> on_modify_priority;
    std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)> on_move_fail;
    std::function<void(Battle*, ActiveMove*, Pokemon*, Pokemon*)> on_modify_type;
    std::function<void(Battle*, const rapidjson::Value&, Pokemon*, Pokemon*, ActiveMove*)> on_modify_target;
    std::function<BoolOrNullOrStringOrVoid(Battle*, Pokemon*, Pokemon*, ActiveMove*)> on_prepare_hit;
    std::function<BoolOrNullOrStringOrVoid(Battle*, Pokemon*, Pokemon*, ActiveMove*)> on_try;
    std::function<BoolOrNullOrNumberOrStringOrVoid(Battle*, Pokemon*, Pokemon*, ActiveMove*)> on_try_hit;
    std::function<BoolOrNullOrStringOrVoid(Battle*, Pokemon*, Pokemon*, ActiveMove*)> on_try_hit_field;
    std::function<BoolOrNullOrStringOrVoid(Battle*, Side*, Pokemon*, ActiveMove*)> on_try_hit_side;
    std::function<BoolOrNullOrStringOrVoid(Battle*, Pokemon*, Pokemon*, ActiveMove*)> on_try_immunity;
    std::function<BoolOrNullOrStringOrVoid(Battle*, Pokemon*, Pokemon*, ActiveMove*)> on_try_move;
    std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)> on_use_move_message;
};
