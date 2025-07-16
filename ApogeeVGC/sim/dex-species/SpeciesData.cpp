#include "SpeciesData.h"

DataType SpeciesData::get_data_type() const
{
	return DataType::POKEDEX;
}

std::unique_ptr<IDexData> SpeciesData::clone() const
{
	return std::make_unique<SpeciesData>(*this);
}