#pragma once

#include <vector>
#include <variant>

// Forward declaration of Pokemon class
class Pokemon;

using SpreadMoveTargets = std::vector<std::variant<Pokemon*, bool, std::monostate>>;