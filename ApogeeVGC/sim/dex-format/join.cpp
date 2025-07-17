#include "join.h"

std::string join(const std::vector<std::string>& array, const std::string& separator)
{
	std::string result;
	if (!array.empty())
	{
		result = array[0];
		for (size_t i = 1; i < array.size(); ++i)
		{
			result += separator + array[i];
		}
	}
	return result;
}