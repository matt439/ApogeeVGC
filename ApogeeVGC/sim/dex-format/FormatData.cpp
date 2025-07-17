#include "FormatData.h"

#include <stdexcept>

FormatData::FormatData(
    // BasicEffect non-optional
    const std::string& name,
    const std::string& real_move,
    const std::string& full_name,
    //EffectType effect_type,
    bool exists,
    int num,
    int gen,
    const std::string& short_desc,
    const std::string& desc,
    NonStandard is_nonstandard,
    bool no_copy,
    bool affects_fainted,
    const std::string& source_effect,
    // Format-specific
    const std::string& mod,
    FormatEffectType format_effect_type,
    bool debug,
    std::variant<bool, std::string> rated,
    GameType game_type,
    const std::vector<std::string>& ruleset,
    const std::vector<std::string>& base_ruleset,
    const std::vector<std::string>& banlist,
    const std::vector<std::string>& restricted,
    const std::vector<std::string>& unbanlist,
    std::unique_ptr<std::vector<std::string>> custom_rules,
    bool no_log) :

Format(name, real_move, full_name, exists,
    num, gen, short_desc, desc, is_nonstandard, no_copy,
    affects_fainted, source_effect, mod,
    format_effect_type, debug, rated, game_type,
    ruleset, base_ruleset, banlist, restricted, unbanlist,
    custom_rules ? std::make_unique<std::vector<std::string>>(*custom_rules) : nullptr,
    no_log)
    , EventMethods()
{
}

FormatData::FormatData(
    const std::string& name,
    const std::string& mod,
    const std::vector<std::string>& ruleset,
    const std::vector<std::string>& banlist,
    const std::string& real_move,
    const std::string& full_name,
    bool exists,
    int num,
    int gen,
    const std::string& short_desc,
    const std::string& desc,
    NonStandard is_nonstandard,
    bool no_copy,
    bool affects_fainted,
    const std::string& source_effect,
    FormatEffectType format_effect_type,
    bool debug,
    std::variant<bool, std::string> rated,
    GameType game_type,
    const std::vector<std::string>& base_ruleset,
    const std::vector<std::string>& restricted,
    const std::vector<std::string>& unbanlist,
    std::unique_ptr<std::vector<std::string>> custom_rules,
    bool no_log) :
Format(name, real_move, full_name, exists,
	num, gen, short_desc, desc, is_nonstandard, no_copy,
	affects_fainted, source_effect, mod,
	format_effect_type, debug, rated, game_type,
	ruleset, base_ruleset, banlist, restricted, unbanlist,
	custom_rules ? std::make_unique<std::vector<std::string>>(*custom_rules) : nullptr,
	no_log)
	, EventMethods()
{
}

FormatData::FormatData(const rapidjson::Value& value) :
	Format("", "", "", true, 0, 0, "", "", NonStandard::NONE, false, false, "",
		"gen9", FormatEffectType::FORMAT, false, true, GameType::SINGLES,
		{}, {}, {}, {}, {}, nullptr, false),
	EventMethods()
{
    // check if object
	if (!value.IsObject())
		throw std::runtime_error("FormatData must be an object");

	// check if "name" field exists and is a string
	if (value.HasMember("name") && value["name"].IsString())
	{
		name = std::make_unique<std::string>(value["name"].GetString());
	}
	else
	{
		throw std::runtime_error("FormatData must have a 'name' string field");
	}

	// check if "desc" field exists and is a string
	if (value.HasMember("desc") && value["desc"].IsString())
	{
		desc = std::make_unique<std::string>(value["desc"].GetString());
	}

    // check if "mod" field exists and is a string
    if (value.HasMember("mod") && value["mod"].IsString())
    {
        mod = value["mod"].GetString();
    }

	// check if "team" field exists and is a string
	if (value.HasMember("team") && value["team"].IsString())
	{
		team = std::make_unique<std::string>(value["team"].GetString());
	}

	// check if "ruleset" field exists and is an array
	if (value.HasMember("ruleset") && value["ruleset"].IsArray())
	{
		for (const auto& item : value["ruleset"].GetArray())
		{
			if (item.IsString())
			{
				ruleset.push_back(item.GetString());
			}
			else
			{
				throw std::runtime_error("FormatData 'ruleset' field must be an array of strings");
			}
		}
	}

	// check if "banlist" field exists and is an array
	if (value.HasMember("banlist") && value["banlist"].IsArray())
	{
		for (const auto& item : value["banlist"].GetArray())
		{
			if (item.IsString())
			{
				banlist.push_back(item.GetString());
			}
			else
			{
				throw std::runtime_error("FormatData 'banlist' field must be an array of strings");
			}
		}
	}

	// check if "gameType" field exists and is a string
	if (value.HasMember("gameType") && value["gameType"].IsString())
	{
		std::string game_type_str = value["gameType"].GetString();
		if (game_type_str == "singles")
			game_type = GameType::SINGLES;
		else if (game_type_str == "doubles")
			game_type = GameType::DOUBLES;
		else if (game_type_str == "triples")
			game_type = GameType::TRIPLES;
		else if (game_type_str == "multi")
			game_type = GameType::MULTI;
		else if (game_type_str == "freeforall")
			game_type = GameType::FREEFORALL;
		else
			throw std::runtime_error("Invalid game type: " + game_type_str);
	}

	// check if "restricted" field exists and is an array
	if (value.HasMember("restricted") && value["restricted"].IsArray())
	{
		for (const auto& item : value["restricted"].GetArray())
		{
			if (item.IsString())
			{
				restricted.push_back(item.GetString());
			}
			else
			{
				throw std::runtime_error("FormatData 'restricted' field must be an array of strings");
			}
		}
	}
}

FormatData::FormatData(const FormatData& other) :
	Format(other),
	EventMethods(other)
{
	// Copy the mod, ruleset, banlist, restricted, and unbanlist vectors
	mod = other.mod;
	ruleset = other.ruleset;
	banlist = other.banlist;
	restricted = other.restricted;
	unbanlist = other.unbanlist;
	// Copy the custom rules if they exist
	if (other.custom_rules)
	{
		custom_rules = std::make_unique<std::vector<std::string>>(*other.custom_rules);
	}
	else
	{
		custom_rules = nullptr;
	}
}

DataType FormatData::get_data_type() const
{
	return DataType::FORMATS_DATA;
}

std::unique_ptr<IDexData> FormatData::clone() const
{
    return std::make_unique<FormatData>(*this);
}