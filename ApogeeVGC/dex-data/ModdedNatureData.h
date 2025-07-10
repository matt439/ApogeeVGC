#pragma once

#include "NatureData.h"

struct ModdedNatureData : public NatureData
{
	ModdedNatureData() = default;
	ModdedNatureData(std::unique_ptr<StatIDExceptHP> plus = nullptr,
		std::unique_ptr<StatIDExceptHP> minus = nullptr);
};