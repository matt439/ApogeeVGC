#pragma once

#include <string>
#include <unordered_map>
#include <any>

struct EffectState
{
    std::string id;
    int effect_order;
    double duration;
    bool has_duration = false;
    std::unordered_map<std::string, std::any> extra_properties;
};