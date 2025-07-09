#pragma once

#include "../global-types/Effect.h"
#include "../pokemon/Pokemon.h"
#include "../battle/Battle.h"
#include "../field/Field.h"
#include <functional>

using OnFieldStartFunc = std::function<void(Battle*, Field*, Pokemon*, Effect)>;
using OnFieldRestartFunc = std::function<void(Battle*, Field*, Pokemon*, Effect)>;
using OnFieldResidualFunc = std::function<void(Battle*, Field&, Pokemon*, Effect)>;
using OnFieldEndFunc = std::function<void(Battle*, Field*)>;