using System.Text.Json;
using ApogeeVGC.LiveAssist;
using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Core;

public partial class Driver
{
    /// <summary>
    /// CLI args for remote MCTS.
    /// </summary>
    private static string? WorkerHost;
    private static int WorkerPort = 9100;

    /// <summary>
    /// Parse remote MCTS CLI args.
    /// </summary>
    public static void SetRemoteArgs(string? workerHost, int? workerPort)
    {
        WorkerHost = workerHost;
        if (workerPort.HasValue) WorkerPort = workerPort.Value;
    }

    /// <summary>
    /// Run as MCTS worker server (on EC2).
    /// Listens for TCP connections and processes Showdown protocol.
    /// </summary>
    private void RunMctsWorker(FormatId formatId)
    {
        Console.WriteLine("[Driver] Starting MCTS Worker Server");

        string solutionRoot = EquivalenceTestHelper.FindSolutionRoot(AppContext.BaseDirectory);

        // Load vocab and team preview model (needed for the agent)
        Console.WriteLine("[Driver] Loading models...");
        Vocab vocab = Vocab.Load(MctsVocabPath, Library);
        using TeamPreviewInference previewModel = new(MctsTeamPreviewModelPath, vocab);
        Console.WriteLine("[Driver] Models loaded.");

        var server = new MctsWorkerServer(Library, vocab, previewModel, WorkerPort);
        Console.WriteLine($"[Driver] Worker ready on port {WorkerPort}");

        server.RunAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Run as remote Showdown battler (on home machine).
    /// Connects to Showdown and forwards to the EC2 worker.
    /// </summary>
    private void RunShowdownBattlerRemote(FormatId formatId)
    {
        Console.WriteLine("[Driver] Starting Remote Showdown Battler");

        if (string.IsNullOrEmpty(WorkerHost))
        {
            Console.WriteLine("[Driver] Error: --worker-host is required for remote mode");
            return;
        }

        string solutionRoot = EquivalenceTestHelper.FindSolutionRoot(AppContext.BaseDirectory);
        string configPath = Path.Combine(solutionRoot, ShowdownConfigFileName);

        if (!File.Exists(configPath))
        {
            Console.WriteLine($"[Driver] Config not found: {configPath}");
            return;
        }

        string configJson = File.ReadAllText(configPath);
        ShowdownBotConfig config = JsonSerializer.Deserialize<ShowdownBotConfig>(configJson,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
            ?? throw new InvalidOperationException("Failed to parse showdown_config.json");

        if (string.IsNullOrEmpty(config.Username) || string.IsNullOrEmpty(config.Password))
        {
            Console.WriteLine("[Driver] Error: username and password required");
            return;
        }

        // Load and pack team
        string teamFilePath = Path.IsPathRooted(config.TeamFile)
            ? config.TeamFile
            : Path.Combine(solutionRoot, config.TeamFile);
        if (!File.Exists(teamFilePath))
        {
            Console.WriteLine($"[Driver] Team file not found: {teamFilePath}");
            return;
        }

        string teamText = File.ReadAllText(teamFilePath);
        List<PokemonSet> team = ShowdownTeamPacker.ImportExportPaste(teamText, Library);
        string packedTeam = ShowdownTeamPacker.Pack(team, Library);

        Console.WriteLine($"[Driver] Team: {team.Count} Pokemon");
        if (!Path.IsPathRooted(config.LogDirectory))
            config.LogDirectory = Path.Combine(solutionRoot, config.LogDirectory);

        // Connect to worker
        Console.WriteLine($"[Driver] Connecting to worker at {WorkerHost}:{WorkerPort}...");
        MctsWorkerClient worker = MctsWorkerClient.ConnectAsync(WorkerHost, WorkerPort).GetAwaiter().GetResult();
        Console.WriteLine("[Driver] Connected to worker.");

        // Run battles
        var orchestrator = new ShowdownRemoteOrchestrator(config, worker, packedTeam);
        orchestrator.RunAsync().GetAwaiter().GetResult();

        worker.Dispose();
    }
}
