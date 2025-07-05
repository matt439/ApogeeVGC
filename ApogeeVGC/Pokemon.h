#pragma once

#include "teams.h"
#include <string>
#include <vector>
#include <unordered_map>
#include <optional>
#include <variant>
#include <any>

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

struct EffectState
{
	std::string id;
	int effect_order;
	std::optional<double> duration;
	std::unordered_map<std::string, std::any> extra_properties;
};

class Pokemon
{
public:
	Pokemon() = default;
private:
	// side
	// battle
	PokemonSet _set;
	std::vector<MoveSlot> _move_slots;
	std::string _hp_type;
	int _hp_power;
	int _position;
	std::string _details;
	//Species _species;
};
