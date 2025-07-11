#pragma once

#include "../battle-queue/BattleQueue.h"

class ModdedBattleQueue : public BattleQueue
{
public:
	int y = 0;
	ModdedBattleQueue() = default;
};