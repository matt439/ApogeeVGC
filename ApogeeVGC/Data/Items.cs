using System.Collections.ObjectModel;
using ApogeeVGC.Sim;

namespace ApogeeVGC.Data;

public record Items
{
    public IReadOnlyDictionary<ItemId, Item> ItemsData { get; }

    public Items()
    {
        ItemsData = new ReadOnlyDictionary<ItemId, Item>(_items);
    }

    private readonly Dictionary<ItemId, Item> _items = new()
    {
        [ItemId.Leftovers] = new Item
        {
            Id = ItemId.Leftovers,
            Name = "Leftovers",
            Num = 234,
            Fling = new FlingData { BasePower = 10 },
            SpriteNum = 242,
            Gen = 2,
            OnResidualOrder = 5,
            OnResidualSubOrder = 4,
        },
        [ItemId.ChoiceSpecs] = new Item
        {
            Id = ItemId.ChoiceSpecs,
            Name = "Choice Specs",
            SpriteNum = 70,
            Fling = new FlingData { BasePower = 10 },
            Num = 297,
            Gen = 4,
        },
        [ItemId.FlameOrb] = new Item
        {
            Id = ItemId.FlameOrb,
            Name = "Flame Orb",
            SpriteNum = 145,
            Fling = new FlingData
            {
                BasePower = 30,
                Status = ConditionId.Burn,
            },
            Num = 273,
            Gen = 4,
        },
        [ItemId.RockyHelmet] = new Item
        {
            Id = ItemId.RockyHelmet,
            Name = "Rocky Helmet",
            SpriteNum = 417,
            Fling = new FlingData { BasePower = 60 },
            Num = 540,
            Gen = 5,
        },
        [ItemId.LightClay] = new Item
        {
            Id = ItemId.LightClay,
            Name = "Light Clay",
            SpriteNum = 252,
            Fling = new FlingData { BasePower = 30 },
            Num = 269,
            Gen = 4,
        },
        [ItemId.AssaultVest] = new Item
        {
            Id = ItemId.AssaultVest,
            Name = "Assault Vest",
            SpriteNum = 581,
            Fling = new FlingData { BasePower = 80 },
            Num = 640,
            Gen = 6,
        },
    };
}

