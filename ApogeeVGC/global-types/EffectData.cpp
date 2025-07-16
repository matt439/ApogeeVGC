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

EffectData::EffectData(const std::string& name,
	const std::string& desc,
	EffectType effect_type,
	NonStandard is_nonstandard,
	std::string short_desc) :
	name(std::make_unique<std::string>(name)),
	desc(std::make_unique<std::string>(desc)),
	effect_type(std::make_unique<EffectType>(effect_type)),
	is_nonstandard(std::make_unique<NonStandard>(is_nonstandard)),
	short_desc(std::make_unique<std::string>(short_desc))
{
}

EffectData::EffectData(const std::string& name) :
	name(std::make_unique<std::string>(name))
{
}

EffectData::EffectData(const EffectData& other) :
	name(other.name ? std::make_unique<std::string>(*other.name) : nullptr),
	desc(other.desc ? std::make_unique<std::string>(*other.desc) : nullptr),
	duration(other.duration ? std::make_unique<int>(*other.duration) : nullptr),
	duration_callback(other.duration_callback ? std::make_unique<std::function<int(Battle*, Pokemon*, Pokemon*, Effect*)>>(*other.duration_callback) : nullptr),
	effect_type(other.effect_type ? std::make_unique<EffectType>(*other.effect_type) : nullptr),
	infiltrates(other.infiltrates ? std::make_unique<bool>(*other.infiltrates) : nullptr),
	is_nonstandard(other.is_nonstandard ? std::make_unique<NonStandard>(*other.is_nonstandard) : nullptr),
	short_desc(other.short_desc ? std::make_unique<std::string>(*other.short_desc) : nullptr)
{
}