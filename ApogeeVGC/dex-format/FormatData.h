#pragma once

#include "../dex-conditions/EventMethods.h"
#include "Format.h"

struct FormatData : public Format, public EventMethods
{
	int x = 0;
};