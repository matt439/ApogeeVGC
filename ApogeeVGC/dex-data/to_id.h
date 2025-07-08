#pragma once

#include "HasID.h"
#include <string>

// Helper: Remove non-alphanumeric and convert to lowercase
static std::string to_id(const std::string& text);

// Overload for int
static std::string to_id(int num);

/**
* Converts anything to an ID. An ID must have only lowercase alphanumeric
* characters.
*
* If a string is passed, it will be converted to lowercase and
* non-alphanumeric characters will be stripped.
*
* If an object with an ID is passed, its ID will be returned.
* Otherwise, an empty string will be returned.
*
* Generally assigned to the global toID, because of how
* commonly it's used.
*/
static std::string to_id(const HasID& obj);