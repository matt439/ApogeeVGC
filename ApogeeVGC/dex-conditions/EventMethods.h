#pragma once

#include "event_methods_aliases.h"
#include <optional>

struct EventMethods
{
	std::optional<OnDamagingHitFunc> on_damaging_hit;
	std::optional<OnEmergencyExitFunc> on_emergency_exit;
	std::optional<OnAfterEachBoostFunc> on_after_each_boost;
	std::optional<OnAfterHitFunc> on_after_hit;
	std::optional<OnAfterMegaFunc> on_after_mega;
	std::optional<OnAfterSetStatusFunc> on_after_set_status;
	std::optional<OnAfterSubDamageFunc> on_after_sub_damage;
	std::optional<OnAfterSwitchInSelfFunc> on_after_switch_in_self;
	std::optional<OnAfterTerastallizationFunc> on_after_terastallization;
	std::optional<OnAfterUseItemFunc> on_after_use_item;
	std::optional<OnAfterTakeItemFunc> on_after_take_item;
	std::optional<OnAfterBoostFunc> on_after_boost;
	std::optional<OnAfterFaintFunc> on_after_faint;
	std::optional<OnAfterMoveSecondarySelfFunc> on_after_move_secondary_self;
	std::optional<OnAfterMoveSecondaryFunc> on_after_move_secondary;
	std::optional<OnAfterMoveFunc> on_after_move;
	std::optional<VoidSourceMoveFunc> on_after_move_self;
	std::optional<OnAttractFunc> on_attract;
	std::optional<OnAccuracyFunc> on_accuracy;
	std::optional<ModifierSourceMoveFunc> on_base_power; // from CommonHandlers
	std::optional<OnBeforeFaintFunc> on_before_faint;
	std::optional<VoidSourceMoveFunc> on_before_move;
	std::optional<OnBeforeSwitchInFunc> on_before_switch_in;
	std::optional<OnBeforeSwitchOutFunc> on_before_switch_out;
	std::optional<OnBeforeTurnFunc> on_before_turn;
	std::optional<OnChangeBoostFunc> on_change_boost;
	std::optional<OnTryBoostFunc> on_try_boost;
	std::optional<VoidSourceMoveFunc> on_charge_move;
	std::optional<OnCriticalHitFunc> on_critical_hit;
	std::optional<OnDamageFunc> on_damage;

	std::optional<OnDeductPPFunc> on_deduct_pp;
	std::optional<OnDisableMoveFunc> on_disable_move;
	std::optional<OnDragOutFunc> on_drag_out;
	std::optional<OnEatItemFunc> on_eat_item;
	std::optional<OnEffectivenessFunc> on_effectiveness;
	std::optional<OnEntryHazardFunc> on_entry_hazard;
	std::optional<VoidEffectFunc> on_faint; // from CommonHandlers
	std::optional<OnFlinchFunc> on_flinch;
	std::optional<OnFractionalPriorityFunc> on_fractional_priority;
	std::optional<OnHitFunc> on_hit;
	std::optional<OnImmunityFunc> on_immunity;
	std::optional<OnLockMoveFunc> on_lock_move;
	std::optional<OnMaybeTrapPokemonFunc> on_maybe_trap_pokemon;
	std::optional<ModifierMoveFunc> on_modify_accuracy; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_modify_atk; // from CommonHandlers
	std::optional<OnModifyBoostFunc> on_modify_boost;
	std::optional<ModifierSourceMoveFunc> on_modify_crit_ratio; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_modify_damage; // from CommonHandlers
	std::optional<ModifierMoveFunc> on_modify_def; // from CommonHandlers
	std::optional<OnModifyMoveFunc> on_modify_move;
	std::optional<ModifierSourceMoveFunc> on_modify_priority; // from CommonHandlers
	std::optional<OnModifySecondariesFunc> on_modify_secondaries;
	std::optional<OnModifyTypeFunc> on_modify_type;
	std::optional<OnModifyTargetFunc> on_modify_target;
	std::optional<ModifierSourceMoveFunc> on_modify_spa; // from CommonHandlers
	std::optional<ModifierMoveFunc> on_modify_spd;
	std::optional<OnModifySpeFunc> on_modify_spe;
	std::optional<ModifierSourceMoveFunc> on_modify_stab; // from CommonHandlers
	std::optional<OnModifyWeightFunc> on_modify_weight;
	std::optional<VoidMoveFunc> on_move_aborted; // from CommonHandlers
	std::optional<OnNegateImmunityFunc> on_negate_immunity;
	std::optional<OnOverrideActionFunc> on_override_action;
	std::optional<ResultSourceMoveFunc> on_prepare_hit;
	std::optional<OnPseudoWeatherChangeFunc> on_pseudo_weather_change;
	std::optional<OnRedirectTargetFunc> on_redirect_target;
	std::optional<OnResidualFunc> on_residual;
	std::optional<OnSetAbilityFunc> on_set_ability;
	std::optional<OnSetStatusFunc> on_set_status;

