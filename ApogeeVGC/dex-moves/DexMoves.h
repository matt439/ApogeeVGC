#pragma once

#include "../dex/IDexDataManager.h"

class DexMoves : public IDexDataManager
{
public:
    int x;

	DataType get_data_type() const override;
};