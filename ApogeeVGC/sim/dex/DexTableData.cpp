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

void DexTableData::set_data(const std::string& key, std::unique_ptr<IDexData> data)
{
	DataType data_type = data->get_data_type();

	switch (data_type)
	{
	case DataType::ABILITIES:
	{
		AbilityData* ability_data = dynamic_cast<AbilityData*>(data.get());
		abilities[key] = std::unique_ptr<AbilityData>(ability_data);
		break;
	}
	case DataType::RULESETS:
	{
		FormatData* format_data = dynamic_cast<FormatData*>(data.get());
		rulesets[key] = std::unique_ptr<FormatData>(format_data);
		break;
	}
	case DataType::ITEMS:
	{
		ItemData* item_data = dynamic_cast<ItemData*>(data.get());
		items[key] = std::unique_ptr<ItemData>(item_data);
		break;
	}
	case DataType::LEARNSETS:
	{
		LearnsetData* learnset_data = dynamic_cast<LearnsetData*>(data.get());
		learnsets[key] = std::unique_ptr<LearnsetData>(learnset_data);
		break;
	}
	case DataType::MOVES:
	{
		MoveData* move_data = dynamic_cast<MoveData*>(data.get());
		moves[key] = std::unique_ptr<MoveData>(move_data);
		break;
	}
	case DataType::NATURES:
	{
		NatureData* nature_data = dynamic_cast<NatureData*>(data.get());
		natures[key] = std::unique_ptr<NatureData>(nature_data);
		break;
	}
	case DataType::POKEDEX:
	{
		SpeciesData* species_data = dynamic_cast<SpeciesData*>(data.get());
		pokedex[key] = std::unique_ptr<SpeciesData>(species_data);
		break;
	}
	case DataType::FORMATS_DATA:
	{
		SpeciesFormatsData* formats_data = dynamic_cast<SpeciesFormatsData*>(data.get());
		this->formats_data[key] = std::unique_ptr<SpeciesFormatsData>(formats_data);
		break;
	}
	case DataType::POKEMON_GO_DATA:
	{
		PokemonGoData* pokemon_go_data = dynamic_cast<PokemonGoData*>(data.get());
		this->pokemon_go_data[key] = std::unique_ptr<PokemonGoData>(pokemon_go_data);
		break;
	}
	case DataType::SCRIPTS:
	{
		AnyObject* any_object_data = dynamic_cast<AnyObject*>(data.get());
		scripts[key] = std::unique_ptr<AnyObject>(any_object_data);
		break;
	}
	case DataType::CONDITIONS:
	{
		ConditionData* condition_data = dynamic_cast<ConditionData*>(data.get());
		conditions[key] = std::unique_ptr<ConditionData>(condition_data);
		break;
	}
	case DataType::TYPE_CHART:
	{
		TypeData* type_data = dynamic_cast<TypeData*>(data.get());
		type_chart[key] = std::unique_ptr<TypeData>(type_data);
		break;
	}
	default:
		throw std::runtime_error("Unknown data type: " + std::to_string(static_cast<int>(data_type)));
	};
}

//void DexTableData::set_data(const std::string& key, IDexData* data)
//{
//	if (!data)
//		throw std::invalid_argument("Data cannot be null");
//
//	set_data(key, std::make_unique<IDexData>(data));
//}

DexTable<std::unique_ptr<AbilityData>>* DexTableData::get_abilities()
{
	return &abilities;
}

DexTable<std::unique_ptr<FormatData>>* DexTableData::get_rulesets()
{
	return &rulesets;
}

DexTable<std::unique_ptr<ItemData>>* DexTableData::get_items()
{
	return &items;
}

DexTable<std::unique_ptr<LearnsetData>>* DexTableData::get_learnsets()
{
	return &learnsets;
}

DexTable<std::unique_ptr<MoveData>>* DexTableData::get_moves()
{
	return &moves;
}

DexTable<std::unique_ptr<NatureData>>* DexTableData::get_natures()
{
	return& natures;
}

DexTable<std::unique_ptr<SpeciesData>>* DexTableData::get_pokedex()
{
	return &pokedex;
}

DexTable<std::unique_ptr<SpeciesFormatsData>>* DexTableData::get_formats_data()
{
	return &formats_data;
}

DexTable<std::unique_ptr<PokemonGoData>>* DexTableData::get_pokemon_go_data()
{
	return &pokemon_go_data;
}

DexTable<std::unique_ptr<AnyObject>>* DexTableData::get_scripts()
{
	return &scripts;
}

DexTable<std::unique_ptr<ConditionData>>* DexTableData::get_conditions()
{
	return &conditions;
}

DexTable<std::unique_ptr<TypeData>>* DexTableData::get_type_chart()
{
	return &type_chart;
}
