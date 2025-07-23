#include "MoveEventMethods.h"

MoveEventMethods::MoveEventMethods(const MoveEventMethods& other) :
	base_power_callback(other.base_power_callback),
	before_move_callback(other.before_move_callback),
	before_turn_callback(other.before_turn_callback),
	damage_callback(other.damage_callback),
	priority_charge_callback(other.priority_charge_callback),
	on_disable_move(other.on_disable_move),
	on_after_hit(other.on_after_hit),
	on_after_sub_damage(other.on_after_sub_damage),
	on_after_move_secondary_self(other.on_after_move_secondary_self),
	on_after_move_secondary(other.on_after_move_secondary),
	on_after_move(other.on_after_move),
	on_damage_priority(other.on_damage_priority ? std::make_unique<int>(*other.on_damage_priority) : nullptr),
	on_damage(other.on_damage),
	on_base_power(other.on_base_power),
	on_effectiveness(other.on_effectiveness),
	on_hit(other.on_hit),
	on_hit_field(other.on_hit_field),
	on_hit_side(other.on_hit_side),
	on_modify_move(other.on_modify_move),
	on_modify_priority(other.on_modify_priority),
	on_move_fail(other.on_move_fail),
	on_modify_type(other.on_modify_type),
	on_modify_target(other.on_modify_target),
	on_prepare_hit(other.on_prepare_hit),
	on_try(other.on_try),
	on_try_hit(other.on_try_hit),
	on_try_hit_field(other.on_try_hit_field),
	on_try_hit_side(other.on_try_hit_side),
	on_try_immunity(other.on_try_immunity),
	on_try_move(other.on_try_move),
	on_use_move_message(other.on_use_move_message)
{
}