	std::optional<OnSetWeatherFunc> on_set_weather;
	std::optional<OnSideConditionStartFunc> on_side_condition_start;
	std::optional<OnStallMoveFunc> on_stall_move;
	std::optional<OnSwitchInFunc> on_switch_in;
	std::optional<OnSwitchOutFunc> on_switch_out;
	std::optional<OnSwapFunc> on_swap;
	std::optional<OnTakeItemFunc> on_take_item;
	std::optional<OnWeatherChangeFunc> on_weather_change;
	std::optional<OnTerrainChangeFunc> on_terrain_change;
	std::optional<OnTrapPokemonFunc> on_trap_pokemon;
	std::optional<OnTryAddVolatileFunc> on_try_add_volatile;
	std::optional<OnTryEatItemFunc> on_try_eat_item;
	std::optional<OnTryHealFunc> on_try_heal;
	std::optional<OnTryPrimaryHitFunc> on_try_primary_hit;
	std::optional<OnTypeFunc> on_type;
	std::optional<OnUseItemFunc> on_use_item;
	std::optional<OnUpdateFunc> on_update;
	std::optional<OnWeatherFunc> on_weather;

	std::optional<ModifierSourceMoveFunc> on_weather_modify_damage; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_modify_damage_phase_1; // from CommonHandlers
	std::optional<OnModifyDamagePhase2Func> on_modify_damage_phase_2;
	std::optional<OnFoeDamagingHitFunc> on_foe_damaging_hit;
	std::optional<OnFoeAfterEachBoostFunc> on_foe_after_each_boost;
	std::optional<OnAfterHitFunc> on_foe_after_hit;
	std::optional<OnFoeAfterSetStatusFunc> on_foe_after_set_status;
	std::optional<OnAfterSubDamageFunc> on_foe_after_sub_damage; // from MoveEventMethods
	std::optional<OnFoeAfterSwitchInSelfFunc> on_foe_after_switch_in_self;
	std::optional<OnFoeAfterUseItemFunc> on_foe_after_use_item;
	std::optional<OnFoeAfterBoostFunc> on_foe_after_boost;
	std::optional<OnFoeAfterFaintFunc> on_foe_after_faint;
	std::optional<OnAfterMoveSecondarySelfFunc> on_foe_after_move_secondary_self; // from MoveEventMethods
	std::optional<OnAfterMoveSecondaryFunc> on_foe_after_move_secondary; // from MoveEventMethods
	std::optional<OnAfterMoveFunc> on_foe_after_move;
	std::optional<VoidSourceMoveFunc> on_foe_after_move_self; // from CommonHandlers
	std::optional<OnFoeAttractFunc> on_foe_attract;
	std::optional<OnFoeAccuracyFunc> on_foe_accuracy;

