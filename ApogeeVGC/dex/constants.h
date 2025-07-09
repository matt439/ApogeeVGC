#pragma once

#include <string_view>
#include <filesystem>

constexpr std::string_view BASE_MOD = "gen9";
const std::filesystem::path DATA_DIR = std::filesystem::path(__FILE__).parent_path() / "../data";
const std::filesystem::path MODS_DIR = DATA_DIR / "mods";