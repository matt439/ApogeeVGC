#pragma once

#include "TextFile.h"
#include "ConditionTextData.h"

struct AbilityText : public TextFile<ConditionTextData>
{
	std::unique_ptr<std::string> activate_from_item = nullptr;
	std::unique_ptr<std::string> activate_no_target = nullptr;
	std::unique_ptr<std::string> copy_boost = nullptr;
	std::unique_ptr<std::string> transform_end = nullptr;

	Descriptions get_descriptions() const;
};