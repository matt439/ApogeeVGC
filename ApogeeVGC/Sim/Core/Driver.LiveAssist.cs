using ApogeeVGC.LiveAssist;
using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.FormatClasses;

namespace ApogeeVGC.Sim.Core;

public partial class Driver
{
    private void RunLiveAssist(FormatId formatId)
    {
        Console.WriteLine("[Driver] Starting Live Assist mode");
        Console.WriteLine("[Driver] Loading library and models...");

        var vocab = Vocab.Load(MctsVocabPath, Library);
        var encoder = new StateEncoder(vocab);

        using var battleModel = new ModelInference(MctsModelPath, encoder);
        using var previewModel = new TeamPreviewInference(MctsTeamPreviewModelPath, vocab);

        var assistant = new LiveAssistant(Library, vocab, battleModel, previewModel);
        assistant.Run();
    }
}
