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

        Vocab vocab = Vocab.Load(MctsVocabPath, Library);
        StateEncoder encoder = new(vocab, Library);

        using ModelInference battleModel = new(MctsModelPath, encoder);
        using TeamPreviewInference previewModel = new(MctsTeamPreviewModelPath, vocab);

        // MCTS enabled: shadow battle constructed from Showdown protocol
        MctsConfig mctsConfig = new() { NumIterations = 200 };

        ShowdownServer server = new(
            Library, vocab, battleModel, previewModel,
            mctsConfig,
            formatId,
            host: "localhost",
            port: 9876);

        Console.WriteLine("[Driver] Models loaded. Starting WebSocket server...");
        server.RunAsync().GetAwaiter().GetResult();
    }
}
