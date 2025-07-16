#pragma once

#include "../dex-data/BasicEffect.h"
#include "MoveData.h"

struct MutableMove : public BasicEffect, public MoveData
{
	MutableMove() = default;

	MutableMove(
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
		//const std::string& name,
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

	MutableMove(const MutableMove& other);
};