	std::optional<ModifierSourceMoveFunc> on_foe_base_power; // from CommonHandlers
	std::optional<OnFoeBeforeFaintFunc> on_foe_before_faint;
	std::optional<VoidSourceMoveFunc> on_foe_before_move; // from CommonHandlers
	std::optional<OnFoeBeforeSwitchInFunc> on_foe_before_switch_in;
	std::optional<OnFoeBeforeSwitchOutFunc> on_foe_before_switch_out;
	std::optional<OnFoeTryBoostFunc> on_foe_try_boost;
	std::optional<VoidSourceMoveFunc> on_foe_charge_move;
	std::optional<OnFoeCriticalHitFunc> on_foe_critical_hit;
	std::optional<OnFoeDamageFunc> on_foe_damage;
	std::optional<OnFoeDeductPPFunc> on_foe_deduct_pp;
	std::optional<OnFoeDisableMoveFunc> on_foe_disable_move;
	std::optional<OnFoeDragOutFunc> on_foe_drag_out;
	std::optional<OnFoeEatItemFunc> on_foe_eat_item;
	std::optional<OnEffectivenessFunc> on_foe_effectiveness; // from MoveEventMethods
	std::optional<OnFoeFaintFunc> on_foe_faint;
	std::optional<OnFoeFlinchFunc> on_foe_flinch;
	std::optional<OnHitFunc> on_foe_hit; // from MoveEventMethods
	std::optional<OnFoeImmunityFunc> on_foe_immunity;
	std::optional<OnFoeLockMoveFunc> on_foe_lock_move;
	std::optional<OnFoeMaybeTrapPokemonFunc> on_foe_maybe_trap_pokemon;
	std::optional<ModifierMoveFunc> on_foe_modify_accuracy; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_foe_modify_atk; // from CommonHandlers
	std::optional<OnFoeModifyBoostFunc> on_foe_modify_boost;
	std::optional<ModifierSourceMoveFunc> on_foe_modify_crit_ratio; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_foe_modify_damage; // from CommonHandlers
	std::optional<ModifierMoveFunc> on_foe_modify_def; // from CommonHandlers
	std::optional<OnModifyMoveFunc> on_foe_modify_move; // from MoveEventMethods
	std::optional<ModifierSourceMoveFunc> on_foe_modify_priority; // from CommonHandlers
	std::optional<OnFoeModifySecondariesFunc> on_foe_modify_secondaries;

	std::optional<ModifierSourceMoveFunc> on_foe_modify_spa; // from CommonHandlers
	std::optional<ModifierMoveFunc> on_foe_modify_spd; // from CommonHandlers
	std::optional<OnFoeModifySpeFunc> on_foe_modify_spe;
	std::optional<ModifierSourceMoveFunc> on_foe_modify_stab; // from CommonHandlers
	std::optional<OnModifyTypeFunc> on_foe_modify_type; // from MoveEventMethods
	std::optional<OnModifyTargetFunc> on_foe_modify_target; // from MoveEventMethods
	std::optional<OnFoeModifyWeightFunc> on_foe_modify_weight;
	std::optional<VoidMoveFunc> on_foe_move_aborted; // from CommonHandlers
	std::optional<OnFoeNegateImmunityFunc> on_foe_negate_immunity;
	std::optional<OnFoeOverrideActionFunc> on_foe_override_action;
	std::optional<ResultSourceMoveFunc> on_foe_prepare_hit; // from CommonHandlers
	std::optional<OnFoeRedirectTargetFunc> on_foe_redirect_target;
	std::optional<OnFoeResidualFunc> on_foe_residual;
	std::optional<OnFoeSetAbilityFunc> on_foe_set_ability;
	std::optional<OnFoeSetStatusFunc> on_foe_set_status;
	std::optional<OnFoeSetWeatherFunc> on_foe_set_weather;
	std::optional<OnFoeSideConditionStartFunc> on_foe_side_condition_start;
	std::optional<OnFoeStallMoveFunc> on_foe_stall_move;
	std::optional<OnFoeSwitchOutFunc> on_foe_switch_out;
	std::optional<OnFoeSwapFunc> on_foe_swap;
	std::optional<OnFoeTakeItemFunc> on_foe_take_item;
	std::optional<OnFoeWeatherChangeFunc> on_foe_weather_change;
	std::optional<OnFoeTerrainChangeFunc> on_foe_terrain_change;
	std::optional<OnFoeTrapPokemonFunc> on_foe_trap_pokemon;
	std::optional<OnFoeTryAddVolatileFunc> on_foe_try_add_volatile;
	std::optional<OnFoeTryEatItemFunc> on_foe_try_eat_item;
	std::optional<OnFoeTryHealFunc> on_foe_try_heal;

