#pragma once

#include "TextFile.h"
#include "MoveTextData.h"

struct MoveText : public TextFile<MoveTextData>
{
	Descriptions get_descriptions() const;
};
