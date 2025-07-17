#pragma once

#include "../dex-data/BasicEffect.h"
#include "../dex-conditions/ConditionData.h"
#include "../global-types/EffectType.h"
#include "AbilityFlags.h"
#include <memory>
#include <string>
#include <functional>

class Ability : public BasicEffect
{
public:
    int rating = 0;
    bool suppress_weather = false;
	std::unique_ptr<AbilityFlags> flags = nullptr; // not optional despite being a pointer
	std::unique_ptr<ConditionData> condition = nullptr; // optional

	Ability() = default;

    Ability(
        const std::string& name,
        int rating = 0,
        bool suppress_weather = false,
        std::unique_ptr<AbilityFlags> flags = std::make_unique<AbilityFlags>(),
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
  //      // optional
		//std::unique_ptr<ConditionData> condition = nullptr,
  //      std::unique_ptr<int> duration = nullptr,
  //      std::unique_ptr<std::string> status = nullptr,
  //      std::unique_ptr<std::string> weather = nullptr,
  //      std::unique_ptr<std::function<int(Battle*, Pokemon*, Pokemon*, Effect*)>> duration_callback = nullptr,
  //      std::unique_ptr<bool> infiltrates = nullptr);

    Ability(const Ability& other);
};