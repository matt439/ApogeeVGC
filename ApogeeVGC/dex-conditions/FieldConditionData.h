#pragma once

#include "Condition.h"
#include "FieldEventMethods.h"

struct FieldConditionData : public Condition, public FieldEventMethods
{
	// TODO: try to shadow these 3 functions
	std::optional<OnFieldStartFunc> on_start = std::nullopt;
	std::optional<OnFieldRestartFunc> on_restart = std::nullopt;
	std::optional<OnFieldEndFunc> on_end = std::nullopt;
};