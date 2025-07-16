#include "ConditionTextData.h"

ConditionTextData::ConditionTextData(const std::string& desc, const std::string& short_desc)
	: BasicTextData(desc, short_desc)
{
}

ConditionTextData::ConditionTextData(const ConditionTextData& other) :
	BasicTextData(other)
{
	if (other.activate)
		activate = std::make_unique<std::string>(*other.activate);
	if (other.add_item)
		add_item = std::make_unique<std::string>(*other.add_item);
	if (other.block)
		block = std::make_unique<std::string>(*other.block);
	if (other.boost)
		boost = std::make_unique<std::string>(*other.boost);
	if (other.cant)
		cant = std::make_unique<std::string>(*other.cant);
	if (other.change_ability)
		change_ability = std::make_unique<std::string>(*other.change_ability);
	if (other.damage)
		damage = std::make_unique<std::string>(*other.damage);
	if (other.end)
		end = std::make_unique<std::string>(*other.end);
	if (other.heal)
		heal = std::make_unique<std::string>(*other.heal);
	if (other.move)
		move = std::make_unique<std::string>(*other.move);
	if (other.start)
		start = std::make_unique<std::string>(*other.start);
	if (other.transform)
		transform = std::make_unique<std::string>(*other.transform);
}