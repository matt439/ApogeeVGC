#pragma once

#include "../dex/IDexDataManager.h"
#include "../global-types/ID.h"
#include "Move.h"
#include <unordered_map>
#include <memory>

class IModdedDex;

class DexMoves : public IDexDataManager
{
public:
	IModdedDex* dex = nullptr;
	std::unordered_map<ID, std::unique_ptr<Move>> move_cache = {};
	std::unique_ptr<std::vector<std::unique_ptr<Move>>> all_cache = nullptr; // nullable

	DexMoves(IModdedDex* dex_ptr);
	Move* get_move(const std::string& name);
	Move* get_move(const Move& move);
	Move* get_move_by_id(const ID& id);
	std::vector<std::unique_ptr<Move>>* get_all_moves();

	DataType get_data_type() const override;
};