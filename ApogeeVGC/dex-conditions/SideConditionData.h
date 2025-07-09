#pragma once

#include "Condition.h"
#include "SideEventMethods.h"

struct SideConditionData : public Condition, public SideEventMethods
{
	// TODO: try to shadow these 3 functions
	std::optional<OnSideStartFunc> on_start = std::nullopt;
	std::optional<OnSideRestartFunc> on_restart = std::nullopt;
	std::optional<OnSideEndFunc> on_end = std::nullopt;
};