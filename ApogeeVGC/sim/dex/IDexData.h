#pragma once

#include "DataType.h"
#include <memory>

class IDexData
{
public:
	virtual ~IDexData() = default;
	virtual DataType get_data_type() const = 0;
	virtual std::unique_ptr<IDexData> clone() const = 0;
};