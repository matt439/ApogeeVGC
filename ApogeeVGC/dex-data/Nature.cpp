#include "Nature.h"

Nature::Nature() : BasicEffect(BasicEffectData("", false)), NatureData(NatureData())
{
	fullname = "nature: " + BasicEffect::name;
	effect_type = EffectType::NATURE;
	gen = 3;
}

Nature::Nature(const BasicEffect& basic_effect, const NatureData& nature_data) :
	BasicEffect(basic_effect), NatureData(nature_data)
{
	fullname = "nature: " + NatureData::name;
	effect_type = EffectType::NATURE;
	gen = 3;
}