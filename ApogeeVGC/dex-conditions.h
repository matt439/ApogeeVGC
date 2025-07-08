#pragma once

#include "global-types.h"
#include "dex-moves.h"
#include "field.h"
#include "dex-data.h"
#include <functional>
#include <optional>
#include <variant>
#include <string>
#include <vector>


struct PokemonConditionData : public Condition, public PokemonEventMethods
{
};

struct SideConditionData : public Condition, public SideEventMethods
{
	// TODO: try to shadow these 3 functions
	std::optional<OnSideStartFunc> on_start;
	std::optional<OnSideRestartFunc> on_restart;
	std::optional<OnSideEndFunc> on_end;
};

struct FieldConditionData : public Condition, public FieldEventMethods
{
	// TODO: try to shadow these 3 functions
	std::optional<OnSideStartFunc> on_start;
	std::optional<OnSideRestartFunc> on_restart;
	std::optional<OnSideEndFunc> on_end;
};

using ConditionData = std::variant<PokemonConditionData, SideConditionData, FieldConditionData>;

class DexConditions
{
	int x;
};