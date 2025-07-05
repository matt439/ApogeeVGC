#pragma once

#include "EffectData.h"

struct BasicEffect : public EffectData
{
	std::string id;
	std::string fullname;
	bool exists;
	int num;
	int gen;
	bool no_copy;
	bool affets_fainted;
	std::string status;
	std::string weather;
	std::string source_effect;
};