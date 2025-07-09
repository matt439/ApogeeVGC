#pragma once

#include "../global-types/ID.h"
#include "../global-types/Effect.h"
#include "MutableMove.h"
#include "MoveHitData.h"
#include <string>
#include <variant>
#include <vector>

class ActiveMove : public MutableMove
{
public:
	std::string name = "";
	std::string effectType = "Move";
    ID id = ID();
	int num = 0;
    ID* weather = nullptr; // optional
    ID* status = nullptr; // optional
	int hit = 0;
    MoveHitData* moveHitData = nullptr; // optional
    std::vector<Pokemon>* hitTargets = nullptr; // optional
    Ability* ability = nullptr; // optional
    std::vector<Pokemon>* allies = nullptr; // optional
    Pokemon* auraBooster = nullptr; // optional
    bool* causedCrashDamage = nullptr; // optional
    ID* forceStatus = nullptr; // optional
    bool* hasAuraBreak = nullptr; // optional
    bool* hasBounced = nullptr; // optional
    bool* hasSheerForce = nullptr; // optional
    bool* isExternal = nullptr; // optional
    bool* lastHit = nullptr; // optional
    int* magnitude = nullptr; // optional
    bool* pranksterBoosted = nullptr; // optional
    bool* selfDropped = nullptr; // optional
    std::variant<std::string, bool>* selfSwitch = nullptr; // optional
    bool* spreadHit = nullptr; // optional
    std::string* statusRoll = nullptr; // optional
    bool* stellarBoosted = nullptr; // optional
    std::variant<int, bool>* totalDamage = nullptr; // optional
    Effect* typeChangerBoosted = nullptr; // optional
    bool* willChangeForme = nullptr; // optional
    bool* infiltrates = nullptr; // optional
    Pokemon* ruinedAtk = nullptr; // optional
    Pokemon* ruinedDef = nullptr; // optional
    Pokemon* ruinedSpA = nullptr; // optional
    Pokemon* ruinedSpD = nullptr; // optional
    bool* isZOrMaxPowered = nullptr; // optional
};
