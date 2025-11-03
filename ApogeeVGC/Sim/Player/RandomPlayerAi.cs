using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Utils;
using System.Text.Json.Nodes;

namespace ApogeeVGC.Sim.Player;

/// <summary>
/// Example random player AI that makes random choices in battle.
/// </summary>
public class RandomPlayerAi(PlayerReadWriteStream stream, double move = 1.0, Prng? prng = null,
    PrngSeed? seed = null, bool debug = false) : BattlePlayer(stream, debug)
{
    protected readonly double MoveWeight = move;
    protected readonly Prng Prng = prng ?? new Prng(seed);

    public override void ReceiveError(Exception error)
    {
        // If we made an unavailable choice we will receive a followup request to
        // allow us the opportunity to correct our decision.
        if (error.Message.StartsWith("[Unavailable choice]"))
        {
            return;
        }
        base.ReceiveError(error);
    }

    public override void ReceiveRequest(JsonObject request)
    {
        try
        {
            Console.WriteLine($"[RandomPlayerAi] Received request with keys: {string.Join(", ", request.Select(kvp => kvp.Key))}");

            if (request.ContainsKey("wait") && request["wait"]?.GetValue<bool>() == true)
            {
                // Wait request - do nothing
                Console.WriteLine("[RandomPlayerAi] Wait request - doing nothing");
                return;
            }

            if (request.ContainsKey("forceSwitch"))
            {
                // Switch request
                Console.WriteLine("[RandomPlayerAi] Handling forced switch");
                HandleForcedSwitch(request);
            }
            else if (request.ContainsKey("teamPreview"))
            {
                // Team preview
                Console.WriteLine("[RandomPlayerAi] Handling team preview");
                _ = ChooseAsync("default");
            }
            else if (request.ContainsKey("active"))
            {
                // Move request
                Console.WriteLine("[RandomPlayerAi] Handling move request");
                HandleMoveRequest(request);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ReceiveRequest: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    private void HandleForcedSwitch(JsonObject request)
    {
        try
        {
            if (request["forceSwitch"] is not JsonArray forceSwitchArray)
            {
                Console.WriteLine("[HandleForcedSwitch] forceSwitch is not an array");
                return;
            }

            if (request["side"] is not JsonObject sideObj || sideObj["pokemon"] is not JsonArray pokemonArray)
            {
                Console.WriteLine("[HandleForcedSwitch] side or pokemon data missing");
                return;
            }

            var chosen = new HashSet<int>();
            var choices = new List<string>();

            Console.WriteLine($"[DEBUG] HandleForcedSwitch: Pokemon count = {pokemonArray.Count}, ForceSwitch count = {forceSwitchArray.Count}");

            for (int i = 0; i < forceSwitchArray.Count; i++)
            {
                bool needsSwitch = forceSwitchArray[i]?.GetValue<bool>() ?? false;
                if (!needsSwitch)
                {
                    choices.Add("pass");
                    continue;
                }

                var canSwitch = new List<int>();
                for (int j = 1; j <= 6; j++)
                {
                    int pokemonIndex = j - 1;
                    if (pokemonIndex >= pokemonArray.Count) break;

                    if (pokemonArray[pokemonIndex] is not JsonObject mon) continue;

                    bool isActive = mon["active"]?.GetValue<bool>() ?? false;
                    string conditionStr = mon["condition"]?.GetValue<string>() ?? "0/0";
                    bool isFainted = conditionStr.Contains("fnt") || conditionStr.StartsWith("0 ");

                    Console.WriteLine($"[DEBUG] Pokemon {j}: active={isActive}, condition='{conditionStr}', isFainted={isFainted}");

                    // Can switch if: not active, not already chosen, and not fainted
                    if (!isActive && 
                        !chosen.Contains(j) && 
                          !isFainted)
                    {
                        canSwitch.Add(j);
                    }
                }

                Console.WriteLine($"[DEBUG] Position {i}: Can switch to {canSwitch.Count} pokemon");

                if (canSwitch.Count == 0)
                {
                    choices.Add("pass");
                }
                else
                {
                    int target = Prng.Sample(canSwitch);
                    chosen.Add(target);
                    choices.Add($"switch {target}");
                }
            }

            _ = ChooseAsync(string.Join(", ", choices));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in HandleForcedSwitch: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    private void HandleMoveRequest(JsonObject request)
    {
        try
        {
            if (request["side"] is not JsonObject sideObj || sideObj["pokemon"] is not JsonArray pokemonArray)
            {
                Console.WriteLine("[HandleMoveRequest] side or pokemon data missing");
                return;
            }

            if (request["active"] is not JsonArray activeArray)
            {
                Console.WriteLine("[HandleMoveRequest] active data missing");
                return;
            }

            var choices = new List<string>();

            // Build a list of active pokemon in order
            var activePokemon = new List<JsonObject?>();
            for (int i = 0; i < pokemonArray.Count && activePokemon.Count < activeArray.Count; i++)
            {
                if (pokemonArray[i] is JsonObject mon && mon["active"]?.GetValue<bool>() == true)
                {
                    activePokemon.Add(mon);
                }
            }

            // Pad with nulls if needed
            while (activePokemon.Count < activeArray.Count)
            {
                activePokemon.Add(null);
            }

            for (int i = 0; i < activeArray.Count; i++)
            {
                if (activeArray[i] is not JsonObject activeData)
                {
                    choices.Add("pass");
                    continue;
                }

                // Get corresponding pokemon from side
                JsonObject? sidePokemon = i < activePokemon.Count ? activePokemon[i] : null;

                if (sidePokemon == null)
                {
                    choices.Add("pass");
                    continue;
                }

                string conditionStr = sidePokemon["condition"]?.GetValue<string>() ?? "0/0";
                bool isFainted = conditionStr.Contains("fnt") || conditionStr.StartsWith("0 ");
                bool commanding = sidePokemon["commanding"]?.GetValue<bool>() ?? false;

                if (isFainted || commanding)
                {
                    choices.Add("pass");
                    continue;
                }

                // Count available moves
                int moveCount = 0;
                if (activeData["moves"] is JsonArray movesArray)
                {
                    moveCount = movesArray.Count;
                }

                // Check if can switch
                bool canSwitch = false;
                bool trapped = sidePokemon["trapped"]?.GetValue<bool>() ?? false;
                if (!trapped)
                {
                    // Count non-active, non-fainted pokemon
                    for (int j = 0; j < pokemonArray.Count; j++)
                    {
                        if (pokemonArray[j] is JsonObject mon)
                        {
                            bool active = mon["active"]?.GetValue<bool>() ?? false;
                            string cond = mon["condition"]?.GetValue<string>() ?? "0/0";
                            bool fainted = cond.Contains("fnt") || cond.StartsWith("0 ");

                            if (!active && !fainted)
                            {
                                canSwitch = true;
                                break;
                            }
                        }
                    }
                }

                // Randomly decide to switch or use a move
                if (canSwitch && (moveCount == 0 || Prng.Random() > MoveWeight))
                {
                    // Choose a random switch target
                    var switchTargets = new List<int>();
                    for (int j = 1; j <= pokemonArray.Count; j++)
                    {
                        int idx = j - 1;
                        if (pokemonArray[idx] is JsonObject mon)
                        {
                            bool active = mon["active"]?.GetValue<bool>() ?? false;
                            string cond = mon["condition"]?.GetValue<string>() ?? "0/0";
                            bool fainted = cond.Contains("fnt") || cond.StartsWith("0 ");

                            if (!active && !fainted)
                            {
                                switchTargets.Add(j);
                            }
                        }
                    }

                    if (switchTargets.Count > 0)
                    {
                        int target = Prng.Sample(switchTargets);
                        choices.Add($"switch {target}");
                    }
                    else
                    {
                        choices.Add("pass");
                    }
                }
                else if (moveCount > 0)
                {
                    // Choose a random move slot (1-based)
                    int moveSlot = Prng.Random(1, moveCount + 1);
                    choices.Add($"move {moveSlot}");
                }
                else
                {
                    choices.Add("pass");
                }
            }

            _ = ChooseAsync(string.Join(", ", choices));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in HandleMoveRequest: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }
}