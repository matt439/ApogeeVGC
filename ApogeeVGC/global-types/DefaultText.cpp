#include "DefaultText.h"

Descriptions DefaultText::get_descriptions() const
{
	std::string desc_str;
	if (text.data.HasMember("desc") && text.data["desc"].IsString())
		desc_str = text.data["desc"].GetString();
	else
		desc_str = ""; // Default to empty if desc is not available

	std::string short_desc_str;
	if (text.data.HasMember("shortDesc") && text.data["shortDesc"].IsString())
		short_desc_str = text.data["shortDesc"].GetString();
	else
		short_desc_str = desc_str; // Fallback to desc if shortDesc is not available

	return { desc_str, short_desc_str };
}