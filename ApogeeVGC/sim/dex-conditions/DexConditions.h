#pragma once

#include "../dex/IDexDataManager.h"
#include "../dex/IModdedDex.h"

// class IModdedDex;

class DexConditions : public IDexDataManager
{
public:
	IModdedDex* dex = nullptr;

	DexConditions() = default;
	DexConditions(IModdedDex* dex_ptr);
	DataType get_data_type() const override;
};