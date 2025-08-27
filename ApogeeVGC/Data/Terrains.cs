using ApogeeVGC.Sim;
using System.Collections.ObjectModel;

namespace ApogeeVGC.Data;

public record Terrains
{
    public IReadOnlyDictionary<TerrainId, Terrain> TerrainData { get; }

    public Terrains()
    {
        TerrainData = new ReadOnlyDictionary<TerrainId, Terrain>(_terrains);
    }

    private readonly Dictionary<TerrainId, Terrain> _terrains = new()
    {
        [TerrainId.Electric] = new Terrain
        {
            TerrainId = TerrainId.Electric,
            BaseDuration = 5,
            DurationExtension = 3,
            // TODO: Implement effects
        },
        [TerrainId.Grassy] = new Terrain
        {
            TerrainId = TerrainId.Grassy,
            BaseDuration = 5,
            DurationExtension = 3,
            // TODO: Implement effects
        },
        [TerrainId.Misty] = new Terrain
        {
            TerrainId = TerrainId.Misty,
            BaseDuration = 5,
            DurationExtension = 3,
            // TODO: Implement effects
        },
        [TerrainId.Psychic] = new Terrain
        {
            TerrainId = TerrainId.Psychic,
            BaseDuration = 5,
            DurationExtension = 3,
            // TODO: Implement effects
        },
    };
}