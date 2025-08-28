using ApogeeVGC.Sim;
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;

namespace ApogeeVGC.Data;

public record Terrains
{
    public IReadOnlyDictionary<TerrainId, Terrain> TerrainData { get; }
    private readonly Library _library;

    public Terrains(Library library)
    {
        _library = library;
        TerrainData = new ReadOnlyDictionary<TerrainId, Terrain>(CreateTerrains());
    }

    private Dictionary<TerrainId, Terrain> CreateTerrains()
    {
        return new Dictionary<TerrainId, Terrain>
        {
            [TerrainId.Electric] = new()
            {
                Id = TerrainId.Electric,
                Name = "Electric Terrain",
                BaseDuration = 5,
                DurationExtension = 3,
                // TODO: Implement effects
            },
            [TerrainId.Grassy] = new()
            {
                Id = TerrainId.Grassy,
                Name = "Grassy Terrain",
                BaseDuration = 5,
                DurationExtension = 3,
                // TODO: Implement effects
            },
            [TerrainId.Misty] = new()
            {
                Id = TerrainId.Misty,
                Name = "Misty Terrain",
                BaseDuration = 5,
                DurationExtension = 3,
                // TODO: Implement effects
            },
            [TerrainId.Psychic] = new()
            {
                Id = TerrainId.Psychic,
                Name = "Psychic Terrain",
                BaseDuration = 5,
                DurationExtension = 3,
                // TODO: Implement effects
            },
        };
    }
}