#pragma once

#include "EventMethods.h"
#include "pokemon_event_methods_aliases.h"
#include <optional>
#include <string>
#include <vector>

struct PokemonEventMethods : public EventMethods
{
	std::optional<OnAllyDamagingHitFunc> on_ally_damaging_hit = std::nullopt;
	std::optional<OnAllyAfterEachBoostFunc> on_ally_after_each_boost = std::nullopt;
	std::optional<OnAfterHitFunc> on_ally_after_hit = std::nullopt; // from MoveEventMethods
	std::optional<OnAllyAfterSetStatusFunc> on_ally_after_set_status = std::nullopt;
	std::optional<OnAfterSubDamageFunc> on_ally_after_sub_damage = std::nullopt; // from MoveEventMethods
	std::optional<OnAllyAfterSwitchInSelfFunc> on_ally_after_switch_in_self = std::nullopt;
	std::optional<OnAllyAfterUseItemFunc> on_ally_after_use_item = std::nullopt;
	std::optional<OnAllyAfterBoostFunc> on_ally_after_boost = std::nullopt;
	std::optional<OnAllyAfterFaintFunc> on_ally_after_faint = std::nullopt;
	std::optional<OnAfterMoveSecondarySelfFunc> on_ally_after_move_secondary_self = std::nullopt; // from MoveEventMethods
	std::optional<OnAfterMoveSecondaryFunc> on_ally_after_move_secondary = std::nullopt; // from MoveEventMethods
	std::optional<OnAfterMoveFunc> on_ally_after_move = std::nullopt; // from MoveEventMethods
	std::optional<VoidSourceMoveFunc> on_ally_after_move_self = std::nullopt; // from CommonHandlers
	std::optional<OnAllyAttractFunc> on_ally_attract = std::nullopt;
	std::optional<OnAllyAccuracyFunc> on_ally_accuracy = std::nullopt;

	std::optional<ModifierSourceMoveFunc> on_ally_base_power = std::nullopt; // from CommonHandlers
	std::optional<OnAllyBeforeFaintFunc> on_ally_before_faint = std::nullopt;
	std::optional<VoidSourceMoveFunc> on_ally_before_move = std::nullopt; // from CommonHandlers
	std::optional<OnAllyBeforeSwitchInFunc> on_ally_before_switch_in = std::nullopt;
	std::optional<OnAllyBeforeSwitchOutFunc> on_ally_before_switch_out = std::nullopt;
	std::optional<OnAllyTryBoostFunc> on_ally_try_boost = std::nullopt;
	std::optional<VoidSourceMoveFunc> on_ally_charge_move = std::nullopt; // from CommonHandlers
	std::optional<OnAllyCriticalHitFunc> on_ally_critical_hit = std::nullopt;
	std::optional<OnAllyDamageFunc> on_ally_damage = std::nullopt;
	std::optional<OnAllyDeductPPFunc> on_ally_deduct_pp = std::nullopt;
	std::optional<OnAllyDisableMoveFunc> on_ally_disable_move = std::nullopt;
	std::optional<OnAllyDragOutFunc> on_ally_drag_out = std::nullopt;
	std::optional<OnAllyEatItemFunc> on_ally_eat_item = std::nullopt;
	std::optional<OnEffectivenessFunc> on_ally_effectiveness = std::nullopt; // from MoveEventMethods

	std::optional<VoidEffectFunc> on_ally_faint = std::nullopt; // from CommonHandlers
	std::optional<OnAllyFlinchFunc> on_ally_flinch = std::nullopt;
	std::optional<OnHitFunc> on_ally_hit = std::nullopt; // from MoveEventMethods
	std::optional<OnAllyImmunityFunc> on_ally_immunity = std::nullopt;
	std::optional<OnAllyLockMoveFunc> on_ally_lock_move = std::nullopt;
	std::optional<OnAllyMaybeTrapPokemonFunc> on_ally_maybe_trap_pokemon = std::nullopt;
	std::optional<ModifierMoveFunc> on_ally_modify_accuracy = std::nullopt; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_ally_modify_atk = std::nullopt; // from CommonHandlers
	std::optional<OnAllyModifyBoostFunc> on_ally_modify_boost = std::nullopt;
	std::optional<ModifierSourceMoveFunc> on_ally_modify_crit_ratio = std::nullopt; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_ally_modify_damage = std::nullopt; // from CommonHandlers
	std::optional<ModifierMoveFunc> on_ally_modify_def = std::nullopt; // from CommonHandlers
	std::optional<OnModifyMoveFunc> on_ally_modify_move = std::nullopt; // from MoveEventMethods
	std::optional<ModifierSourceMoveFunc> on_ally_modify_priority = std::nullopt; // from CommonHandlers
	std::optional<OnAllyModifySecondariesFunc> on_ally_modify_secondaries = std::nullopt;

