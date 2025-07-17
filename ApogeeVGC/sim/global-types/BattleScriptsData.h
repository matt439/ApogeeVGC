#pragma once

struct BattleScriptsData
{
	int gen = 0;

	BattleScriptsData() = default;
	BattleScriptsData(const BattleScriptsData&) = default;
	BattleScriptsData(int generation);
};