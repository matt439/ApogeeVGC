#include "Ability.h"

Ability::Ability(const AbilityData& data) : BasicEffect(data)
{
    fullname = "ability: " + name;
    // effect_type is always "Ability"
    suppress_weather = data.suppress_weather.value_or(false);
    flags = data.flags.value_or(AbilityFlags{});
    rating = data.rating.value_or(0);
    condition = data.condition;

    // Set gen based on num if not already set
    if (gen == 0)
    {
        if (num >= 268) gen = 9;
        else if (num >= 234) gen = 8;
        else if (num >= 192) gen = 7;
        else if (num >= 165) gen = 6;
        else if (num >= 124) gen = 5;
        else if (num >= 77) gen = 4;
        else if (num >= 1) gen = 3;
    }
}