#include "Ability.h"

//Ability::Ability(const AbilityData& data) : BasicEffect(data)
//{
//    fullname = "ability: " + name;
//    // effect_type is always "Ability"
//
//    if (data.suppress_weather != nullptr)
//    {
//        suppress_weather = *data.suppress_weather;
//    }
//	if (data.flags != nullptr)
//	{
//        flags = *data.flags;
//	}
//	if (data.rating != nullptr)
//	{
//		rating = *data.rating;
//	}
//	if (data.condition != nullptr)
//	{
//		condition = data.condition;
//	}
//
//    // Set gen based on num if not already set
//    if (gen == 0)
//    {
//        if (num >= 268) gen = 9;
//        else if (num >= 234) gen = 8;
//        else if (num >= 192) gen = 7;
//        else if (num >= 165) gen = 6;
//        else if (num >= 124) gen = 5;
//        else if (num >= 77) gen = 4;
//        else if (num >= 1) gen = 3;
//    }
//}

Ability::Ability(
	const std::string& name,
    int rating,
    bool suppress_weather,
	std::unique_ptr<AbilityFlags> flags,
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
    const std::string& source_effect,
    // optional
    std::unique_ptr<ConditionData> condition,
    std::unique_ptr<int> duration,
    std::unique_ptr<std::string> status,
    std::unique_ptr<std::string> weather,
    std::unique_ptr<std::function<int(Battle*, Pokemon*, Pokemon*, Effect*)>> duration_callback,
    std::unique_ptr<bool> infiltrates) :
	rating(rating),
	suppress_weather(suppress_weather),
	flags(std::move(flags)),
	condition(std::move(condition)),
	BasicEffect(
		name,
		real_move,
		full_name,
		EffectType::ABILITY,
		exists,
		num,
		gen,
		short_desc,
		desc,
		is_nonstandard,
		no_copy,
		affects_fainted,
		source_effect,
		std::move(duration),
		std::move(status),
		std::move(weather),
		std::move(duration_callback),
		std::move(infiltrates))
{
	fullname = "ability: " + name;
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

//Ability::Ability(
//    const std::string& name,
//    int rating,
//    bool suppress_weather,
//    AbilityFlags flags,
//    std::unique_ptr<ConditionData> condition,
//    const std::string& real_move,
//    const std::string& full_name,
//    // EffectType effect_type = EffectType::CONDITION,
//    bool* exists,
//    int num,
//    int gen,
//    const std::string& short_desc,
//    const std::string& desc,
//    std::unique_ptr<NonStandard> is_nonstandard,
//    std::unique_ptr<int> duration,
//    bool no_copy,
//    bool affects_fainted,
//    std::unique_ptr<std::string> status,
//    std::unique_ptr<std::string> weather,
//    const std::string& source_effect,
//    std::unique_ptr<std::function<int(Battle*, Pokemon*, Pokemon*, Effect*)>> duration_callback,
//    std::unique_ptr<bool> infiltrates) :
//    rating(rating),
//    suppress_weather(suppress_weather),
//    flags(flags),
//    condition(std::move(condition)),
//    BasicEffect(
//	    name,
//	    real_move,
//	    full_name,
//	    EffectType::ABILITY,
//	    exists,
//	    num,
//	    gen,
//	    short_desc,
//	    desc,
//	    std::move(is_nonstandard),
//	    std::move(duration),
//	    no_copy,
//	    affects_fainted,
//	    std::move(status),
//	    std::move(weather),
//	    source_effect,
//	    std::move(duration_callback),
//	    std::move(infiltrates))
//{
//    fullname = "ability: " + name;
//    
//    // Set gen based on num if not already set
//    if (gen == 0)
//    {
//        if (num >= 268) gen = 9;
//        else if (num >= 234) gen = 8;
//        else if (num >= 192) gen = 7;
//        else if (num >= 165) gen = 6;
//        else if (num >= 124) gen = 5;
//        else if (num >= 77) gen = 4;
//        else if (num >= 1) gen = 3;
//    }
//}