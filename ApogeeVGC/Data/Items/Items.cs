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
        foreach (var kvp in CreateItemsAbc())
            items[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateItemsDef())
            items[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateItemsGhi())
            items[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateItemsJkl())
            items[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateItemsMno())
            items[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateItemsPqr())
            items[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateItemsStu())
            items[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateItemsVwx())
            items[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateItemsYz())
            items[kvp.Key] = kvp.Value;

        return items;
    }

    // Partial methods to be implemented in other files
    private partial Dictionary<ItemId, Item> CreateItemsAbc();
    private partial Dictionary<ItemId, Item> CreateItemsDef();
    private partial Dictionary<ItemId, Item> CreateItemsGhi();
    private partial Dictionary<ItemId, Item> CreateItemsJkl();
    private partial Dictionary<ItemId, Item> CreateItemsMno();
    private partial Dictionary<ItemId, Item> CreateItemsPqr();
    private partial Dictionary<ItemId, Item> CreateItemsStu();
    private partial Dictionary<ItemId, Item> CreateItemsVwx();
    private partial Dictionary<ItemId, Item> CreateItemsYz();
}