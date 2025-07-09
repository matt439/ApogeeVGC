#pragma once

#include <unordered_map>
#include <string>

// helper structs for ModdedDex
struct TypeData
{
    std::unordered_map<std::string, int> damage_taken;
};