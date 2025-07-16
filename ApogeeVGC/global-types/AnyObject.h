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
	std::unique_ptr<rapidjson::Value> data = nullptr;

	AnyObject() = default;
	AnyObject(const rapidjson::Value& value);
	AnyObject(const AnyObject& other);

	DataType get_data_type() const override;

	std::unique_ptr<IDexData> clone() const override;
};