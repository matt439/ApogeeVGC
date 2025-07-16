#pragma once

#include "../dex/IDexData.h"

struct SpeciesData : public IDexData
{
	int x = 0;

	DataType get_data_type() const override;
};