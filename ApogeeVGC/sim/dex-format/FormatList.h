#pragma once

//#include "FormatData.h"
//#include "SectionInfo.h"
#include "IFormatListEntry.h"
#include <vector>
#include <memory>

using FormatList = std::vector<std::unique_ptr<IFormatListEntry>>;