#pragma once

//#include "../dex-data/BasicEffectData.h"
//#include <string>
//#include <vector>
//#include <variant>
//// #include "FlingData.h"
//
//struct FlingData; // forward declaration
//
//struct ItemInitData : public BasicEffectData
//{
//	ItemInitData() = default;
//	ItemInitData(const BasicEffectData& basic_effect_data);
//    ItemInitData(const ItemInitData&) = default;
//    ItemInitData(const std::string& name, bool exists);
//
//	FlingData* fling = nullptr; // optional
//	std::string* on_drive = nullptr; // optional
//	std::string* on_memory = nullptr; // optional
//	std::string* mega_stone = nullptr; // optional
//	std::string* mega_evolves = nullptr; // optional
//	std::variant<bool, std::string>* z_move = nullptr; // optional
//	std::string* z_move_type = nullptr; // optional
//	std::string* z_move_from = nullptr; // optional
//	std::vector<std::string>* item_user = nullptr; // optional
//	bool* is_berry = nullptr; // optional
//	bool* ignore_klutz = nullptr; // optional
//	std::string* on_plate = nullptr; // optional
//	bool* is_gem = nullptr; // optional
//	bool* is_pokeball = nullptr; // optional
//	bool* is_primal_orb = nullptr; // optional
//};