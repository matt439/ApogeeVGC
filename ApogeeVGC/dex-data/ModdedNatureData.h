#pragma once

#include "NatureData.h"

struct ModdedNatureData : public NatureData
{
	ModdedNatureData() = default;
	ModdedNatureData(const ModdedNatureData&) = default;
	ModdedNatureData(const NatureData& base);
};