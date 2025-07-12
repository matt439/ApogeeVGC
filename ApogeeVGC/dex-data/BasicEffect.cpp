#include "BasicEffect.h"

#include "to_id.h"

//BasicEffect::BasicEffect(const BasicEffectData& data)
//{
//    name = data.name; // name is not optional
//
//    if (data.real_move != nullptr)
//    {
//        id = to_id(*data.real_move);
//    }
//    else
//    {
//        id = to_id(name);
//    }
//
//	if (data.fullname != nullptr)
//	{
//		fullname = *data.fullname;
//	}
//	else
//	{
//		fullname = name;
//	}
//
//	if (data.effect_type != nullptr)
//	{
//		effect_type = *data.effect_type;
//	}
//
//	if (data.exists != nullptr)
//	{
//		exists = *data.exists;
//	}
//	else
//	{
//		exists = !id.empty();
//	}
//
//	if (data.num != nullptr)
//	{
//		num = *data.num;
//	}
//
//	if (data.gen != nullptr)
//	{
//		gen = *data.gen;
//	}
//
//	if (data.short_desc != nullptr)
//	{
//		short_desc = *data.short_desc;
//	}
//
//	if (data.desc != nullptr)
//	{
//		desc = *data.desc;
//	}
//
//	if (data.is_nonstandard != nullptr)
//	{
//		is_nonstandard = data.is_nonstandard;
//	}
//
//	if (data.duration != nullptr)
//	{
//		duration = data.duration;
//	}
//
//	if (data.no_copy != nullptr)
//	{
//		no_copy = *data.no_copy;
//	}
//
//	if (data.affects_fainted != nullptr)
//	{
//		affects_fainted = *data.affects_fainted;
//	}
//
//	if (data.status != nullptr)
//	{
//		status = data.status;
//	}
//
//	if (data.weather != nullptr)
//	{
//		weather = data.weather;
//	}
//
//	if (data.source_effect != nullptr)
//	{
//		source_effect = *data.source_effect;
//	}
//}

BasicEffect::BasicEffect(
    const std::string& name,
    const std::string& real_move,
    const std::string& full_name,
    EffectType effect_type,
    bool exists,
    int num,
    int gen,
    const std::string& short_desc,
    const std::string& desc,
    NonStandard is_nonstandard,
    bool no_copy,
    bool affects_fainted,
	const std::string& source_effect,
	std::unique_ptr<int> duration,
    std::unique_ptr<std::string> status,
    std::unique_ptr<std::string> weather,
	std::unique_ptr<std::function<int(Battle*, Pokemon*, Pokemon*, Effect*)>> duration_callback,
	std::unique_ptr<bool> infiltrates) :
	num(num),
	gen(gen),
	no_copy(no_copy),
	affects_fainted(affects_fainted),
	source_effect(source_effect),
	status(std::move(status)),
	weather(std::move(weather)),
EffectData(
	std::make_unique<std::string>(name),
	std::make_unique<std::string>(desc),
	// can't use std::move here because its already been moved above
	std::move(duration),
	std::move(duration_callback),
	std::make_unique<EffectType>(effect_type),
	std::move(infiltrates),
	// can't use std::move here because its already been moved above
	std::make_unique<NonStandard>(is_nonstandard),
	std::make_unique<std::string>(short_desc))
{
	EffectData::name = std::make_unique<std::string>(name);

	if (!real_move.empty())
		this->id = to_id(real_move);
	else
		this->id = to_id(name);

	if (!full_name.empty())
		this->fullname = full_name;
	else
		this->fullname = name;

	if (!exists)
		this->exists = !this->id.empty();
	else
		this->exists = exists;
}

BasicEffect::BasicEffect(const BasicEffect& other) :
	EffectData(other),
	id(other.id),
	fullname(other.fullname),
	exists(other.exists),
	num(other.num),
	gen(other.gen),
	no_copy(other.no_copy),
	affects_fainted(other.affects_fainted),
	status(other.status ? std::make_unique<std::string>(*other.status) : nullptr),
	weather(other.weather ? std::make_unique<std::string>(*other.weather) : nullptr),
	source_effect(other.source_effect)
{
}

//BasicEffect::BasicEffect(
//    const std::string& name,
//    const std::string& real_move,
//    const std::string& full_name,
//    EffectType effect_type,
//    bool exists,
//    int num,
//    int gen,
//    const std::string& short_desc,
//    const std::string& desc,
//    NonStandard is_nonstandard,
//    bool no_copy,
//    bool affects_fainted,
//    const std::string& source_effect,
//    const BasicEffectOptionalData& optional_data) :



std::string_view BasicEffect::to_string() const
{
    return *name;
}