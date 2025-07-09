#pragma once

#include "StatID.h"
#include <unordered_map>

// same as StatsTable, just allow missing keys
using SparseStatsTable = std::unordered_map<StatID, int>; 