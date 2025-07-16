#include "PokemonEventMethods.h"

PokemonEventMethods::PokemonEventMethods(const PokemonEventMethods& other) :
	EventMethods(other)
{
	on_ally_damaging_hit = other.on_ally_damaging_hit;
	on_ally_after_each_boost = other.on_ally_after_each_boost;
	on_ally_after_hit = other.on_ally_after_hit;
	on_ally_after_set_status = other.on_ally_after_set_status;
	on_ally_after_sub_damage = other.on_ally_after_sub_damage;
	on_ally_after_switch_in_self = other.on_ally_after_switch_in_self;
	on_ally_after_use_item = other.on_ally_after_use_item;
	on_ally_after_boost = other.on_ally_after_boost;
	on_ally_after_faint = other.on_ally_after_faint;
	on_ally_after_move_secondary_self = other.on_ally_after_move_secondary_self;
	on_ally_after_move_secondary = other.on_ally_after_move_secondary;
	on_ally_after_move = other.on_ally_after_move;
	on_ally_after_move_self = other.on_ally_after_move_self;
	on_ally_attract = other.on_ally_attract;
	on_ally_accuracy = other.on_ally_accuracy;

	on_ally_base_power = other.on_ally_base_power;
	on_ally_before_faint = other.on_ally_before_faint;
	on_ally_before_move = other.on_ally_before_move;
	on_ally_before_switch_in = other.on_ally_before_switch_in;
	on_ally_before_switch_out = other.on_ally_before_switch_out;
	on_ally_try_boost = other.on_ally_try_boost;
	on_ally_charge_move = other.on_ally_charge_move;
	on_ally_critical_hit = other.on_ally_critical_hit;
	on_ally_damage = other.on_ally_damage;
	on_ally_deduct_pp = other.on_ally_deduct_pp;
	on_ally_disable_move = other.on_ally_disable_move;
	on_ally_drag_out = other.on_ally_drag_out;
	on_ally_eat_item = other.on_ally_eat_item;
	on_ally_effectiveness = other.on_ally_effectiveness;

	on_ally_faint = other.on_ally_faint;
	on_ally_flinch = other.on_ally_flinch;
	on_ally_hit = other.on_ally_hit;
	on_ally_immunity = other.on_ally_immunity;
	on_ally_lock_move = other.on_ally_lock_move ? std::make_unique<OnAllyLockMoveFunc>(*other.on_ally_lock_move) : nullptr;
	on_ally_maybe_trap_pokemon = other.on_ally_maybe_trap_pokemon;
	on_ally_modify_accuracy = other.on_ally_modify_accuracy;
	on_ally_modify_atk = other.on_ally_modify_atk;
	on_ally_modify_boost = other.on_ally_modify_boost;
	on_ally_modify_crit_ratio = other.on_ally_modify_crit_ratio;
	on_ally_modify_damage = other.on_ally_modify_damage;
	on_ally_modify_def = other.on_ally_modify_def;
	on_ally_modify_move = other.on_ally_modify_move;
	on_ally_modify_priority = other.on_ally_modify_priority;
	on_ally_modify_secondaries = other.on_ally_modify_secondaries;

	on_ally_modify_spa = other.on_ally_modify_spa;
	on_ally_modify_spd = other.on_ally_modify_spd;
	on_ally_modify_spe = other.on_ally_modify_spe;
	on_ally_modify_stab = other.on_ally_modify_stab;
	on_ally_modify_type = other.on_ally_modify_type;
	on_ally_modify_target = other.on_ally_modify_target;
	on_ally_modify_weight = other.on_ally_modify_weight;
	on_ally_move_aborted = other.on_ally_move_aborted;
	on_ally_negate_immunity = other.on_ally_negate_immunity;
	on_ally_override_action = other.on_ally_override_action;
	on_ally_prepare_hit = other.on_ally_prepare_hit;
	on_ally_redirect_target = other.on_ally_redirect_target;
	on_ally_residual = other.on_ally_residual;
	on_ally_set_ability = other.on_ally_set_ability;

	on_ally_set_status = other.on_ally_set_status;
	on_ally_set_weather = other.on_ally_set_weather;
	on_ally_stall_move = other.on_ally_stall_move;
	on_ally_switch_out = other.on_ally_switch_out;
	on_ally_take_item = other.on_ally_take_item;
	on_ally_terrain_change = other.on_ally_terrain_change;
	on_ally_trap_pokemon = other.on_ally_trap_pokemon;
	on_ally_try_add_volatile = other.on_ally_try_add_volatile;
	on_ally_try_eat_item = other.on_ally_try_eat_item;
	on_ally_try_heal = other.on_ally_try_heal ? std::make_unique<OnAllyTryHealFunc>(*other.on_ally_try_heal) : nullptr;
	
	on_ally_try_hit = other.on_ally_try_hit;
	on_ally_try_hit_field = other.on_ally_try_hit_field;
	on_ally_try_hit_side = other.on_ally_try_hit_side;
	on_ally_invulnerability = other.on_ally_invulnerability;
	on_ally_try_move = other.on_ally_try_move;
	on_ally_try_primary_hit = other.on_ally_try_primary_hit;
	on_ally_type = other.on_ally_type;
	on_ally_weather_modify_damage = other.on_ally_weather_modify_damage;
	on_ally_modify_damage_phase_1 = other.on_ally_modify_damage_phase_1;
	on_ally_modify_damage_phase_2 = other.on_ally_modify_damage_phase_2;
}