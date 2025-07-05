#pragma once

#include "EffectData.h"
#include "MoveEventMethods.h"
#include "HitEffect.h"
#include <string>
#include <vector>
#include <optional>
#include <variant>
#include <array>
#include <unordered_map>

enum class MoveCategory
{
    PHYSICAL,
    SPECIAL,
    STATUS,
};
enum class SelfSwitchType
{
    COPY_VOLATILE,
	SHED_TAIL,
};
enum class SelfDestructType
{
    ALWAYS,
	IF_HIT,
};
struct ZMoveData {
    std::optional<int> base_power;
    std::optional<IDEntry> effect;
    std::optional<SparseBoostsTable> boost;
};
struct MaxMoveData {
    int base_power;
};
struct SelfBoost {
    std::optional<SparseBoostsTable> boosts;
};

struct MoveData : public EffectData, public MoveEventMethods, public HitEffect
{
    std::string name;
    std::optional<int> num;
    // std::optional<ConditionData> condition; // Define ConditionData as needed
    int base_power;
    std::variant<bool, int> accuracy; // true or number
    int pp;
    MoveCategory category;
    std::string type;
    int priority;
    // MoveTarget target; // Define as needed
    // MoveFlags flags;   // Define as needed
    std::optional<std::string> real_move;

    std::variant<std::monostate, int, std::string, bool> damage; // number | 'level' | false | null
    std::optional<std::string> contest_type;
    std::optional<bool> no_pp_boosts;

    // Z-move data
    std::variant<bool, IDEntry> is_z;
    std::optional<ZMoveData> z_move;

    // Max move data
    std::variant<bool, std::string> is_max;
    std::optional<MaxMoveData> max_move;

    // Hit effects
    std::variant<bool, std::string> ohko; // boolean | 'Ice'
    std::optional<bool> thaws_target;
    std::optional<std::vector<int>> heal;
    std::optional<bool> force_switch;
    std::variant<SelfSwitchType, bool> self_switch;
    std::optional<SelfBoost> self_boost;
    std::variant<SelfDestructType, bool> selfdestruct;
    std::optional<bool> breaks_protect;
    std::optional<std::array<int, 2>> recoil;
    std::optional<std::array<int, 2>> drain;
    std::optional<bool> mind_blown_recoil;
    std::optional<bool> steals_boosts;
    std::optional<bool> struggle_recoil;
    // std::optional<SecondaryEffect> secondary; // Define as needed
    // std::optional<std::vector<SecondaryEffect>> secondaries;
    // std::optional<SecondaryEffect> self;
    std::optional<bool> has_sheer_force;

    // Hit effect modifiers
    std::optional<bool> always_hit;
    std::optional<std::string> base_move_type;
    std::optional<int> base_power_modifier;
    std::optional<int> crit_modifier;
    std::optional<int> crit_ratio;
    std::optional<std::string> override_offensive_pokemon; // 'target' | 'source'
    std::optional<StatIDExceptHP> override_offensive_stat;
    std::optional<std::string> override_defensive_pokemon; // 'target' | 'source'
    std::optional<StatIDExceptHP> override_defensive_stat;
    std::optional<bool> force_stab;
    std::optional<bool> ignore_ability;
    std::optional<bool> ignore_accuracy;
    std::optional<bool> ignore_defensive;
    std::optional<bool> ignore_evasion;
    std::variant<bool, std::unordered_map<std::string, bool>> ignore_immunity;
    std::optional<bool> ignore_negative_offensive;
    std::optional<bool> ignore_offensive;
    std::optional<bool> ignore_positive_defensive;
    std::optional<bool> ignore_positive_evasion;
    std::optional<bool> multiaccuracy;
    std::variant<int, std::vector<int>> multihit;
    std::optional<std::string> multihit_type; // 'parentalbond'
    std::optional<bool> no_damage_variance;
    // MoveTarget non_ghost_target; // Define as needed
    std::optional<int> spread_modifier;
    std::optional<bool> sleep_usable;
    std::optional<bool> smart_target;
    std::optional<bool> tracks_target;
    std::optional<bool> will_crit;
    std::optional<bool> calls_move;

    // Mechanics flags
    std::optional<bool> has_crash_damage;
    std::optional<bool> is_confusion_self_hit;
    std::optional<bool> stalling_move;
    std::optional<ID> base_move;
};