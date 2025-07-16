#include "AbilityText.h"

AbilityText::AbilityText(const AbilityText& other)
	: TextFile<ConditionTextData>(other),
	activate_from_item(other.activate_from_item ? std::make_unique<std::string>(*other.activate_from_item) : nullptr),
	activate_no_target(other.activate_no_target ? std::make_unique<std::string>(*other.activate_no_target) : nullptr),
	copy_boost(other.copy_boost ? std::make_unique<std::string>(*other.copy_boost) : nullptr),
	transform_end(other.transform_end ? std::make_unique<std::string>(*other.transform_end) : nullptr)
{
}


Descriptions AbilityText::get_descriptions() const
{
	std::string desc_str;
	if (data->desc)
		desc_str = *data->desc;
	else
		desc_str = "";

	std::string short_desc_str;
	if (data->short_desc)
		short_desc_str = *data->short_desc;
	else
		short_desc_str = desc_str; // Fallback to desc if short_desc is not available

	return { desc_str, short_desc_str };
}

TextEntryType AbilityText::get_type() const
{
	return TextEntryType::ABILITY;
}

std::unique_ptr<ITextEntry> AbilityText::clone() const
{
	return std::make_unique<AbilityText>(*this);
}

