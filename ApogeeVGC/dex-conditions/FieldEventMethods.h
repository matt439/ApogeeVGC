#pragma once

#include "EventMethods.h"
#include "field_event_methods_aliases.h"

struct FieldEventMethods : public EventMethods
{
	std::optional<OnFieldStartFunc> on_field_start;
	std::optional<OnFieldRestartFunc> on_field_restart;
	std::optional<OnFieldResidualFunc> on_field_residual;
	std::optional<OnFieldEndFunc> on_field_end;
	std::optional<int> on_field_residual_order;
	std::optional<int> on_field_residual_priority;
	std::optional<int> on_field_residual_sub_order;
};