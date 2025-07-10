#include "ModdedEffectData.h"

ModdedEffectData::ModdedEffectData(
	std::unique_ptr<std::string> name,
	std::unique_ptr<std::string> desc,
	std::unique_ptr<int> duration,
	std::unique_ptr<std::function<int(Battle*, Pokemon*, Pokemon*, Effect*)>> duration_callback,
	std::unique_ptr<EffectType> effect_type,
	std::unique_ptr<bool> infiltrates,
	std::unique_ptr<NonStandard> is_nonstandard,
	std::unique_ptr<std::string> short_desc) :
	EffectData(
		std::move(name),
		std::move(desc),
		std::move(duration),
		std::move(duration_callback),
		std::move(effect_type),
		std::move(infiltrates),
		std::move(is_nonstandard),
		std::move(short_desc))
{
}