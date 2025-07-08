#pragma once

#include "../global-types/Effect.h"
#include <functional>

class Battle;
class Field;
class Pokemon;

using OnFieldStartFunc = std::function<void(Battle*, Field*, Pokemon*, Effect)>;
using OnFieldRestartFunc = std::function<void(Battle*, Field*, Pokemon*, Effect)>;
using OnFieldResidualFunc = std::function<void(Battle*, Field&, Pokemon*, Effect)>;
using OnFieldEndFunc = std::function<void(Battle*, Field*)>;