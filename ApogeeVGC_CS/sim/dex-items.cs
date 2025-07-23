using System;
using System.Collections.Generic;

namespace ApogeeVGC_CS.sim
{
    // FlingData structure
    public class FlingData
    {
        public int BasePower { get; set; }
        public string? Status { get; set; }
        public string? VolatileStatus { get; set; }
        public object? Effect { get; set; } // Replace with delegate or specific type if needed
    }

    // ItemData structure
    public class ItemData : IPokemonEventMethods
    {
        public string Name { get; set; } = string.Empty;
        // Add other properties from Item as needed
    }

    // ModdedItemData structure
    public class ModdedItemData : ItemData
    {
        public bool Inherit { get; set; }
        public Action<Battle, Pokemon>? OnCustap { get; set; }
    }

    // ItemDataTable and ModdedItemDataTable
    public class ItemDataTable : Dictionary<string, ItemData> { }
    public class ModdedItemDataTable : Dictionary<string, ModdedItemData> { }

    // Item class
    public class Item : BasicEffect
    {
        public string EffectType { get; set; } = "Item";
        public int Num { get; set; }
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
        public object? Condition { get; set; } // Replace with ConditionData if available
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

        public int Gen { get; set; }
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public Item(IAnyObject data) : base(data)
        {
            Name = data.ContainsKey("name") ? data["name"].ToString() ?? string.Empty : string.Empty;
            EffectType = "Item";
            // Assign other fields from data as needed

            // Generation logic
            if (!Gen)
            {
                if (Num >= 1124) Gen = 9;
                else if (Num >= 927) Gen = 8;
                else if (Num >= 689) Gen = 7;
                else if (Num >= 577) Gen = 6;
                else if (Num >= 537) Gen = 5;
                else if (Num >= 377) Gen = 4;
                else Gen = 3;
            }

            // Fling logic
            if (IsBerry) Fling = new FlingData { BasePower = 10 };
            if (Id.EndsWith("plate")) Fling = new FlingData { BasePower = 90 };
            if (OnDrive != null) Fling = new FlingData { BasePower = 70 };
            if (MegaStone != null) Fling = new FlingData { BasePower = 80 };
            if (OnMemory != null) Fling = new FlingData { BasePower = 50 };

            // TODO: assignMissingFields logic
        }
    }

    public class DexItems
    {
        public ModdedDex Dex { get; }
        public Dictionary<string, Item> ItemCache { get; } = new();
        public IReadOnlyList<Item>? AllCache { get; set; } = null;

        public DexItems(ModdedDex dex)
        {
            Dex = dex;
        }
    }
}