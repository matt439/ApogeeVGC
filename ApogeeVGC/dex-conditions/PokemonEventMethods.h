#pragma once

#include "EventMethods.h"
#include "pokemon_event_methods_aliases.h"
#include <optional>
#include <string>
#include <vector>

struct PokemonEventMethods : public EventMethods
{
	std::optional<OnAllyDamagingHitFunc> on_ally_damaging_hit;
	std::optional<OnAllyAfterEachBoostFunc> on_ally_after_each_boost;
	std::optional<OnAfterHitFunc> on_ally_after_hit; // from MoveEventMethods
	std::optional<OnAllyAfterSetStatusFunc> on_ally_after_set_status;
	std::optional<OnAfterSubDamageFunc> on_ally_after_sub_damage; // from MoveEventMethods
	std::optional<OnAllyAfterSwitchInSelfFunc> on_ally_after_switch_in_self;
	std::optional<OnAllyAfterUseItemFunc> on_ally_after_use_item;
	std::optional<OnAllyAfterBoostFunc> on_ally_after_boost;
	std::optional<OnAllyAfterFaintFunc> on_ally_after_faint;
	std::optional<OnAfterMoveSecondarySelfFunc> on_ally_after_move_secondary_self; // from MoveEventMethods
	std::optional<OnAfterMoveSecondaryFunc> on_ally_after_move_secondary; // from MoveEventMethods
	std::optional<OnAfterMoveFunc> on_ally_after_move; // from MoveEventMethods
	std::optional<VoidSourceMoveFunc> on_ally_after_move_self; // from CommonHandlers
	std::optional<OnAllyAttractFunc> on_ally_attract;
	std::optional<OnAllyAccuracyFunc> on_ally_accuracy;

	std::optional<ModifierSourceMoveFunc> on_ally_base_power; // from CommonHandlers
	std::optional<OnAllyBeforeFaintFunc> on_ally_before_faint;
	std::optional<VoidSourceMoveFunc> on_ally_before_move; // from CommonHandlers
	std::optional<OnAllyBeforeSwitchInFunc> on_ally_before_switch_in;
	std::optional<OnAllyBeforeSwitchOutFunc> on_ally_before_switch_out;
	std::optional<OnAllyTryBoostFunc> on_ally_try_boost;
	std::optional<VoidSourceMoveFunc> on_ally_charge_move; // from CommonHandlers
	std::optional<OnAllyCriticalHitFunc> on_ally_critical_hit;
	std::optional<OnAllyDamageFunc> on_ally_damage;
	std::optional<OnAllyDeductPPFunc> on_ally_deduct_pp;
	std::optional<OnAllyDisableMoveFunc> on_ally_disable_move;
	std::optional<OnAllyDragOutFunc> on_ally_drag_out;
	std::optional<OnAllyEatItemFunc> on_ally_eat_item;
	std::optional<OnEffectivenessFunc> on_ally_effectiveness; // from MoveEventMethods

	std::optional<VoidEffectFunc> on_ally_faint; // from CommonHandlers
	std::optional<OnAllyFlinchFunc> on_ally_flinch;
	std::optional<OnHitFunc> on_ally_hit; // from MoveEventMethods
	std::optional<OnAllyImmunityFunc> on_ally_immunity;
	std::optional<OnAllyLockMoveFunc> on_ally_lock_move;
	std::optional<OnAllyMaybeTrapPokemonFunc> on_ally_maybe_trap_pokemon;
	std::optional<ModifierMoveFunc> on_ally_modify_accuracy; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_ally_modify_atk; // from CommonHandlers
	std::optional<OnAllyModifyBoostFunc> on_ally_modify_boost;
	std::optional<ModifierSourceMoveFunc> on_ally_modify_crit_ratio; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_ally_modify_damage; // from CommonHandlers
	std::optional<ModifierMoveFunc> on_ally_modify_def; // from CommonHandlers
	std::optional<OnModifyMoveFunc> on_ally_modify_move; // from MoveEventMethods
	std::optional<ModifierSourceMoveFunc> on_ally_modify_priority; // from CommonHandlers
	std::optional<OnAllyModifySecondariesFunc> on_ally_modify_secondaries;

	std::optional<ModifierSourceMoveFunc> on_ally_modify_spa; // from CommonHandlers
	std::optional<ModifierMoveFunc> on_ally_modify_spd; // from CommonHandlers
	std::optional<OnAllyModifySpeFunc> on_ally_modify_spe;
	std::optional<ModifierSourceMoveFunc> on_ally_modify_stab; // from CommonHandlers
	std::optional<OnModifyTypeFunc> on_ally_modify_type; // from MoveEventMethods
	std::optional<OnModifyTargetFunc> on_ally_modify_target; // from MoveEventMethods
	std::optional<OnAllyModifyWeightFunc> on_ally_modify_weight;
	std::optional<VoidMoveFunc> on_ally_move_aborted; // from CommonHandlers
	std::optional<OnAllyNegateImmunityFunc> on_ally_negate_immunity;
	std::optional<OnAllyOverrideActionFunc> on_ally_override_action;
	std::optional<ResultSourceMoveFunc> on_ally_prepare_hit; // from CommonHandlers
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

	std::optional<OnTryHitFunc> on_ally_try_hit; // from MoveEventMethods
	std::optional<OnTryHitFieldFunc> on_ally_try_hit_field; // from MoveEventMethods
	std::optional<ResultMoveFunc> on_ally_try_hit_side; // from CommonHandlers
	std::optional<ExtResultMoveFunc> on_ally_invulnerability; // from CommonHandlers
	std::optional<OnTryMoveFunc> on_ally_try_move; // from MoveEventMethods
	std::optional<OnAllyTryPrimaryHitFunc> on_ally_try_primary_hit;
	std::optional<OnTypeFunc> on_ally_type; // from MoveEventMethods
	std::optional<ModifierSourceMoveFunc> on_ally_weather_modify_damage; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_ally_modify_damage_phase_1; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_ally_modify_damage_phase_2; // from CommonHandlers
};