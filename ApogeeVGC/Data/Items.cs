using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.ItemSpecific;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

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
            [ItemId.ChoiceSpecs] = new()
            {
                Id = ItemId.ChoiceSpecs,
                Name = "Choice Specs",
                SpriteNum = 70,
                Fling = new FlingData { BasePower = 10 },
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    // Remove any existing choice lock when this Pokemon enters battle
                    // This allows switching to reset the choice lock
                    if (pokemon.Volatiles.ContainsKey(ConditionId.ChoiceLock))
                    {
                        battle.Debug("ChoiceSpecs: Removing existing choicelock on switch-in");

                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.ChoiceLock]);
                    }
                }),
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, pokemon, _) =>
                {
                    pokemon.AddVolatile(ConditionId.ChoiceLock);

                    // Set the locked move immediately after adding the volatile
                    if (pokemon.Volatiles.ContainsKey(ConditionId.ChoiceLock) &&
                        pokemon.Volatiles[ConditionId.ChoiceLock].Move == null)
                    {
                        battle.Debug(
                            $"[ChoiceSpecs.OnModifyMove] {pokemon.Name}: Setting locked move to {move.Id}");

                        pokemon.Volatiles[ConditionId.ChoiceLock].Move = move.Id;
                    }
                }),
                //OnModifySpAPriority = 1,
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, _, _, _) =>
                {
                    battle.ChainModify(1.5);
                    return battle.FinalModify(spa);
                }, 1),
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
                //OnModifySpDPriority = 1,
                OnModifySpD = new OnModifySpDEventInfo((battle, spd, _, _, _) =>
                {
                    battle.ChainModify(1.5);
                    return battle.FinalModify(spd);
                }, 1),
                OnDisableMove = new OnDisableMoveEventInfo((_, pokemon) =>
                {
                    foreach (MoveSlot moveSlot in from moveSlot in pokemon.MoveSlots
                             let move = _library.Moves[moveSlot.Move]
                             where move.Category == MoveCategory.Status && move.Id != MoveId.MeFirst
                             select moveSlot)
                    {
                        pokemon.DisableMove(moveSlot.Id);
                    }
                }),
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