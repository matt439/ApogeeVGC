#pragma once

#include <map>
#include <string>

/**
 * Like Object.assign but only assigns fields missing from self.
 * Facilitates consistent field ordering in constructors.
 * Modifies self in-place.
 */
template<typename ValueType>
static void assign_missing_fields(std::map<std::string, ValueType>& self,
    const std::map<std::string, ValueType>& data);