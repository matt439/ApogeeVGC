#pragma once

#include "../dex/IDexData.h"

struct LearnsetData : public IDexData
{
	int x = 0;

	DataType get_data_type() const override;

	std::unique_ptr<IDexData> clone() const override;
};