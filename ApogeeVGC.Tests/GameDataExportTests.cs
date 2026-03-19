using System.Text.Json;
using ApogeeVGC.Data;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.Tests;

/// <summary>
/// Exports species and move data from the C# Library to JSON files
/// for use by the Python training pipeline.
/// </summary>
[Collection(LibraryCollection.Name)]
public class GameDataExportTests(LibraryFixture fixture)
{
    private static readonly string OutputDir = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "Tools", "DLModel", "game_data");

    [Fact]
    public void ExportGameData()
    {
        Directory.CreateDirectory(OutputDir);

        ExportSpeciesData();
        ExportMoveData();
    }

    private void ExportSpeciesData()
    {
        var speciesDict = new Dictionary<string, object>();

        foreach (var (specieId, species) in fixture.Library.Species)
        {
            var types = new List<string>();
            foreach (var t in species.Types)
                types.Add(t.ToString());

            speciesDict[species.Name] = new
            {
                types,
                baseStats = new Dictionary<string, int>
                {
                    ["hp"] = species.BaseStats.Hp,
                    ["atk"] = species.BaseStats.Atk,
                    ["def"] = species.BaseStats.Def,
                    ["spa"] = species.BaseStats.SpA,
                    ["spd"] = species.BaseStats.SpD,
                    ["spe"] = species.BaseStats.Spe,
                }
            };
        }

        string json = JsonSerializer.Serialize(speciesDict, new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        string path = Path.Combine(OutputDir, "species_data.json");
        File.WriteAllText(path, json);
    }

    private void ExportMoveData()
    {
        var moveDict = new Dictionary<string, object>();

        foreach (var (moveId, move) in fixture.Library.Moves)
        {
            bool isSpread = move.Target is MoveTarget.AllAdjacentFoes
                or MoveTarget.AllAdjacent or MoveTarget.All;

            moveDict[move.Name] = new
            {
                basePower = move.BasePower,
                type = move.Type.ToString(),
                category = move.Category.ToString(),
                isSpread,
            };
        }

        string json = JsonSerializer.Serialize(moveDict, new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        string path = Path.Combine(OutputDir, "move_data.json");
        File.WriteAllText(path, json);
    }
}
