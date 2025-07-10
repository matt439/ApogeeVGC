#pragma once

//#include "../dex-data/BasicEffectData.h"
#include "../dex-conditions/PokemonEventMethods.h"
#include "Ability.h"
#include "AbilityEventMethods.h"
#include <string>
//#include "AbilityFlags.h"

struct AbilityData : public Ability, public AbilityEventMethods, public PokemonEventMethods
{
    AbilityData(
        // Ability non-optional parameters
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
};