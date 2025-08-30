using System.Collections.ObjectModel;
using ApogeeVGC.Sim;

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
                Condition = _library.Conditions[ConditionId.Leftovers].Copy(),
                OnBeforeResiduals = (itemHolder, context) =>
                {
                    if (!itemHolder.HasItem(ItemId.Leftovers))
                    {
                        throw new InvalidOperationException($"{itemHolder.Specie.Name} tried to use Leftovers" +
                                                            $"effect but does not hold Leftovers");
                    }
                    if (itemHolder.HasCondition(ConditionId.Leftovers))
                    {
                        throw new InvalidOperationException($"{itemHolder.Specie.Name} already has Leftovers" +
                                                            $"condition");
                    }
                    itemHolder.AddCondition(_library.Conditions[ConditionId.Leftovers].Copy(), context);
                },
                //OnResidualOrder = 5,
                //OnResidualSubOrder = 4,
                //OnResidual
            },
            [ItemId.ChoiceSpecs] = new()
            {
                Id = ItemId.ChoiceSpecs,
                Name = "Choice Specs",
                SpriteNum = 70,
                Fling = new FlingData { BasePower = 10 },
                Num = 297,
                Gen = 4,
                IsChoice = true,
                OnStart = (user, _) =>
                {
                    // remove any existing choice lock
                    if (user.HasCondition(ConditionId.ChoiceLock))
                    {
                        user.RemoveCondition(ConditionId.ChoiceLock);
                    }
                },
                OnModifyMove = (_, user, _, context) =>
                {
                    if (!user.HasItem(ItemId.ChoiceSpecs))
                    {
                        throw new InvalidOperationException($"{user.Specie.Name} tried to use Choice Specs" +
                                                            $"effect but does not hold Choice Specs");
                    }
                    if (!user.HasCondition(ConditionId.ChoiceLock))
                    {
                        user.AddCondition(_library.Conditions[ConditionId.ChoiceLock].Copy(), context);
                    }
                    if (!user.HasCondition(ConditionId.ChoiceSpecs))
                    {
                        user.AddCondition(_library.Conditions[ConditionId.ChoiceSpecs].Copy(), context);
                    }
                },
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
                Num = 273,
                Gen = 4,
                OnBeforeResiduals = (itemHolder, context) =>
                {
                    if (!itemHolder.HasItem(ItemId.FlameOrb))
                    {
                        throw new InvalidOperationException($"{itemHolder.Specie.Name} tried to use Flame Orb" +
                                                            $"effect but does not hold Flame Orb");
                    }
                    if (itemHolder.HasCondition(ConditionId.FlameOrb))
                    {
                        throw new InvalidOperationException($"{itemHolder.Specie.Name} already has Flame Orb" +
                                                            $"condition");
                    }
                    itemHolder.AddCondition(_library.Conditions[ConditionId.FlameOrb].Copy(), context);
                },
            },
            [ItemId.RockyHelmet] = new()
            {
                Id = ItemId.RockyHelmet,
                Name = "Rocky Helmet",
                SpriteNum = 417,
                Fling = new FlingData { BasePower = 60 },
                Num = 540,
                Gen = 5,
                OnDamagingHitOrder = 2,
                OnDamagingHit = (_, target, source, move, context) =>
                {
                    if (!(move.Flags.Contact ?? false)) return;

                    int actualDamage = source.Damage(source.UnmodifiedHp / 6);

                    if (context.PrintDebug)
                    {
                        UiGenerator.PrintRockyHelmetDamage(source, actualDamage, target);
                    }
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
            [ItemId.AssaultVest] = new()
            {
                Id = ItemId.AssaultVest,
                Name = "Assault Vest",
                SpriteNum = 581,
                Fling = new FlingData { BasePower = 80 },
                Num = 640,
                Gen = 6,
                OnStart = (pokemon, context) =>
                {
                    pokemon.AddCondition(_library.Conditions[ConditionId.AssaultVest].Copy(), context);
                },
            },
        };
    }
}

