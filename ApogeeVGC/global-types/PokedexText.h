#pragma once

#include "TextFile.h"
#include "BasicTextData.h"

struct PokedexText : public TextFile<BasicTextData>
{
	Descriptions get_descriptions() const;
};
