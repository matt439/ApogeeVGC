#pragma once

#include <string>
#include <memory>
#include <rapidjson/document.h>

struct SectionInfo
{
    std::string section = "";
    std::unique_ptr<int> column = nullptr; // optional

	SectionInfo() = default;
	SectionInfo(const std::string& section, std::unique_ptr<int> column = nullptr);
	SectionInfo(const rapidjson::Value& value);
};