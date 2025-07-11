#pragma once

#include "TextFile.h"
#include "ConditionTextData.h"

struct ItemText : public TextFile<ConditionTextData>
{
	Descriptions get_descriptions() const;
};