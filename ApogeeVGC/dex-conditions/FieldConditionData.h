#pragma once

#include "Condition.h"
#include "FieldEventMethods.h"

struct FieldConditionData : public Condition, public FieldEventMethods
{
	// TODO: try to shadow these 3 functions
	OnFieldStartFunc* on_start = nullptr; // optional
	OnFieldRestartFunc* on_restart = nullptr; // optional
	OnFieldEndFunc* on_end = nullptr; // optional
};