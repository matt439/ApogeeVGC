using System.Text.Json;
using ApogeeVGC.Data;
using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// Handles AI decision-making for a single Showdown battle.
/// Delegates battle decisions to an IShowdownPlayer implementation
/// (DL-Greedy, Random, Ensemble).
/// Team preview always uses TeamPreviewInference.
/// For ensemble players, manages the shadow battle lifecycle.
/// </summary>
public sealed class ShowdownBattleAgent
{
    private readonly ShowdownState _state;
    private readonly Vocab _vocab;
    private readonly ActionMapper _actionMapper;
    private readonly TeamPreviewInference _previewModel;
    private readonly IShowdownPlayer _player;
    private bool _shadowInitialized;

    public ShowdownState State => _state;

    public ShowdownBattleAgent(
        Library library,
        Vocab vocab,
        TeamPreviewInference previewModel,
        IShowdownPlayer player)
    {
        _state = new ShowdownState(library);
        _vocab = vocab;
        _actionMapper = new ActionMapper(vocab);
        _previewModel = previewModel;
        _player = player;
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

        // Force switch or normal move request
        if (root.TryGetProperty("forceSwitch", out _) ||
            root.TryGetProperty("active", out _))
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
            Console.WriteLine($"  -> /choose {choice}");

            // Initialize shadow battle for ensemble player
            if (_player is ShowdownPlayerEnsemble ensemblePlayer && !_shadowInitialized)
            {
                SideId sideId = _state.MySide == "p1" ? SideId.P1 : SideId.P2;
                ensemblePlayer.InitializeShadowBattle(_state, sideId);
                _shadowInitialized = true;
            }

            return choice;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Agent] Team preview error: {ex.Message}");
            return "team 1234";
        }
    }

    private string HandleBattle()
    {
        try
        {
            BattlePerspective perspective = _state.BuildPerspective();
            var (actionSet, maskA, maskB) = _state.BuildLegalActions(_vocab, _actionMapper);

            Console.WriteLine($"\n--- Turn {_state.CurrentTurn} [{_player.Name}] ---");

            // Delegate decision to the configured player
            (LegalAction bestA, LegalAction? bestB) =
                _player.ChooseBattle(perspective, actionSet, maskA, maskB);

            // Serialize to /choose format
            string choice = ShowdownChoiceSerializer.SerializeBattleChoice(bestA, bestB, _state);
            Console.WriteLine($"  -> /choose {choice}");

            return choice;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Agent] Battle decision error: {ex.Message}");
            Console.WriteLine($"  {ex.StackTrace}");
            return "default";
        }
    }
}
