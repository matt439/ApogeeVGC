#include "PokemonSources.h"

PokemonSources::PokemonSources(int sources_before, int sources_after)
	: sources_before(sources_before), sources_after(sources_after)
{
}

int PokemonSources::size() const
{
	return static_cast<int>(sources.size());
}

void PokemonSources::add(const PokemonSource& source, ID* limited_egg_move)
{
	// TODO - implement logic to handle limited egg moves
}

void PokemonSources::add_gen(int gen)
{
	// TODO - implement logic to add a generation to sources
}

int PokemonSources::min_source_gen() const
{
	return -1; // TODO - implement logic to find the minimum source generation
}

int PokemonSources::max_source_gen() const
{
	return -1; // TODO - implement logic to find the maximum source generation
}

void PokemonSources::intersect_with(const PokemonSources& other)
{
	// TODO - implement logic to intersect with another PokemonSources instance
}