#pragma once

#include "BasicTextData.h"

struct ConditionTextData : public BasicTextData
{
	std::unique_ptr<std::string> activate = nullptr;
	std::unique_ptr<std::string> add_item = nullptr;
	std::unique_ptr<std::string> block = nullptr;
	std::unique_ptr<std::string> boost = nullptr;
	std::unique_ptr<std::string> cant = nullptr;
	std::unique_ptr<std::string> change_ability = nullptr;
	std::unique_ptr<std::string> damage = nullptr;
	std::unique_ptr<std::string> end = nullptr;
	std::unique_ptr<std::string> heal = nullptr;
	std::unique_ptr<std::string> move = nullptr;
	std::unique_ptr<std::string> start = nullptr;
	std::unique_ptr<std::string> transform = nullptr;

	ConditionTextData() = default;
	ConditionTextData(const std::string& desc, const std::string& short_desc);
	ConditionTextData(const ConditionTextData& other);
};