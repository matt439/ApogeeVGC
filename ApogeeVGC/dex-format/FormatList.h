#pragma once

#include "FormatData.h"
#include <string>
#include <vector>
#include <variant>
#include <memory>

// Section info struct
struct SectionInfo
{
    std::string section = "";
    std::unique_ptr<int> column = nullptr; // optional
};

// FormatList type: vector of variant
using FormatList = std::vector<std::variant<FormatData, SectionInfo>>;