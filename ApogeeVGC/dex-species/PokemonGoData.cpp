#include "PokemonGoData.h"

DataType PokemonGoData::get_data_type() const
{
	return DataType::POKEMON_GO_DATA;
}

std::unique_ptr<IDexData> PokemonGoData::clone() const
{
	return std::make_unique<PokemonGoData>(*this);
}