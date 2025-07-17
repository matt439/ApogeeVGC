#pragma once

#include "TextFile.h"
#include "ConditionTextData.h"
#include "ITextEntry.h"

struct ItemText : public TextFile<ConditionTextData>, public ITextEntry
{
	Descriptions get_descriptions() const override;

	TextEntryType get_type() const override;

	std::unique_ptr<ITextEntry> clone() const override;
};