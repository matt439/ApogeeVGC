using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Items;

namespace ApogeeVGC.Data.Items;

public partial record Items
{
    private partial Dictionary<ItemId, Item> CreateItemsPqr()
    {
        return new Dictionary<ItemId, Item>
        {
            [ItemId.RockyHelmet] = new()
            {
                Id = ItemId.RockyHelmet,
                Name = "Rocky Helmet",
                SpriteNum = 417,
                Fling = new FlingData { BasePower = 60 },
                //OnDamagingHitOrder = 2,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, source, move) =>
                {
                    if (battle.CheckMoveMakesContact(move, source, target))
                    {
                        battle.Damage(source.BaseMaxHp / 6, source, target);
                    }
                })
                {
                    Order = 2,
                },
                Num = 540,
                Gen = 5,
            },
        };
    }
}
