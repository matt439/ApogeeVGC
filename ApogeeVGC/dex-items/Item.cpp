#include "Item.h"

Item::Item(const ItemInitData& data) :
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