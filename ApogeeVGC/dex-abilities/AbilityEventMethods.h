#pragma once

#include <functional>

class Battle;
class Pokemon;
class Side;
class Field;

struct AbilityEventMethods
{
	std::function<void(Battle*, Pokemon*)> on_check_show = nullptr; // optional
	std::function<void(Battle*, Pokemon*, Side*, Field*)> on_end = nullptr; // optional
	std::function<void(Battle*, Pokemon*)> on_start = nullptr; // optional
};