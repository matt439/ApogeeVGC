#pragma once

#include "TypeInfo.h"
#include <memory>

static const std::unique_ptr<TypeInfo> EMPTY_TYPE_INFO = std::make_unique<TypeInfo>(
	ID(""),
	"",
	TypeInfoEffectType::EFFECT_TYPE,
	false
);