#pragma once

#include "../dex-abilities/Ability.h"
#include "../dex-items/Item.h"
#include "../dex-moves/ActiveMove.h"
#include "../dex-species/Species.h"
#include "../dex-conditions/Condition.h"
#include "../dex-format/Format.h"
#include <variant>

using Effect = std::variant<
    Ability*,
    Item*,
    ActiveMove*,
    Species*,
    Condition*,
    Format*
>;