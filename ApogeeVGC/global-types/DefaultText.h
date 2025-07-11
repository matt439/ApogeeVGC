#pragma once

#include "../dex/Descriptions.h"
#include "AnyObject.h"

struct DefaultText
{
	AnyObject data = {};
	Descriptions get_descriptions() const;
};
