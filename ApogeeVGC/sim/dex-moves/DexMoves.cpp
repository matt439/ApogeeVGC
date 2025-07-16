#include "DexMoves.h"

#include "../dex/IModdedDex.h"

DexMoves::DexMoves(IModdedDex* dex_ptr)
	: dex(dex_ptr)
{
}

DataType DexMoves::get_data_type() const
{
	return DataType::MOVES;
}

Move* DexMoves::get_move(const std::string& name)
{
	return nullptr; // TODO implement this properly
}

Move* DexMoves::get_move(const Move& move)
{
	return nullptr; // TODO implement this properly
}

Move* DexMoves::get_move_by_id(const ID& id)
{
	return nullptr; // TODO implement this properly
}

std::vector<std::unique_ptr<Move>>* DexMoves::get_all_moves()
{
	return nullptr; // TODO implement this properly
}