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