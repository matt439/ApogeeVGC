#pragma once

#include "../dex/Descriptions.h"
#include "AnyObject.h"

struct DefaultText
{
	AnyObject text = {};
	Descriptions get_descriptions() const;
};
