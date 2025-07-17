#include "MoveData.h"

MoveData::MoveData(
	// MoveData
	const std::string& name,
	MoveTarget target,
	MoveFlags& flags,
	int base_power,
	std::variant<bool, int> accuracy,
	int pp,
	MoveCategory category,
	const std::string& type,
	int priority) :

	//// MoveData optional
	//std::unique_ptr<int> num,
	//std::unique_ptr<ConditionData> condition,
	//std::unique_ptr<std::string> real_move,
	//std::unique_ptr<std::variant<std::monostate, int, std::string, bool>> damage,
	//std::unique_ptr<std::string> contest_type,
	//std::unique_ptr<bool> no_pp_boosts,
	//std::unique_ptr<std::variant<bool, IDEntry>> is_z,
	//std::unique_ptr<ZMoveData> z_move,
	//std::unique_ptr<std::variant<bool, std::string>> is_max,
	//std::unique_ptr<MaxMoveData> max_move,
	//std::unique_ptr<std::variant<bool, std::string>> ohko,
	//std::unique_ptr<bool> thaws_target,
	//std::unique_ptr<std::vector<int>> heal,
	//std::unique_ptr<bool> force_switch,
	//std::unique_ptr<std::variant<SelfSwitchType, bool>> self_switch,
	//std::unique_ptr<SelfBoost> self_boost,
	//std::unique_ptr<std::variant<SelfDestructType, bool>> selfdestruct,
	//std::unique_ptr<bool> breaks_protect,
	//std::unique_ptr<std::array<int, 2>> recoil,
	//std::unique_ptr<std::array<int, 2>> drain,
	//std::unique_ptr<bool> mind_blown_recoil,
	//std::unique_ptr<bool> steals_boosts,
	//std::unique_ptr<bool> struggle_recoil,
	//std::unique_ptr<SecondaryEffect> secondary,
	//std::unique_ptr<std::vector<SecondaryEffect>> secondaries,
	//std::unique_ptr<SecondaryEffect> self,
	//std::unique_ptr<bool> has_sheer_force,
	//std::unique_ptr<bool> always_hit,
	//std::unique_ptr<std::string> base_move_type,
	//std::unique_ptr<int> base_power_modifier,
	//std::unique_ptr<int> crit_modifier,
	//std::unique_ptr<int> crit_ratio,
	//std::unique_ptr<std::string> override_offensive_pokemon,
	//std::unique_ptr<StatIDExceptHP> override_offensive_stat,
	//std::unique_ptr<std::string> override_defensive_pokemon,
	//std::unique_ptr<StatIDExceptHP> override_defensive_stat,
	//std::unique_ptr<bool> force_stab,
	//std::unique_ptr<bool> ignore_ability,
	//std::unique_ptr<bool> ignore_accuracy,
	//std::unique_ptr<bool> ignore_defensive,
	//std::unique_ptr<bool> ignore_evasion,
	//std::unique_ptr<std::variant<bool, std::unordered_map<std::string, bool>>> ignore_immunity,
	//std::unique_ptr<bool> ignore_negative_offensive,
	//std::unique_ptr<bool> ignore_offensive,
	//std::unique_ptr<bool> ignore_positive_defensive,
	//std::unique_ptr<bool> ignore_positive_evasion,
	//std::unique_ptr<bool> multiaccuracy,
	//std::unique_ptr<std::variant<int, std::vector<int>>> multihit,
	//std::unique_ptr<std::string> multihit_type,
	//std::unique_ptr<bool> no_damage_variance,
	//std::unique_ptr<MoveTarget> non_ghost_target,
	//std::unique_ptr<int> spread_modifier,
	//std::unique_ptr<bool> sleep_usable,
	//std::unique_ptr<bool> smart_target,
	//std::unique_ptr<bool> tracks_target,
	//std::unique_ptr<bool> will_crit,
	//std::unique_ptr<bool> calls_move,
	//std::unique_ptr<bool> has_crash_damage,
	//std::unique_ptr<bool> is_confusion_self_hit,
	//std::unique_ptr<bool> stalling_move,
	//std::unique_ptr<ID> base_move,

	//// EffectData
	//std::unique_ptr<std::string> desc,
	//std::unique_ptr<int> duration,
	//std::unique_ptr<std::function<int(Battle*, Pokemon*, Pokemon*, Effect*)>> duration_callback,
	//std::unique_ptr<EffectType> effect_type,
	//std::unique_ptr<bool> infiltrates,
	//std::unique_ptr<NonStandard> is_nonstandard,
	//std::unique_ptr<std::string> short_desc,

	//// HitEffect
	//std::unique_ptr<OnHitFunc> on_hit,
	//std::unique_ptr<SparseBoostsTable> boosts,
	//std::unique_ptr<std::string> status,
	//std::unique_ptr<std::string> volatile_status,
	//std::unique_ptr<std::string> side_condition,
	//std::unique_ptr<std::string> slot_condition,
	//std::unique_ptr<std::string> pseudo_weather,
	//std::unique_ptr<std::string> terrain,
	//std::unique_ptr<std::string> weather) :

	//EffectData(
	//	std::make_unique<std::string>(name),
	//	std::make_unique<std::string>(desc ? *desc : ""),
	//	std::move(duration),
	//	std::move(duration_callback),
	//	std::move(effect_type),
	//	std::move(infiltrates),
	//	std::move(is_nonstandard),
	//	std::move(short_desc)),
	//HitEffect(
	//	std::move(on_hit),
	//	std::move(boosts),
	//	std::move(status),
	//	std::move(volatile_status),
	//	std::move(side_condition),
	//	std::move(slot_condition),
	//	std::move(pseudo_weather),
	//	std::move(terrain),
	//	std::move(weather)),


EffectData(name),
MoveEventMethods(),
HitEffect(),
	target(target),
	flags(flags),
	base_power(base_power),
	accuracy(accuracy),
	pp(pp),
	category(category),
	type(type),
	priority(priority)
{
}

MoveData::MoveData(const MoveData& other) :
	EffectData(other),
	MoveEventMethods(other),
	HitEffect(other),
	target(other.target),
	flags(other.flags),
	base_power(other.base_power),
	accuracy(other.accuracy),
	pp(other.pp),
	category(other.category),
	type(other.type),
	priority(other.priority)
{
}

DataType MoveData::get_data_type() const
{
	return DataType::MOVES;
}

std::unique_ptr<IDexData> MoveData::clone() const
{
	return std::make_unique<MoveData>(*this);
}