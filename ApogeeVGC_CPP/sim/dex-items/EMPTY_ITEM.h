#pragma once

#include "Item.h"
#include <memory>

// Provide a default empty item
static const std::unique_ptr<Item> EMPTY_ITEM =
	std::make_unique<Item>("empty");


// static const Item EMPTY_ITEM = Item("empty");