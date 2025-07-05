#pragma once

// #include "Battle.h"
// #include "Pokemon.h"
// #include "Effect.h"
#include <string>
#include <functional>

class Battle; // Forward declaration
class Pokemon; // Forward declaration
class Effect; // Forward declaration

struct EffectData
{
	std::string name;
	std::string desc;
	int duration = 0;
	std::function<int(Battle*, Pokemon*, Pokemon*, Effect*)> duration_callback = nullptr;
	std::string effect_type;
	bool infiltrates = false;
	std::string is_nonstandard;
	std::string short_desc;
};