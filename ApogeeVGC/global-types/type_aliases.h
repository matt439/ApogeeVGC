#pragma once

#include "../pokemon/Pokemon.h"
#include "StatID.h"
#include "StatIDExceptHP.h"
#include "BoostID.h"
#include "ZMoveOption.h"

#include <string>
#include <unordered_map>
#include <vector>
#include <any>
#include <optional>
#include <variant>

using ID = std::string; // Lowercase alphanumeric string, can be empty
using IDEntry = std::string; // Lowercase alphanumeric string, can be empty
using PokemonSlot = std::string; // Lowercase alphanumeric string, can be empty, used for slots
using AnyObject = std::unordered_map<std::string, std::any>; // Object with string keys and any type of values

using StatsExceptHPTable = std::unordered_map<StatIDExceptHP, int>;
using StatsTable = std::unordered_map<StatID, int>;
using SparseStatsTable = std::unordered_map<StatID, int>; // same as StatsTable, just allow missing keys

using BoostsTable = std::unordered_map<BoostID, int>;
using SparseBoostsTable = std::unordered_map<BoostID, int>;

using SpreadMoveTargets = std::vector<std::variant<Pokemon*, bool, std::monostate>>;
using SpreadMoveDamage = std::vector<std::variant<int, bool, std::monostate>>;

using ZMoveOptions = std::vector<std::optional<ZMoveOption>>;