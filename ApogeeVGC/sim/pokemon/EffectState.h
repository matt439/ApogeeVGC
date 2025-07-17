#pragma once

#include <string>
#include <unordered_map>
#include <any>

struct EffectState
{
	std::string id = "";
	int effect_order = 0;
	double* duration = nullptr; // optional
	std::unordered_map<std::string, std::any> extra_properties = {};
};