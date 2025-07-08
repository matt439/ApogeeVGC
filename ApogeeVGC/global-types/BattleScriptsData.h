#pragma once

struct BattleScriptsData
{
	int gen = -1;

	BattleScriptsData() = default;
	BattleScriptsData(const BattleScriptsData&) = default;
	BattleScriptsData(int generation);
};