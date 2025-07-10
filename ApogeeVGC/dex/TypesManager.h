#pragma once

#include "TypeData.h"
#include <memory>
#include <string>

struct TypesManager
{
    std::unique_ptr<TypeData> get(const std::string& type) const;
};