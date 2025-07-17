#pragma once

#include "../dex/Descriptions.h"
#include <string>
#include <memory>

struct BasicTextData
{
	std::unique_ptr<std::string> desc = nullptr;
	std::unique_ptr<std::string> short_desc = nullptr;

	// Descriptions get_descriptions() const;

	BasicTextData() = default;
	BasicTextData(const std::string& desc, const std::string& short_desc);
	BasicTextData(const BasicTextData& other);
};