#pragma once

#include "../global-types/type_aliases.h"
#include <optional>

struct ZMoveData
{
    std::optional<int> base_power;
    std::optional<IDEntry> effect;
    std::optional<SparseBoostsTable> boost;
};