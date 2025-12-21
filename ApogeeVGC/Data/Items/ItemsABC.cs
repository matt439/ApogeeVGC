using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.ItemSpecific;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.Items;

public partial record Items
{
    private partial Dictionary<ItemId, Item> CreateItemsABC()
    {
        return new Dictionary<ItemId, Item>
        {
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
        };
    }
}
