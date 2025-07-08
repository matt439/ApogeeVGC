#pragma once

#include "EventMethods.h"
#include "side_event_methods_aliases.h"
#include <optional>

struct SideEventMethods : public EventMethods
{
	std::optional<OnSideStartFunc> on_side_start;
	std::optional<OnSideRestartFunc> on_side_restart;
	std::optional<OnSideResidualFunc> on_side_residual;
	std::optional<OnSideEndFunc> on_side_end;
	std::optional<int> on_side_residual_order;
	std::optional<int> on_side_residual_priority;
	std::optional<int> on_side_residual_sub_order;
};