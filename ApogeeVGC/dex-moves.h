#pragma once

#include "global-types.h"
#include "rapidjson/document.h"
#include <optional>
#include <string>
#include <variant>
#include <functional>
#include <unordered_map>
#include <vector>
#include <any>
#include <array>

// Forward declarations
class Battle;
class Pokemon;
class Side;
class ActiveMove;

// union types for return values
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

struct HitEffect
{
    // Optional function, type depends on MoveEventMethods::on_hit signature  
    std::optional<OnHitFunc> on_hit;

    // set pokemon conditions  
    std::optional<SparseBoostsTable> boosts;
    std::optional<std::string> status;
    std::optional<std::string> volatile_status;

    // set side/slot conditions  
    std::optional<std::string> side_condition;
    std::optional<std::string> slot_condition;

    // set field conditions  
    std::optional<std::string> pseudo_weather;
    std::optional<std::string> terrain;
    std::optional<std::string> weather;
};

struct SecondaryEffect : public HitEffect
{
    std::optional<int> chance;                  // Probability of the effect
    std::optional<Ability*> ability;            // Pointer to Ability, optional
    std::optional<bool> kingsrock;              // Gen 2 specific mechanics flag
    std::optional<HitEffect> self;              // Nested HitEffect, optional
};

enum class MoveCategory
{
    PHYSICAL,
    SPECIAL,
    STATUS,
};

enum class SelfSwitchType
{
    COPY_VOLATILE,
    SHED_TAIL,
};

enum class SelfDestructType
{
    ALWAYS,
    IF_HIT,
};

struct ZMoveData
{
    std::optional<int> base_power;
    std::optional<IDEntry> effect;
    std::optional<SparseBoostsTable> boost;
};

struct MaxMoveData
{
    int base_power;
};

struct SelfBoost
{
    std::optional<SparseBoostsTable> boosts;
};

struct MoveData : public EffectData, MoveEventMethods, HitEffect
{
    std::string name;
    std::optional<int> num;
    // std::optional<ConditionData> condition; // Define ConditionData as needed
    int base_power;
    std::variant<bool, int> accuracy; // true or number
    int pp;
    MoveCategory category;
    std::string type;
    int priority;
    // MoveTarget target; // Define as needed
    // MoveFlags flags;   // Define as needed
    std::optional<std::string> real_move;

    std::variant<std::monostate, int, std::string, bool> damage; // number | 'level' | false | null
    std::optional<std::string> contest_type;
    std::optional<bool> no_pp_boosts;

    // Z-move data
    std::variant<bool, IDEntry> is_z;
    std::optional<ZMoveData> z_move;

    // Max move data
    std::variant<bool, std::string> is_max;
    std::optional<MaxMoveData> max_move;

    // Hit effects
    std::variant<bool, std::string> ohko; // boolean | 'Ice'
    std::optional<bool> thaws_target;
    std::optional<std::vector<int>> heal;
    std::optional<bool> force_switch;
    std::variant<SelfSwitchType, bool> self_switch;
    std::optional<SelfBoost> self_boost;
    std::variant<SelfDestructType, bool> selfdestruct;
    std::optional<bool> breaks_protect;
    std::optional<std::array<int, 2>> recoil;
    std::optional<std::array<int, 2>> drain;
    std::optional<bool> mind_blown_recoil;
    std::optional<bool> steals_boosts;
    std::optional<bool> struggle_recoil;
    std::optional<SecondaryEffect> secondary;
    std::optional<std::vector<SecondaryEffect>> secondaries;
    std::optional<SecondaryEffect> self;
    std::optional<bool> has_sheer_force;

    // Hit effect modifiers
    std::optional<bool> always_hit;
    std::optional<std::string> base_move_type;
    std::optional<int> base_power_modifier;
    std::optional<int> crit_modifier;
    std::optional<int> crit_ratio;
    std::optional<std::string> override_offensive_pokemon; // 'target' | 'source'
    std::optional<StatIDExceptHP> override_offensive_stat;
    std::optional<std::string> override_defensive_pokemon; // 'target' | 'source'
    std::optional<StatIDExceptHP> override_defensive_stat;
    std::optional<bool> force_stab;
    std::optional<bool> ignore_ability;
    std::optional<bool> ignore_accuracy;
    std::optional<bool> ignore_defensive;
    std::optional<bool> ignore_evasion;
    std::variant<bool, std::unordered_map<std::string, bool>> ignore_immunity;
    std::optional<bool> ignore_negative_offensive;
    std::optional<bool> ignore_offensive;
    std::optional<bool> ignore_positive_defensive;
    std::optional<bool> ignore_positive_evasion;
    std::optional<bool> multiaccuracy;
    std::variant<int, std::vector<int>> multihit;
    std::optional<std::string> multihit_type; // 'parentalbond'
    std::optional<bool> no_damage_variance;
    // MoveTarget non_ghost_target; // Define as needed
    std::optional<int> spread_modifier;
    std::optional<bool> sleep_usable;
    std::optional<bool> smart_target;
    std::optional<bool> tracks_target;
    std::optional<bool> will_crit;
    std::optional<bool> calls_move;

    // Mechanics flags
    std::optional<bool> has_crash_damage;
    std::optional<bool> is_confusion_self_hit;
    std::optional<bool> stalling_move;
    std::optional<ID> base_move;
};

//struct MutableMove : public BasicEffect, public MoveData
//{
//    // Additional fields or methods can be added here if needed
//};
//
//struct ActiveMove : public MutableMove
//{
//};

struct Move
{
	Move() = default;
};