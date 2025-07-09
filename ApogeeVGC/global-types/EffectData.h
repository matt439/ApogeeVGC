#pragma once

#include "../battle/Battle.h"
#include "../pokemon/Pokemon.h"
#include "NonStandard.h"
#include "Effect.h"
#include <optional>
#include <string>
#include <functional>

struct EffectData
{
	std::optional<std::string> name = std::nullopt;
	std::optional<std::string> desc = std::nullopt;
	std::optional<int> duration = std::nullopt;
	std::optional<std::function<int(Battle*, Pokemon*, Pokemon*, std::optional<Effect>)>> duration_callback = std::nullopt;
	std::optional<std::string> effect_type = std::nullopt;
	std::optional<bool> infiltrates = std::nullopt;
	std::optional<NonStandard> is_nonstandard = std::nullopt;
	std::optional<std::string> short_desc = std::nullopt;

	EffectData() = default;
	EffectData(const EffectData&) = default;
};