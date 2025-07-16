#pragma once

#include "../dex/IModdedDex.h"
#include "../dex/IDexDataManager.h"

class DexAbilities : public IDexDataManager
{
public:
	IModdedDex* dex = nullptr;

	DexAbilities() = default;
	DexAbilities(IModdedDex* dex_ptr);

	DataType get_data_type() const override;
};
