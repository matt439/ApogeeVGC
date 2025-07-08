#pragma once

#include "Condition.h"
#include "PokemonEventMethods.h"

struct PokemonConditionData : public Condition, public PokemonEventMethods
{
};