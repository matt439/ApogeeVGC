#pragma once

#include <string>
#include <variant>

struct MoveSlot
{
	std::string id = "";
	std::string move = "";
	int pp = 0;
	int maxpp = 0;
	std::string* target = nullptr; // optional
	std::variant<bool, std::string> disabled = false; // std::variant for union type
	std::string* disabled_source = nullptr; // optional
	bool used = false;
	bool* is_virtual = nullptr; // optional
};