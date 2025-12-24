using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Items;

namespace ApogeeVGC.Data.Items;

public partial record Items
{
    private partial Dictionary<ItemId, Item> CreateItemsDef()
    {
        return new Dictionary<ItemId, Item>
        {
            [ItemId.FlameOrb] = new()
            {
                Id = ItemId.FlameOrb,
                Name = "Flame Orb",
                SpriteNum = 145,
                Fling = new FlingData
                {
                    BasePower = 30,
                    Status = ConditionId.Burn,
                },
                //OnResidualOrder = 28,
                //OnResidualSubOrder = 3,
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    battle.Debug($"FlameOrb OnResidual: Called for {pokemon?.Name ?? "null pokemon"}");

                    if (pokemon == null)
                    {
                        battle.Debug("FlameOrb OnResidual: pokemon is null, returning");
                        return;
                    }

                    battle.Debug($"FlameOrb OnResidual: Calling TrySetStatus for {pokemon.Name}");

                    pokemon.TrySetStatus(ConditionId.Burn, pokemon);
                })
                {
                    Order = 28,
                    SubOrder = 3,
                },
                Num = 273,
                Gen = 4,
            },
        };
    }
}
