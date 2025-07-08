#pragma once

#include "function_type_aliases.h"

struct CommonHandlers
{
	ModifierEffectFunc ModifierEffect = nullptr;
	ModifierMoveFunc ModifierMove = nullptr;
	std::variant<bool, ResultMoveFunc> ResultMove = nullptr;
	std::variant<bool, ExtResultMoveFunc> ExtResultMove = nullptr;
	VoidEffectFunc VoidEffect = nullptr;
	VoidMoveFunc VoidMove = nullptr;
	ModifierSourceEffectFunc ModifierSourceEffect = nullptr;
	ModifierSourceMoveFunc ModifierSourceMove = nullptr;
	std::variant<bool, ResultSourceMoveFunc> ResultSourceMove = nullptr;
	std::variant<bool, ExtResultSourceMoveFunc> ExtResultSourceMove = nullptr;
	VoidSourceEffectFunc VoidSourceEffect = nullptr;
	VoidSourceMoveFunc VoidSourceMove = nullptr;

	CommonHandlers() = default;
	CommonHandlers(const CommonHandlers&) = default;
};