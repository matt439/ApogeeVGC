#pragma once

#include "global-types.h"
#include <string>
#include <vector>

struct PokemonSet
{
	/**
	 * Nickname. Should be identical to its base species if not specified
	 * by the player, e.g. "Minior".
	 */
	std::string name;
	/**
	 * Species name (including forme if applicable), e.g. "Minior-Red".
	 * This should always be converted to an id before use.
	 */
	std::string species;
	/**
	 * This can be an id, e.g. "whiteherb" or a full name, e.g. "White Herb".
	 * This should always be converted to an id before use.
	 */
	std::string item;
	/**
	 * This can be an id, e.g. "shieldsdown" or a full name,
	 * e.g. "Shields Down".
	 * This should always be converted to an id before use.
	 */
	std::string ability;
	/**
	 * Each move can be an id, e.g. "shellsmash" or a full name,
	 * e.g. "Shell Smash"
	 * These should always be converted to ids before use.
	 */
	std::vector<std::string> moves;
	/**
	 * This can be an id, e.g. "adamant" or a full name, e.g. "Adamant".
	 * This should always be converted to an id before use.
	 */
	std::string nature;
	std::string gender;
	/**
	 * Effort Values, used in stat calculation.
	 * These must be between 0 and 255, inclusive.
	 *
	 * Also used to store AVs for Let's Go
	 */
	StatsTable evs;
	/**
	 * Individual Values, used in stat calculation.
	 * These must be between 0 and 31, inclusive.
	 *
	 * These are also used as DVs, or determinant values, in Gens
	 * 1 and 2, which are represented as even numbers from 0 to 30.
	 *
	 * In Gen 2-6, these must match the Hidden Power type.
	 *
	 * In Gen 7+, Bottle Caps means these can either match the
	 * Hidden Power type or 31.
	 */
	StatsTable ivs;
	/**
	 * This is usually between 1 and 100, inclusive,
	 * but the simulator supports levels up to 9999 for testing purposes.
	 */
	int level;
	/**
	 * While having no direct competitive effect, certain Pokemon cannot
	 * be legally obtained as shiny, either as a whole or with certain
	 * event-only abilities or moves.
	 */
	bool shiny;
	/**
	 * This is technically "Friendship", but the community calls this
	 * "Happiness".
	 *
	 * It's used to calculate the power of the moves Return and Frustration.
	 * This value must be between 0 and 255, inclusive.
	 */
	int happiness;
	/**
	 * The pokeball this Pokemon is in. Like shininess, this property
	 * has no direct competitive effects, but has implications for
	 * event legality. For example, any Rayquaza that knows V-Create
	 * must be sent out from a Cherish Ball.
	 *
	 * TODO: actually support this in the validator, switching animations,
	 * and the teambuilder.
	 */
	std::string pokeball;
	/**
	 * Hidden Power type. Optional in older gens, but used in Gen 7+
	 * because `ivs` contain post-Battle-Cap values.
	 */
	std::string hp_type;
	/**
	 * Tera Type
	 */
	std::string tera_type;
};
