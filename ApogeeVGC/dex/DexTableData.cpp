#include "DexTableData.h"

#include <stdexcept>

IDexData* DexTableData::get_data(DataType data_type, const std::string& key)
{
	switch (data_type)
	{
	case DataType::ABILITIES:
		return abilities[key].get();
	case DataType::RULESETS:
		return rulesets[key].get();
	case DataType::ITEMS:
		return items[key].get();
	case DataType::LEARNSETS:
		return learnsets[key].get();
	case DataType::MOVES:
		return moves[key].get();
	case DataType::NATURES:
		return natures[key].get();
	case DataType::POKEDEX:
		return pokedex[key].get();
	case DataType::FORMATS_DATA:
		return formats_data[key].get();
	case DataType::POKEMON_GO_DATA:
		return pokemon_go_data[key].get();
	case DataType::SCRIPTS:
		return scripts[key].get();
	case DataType::CONDITIONS:
		return conditions[key].get();
	case DataType::TYPE_CHART:
		return type_chart[key].get();
	default:
		throw std::runtime_error("Unknown data type: " + std::to_string(static_cast<int>(data_type)));
	};
}

void DexTableData::set_data(IDexData* data, const std::string& key)
{
	DataType data_type = data->get_data_type();

	switch (data_type)
	{
	case DataType::ABILITIES:
		abilities[key] = std::unique_ptr<AbilityData>(static_cast<AbilityData*>(data));
		break;
	case DataType::RULESETS:
		rulesets[key] = std::unique_ptr<FormatData>(static_cast<FormatData*>(data));
		break;
	case DataType::ITEMS:
		items[key] = std::unique_ptr<ItemData>(static_cast<ItemData*>(data));
		break;
	case DataType::LEARNSETS:
		learnsets[key] = std::unique_ptr<LearnsetData>(static_cast<LearnsetData*>(data));
		break;
	case DataType::MOVES:
		moves[key] = std::unique_ptr<MoveData>(static_cast<MoveData*>(data));
		break;
	case DataType::NATURES:
		natures[key] = std::unique_ptr<NatureData>(static_cast<NatureData*>(data));
		break;
	case DataType::POKEDEX:
		pokedex[key] = std::unique_ptr<SpeciesData>(static_cast<SpeciesData*>(data));
		break;
	case DataType::FORMATS_DATA:
		formats_data[key] = std::unique_ptr<SpeciesFormatsData>(static_cast<SpeciesFormatsData*>(data));
		break;
	case DataType::POKEMON_GO_DATA:
		pokemon_go_data[key] = std::unique_ptr<PokemonGoData>(static_cast<PokemonGoData*>(data));
		break;
	case DataType::SCRIPTS:
		scripts[key] = std::unique_ptr<AnyObject>(static_cast<AnyObject*>(data));
		break;
	case DataType::CONDITIONS:
		conditions[key] = std::unique_ptr<ConditionData>(static_cast<ConditionData*>(data));
		break;
	case DataType::TYPE_CHART:
		type_chart[key] = std::unique_ptr<TypeData>(static_cast<TypeData*>(data));
		break;
	default:
		throw std::runtime_error("Unknown data type: " + std::to_string(static_cast<int>(data_type)));
	};
}