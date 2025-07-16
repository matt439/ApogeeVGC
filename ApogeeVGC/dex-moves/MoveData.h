#pragma once

#include "../dex/IDexData.h"
#include "../global-types/EffectData.h"
#include "../global-types/IDEntry.h"
#include "../global-types/ID.h"
#include "../global-types/StatIDExceptHP.h"
#include "../dex-conditions/ConditionData.h"
#include "ZMoveData.h"
#include "MaxMoveData.h"
#include "MoveCategory.h"
#include "SecondaryEffect.h"
#include "SelfSwitchType.h"
#include "SelfDestructType.h"
#include "SelfBoost.h"
#include "MoveTarget.h"
#include "MoveFlags.h"
#include "MoveEventMethods.h"
#include "HitEffect.h"

#include <array>

//struct ZMoveData;
//struct MaxMoveData;
//struct SecondaryEffect;
//struct SelfBoost;

struct MoveData : public EffectData, public MoveEventMethods, public HitEffect, public IDexData
{
	// std::string name = "";
	std::unique_ptr<int> num = nullptr; // optional
    std::unique_ptr<ConditionData> condition; // optional
	int base_power = 0;
	std::variant<bool, int> accuracy = false; // boolean | number
    int pp = 0;
	MoveCategory category = MoveCategory::STATUS;
    std::string type = "";
	int priority = 0;
    MoveTarget target;
    MoveFlags& flags;
	std::unique_ptr<std::string> real_move = nullptr; // optional

    std::variant<std::monostate, int, std::string, bool> damage = false; // optional, number | 'level' | false | null
	std::unique_ptr<std::string> contest_type = nullptr; // optional
	std::unique_ptr<bool> no_pp_boosts = nullptr; // optional

    // Z-move data
	std::unique_ptr<std::variant<bool, IDEntry>> is_z = nullptr; // optional, boolean | IDEntry
	std::unique_ptr<ZMoveData> z_move = nullptr; // optional

    // Max move data
	std::unique_ptr<std::variant<bool, std::string>> is_max = nullptr; // optional, boolean | type
	std::unique_ptr<MaxMoveData> max_move = nullptr; // optional

    // Hit effects
    std::unique_ptr<std::variant<bool, std::string>> ohko = nullptr; // optional, boolean | 'Ice'
	std::unique_ptr<bool> thaws_target = nullptr; // optional
	std::unique_ptr<std::vector<int>> heal = nullptr; // optional
	std::unique_ptr<bool> force_switch = nullptr; // optional
	std::unique_ptr<std::variant<SelfSwitchType, bool>> self_switch = nullptr; // optional, SelfSwitchType | boolean
	std::unique_ptr<SelfBoost> self_boost = nullptr; // optional // SelfBoost struct
	std::unique_ptr<std::variant<SelfDestructType, bool>> selfdestruct = nullptr; // optional, SelfDestructType | boolean
	std::unique_ptr<bool> breaks_protect = nullptr; // optional
	std::unique_ptr<std::array<int, 2>> recoil = nullptr; // optional
	std::unique_ptr<std::array<int, 2>> drain = nullptr; // optional
	std::unique_ptr<bool> mind_blown_recoil = nullptr; // optional
	std::unique_ptr<bool> steals_boosts = nullptr; // optional
	std::unique_ptr<bool> struggle_recoil = nullptr; // optional
	std::unique_ptr<SecondaryEffect> secondary = nullptr; // optional // Single secondary effect
	std::unique_ptr<std::vector<SecondaryEffect>> secondaries = nullptr; // optional // Multiple secondary effects
	std::unique_ptr<SecondaryEffect> self = nullptr; // optional // Self-targeting secondary effect
	std::unique_ptr<bool> has_sheer_force = nullptr; // optional // Indicates if the move has Sheer Force effect

