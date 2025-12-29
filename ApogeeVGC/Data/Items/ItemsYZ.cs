using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Items;

public partial record Items
{
    private partial Dictionary<ItemId, Item> CreateItemsYz()
    {
        return new Dictionary<ItemId, Item>
        {
            // Y items
            [ItemId.YacheBerry] = new()
            {
                Id = ItemId.YacheBerry,
                Name = "Yache Berry",
                SpriteNum = 567,
                IsBerry = true,
                NaturalGift = (80, "Ice"),
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, source, target, move) =>
                    {
                        if (move.Type == MoveType.Ice && target.GetMoveHitData(move).TypeMod > 0)
                        {
                            var hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                         move.Flags.BypassSub != true &&
                                         !(move.Infiltrates == true && battle.Gen >= 6);
                            if (hitSub) return damage;

                            if (target.EatItem())
                            {
                                battle.Debug("-50% reduction");
                                battle.Add("-enditem", target, "item: Yache Berry", "[weaken]");
                                battle.ChainModify(0.5);
                                return battle.FinalModify(damage);
                            }
                        }

                        return damage;
                    }),
                // OnEat: empty function
                Num = 188,
                Gen = 4,
            },

            // Z items
            [ItemId.ZapPlate] = new()
            {
                Id = ItemId.ZapPlate,
                Name = "Zap Plate",
                SpriteNum = 572,
                OnPlate = "Electric",
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, user, target, move) =>
                {
                    if (move.Type == MoveType.Electric)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 15),
                OnTakeItem = new OnTakeItemEventInfo((Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion>)(
                    (_, item, pokemon, source, _) =>
                    {
                        if (source?.BaseSpecies.Num == 493 || pokemon.BaseSpecies.Num == 493)
                        {
                            return BoolVoidUnion.FromBool(false);
                        }
                        return BoolVoidUnion.FromBool(true);
                    })),
                ForcedForme = "Arceus-Electric",
                Num = 300,
                Gen = 4,
            },
            [ItemId.ZoomLens] = new()
            {
                Id = ItemId.ZoomLens,
                Name = "Zoom Lens",
                SpriteNum = 574,
                Fling = new FlingData { BasePower = 10 },
                OnSourceModifyAccuracy = new OnSourceModifyAccuracyEventInfo(
                    (battle, accuracy, target, source, move) =>
                    {
                        if (battle.Queue.WillMove(target) == null)
                        {
                            battle.Debug("Zoom Lens boosting accuracy");
                            battle.ChainModify([4915, 4096]);
                            var result = battle.FinalModify(accuracy);
                            return DoubleVoidUnion.FromDouble(result);
                        }

                        return DoubleVoidUnion.FromVoid();
                    }, -2),
                Num = 276,
                Gen = 4,
            },
            // Skip Zygardite - mega stone (isNonstandard: "Future")
        };
    }
}