#pragma once

#include "TextEntryType.h"
#include <memory>

struct ITextEntry
{
	virtual ~ITextEntry() = default;
	virtual TextEntryType get_type() const = 0;
	virtual Descriptions get_descriptions() const = 0;
	virtual std::unique_ptr<ITextEntry> clone() const = 0;
};