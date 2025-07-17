#pragma once

#include "../sim/dex/IDexData.h"
#include <rapidjson/document.h>

rapidjson::Value deep_clone(const rapidjson::Value& value, rapidjson::Document::AllocatorType& allocator);