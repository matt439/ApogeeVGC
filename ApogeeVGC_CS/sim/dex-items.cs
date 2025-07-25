using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ApogeeVGC_CS.sim
{
    public class FlingData
    {
        public int BasePower { get; set; }
        public string? Status { get; set; }
        public string? VolatileStatus { get; set; }
        public object? Effect { get; set; } // Replace with delegate or specific type if needed
    }

    public interface IItemData : IPokemonEventMethods
    {
        public string Name { get; set; }
        // Add other properties from Item as needed
    }

    public interface IModdedItemData : IItemData
    {
        public bool Inherit { get; set; }
        public Action<Battle, Pokemon>? OnCustap { get; set; }
    }

    public class ItemDataTable : Dictionary<string, IItemData> { }
    public class ModdedItemDataTable : Dictionary<string, IModdedItemData> { }

    public class Item : BasicEffect, IItem
    {
        public new EffectType EffectType { get; set; } = EffectType.Item;
        public new int Num { get; set; }
        public FlingData? Fling { get; set; }
        public string? OnDrive { get; set; }
        public string? OnMemory { get; set; }
        public string? MegaStone { get; set; }
        public string? MegaEvolves { get; set; }
        public object? ZMove { get; set; } // true or string
        public string? ZMoveType { get; set; }
        public string? ZMoveFrom { get; set; }
        public List<string>? ItemUser { get; set; }
        public bool IsBerry { get; set; }
        public bool IgnoreKlutz { get; set; }
        public string? OnPlate { get; set; }
        public bool IsGem { get; set; }
        public bool IsPokeball { get; set; }
        public bool IsPrimalOrb { get; set; }
        public IConditionData? Condition { get; set; }
        public string? ForcedForme { get; set; }
        public bool? IsChoice { get; set; }
        public (int BasePower, string Type)? NaturalGift { get; set; }
        public int? SpriteNum { get; set; }
        public object? Boosts { get; set; } // Replace with SparseBoostsTable or bool

        // Event handlers
        public Delegate? OnEat { get; set; }
        public Delegate? OnUse { get; set; }
        public Action<Battle, Pokemon>? OnStart { get; set; }
        public Action<Battle, Pokemon>? OnEnd { get; set; }

        //public int Gen { get; set; }
        //public string Id { get; set; } = string.Empty;
        //public string Name { get; set; } = string.Empty;

        public Item(IAnyObject data) : base(data)
        {
            if (data.TryGetString("name", out var name))
            {
                Fullname = name;
            }

            if (data.TryGetEnum<EffectType>("effectType", out var effectType))
            {
                EffectType = effectType;
            }

            if (data.TryGetClass<FlingData>("fling", out var flingData))
            {
                Fling = flingData;
            }

            if (data.TryGetString("onDrive", out var onDrive))
            {
                OnDrive = onDrive;
            }

            if (data.TryGetString("onMemory", out var onMemory))
            {
                OnMemory = onMemory;
            }

            if (data.TryGetString("megaStone", out var megaStone))
            {
                MegaStone = megaStone;
            }

            if (data.TryGetString("megaEvolves", out var megaEvolves))
            {
                MegaEvolves = megaEvolves;
            }

            // Z-Move can be a boolean or a string
            if (data.TryGetBool("zMove", out var zMoveBool))
            {
                ZMove = zMoveBool;
            }
            else if (data.TryGetString("zMove", out var zMoveString))
            {
                ZMove = zMoveString;
            }

            if (data.TryGetString("zMoveType", out var zMoveType))
            {
                ZMoveType = zMoveType;
            }

            if (data.TryGetString("zMoveFrom", out var zMoveFrom))
            {
                ZMoveFrom = zMoveFrom;
            }

            if (data.TryGetList<string>("itemUser", out var itemUser))
            {
                ItemUser = itemUser;
            }

            if (data.TryGetBool("isBerry", out var isBerry))
            {
                IsBerry = isBerry;
            }

            if (data.TryGetBool("ignoreKlutz", out var ignoreKlutz))
            {
                IgnoreKlutz = ignoreKlutz;
            }

            if (data.TryGetString("onPlate", out var onPlate))
            {
                OnPlate = onPlate;
            }

            if (data.TryGetBool("isGem", out var isGem))
            {
                IsGem = isGem;
            }

            if (data.TryGetBool("isPokeball", out var isPokeball))
            {
                IsPokeball = isPokeball;
            }

            if (data.TryGetBool("isPrimalOrb", out var isPrimalOrb))
            {
                IsPrimalOrb = isPrimalOrb;
            }

            if (data.TryGetInt("gen", out var gen))
            {
                Gen = gen;
            }
            else
            {
                // Auto-assign generation based on item number
                if (Num >= 1124)
                    Gen = 9;
                else if (Num >= 927)
                    Gen = 8;
                else if (Num >= 689)
                    Gen = 7;
                else if (Num >= 577)
                    Gen = 6;
                else if (Num >= 537)
                    Gen = 5;
                else if (Num >= 377)
                    Gen = 4;
                else
                    Gen = 3; // Default to Gen 3 for older items
            }

            if (IsBerry)
            {
                Fling ??= new FlingData { BasePower = 10 };
            }
            if (Id.EndsWith("plate"))
            {
                Fling ??= new FlingData { BasePower = 90 };
            }
            if (OnDrive != null)
            {
                Fling ??= new FlingData { BasePower = 70 };
            }
            if (MegaStone != null)
            {
                Fling ??= new FlingData { BasePower = 80 };
            }
            if (OnMemory != null)
            {
                Fling ??= new FlingData { BasePower = 50 };
            }
        }
    }

    public static class ItemConstants
    {
        public static readonly Item EmptyItem = new (new DefaultTextData
        {
            ["name"] = "",
            ["exists"] = false
        });
    }

    public class DexItems(ModdedDex dex)
    {
        private readonly ModdedDex _dex = dex;
        private readonly Dictionary<string, Item> _itemCache = [];
        private readonly List<Item>? _allCache = null;

        public Item Get(string name)
        {
            throw new NotImplementedException("Get method is not implemented yet.");
        }

        public Item Get(Item item)
        {
            throw new NotImplementedException("Get method is not implemented yet.");
        }

        public Item GetByID(Id id)
        {
            throw new NotImplementedException("GetByID method is not implemented yet.");
        }

        public List<Item> All()
        {
            throw new NotImplementedException("All method is not implemented yet.");
        }
    }
}