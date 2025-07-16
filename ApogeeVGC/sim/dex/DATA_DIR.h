#pragma once

#include <filesystem>

const std::filesystem::path DATA_DIR = std::filesystem::path(__FILE__).parent_path() / "../data";