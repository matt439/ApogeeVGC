using System.Text.Json;
using ApogeeVGC.LiveAssist;
using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Core;

public partial class Driver
{
    private const string ShowdownConfigFileName = "showdown_config.json";

    private void RunShowdownBattler(FormatId formatId)
    {
        Console.WriteLine("[Driver] Starting Showdown Battler mode");

        // Resolve paths relative to solution root (exe runs from bin/)
        string solutionRoot = EquivalenceTestHelper.FindSolutionRoot(AppContext.BaseDirectory);
        string ShowdownConfigPath = Path.Combine(solutionRoot, ShowdownConfigFileName);

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

        // Load team from export paste file (resolve relative to solution root)
        string teamFilePath = Path.IsPathRooted(config.TeamFile)
            ? config.TeamFile
            : Path.Combine(solutionRoot, config.TeamFile);
        if (!File.Exists(teamFilePath))
        {
            Console.WriteLine($"[Driver] Team file not found: {teamFilePath}");
            Console.WriteLine("[Driver] Create a team file in Showdown's export format (copy from teambuilder).");
            return;
        }

        string teamText = File.ReadAllText(teamFilePath);
        List<PokemonSet> team = ShowdownTeamPacker.ImportExportPaste(teamText, Library);
        string packedTeam = ShowdownTeamPacker.Pack(team, Library);

        Console.WriteLine($"[Driver] Loaded team ({team.Count} Pokemon):");
        foreach (PokemonSet set in team)
            Console.WriteLine($"  {Library.Species[set.Species].Name} @ {Library.Items[set.Item].Name}");
        Console.WriteLine();

        // Resolve log directory relative to solution root
        if (!Path.IsPathRooted(config.LogDirectory))
            config.LogDirectory = Path.Combine(solutionRoot, config.LogDirectory);

        // Load models
        Console.WriteLine("[Driver] Loading models...");
        Vocab vocab = Vocab.Load(MctsVocabPath, Library);
        StateEncoder encoder = new(vocab, Library);
        bool needsBattleModel = config.Player.ToLowerInvariant()
            is "dlgreedy" or "dl-greedy" or "greedy";
        ModelInference? battleModel = needsBattleModel
            ? new ModelInference(MctsModelPath, encoder) : null;
        using TeamPreviewInference previewModel = new(MctsTeamPreviewModelPath, vocab);
        Console.WriteLine("[Driver] Models loaded.");

        // Create the configured player
        IShowdownPlayer player = CreateShowdownPlayer(config, battleModel, formatId);
        Console.WriteLine($"[Driver] Player: {player.Name}");

        // Create and run orchestrator
        var orchestrator = new ShowdownBattleOrchestrator(
            config, Library, vocab, previewModel, player, packedTeam);

        orchestrator.RunAsync().GetAwaiter().GetResult();

        // Dispose battle model if loaded
        battleModel?.Dispose();

        if (WaitForInput)
        {
            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine();
        }
    }

    private IShowdownPlayer CreateShowdownPlayer(
        ShowdownBotConfig config, ModelInference? battleModel, FormatId formatId)
    {
        return config.Player.ToLowerInvariant() switch
        {
            "dlgreedy" or "dl-greedy" or "greedy" =>
                new ShowdownPlayerDLGreedy(battleModel
                    ?? throw new InvalidOperationException("DL-Greedy requires battle model")),
            "random" => new ShowdownPlayerRandom(),
            "ensemble" => new ShowdownPlayerEnsemble(Library, formatId),
            _ => throw new InvalidOperationException(
                $"Unknown player type: '{config.Player}'. Valid options: dlgreedy, random, ensemble"),
        };
    }
}
