#pragma once

#include "../dex/IDexDataManager.h"

class IModdedDex;

class DexMoves : public IDexDataManager
{
public:
	IModdedDex* dex = nullptr;

	DexMoves(IModdedDex* dex_ptr);

	DataType get_data_type() const override;
};