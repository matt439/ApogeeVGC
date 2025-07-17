#pragma once

#include "FormatData.h"
#include "SectionInfo.h"
#include <string>
#include <vector>
#include <variant>
#include <memory>

// FormatList type: vector of variant
using FormatList = std::vector<std::variant<std::unique_ptr<FormatData>, std::unique_ptr<SectionInfo>>>;