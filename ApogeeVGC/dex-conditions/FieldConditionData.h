#pragma once

#include "Condition.h"
#include "FieldEventMethods.h"

struct FieldConditionData : public Condition, public FieldEventMethods
{
	// TODO: try to shadow these 3 functions
	std::optional<OnFieldStartFunc> on_start;
	std::optional<OnFieldRestartFunc> on_restart;
	std::optional<OnFieldEndFunc> on_end;
};