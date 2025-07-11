#pragma once

//#include <string>
//#include <unordered_map>
//#include <any>

#include <rapidjson/document.h>

// using AnyObject = std::unordered_map<std::string, std::any>; // Object with string keys and any type of values

using AnyObject = rapidjson::Value;