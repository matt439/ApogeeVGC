using ApogeeVGC.Sim.Items;

namespace ApogeeVGC.Data.Items;

public partial record Items
{
    private partial Dictionary<ItemId, Item> CreateItemsMno()
    {
        return new Dictionary<ItemId, Item>
        {
            [ItemId.None] = new()
            {
                Id = ItemId.None,
                Name = "None",
                SpriteNum = 0,
                Num = 0,
                Gen = 0,
            },
        };
    }
}
