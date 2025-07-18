#pragma once

//#include "../battle/Battle.h"
//#include "../pokemon/Pokemon.h"
#include "NonStandard.h"
#include "Effect.h"
#include "EffectType.h"
#include <string>
#include <functional>
#include <memory>

class Battle;
class Pokemon;

struct EffectData
{
	std::unique_ptr<std::string> name = nullptr; // optional
	std::unique_ptr<std::string> desc = nullptr; // optional
	std::unique_ptr<int> duration = nullptr; // optional
	std::unique_ptr<std::function<int(Battle*, Pokemon*, Pokemon*, Effect*)>> duration_callback = nullptr; // optional
	std::unique_ptr<EffectType> effect_type = nullptr; // optional
	std::unique_ptr<bool> infiltrates = nullptr; // optional
	std::unique_ptr<NonStandard> is_nonstandard = nullptr; // optional
	std::unique_ptr<std::string> short_desc = nullptr; // optional

	EffectData() = default;
	EffectData(
		std::unique_ptr<std::string> name = nullptr,
		std::unique_ptr<std::string> desc = nullptr,
		std::unique_ptr<int> duration = nullptr,
		std::unique_ptr<std::function<int(Battle*, Pokemon*, Pokemon*, Effect*)>> duration_callback = nullptr,
		std::unique_ptr<EffectType> effect_type = nullptr,
		std::unique_ptr<bool> infiltrates = nullptr,
		std::unique_ptr<NonStandard> is_nonstandard = nullptr,
		std::unique_ptr<std::string> short_desc = nullptr);

	// This constructor is used by BasicEffect. It only includes the non-optional fields of BasicEffect.
	EffectData(const std::string& name,
		const std::string& desc,
		EffectType effect_type,
		NonStandard is_nonstandard,
		std::string short_desc);

	// This constructor is used by MoveData
	EffectData(const std::string& name);

	EffectData(const EffectData& other);
};