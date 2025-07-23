#include "SpeciesFormatsData.h"

DataType SpeciesFormatsData::get_data_type() const
{
	return DataType::FORMATS_DATA;
}

std::unique_ptr<IDexData> SpeciesFormatsData::clone() const
{
	return std::make_unique<SpeciesFormatsData>(*this);
}