	std::optional<OnTryHitFunc> on_foe_try_hit; // from MoveEventMethods
	std::optional<OnTryHitFieldFunc> on_foe_try_hit_field; // from MoveEventMethods
	std::optional<ResultMoveFunc> on_foe_try_hit_side; // from CommonHandlers
	std::optional<ExtResultMoveFunc> on_foe_invulnerability; // from CommonHandlers
	std::optional<OnTryMoveFunc> on_foe_try_move; // from MoveEventMethods
	std::optional<OnFoeTryPrimaryHitFunc> on_foe_try_primary_hit;
	std::optional<OnFoeTypeFunc> on_foe_type;
	std::optional<ModifierSourceMoveFunc> on_foe_weather_modify_damage; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_foe_modify_damage_phase_1; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_foe_modify_damage_phase_2; // from CommonHandlers
	std::optional<OnSourceDamagingHitFunc> on_source_damaging_hit;
	std::optional<OnSourceAfterEachBoostFunc> on_source_after_each_boost;
	std::optional<OnAfterHitFunc> on_source_after_hit; // from MoveEventMethods
	std::optional<OnSourceAfterSetStatusFunc> on_source_after_set_status;
	std::optional<OnAfterSubDamageFunc> on_source_after_sub_damage; // from MoveEventMethods
	std::optional<OnSourceAfterSwitchInSelfFunc> on_source_after_switch_in_self;
	std::optional<OnSourceAfterUseItemFunc> on_source_after_use_item;
	std::optional<OnSourceAfterBoostFunc> on_source_after_boost;
	std::optional<OnSourceAfterFaintFunc> on_source_after_faint;
	std::optional<OnAfterMoveSecondarySelfFunc> on_source_after_move_secondary_self; // from MoveEventMethods
	std::optional<OnAfterMoveSecondaryFunc> on_source_after_move_secondary; // from MoveEventMethods
	std::optional<OnAfterMoveFunc> on_source_after_move; // from MoveEventMethods
	std::optional<VoidSourceMoveFunc> on_source_after_move_self;
	std::optional<OnSourceAttractFunc> on_source_attract;
	std::optional<OnSourceAccuracyFunc> on_source_accuracy;

	std::optional<ModifierSourceMoveFunc> on_source_base_power; // from CommonHandlers
	std::optional<OnSourceBeforeFaintFunc> on_source_before_faint;
	std::optional<VoidSourceMoveFunc> on_source_before_move;
	std::optional<OnSourceBeforeSwitchInFunc> on_source_before_switch_in;
	std::optional<OnSourceBeforeSwitchOutFunc> on_source_before_switch_out;
	std::optional<OnSourceTryBoostFunc> on_source_try_boost;
	std::optional<VoidSourceMoveFunc> on_source_charge_move; // from CommonHandlers
	std::optional<OnSourceCriticalHitFunc> on_source_critical_hit;
	std::optional<OnSourceDamageFunc> on_source_damage;
	std::optional<OnSourceDeductPPFunc> on_source_deduct_pp;
	std::optional<OnSourceDisableMoveFunc> on_source_disable_move;
	std::optional<OnSourceDragOutFunc> on_source_drag_out;
	std::optional<OnSourceEatItemFunc> on_source_eat_item;
	std::optional<OnEffectivenessFunc> on_source_effectiveness; // from MoveEventMethods
	std::optional<VoidEffectFunc> on_source_faint; // from CommonHandlers

	std::optional<OnSourceFlinchFunc> on_source_flinch;
	std::optional<OnHitFunc> on_source_hit; // from MoveEventMethods
	std::optional<OnSourceImmunityFunc> on_source_immunity;
	std::optional<OnSourceLockMoveFunc> on_source_lock_move;
	std::optional<OnSourceMaybeTrapPokemonFunc> on_source_maybe_trap_pokemon;
	std::optional<ModifierMoveFunc> on_source_modify_accuracy; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_source_modify_atk; // from CommonHandlers
	std::optional<OnSourceModifyBoostFunc> on_source_modify_boost;
	std::optional<ModifierSourceMoveFunc> on_source_modify_crit_ratio; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_source_modify_damage; // from CommonHandlers
	std::optional<ModifierMoveFunc> on_source_modify_def; // from CommonHandlers
	std::optional<OnModifyMoveFunc> on_source_modify_move; // from MoveEventMethods
	std::optional<ModifierSourceMoveFunc> on_source_modify_priority; // from CommonHandlers
	std::optional<OnSourceModifySecondariesFunc> on_source_modify_secondaries;

