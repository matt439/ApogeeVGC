#pragma once

#include "DataType.h"

struct IDexData
{
	virtual ~IDexData() = default;
	virtual DataType get_data_type() const = 0;
};