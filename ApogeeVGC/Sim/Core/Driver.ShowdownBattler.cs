using System.Text.Json;
using ApogeeVGC.LiveAssist;
using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Core;

public partial class Driver
{
    private const string ShowdownConfigPath = "showdown_config.json";

    private void RunShowdownBattler(FormatId formatId)
    {
        Console.WriteLine("[Driver] Starting Showdown Battler mode");

        // Load config
        if (!File.Exists(ShowdownConfigPath))
        {
            Console.WriteLine($"[Driver] Config file not found: {ShowdownConfigPath}");
            Console.WriteLine("[Driver] Create showdown_config.json with the following structure:");
            Console.WriteLine(JsonSerializer.Serialize(new ShowdownBotConfig(), new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            }));
            return;
        }

        string configJson = File.ReadAllText(ShowdownConfigPath);
        ShowdownBotConfig config = JsonSerializer.Deserialize<ShowdownBotConfig>(configJson,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
            ?? throw new InvalidOperationException("Failed to parse showdown_config.json");

        if (string.IsNullOrEmpty(config.Username) || string.IsNullOrEmpty(config.Password))
        {
            Console.WriteLine("[Driver] Error: username and password must be set in showdown_config.json");
            return;
        }

        // Load team from export paste file
        if (!File.Exists(config.TeamFile))
        {
            Console.WriteLine($"[Driver] Team file not found: {config.TeamFile}");
            Console.WriteLine("[Driver] Create a team file in Showdown's export format (copy from teambuilder).");
            return;
        }

        string teamText = File.ReadAllText(config.TeamFile);
        List<PokemonSet> team = ShowdownTeamPacker.ImportExportPaste(teamText, Library);
        string packedTeam = ShowdownTeamPacker.Pack(team, Library);

        Console.WriteLine($"[Driver] Loaded team ({team.Count} Pokemon):");
        foreach (PokemonSet set in team)
            Console.WriteLine($"  {Library.Species[set.Species].Name} @ {Library.Items[set.Item].Name}");
        Console.WriteLine();

        // Load models
        Console.WriteLine("[Driver] Loading models...");
        Vocab vocab = Vocab.Load(MctsVocabPath, Library);
        StateEncoder encoder = new(vocab);
        using ModelInference battleModel = new(MctsModelPath, encoder);
        using TeamPreviewInference previewModel = new(MctsTeamPreviewModelPath, vocab);
        Console.WriteLine("[Driver] Models loaded.");

        // Create the configured player
        IShowdownPlayer player = CreateShowdownPlayer(config, battleModel);
        Console.WriteLine($"[Driver] Player: {player.Name}");

        // Create and run orchestrator
        var orchestrator = new ShowdownBattleOrchestrator(
            config, Library, vocab, previewModel, player, packedTeam);

        orchestrator.RunAsync().GetAwaiter().GetResult();
    }

    private static IShowdownPlayer CreateShowdownPlayer(ShowdownBotConfig config, ModelInference battleModel)
    {
        return config.Player.ToLowerInvariant() switch
        {
            "dlgreedy" or "dl-greedy" or "greedy" => new ShowdownPlayerDLGreedy(battleModel),
            "random" => new ShowdownPlayerRandom(),
            _ => throw new InvalidOperationException(
                $"Unknown player type: '{config.Player}'. Valid options: dlgreedy, random"),
        };
    }
}
