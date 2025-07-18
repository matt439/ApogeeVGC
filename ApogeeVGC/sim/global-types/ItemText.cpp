#include "ItemText.h"

Descriptions ItemText::get_descriptions() const
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

TextEntryType ItemText::get_type() const
{
	return TextEntryType::ITEM;
}

std::unique_ptr<ITextEntry> ItemText::clone() const
{
	return std::make_unique<ItemText>(*this);
}