#pragma once

#include <string>
#include <optional>
#include <variant>

struct MoveSlot
{
	std::string id = "";
	std::string move = "";
	int pp = 0;
	int maxpp = 0;
	std::optional<std::string> target = std::nullopt;
	std::variant<bool, std::string> disabled = false; // std::variant for union type
	std::optional<std::string> disabled_source = std::nullopt; // std::optional for optional field
	bool used = false;
	std::optional<bool> is_virtual = std::nullopt;
};