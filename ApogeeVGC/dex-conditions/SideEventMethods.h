#pragma once

#include "EventMethods.h"
#include "side_event_methods_aliases.h"
#include <optional>

struct SideEventMethods : public EventMethods
{
	std::optional<OnSideStartFunc> on_side_start = std::nullopt;
	std::optional<OnSideRestartFunc> on_side_restart = std::nullopt;
	std::optional<OnSideResidualFunc> on_side_residual = std::nullopt;
	std::optional<OnSideEndFunc> on_side_end = std::nullopt;
	std::optional<int> on_side_residual_order = std::nullopt;
	std::optional<int> on_side_residual_priority = std::nullopt;
	std::optional<int> on_side_residual_sub_order = std::nullopt;
};