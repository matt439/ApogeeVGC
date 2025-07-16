#include "ActiveMove.h"

ActiveMove::ActiveMove(
	int hit,
	// MoveData
	const std::string& name,
	MoveTarget target,
	MoveFlags& flags,
	int base_power,
	std::variant<bool, int> accuracy,
	int pp,
	MoveCategory category,
	const std::string& type,
	int priority,
	// BasicEffect
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
	MutableMove(
		name, target, flags, base_power, accuracy, pp, category, type, priority,
		real_move, full_name, exists, num, gen, short_desc, desc,
		is_nonstandard, no_copy, affects_fainted, source_effect),
	hit(hit)
{
}

ActiveMove::ActiveMove(const ActiveMove& other) :
	MutableMove(other),
	hit(other.hit)
{
}