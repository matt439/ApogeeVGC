#pragma once

#include "EffectData.h"

struct ModdedEffectData : public EffectData
{
	bool inherit = false;

	ModdedEffectData() = default;
	ModdedEffectData(const ModdedEffectData&) = default;
	ModdedEffectData(const EffectData& base);
};