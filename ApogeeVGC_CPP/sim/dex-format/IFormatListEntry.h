#pragma once

#include "FormatListEntryType.h"
#include <memory>

struct IFormatListEntry
{
	virtual ~IFormatListEntry() = default;
	virtual FormatListEntryType get_type() const = 0;
	virtual std::unique_ptr<IFormatListEntry> clone_list_entry() const = 0;
};