#pragma once

#include "dex-conditions.h"
#include "dex-data.h"
#include "IModdedDex.h"
#include <functional>
#include <optional>
#include <string>
#include <vector>
#include <variant>
#include <unordered_map>

struct FlingData
{
	int base_power;
	std::optional<std::string> status;
	std::optional<std::string> volatile_status;
	std::optional<ResultMoveFunc> effect;
};

struct Item; // Forward declaration

struct ItemData : public Item, public PokemonEventMethods
{
	std::string name;
};

struct ModdedItemData : public PokemonEventMethods
{
	bool inherit = true;
	std::optional<std::function<void(Battle*, Pokemon*)>> on_custap;
};

using ItemDataTable = std::unordered_map<std::string, ItemData>;
using ModdedItemDataTable = std::unordered_map<std::string, ModdedItemData>;

struct ItemData : public BasicEffectData
{
    std::optional<FlingData> fling;
    std::optional<std::string> on_drive;
    std::optional<std::string> on_memory;
    std::optional<std::string> mega_stone;
    std::optional<std::string> mega_evolves;
    std::optional<std::variant<bool, std::string>> z_move;
    std::optional<std::string> z_move_type;
    std::optional<std::string> z_move_from;
    std::optional<std::vector<std::string>> item_user;
    std::optional<bool> is_berry;
    std::optional<bool> ignore_klutz;
    std::optional<std::string> on_plate;
    std::optional<bool> is_gem;
    std::optional<bool> is_pokeball;
    std::optional<bool> is_primal_orb;
};

struct Item : public BasicEffect
{
	// std::string fullname; // already defined in BasicEffect
    
	// static constexpr EffectType effect_type = EffectType::ITEM;

	//int num = 0; // already defined in BasicEffect

    /**
     * A Move-like object depicting what happens when Fling is used on
     * this item.
     */
    std::optional<FlingData> fling;
    /**
     * If this is a Drive: The type it turns Techno Blast into.
     * undefined, if not a Drive.
     */
    std::optional<std::string> on_drive;
    /**
     * If this is a Memory: The type it turns Multi-Attack into.
     * undefined, if not a Memory.
     */
    std::optional<std::string> on_memory;
    /**
     * If this is a mega stone: The name (e.g. Charizard-Mega-X) of the
     * forme this allows transformation into.
     * undefined, if not a mega stone.
     */
    std::optional<std::string> mega_stone;
    /**
     * If this is a mega stone: The name (e.g. Charizard) of the
     * forme this allows transformation from.
     * undefined, if not a mega stone.
     */
    std::optional<std::string> mega_evolves;
    /**
     * If this is a Z crystal: true if the Z Crystal is generic
     * (e.g. Firium Z). If species-specific, the name
     * (e.g. Inferno Overdrive) of the Z Move this crystal allows
     * the use of.
     * undefined, if not a Z crystal.
     */
    std::optional<std::variant<bool, std::string>> z_move;
    /**
     * If this is a generic Z crystal: The type (e.g. Fire) of the
     * Z Move this crystal allows the use of (e.g. Fire)
     * undefined, if not a generic Z crystal
     */
    std::optional<std::string> z_move_type;
    /**
     * If this is a species-specific Z crystal: The name
     * (e.g. Play Rough) of the move this crystal requires its
     * holder to know to use its Z move.
     * undefined, if not a species-specific Z crystal
     */
    std::optional<std::string> z_move_from;
    /**
     * If this is a species-specific Z crystal: An array of the
     * species of Pokemon that can use this crystal's Z move.
     * Note that these are the full names, e.g. 'Mimikyu-Busted'
     * undefined, if not a species-specific Z crystal
     */
    std::optional<std::vector<std::string>> item_user;
    /** Is this item a Berry? */
    bool is_berry = false;
    /** Whether or not this item ignores the Klutz ability. */
    bool ignore_klutz = false;
    /** The type the holder will change into if it is an Arceus. */
    std::optional<std::string> on_plate;
    /** Is this item a Gem? */
    bool is_gem = false;
    /** Is this item a Pokeball? */
    bool is_pokeball = false;
    /** Is this item a Red or Blue Orb? */
    bool is_primal_orb = false;

    std::optional<ConditionData> condition;
    std::optional<std::string> forced_forme;
    std::optional<bool> is_choice;
    struct NaturalGift
    {
        int base_power;
        std::string type;
    };
    std::optional<NaturalGift> natural_gift;
    std::optional<int> spritenum;
    std::optional<std::variant<SparseBoostsTable, bool>> boosts;

    std::optional<std::variant<bool, std::function<void(Battle*, Pokemon*)>>> on_eat;
    std::optional<std::variant<bool, std::function<void(Battle*, Pokemon*)>>> on_use;
    std::optional<std::function<void(Battle*, Pokemon*)>> on_start;
    std::optional<std::function<void(Battle*, Pokemon*)>> on_end;

	Item(const ItemData& data) :
        BasicEffect(data),
        fling(data.fling),
        on_drive(data.on_drive),
        on_memory(data.on_memory),
        mega_stone(data.mega_stone),
        mega_evolves(data.mega_evolves),
        z_move(data.z_move),
        z_move_type(data.z_move_type),
        z_move_from(data.z_move_from),
        item_user(data.item_user),
        is_berry(data.is_berry.value_or(false)),
        ignore_klutz(data.ignore_klutz.value_or(false)),
        on_plate(data.on_plate),
        is_gem(data.is_gem.value_or(false)),
        is_pokeball(data.is_pokeball.value_or(false)),
        is_primal_orb(data.is_primal_orb.value_or(false))
    {
        fullname = "item: " + data.name;
		effect_type = EffectType::ITEM;
        
        // Set gen based on num if not already set
        if (gen == 0)
        {
            if (num >= 1124) gen = 9;
            else if (num >= 927) gen = 8;
            else if (num >= 689) gen = 7;
            else if (num >= 577) gen = 6;
            else if (num >= 537) gen = 5;
            else if (num >= 377) gen = 4;
            else gen = 3;
            // Gen 2 items must be specified manually
        }

        // Set fling based on item type
        if (is_berry)
        {
            fling = FlingData{ 10, std::nullopt, std::nullopt, std::nullopt };
        }
        if (!id.empty() && id.size() >= 5 && id.substr(id.size() - 5) == "plate")
        {
            fling = FlingData{ 90, std::nullopt, std::nullopt, std::nullopt };
        }
        if (on_drive)
        {
            fling = FlingData{ 70, std::nullopt, std::nullopt, std::nullopt };
        }
        if (mega_stone)
        {
            fling = FlingData{ 80, std::nullopt, std::nullopt, std::nullopt };
        }
        if (on_memory)
        {
            fling = FlingData{ 50, std::nullopt, std::nullopt, std::nullopt };
        }
    }
};

struct DexItems
{
    IModdedDex* dex;
    std::unordered_map<std::string, Item> item_cache;
    std::optional<std::vector<Item>> all_cache;

    explicit DexItems(IModdedDex* dex_ptr);

    // Get by Item (returns reference)
    const Item& get(const Item& item) const;

    // Get by name (string)
    const Item& get(const std::string& name);

    // Get by ID
    const Item& get_by_id(const std::string& id);

    // Get all items
    const std::vector<Item>& all();
};
