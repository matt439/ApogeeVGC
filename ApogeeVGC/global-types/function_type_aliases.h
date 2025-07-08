#pragma once

#include "Effect.h"
#include <functional>
#include <optional>
#include <string>
#include <variant>

class Battle;
class Pokemon;

// Function type aliases
using ModifierEffectFunc = std::function<std::optional<int>(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using ModifierMoveFunc = std::function<std::optional<int>(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;
using ResultMoveFunc = std::function<std::variant<bool, std::monostate, std::string>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using ExtResultMoveFunc = std::function<std::variant<bool, std::monostate, int, std::string>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using VoidEffectFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using VoidMoveFunc = std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using ModifierSourceEffectFunc = std::function<std::optional<int>(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using ModifierSourceMoveFunc = std::function<std::optional<int>(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;
using ResultSourceMoveFunc = std::function<std::variant<bool, std::monostate, std::string>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using ExtResultSourceMoveFunc = std::function<std::variant<bool, std::monostate, int, std::string>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using VoidSourceEffectFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using VoidSourceMoveFunc = std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;