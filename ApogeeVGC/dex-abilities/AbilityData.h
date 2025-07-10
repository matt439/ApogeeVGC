#pragma once

//#include "../dex-data/BasicEffectData.h"
#include "../dex-conditions/PokemonEventMethods.h"
#include "Ability.h"
#include "AbilityEventMethods.h"
#include <string>
//#include "AbilityFlags.h"

struct AbilityData : public Ability, public AbilityEventMethods, public PokemonEventMethods
{
	std::string name = "";
	
	
	//   bool* suppress_weather = nullptr; // optional
	//AbilityFlags* flags = nullptr; // optional
	//int* rating = nullptr; // optional
	//// TODO: link to ConditionData
	//ConditionData* condition = nullptr; // optional
};