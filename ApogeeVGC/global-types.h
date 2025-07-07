#pragma once

#include <unordered_map>
#include <any>
#include <variant>
#include <string>
#include <functional>
#include <optional>

using ID = std::string; // Lowercase alphanumeric string, can be empty
using IDEntry = std::string; // Lowercase alphanumeric string, can be empty
using PokemonSlot = std::string; // Lowercase alphanumeric string, can be empty, used for slots
using AnyObject = std::unordered_map<std::string, std::any>; // Object with string keys and any type of values

enum class GenderName
{
    MALE,
    FEMALE,
    NEUTRAL,
    NONE,
};

enum class StatIDExceptHP
{
    ATK,
    DEF,
    SPA,
    SPD,
    SPE,
};
enum class StatID
{
    HP,
    ATK,
    DEF,
    SPA,
    SPD,
    SPE,
};
using StatsExceptHPTable = std::unordered_map<StatIDExceptHP, int>;
using StatsTable = std::unordered_map<StatID, int>;
using SparseStatsTable = std::unordered_map<StatID, int>; // same as StatsTable, just allow missing keys
enum class BoostID
{
    ATK,
    DEF,
    SPA,
    SPD,
    SPE,
    ACCURACY,
    EVASION,
};
using BoostsTable = std::unordered_map<BoostID, int>;
using SparseBoostsTable = std::unordered_map<BoostID, int>;
enum class NonStandard
{
    PAST,
    FUTURE,
    UNOBTAINABLE,
    CAP,
    LGPE,
    CUSTOM,
    GIGANTAMAX,
};

enum class SinglesTier
{
    AG,
    UBER,
    UBER_PARENS,
    OU,
    OU_PARENS,
    UUBL,
    UU,
    RUBL,
    RU,
    NUBL,
    NU,
    NU_PARENS,
    PUBL,
    PU,
    PU_PARENS,
    ZUBL,
    ZU,
    NFE,
    LC,
};
enum class DoublesTier
{
    DUBER,
    DUBER_PARENS,
    DOU,
    DOU_PARENS,
    DBL,
    DUU,
    DUU_PARENS,
    NFE,
    LC,
};
enum class OtherTier
{
    UNRELEASED,
    ILLEGAL,
    CAP,
    CAP_NFE,
    CAP_LC,
};

// Forward declarations for Effect types
class Ability;
class Item;
class ActiveMove;
class Species;
class Condition;
class Format;

using Effect = std::variant<
    Ability*,
    Item*,
    ActiveMove*,
    Species*,
    Condition*,
    Format*
>;

// // Forward declarations for Battle and Pokemon
class Battle;
class Pokemon;

// Function type aliases
using ModifierEffectFunc = std::function<std::optional<int>(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using ModifierMoveFunc = std::function<std::optional<int>(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;
using ResultMoveFunc = std::function<std::variant<bool, std::monostate, std::string>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using ExtResultMoveFunc = std::function<std::variant<bool, std::monostate, int, std::string>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using VoidEffectFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using VoidMoveFunc = std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using ModifierSourceEffectFunc = std::function<std::optional<int>(Battle*, int, Pokemon*, Pokemon*, Effect)>;
using ModifierSourceMoveFunc = std::function<std::optional<int>(Battle*, int, Pokemon*, Pokemon*, ActiveMove*)>;
using ResultSourceMoveFunc = std::function<std::variant<bool, std::monostate, std::string>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using ExtResultSourceMoveFunc = std::function<std::variant<bool, std::monostate, int, std::string>(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;
using VoidSourceEffectFunc = std::function<void(Battle*, Pokemon*, Pokemon*, Effect)>;
using VoidSourceMoveFunc = std::function<void(Battle*, Pokemon*, Pokemon*, ActiveMove*)>;

// Handler struct
struct CommonHandlers
{
    ModifierEffectFunc ModifierEffect;
    ModifierMoveFunc ModifierMove;
    std::variant<bool, ResultMoveFunc> ResultMove;
    std::variant<bool, ExtResultMoveFunc> ExtResultMove;
    VoidEffectFunc VoidEffect;
    VoidMoveFunc VoidMove;
    ModifierSourceEffectFunc ModifierSourceEffect;
    ModifierSourceMoveFunc ModifierSourceMove;
    std::variant<bool, ResultSourceMoveFunc> ResultSourceMove;
    std::variant<bool, ExtResultSourceMoveFunc> ExtResultSourceMove;
    VoidSourceEffectFunc VoidSourceEffect;
    VoidSourceMoveFunc VoidSourceMove;
};

struct EffectData
{
    std::optional<std::string> name;
    std::optional<std::string> desc;
    std::optional<int> duration;
    std::optional<std::function<int(Battle*, Pokemon*, Pokemon*, std::optional<Effect>)>> duration_callback;
    std::optional<std::string> effect_type;
    std::optional<bool> infiltrates;
    std::optional<NonStandard> is_nonstandard;
    std::optional<std::string> short_desc;
};

struct ModdedEffectData : public EffectData
{
    bool inherit = false;
};

enum class EffectType {
    CONDITION,
    POKEMON,
    MOVE,
    ITEM,
    ABILITY,
    FORMAT,
    NATURE,
    RULESET,
    WEATHER,
    STATUS,
    TERRAIN,
    RULE,
    VALIDATOR_RULE
};

struct IBasicEffect : public EffectData
{
    ID id;
    EffectType effect_type;
    bool exists;
    std::string fullname;
    int gen = -1;
    std::string source_effect;

    virtual std::string to_string() const {
        return fullname;
    }
};

enum class GameType
{
    SINGLES,
    DOUBLES,
    TRIPLES,
    ROTATION,
    MULTI,
    FREEFORALL
};

enum class SideID
{
    P1,
    P2,
    P3,
    P4
};

#include <vector>

using SpreadMoveTargets = std::vector<std::variant<Pokemon*, bool, std::monostate>>;
using SpreadMoveDamage = std::vector<std::variant<int, bool, std::monostate>>;

struct ZMoveOption
{
    std::string move;
    // MoveTarget target; // Define MoveTarget as needed
};

using ZMoveOptions = std::vector<std::optional<ZMoveOption>>;

struct BattleScriptsData
{
    int gen;
};
