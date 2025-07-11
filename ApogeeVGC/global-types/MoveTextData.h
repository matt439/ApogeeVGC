#pragma once

#include "BasicTextData.h"

struct MoveTextData : public BasicTextData
{
	std::unique_ptr<std::string> already_started = nullptr;
	std::unique_ptr<std::string> block_self = nullptr;
	std::unique_ptr<std::string> clear_boost = nullptr;
	std::unique_ptr<std::string> end_from_item = nullptr;
	std::unique_ptr<std::string> fail = nullptr;
	std::unique_ptr<std::string> fail_select = nullptr;
	std::unique_ptr<std::string> fail_too_heavy = nullptr;
	std::unique_ptr<std::string> fail_wrong_forme = nullptr;
	std::unique_ptr<std::string> mega_no_item = nullptr;
	std::unique_ptr<std::string> prepare = nullptr;
	std::unique_ptr<std::string> remove_item = nullptr;
	std::unique_ptr<std::string> start_from_item = nullptr;
	std::unique_ptr<std::string> start_from_z_effect = nullptr;
	std::unique_ptr<std::string> switch_out = nullptr;
	std::unique_ptr<std::string> take_item = nullptr;
	std::unique_ptr<std::string> type_change = nullptr;
	std::unique_ptr<std::string> upkeep = nullptr;
};