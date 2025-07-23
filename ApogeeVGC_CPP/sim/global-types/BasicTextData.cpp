#include "BasicTextData.h"

BasicTextData::BasicTextData(const std::string& desc, const std::string& short_desc)
	: desc(std::make_unique<std::string>(desc)),
	short_desc(std::make_unique<std::string>(short_desc))
{
}

BasicTextData::BasicTextData(const BasicTextData& other)
	: desc(std::make_unique<std::string>(*other.desc)),
	short_desc(std::make_unique<std::string>(*other.short_desc))
{
}