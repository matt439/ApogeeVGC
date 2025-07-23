#pragma once

#include "TextFile.h"
#include "BasicTextData.h"
#include "ITextEntry.h"

struct PokedexText : public TextFile<BasicTextData>, public ITextEntry
{
	Descriptions get_descriptions() const override;

	TextEntryType get_type() const override;

	std::unique_ptr<ITextEntry> clone() const override;
};
