using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using Spectre.Console;

namespace ApogeeVGC.Player;

/// <summary>
/// Console-based player that displays battle state and accepts input via console.
/// Uses Spectre.Console for rich terminal UI.
/// </summary>
public class PlayerConsole : IPlayer
{
    public SideId SideId { get; }
    public PlayerOptions Options { get; }
    public PlayerUiType UiType => PlayerUiType.Console;
    public IBattleController BattleController { get; }
    public bool PrintDebug { get; }

    private BattlePerspective? _currentPerspective;
    private readonly List<BattleMessage> _recentMessages = new();
    private const int MaxMessages = 10;

    public PlayerConsole(SideId sideId, PlayerOptions options, IBattleController battleController)
    {
        SideId = sideId;
        Options = options;
        BattleController = battleController;
        PrintDebug = options.PrintDebug;

        // Set console to UTF-8 encoding to support Unicode characters
        try
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }
        catch
        {
            // If UTF-8 fails, fallback to default encoding
        }

        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText("Pokemon Battle")
            .Centered()
            .Color(Color.Cyan1));
        AnsiConsole.WriteLine();

    }

    public async Task<Choice> GetNextChoiceAsync(IChoiceRequest choiceRequest,
        BattleRequestType requestType, BattlePerspective perspective,
        CancellationToken cancellationToken)
    {
        _currentPerspective = perspective;

        // Fire the choice requested event
        ChoiceRequested?.Invoke(this, new ChoiceRequestEventArgs
        {
            Choice = choiceRequest,
            TimeLimit = TimeSpan.FromSeconds(90),
            RequestTime = DateTime.UtcNow,
        });

        // Render current battle state
        RenderBattleState(perspective);

        // Get choice based on request type
        Choice choice = requestType switch
        {
            BattleRequestType.TeamPreview when choiceRequest is TeamPreviewRequest tpr =>
                await GetTeamPreviewChoiceAsync(tpr, cancellationToken),
            BattleRequestType.TurnStart when choiceRequest is MoveRequest mr =>
                await GetMoveChoiceAsync(mr, cancellationToken),
            BattleRequestType.ForceSwitch when choiceRequest is SwitchRequest sr =>
                await GetSwitchChoiceAsync(sr, cancellationToken),
            _ => throw new NotImplementedException($"Request type {requestType} not implemented")
        };

        // Fire the choice submitted event
        ChoiceSubmitted?.Invoke(this, choice);

        return choice;
    }

    public Choice GetChoiceSync(IChoiceRequest choice, BattleRequestType requestType,
        BattlePerspective perspective)
    {
        throw new NotSupportedException(
            "Console player requires async input and cannot be used in synchronous mode");
    }

    public void UpdateUi(BattlePerspective perspective)
    {
        _currentPerspective = perspective;
    }

    public void UpdateMessages(IEnumerable<BattleMessage> messages)
    {
        foreach (var message in messages)
        {
            _recentMessages.Add(message);
        }

        // Keep only recent messages
        while (_recentMessages.Count > MaxMessages)
        {
            _recentMessages.RemoveAt(0);
        }

        // Display messages
        foreach (var message in messages)
        {
            var text = message.ToDisplayText();
            if (!string.IsNullOrEmpty(text))
            {
                AnsiConsole.MarkupLine($"[grey]{text}[/]");
            }
        }
    }

    private void RenderBattleState(BattlePerspective perspective)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey);

        table.AddColumn(new TableColumn("[bold yellow]Opponent[/]").Centered());
        table.AddColumn(new TableColumn("[bold cyan]Your Team[/]").Centered());

        // Opponent's active Pokemon
        var opponentActive = perspective.OpponentSide.Active.FirstOrDefault();
        var opponentInfo = opponentActive != null
            ? $"[bold]{opponentActive.Name}[/]\nHP: {RenderHealthBar(opponentActive.HpPercentage)}"
            : "[grey]No active Pokemon[/]";

        // Player's active Pokemon - handle fainted Pokemon properly
        var playerActive = perspective.PlayerSide.Active.FirstOrDefault();
        string playerInfo;
        if (playerActive != null)
        {
            if (playerActive.Fainted)
            {
                playerInfo = $"[bold]{playerActive.Name}[/]\n[red]Fainted[/]";
            }
            else
            {
                playerInfo =
                    $"[bold]{playerActive.Name}[/]\nHP: {RenderHealthBar(playerActive.Hp, playerActive.MaxHp)}\n{playerActive.Hp}/{playerActive.MaxHp} HP";
            }
        }
        else
        {
            playerInfo = "[grey]No active Pokemon[/]";
        }

        table.AddRow(opponentInfo, playerInfo);

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private string RenderHealthBar(double hpPercentage)
    {
        var barLength = 20;
        var filled = (int)(barLength * (hpPercentage / 100.0));
        var empty = barLength - filled;

        var color = hpPercentage > 50 ? "green" : hpPercentage > 20 ? "yellow" : "red";

        // Use ASCII-safe characters that work in all console encodings
        // = for health, - for damage
        var bar = $"[{color}]{"=".Repeat(filled)}[/]{"-".Repeat(empty)}";

        return $"{bar} {hpPercentage:F1}%";
    }

    private string RenderHealthBar(int hp, int maxHp)
    {
        var percentage = (double)hp / maxHp * 100;
        return RenderHealthBar(percentage);
    }

    private async Task<Choice> GetTeamPreviewChoiceAsync(TeamPreviewRequest request,
        CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine("[bold cyan]Team Preview[/]");
        AnsiConsole.MarkupLine("Select the order for your Pokemon (1-6):");
        AnsiConsole.WriteLine();

        var pokemon = request.Side.Pokemon;
        for (int i = 0; i < pokemon.Count; i++)
        {
            // Display the Pokemon's actual name from Details (e.g., "Pikachu, L50")
            AnsiConsole.MarkupLine($"  {i + 1}. [bold]{pokemon[i].Details}[/]");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Press Enter to use default order (1,2,3,4,5,6)[/]");

        Console.ReadLine(); // Wait for user

        // Return default order
        var actions = Enumerable.Range(0, pokemon.Count)
            .Select((index, position) => new ChosenAction
            {
                Choice = ChoiceType.Team,
                Pokemon = null,
                MoveId = Sim.Moves.MoveId.None,
                Index = index,
                Priority = -position
            }).ToList();

        return new Choice
        {
            Actions = actions
        };
    }

    private async Task<Choice> GetMoveChoiceAsync(MoveRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Active.Count == 0)
        {
            throw new InvalidOperationException("No active Pokemon to make a move choice");
        }

        var pokemonRequest = request.Active[0];

        AnsiConsole.MarkupLine("[bold cyan]Select your move:[/]");
        AnsiConsole.WriteLine();

        var choices = new List<string>();
        var moveIndices = new List<int>();

        // Add moves
        for (int i = 0; i < pokemonRequest.Moves.Count; i++)
        {
            var move = pokemonRequest.Moves[i];
            var disabled = IsDisabled(move.Disabled);

            if (!disabled)
            {
                // Just show move name without PP (not available in data structure)
                choices.Add($"{i + 1}. {move.Move.Name}");
                moveIndices.Add(i);
            }
        }

        // Add switch option
        choices.Add("S. Switch Pokemon");

        // Run the blocking prompt on a background thread to avoid blocking the async pipeline
        var selection = await Task.Run(() => AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose an action:")
                .AddChoices(choices)), cancellationToken);

        if (selection.StartsWith("S"))
        {
            // Switch - show available Pokemon  
            return await GetSwitchChoiceAsync(
                new SwitchRequest { Side = request.Side, ForceSwitch = new[] { false } },
                cancellationToken);
        }

        // Move selected
        var moveIndex = moveIndices[choices.IndexOf(selection)];
        var selectedMove = pokemonRequest.Moves[moveIndex];

        return new Choice
        {
            Actions = new List<ChosenAction>
            {
                new()
                {
                    Choice = ChoiceType.Move,
                    Pokemon = null,
                    MoveId = selectedMove.Id,
                    TargetLoc = 0
                }
            }
        };
    }

    private async Task<Choice> GetSwitchChoiceAsync(SwitchRequest request,
        CancellationToken cancellationToken)
    {
        // Build list of available Pokemon with their original indices
        var availablePokemonWithIndex = request.Side.Pokemon
            .Select((p, index) => new { PokemonData = p, OriginalIndex = index })
            .Where(x => !x.PokemonData.Active)
            .ToList();

        if (availablePokemonWithIndex.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No Pokemon available to switch![/]");
            throw new InvalidOperationException("No Pokemon available to switch");
        }

        AnsiConsole.MarkupLine("[bold cyan]Select Pokemon to switch to:[/]");
        AnsiConsole.WriteLine();

        // Build display choices
        var choices = availablePokemonWithIndex
            .Select((item, i) =>
            {
                var p = item.PokemonData;
                // Parse the Condition (format: "HP/MaxHP status" or "HP/MaxHP" or "0 fnt")
                string hpDisplay;
                string conditionStr = p.Condition.ToString();
                if (string.IsNullOrEmpty(conditionStr) || conditionStr == "None")
                {
                    hpDisplay = "HP: Unknown";
                }
                else
                {
                    // Condition is in format like "205/205" or "23/205 psn" or "0 fnt"
                    hpDisplay = $"HP: {conditionStr}";
                }

                return $"{i + 1}. {p.Details} ({hpDisplay})";
            })
            .ToList();

        // Run the blocking prompt on a background thread
        var selection = await Task.Run(() => AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose a Pokemon:")
                .AddChoices(choices)), cancellationToken);

        var selectedDisplayIndex = choices.IndexOf(selection);
        var selectedItem = availablePokemonWithIndex[selectedDisplayIndex];

        // The target should be identified by the original index in the Side.Pokemon list
        // The Side.Choose method will look up the Pokemon by this index
        return new Choice
        {
            Actions = new List<ChosenAction>
            {
                new()
                {
                    Choice = ChoiceType.Switch,
                    Pokemon = null,
                    MoveId = Sim.Moves.MoveId.None,
                    Index = selectedItem.OriginalIndex
                }
            }
        };
    }

    private bool IsDisabled(object? disabled)
    {
        return disabled switch
        {
            Sim.Utils.Unions.BoolMoveIdBoolUnion boolUnion => boolUnion.Value,
            _ => false
        };
    }

    public event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    public event EventHandler<Choice>? ChoiceSubmitted;

    public Task NotifyTimeoutWarningAsync(TimeSpan remainingTime)
    {
        AnsiConsole.MarkupLine(
            $"[yellow]Warning: {remainingTime.TotalSeconds:F0} seconds remaining![/]");
        return Task.CompletedTask;
    }

    public Task NotifyChoiceTimeoutAsync()
    {
        AnsiConsole.MarkupLine("[red]Time's up! Auto-selecting...[/]");
        return Task.CompletedTask;
    }
}

// Helper extension
internal static class StringExtensions
{
    public static string Repeat(this string s, int count)
    {
        return string.Concat(Enumerable.Repeat(s, count));
    }
}