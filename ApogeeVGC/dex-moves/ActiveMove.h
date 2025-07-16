#pragma once

#include "../global-types/ID.h"
#include "../global-types/Effect.h"
#include "MutableMove.h"
#include "MoveHitData.h"
#include <string>
#include <variant>
#include <vector>
#include <memory>

class ActiveMove : public MutableMove
{
public:
	std::string name = "";
	std::string effect_type = "Move";
    ID id = ID();
	int num = 0;
    int hit = 0;
	std::unique_ptr<ID> weather = nullptr; // optional
	std::unique_ptr<ID> status = nullptr; // optional
	std::unique_ptr<MoveHitData> move_hit_data = nullptr; // optional
	std::unique_ptr<std::vector<Pokemon>> hit_targets = nullptr; // optional
	std::unique_ptr<Ability> ability = nullptr; // optional
	std::unique_ptr<std::vector<Pokemon>> allies = nullptr; // optional
	std::unique_ptr<Pokemon> aura_booster = nullptr; // optional
	std::unique_ptr<bool> caused_crash_damage = nullptr; // optional
	std::unique_ptr<ID> force_status = nullptr; // optional
	std::unique_ptr<bool> has_aura_break = nullptr; // optional
	std::unique_ptr<bool> has_bounced = nullptr; // optional
	std::unique_ptr<bool> has_sheer_force = nullptr; // optional
	std::unique_ptr<bool> is_external = nullptr; // optional
	std::unique_ptr<bool> last_hit = nullptr; // optional
	std::unique_ptr<int> magnitude = nullptr; // optional
	std::unique_ptr<bool> prankster_boosted = nullptr; // optional
	std::unique_ptr<bool> self_dropped = nullptr; // optional
	std::unique_ptr<std::variant<std::string, bool>> self_switch = nullptr; // optional
	std::unique_ptr<bool> spread_hit = nullptr; // optional
	std::unique_ptr<std::string> status_roll = nullptr; // optional
	std::unique_ptr<bool> stellar_boosted = nullptr; // optional
	std::unique_ptr<std::variant<int, bool>> total_damage = nullptr; // optional
	std::unique_ptr<Effect> type_changer_boosted = nullptr; // optional
	std::unique_ptr<bool> will_change_forme = nullptr; // optional
	std::unique_ptr<bool> infiltrates = nullptr; // optional
	std::unique_ptr<Pokemon> ruined_atk = nullptr; // optional
	std::unique_ptr<Pokemon> ruined_def = nullptr; // optional
	std::unique_ptr<Pokemon> ruined_spa = nullptr; // optional
	std::unique_ptr<Pokemon> ruined_spd = nullptr; // optional
	std::unique_ptr<bool> is_z_or_max_powered = nullptr; // optional
};
