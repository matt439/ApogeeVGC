using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Items;

namespace ApogeeVGC.Data.Items;

public partial record Items
{
    private partial Dictionary<ItemId, Item> CreateItemsJKL()
    {
        return new Dictionary<ItemId, Item>
        {
            [ItemId.Leftovers] = new()
            {
                Id = ItemId.Leftovers,
                Name = "Leftovers",
                Num = 234,
                Fling = new FlingData { BasePower = 10 },
                SpriteNum = 242,
                Gen = 2,
                //OnResidualOrder = 5,
                //OnResidualSubOrder = 4,
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    if (pokemon == null)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Debug("Leftovers OnResidual: pokemon is null");
                        }

                        return;
                    }

                    battle.Heal(pokemon.BaseMaxHp / 16);
                })
                {
                    Order = 5,
                    SubOrder = 4,
                },
            },
            [ItemId.LightClay] = new()
            {
                Id = ItemId.LightClay,
                Name = "Light Clay",
                SpriteNum = 252,
                Fling = new FlingData { BasePower = 30 },
                Num = 269,
                Gen = 4,
            },
        };
    }
}
