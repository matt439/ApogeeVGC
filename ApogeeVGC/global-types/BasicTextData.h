#pragma once

#include <string>
#include <memory>

struct BasicTextData
{
	std::unique_ptr<std::string> desc = nullptr;
	std::unique_ptr<std::string> short_desc = nullptr;
};