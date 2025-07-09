#pragma once

#include <string>
#include <unordered_map>
#include <any>

using AnyObject = std::unordered_map<std::string, std::any>; // Object with string keys and any type of values