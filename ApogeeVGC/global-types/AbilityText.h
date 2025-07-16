#pragma once

#include "TextFile.h"
#include "ConditionTextData.h"
#include "ITextEntry.h"

struct AbilityText : public TextFile<ConditionTextData>, public ITextEntry
{
	std::unique_ptr<std::string> activate_from_item = nullptr;
	std::unique_ptr<std::string> activate_no_target = nullptr;
	std::unique_ptr<std::string> copy_boost = nullptr;
	std::unique_ptr<std::string> transform_end = nullptr;

	AbilityText() = default;
	AbilityText(const AbilityText& other);

	Descriptions get_descriptions() const override;

	TextEntryType get_type() const override;

	std::unique_ptr<ITextEntry> clone() const override;


};