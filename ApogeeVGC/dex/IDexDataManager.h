#pragma once

#include "DataType.h"

class IDexDataManager
{
public:
	virtual ~IDexDataManager() = default;
	virtual DataType get_data_type() const = 0;
};