	std::optional<ModifierSourceMoveFunc> on_source_modify_spa; // from CommonHandlers
	std::optional<ModifierMoveFunc> on_source_modify_spd; // from CommonHandlers
	std::optional<OnSourceModifySpeFunc> on_source_modify_spe;
	std::optional<ModifierSourceMoveFunc> on_source_modify_stab; // from CommonHandlers
	std::optional<OnModifyTypeFunc> on_source_modify_type; // from MoveEventMethods
	std::optional<OnModifyTargetFunc> on_source_modify_target; // from MoveEventMethods
	std::optional<OnSourceModifyWeightFunc> on_source_modify_weight;
	std::optional<VoidMoveFunc> on_source_move_aborted; // from CommonHandlers
	std::optional<OnSourceNegateImmunityFunc> on_source_negate_immunity;
	std::optional<OnSourceOverrideActionFunc> on_source_override_action;
	std::optional<ResultSourceMoveFunc> on_source_prepare_hit; // from CommonHandlers
	std::optional<OnSourceRedirectTargetFunc> on_source_redirect_target;
	std::optional<OnSourceResidualFunc> on_source_residual;
	std::optional<OnSourceSetAbilityFunc> on_source_set_ability;
	std::optional<OnSourceSetStatusFunc> on_source_set_status;

	std::optional<OnSourceSetWeatherFunc> on_source_set_weather;
	std::optional<OnSourceStallMoveFunc> on_source_stall_move;
	std::optional<OnSourceSwitchOutFunc> on_source_switch_out;
	std::optional<OnSourceTakeItemFunc> on_source_take_item;
	std::optional<OnSourceTerrainFunc> on_source_terrain_change;
	std::optional<OnSourceTrapPokemonFunc> on_source_trap_pokemon;
	std::optional<OnSourceTryAddVolatileFunc> on_source_try_add_volatile;
	std::optional<OnSourceTryEatItemFunc> on_source_try_eat_item;
	std::optional<OnSourceTryHealFunc> on_source_try_heal;
	std::optional<OnTryHitFunc> on_source_try_hit; // from MoveEventMethods
	std::optional<OnTryHitFieldFunc> on_source_try_hit_field; // from MoveEventMethods
	std::optional<ResultMoveFunc> on_source_try_hit_side; // from CommonHandlers
	std::optional<ExtResultMoveFunc> on_source_invulnerability; // from CommonHandlers
	std::optional<OnTryMoveFunc> on_source_try_move; // from MoveEventMethods
	std::optional<OnSourceTryPrimaryHitFunc> on_source_try_primary_hit;

	std::optional<OnSourceTypeFunc> on_source_type;
	std::optional<ModifierSourceMoveFunc> on_source_weather_modify_damage; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_source_modify_damage_phase_1; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_source_modify_damage_phase_2; // from CommonHandlers
	std::optional<OnAnyDamagingHitFunc> on_any_damaging_hit;
	std::optional<OnAnyAfterEachBoostFunc> on_any_after_each_boost;
	std::optional<OnAfterHitFunc> on_any_after_hit; // from MoveEventMethods
	std::optional<OnAnyAfterSetStatusFunc> on_any_after_set_status;
	std::optional<OnAfterSubDamageFunc> on_any_after_sub_damage; // from MoveEventMethods
	std::optional<OnAnyAfterSwitchInSelfFunc> on_any_after_switch_in_self;
	std::optional<OnAnyAfterUseItemFunc> on_any_after_use_item;
	std::optional<OnAnyAfterBoostFunc> on_any_after_boost;
	std::optional<OnAnyAfterFaintFunc> on_any_after_faint;
	std::optional<OnAnyAfterMegaFunc> on_any_after_mega;

	std::optional<OnAfterMoveSecondarySelfFunc> on_any_after_move_secondary_self; // from MoveEventMethods
	std::optional<OnAfterMoveSecondaryFunc> on_any_after_move_secondary; // from MoveEventMethods
	std::optional<OnAfterMoveFunc> on_any_after_move; // from MoveEventMethods
	std::optional<VoidSourceMoveFunc> on_any_after_move_self;
	std::optional<OnAnyAttractFunc> on_any_attract;
	std::optional<OnAnyAccuracyFunc> on_any_accuracy;
	std::optional<ModifierSourceMoveFunc> on_any_base_power; // from CommonHandlers
	std::optional<OnAnyBeforeFaintFunc> on_any_before_faint;
	std::optional<VoidSourceMoveFunc> on_any_before_move;
	std::optional<OnAnyBeforeSwitchInFunc> on_any_before_switch_in;
	std::optional<OnAnyBeforeSwitchOutFunc> on_any_before_switch_out;
	std::optional<OnAnyTryBoostFunc> on_any_try_boost;
	std::optional<VoidSourceMoveFunc> on_any_charge_move;
	std::optional<OnAnyCriticalHitFunc> on_any_critical_hit;

