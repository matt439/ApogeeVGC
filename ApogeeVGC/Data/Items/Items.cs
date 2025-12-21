using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Items;

namespace ApogeeVGC.Data.Items;

public partial record Items
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
        var items = new Dictionary<ItemId, Item>();

        // Combine all partial item dictionaries
        foreach (var kvp in CreateItemsABC())
            items[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateItemsDEF())
            items[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateItemsGHI())
            items[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateItemsJKL())
            items[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateItemsMNO())
            items[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateItemsPQR())
            items[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateItemsSTU())
            items[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateItemsVWX())
            items[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateItemsYZ())
            items[kvp.Key] = kvp.Value;

        return items;
    }

    // Partial methods to be implemented in other files
    private partial Dictionary<ItemId, Item> CreateItemsABC();
    private partial Dictionary<ItemId, Item> CreateItemsDEF();
    private partial Dictionary<ItemId, Item> CreateItemsGHI();
    private partial Dictionary<ItemId, Item> CreateItemsJKL();
    private partial Dictionary<ItemId, Item> CreateItemsMNO();
    private partial Dictionary<ItemId, Item> CreateItemsPQR();
    private partial Dictionary<ItemId, Item> CreateItemsSTU();
    private partial Dictionary<ItemId, Item> CreateItemsVWX();
    private partial Dictionary<ItemId, Item> CreateItemsYZ();
}
