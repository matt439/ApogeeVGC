#pragma once

#include "../dex/IDexData.h"

struct SpeciesFormatsData : public IDexData
{
	int x = 0;

	DataType get_data_type() const override;
};