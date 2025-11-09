using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Data;

public record Items
{
    public IReadOnlyDictionary<ItemId, Item> ItemsData { get; }
    private readonly Library _library;

    public Items(Library library)
    {
        _library = library;
        ItemsData = new ReadOnlyDictionary<ItemId, Item>(CreateItems());
    }

    private Dictionary<ItemId, Item> CreateItems()
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
                OnResidualOrder = 5,
                OnResidualSubOrder = 4,
                OnResidual = (battle, pokemon, _, _) =>
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
                },
            },
            [ItemId.ChoiceSpecs] = new()
            {
                Id = ItemId.ChoiceSpecs,
                Name = "Choice Specs",
                SpriteNum = 70,
                Fling = new FlingData { BasePower = 10 },
                OnStart = (battle, pokemon) =>
                {
                    if (pokemon.Volatiles.ContainsKey(ConditionId.ChoiceLock))
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Debug("removing choicelock");
                        }
                        return;
                    }
                    pokemon.RemoveVolatile(_library.Conditions[ConditionId.ChoiceLock]);
                },
                OnModifyMove = (_, _, pokemon, _) =>
                {
                    pokemon.AddVolatile(ConditionId.ChoiceLock);
                },
                OnModifySpAPriority = 1,
                OnModifySpA = (battle, _, _, _, _) => battle.ChainModify(1.5),
                IsChoice = true,
                Num = 297,
                Gen = 4,
            },
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
                OnResidualOrder = 28,
                OnResidualSubOrder = 3,
                OnResidual = (_, pokemon, _, _) =>
                {
                    if (pokemon == null)
                    {
                        return;
                    }
                    pokemon.TrySetStatus(ConditionId.Burn, pokemon);
                },
                Num = 273,
                Gen = 4,
            },
            [ItemId.RockyHelmet] = new()
            {
                Id = ItemId.RockyHelmet,
                Name = "Rocky Helmet",
                SpriteNum = 417,
                Fling = new FlingData { BasePower = 60 },
                OnDamagingHitOrder = 2,
                OnDamagingHit = (battle, _, target, source, move) =>
                {
                    if (battle.CheckMoveMakesContact(move, source, target))
                    {
                        battle.Damage(source.BaseMaxHp / 6, source, target);
                    }
                },
                Num = 540,
                Gen = 5,
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
            [ItemId.AssaultVest] = new()
            {
                Id = ItemId.AssaultVest,
                Name = "Assault Vest",
                SpriteNum = 581,
                Fling = new FlingData { BasePower = 80 },
                OnModifySpDPriority = 1,
                OnModifySpD = (battle, _, _, _, _) => battle.ChainModify(1.5),
                OnDisableMove = (_, pokemon) =>
                {
                    foreach (MoveSlot moveSlot in from moveSlot in pokemon.MoveSlots
                             let move = _library.Moves[moveSlot.Move]
                             where move.Category == MoveCategory.Status && move.Id != MoveId.MeFirst
                             select moveSlot)
                    {
                        pokemon.DisableMove(moveSlot.Id);
                    }
                },
                Num = 640,
                Gen = 6,
            },

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

