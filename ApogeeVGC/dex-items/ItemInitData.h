#pragma once

#include "../dex-data/BasicEffectData.h"
#include "FlingData.h"

struct ItemInitData : public BasicEffectData
{
	ItemInitData() = default;
	ItemInitData(const BasicEffectData& basic_effect_data);
    ItemInitData(const ItemInitData&) = default;
    ItemInitData(const std::string& name, bool exists);

	std::optional<FlingData> fling = std::nullopt;
	std::optional<std::string> on_drive = std::nullopt;
	std::optional<std::string> on_memory = std::nullopt;
	std::optional<std::string> mega_stone = std::nullopt;
	std::optional<std::string> mega_evolves = std::nullopt;
	std::optional<std::variant<bool, std::string>> z_move = std::nullopt;
	std::optional<std::string> z_move_type = std::nullopt;
	std::optional<std::string> z_move_from = std::nullopt;
	std::optional<std::vector<std::string>> item_user = std::nullopt;
	std::optional<bool> is_berry = std::nullopt;
	std::optional<bool> ignore_klutz = std::nullopt;
	std::optional<std::string> on_plate = std::nullopt;
	std::optional<bool> is_gem = std::nullopt;
	std::optional<bool> is_pokeball = std::nullopt;
	std::optional<bool> is_primal_orb = std::nullopt;
};