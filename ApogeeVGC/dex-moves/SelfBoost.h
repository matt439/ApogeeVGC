#pragma once

#include "../global-types/type_aliases.h"
#include <optional>

struct SelfBoost
{
    std::optional<SparseBoostsTable> boosts;
};