#pragma once

#include <variant>
#include <string>

// union types for return values
using NumberOrBoolOrNull = std::variant<int, bool, std::monostate>;
using NumberOrBoolOrNullOrVoid = std::variant<int, bool, std::monostate>; // void can be represented by std::monostate
using BoolOrVoid = std::variant<bool, std::monostate>;
using BoolOrNullOrStringOrVoid = std::variant<bool, std::monostate, std::string>;
using BoolOrNullOrNumberOrStringOrVoid = std::variant<bool, std::monostate, int, std::string>;