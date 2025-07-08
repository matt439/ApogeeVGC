#pragma once

#include "NatureData.h"
#include "../global-types/type_aliases.h"
#include <unordered_map>

using NatureDataTable = std::unordered_map<IDEntry, NatureData>;
