#pragma once

#include <string>
#include <unordered_map>
#include <any>
#include <optional>

struct EffectState
{
	std::string id;
	int effect_order;
	std::optional<double> duration;
	std::unordered_map<std::string, std::any> extra_properties;
};