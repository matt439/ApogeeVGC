#pragma once

// #include "Pokemon.h"
#include <optional>
#include <variant>

struct Attacker {
    // Pokemon* source;
    int damage;
    bool thisTurn;
    std::optional<std::string> move;
    std::string slot; // ID of the PokemonSlot
    std::optional<std::variant<int, bool>> damage_value;
};