	std::optional<OnAnyDamageFunc> on_any_damage;
	std::optional<OnAnyDeductPPFunc> on_any_deduct_pp;
	std::optional<OnAnyDisableMoveFunc> on_any_disable_move;
	std::optional<OnAnyDragOutFunc> on_any_drag_out;
	std::optional<OnAnyEatItemFunc> on_any_eat_item;
	std::optional<OnEffectivenessFunc> on_any_effectiveness; // from MoveEventMethods
	std::optional<VoidEffectFunc> on_any_faint; // from CommonHandlers
	std::optional<OnAnyFlinchFunc> on_any_flinch;
	std::optional<OnHitFunc> on_any_hit; // from MoveEventMethods
	std::optional<OnAnyImmunityFunc> on_any_immunity;
	std::optional<OnAnyLockMoveFunc> on_any_lock_move;
	std::optional<OnAnyMaybeTrapPokemonFunc> on_any_maybe_trap_pokemon;
	std::optional<ModifierMoveFunc> on_any_modify_accuracy; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_any_modify_atk; // from CommonHandlers

	std::optional<OnAnyModifyBoostFunc> on_any_modify_boost;
	std::optional<ModifierSourceMoveFunc> on_any_modify_crit_ratio; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_any_modify_damage; // from CommonHandlers
	std::optional<ModifierMoveFunc> on_any_modify_def; // from CommonHandlers
	std::optional<OnModifyMoveFunc> on_any_modify_move; // from MoveEventMethods
	std::optional<ModifierSourceMoveFunc> on_any_modify_priority; // from CommonHandlers
	std::optional<OnAnyModifySecondariesFunc> on_any_modify_secondaries;
	std::optional<ModifierSourceMoveFunc> on_any_modify_spa; // from CommonHandlers
	std::optional<ModifierMoveFunc> on_any_modify_spd; // from CommonHandlers
	std::optional<OnAnyModifySpeFunc> on_any_modify_spe;
	std::optional<ModifierSourceMoveFunc> on_any_modify_stab; // from CommonHandlers
	std::optional<OnModifyTypeFunc> on_any_modify_type; // from MoveEventMethods
	std::optional<OnModifyTargetFunc> on_any_modify_target; // from MoveEventMethods
	std::optional<OnAnyModifyWeightFunc> on_any_modify_weight;
	std::optional<VoidMoveFunc> on_any_move_aborted;

	std::optional<OnAnyNegateImmunityFunc> on_any_negate_immunity;
	std::optional<OnAnyOverrideActionFunc> on_any_override_action;
	std::optional<ResultSourceMoveFunc> on_any_prepare_hit;
	std::optional<OnAnyRedirectTargetFunc> on_any_redirect_target;
	std::optional<OnAnyResidualFunc> on_any_residual;
	std::optional<OnAnySetAbilityFunc> on_any_set_ability;
	std::optional<OnAnySetStatusFunc> on_any_set_status;
	std::optional<OnAnySetWeatherFunc> on_any_set_weather;
	std::optional<OnAnyStallMoveFunc> on_any_stall_move;
	std::optional<OnAnySwitchInFunc> on_any_switch_in;
	std::optional<OnAnySwitchOutFunc> on_any_switch_out;
	std::optional<OnAnyTakeItemFunc> on_any_take_item;

