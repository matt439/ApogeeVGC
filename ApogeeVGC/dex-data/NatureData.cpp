#include "NatureData.h"

NatureData::NatureData(const std::string& name, const StatIDExceptHP* plus,
	const StatIDExceptHP* minus)
	: name(name), plus(plus), minus(minus)
{
}

NatureData::NatureData(const BasicEffectData& basic_effect_data,
	const StatIDExceptHP* plus, const StatIDExceptHP* minus)
	: name(basic_effect_data.name), plus(plus), minus(minus)
{
}