#pragma once

#include <unordered_map>

//type GenderName = 'M' | 'F' | 'N' | '';
//type StatIDExceptHP = 'atk' | 'def' | 'spa' | 'spd' | 'spe';
//type StatID = 'hp' | StatIDExceptHP;
//type StatsExceptHPTable = { [stat in StatIDExceptHP] : number };
//type StatsTable = { [stat in StatID] : number };
//type SparseStatsTable = Partial<StatsTable>;
//type BoostID = StatIDExceptHP | 'accuracy' | 'evasion';
//type BoostsTable = { [boost in BoostID] : number };
//type SparseBoostsTable = Partial<BoostsTable>;

enum class GenderName { Male, Female, Neutral, None };
enum class StatIDExceptHP { Atk, Def, SpA, SpD, Spe };
enum class StatID { HP, Atk, Def, SpA, SpD, Spe };
using StatsExceptHPTable = std::unordered_map<StatIDExceptHP, int>;
using StatsTable = std::unordered_map<StatID, int>;
using SparseStatsTable = std::unordered_map<StatID, int>; // same as StatsTable, just allow missing keys
enum class BoostID { Atk, Def, SpA, SpD, Spe, Accuracy, Evasion };
using BoostsTable = std::unordered_map<BoostID, int>;
using SparseBoostsTable = std::unordered_map<BoostID, int>;
enum class NonStandard { PAST, FUTURE, UNOBTAINABLE, CAP, LGPE, CUSTOM, GIGANTAMAX };