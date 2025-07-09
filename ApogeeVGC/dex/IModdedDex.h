#pragma once

#include "DexTableData.h"
#include <optional>
#include <string>

struct IModdedDex
{
	virtual ~IModdedDex() = default;
	virtual std::optional<std::string> get_alias(const std::string& id) = 0;
	virtual std::optional<DexTableData> get_data_cache() = 0;
    virtual DexTableData& data() = 0;
	virtual int get_gen() const = 0;
};
