#pragma once

#include "function_type_aliases.h"

struct MoveEventMethods
{
	BasePowerCallbackFunc base_power_callback = nullptr;
	BeforeMoveCallbackFunc before_move_callback = nullptr;
	BeforeTurnCallbackFunc before_turn_callback = nullptr;
	DamageCallbackFunc damage_callback = nullptr;
	PriorityChargeCallbackFunc priority_charge_callback = nullptr;

	OnDisableMoveFunc on_disable_move = nullptr;
	OnAfterHitFunc on_after_hit = nullptr;
	OnAfterSubDamageFunc on_after_sub_damage = nullptr;
	OnAfterMoveSecondarySelfFunc on_after_move_secondary_self = nullptr;
	OnAfterMoveSecondaryFunc on_after_move_secondary = nullptr;
	OnAfterMoveFunc on_after_move = nullptr;

	std::unique_ptr<int> on_damage_priority = nullptr; // Optional priority for on_damage function
	OnDamageFunc on_damage = nullptr;

	OnBasePowerFunc on_base_power = nullptr;
	OnEffectivenessFunc on_effectiveness = nullptr;
	OnHitFunc on_hit = nullptr;
	OnHitFieldFunc on_hit_field = nullptr;
	OnHitSideFunc on_hit_side = nullptr;
	OnModifyMoveFunc on_modify_move = nullptr;
	OnModifyPriorityFunc on_modify_priority = nullptr;
	OnMoveFailFunc on_move_fail = nullptr;
	OnModifyTypeFunc on_modify_type = nullptr;
	OnModifyTargetFunc on_modify_target = nullptr;
	OnPrepareHitFunc on_prepare_hit = nullptr;
	OnTryFunc on_try = nullptr;
	OnTryHitFunc on_try_hit = nullptr;
	OnTryHitFieldFunc on_try_hit_field = nullptr;
	OnTryHitSideFunc on_try_hit_side = nullptr;
	OnTryImmunityFunc on_try_immunity = nullptr;
	OnTryMoveFunc on_try_move = nullptr;
	OnUseMoveMessageFunc on_use_move_message = nullptr;

	MoveEventMethods() = default;
	MoveEventMethods(const MoveEventMethods& other);
};