#pragma once

#include <string>

class Trainer
{
public:
	Trainer() = default;
private:
	std::string _name;
	int _elo = 0;
};