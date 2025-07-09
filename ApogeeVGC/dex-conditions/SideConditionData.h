#pragma once

#include "Condition.h"
#include "SideEventMethods.h"

struct SideConditionData : public Condition, public SideEventMethods
{
	// TODO: try to shadow these 3 functions
	OnSideStartFunc* on_start = nullptr; // optional
	OnSideRestartFunc* on_restart = nullptr; // optional
	OnSideEndFunc* on_end = nullptr; // optional
};