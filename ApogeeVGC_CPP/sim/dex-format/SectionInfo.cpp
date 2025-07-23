#include "SectionInfo.h"

#include <stdexcept>

SectionInfo::SectionInfo(const std::string& section, std::unique_ptr<int> column)
	: section(section), column(std::move(column))
{
}

SectionInfo::SectionInfo(const rapidjson::Value& value)
{
	// check if value is an object
	if (!value.IsObject())
		throw std::runtime_error("SectionInfo must be an object");

	// check if "section" field exists and is a string
	if (value.HasMember("section") && value["section"].IsString())
	{
		section = value["section"].GetString();
	}
	else
	{
		throw std::runtime_error("SectionInfo must have a 'section' string field");
	}

	// check if "column" field exists and is an integer
	if (value.HasMember("column") && value["column"].IsInt())
	{
		column = std::make_unique<int>(value["column"].GetInt());
	}
	else if (value.HasMember("column") && !value["column"].IsNull())
	{
		throw std::runtime_error("SectionInfo 'column' field must be an integer");
	}
	else
	{
		column = nullptr; // optional field, can be null
	}
}

SectionInfo::SectionInfo(const SectionInfo& other) :
	section(other.section),
	column(other.column ? std::make_unique<int>(*other.column) : nullptr)
{
}

FormatListEntryType SectionInfo::get_type() const
{
	return FormatListEntryType::SECTION_INFO;
}

std::unique_ptr<IFormatListEntry> SectionInfo::clone_list_entry() const
{
	return std::make_unique<SectionInfo>(*this);
}