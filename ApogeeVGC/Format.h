#pragma once

enum class FormatType
{
	VGC_REG_I,
	OU_GEN_9,
	RANDOM_GEN_9,
};

enum class GameType
{
	SINGLE,
	DOUBLE,
};

#include <vector>

struct Format
{
	Format() = default;
	GameType gameType = GameType::DOUBLE;
	std::vector<unsigned int> allowed_species;
	std::vector<unsigned int> banned_species;
};

const Format VGCRegIFormat =
{
	GameType::DOUBLE,
	{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, // Example species IDs
	{ 11, 12 } // Example banned species IDs
};

const Format OUGen9Format =
{
	GameType::SINGLE,
	{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, // Example species IDs
	{ 11, 12 } // Example banned species IDs
};

const Format RandomGen9Format =
{
	GameType::SINGLE,
	{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, // Example species IDs
	{ 11, 12 } // Example banned species IDs
};

#include <stdexcept>

inline Format get_format(FormatType format_type)
{
	switch (format_type)
	{
	case FormatType::VGC_REG_I:
		return VGCRegIFormat;
	case FormatType::OU_GEN_9:
		return OUGen9Format;
	case FormatType::RANDOM_GEN_9:
		return RandomGen9Format;
	}
	throw std::invalid_argument("Unknown format type");
}