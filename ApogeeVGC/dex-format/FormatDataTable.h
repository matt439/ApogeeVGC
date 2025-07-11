#pragma once

#include "../global-types/IDEntry.h"
#include "FormatData.h"
#include <unordered_map>

using FormatDataTable = std::unordered_map<IDEntry, FormatData>;
