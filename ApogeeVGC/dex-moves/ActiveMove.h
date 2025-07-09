#pragma once

#include "MutableMove.h"
#include "MoveHitData.h"
#include <string>
#include <optional>
#include <variant>
#include <vector>

class ActiveMove : public MutableMove
{
public:
	std::string name = "";
	std::string effectType = "Move";
    ID id = ID();
	int num = 0;
    std::optional<ID> weather = std::nullopt;
    std::optional<ID> status = std::nullopt;
	int hit = 0;
    std::optional<MoveHitData> moveHitData = std::nullopt;
    std::optional<std::vector<Pokemon>> hitTargets = std::nullopt;
    std::optional<Ability> ability = std::nullopt;
    std::optional<std::vector<Pokemon>> allies = std::nullopt;
    std::optional<Pokemon> auraBooster = std::nullopt;
    std::optional<bool> causedCrashDamage = std::nullopt;
    std::optional<ID> forceStatus = std::nullopt;
    std::optional<bool> hasAuraBreak = std::nullopt;
    std::optional<bool> hasBounced = std::nullopt;
    std::optional<bool> hasSheerForce = std::nullopt;
    std::optional<bool> isExternal = std::nullopt;
    std::optional<bool> lastHit = std::nullopt;
    std::optional<int> magnitude = std::nullopt;
    std::optional<bool> pranksterBoosted = std::nullopt;
    std::optional<bool> selfDropped = std::nullopt;
    std::optional<std::variant<std::string, bool>> selfSwitch = std::nullopt;
    std::optional<bool> spreadHit = std::nullopt;
    std::optional<std::string> statusRoll = std::nullopt;
    std::optional<bool> stellarBoosted = std::nullopt;
    std::optional<std::variant<int, bool>> totalDamage = std::nullopt;
    std::optional<Effect> typeChangerBoosted = std::nullopt;
    std::optional<bool> willChangeForme = std::nullopt;
    std::optional<bool> infiltrates = std::nullopt;
    std::optional<Pokemon> ruinedAtk = std::nullopt;
    std::optional<Pokemon> ruinedDef = std::nullopt;
    std::optional<Pokemon> ruinedSpA = std::nullopt;
    std::optional<Pokemon> ruinedSpD = std::nullopt;
    std::optional<bool> isZOrMaxPowered = std::nullopt;
};
