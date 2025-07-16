#pragma once

//#include "NatureData.h"
#include "../global-types/IDEntry.h"
#include <unordered_map>

struct NatureData;

using NatureDataTable = std::unordered_map<IDEntry, NatureData>;
