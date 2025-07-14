#pragma once

#include "../dex/IDexDataManager.h"

class DexStats : public IDexDataManager
{
public:
    int x = 0;

    DataType get_data_type() const override;
};