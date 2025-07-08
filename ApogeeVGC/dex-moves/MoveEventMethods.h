#pragma once

#include "function_type_aliases.h"

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