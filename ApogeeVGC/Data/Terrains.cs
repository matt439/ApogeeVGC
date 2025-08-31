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
                OnStart = (pokemon, context) =>
                 {
                     // TODO: check for 'terrain extenders'

                     // For each pokemon on the field, add the electric terrain condition
                     foreach (Pokemon p in pokemon)
                     {
                         p.AddCondition(_library.Conditions[ConditionId.ElectricTerrain].Copy(), context);
                     }

                     if (context.PrintDebug)
                     {
                         UiGenerator.PrintElectricTerrainStart();
                     }
                 },
                OnEnd = (pokemon, context) =>
                {
                    // For each pokemon on the field, remove the electric terrain condition
                    foreach (Pokemon p in pokemon)
                    {
                        if (!p.RemoveCondition(ConditionId.ElectricTerrain))
                        {
                            throw new InvalidOperationException($"Failed to remove Electric Rerrain condition" +
                                                                $"from {p.Specie.Name}");
                        }
                    }
                    if (context.PrintDebug)
                    {
                        UiGenerator.PrintElectricTerrainEnd();
                    }
                },
                //OnIncrementTurnCounter = (_, element, context) =>
                //{
                //    if (context.PrintDebug)
                //    {
                //        UiGenerator.PrintFieldElementCounter(element);
                //    }
                //},
                OnPokemonSwitchIn = (pokemon, context) =>
                {
                    pokemon.AddCondition(_library.Conditions[ConditionId.ElectricTerrain].Copy(), context);
                },
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