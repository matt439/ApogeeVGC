#pragma once

#include "EffectData.h"
#include "MoveEventMethods.h"
#include "HitEffect.h"

struct MoveData : public EffectData, MoveEventMethods, HitEffect
{
public:
	MoveData() = default;
};