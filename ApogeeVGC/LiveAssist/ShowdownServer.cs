using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using ApogeeVGC.Data;
using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// WebSocket server that receives Showdown battle protocol from the Tampermonkey userscript,
/// runs ONNX inference (with optional MCTS), and sends recommendations back to the overlay.
/// </summary>
public sealed class ShowdownServer
{
    private readonly Library _library;
    private readonly Vocab _vocab;
    private readonly ModelInference _battleModel;
    private readonly TeamPreviewInference _previewModel;
    private readonly ActionMapper _actionMapper;
    private readonly MctsSearch? _mctsSearch;
    private readonly FormatId _formatId;
    private readonly string _host;
    private readonly int _port;

    public ShowdownServer(
        Library library,
        Vocab vocab,
        ModelInference battleModel,
        TeamPreviewInference previewModel,
        MctsConfig? mctsConfig,
        FormatId formatId,
        string host = "localhost",
        int port = 9876)
    {
        _library = library;
        _vocab = vocab;
        _battleModel = battleModel;
        _previewModel = previewModel;
        _actionMapper = new ActionMapper(vocab);
        _formatId = formatId;
        _host = host;
        _port = port;

        // MCTS is optional — if config is provided, we use search; otherwise policy-only
        if (mctsConfig != null)
        {
            _mctsSearch = new MctsSearch(mctsConfig, battleModel, _actionMapper);
            Console.WriteLine($"[Apogee] MCTS enabled: {mctsConfig.NumIterations} iterations");
        }
        else
        {
            Console.WriteLine("[Apogee] Policy-only mode (no MCTS)");
        }
    }

    public async Task RunAsync(CancellationToken ct = default)
    {
        var listener = new HttpListener();
        string prefix = $"http://{_host}:{_port}/";
        listener.Prefixes.Add(prefix);
        listener.Start();

        Console.WriteLine($"[Apogee] Server listening on ws://{_host}:{_port}");
        Console.WriteLine("[Apogee] Waiting for connection from Showdown...");

        try
        {
            while (!ct.IsCancellationRequested)
            {
                HttpListenerContext context = await listener.GetContextAsync();

                if (context.Request.IsWebSocketRequest)
                {
                    HttpListenerWebSocketContext wsContext =
                        await context.AcceptWebSocketAsync(null);
                    Console.WriteLine("[Apogee] Client connected");
                    _ = HandleClientAsync(wsContext.WebSocket, ct);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }
        finally
        {
            listener.Stop();
        }
    }

    private async Task HandleClientAsync(WebSocket ws, CancellationToken ct)
    {
        var state = new ShowdownState(_library);
        var shadowBattle = _mctsSearch != null ? new ShadowBattle(_library, _formatId) : null;
        bool shadowInitialized = false;
        byte[] buffer = new byte[64 * 1024]; // 64KB buffer for large messages

        // Wire up turn boundary event to advance the shadow battle
        if (shadowBattle != null)
        {
            state.OnTurnBoundary += turn =>
            {
                string? p1Choice = state.BuildChoiceString("p1");
                string? p2Choice = state.BuildChoiceString("p2");
                shadowBattle.AdvanceTurn(p1Choice, p2Choice, state);
                state.ClearTurnActions();
            };
        }

        try
        {
            while (ws.State == WebSocketState.Open && !ct.IsCancellationRequested)
            {
                // Read full message (may span multiple frames)
                using var ms = new MemoryStream();
                WebSocketReceiveResult result;
                do
                {
                    result = await ws.ReceiveAsync(buffer, ct);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("[Apogee] Client disconnected");
                        return;
                    }
                    ms.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);

                string raw = Encoding.UTF8.GetString(ms.ToArray());

                try
                {
                    using JsonDocument msg = JsonDocument.Parse(raw);
                    string? type = msg.RootElement.TryGetProperty("type", out JsonElement t)
                        ? t.GetString() : null;

                    switch (type)
                    {
                        case "battle":
                        {
                            string data = msg.RootElement.GetProperty("data").GetString() ?? "";
                            string[] lines = data.Split('\n');
                            state.Update(lines);
                            break;
                        }

                        case "request":
                        {
                            JsonElement data = msg.RootElement.GetProperty("data");
                            // Clone the data since msg will be disposed
                            using JsonDocument requestDoc = JsonDocument.Parse(data.GetRawText());
                            state.UpdateRequest(requestDoc);

                            if (state.Phase == "teampreview")
                            {
                                // Initialize shadow battle on first team preview request
                                if (shadowBattle != null && !shadowInitialized)
                                {
                                    shadowBattle.Initialize(state);
                                    shadowInitialized = true;
                                }

                                string response = HandleTeamPreview(state);
                                await SendAsync(ws, response, ct);
                            }
                            else if (state.Phase == "battle")
                            {
                                string response = HandleBattle(state, shadowBattle);
                                await SendAsync(ws, response, ct);
                            }
                            break;
                        }
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"[Apogee] JSON parse error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Apogee] Error: {ex.Message}");
                }
            }
        }
        catch (WebSocketException)
        {
            Console.WriteLine("[Apogee] Client disconnected (WebSocket error)");
        }
        catch (OperationCanceledException)
        {
            // Server shutting down
        }
    }

    private string HandleTeamPreview(ShowdownState state)
    {
        try
        {
            BattlePerspective perspective = state.BuildTeamPreviewPerspective();
            TeamPreviewOutput output = _previewModel.Evaluate(perspective);

            PreviewConfig config = _previewModel.Configs[output.ConfigIndex];
            HashSet<int> leadSet = new(config.Lead);
            HashSet<int> bringSet = new(config.Bring);

            // Display in terminal
            Console.WriteLine($"\n--- Team Preview (confidence: {output.Confidence:P1}) ---");
            for (int i = 0; i < state.OwnTeam.Count && i < 6; i++)
            {
                string role = leadSet.Contains(i) ? "LEAD" : bringSet.Contains(i) ? "BENCH" : "-";
                Console.WriteLine($"  {state.OwnTeam[i].Species,-20} {role}");
            }

            // Build response
            var pokemon = new List<object>();
            for (int i = 0; i < state.OwnTeam.Count && i < 6; i++)
            {
                string role = leadSet.Contains(i) ? "lead" : bringSet.Contains(i) ? "bench" : "not_brought";
                pokemon.Add(new
                {
                    species = state.OwnTeam[i].Species,
                    role,
                    confidence = output.Confidence,
                });
            }

            return JsonSerializer.Serialize(new { type = "team_preview", pokemon, configIndex = output.ConfigIndex });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Apogee] Team preview error: {ex.Message}");
            return "{}";
        }
    }

