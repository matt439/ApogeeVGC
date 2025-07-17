#pragma once

#include "IFormatListEntry.h"
#include <string>
#include <memory>
#include <rapidjson/document.h>

struct SectionInfo : public IFormatListEntry
{
    std::string section = "";
    std::unique_ptr<int> column = nullptr; // optional

	SectionInfo() = default;
	SectionInfo(const std::string& section, std::unique_ptr<int> column = nullptr);
	SectionInfo(const rapidjson::Value& value);
	SectionInfo(const SectionInfo& other);

	FormatListEntryType get_type() const override;

	std::unique_ptr<IFormatListEntry> clone_list_entry() const override;
};