    // Hit effect modifiers
	std::unique_ptr<bool> always_hit = nullptr; // optional
	std::unique_ptr<std::string> base_move_type = nullptr; // optional // 'Water' | 'Fire' | etc.
	std::unique_ptr<int> base_power_modifier = nullptr; // optional // Modifier for base power
	std::unique_ptr<int> crit_modifier = nullptr; // optional // Modifier for critical hit power
	std::unique_ptr<int> crit_ratio = nullptr; // optional // Critical hit ratio
	std::unique_ptr<std::string> override_offensive_pokemon = nullptr; // optional // 'target' | 'source'
	std::unique_ptr<StatIDExceptHP> override_offensive_stat = nullptr; // optional // Stat to override for offensive calculations
	std::unique_ptr<std::string> override_defensive_pokemon = nullptr; // optional // 'target' | 'source'
	std::unique_ptr<StatIDExceptHP> override_defensive_stat = nullptr; // optional // Stat to override for defensive calculations
	std::unique_ptr<bool> force_stab = nullptr; // optional // Force STAB (Same Type Attack Bonus)
	std::unique_ptr<bool> ignore_ability = nullptr; // optional // Ignore target's ability
	std::unique_ptr<bool> ignore_accuracy = nullptr; // optional // Ignore accuracy checks
	std::unique_ptr<bool> ignore_defensive = nullptr; // optional // Ignore defensive modifiers
	std::unique_ptr<bool> ignore_evasion = nullptr; // optional // Ignore evasion modifiers
	std::unique_ptr<std::variant<bool, std::unordered_map<std::string, bool>>> ignore_immunity = nullptr; // boolean | { 'type': true/false }
	std::unique_ptr<bool> ignore_negative_offensive = nullptr; // optional // Ignore negative offensive modifiers
	std::unique_ptr<bool> ignore_offensive = nullptr; // optional // Ignore all offensive modifiers
	std::unique_ptr<bool> ignore_positive_defensive = nullptr; // optional // Ignore positive defensive modifiers
	std::unique_ptr<bool> ignore_positive_evasion = nullptr; // optional // Ignore positive evasion modifiers
	std::unique_ptr<bool> multiaccuracy = nullptr; // optional // Multi-hit accuracy check
	std::unique_ptr<std::variant<int, std::vector<int>>> multihit = nullptr; // optional, number | [min_hits, max_hits] or just number for single hit
	std::unique_ptr<std::string> multihit_type = nullptr; // optional // 'parentalbond'
	std::unique_ptr<bool> no_damage_variance = nullptr; // optional // No damage variance (e.g., no minimum or maximum damage)
    std::unique_ptr<MoveTarget> non_ghost_target; // optional, Define as needed
	std::unique_ptr<int> spread_modifier = nullptr; // optional // Modifier for spread moves
	std::unique_ptr<bool> sleep_usable = nullptr; // optional // Can be used while asleep
	std::unique_ptr<bool> smart_target = nullptr; // optional // Smart targeting (e.g., ignores Substitute)
	std::unique_ptr<bool> tracks_target = nullptr; // optional // Tracks target's position
	std::unique_ptr<bool> will_crit = nullptr; // optional // Will always result in a critical hit
	std::unique_ptr<bool> calls_move = nullptr; // optional // Indicates if the move calls another move

    // Mechanics flags
	std::unique_ptr<bool> has_crash_damage = nullptr; // optional // Indicates if the move has crash damage
	std::unique_ptr<bool> is_confusion_self_hit = nullptr; // optional // Indicates if the move is a self-hit due to confusion
	std::unique_ptr<bool> stalling_move = nullptr; // optional // Indicates if the move is a stalling move
	std::unique_ptr<ID> base_move = nullptr; // optional // Base move ID for moves that are based on another move

	MoveData() = default;

	MoveData(
		// MoveData
		const std::string& name,
		MoveTarget target,
		MoveFlags& flags,
		int base_power = 0,
		std::variant<bool, int> accuracy = false,
		int pp = 0,
		MoveCategory category = MoveCategory::STATUS,
		const std::string& type = "",
		int priority = 0);

