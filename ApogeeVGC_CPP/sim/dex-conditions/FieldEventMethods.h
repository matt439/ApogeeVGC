#pragma once

#include "EventMethods.h"
#include "field_event_methods_aliases.h"

struct FieldEventMethods : public EventMethods
{
	OnFieldStartFunc* on_field_start = nullptr; // optional
	OnFieldRestartFunc* on_field_restart = nullptr; // optional
	OnFieldResidualFunc* on_field_residual = nullptr; // optional
	OnFieldEndFunc* on_field_end = nullptr; // optional
	int* on_field_residual_order = nullptr; // optional
	int* on_field_residual_priority = nullptr; // optional
	int* on_field_residual_sub_order = nullptr; // optional
};