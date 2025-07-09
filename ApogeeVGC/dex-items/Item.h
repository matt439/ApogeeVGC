#pragma once

#include "../dex-data/BasicEffect.h"
#include "../dex-conditions/ConditionData.h"
#include "../global-types/type_aliases.h"
#include "FlingData.h"
#include "ItemInitData.h"

struct Item : public BasicEffect
{
    // std::string fullname; // already defined in BasicEffect

    // static constexpr EffectType effect_type = EffectType::ITEM;

    //int num = 0; // already defined in BasicEffect

    /**
     * A Move-like object depicting what happens when Fling is used on
     * this item.
     */
	std::optional<FlingData> fling = std::nullopt;
    /**
     * If this is a Drive: The type it turns Techno Blast into.
     * undefined, if not a Drive.
     */
	std::optional<std::string> on_drive = std::nullopt;
    /**
     * If this is a Memory: The type it turns Multi-Attack into.
     * undefined, if not a Memory.
     */
	std::optional<std::string> on_memory = std::nullopt;
    /**
     * If this is a mega stone: The name (e.g. Charizard-Mega-X) of the
     * forme this allows transformation into.
     * undefined, if not a mega stone.
     */
	std::optional<std::string> mega_stone = std::nullopt;
    /**
     * If this is a mega stone: The name (e.g. Charizard) of the
     * forme this allows transformation from.
     * undefined, if not a mega stone.
     */
	std::optional<std::string> mega_evolves = std::nullopt;
    /**
     * If this is a Z crystal: true if the Z Crystal is generic
     * (e.g. Firium Z). If species-specific, the name
     * (e.g. Inferno Overdrive) of the Z Move this crystal allows
     * the use of.
     * undefined, if not a Z crystal.
     */
	std::optional<std::variant<bool, std::string>> z_move = std::nullopt;
    /**
     * If this is a generic Z crystal: The type (e.g. Fire) of the
     * Z Move this crystal allows the use of (e.g. Fire)
     * undefined, if not a generic Z crystal
     */
	std::optional<std::string> z_move_type = std::nullopt;
    /**
     * If this is a species-specific Z crystal: The name
     * (e.g. Play Rough) of the move this crystal requires its
     * holder to know to use its Z move.
     * undefined, if not a species-specific Z crystal
     */
	std::optional<std::string> z_move_from = std::nullopt;
    /**
     * If this is a species-specific Z crystal: An array of the
     * species of Pokemon that can use this crystal's Z move.
     * Note that these are the full names, e.g. 'Mimikyu-Busted'
     * undefined, if not a species-specific Z crystal
     */
	std::optional<std::vector<std::string>> item_user = std::nullopt;
    /** Is this item a Berry? */
    bool is_berry = false;
    /** Whether or not this item ignores the Klutz ability. */
    bool ignore_klutz = false;
    /** The type the holder will change into if it is an Arceus. */
	std::optional<std::string> on_plate = std::nullopt;
    /** Is this item a Gem? */
    bool is_gem = false;
    /** Is this item a Pokeball? */
    bool is_pokeball = false;
    /** Is this item a Red or Blue Orb? */
    bool is_primal_orb = false;

	std::optional<ConditionData> condition = std::nullopt;
	std::optional<std::string> forced_forme = std::nullopt;
	std::optional<bool> is_choice = std::nullopt;
    struct NaturalGift
    {
		int base_power = 0;
		std::string type = "";
    };
	std::optional<NaturalGift> natural_gift = std::nullopt;
	std::optional<int> spritenum = std::nullopt;
	std::optional<std::variant<SparseBoostsTable, bool>> boosts = std::nullopt;

	std::optional<std::variant<bool, std::function<void(Battle*, Pokemon*)>>> on_eat = std::nullopt;
	std::optional<std::variant<bool, std::function<void(Battle*, Pokemon*)>>> on_use = std::nullopt;
	std::optional<std::function<void(Battle*, Pokemon*)>> on_start = std::nullopt;
	std::optional<std::function<void(Battle*, Pokemon*)>> on_end = std::nullopt;

    Item(const ItemInitData& data);
};