		//// MoveData optional
		//std::unique_ptr<int> num = nullptr,
		//std::unique_ptr<ConditionData> condition,
		//std::unique_ptr<std::string> real_move = nullptr,
		//std::unique_ptr<std::variant<std::monostate, int, std::string, bool>> damage = nullptr,
		//std::unique_ptr<std::string> contest_type = nullptr,
		//std::unique_ptr<bool> no_pp_boosts = nullptr,
		//std::unique_ptr<std::variant<bool, IDEntry>> is_z = nullptr,
		//std::unique_ptr<ZMoveData> z_move = nullptr,
		//std::unique_ptr<std::variant<bool, std::string>> is_max = nullptr,
		//std::unique_ptr<MaxMoveData> max_move = nullptr,
		//std::unique_ptr<std::variant<bool, std::string>> ohko = nullptr,
		//std::unique_ptr<bool> thaws_target = nullptr,
		//std::unique_ptr<std::vector<int>> heal = nullptr,
		//std::unique_ptr<bool> force_switch = nullptr,
		//std::unique_ptr<std::variant<SelfSwitchType, bool>> self_switch = nullptr,
		//std::unique_ptr<SelfBoost> self_boost = nullptr,
		//std::unique_ptr<std::variant<SelfDestructType, bool>> selfdestruct = nullptr,
		//std::unique_ptr<bool> breaks_protect = nullptr,
		//std::unique_ptr<std::array<int, 2>> recoil = nullptr,
		//std::unique_ptr<std::array<int, 2>> drain = nullptr,
		//std::unique_ptr<bool> mind_blown_recoil = nullptr,
		//std::unique_ptr<bool> steals_boosts = nullptr,
		//std::unique_ptr<bool> struggle_recoil = nullptr,
		//std::unique_ptr<SecondaryEffect> secondary = nullptr,
		//std::unique_ptr<std::vector<SecondaryEffect>> secondaries = nullptr,
		//std::unique_ptr<SecondaryEffect> self = nullptr,
		//std::unique_ptr<bool> has_sheer_force = nullptr,
		//std::unique_ptr<bool> always_hit = nullptr,
		//std::unique_ptr<std::string> base_move_type = nullptr,
		//std::unique_ptr<int> base_power_modifier = nullptr,
		//std::unique_ptr<int> crit_modifier = nullptr,
		//std::unique_ptr<int> crit_ratio = nullptr,
		//std::unique_ptr<std::string> override_offensive_pokemon = nullptr,
		//std::unique_ptr<StatIDExceptHP> override_offensive_stat = nullptr,
		//std::unique_ptr<std::string> override_defensive_pokemon = nullptr,
		//std::unique_ptr<StatIDExceptHP> override_defensive_stat = nullptr,
		//std::unique_ptr<bool> force_stab = nullptr,
		//std::unique_ptr<bool> ignore_ability = nullptr,
		//std::unique_ptr<bool> ignore_accuracy = nullptr,
		//std::unique_ptr<bool> ignore_defensive = nullptr,
		//std::unique_ptr<bool> ignore_evasion = nullptr,
		//std::unique_ptr<std::variant<bool, std::unordered_map<std::string, bool>>> ignore_immunity = nullptr,
		//std::unique_ptr<bool> ignore_negative_offensive = nullptr,
		//std::unique_ptr<bool> ignore_offensive = nullptr,
		//std::unique_ptr<bool> ignore_positive_defensive = nullptr,
		//std::unique_ptr<bool> ignore_positive_evasion = nullptr,
		//std::unique_ptr<bool> multiaccuracy = nullptr,
		//std::unique_ptr<std::variant<int, std::vector<int>>> multihit = nullptr,
		//std::unique_ptr<std::string> multihit_type = nullptr,
		//std::unique_ptr<bool> no_damage_variance = nullptr,
		//std::unique_ptr<MoveTarget> non_ghost_target,
		//std::unique_ptr<int> spread_modifier = nullptr,
		//std::unique_ptr<bool> sleep_usable = nullptr,
		//std::unique_ptr<bool> smart_target = nullptr,
		//std::unique_ptr<bool> tracks_target = nullptr,
		//std::unique_ptr<bool> will_crit = nullptr,
		//std::unique_ptr<bool> calls_move = nullptr,
		//std::unique_ptr<bool> has_crash_damage = nullptr,
		//std::unique_ptr<bool> is_confusion_self_hit = nullptr,
		//std::unique_ptr<bool> stalling_move = nullptr,
		//std::unique_ptr<ID> base_move = nullptr,
		
		//// EffectData
		//std::unique_ptr<std::string> desc = nullptr,
		//std::unique_ptr<int> duration = nullptr,
		//std::unique_ptr<std::function<int(Battle*, Pokemon*, Pokemon*, Effect*)>> duration_callback = nullptr,
		//std::unique_ptr<EffectType> effect_type = nullptr,
		//std::unique_ptr<bool> infiltrates = nullptr,
		//std::unique_ptr<NonStandard> is_nonstandard = nullptr,
		//std::unique_ptr<std::string> short_desc = nullptr,
		//
		//// HitEffect
		//std::unique_ptr<OnHitFunc> on_hit = nullptr,
		//std::unique_ptr<SparseBoostsTable> boosts = nullptr,
		//std::unique_ptr<std::string> status = nullptr,
		//std::unique_ptr<std::string> volatile_status = nullptr,
		//std::unique_ptr<std::string> side_condition = nullptr,
		//std::unique_ptr<std::string> slot_condition = nullptr,
		//std::unique_ptr<std::string> pseudo_weather = nullptr,
		//std::unique_ptr<std::string> terrain = nullptr,
		//std::unique_ptr<std::string> weather = nullptr);

	MoveData(const MoveData& other);

	DataType get_data_type() const override;

	std::unique_ptr<IDexData> clone() const override;
};