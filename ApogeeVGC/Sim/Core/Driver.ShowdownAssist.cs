using ApogeeVGC.LiveAssist;
using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.FormatClasses;

namespace ApogeeVGC.Sim.Core;

public partial class Driver
{
    private void RunShowdownAssist(FormatId formatId)
    {
        Console.WriteLine("[Driver] Starting Showdown Live Assist mode");
        Console.WriteLine("[Driver] Loading library and models...");

        var vocab = Vocab.Load(MctsVocabPath, Library);
        var encoder = new StateEncoder(vocab);

        using var battleModel = new ModelInference(MctsModelPath, encoder);
        using var previewModel = new TeamPreviewInference(MctsTeamPreviewModelPath, vocab);

        // MCTS enabled: shadow battle constructed from Showdown protocol
        var mctsConfig = new MctsConfig { NumIterations = 200 };

        var server = new ShowdownServer(
            Library, vocab, battleModel, previewModel,
            mctsConfig,
            formatId,
            host: "localhost",
            port: 9876);

        Console.WriteLine("[Driver] Models loaded. Starting WebSocket server...");
        server.RunAsync().GetAwaiter().GetResult();
    }
}
