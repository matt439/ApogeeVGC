#pragma once

#include <variant>

class Ability;
class Item;
class ActiveMove;
class Species;
class Condition;
class Format;

using Effect = std::variant<
    Ability*,
    Item*,
    ActiveMove*,
    Species*,
    Condition*,
    Format*
>;