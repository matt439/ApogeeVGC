#pragma once

#include "EventMethods.h"
#include "side_event_methods_aliases.h"

struct SideEventMethods : public EventMethods
{
	OnSideStartFunc* on_side_start = nullptr; // optional
	OnSideRestartFunc* on_side_restart = nullptr; // optional
	OnSideResidualFunc* on_side_residual = nullptr; // optional
	OnSideEndFunc* on_side_end = nullptr; // optional
	int* on_side_residual_order = nullptr; // optional
	int* on_side_residual_priority = nullptr; // optional
	int* on_side_residual_sub_order = nullptr; // optional
};