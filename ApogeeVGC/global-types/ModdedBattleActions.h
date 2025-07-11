#pragma once

#include "Effect.h"
#include <memory>
#include <functional>
#include <vector>
#include <string>
#include <variant>

class Pokemon;
class Move;
class ActiveMove;
class SpreadMoveDamage;
class SpreadMoveTargets;
class MoveOptions;
class Action;

struct ModdedBattleActions
{
	bool inherit = true;

    std::function<void(std::vector<Pokemon*>*, Pokemon*, ActiveMove*)> after_move_secondary_event = nullptr;
    std::function<int(int, Move*, Pokemon*)> calc_recoil_damage = nullptr;
    std::function<std::string(Pokemon*)> can_mega_evo = nullptr;
    std::function<std::string(Pokemon*)> can_mega_evo_x = nullptr;
    std::function<std::string(Pokemon*)> can_mega_evo_y = nullptr;
    std::function<std::string(Pokemon*)> can_terastallize = nullptr;
    std::function<std::string(Pokemon*)> can_ultra_burst = nullptr;
    std::function<void(Pokemon*)> can_z_move = nullptr; // Replace void with ZMoveOptions if defined
    std::function<SpreadMoveDamage(int, SpreadMoveTargets*, Pokemon*, ActiveMove*, ActiveMove*, bool, bool)> force_switch = nullptr;
    std::function<ActiveMove* (Move*, Pokemon*)> get_active_max_move = nullptr;
    std::function<ActiveMove* (Move*, Pokemon*)> get_active_z_move = nullptr;
    std::function<Move(Move*, Pokemon*)> get_max_move = nullptr;
    std::function<SpreadMoveDamage(int, SpreadMoveTargets*, Pokemon*, ActiveMove*, ActiveMove*, bool, bool)> get_spread_damage = nullptr;
    std::function<std::variant<std::string, bool>(Move*, Pokemon*, bool)> get_z_move = nullptr;
    std::function<std::vector<bool>(std::vector<Pokemon*>*, Pokemon*, ActiveMove*)> hit_step_accuracy = nullptr;
    std::function<void(std::vector<Pokemon*>*, Pokemon*, ActiveMove*)> hit_step_break_protect = nullptr;
    std::function<SpreadMoveDamage(std::vector<Pokemon*>*, Pokemon*, ActiveMove*)> hit_step_move_hit_loop = nullptr;
    std::function<std::vector<bool>(std::vector<Pokemon*>*, Pokemon*, ActiveMove*)> hit_step_try_immunity = nullptr;
    std::function<void(std::vector<Pokemon*>*, Pokemon*, ActiveMove*)> hit_step_steal_boosts = nullptr;
    std::function<std::vector<std::variant<bool, std::string>>(std::vector<Pokemon*>*, Pokemon*, ActiveMove*)> hit_step_try_hit_event = nullptr;
    std::function<std::vector<bool>(std::vector<Pokemon*>*, Pokemon*, ActiveMove*)> hit_step_invulnerability_event = nullptr;
    std::function<std::vector<bool>(std::vector<Pokemon*>*, Pokemon*, ActiveMove*)> hit_step_type_immunity = nullptr;
    std::function<std::variant<int, bool>(Pokemon*, Pokemon*, ActiveMove*, ActiveMove*, bool, bool)> move_hit = nullptr;

    std::function<void(Action*)> run_action = nullptr;
    std::function<bool(Pokemon*)> run_mega_evo = nullptr;
    std::function<bool(Pokemon*)> run_mega_evo_x = nullptr;
    std::function<bool(Pokemon*)> run_mega_evo_y = nullptr;
    std::function<void(Move*, Pokemon*, int*, MoveOptions*)> run_move = nullptr;
    std::function<SpreadMoveDamage* (SpreadMoveDamage*, SpreadMoveTargets*, Pokemon*, ActiveMove*, ActiveMove*, bool*, bool*)> run_move_effects = nullptr;
    std::function<bool(Pokemon*)> run_switch = nullptr;
    std::function<void(ActiveMove*, Pokemon*)> run_z_power = nullptr;
    std::function<void(SpreadMoveTargets*, Pokemon*, ActiveMove*, ActiveMove*, bool*)> secondaries = nullptr;
    std::function<void(SpreadMoveTargets*, Pokemon*, ActiveMove*, ActiveMove*, bool*)> self_drops = nullptr;
    std::function<std::pair<SpreadMoveDamage*, SpreadMoveTargets*>(SpreadMoveTargets*, Pokemon*, ActiveMove*, ActiveMove*, bool*, bool*)> spread_move_hit = nullptr;
    std::function<std::variant<bool, std::string>(Pokemon*, int*, Effect*, bool*)> switch_in = nullptr;

    std::function<bool(std::string*)> target_type_choices = nullptr;
    std::function<void(Pokemon*)> terastallize = nullptr;
    std::function<std::variant<int, bool, std::string>(Pokemon*, Pokemon*, ActiveMove*)> try_move_hit = nullptr;
    std::function<SpreadMoveDamage* (SpreadMoveDamage*, SpreadMoveTargets*, Pokemon*, ActiveMove*, ActiveMove*, bool*)> try_primary_hit_event = nullptr;
    std::function<bool(std::vector<Pokemon*>*, Pokemon*, ActiveMove*, bool*)> try_spread_move_hit = nullptr;
    std::function<bool(Move*, Pokemon*, MoveOptions*)> use_move = nullptr;
    std::function<bool(Move*, Pokemon*, MoveOptions*)> use_move_inner = nullptr;
    std::function<std::variant<int, std::nullptr_t, bool>(Pokemon*, Pokemon*, std::variant<std::string, int, ActiveMove*>, bool)> get_damage = nullptr;
    std::function<void(int, Pokemon*, Pokemon*, ActiveMove*, bool*)> modify_damage = nullptr;

    // OMs (Other Metagames)
    std::function<Species* (Species*, void*)> mutate_original_species = nullptr;
    std::function<void* (Species*, Pokemon*)> get_forme_change_deltas = nullptr;
    std::function<Species* (std::string*, std::string*, Pokemon*)> get_mixed_species = nullptr;

	ModdedBattleActions() = default;
};