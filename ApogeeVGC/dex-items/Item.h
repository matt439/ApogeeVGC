#pragma once

#include "../dex-data/BasicEffect.h"
#include "../dex-conditions/ConditionData.h"
// #include "../global-types/type_aliases.h"
#include "FlingData.h"
#include "ItemInitData.h"
#include <string>
#include <variant>
#include <vector>
#include <functional>
#include <memory>

struct Item : public BasicEffect
{
    // std::string fullname; // already defined in BasicEffect

    // static constexpr EffectType effect_type = EffectType::ITEM;

    //int num = 0; // already defined in BasicEffect

    /**
     * A Move-like object depicting what happens when Fling is used on
     * this item.
     */
	std::unique_ptr<FlingData> fling = nullptr; // optional
    /**
     * If this is a Drive: The type it turns Techno Blast into.
     * undefined, if not a Drive.
     */
    std::unique_ptr<std::string> on_drive = nullptr; // optional
    /**
     * If this is a Memory: The type it turns Multi-Attack into.
     * undefined, if not a Memory.
     */
    std::unique_ptr<std::string> on_memory = nullptr; // optional
    /**
     * If this is a mega stone: The name (e.g. Charizard-Mega-X) of the
     * forme this allows transformation into.
     * undefined, if not a mega stone.
     */
    std::unique_ptr<std::string> mega_stone = nullptr; // optional
    /**
     * If this is a mega stone: The name (e.g. Charizard) of the
     * forme this allows transformation from.
     * undefined, if not a mega stone.
     */
    std::unique_ptr<std::string> mega_evolves = nullptr; // optional
    /**
     * If this is a Z crystal: true if the Z Crystal is generic
     * (e.g. Firium Z). If species-specific, the name
     * (e.g. Inferno Overdrive) of the Z Move this crystal allows
     * the use of.
     * undefined, if not a Z crystal.
     */
    std::unique_ptr<std::variant<bool, std::string>> z_move = nullptr; // optional
    /**
     * If this is a generic Z crystal: The type (e.g. Fire) of the
     * Z Move this crystal allows the use of (e.g. Fire)
     * undefined, if not a generic Z crystal
     */
    std::unique_ptr<std::string> z_move_type = nullptr; // optional
    /**
     * If this is a species-specific Z crystal: The name
     * (e.g. Play Rough) of the move this crystal requires its
     * holder to know to use its Z move.
     * undefined, if not a species-specific Z crystal
     */
    std::unique_ptr<std::string> z_move_from = nullptr; // optional
    /**
     * If this is a species-specific Z crystal: An array of the
     * species of Pokemon that can use this crystal's Z move.
     * Note that these are the full names, e.g. 'Mimikyu-Busted'
     * undefined, if not a species-specific Z crystal
     */
    std::unique_ptr<std::vector<std::string>> item_user = nullptr; // optional
    /** Is this item a Berry? */
    bool is_berry = false;
    /** Whether or not this item ignores the Klutz ability. */
    bool ignore_klutz = false;
    /** The type the holder will change into if it is an Arceus. */
    std::unique_ptr<std::string> on_plate = nullptr; // optional
    /** Is this item a Gem? */
    bool is_gem = false;
    /** Is this item a Pokeball? */
    bool is_pokeball = false;
    /** Is this item a Red or Blue Orb? */
    bool is_primal_orb = false;

    std::unique_ptr<ConditionData> condition = nullptr; // optional
    std::unique_ptr<std::string> forced_forme = nullptr; // optional
    std::unique_ptr<bool> is_choice = nullptr; // optional
    struct NaturalGift
    {
		int base_power = 0;
		std::string type = "";
    };
    std::unique_ptr<NaturalGift> natural_gift = nullptr; // optional
    std::unique_ptr<int> spritenum = nullptr; // optional
    std::unique_ptr<std::variant<SparseBoostsTable, bool>> boosts = nullptr; // optional

    std::unique_ptr<std::variant<bool, std::function<void(Battle*, Pokemon*)>>> on_eat = nullptr; // optional
    std::unique_ptr<std::variant<bool, std::function<void(Battle*, Pokemon*)>>> on_use = nullptr; // optional
    std::unique_ptr<std::function<void(Battle*, Pokemon*)>> on_start = nullptr; // optional
    std::unique_ptr<std::function<void(Battle*, Pokemon*)>> on_end = nullptr; // optional

	Item() = default;
    // Item(const ItemInitData& data);
    Item(
        const std::string& name,
        bool is_berry = false,
        bool ignore_klutz = false,
        bool is_gem = false,
        bool is_pokeball = false,
        bool is_primal_orb = false,

        const std::string& real_move = "",
        const std::string& full_name = "",
        bool* exists = nullptr,
        int num = 0,
        int gen = 0,
        const std::string& short_desc = "",
        const std::string& desc = "",
        NonStandard is_nonstandard = NonStandard::NONE,
        bool no_copy = false,
        bool affects_fainted = false,
        const std::string& source_effect = "",

		// optional
		std::unique_ptr<FlingData> fling = nullptr,
		std::unique_ptr<std::string> on_drive = nullptr,
		std::unique_ptr<std::string> on_memory = nullptr,
		std::unique_ptr<std::string> mega_stone = nullptr,
		std::unique_ptr<std::string> mega_evolves = nullptr,
		std::unique_ptr<std::variant<bool, std::string>> z_move = nullptr,
		std::unique_ptr<std::string> z_move_type = nullptr,
		std::unique_ptr<std::string> z_move_from = nullptr,
		std::unique_ptr<std::vector<std::string>> item_user = nullptr,
		std::unique_ptr<std::string> on_plate = nullptr,
		std::unique_ptr<ConditionData> condition = nullptr,
		std::unique_ptr<std::string> forced_forme = nullptr,
		std::unique_ptr<bool> is_choice = nullptr,
		std::unique_ptr<NaturalGift> natural_gift = nullptr,
		std::unique_ptr<int> spritenum = nullptr,
		std::unique_ptr<std::variant<SparseBoostsTable, bool>> boosts = nullptr,
		std::unique_ptr<std::variant<bool, std::function<void(Battle*, Pokemon*)>>> on_eat = nullptr,
		std::unique_ptr<std::variant<bool, std::function<void(Battle*, Pokemon*)>>> on_use = nullptr,
		std::unique_ptr<std::function<void(Battle*, Pokemon*)>> on_start = nullptr,
		std::unique_ptr<std::function<void(Battle*, Pokemon*)>> on_end = nullptr,

        std::unique_ptr<int> duration = nullptr,
        std::unique_ptr<std::string> status = nullptr,
        std::unique_ptr<std::string> weather = nullptr,
        std::unique_ptr<std::function<int(Battle*, Pokemon*, Pokemon*, Effect*)>> duration_callback = nullptr,
        std::unique_ptr<bool> infiltrates = nullptr);
};