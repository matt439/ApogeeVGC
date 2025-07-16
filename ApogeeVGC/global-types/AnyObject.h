#pragma once

//#include <string>
//#include <unordered_map>
//#include <any>
#include "../dex/IDexData.h"
#include <rapidjson/document.h>

// using AnyObject = std::unordered_map<std::string, std::any>; // Object with string keys and any type of values

//using AnyObject = rapidjson::Value;

struct AnyObject : public IDexData
{
	rapidjson::Value data = rapidjson::Value(rapidjson::kObjectType); // Default to an empty object

	AnyObject() = default;

	DataType get_data_type() const override;
};