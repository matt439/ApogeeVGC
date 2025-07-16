#pragma once

#include "../global-types/ID.h"
#include "../global-types/Effect.h"
#include "../pokemon/Pokemon.h"
#include "../dex-abilities/Ability.h"
#include "MutableMove.h"
#include "MoveHitData.h"
#include <string>
#include <variant>
#include <vector>
#include <memory>

//class Ability;
//class Pokemon;

class ActiveMove : public MutableMove
{
public:
	std::string name = "";
	EffectType effect_type = EffectType::MOVE;
    //ID id = ID();
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

	ActiveMove() = default;

	ActiveMove(
		int hit,
		// MoveData
		const std::string& name,
		MoveTarget target,
		MoveFlags& flags,
		int base_power = 0,
		std::variant<bool, int> accuracy = false,
		int pp = 0,
		MoveCategory category = MoveCategory::STATUS,
		const std::string& type = "",
		int priority = 0,
		// BasicEffect
		const std::string& real_move = "",
		const std::string& full_name = "",
		bool exists = true,
		int num = 0,
		int gen = 0,
		const std::string& short_desc = "",
		const std::string& desc = "",
		NonStandard is_nonstandard = NonStandard::NONE,
		bool no_copy = false,
		bool affects_fainted = false,
		const std::string& source_effect = "");

	ActiveMove(const ActiveMove& other);
};
