#include "ModdedEffectData.h"

ModdedEffectData::ModdedEffectData(const EffectData& base)
	: EffectData(base), inherit(true)
{
}