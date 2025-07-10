#include "Item.h"

//Item::Item(const ItemInitData& data) :
//    BasicEffect(data),
//    fling(data.fling),
//    on_drive(data.on_drive),
//    on_memory(data.on_memory),
//    mega_stone(data.mega_stone),
//    mega_evolves(data.mega_evolves),
//    z_move(data.z_move),
//    z_move_type(data.z_move_type),
//    z_move_from(data.z_move_from),
//    item_user(data.item_user),
//    is_berry(data.is_berry.value_or(false)),
//    ignore_klutz(data.ignore_klutz.value_or(false)),
//    on_plate(data.on_plate),
//    is_gem(data.is_gem.value_or(false)),
//    is_pokeball(data.is_pokeball.value_or(false)),
//    is_primal_orb(data.is_primal_orb.value_or(false))
//{
//    fullname = "item: " + data.name;
//    effect_type = EffectType::ITEM;
//
//    // Set gen based on num if not already set
//    if (gen == 0)
//    {
//        if (num >= 1124) gen = 9;
//        else if (num >= 927) gen = 8;
//        else if (num >= 689) gen = 7;
//        else if (num >= 577) gen = 6;
//        else if (num >= 537) gen = 5;
//        else if (num >= 377) gen = 4;
//        else gen = 3;
//        // Gen 2 items must be specified manually
//    }
//
//    // Set fling based on item type
//    if (is_berry)
//    {
//        fling = FlingData{ 10, std::nullopt, std::nullopt, std::nullopt };
//    }
//    if (!id.empty() && id.size() >= 5 && id.substr(id.size() - 5) == "plate")
//    {
//        fling = FlingData{ 90, std::nullopt, std::nullopt, std::nullopt };
//    }
//    if (on_drive)
//    {
//        fling = FlingData{ 70, std::nullopt, std::nullopt, std::nullopt };
//    }
//    if (mega_stone)
//    {
//        fling = FlingData{ 80, std::nullopt, std::nullopt, std::nullopt };
//    }
//    if (on_memory)
//    {
//        fling = FlingData{ 50, std::nullopt, std::nullopt, std::nullopt };
//    }
//}

Item::Item(
    const std::string& name,
    bool is_berry,
    bool ignore_klutz,
    bool is_gem,
    bool is_pokeball,
    bool is_primal_orb,

    const std::string& real_move,
    const std::string& full_name,
    bool* exists,
    int num,
    int gen,
    const std::string& short_desc,
    const std::string& desc,
    NonStandard is_nonstandard,
    bool no_copy,
    bool affects_fainted,
    const std::string& source_effect,

    std::unique_ptr<FlingData> fling,
    std::unique_ptr<std::string> on_drive,
    std::unique_ptr<std::string> on_memory,
    std::unique_ptr<std::string> mega_stone,
    std::unique_ptr<std::string> mega_evolves,
    std::unique_ptr<std::variant<bool, std::string>> z_move,
    std::unique_ptr<std::string> z_move_type,
    std::unique_ptr<std::string> z_move_from,
    std::unique_ptr<std::vector<std::string>> item_user,
    std::unique_ptr<std::string> on_plate,
    std::unique_ptr<ConditionData> condition,
    std::unique_ptr<std::string> forced_forme,
    std::unique_ptr<bool> is_choice,
    std::unique_ptr<NaturalGift> natural_gift,
    std::unique_ptr<int> spritenum,
    std::unique_ptr<std::variant<SparseBoostsTable, bool>> boosts,
    std::unique_ptr<std::variant<bool, std::function<void(Battle*, Pokemon*)>>> on_eat,
    std::unique_ptr<std::variant<bool, std::function<void(Battle*, Pokemon*)>>> on_use,
    std::unique_ptr<std::function<void(Battle*, Pokemon*)>> on_start,
    std::unique_ptr<std::function<void(Battle*, Pokemon*)>> on_end,
    
    std::unique_ptr<int> duration,
    std::unique_ptr<std::string> status,
    std::unique_ptr<std::string> weather,
    std::unique_ptr<std::function<int(Battle*, Pokemon*, Pokemon*, Effect*)>> duration_callback,
    std::unique_ptr<bool> infiltrates) :
BasicEffect(
    name,
    real_move,
    full_name,
    EffectType::ITEM,
    exists,
    num,
    gen,
    short_desc,
    desc,
    std::move(is_nonstandard),
    no_copy,
    affects_fainted,
    source_effect,
	std::move(duration),
    std::move(status),
    std::move(weather),
    std::move(duration_callback),
    std::move(infiltrates)),

    fling(std::move(fling)),
    on_drive(std::move(on_drive)),
    on_memory(std::move(on_memory)),
    mega_stone(std::move(mega_stone)),
    mega_evolves(std::move(mega_evolves)),
    z_move(std::move(z_move)),
    z_move_type(std::move(z_move_type)),
    z_move_from(std::move(z_move_from)),
    item_user(std::move(item_user)),
    on_plate(std::move(on_plate)),
    condition(std::move(condition)),
    forced_forme(std::move(forced_forme)),
    is_choice(std::move(is_choice)),
    natural_gift(std::move(natural_gift)),
    spritenum(std::move(spritenum)),
    boosts(std::move(boosts)),
    on_eat(std::move(on_eat)),
    on_use(std::move(on_use)),
    on_start(std::move(on_start)),
    on_end(std::move(on_end))
{

}