#pragma once

#include "TypeData.h"
#include <optional>
#include <string>

struct TypesManager
{
    std::optional<TypeData> get(const std::string& type) const;
};