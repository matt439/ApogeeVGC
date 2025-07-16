#pragma once

#include "TextFile.h"
#include "MoveTextData.h"
#include "ITextEntry.h"

struct MoveText : public TextFile<MoveTextData>, public ITextEntry
{
	Descriptions get_descriptions() const override;

	TextEntryType get_type() const override;

	std::unique_ptr<ITextEntry> clone() const override;
};
