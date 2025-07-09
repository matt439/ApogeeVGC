#pragma once

#include "EventMethods.h"
#include "field_event_methods_aliases.h"

struct FieldEventMethods : public EventMethods
{
	std::optional<OnFieldStartFunc> on_field_start = std::nullopt;
	std::optional<OnFieldRestartFunc> on_field_restart = std::nullopt;
	std::optional<OnFieldResidualFunc> on_field_residual = std::nullopt;
	std::optional<OnFieldEndFunc> on_field_end = std::nullopt;
	std::optional<int> on_field_residual_order = std::nullopt;
	std::optional<int> on_field_residual_priority = std::nullopt;
	std::optional<int> on_field_residual_sub_order = std::nullopt;
};