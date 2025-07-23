#pragma once

#include "../dex/Descriptions.h"
#include "AnyObject.h"
#include "ITextEntry.h"

struct DefaultText : public ITextEntry
{
	AnyObject text = {};

	DefaultText() = default;
	DefaultText(const AnyObject& text_data);
	DefaultText(const DefaultText& other);

	Descriptions get_descriptions() const override;

	TextEntryType get_type() const override;

	std::unique_ptr<ITextEntry> clone() const override;
};
