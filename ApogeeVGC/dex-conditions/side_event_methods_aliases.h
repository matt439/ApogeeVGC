#pragma once

#include "../global-types/Effect.h"
#include <functional>

class Battle;
class Side;
class Pokemon;

using OnSideStartFunc = std::function<void(Battle*, Side*, Pokemon*, Effect)>;
using OnSideRestartFunc = std::function<void(Battle*, Side*, Pokemon*, Effect)>;
using OnSideResidualFunc = std::function<void(Battle*, Side&, Pokemon*, Effect)>;
using OnSideEndFunc = std::function<void(Battle*, Side*)>;