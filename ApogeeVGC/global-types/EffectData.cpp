#include "EffectData.h"

//EffectData::EffectData(const std::string& name)
//	: name(std::make_unique<std::string>(name))
//{
//}
//
//EffectData::EffectData(const std::string& name, const std::string& desc)
//	: name(std::make_unique<std::string>(name)), desc(std::make_unique<std::string>(desc))
//{
//}

EffectData::EffectData(
	std::unique_ptr<std::string> name,
	std::unique_ptr<std::string> desc,
	std::unique_ptr<int> duration,
	std::unique_ptr<std::function<int(Battle*, Pokemon*, Pokemon*, Effect*)>> duration_callback,
	std::unique_ptr<EffectType> effect_type,
	std::unique_ptr<bool> infiltrates,
	std::unique_ptr<NonStandard> is_nonstandard,
	std::unique_ptr<std::string> short_desc)
	: name(std::move(name)),
	desc(std::move(desc)),
	duration(std::move(duration)),
	duration_callback(std::move(duration_callback)),
	effect_type(std::move(effect_type)),
	infiltrates(std::move(infiltrates)),
	is_nonstandard(std::move(is_nonstandard)),
	short_desc(std::move(short_desc))
{
}