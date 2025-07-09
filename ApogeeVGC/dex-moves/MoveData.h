#pragma once

#include "../global-types/EffectData.h"
#include "MoveEventMethods.h"
#include "HitEffect.h"
#include "ZMoveData.h"
#include "MaxMoveData.h"
#include "MoveCategory.h"
#include "SecondaryEffect.h"
#include "SelfSwitchType.h"
#include "SelfDestructType.h"
#include "SelfBoost.h"

struct MoveData : public EffectData, MoveEventMethods, HitEffect
{
	std::string name = "";
	std::optional<int> num = std::nullopt;
    // std::optional<ConditionData> condition; // Define ConditionData as needed
	int base_power = 0;
	std::variant<bool, int> accuracy = false; // boolean | number
    int pp = 0;
	MoveCategory category = MoveCategory::STATUS;
    std::string type = "";
	int priority = 0;
    // MoveTarget target; // Define as needed
    // MoveFlags flags;   // Define as needed
	std::optional<std::string> real_move = std::nullopt;

    std::variant<std::monostate, int, std::string, bool> damage = false; // number | 'level' | false | null
	std::optional<std::string> contest_type = std::nullopt;
	std::optional<bool> no_pp_boosts = std::nullopt;

    // Z-move data
	std::variant<bool, IDEntry> is_z = false; // boolean | IDEntry
	std::optional<ZMoveData> z_move = std::nullopt;

    // Max move data
	std::variant<bool, std::string> is_max = false; // boolean | 'Water' | 'Fire' | etc.
	std::optional<MaxMoveData> max_move = std::nullopt;

    // Hit effects
    std::variant<bool, std::string> ohko = false; // boolean | 'Ice'
	std::optional<bool> thaws_target = std::nullopt;
	std::optional<std::vector<int>> heal = std::nullopt; // [amount, max_amount]
	std::optional<bool> force_switch = std::nullopt;
	std::variant<SelfSwitchType, bool> self_switch = false; // SelfSwitchType | boolean
	std::optional<SelfBoost> self_boost = std::nullopt; // SelfBoost struct
	std::variant<SelfDestructType, bool> selfdestruct = false; // SelfDestructType | boolean
	std::optional<bool> breaks_protect = std::nullopt;
	std::optional<std::array<int, 2>> recoil = std::nullopt; // [amount, max_amount]
	std::optional<std::array<int, 2>> drain = std::nullopt; // [amount, max_amount]
	std::optional<bool> mind_blown_recoil = std::nullopt;
	std::optional<bool> steals_boosts = std::nullopt;
	std::optional<bool> struggle_recoil = std::nullopt;
	std::optional<SecondaryEffect> secondary = std::nullopt; // Single secondary effect
	std::optional<std::vector<SecondaryEffect>> secondaries = std::nullopt; // Multiple secondary effects
	std::optional<SecondaryEffect> self = std::nullopt; // Self-targeting secondary effect
	std::optional<bool> has_sheer_force = std::nullopt; // Indicates if the move has Sheer Force effect

    // Hit effect modifiers
	std::optional<bool> always_hit = std::nullopt;
	std::optional<std::string> base_move_type = std::nullopt; // 'Water' | 'Fire' | etc.
	std::optional<int> base_power_modifier = std::nullopt; // Modifier for base power
	std::optional<int> crit_modifier = std::nullopt; // Modifier for critical hit power
	std::optional<int> crit_ratio = std::nullopt; // Critical hit ratio
    std::optional<std::string> override_offensive_pokemon = std::nullopt; // 'target' | 'source'
	std::optional<StatIDExceptHP> override_offensive_stat = std::nullopt; // Stat to override for offensive calculations
    std::optional<std::string> override_defensive_pokemon = std::nullopt; // 'target' | 'source'
	std::optional<StatIDExceptHP> override_defensive_stat = std::nullopt; // Stat to override for defensive calculations
	std::optional<bool> force_stab = std::nullopt; // Force STAB (Same Type Attack Bonus)
	std::optional<bool> ignore_ability = std::nullopt; // Ignore target's ability
	std::optional<bool> ignore_accuracy = std::nullopt; // Ignore accuracy checks
	std::optional<bool> ignore_defensive = std::nullopt; // Ignore defensive modifiers
	std::optional<bool> ignore_evasion = std::nullopt; // Ignore evasion modifiers
	std::variant<bool, std::unordered_map<std::string, bool>> ignore_immunity = false; // boolean | { 'type': true/false }
	std::optional<bool> ignore_negative_offensive = std::nullopt; // Ignore negative offensive modifiers
	std::optional<bool> ignore_offensive = std::nullopt; // Ignore all offensive modifiers
	std::optional<bool> ignore_positive_defensive = std::nullopt; // Ignore positive defensive modifiers
	std::optional<bool> ignore_positive_evasion = std::nullopt; // Ignore positive evasion modifiers
	std::optional<bool> multiaccuracy = std::nullopt; // Multi-hit accuracy check
	std::variant<int, std::vector<int>> multihit = 1; // number | [min_hits, max_hits] or just number for single hit
	std::optional<std::string> multihit_type = std::nullopt; // 'parentalbond'
	std::optional<bool> no_damage_variance = std::nullopt; // No damage variance (e.g., no minimum or maximum damage)
    // MoveTarget non_ghost_target; // Define as needed
	std::optional<int> spread_modifier = std::nullopt; // Modifier for spread moves
	std::optional<bool> sleep_usable = std::nullopt; // Can be used while asleep
	std::optional<bool> smart_target = std::nullopt; // Smart targeting (e.g., ignores Substitute)
	std::optional<bool> tracks_target = std::nullopt; // Tracks target's position
	std::optional<bool> will_crit = std::nullopt; // Will always result in a critical hit
	std::optional<bool> calls_move = std::nullopt; // Indicates if the move calls another move

    // Mechanics flags
	std::optional<bool> has_crash_damage = std::nullopt; // Indicates if the move has crash damage
	std::optional<bool> is_confusion_self_hit = std::nullopt; // Indicates if the move is a self-hit due to confusion
	std::optional<bool> stalling_move = std::nullopt; // Indicates if the move is a stalling move
	std::optional<ID> base_move = std::nullopt; // Base move ID for moves that are based on another move
};