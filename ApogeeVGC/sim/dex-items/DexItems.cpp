#include "DexItems.h"

#include "../dex/IDex.h"
#include "../dex-data/to_id.h"  
#include "../dex/DexTableData.h"
#include "ItemData.h"
#include "Item.h"
#include "EMPTY_ITEM.h"

DexItems::DexItems(IModdedDex* dex_ptr)  
   : dex(dex_ptr)
{
}  

Item* DexItems::get_item(const Item& item) const
{  
	return nullptr; // TODO implement this properly
}  

Item* DexItems::get_item(const std::string& name)
{  
	return nullptr; // TODO implement this properly
}

Item* DexItems::get_item_by_id(const ID& id)
{
	return nullptr; // TODO implement this properly
}

std::vector<std::unique_ptr<Item>>* DexItems::get_all_items()
{  
	return nullptr; // TODO implement this properly
}

DataType DexItems::get_data_type() const
{
	return DataType::ITEMS;
}