using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Player;

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
        if (request.Wait == true)
        {
            // Wait request - do nothing
            return;
        }

        if (request.ForceSwitch != null)
        {
            // Switch request
            HandleForcedSwitch(request);
        }
        else if (request.TeamPreview == true)
        {
            // Team preview
            string choice = ChooseTeamPreview(request.Side.Pokemon);
            _ = ChooseAsync(choice);
        }
        else
        {
            // Move request
            HandleMoveRequest(request);
        }
    }

    private void HandleForcedSwitch(IChoiceRequest request)
    {
        var pokemon = request.Side.Pokemon;
        var forceSwitch = request.ForceSwitch!;
        var chosen = new HashSet<int>();
        var choices = new List<string>();

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
                if (j > forceSwitch.Count && // not active
                    !chosen.Contains(j) && // not chosen for simultaneous switch
                    IsFainted(mon.Condition) == (i < pokemon.Count && pokemon[i].Reviving)) // fainted status matches reviving requirement
                {
                    canSwitch.Add(j);
                }
            }

            if (canSwitch.Count == 0)
            {
                choices.Add("pass");
            }
            else
            {
                var switchOptions = canSwitch.Select(slot => new SwitchOption
                {
                    Slot = slot,
                    Pokemon = pokemon[slot - 1],
                }).ToList();

                int target = ChooseSwitch(null, switchOptions);
                chosen.Add(target);
                choices.Add($"switch {target}");
            }
        }

        _ = ChooseAsync(string.Join(", ", choices));
    }

    private void HandleMoveRequest(IChoiceRequest request)
    {
        var pokemon = request.Side.Pokemon;
        var choices = new List<string>();

        // For each active pokemon, make a choice
        var activePokemon = pokemon.Where(p => p.Active).ToList();

        foreach (PokemonSwitchRequestData mon in activePokemon)
        {
            if (IsFainted(mon.Condition) || mon.Commanding)
            {
                choices.Add("pass");
                continue;
            }

            // Get available moves
            var availableMoves = mon.Moves
                .Select((move, index) => new { Move = move, Slot = index + 1 })
                .Where(m => m.Move != null)
                .Select(m => new MoveOption
                {
                    Slot = m.Slot,
                    Choice = $"move {m.Slot}",
                })
                .ToList();

            // Get available switches
            var canSwitch = new List<int>();
            if (!mon.Active) // If trapped, we can't switch
            {
                for (int j = 1; j <= 6; j++)
                {
                    int pokemonIndex = j - 1;
                    if (pokemonIndex >= pokemon.Count) break;

                    PokemonSwitchRequestData switchMon = pokemon[pokemonIndex];
                    if (!switchMon.Active && !IsFainted(switchMon.Condition))
                    {
                        canSwitch.Add(j);
                    }
                }
            }

            // Randomly decide to switch or use a move
            if (canSwitch.Count > 0 && (availableMoves.Count == 0 || Prng.Random() > MoveWeight))
            {
                var switchOptions = canSwitch.Select(slot => new SwitchOption
                {
                    Slot = slot,
                    Pokemon = pokemon[slot - 1],
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
                throw new InvalidOperationException(
                    $"{GetType().Name} unable to make choice. " +
                    $"No available moves or switches.");
            }
        }

        _ = ChooseAsync(string.Join(", ", choices));
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