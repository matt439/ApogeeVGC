#pragma once

#include <string>
#include <unordered_map>
#include <any>
#include <optional>

struct EffectState
{
	std::string id = "";
	int effect_order = 0;
	std::optional<double> duration = std::nullopt;
	std::unordered_map<std::string, std::any> extra_properties = {};
};