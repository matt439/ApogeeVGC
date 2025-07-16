#include "to_lower.h"

std::string to_lower(const std::string& input)
{
	std::string result = input;
	for (char& c : result) {
		c = std::tolower(c);
	}
	return result;
}