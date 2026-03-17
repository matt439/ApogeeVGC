using System.Text.Json;
using ApogeeVGC.Data;
using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.BattleClasses;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// Handles AI decision-making for a single Showdown battle.
/// Uses ShowdownState for protocol tracking and DL models for inference.
/// Pattern follows ShowdownServer.HandleBattlePolicyOnly().
/// </summary>
public sealed class ShowdownBattleAgent
{
    private readonly ShowdownState _state;
    private readonly Vocab _vocab;
    private readonly ActionMapper _actionMapper;
    private readonly ModelInference _battleModel;
    private readonly TeamPreviewInference _previewModel;

    public ShowdownState State => _state;

    public ShowdownBattleAgent(
        Library library,
        Vocab vocab,
        ModelInference battleModel,
        TeamPreviewInference previewModel)
    {
        _state = new ShowdownState(library);
        _vocab = vocab;
        _actionMapper = new ActionMapper(vocab);
        _battleModel = battleModel;
        _previewModel = previewModel;
    }

    /// <summary>
    /// Process a batch of battle protocol lines.
    /// </summary>
    public void ProcessBattleLines(string[] lines)
    {
        _state.Update(lines);
    }

    /// <summary>
    /// Process a |request| JSON and return the /choose command string,
    /// or null if no decision is needed (e.g. wait request).
    /// </summary>
    public string? HandleRequest(string requestJson)
    {
        using JsonDocument doc = JsonDocument.Parse(requestJson);
        // Clone so it survives disposal
        using JsonDocument clone = JsonDocument.Parse(doc.RootElement.GetRawText());
        _state.UpdateRequest(JsonDocument.Parse(clone.RootElement.GetRawText()));

        JsonElement root = clone.RootElement;

        // Wait request — no decision needed
        if (root.TryGetProperty("wait", out JsonElement wait) &&
            wait.ValueKind == JsonValueKind.True)
            return null;

        // Team preview
        if (root.TryGetProperty("teamPreview", out JsonElement tp) &&
            tp.ValueKind == JsonValueKind.True)
        {
            return HandleTeamPreview();
        }

        // Force switch
        if (root.TryGetProperty("forceSwitch", out _))
        {
            return HandleBattle();
        }

        // Normal move request
        if (root.TryGetProperty("active", out _))
        {
            return HandleBattle();
        }

        return null;
    }

    private string HandleTeamPreview()
    {
        try
        {
            BattlePerspective perspective = _state.BuildTeamPreviewPerspective();
            TeamPreviewOutput output = _previewModel.Evaluate(perspective);

            PreviewConfig config = _previewModel.Configs[output.ConfigIndex];
            HashSet<int> leadSet = new(config.Lead);
            HashSet<int> bringSet = new(config.Bring);

            Console.WriteLine($"\n--- Team Preview (confidence: {output.Confidence:P1}) ---");
            for (int i = 0; i < _state.OwnTeam.Count && i < 6; i++)
            {
                string role = leadSet.Contains(i) ? "LEAD" : bringSet.Contains(i) ? "BENCH" : "-";
                Console.WriteLine($"  {_state.OwnTeam[i].Species,-20} {role}");
            }

            string choice = ShowdownChoiceSerializer.SerializeTeamPreview(output.OrderedIndices);
            Console.WriteLine($"  → /choose {choice}");
            return choice;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Agent] Team preview error: {ex.Message}");
            // Fallback: bring first 4 in order
            return "team 1234";
        }
    }

    private string HandleBattle()
    {
        try
        {
            BattlePerspective perspective = _state.BuildPerspective();
            var (actionSet, maskA, maskB) = _state.BuildLegalActions(_vocab, _actionMapper);

            // Model inference
            ModelOutput output = _battleModel.Evaluate(perspective);
            float value = output.Value;

            // Masked softmax
            float[] probsA = ModelInference.MaskedSoftmax(output.PolicyA, maskA);
            float[] probsB = actionSet.SlotB.Count > 0
                ? ModelInference.MaskedSoftmax(output.PolicyB, maskB)
                : [];

            // Pick best action per slot (argmax)
            LegalAction bestA = PickBest(actionSet.SlotA, probsA);
            LegalAction? bestB = actionSet.SlotB.Count > 0
                ? PickBestB(actionSet.SlotB, probsB, bestA)
                : null;

            // Serialize to /choose format
            string choice = ShowdownChoiceSerializer.SerializeBattleChoice(bestA, bestB, _state);

            // Log
            float pct = value * 100;
            string indicator = pct >= 55 ? "+" : pct >= 45 ? "~" : "-";
            Console.WriteLine($"\n--- Turn {_state.CurrentTurn} --- Win: {pct:F1}% [{indicator}]");
            Console.WriteLine($"  → /choose {choice}");

            return choice;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Agent] Battle decision error: {ex.Message}");
            Console.WriteLine($"  {ex.StackTrace}");
            return "default";
        }
    }

    /// <summary>Pick highest-probability legal action for slot A.</summary>
    private static LegalAction PickBest(IReadOnlyList<LegalAction> actions, float[] probs)
    {
        LegalAction best = actions[0];
        float bestProb = -1f;
        foreach (LegalAction action in actions)
        {
            float p = action.VocabIndex < probs.Length ? probs[action.VocabIndex] : 0f;
            if (p > bestProb)
            {
                bestProb = p;
                best = action;
            }
        }
        return best;
    }

    /// <summary>
    /// Pick highest-probability legal action for slot B,
    /// ensuring no duplicate switch target with slot A.
    /// </summary>
    private static LegalAction PickBestB(
        IReadOnlyList<LegalAction> actions, float[] probs, LegalAction slotA)
    {
        LegalAction best = actions[0];
        float bestProb = -1f;
        foreach (LegalAction action in actions)
        {
            // Prevent both slots switching to the same Pokemon
            if (action.ChoiceType == Sim.Choices.ChoiceType.Switch &&
                slotA.ChoiceType == Sim.Choices.ChoiceType.Switch &&
                action.SwitchIndex == slotA.SwitchIndex)
                continue;

            float p = action.VocabIndex < probs.Length ? probs[action.VocabIndex] : 0f;
            if (p > bestProb)
            {
                bestProb = p;
                best = action;
            }
        }
        return best;
    }
}
