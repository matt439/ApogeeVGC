using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Utils;

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

    public override void ReceiveRequest(IChoiceRequest request)
    {
        Console.WriteLine($"[RandomPlayerAi] Received request type: {request.GetType().Name}");
        Console.WriteLine($"[RandomPlayerAi] Wait: {request.Wait}, TeamPreview: {request.TeamPreview}, ForceSwitch: {request.ForceSwitch != null}");
        
        try
        {
            if (request.Wait == true)
            {
                // Wait request - do nothing
                Console.WriteLine("[RandomPlayerAi] Wait request - doing nothing");
                return;
            }

            if (request.ForceSwitch != null)
            {
                // Switch request
                Console.WriteLine("[RandomPlayerAi] Handling forced switch");
                HandleForcedSwitch(request);
            }
            else if (request.TeamPreview == true)
            {
                // Team preview
                Console.WriteLine("[RandomPlayerAi] Handling team preview");
                string choice = ChooseTeamPreview(request.Side.Pokemon);
                _ = ChooseAsync(choice);
            }
            else
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

    private void HandleForcedSwitch(IChoiceRequest request)
    {
        try
        {
            var pokemon = request.Side.Pokemon;
            var forceSwitch = request.ForceSwitch!;
            var chosen = new HashSet<int>();
            var choices = new List<string>();

            Console.WriteLine($"[DEBUG] HandleForcedSwitch: Pokemon count = {pokemon.Count}, ForceSwitch count = {forceSwitch.Count}");

            for (int i = 0; i < forceSwitch.Count; i++)
            {
                if (!forceSwitch[i])
                {
                    choices.Add("pass");
                    continue;
                }

                var canSwitch = new List<int>();
                for (int j = 1; j <= 6; j++)
                {
                    int pokemonIndex = j - 1;
                    if (pokemonIndex >= pokemon.Count) break;

                    PokemonSwitchRequestData mon = pokemon[pokemonIndex];

                    // Check if we can switch to this pokemon
                    // FIXED: Check bounds before accessing pokemon[i]
                    bool needsReviving = i < pokemon.Count && pokemon[i].Reviving;
                    if (j > forceSwitch.Count && // not active
                        !chosen.Contains(j) && // not chosen for simultaneous switch
                        IsFainted(mon.Condition) == needsReviving) // fainted status matches reviving requirement
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
                    var switchOptions = canSwitch.Select(slot => 
                    {
                        Console.WriteLine($"[DEBUG] Creating switch option for slot {slot} (index {slot - 1})");
                        if (slot - 1 >= pokemon.Count)
                        {
                            throw new InvalidOperationException($"Invalid pokemon slot {slot}: pokemon count is {pokemon.Count}");
                        }
                        return new SwitchOption
                        {
                            Slot = slot,
                            Pokemon = pokemon[slot - 1],
                        };
                    }).ToList();

                    int target = ChooseSwitch(null, switchOptions);
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

    private void HandleMoveRequest(IChoiceRequest request)
    {
        try
        {
            var pokemon = request.Side.Pokemon;
            var moveRequest = request as MoveRequest;
            var choices = new List<string>();

            Console.WriteLine($"[DEBUG] HandleMoveRequest: Pokemon count = {pokemon.Count}");
            Console.WriteLine($"[DEBUG] Active pokemon count = {moveRequest?.Active?.Count ?? 0}");

            if (moveRequest?.Active == null || moveRequest.Active.Count == 0)
            {
                Console.WriteLine("[DEBUG] No active pokemon in move request");
                return;
            }

            // For each active pokemon slot, make a choice
            for (int i = 0; i < moveRequest.Active.Count; i++)
            {
                var activeMon = moveRequest.Active[i];
                
                // Find the corresponding pokemon in the side data
                // The active pokemon should be at the beginning of the Side.Pokemon list
                if (i >= pokemon.Count)
                {
                    Console.WriteLine($"[DEBUG] No pokemon data for active slot {i}");
                    choices.Add("pass");
                    continue;
                }

                var mon = pokemon[i];

                if (IsFainted(mon.Condition) || mon.Commanding)
                {
                    choices.Add("pass");
                    continue;
                }

                // Check if trapped
                bool trapped = activeMon.Trapped == true || activeMon.MaybeTrapped == true;

                // Get available moves from the active data
                var availableMoves = activeMon.Moves
                    .Select((move, index) => new { Move = move, Slot = index + 1 })
                    .Where(m => m.Move != null && m.Move.Disabled != true)
                    .Select(m => new MoveOption
                    {
                        Slot = m.Slot,
                        Choice = $"move {m.Slot}",
                    })
                    .ToList();

                // Get available switches (if not trapped)
                var canSwitch = new List<int>();
                if (!trapped)
                {
                    for (int j = moveRequest.Active.Count; j < pokemon.Count; j++)
                    {
                        PokemonSwitchRequestData switchMon = pokemon[j];
                        if (!switchMon.Active && !IsFainted(switchMon.Condition))
                        {
                            canSwitch.Add(j + 1); // 1-indexed
                        }
                    }
                }

                Console.WriteLine($"[DEBUG] Available moves: {availableMoves.Count}, Can switch to: {canSwitch.Count} pokemon");

                // Randomly decide to switch or use a move
                if (canSwitch.Count > 0 && (availableMoves.Count == 0 || Prng.Random() > MoveWeight))
                {
                    var switchOptions = canSwitch.Select(slot =>
                    {
                        int pokemonIndex = slot - 1;
                        Console.WriteLine($"[DEBUG] Creating switch option for slot {slot} (index {pokemonIndex})");
                        if (pokemonIndex >= pokemon.Count)
                        {
                            throw new InvalidOperationException($"Invalid pokemon slot {slot}: pokemon count is {pokemon.Count}");
                        }
                        return new SwitchOption
                        {
                            Slot = slot,
                            Pokemon = pokemon[pokemonIndex],
                        };
                    }).ToList();

                    int target = ChooseSwitch(mon, switchOptions);
                    choices.Add($"switch {target}");
                }
                else if (availableMoves.Count > 0)
                {
                    string move = ChooseMove(mon, availableMoves);
                    choices.Add(move);
                }
                else
                {
                    choices.Add("pass");
                }
            }

            Console.WriteLine($"[DEBUG] Making choices: {string.Join(", ", choices)}");
            _ = ChooseAsync(string.Join(", ", choices));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in HandleMoveRequest: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    protected virtual string ChooseTeamPreview(IReadOnlyList<PokemonSwitchRequestData> team)
    {
        return "default";
    }

    protected virtual string ChooseMove(
        PokemonSwitchRequestData active,
        IReadOnlyList<MoveOption> moves)
    {
        return Prng.Sample(moves).Choice;
    }

    protected virtual int ChooseSwitch(
        PokemonSwitchRequestData? active,
        IReadOnlyList<SwitchOption> switches)
    {
        return Prng.Sample(switches).Slot;
    }

    private static bool IsFainted(ConditionId condition)
    {
        // Check if the condition indicates the pokemon is fainted
        // This is a simplified check - you may need to adjust based on your ConditionId enum
        return condition.ToString().EndsWith("fnt") ||
               condition.ToString().Contains("Faint", StringComparison.OrdinalIgnoreCase);
    }
}

public record MoveOption
{
    public required int Slot { get; init; }
    public required string Choice { get; init; }
}

public record SwitchOption
{
    public required int Slot { get; init; }
    public required PokemonSwitchRequestData Pokemon { get; init; }
}