	std::optional<ModifierSourceMoveFunc> on_ally_modify_spa = std::nullopt; // from CommonHandlers
	std::optional<ModifierMoveFunc> on_ally_modify_spd = std::nullopt; // from CommonHandlers
	std::optional<OnAllyModifySpeFunc> on_ally_modify_spe = std::nullopt;
	std::optional<ModifierSourceMoveFunc> on_ally_modify_stab = std::nullopt; // from CommonHandlers
	std::optional<OnModifyTypeFunc> on_ally_modify_type = std::nullopt; // from MoveEventMethods
	std::optional<OnModifyTargetFunc> on_ally_modify_target = std::nullopt; // from MoveEventMethods
	std::optional<OnAllyModifyWeightFunc> on_ally_modify_weight = std::nullopt;
	std::optional<VoidMoveFunc> on_ally_move_aborted = std::nullopt; // from CommonHandlers
	std::optional<OnAllyNegateImmunityFunc> on_ally_negate_immunity = std::nullopt;
	std::optional<OnAllyOverrideActionFunc> on_ally_override_action = std::nullopt;
	std::optional<ResultSourceMoveFunc> on_ally_prepare_hit = std::nullopt; // from CommonHandlers
	std::optional<OnAllyRedirectTargetFunc> on_ally_redirect_target = std::nullopt;
	std::optional<OnAllyResidualFunc> on_ally_residual = std::nullopt;
	std::optional<OnAllySetAbilityFunc> on_ally_set_ability = std::nullopt;

	std::optional<OnAllySetStatusFunc> on_ally_set_status = std::nullopt;
	std::optional<OnAllySetWeatherFunc> on_ally_set_weather = std::nullopt;
	std::optional<OnAllyStallMoveFunc> on_ally_stall_move = std::nullopt;
	std::optional<OnAllySwitchOutFunc> on_ally_switch_out = std::nullopt;
	std::optional<OnAllyTakeItemFunc> on_ally_take_item = std::nullopt;
	std::optional<OnAllyTerrainFunc> on_ally_terrain_change = std::nullopt;
	std::optional<OnAllyTrapPokemonFunc> on_ally_trap_pokemon = std::nullopt;
	std::optional<OnAllyTryAddVolatileFunc> on_ally_try_add_volatile = std::nullopt;
	std::optional<OnAllyTryEatItemFunc> on_ally_try_eat_item = std::nullopt;
	std::optional<OnAllyTryHealFunc> on_ally_try_heal = std::nullopt;

	std::optional<OnTryHitFunc> on_ally_try_hit = std::nullopt; // from MoveEventMethods
	std::optional<OnTryHitFieldFunc> on_ally_try_hit_field = std::nullopt; // from MoveEventMethods
	std::optional<ResultMoveFunc> on_ally_try_hit_side = std::nullopt; // from CommonHandlers
	std::optional<ExtResultMoveFunc> on_ally_invulnerability = std::nullopt; // from CommonHandlers
	std::optional<OnTryMoveFunc> on_ally_try_move = std::nullopt; // from MoveEventMethods
	std::optional<OnAllyTryPrimaryHitFunc> on_ally_try_primary_hit = std::nullopt;
	std::optional<OnTypeFunc> on_ally_type = std::nullopt; // from MoveEventMethods
	std::optional<ModifierSourceMoveFunc> on_ally_weather_modify_damage = std::nullopt; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_ally_modify_damage_phase_1 = std::nullopt; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_ally_modify_damage_phase_2 = std::nullopt; // from CommonHandlers
};