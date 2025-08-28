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
            [TerrainId.Electric] = new Terrain
            {
                Id = TerrainId.Electric,
                BaseDuration = 5,
                DurationExtension = 3,
                // TODO: Implement effects
            },
            [TerrainId.Grassy] = new Terrain
            {
                Id = TerrainId.Grassy,
                BaseDuration = 5,
                DurationExtension = 3,
                // TODO: Implement effects
            },
            [TerrainId.Misty] = new Terrain
            {
                Id = TerrainId.Misty,
                BaseDuration = 5,
                DurationExtension = 3,
                // TODO: Implement effects
            },
            [TerrainId.Psychic] = new Terrain
            {
                Id = TerrainId.Psychic,
                BaseDuration = 5,
                DurationExtension = 3,
                // TODO: Implement effects
            },
        };
    }
}