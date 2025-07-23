#include "MoveTextData.h"

MoveTextData::MoveTextData(const std::string& desc, const std::string& short_desc) :
	BasicTextData(desc, short_desc)
{
}

MoveTextData::MoveTextData(const MoveTextData& other) :
	BasicTextData(other)
{
	if (other.already_started)
		already_started = std::make_unique<std::string>(*other.already_started);
	if (other.block_self)
		block_self = std::make_unique<std::string>(*other.block_self);
	if (other.clear_boost)
		clear_boost = std::make_unique<std::string>(*other.clear_boost);
	if (other.end_from_item)
		end_from_item = std::make_unique<std::string>(*other.end_from_item);
	if (other.fail)
		fail = std::make_unique<std::string>(*other.fail);
	if (other.fail_select)
		fail_select = std::make_unique<std::string>(*other.fail_select);
	if (other.fail_too_heavy)
		fail_too_heavy = std::make_unique<std::string>(*other.fail_too_heavy);
	if (other.fail_wrong_forme)
		fail_wrong_forme = std::make_unique<std::string>(*other.fail_wrong_forme);
	if (other.mega_no_item)
		mega_no_item = std::make_unique<std::string>(*other.mega_no_item);
	if (other.prepare)
		prepare = std::make_unique<std::string>(*other.prepare);
	if (other.remove_item)
		remove_item = std::make_unique<std::string>(*other.remove_item);
	if (other.start_from_item)
		start_from_item = std::make_unique<std::string>(*other.start_from_item);
	if (other.start_from_z_effect)
		start_from_z_effect = std::make_unique<std::string>(*other.start_from_z_effect);
	if (other.switch_out)
		switch_out = std::make_unique<std::string>(*other.switch_out);
	if (other.take_item)
		take_item = std::make_unique<std::string>(*other.take_item);
	if (other.type_change)
		type_change = std::make_unique<std::string>(*other.type_change);
	if (other.upkeep)
		upkeep = std::make_unique<std::string>(*other.upkeep);
}