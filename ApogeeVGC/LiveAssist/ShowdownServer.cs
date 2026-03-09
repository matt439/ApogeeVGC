using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using ApogeeVGC.Data;
using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.BattleClasses;

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
    private readonly string _host;
    private readonly int _port;

    public ShowdownServer(
        Library library,
        Vocab vocab,
        ModelInference battleModel,
        TeamPreviewInference previewModel,
        MctsConfig? mctsConfig,
        string host = "localhost",
        int port = 9876)
    {
        _library = library;
        _vocab = vocab;
        _battleModel = battleModel;
        _previewModel = previewModel;
        _actionMapper = new ActionMapper(vocab);
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
        byte[] buffer = new byte[64 * 1024]; // 64KB buffer for large messages

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
                                string response = HandleTeamPreview(state);
                                await SendAsync(ws, response, ct);
                            }
                            else if (state.Phase == "battle")
                            {
                                string response = HandleBattle(state);
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

            // Apply sigmoid
            float[] bringScores = ApplySigmoid(output.BringScores);
            float[] leadScores = ApplySigmoid(output.LeadScores);

            // Display in terminal
            Console.WriteLine("\n--- Team Preview ---");
            for (int i = 0; i < state.OwnTeam.Count && i < 6; i++)
            {
                float bring = i < bringScores.Length ? bringScores[i] * 100 : 0;
                float lead = i < leadScores.Length ? leadScores[i] * 100 : 0;
                Console.WriteLine($"  {state.OwnTeam[i].Species,-20} Bring: {bring,5:F0}%  Lead: {lead,5:F0}%");
            }

            // Build response
            var pokemon = new List<object>();
            for (int i = 0; i < state.OwnTeam.Count && i < 6; i++)
            {
                pokemon.Add(new
                {
                    species = state.OwnTeam[i].Species,
                    bringScore = i < bringScores.Length ? bringScores[i] : 0f,
                    leadScore = i < leadScores.Length ? leadScores[i] : 0f,
                });
            }

            return JsonSerializer.Serialize(new { type = "team_preview", pokemon });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Apogee] Team preview error: {ex.Message}");
            return "{}";
        }
    }

    private string HandleBattle(ShowdownState state)
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
            Console.WriteLine($"\n--- Turn {state.CurrentTurn} --- Win: {pct:F1}% [{color}]");

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

    private static float[] ApplySigmoid(float[] raw)
    {
        float[] result = new float[raw.Length];
        for (int i = 0; i < raw.Length; i++)
            result[i] = 1f / (1f + MathF.Exp(-raw[i]));
        return result;
    }

    private static async Task SendAsync(WebSocket ws, string message, CancellationToken ct)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, ct);
    }
}