	std::optional<OnAnyTerrainFunc> on_any_terrain_change;
	std::optional<OnAnyTrapPokemonFunc> on_any_trap_pokemon;
	std::optional<OnAnyTryAddVolatileFunc> on_any_try_add_volatile;
	std::optional<OnAnyTryEatItemFunc> on_any_try_eat_item;
	std::optional<OnAnyTryHealFunc> on_any_try_heal;
	std::optional<OnTryHitFunc> on_any_try_hit; // from MoveEventMethods
	std::optional<OnTryHitFieldFunc> on_any_try_hit_field; // from MoveEventMethods
	std::optional<ResultMoveFunc> on_any_try_hit_side; // from CommonHandlers
	std::optional<ExtResultMoveFunc> on_any_invulnerability; // from CommonHandlers
	std::optional<OnTryMoveFunc> on_any_try_move; // from MoveEventMethods
	std::optional<OnAnyTryPrimaryHitFunc> on_any_try_primary_hit;
	std::optional<OnAnyTypeFunc> on_any_type;
	std::optional<ModifierSourceMoveFunc> on_any_weather_modify_damage; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_any_modify_damage_phase_1; // from CommonHandlers
	std::optional<ModifierSourceMoveFunc> on_any_modify_damage_phase_2; // from CommonHandlers

	std::optional<int> on_accuracy_priority;
	std::optional<int> on_damaging_hit_order;
	std::optional<int> on_after_move_secondary_priority;
	std::optional<int> on_after_move_secondary_self_priority;
	std::optional<int> on_after_move_self_priority;
	std::optional<int> on_after_set_status_priority;
	std::optional<int> on_any_base_power_priority;
	std::optional<int> on_any_invulnerability_priority;
	std::optional<int> on_any_modify_accuracy_priority;
	std::optional<int> on_any_faint_priority;
	std::optional<int> on_any_prepare_hit_priority;
	std::optional<int> on_any_switch_in_priority;
	std::optional<int> on_any_switch_in_sub_order;
	std::optional<int> on_ally_base_power_priority;
	std::optional<int> on_ally_modify_atk_priority;
	std::optional<int> on_ally_modify_spa_priority;
	std::optional<int> on_ally_modify_spd_priority;
	std::optional<int> on_attract_priority;
	std::optional<int> on_base_power_priority;
	std::optional<int> on_before_move_priority;
	std::optional<int> on_before_switch_out_priority;
	std::optional<int> on_change_boost_priority;
	std::optional<int> on_damage_priority;
	std::optional<int> on_drag_out_priority;
	std::optional<int> on_effectiveness_priority;
	std::optional<int> on_foe_base_power_priority;
	std::optional<int> on_foe_before_move_priority;
	std::optional<int> on_foe_modify_def_priority;
	std::optional<int> on_foe_modify_spd_priority;
	std::optional<int> on_foe_redirect_target_priority;
	std::optional<int> on_foe_trap_pokemon_priority;
	std::optional<int> on_fractional_priority_priority;
	std::optional<int> on_hit_priority;
	std::optional<int> on_invulnerability_priority;
	std::optional<int> on_modify_accuracy_priority;
	std::optional<int> on_modify_atk_priority;
	std::optional<int> on_modify_crit_ratio_priority;
	std::optional<int> on_modify_def_priority;
	std::optional<int> on_modify_move_priority;
	std::optional<int> on_modify_priority_priority;
	std::optional<int> on_modify_spa_priority;
	std::optional<int> on_modify_spd_priority;
	std::optional<int> on_modify_spe_priority;
	std::optional<int> on_modify_stab_priority;
	std::optional<int> on_modify_type_priority;
	std::optional<int> on_modify_weight_priority;
	std::optional<int> on_redirect_target_priority;
	std::optional<int> on_residual_order;
	std::optional<int> on_residual_priority;
	std::optional<int> on_residual_sub_order;
	std::optional<int> on_source_base_power_priority;
	std::optional<int> on_source_invulnerability_priority;
	std::optional<int> on_source_modify_accuracy_priority;
	std::optional<int> on_source_modify_atk_priority;
	std::optional<int> on_source_modify_damage_priority;
	std::optional<int> on_source_modify_spa_priority;
	std::optional<int> on_switch_in_priority;
	std::optional<int> on_switch_in_sub_order;
	std::optional<int> on_trap_pokemon_priority;
	std::optional<int> on_try_boost_priority;
	std::optional<int> on_try_eat_item_priority;
	std::optional<int> on_try_heal_priority;
	std::optional<int> on_try_hit_priority;
	std::optional<int> on_try_move_priority;
	std::optional<int> on_try_primary_hit_priority;
	std::optional<int> on_type_priority;
};
