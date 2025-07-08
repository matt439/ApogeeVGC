#pragma once

#include "../global-types/EffectType.h"
#include "../global-types/NonStandard.h"
#include "../global-types/EffectData.h"
#include "BasicEffectData.h"
#include <string>
#include <optional>

struct BasicEffect : public EffectData
{
    /**
     * ID. This will be a lowercase version of the name with all the
     * non-alphanumeric characters removed. So, for instance, "Mr. Mime"
     * becomes "mrmime", and "Basculin-Blue-Striped" becomes
     * "basculinbluestriped".
     */
    std::string id = "";
    /**
     * Name. Currently does not support Unicode letters, so "Flabébé"
     * is "Flabebe" and "Nidoran♀" is "Nidoran-F".
     */
    std::string name = "";
    /**
     * Full name. Prefixes the name with the effect type. For instance,
     * Leftovers would be "item: Leftovers", confusion the status
     * condition would be "confusion", etc.
     */
    std::string fullname = "";
    /** Effect type. */
    EffectType effect_type = EffectType::CONDITION;
    /**
     * Does it exist? For historical reasons, when you use an accessor
     * for an effect that doesn't exist, you get a dummy effect that
     * doesn't do anything, and this field set to false.
     */
    bool exists = true;
    /**
     * Dex number? For a Pokemon, this is the National Dex number. For
     * other effects, this is often an internal ID (e.g. a move
     * number). Not all effects have numbers, this will be 0 if it
     * doesn't. Nonstandard effects (e.g. CAP effects) will have
     * negative numbers.
     */
    int num = 0;
    /**
     * The generation of Pokemon game this was INTRODUCED (NOT
     * necessarily the current gen being simulated.) Not all effects
     * track generation; this will be 0 if not known.
     */
    int gen = 0;
    /**
     * A shortened form of the description of this effect.
     * Not all effects have this.
     */
    std::string short_desc = "";
    /** The full description for this effect. */
    std::string desc = "";
    /**
     * Is this item/move/ability/pokemon nonstandard? Specified for effects
     * that have no use in standard formats: made-up pokemon (CAP),
     * glitches (MissingNo etc), Pokestar pokemon, etc.
     */
    std::optional<NonStandard> is_nonstandard = std::nullopt;
    /** The duration of the condition - only for pure conditions. */
    std::optional<int> duration = std::nullopt;
    /** Whether or not the condition is ignored by Baton Pass - only for pure conditions. */
    bool no_copy = false;
    /** Whether or not the condition affects fainted Pokemon. */
    bool affects_fainted = false;
    /** Moves only: what status does it set? */
    std::optional<std::string> status = std::nullopt;
    /** Moves only: what weather does it set? */
    std::optional<std::string> weather = std::nullopt;
    /** ??? */
    std::string source_effect = "";

    BasicEffect() = default;
    BasicEffect(const BasicEffect&) = default;
    BasicEffect(const BasicEffectData& data);
    std::string_view to_string() const;
};