#pragma once

#include "EventMethods.h"
#include "pokemon_event_methods_aliases.h"

struct PokemonEventMethods : public EventMethods
{
	OnAllyDamagingHitFunc on_ally_damaging_hit = nullptr; // optional
	OnAllyAfterEachBoostFunc on_ally_after_each_boost = nullptr; // optional
	OnAfterHitFunc on_ally_after_hit = nullptr; // optional // from MoveEventMethods
	OnAllyAfterSetStatusFunc on_ally_after_set_status = nullptr; // optional
	OnAfterSubDamageFunc on_ally_after_sub_damage = nullptr; // optional // from MoveEventMethods
	OnAllyAfterSwitchInSelfFunc on_ally_after_switch_in_self = nullptr; // optional
	OnAllyAfterUseItemFunc on_ally_after_use_item = nullptr; // optional
	OnAllyAfterBoostFunc on_ally_after_boost = nullptr; // optional
	OnAllyAfterFaintFunc on_ally_after_faint = nullptr; // optional
	OnAfterMoveSecondarySelfFunc on_ally_after_move_secondary_self = nullptr; // optional // from MoveEventMethods
	OnAfterMoveSecondaryFunc on_ally_after_move_secondary = nullptr; // optional // from MoveEventMethods
	OnAfterMoveFunc on_ally_after_move = nullptr; // optional // from MoveEventMethods
	VoidSourceMoveFunc on_ally_after_move_self = nullptr; // optional // from CommonHandlers
	OnAllyAttractFunc on_ally_attract = nullptr; // optional
	OnAllyAccuracyFunc on_ally_accuracy = nullptr; // optional

	ModifierSourceMoveFunc on_ally_base_power = nullptr; // optional // from CommonHandlers
	OnAllyBeforeFaintFunc on_ally_before_faint = nullptr; // optional
	VoidSourceMoveFunc on_ally_before_move = nullptr; // optional // from CommonHandlers
	OnAllyBeforeSwitchInFunc on_ally_before_switch_in = nullptr; // optional
	OnAllyBeforeSwitchOutFunc on_ally_before_switch_out = nullptr; // optional
	OnAllyTryBoostFunc on_ally_try_boost = nullptr; // optional
	VoidSourceMoveFunc on_ally_charge_move = nullptr; // optional // from CommonHandlers
	OnAllyCriticalHitFunc on_ally_critical_hit = nullptr; // optional
	OnAllyDamageFunc on_ally_damage = nullptr; // optional
	OnAllyDeductPPFunc on_ally_deduct_pp = nullptr; // optional
	OnAllyDisableMoveFunc on_ally_disable_move = nullptr; // optional
	OnAllyDragOutFunc on_ally_drag_out = nullptr; // optional
	OnAllyEatItemFunc on_ally_eat_item = nullptr; // optional
	OnEffectivenessFunc on_ally_effectiveness = nullptr; // optional // from MoveEventMethods

	VoidEffectFunc on_ally_faint = nullptr; // optional // from CommonHandlers
	OnAllyFlinchFunc on_ally_flinch = nullptr; // optional
	OnHitFunc on_ally_hit = nullptr; // optional // from MoveEventMethods
	OnAllyImmunityFunc on_ally_immunity = nullptr; // optional
	std::unique_ptr<OnAllyLockMoveFunc> on_ally_lock_move = nullptr; // optional
	OnAllyMaybeTrapPokemonFunc on_ally_maybe_trap_pokemon = nullptr; // optional
	ModifierMoveFunc on_ally_modify_accuracy = nullptr; // optional // from CommonHandlers
	ModifierSourceMoveFunc on_ally_modify_atk = nullptr; // optional // from CommonHandlers
	OnAllyModifyBoostFunc on_ally_modify_boost = nullptr; // optional
	ModifierSourceMoveFunc on_ally_modify_crit_ratio = nullptr; // optional // from CommonHandlers
	ModifierSourceMoveFunc on_ally_modify_damage = nullptr; // optional // from CommonHandlers
	ModifierMoveFunc on_ally_modify_def = nullptr; // optional // from CommonHandlers
	OnModifyMoveFunc on_ally_modify_move = nullptr; // optional // from MoveEventMethods
	ModifierSourceMoveFunc on_ally_modify_priority = nullptr; // optional // from CommonHandlers
	OnAllyModifySecondariesFunc on_ally_modify_secondaries = nullptr; // optional

	ModifierSourceMoveFunc on_ally_modify_spa = nullptr; // optional // from CommonHandlers
	ModifierMoveFunc on_ally_modify_spd = nullptr; // optional // from CommonHandlers
	OnAllyModifySpeFunc on_ally_modify_spe = nullptr; // optional
	ModifierSourceMoveFunc on_ally_modify_stab = nullptr; // optional // from CommonHandlers
	OnModifyTypeFunc on_ally_modify_type = nullptr; // optional // from MoveEventMethods
	OnModifyTargetFunc on_ally_modify_target = nullptr; // optional // from MoveEventMethods
	OnAllyModifyWeightFunc on_ally_modify_weight = nullptr; // optional
	VoidMoveFunc on_ally_move_aborted = nullptr; // optional // from CommonHandlers
	OnAllyNegateImmunityFunc on_ally_negate_immunity = nullptr; // optional
	OnAllyOverrideActionFunc on_ally_override_action = nullptr; // optional
	ResultSourceMoveFunc on_ally_prepare_hit = nullptr; // optional // from CommonHandlers
	OnAllyRedirectTargetFunc on_ally_redirect_target = nullptr; // optional
	OnAllyResidualFunc on_ally_residual = nullptr; // optional
	OnAllySetAbilityFunc on_ally_set_ability = nullptr; // optional

	OnAllySetStatusFunc on_ally_set_status = nullptr; // optional
	OnAllySetWeatherFunc on_ally_set_weather = nullptr; // optional
	OnAllyStallMoveFunc on_ally_stall_move = nullptr; // optional
	OnAllySwitchOutFunc on_ally_switch_out = nullptr; // optional
	OnAllyTakeItemFunc on_ally_take_item = nullptr; // optional
	OnAllyTerrainFunc on_ally_terrain_change = nullptr; // optional
	OnAllyTrapPokemonFunc on_ally_trap_pokemon = nullptr; // optional
	OnAllyTryAddVolatileFunc on_ally_try_add_volatile = nullptr; // optional
	OnAllyTryEatItemFunc on_ally_try_eat_item = nullptr; // optional
	std::unique_ptr<OnAllyTryHealFunc> on_ally_try_heal = nullptr; // optional

	OnTryHitFunc on_ally_try_hit = nullptr; // optional // from MoveEventMethods
	OnTryHitFieldFunc on_ally_try_hit_field = nullptr; // optional // from MoveEventMethods
	ResultMoveFunc on_ally_try_hit_side = nullptr; // optional // from CommonHandlers
	ExtResultMoveFunc on_ally_invulnerability = nullptr; // optional // from CommonHandlers
	OnTryMoveFunc on_ally_try_move = nullptr; // optional // from MoveEventMethods
	OnAllyTryPrimaryHitFunc on_ally_try_primary_hit = nullptr; // optional
	OnTypeFunc on_ally_type = nullptr; // optional // from MoveEventMethods
	ModifierSourceMoveFunc on_ally_weather_modify_damage = nullptr; // optional // from CommonHandlers
	ModifierSourceMoveFunc on_ally_modify_damage_phase_1 = nullptr; // optional // from CommonHandlers
	ModifierSourceMoveFunc on_ally_modify_damage_phase_2 = nullptr; // optional // from CommonHandlers
};
