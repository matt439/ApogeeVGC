#pragma once

#include "PokemonConditionData.h"
#include "SideConditionData.h"
#include "FieldConditionData.h"
#include <variant>

using ConditionData = std::variant<PokemonConditionData, SideConditionData, FieldConditionData>;
