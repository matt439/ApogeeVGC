#pragma once

#include <string>
#include <vector>

/**
* Adds all the elements of an array into a string, separated by the specified separator string.
* @param separator A string used to separate one element of the array from the next in the resulting string.
* If omitted, the array elements are separated with a comma.
*/
std::string join(const std::vector<std::string>& array, const std::string& separator = "");