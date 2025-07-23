#pragma once

#include "../global-types/ID.h"
#include "PokemonSource.h"
#include <vector>
#include <memory>

/**
 * Represents a set of possible ways to get a Pokémon with a given
 * set.
 *
 * `new PokemonSources()` creates an empty set;
 * `new PokemonSources(dex.gen)` allows all Pokemon.
 *
 * The set mainly stored as an Array `sources`, but for sets that
 * could be sourced from anywhere (for instance, TM moves), we
 * instead just set `sourcesBefore` to a number meaning "any
 * source at or before this gen is possible."
 *
 * In other words, this variable represents the set of all
 * sources in `sources`, union all sources at or before
 * gen `sourcesBefore`.
 */
class PokemonSources
{
public:
    /**
     * A set of specific possible PokemonSources; implemented as
     * an Array rather than a Set for perf reasons.
     */
    std::vector<PokemonSource> sources = {};

    // If nonzero: the set also contains all possible sources from this gen and earlier.
    int sources_before = 0;

    // The set requires sources from this gen or later.
    int sources_after = 0;

    // Whether the source is hidden
    std::unique_ptr<bool> is_hidden = nullptr; // nullable

    // Moves that can only be obtained from an egg with another father in gen 2-5.
    // nullptr = definitely not a limited egg move; empty vector = maybe.
    std::unique_ptr<std::vector<ID>> limited_egg_moves = nullptr; // optional

    // Moves that should be in limitedEggMoves but can be learned universally in a past generation.
    std::unique_ptr<std::vector<ID>> possibly_limited_egg_moves = nullptr; // optional

    // Moves that should be in limitedEggMoves but can be learned via Gen 1-2 tradeback.
    std::unique_ptr<std::vector<ID>> tradeback_limited_egg_moves = nullptr; // optional

    // Tracks level up egg moves for female-only Pokemon.
    std::unique_ptr<std::vector<ID>> level_up_egg_moves = nullptr; // optional

    // Moves that can be learned via Pomeg glitch and do not require a particular parent.
    std::unique_ptr<std::vector<ID>> pomeg_egg_moves = nullptr; // optional

    // Event egg source that may be used with the Pomeg glitch.
    // nullptr = definitely not an event egg that can be used with the Pomeg glitch.
    std::unique_ptr<std::string> pomeg_event_egg = nullptr; // optional

    // For event-only Pokemon that do not have a minimum source gen identified by its moves.
    std::unique_ptr<int> event_only_min_source_gen = nullptr; // optional

    // List of movepools, identified by gen and species, which moves can be pulled from.
    std::unique_ptr<std::vector<std::string>> learnset_domain = nullptr; // optional

    // Some Pokemon evolve by having a move in their learnset (like Piloswine with Ancient Power).
    // These can only carry three other moves from their prevo, because the fourth move must be the evo move.
    // This restriction doesn't apply to gen 6+ eggs.
    int move_evo_carry_count = 0;

    // Various other special-case fields
    std::unique_ptr<std::string> baby_only = nullptr; // optional
    std::unique_ptr<std::string> sketch_move = nullptr; // optional
    int dream_world_move_count = 0;
    std::unique_ptr<std::string> hm = nullptr; // optional
    std::unique_ptr<bool> is_from_pokemon_go = nullptr; // optional
    std::unique_ptr<std::string> pokemon_go_source = nullptr; // optional
    std::unique_ptr<std::vector<std::string>> restrictive_moves = nullptr; // optional
    std::unique_ptr<ID> restricted_move = nullptr; // optional

	PokemonSources() = default;
    PokemonSources(int sources_before = 0, int sources_after = 0);

    int size() const;
    void add(const PokemonSource& source, ID* limited_egg_move = nullptr);
    void add_gen(int gen);
	int min_source_gen() const;
	int max_source_gen() const;
	void intersect_with(const PokemonSources& other);
};
