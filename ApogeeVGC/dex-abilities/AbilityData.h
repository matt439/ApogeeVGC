#pragma once

#include "../dex-data/BasicEffectData.h"
#include "AbilityFlags.h"

struct AbilityData : public BasicEffectData
{
    bool* suppress_weather = nullptr; // optional
	AbilityFlags* flags = nullptr; // optional
	int* rating = nullptr; // optional
	// TODO: link to ConditionData
	ConditionData* condition = nullptr; // optional
};