    private string HandleBattle(ShowdownState state, ShadowBattle? shadowBattle)
    {
        // Try MCTS if shadow battle is available
        if (_mctsSearch != null && shadowBattle?.GetBattle() is Battle battle)
        {
            try
            {
                return HandleBattleMcts(state, battle);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Apogee] MCTS failed, falling back to policy: {ex.Message}");
            }
        }

        // Policy-only fallback
        return HandleBattlePolicyOnly(state);
    }

    private string HandleBattleMcts(ShowdownState state, Battle battle)
    {
        BattlePerspective perspective = state.BuildPerspective();
        SideId sideId = state.MySide == "p1" ? SideId.P1 : SideId.P2;

        // Get the request from the shadow battle's side
        Side ourSide = sideId == SideId.P1 ? battle.P1 : battle.P2;
        IChoiceRequest? request = ourSide.ActiveRequest;

        if (request == null)
        {
            Console.WriteLine("[Apogee] No request from shadow battle, falling back to policy");
            return HandleBattlePolicyOnly(state);
        }

        var (actionA, actionB) = _mctsSearch.Search(battle, sideId, request, perspective);

        // Format the MCTS result
        string actionAStr = _vocab.GetActionKey(actionA.VocabIndex);
        string? actionBStr = actionB != null ? _vocab.GetActionKey(actionB.Value.VocabIndex) : null;

        // Get value from model for display
        ModelOutput output = _battleModel.Evaluate(perspective);
        float value = output.Value;

        // Display in terminal
        float pct = value * 100;
        string color = pct >= 55 ? "+" : pct >= 45 ? "~" : "-";
        Console.WriteLine($"\n--- Turn {state.CurrentTurn} [MCTS] --- Win: {pct:F1}% [{color}]");
        Console.WriteLine($"  Best A: {actionAStr}");
        if (actionBStr != null)
            Console.WriteLine($"  Best B: {actionBStr}");

        // Build response with MCTS recommendations
        var slotA = new List<object> { new { action = actionAStr, prob = 1.0f } };
        var slotB = actionBStr != null
            ? new List<object> { new { action = actionBStr, prob = 1.0f } }
            : new List<object>();

        return JsonSerializer.Serialize(new
        {
            type = "recommendation",
            value,
            mode = "mcts",
            slotA,
            slotB,
        });
    }

    private string HandleBattlePolicyOnly(ShowdownState state)
    {
        try
        {
            BattlePerspective perspective = state.BuildPerspective();

            // Get legal actions and masks
            var (actionSet, maskA, maskB) = state.BuildLegalActions(_vocab, _actionMapper);

            // Run model inference
            ModelOutput output = _battleModel.Evaluate(perspective);
            float value = output.Value;

            // Masked softmax for probabilities
            float[] probsA = ModelInference.MaskedSoftmax(output.PolicyA, maskA);
            float[] probsB = ModelInference.MaskedSoftmax(output.PolicyB, maskB);

            // Get top actions for display
            var slotAActions = ShowdownState.GetTopActions(probsA, actionSet.SlotA, _vocab);
            var slotBActions = ShowdownState.GetTopActions(probsB, actionSet.SlotB, _vocab);

            // Display in terminal
            float pct = value * 100;
            string color = pct >= 55 ? "+" : pct >= 45 ? "~" : "-";
            Console.WriteLine($"\n--- Turn {state.CurrentTurn} [Policy] --- Win: {pct:F1}% [{color}]");

            Console.WriteLine("  Slot A:");
            foreach (ActionRecommendation a in slotAActions)
                Console.WriteLine($"    {a.Action,-25} {a.Prob * 100,5:F1}%");

            if (slotBActions.Count > 0)
            {
                Console.WriteLine("  Slot B:");
                foreach (ActionRecommendation a in slotBActions)
                    Console.WriteLine($"    {a.Action,-25} {a.Prob * 100,5:F1}%");
            }

            // Build response for overlay
            return JsonSerializer.Serialize(new
            {
                type = "recommendation",
                value,
                mode = "policy",
                slotA = slotAActions.Select(a => new { action = a.Action, prob = a.Prob }),
                slotB = slotBActions.Select(a => new { action = a.Action, prob = a.Prob }),
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Apogee] Battle inference error: {ex.Message}");
            Console.WriteLine($"  {ex.StackTrace}");
            return "{}";
        }
    }

    private static async Task SendAsync(WebSocket ws, string message, CancellationToken ct)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, ct);
    }
}
