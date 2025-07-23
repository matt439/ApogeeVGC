#include "LearnsetData.h"

DataType LearnsetData::get_data_type() const
{
	return DataType::LEARNSETS;
}

std::unique_ptr<IDexData> LearnsetData::clone() const
{
	return std::make_unique<LearnsetData>(*this);
}