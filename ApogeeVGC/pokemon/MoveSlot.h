#pragma once

#include <string>
#include <optional>
#include <variant>

struct MoveSlot
{
	std::string id;
	std::string move;
	int pp;
	int maxpp;
	std::optional<std::string> target;
	std::variant<bool, std::string> disabled; // std::variant for union type
	std::optional<std::string> disabled_source; // std::optional for optional field
	bool used;
	std::optional<bool> is_virtual;
};