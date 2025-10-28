using ApogeeVGC.Sim.Items;

namespace ApogeeVGC.Sim.Choices;

/// <summary>
/// Data Transfer Object for Item - contains only serializable data needed for requests.
/// </summary>
public record ItemDto
{
    public required ItemId Id { get; init; }
    public required string Name { get; init; }
    public int Num { get; init; }

    public static ItemDto FromItem(Item item)
    {
        return new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Num = item.Num,
        };
    }
}
