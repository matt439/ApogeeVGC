#pragma once

#include "../dex/IDexDataManager.h"
#include "../dex/IModdedDex.h"

//class IModdedDex;

class DexSpecies : public IDexDataManager
{
public:
	IModdedDex* dex = nullptr;

	DexSpecies() = default;
	DexSpecies(IModdedDex* dex_ptr);

	DataType get_data_type() const override;
};