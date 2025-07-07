#pragma once

#include "global-types.h"
//#include "dex.h"
#include "IModdedDex.h"
#include <string>
#include <algorithm>
#include <cctype>
#include <optional>

// Helper: Remove non-alphanumeric and convert to lowercase
std::string to_id(const std::string& text)
{
    std::string result;
    for (char c : text)
    {
        if (std::isalnum(static_cast<unsigned char>(c)))
        {
            result += std::tolower(static_cast<unsigned char>(c));
        }
    }
    return result;
}

// Overload for int
std::string to_id(int num)
{
    return to_id(std::to_string(num));
}

// Example struct with id fields
struct HasID
{
    std::optional<std::string> id;
    std::optional<std::string> userid;
    std::optional<std::string> roomid;
};

/**
* Converts anything to an ID. An ID must have only lowercase alphanumeric
* characters.
*
* If a string is passed, it will be converted to lowercase and
* non-alphanumeric characters will be stripped.
*
* If an object with an ID is passed, its ID will be returned.
* Otherwise, an empty string will be returned.
*
* Generally assigned to the global toID, because of how
* commonly it's used.
*/
std::string to_id(const HasID& obj)
{
    if (obj.id) return to_id(*obj.id);
    if (obj.userid) return to_id(*obj.userid);
    if (obj.roomid) return to_id(*obj.roomid);
    return "";
}

#include <map>

/**
 * Like Object.assign but only assigns fields missing from self.
 * Facilitates consistent field ordering in constructors.
 * Modifies self in-place.
 */
template<typename ValueType>
void assign_missing_fields(std::map<std::string, ValueType>& self, const std::map<std::string, ValueType>& data)
{
    for (const auto& [k, v] : data)
    {
        if (self.find(k) != self.end()) continue;
        self[k] = v;
    }
}

struct BasicEffectData
{
    std::string name;
    std::optional<std::string> real_move;
    std::optional<std::string> fullname;
    std::optional<EffectType> effect_type;
    std::optional<bool> exists;
    std::optional<int> num;
    std::optional<int> gen;
    std::optional<std::string> short_desc;
    std::optional<std::string> desc;
    std::optional<NonStandard> is_nonstandard;
    std::optional<int> duration;
    std::optional<bool> no_copy;
    std::optional<bool> affects_fainted;
    std::optional<std::string> status;
    std::optional<std::string> weather;
    std::optional<std::string> source_effect;

    BasicEffectData(const std::string& name, bool exists);
};

struct BasicEffect : public EffectData
{
    /**
     * ID. This will be a lowercase version of the name with all the
     * non-alphanumeric characters removed. So, for instance, "Mr. Mime"
     * becomes "mrmime", and "Basculin-Blue-Striped" becomes
     * "basculinbluestriped".
     */
    std::string id;
    /**
     * Name. Currently does not support Unicode letters, so "Flabébé"
     * is "Flabebe" and "Nidoran♀" is "Nidoran-F".
     */
    std::string name;
    /**
     * Full name. Prefixes the name with the effect type. For instance,
     * Leftovers would be "item: Leftovers", confusion the status
     * condition would be "confusion", etc.
     */
    std::string fullname;
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
    std::string short_desc;
    /** The full description for this effect. */
    std::string desc;
    /**
     * Is this item/move/ability/pokemon nonstandard? Specified for effects
     * that have no use in standard formats: made-up pokemon (CAP),
     * glitches (MissingNo etc), Pokestar pokemon, etc.
     */
    std::optional<NonStandard> is_nonstandard;
    /** The duration of the condition - only for pure conditions. */
    std::optional<int> duration;
    /** Whether or not the condition is ignored by Baton Pass - only for pure conditions. */
    bool no_copy = false;
    /** Whether or not the condition affects fainted Pokemon. */
    bool affects_fainted = false;
    /** Moves only: what status does it set? */
    std::optional<std::string> status;
    /** Moves only: what weather does it set? */
    std::optional<std::string> weather;
    /** ??? */
    std::string source_effect;

    BasicEffect(const BasicEffectData& data)
    {
        name = data.name;
        id = data.real_move ? to_id(*data.real_move) : to_id(name);
        fullname = data.fullname.value_or(name);
        effect_type = data.effect_type.value_or(EffectType::CONDITION);
        exists = data.exists.value_or(!id.empty());
        num = data.num.value_or(0);
        gen = data.gen.value_or(0);
        short_desc = data.short_desc.value_or("");
        desc = data.desc.value_or("");
        is_nonstandard = data.is_nonstandard;
        duration = data.duration;
        no_copy = data.no_copy.value_or(false);
        affects_fainted = data.affects_fainted.value_or(false);
        status = data.status;
        weather = data.weather;
        source_effect = data.source_effect.value_or("");
    }

	std::string_view to_string() const
	{
		return this->name;
	}

};

struct NatureData : public BasicEffectData
{
	// std::string name; // BasicEffectData already has name
    std::optional<StatIDExceptHP> plus;
    std::optional<StatIDExceptHP> minus;

    NatureData(const std::string& name, bool exists);
};

struct Nature : public BasicEffect
{
    std::optional<StatIDExceptHP> plus;
    std::optional<StatIDExceptHP> minus;

    Nature(const NatureData& data) :
        BasicEffect(data),
        plus(data.plus),
        minus(data.minus)
    {
        fullname = "nature: " + name;
        effect_type = EffectType::NATURE;
        gen = 3;
    }
};

using NatureDataTable = std::unordered_map<IDEntry, NatureData>;

#include <unordered_map>
#include <vector>

struct DexNatures
{
    IModdedDex* dex;
    std::unordered_map<std::string, Nature> nature_cache;
    std::optional<std::vector<Nature>> all_cache;

    explicit DexNatures(IModdedDex* dex_ptr);

    // Get by Nature (returns reference)
    const Nature& get(const Nature& nature) const;

    // Get by name (string)
    const Nature& get(const std::string& name);

    // Get by ID
    const Nature& get_by_id(const std::string& id);

    // Get all natures
    const std::vector<Nature>& all();
};
