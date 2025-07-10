#pragma once

#include "../dex/IModdedDex.h"

class DexAbilities
{
public:
	IModdedDex* dex = nullptr;

	DexAbilities() = default;
	DexAbilities(IModdedDex* dex_ptr);
};
