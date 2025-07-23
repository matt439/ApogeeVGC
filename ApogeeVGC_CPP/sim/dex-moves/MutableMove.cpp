#include "MutableMove.h"

MutableMove::MutableMove(
	// MoveData
	const std::string& name,
	MoveTarget target,
	MoveFlags& flags,
	int base_power,
	std::variant<bool, int> accuracy,
	int pp,
	MoveCategory categor,
	const std::string& type,
	int priority,
	// BasicEffect
	//const std::string& name,
	const std::string& real_move,
	const std::string& full_name,
	bool exists,
	int num,
	int gen,
	const std::string& short_desc,
	const std::string& desc,
	NonStandard is_nonstandard,
	bool no_copy,
	bool affects_fainted,
	const std::string& source_effect) :
	BasicEffect(name, real_move, full_name, EffectType::MOVE, exists, num,
		gen, short_desc, desc, is_nonstandard, no_copy, affects_fainted, source_effect),
	MoveData(name, target, flags, base_power, accuracy, pp, categor, type, priority)
{
}

MutableMove::MutableMove(const MutableMove& other) :
	BasicEffect(other),
	MoveData(other)
{
}