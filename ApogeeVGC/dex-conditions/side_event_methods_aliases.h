#pragma once

#include "../global-types/Effect.h"
#include "../battle/Battle.h"
#include "../side/Side.h"
#include "../pokemon/Pokemon.h"
#include <functional>

using OnSideStartFunc = std::function<void(Battle*, Side*, Pokemon*, Effect)>;
using OnSideRestartFunc = std::function<void(Battle*, Side*, Pokemon*, Effect)>;
using OnSideResidualFunc = std::function<void(Battle*, Side&, Pokemon*, Effect)>;
using OnSideEndFunc = std::function<void(Battle*, Side*)>;