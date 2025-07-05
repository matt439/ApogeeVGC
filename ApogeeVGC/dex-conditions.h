#pragma once

#include "global-types.h"
#include "dex-moves.h"
#include <functional>
#include <optional>
#include <variant>
#include <string>
#include <vector>

struct SecondaryEffect;

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


struct EventMethods {
    // Optional callbacks
    std::optional<std::function<void(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>> on_damaging_hit;
    std::optional<std::function<void(Battle*, Pokemon*)>> on_emergency_exit;
    std::optional<std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*, Effect)>> on_after_each_boost;
    OnAfterHitFunc on_after_hit;
    std::optional<std::function<void(Battle*, Pokemon*)>> on_after_mega;
    std::optional<std::function<void(Battle*, Condition*, Pokemon*, Pokemon*, Effect)>> on_after_set_status;
    OnAfterSubDamageFunc on_after_sub_damage;
    std::optional<std::function<void(Battle*, Pokemon*)>> on_after_switch_in_self;
    std::optional<std::function<void(Battle*, Pokemon*)>> on_after_terastallization;
    std::optional<std::function<void(Battle*, Item*, Pokemon*)>> on_after_use_item;
    std::optional<std::function<void(Battle*, Item*, Pokemon*)>> on_after_take_item;
    std::optional<std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*, Effect)>> on_after_boost;
    std::optional<std::function<void(Battle*, int, Pokemon*, Pokemon*, Effect)>> on_after_faint;
    OnAfterMoveSecondarySelfFunc on_after_move_secondary_self;
    OnAfterMoveSecondaryFunc on_after_move_secondary;
    OnAfterMoveFunc on_after_move;
    VoidSourceMoveFunc on_after_move_self;
    std::optional<std::function<void(Battle*, Pokemon*, Pokemon*)>> on_attract;
    std::optional<std::function<std::variant<int, bool, std::monostate>(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>> on_accuracy;
    ModifierSourceMoveFunc on_base_power;
    std::optional<std::function<void(Battle*, Pokemon*, Effect)>> on_before_faint;
    VoidSourceMoveFunc on_before_move;
    std::optional<std::function<void(Battle*, Pokemon*)>> on_before_switch_in;
    std::optional<std::function<void(Battle*, Pokemon*)>> on_before_switch_out;
    std::optional<std::function<void(Battle*, Pokemon*)>> on_before_turn;
    std::optional<std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*, Effect)>> on_change_boost;
    std::optional<std::function<void(Battle*, const SparseBoostsTable&, Pokemon*, Pokemon*, Effect)>> on_try_boost;
    VoidSourceMoveFunc on_charge_move;
    std::variant<bool, std::function<std::variant<bool, std::monostate>(Battle*, Pokemon*, std::nullptr_t, ActiveMove*)>> on_critical_hit;
    std::optional<std::function<std::variant<int, bool, std::monostate>(Battle*, int, Pokemon*, Pokemon*, Effect)>> on_damage;

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


};