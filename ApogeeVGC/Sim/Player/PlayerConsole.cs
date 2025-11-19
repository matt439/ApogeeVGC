using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;
using Spectre.Console;

namespace ApogeeVGC.Sim.Player;

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
        var battleMessages = messages as BattleMessage[] ?? messages.ToArray();
        foreach (BattleMessage message in battleMessages)
        {
            _recentMessages.Add(message);
        }

        // Keep only recent messages
        while (_recentMessages.Count > MaxMessages)
        {
            _recentMessages.RemoveAt(0);
        }

        // Display messages
        foreach (BattleMessage message in battleMessages)
        {
            string text = message.ToDisplayText();
            if (!string.IsNullOrEmpty(text))
            {
                AnsiConsole.MarkupLine($"[grey]{text}[/]");
            }
        }
    }

    private void RenderBattleState(BattlePerspective perspective)
    {
        // Display field state if any conditions are active
        string fieldState = GetFieldStateDisplay(perspective.Field);
        if (!string.IsNullOrEmpty(fieldState))
        {
            AnsiConsole.MarkupLine($"[bold]{fieldState}[/]");
            AnsiConsole.WriteLine();
        }

        // Display side conditions for both sides
        string opponentSideState = GetSideConditionsDisplay(perspective.OpponentSide.SideConditionsWithDuration, "Opponent");
        if (!string.IsNullOrEmpty(opponentSideState))
        {
            AnsiConsole.MarkupLine($"[bold]{opponentSideState}[/]");
        }

        string playerSideState = GetSideConditionsDisplay(perspective.PlayerSide.SideConditionsWithDuration, "Your Team");
        if (!string.IsNullOrEmpty(playerSideState))
        {
            AnsiConsole.MarkupLine($"[bold]{playerSideState}[/]");
        }

        if (!string.IsNullOrEmpty(opponentSideState) || !string.IsNullOrEmpty(playerSideState))
        {
            AnsiConsole.WriteLine();
        }

        Table table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey);

        table.AddColumn(new TableColumn("[bold yellow]Opponent[/]").Centered());
        table.AddColumn(new TableColumn("[bold cyan]Your Team[/]").Centered());

        // Opponent's active Pokemon
        PokemonPerspective? opponentActive = perspective.OpponentSide.Active.FirstOrDefault();
        string opponentInfo;
        if (opponentActive != null)
        {
            string statusDisplay = GetStatusDisplay(opponentActive.Status);
            string statusLine = !string.IsNullOrEmpty(statusDisplay) ? $"\n{statusDisplay}" : "";

            string volatileDisplay = GetVolatilesDisplay(opponentActive.VolatilesWithDuration);
            string volatilesLine =
                !string.IsNullOrEmpty(volatileDisplay) ? $"\n{volatileDisplay}" : "";

            string boostsDisplay = GetStatBoostsDisplay(opponentActive.Boosts);
            string boostsLine = $"\n{boostsDisplay}";

            string teraDisplay =
                GetTeraDisplay(opponentActive.Terastallized, opponentActive.TeraType);
            string teraLine = !string.IsNullOrEmpty(teraDisplay) ? $"\n{teraDisplay}" : "";

            opponentInfo =
                $"[bold]{opponentActive.Name}[/]\nHP: {RenderHealthBar(opponentActive.Hp, opponentActive.MaxHp)}\n{opponentActive.Hp}/{opponentActive.MaxHp} HP{statusLine}{volatilesLine}{boostsLine}{teraLine}";
        }
        else
        {
            opponentInfo = "[grey]No active Pokemon[/]";
        }

        // Player's active Pokemon - handle fainted Pokemon properly
        PokemonPerspective? playerActive = perspective.PlayerSide.Active.FirstOrDefault();
        string playerInfo;
        if (playerActive != null)
        {
            if (playerActive.Fainted)
            {
                playerInfo = $"[bold]{playerActive.Name}[/]\n[red]Fainted[/]";
            }
            else
            {
                string statusDisplay = GetStatusDisplay(playerActive.Status);
                string statusLine =
                    !string.IsNullOrEmpty(statusDisplay) ? $"\n{statusDisplay}" : "";

                string volatileDisplay = GetVolatilesDisplay(playerActive.VolatilesWithDuration);
                string volatilesLine = !string.IsNullOrEmpty(volatileDisplay)
                    ? $"\n{volatileDisplay}"
                    : "";

                string boostsDisplay = GetStatBoostsDisplay(playerActive.Boosts);
                string boostsLine = $"\n{boostsDisplay}";

                string teraDisplay =
                    GetTeraDisplay(playerActive.Terastallized, playerActive.TeraType);
                string teraLine = !string.IsNullOrEmpty(teraDisplay) ? $"\n{teraDisplay}" : "";

                playerInfo =
                    $"[bold]{playerActive.Name}[/]\nHP: {RenderHealthBar(playerActive.Hp, playerActive.MaxHp)}\n{playerActive.Hp}/{playerActive.MaxHp} HP{statusLine}{volatilesLine}{boostsLine}{teraLine}";
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

    private static string RenderHealthBar(double hpPercentage)
    {
        const int barLength = 20;
        int filled = (int)(barLength * (hpPercentage / 100.0));
        int empty = barLength - filled;

        string color = hpPercentage > 50 ? "green" : hpPercentage > 20 ? "yellow" : "red";

        // Use ASCII-safe characters that work in all console encoding
        // = for health, - for damage
        string bar = $"[{color}]{"=".Repeat(filled)}[/]{"-".Repeat(empty)}";

        return $"{bar} {hpPercentage:F1}%";
    }

    private static string RenderHealthBar(int hp, int maxHp)
    {
        double percentage = (double)hp / maxHp * 100;
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
        AnsiConsole.MarkupLine(
            "[grey]Enter a single number (e.g., 2) to lead with that Pokemon[/]");
        AnsiConsole.MarkupLine("[grey]Or enter custom order (e.g., 2,1,3,4,5,6)[/]");

        string? input = Console.ReadLine();

        // Parse custom order if provided
        List<int> order;
        if (!string.IsNullOrWhiteSpace(input))
        {
            try
            {
                // Check if input contains comma (full custom order) or single number
                if (input.Contains(','))
                {
                    // Parse comma-separated indices (1-based)
                    order = input.Split(',')
                        .Select(s => int.Parse(s.Trim()) - 1) // Convert to 0-based
                        .ToList();

                    // Validate order
                    if (order.Count != pokemon.Count ||
                        order.Any(i => i < 0 || i >= pokemon.Count) ||
                        order.Distinct().Count() != order.Count)
                    {
                        AnsiConsole.MarkupLine(
                            "[yellow]Invalid order, using default (1,2,3,4,5,6)[/]");
                        order = Enumerable.Range(0, pokemon.Count).ToList();
                    }
                    else
                    {
                        AnsiConsole.MarkupLine(
                            $"[green]Using order: {string.Join(",", order.Select(i => i + 1))}[/]");
                    }
                }
                else
                {
                    // Single integer - use as lead, fill rest in ascending order
                    int leadIndex = int.Parse(input.Trim()) - 1; // Convert to 0-based

                    if (leadIndex < 0 || leadIndex >= pokemon.Count)
                    {
                        AnsiConsole.MarkupLine(
                            "[yellow]Invalid Pokemon number, using default (1,2,3,4,5,6)[/]");
                        order = Enumerable.Range(0, pokemon.Count).ToList();
                    }
                    else
                    {
                        // Start with the lead Pokemon, then add all others in ascending order
                        order = [leadIndex];
                        for (int i = 0; i < pokemon.Count; i++)
                        {
                            if (i != leadIndex)
                            {
                                order.Add(i);
                            }
                        }

                        AnsiConsole.MarkupLine(
                            $"[green]Leading with {pokemon[leadIndex].Details}, order: {string.Join(",", order.Select(i => i + 1))}[/]");
                    }
                }
            }
            catch
            {
                AnsiConsole.MarkupLine(
                    "[yellow]Invalid input, using default order (1,2,3,4,5,6)[/]");
                order = Enumerable.Range(0, pokemon.Count).ToList();
            }
        }
        else
        {
            // Use default order
            order = Enumerable.Range(0, pokemon.Count).ToList();
        }

        // Build actions based on the selected order
        // order[newPosition] = originalPokemonIndex
        // For each new position, create an action with:
        //   - Index = newPosition (where this Pokemon will be placed in the team)
//   - TargetLoc = originalPokemonIndex (which Pokemon from the original team to use)
        //   - Priority = -newPosition (earlier positions have higher priority for sorting)
        //   - Pokemon = null (will be set by ProcessChosenTeamAction using TargetLoc)
        var actions = order.Select((originalPokemonIndex, newPosition) => new ChosenAction
        {
            Choice = ChoiceType.Team,
            Pokemon = null, // Will be set by ProcessChosenTeamAction
            MoveId = MoveId.None,
            Index = newPosition, // The new position in the team (0, 1, 2, ...)
            TargetLoc = originalPokemonIndex, // Which Pokemon from the original team
            Priority = -newPosition, // Sorting priority (0, -1, -2, ... so 0 is highest)
        }).ToList();

        return new Choice
        {
            Actions = actions,
        };
    }

    private async Task<Choice> GetMoveChoiceAsync(MoveRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Active.Count == 0)
        {
            throw new InvalidOperationException("No active Pokemon to make a move choice");
        }

        PokemonMoveRequestData pokemonRequest = request.Active[0];

        AnsiConsole.MarkupLine("[bold cyan]Select your move:[/]");
        AnsiConsole.WriteLine();

        var choices = new List<string>();
        var moveIndices = new List<int>();
        var teraOptions = new List<bool>();

        // Check if terastallization is available
        MoveType? teraType = pokemonRequest.CanTerastallize switch
        {
            MoveTypeMoveTypeFalseUnion mtfu => mtfu.MoveType,
            _ => null,
        };

        // Add moves
        for (int i = 0; i < pokemonRequest.Moves.Count; i++)
        {
            PokemonMoveData move = pokemonRequest.Moves[i];
            bool disabled = IsDisabled(move.Disabled);

            if (!disabled)
            {
                // Add regular move option
                choices.Add($"{i + 1}. {move.Move.Name}");
                moveIndices.Add(i);
                teraOptions.Add(false);

                // Add terastallize variant if available
                if (teraType.HasValue)
                {
                    choices.Add(
                        $"{i + 1}. {move.Move.Name} [bold {GetTeraTypeColor(teraType.Value)}](+ TERA {teraType.Value.ToString().ToUpper()})[/]");
                    moveIndices.Add(i);
                    teraOptions.Add(true);
                }
            }
        }

        // Add switch option
        choices.Add("S. Switch Pokemon");

        // Run the blocking prompt on a background thread to avoid blocking the async pipeline
        string selection = await Task.Run(() => AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose an action:")
                .AddChoices(choices)), cancellationToken);

        if (selection.StartsWith('S'))
        {
            // Switch - show available Pokemon  
            return await GetSwitchChoiceAsync(
                new SwitchRequest { Side = request.Side, ForceSwitch = [false] },
                cancellationToken);
        }

        // Move selected
        int choiceIndex = choices.IndexOf(selection);
        int moveIndex = moveIndices[choiceIndex];
        bool useTerastallize = teraOptions[choiceIndex];
        PokemonMoveData selectedMove = pokemonRequest.Moves[moveIndex];

        // Display which move was selected
        AnsiConsole.MarkupLine(
            useTerastallize
                ? $"[bold cyan]Selected: {selectedMove.Move.Name} with Terastallization ({teraType})[/]"
                : $"[bold cyan]Selected: {selectedMove.Move.Name}[/]");

        AnsiConsole.WriteLine();

        return new Choice
        {
            Actions = new List<ChosenAction>
            {
                new()
                {
                    Choice = ChoiceType.Move,
                    Pokemon = null,
                    MoveId = selectedMove.Id,
                    TargetLoc = 0,
                    Terastallize = useTerastallize ? teraType : null
                }
            }
        };
    }

    private async Task<Choice> GetSwitchChoiceAsync(SwitchRequest request,
        CancellationToken cancellationToken)
    {
        // Build list of available Pokemon with their original indices, excluding fainted ones
        var availablePokemonWithIndex = request.Side.Pokemon
            .Select((p, index) => new { PokemonData = p, OriginalIndex = index })
            .Where(x => !x.PokemonData.Active && !IsPokemonFainted(x.OriginalIndex))
            .ToList();

        if (availablePokemonWithIndex.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No Pokemon available to switch![/]");
            throw new InvalidOperationException("No Pokemon available to switch");
        }

        AnsiConsole.MarkupLine("[bold cyan]Select Pokemon to switch to:[/]");
        AnsiConsole.WriteLine();

        // Build display choices with HP information from current perspective
        var choices = availablePokemonWithIndex
            .Select((item, i) =>
            {
                PokemonSwitchRequestData p = item.PokemonData;

                // Get current HP from perspective if available
                string hpDisplay;
                if (_currentPerspective != null)
                {
                    // Find matching Pokemon in perspective by position/index
                    PokemonPerspective? perspectivePokemon = _currentPerspective.PlayerSide.Pokemon
                        .FirstOrDefault(pp => pp.Position == item.OriginalIndex);

                    if (perspectivePokemon != null)
                    {
                        hpDisplay = perspectivePokemon.Fainted
                            ? "Fainted"
                            : $"{perspectivePokemon.Hp}/{perspectivePokemon.MaxHp}";
                    }
                    else
                    {
                        // Fallback to max HP from stats
                        int maxHp = p.Stats.Hp;
                        hpDisplay = $"{maxHp}/{maxHp}";
                    }
                }
                else
                {
                    // Fallback to max HP from stats if no perspective
                    int maxHp = p.Stats.Hp;
                    hpDisplay = $"{maxHp}/{maxHp}";
                }

                return $"{i + 1}. {p.Details} (HP: {hpDisplay})";
            })
            .ToList();

        // Run the blocking prompt on a background thread
        string selection = await Task.Run(() => AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose a Pokemon:")
                .AddChoices(choices)), cancellationToken);

        int selectedDisplayIndex = choices.IndexOf(selection);
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
                    MoveId = MoveId.None,
                    Index = selectedItem.OriginalIndex,
                }
            }
        };
    }

    /// <summary>
    /// Helper method to check if a Pokemon is fainted based on the current perspective
    /// </summary>
    private bool IsPokemonFainted(int pokemonIndex)
    {
        if (_currentPerspective == null)
            return false;

        PokemonPerspective? perspectivePokemon = _currentPerspective.PlayerSide.Pokemon
            .FirstOrDefault(pp => pp.Position == pokemonIndex);

        return perspectivePokemon?.Fainted ?? false;
    }

    /// <summary>
    /// Get status condition display markup for console
    /// </summary>
    private string GetStatusDisplay(ConditionId status)
    {
        return status switch
        {
            ConditionId.Burn => "[orange3]BRN[/]",
            ConditionId.Paralysis => "[yellow]PAR[/]",
            ConditionId.Sleep => "[purple]SLP[/]",
            ConditionId.Freeze => "[cyan]FRZ[/]",
            ConditionId.Poison => "[magenta]PSN[/]",
            ConditionId.Toxic => "[darkmagenta]TOX[/]",
            _ => "",
        };
    }

    /// <summary>
    /// Get volatile conditions display with durations
    /// Shows all volatile conditions including hidden ones like Stall, Protect, etc.
    /// </summary>
    private string GetVolatilesDisplay(IReadOnlyDictionary<ConditionId, int?> volatilesWithDuration)
    {
        if (volatilesWithDuration.Count == 0)
        {
            return "";
        }

        var volatileStrings = new List<string>();

        foreach ((ConditionId conditionId, int? duration) in volatilesWithDuration)
        {
            string conditionName = GetConditionDisplayName(conditionId);
            string durationText = duration.HasValue ? $":{duration}" : "";
            string color = GetConditionColor(conditionId);

            volatileStrings.Add($"[{color}]{conditionName}{durationText}[/]");
        }

        return string.Join(" ", volatileStrings);
    }

    /// <summary>
    /// Get a friendly display name for a condition
    /// </summary>
    private string GetConditionDisplayName(ConditionId condition)
    {
        return condition switch
        {
            ConditionId.Stall => "Stall",
            ConditionId.Protect => "Protect",
            ConditionId.Confusion => "Confused",
            ConditionId.Substitute => "Sub",
            ConditionId.LeechSeed => "Seeded",
            ConditionId.ChoiceLock => "ChoiceLock",
            ConditionId.HealBlock => "HealBlock",
            ConditionId.Embargo => "Embargo",
            ConditionId.Ingrain => "Ingrain",
            ConditionId.MagnetRise => "MagRise",
            ConditionId.Telekinesis => "Telekns",
            ConditionId.PartiallyTrapped => "Trapped",
            ConditionId.FocusEnergy => "FocusEng",
            ConditionId.Nightmare => "Nightmare",
            ConditionId.GastroAcid => "GastroAcd",
            ConditionId.Yawn => "Yawn",
            ConditionId.Tailwind => "Tailwind",
            ConditionId.Reflect => "Reflect",
            ConditionId.LightScreen => "LightScreen",
            ConditionId.TrickRoom => "TrickRoom",
            ConditionId.Gravity => "Gravity",
            ConditionId.MagicRoom => "MagicRoom",
            ConditionId.WonderRoom => "WonderRoom",
            ConditionId.SmackDown => "SmackDown",
            ConditionId.Roost => "Roost",
            ConditionId.QuarkDrive => "QuarkDrive",
            ConditionId.Commanding => "Commanding",
            ConditionId.Commanded => "Commanded",
            ConditionId.Detect => "Detect",
            ConditionId.MaxGuard => "MaxGuard",
            ConditionId.KingsShield => "KingsShield",
            ConditionId.SpikyShield => "SpikyShield",
            ConditionId.BanefulBunker => "BanefulBunker",
            ConditionId.Obstruct => "Obstruct",
            ConditionId.SilkTrap => "SilkTrap",
            ConditionId.BurningBulwark => "BurningBulwark",
            ConditionId.QuickGuard => "QuickGuard",
            ConditionId.WideGuard => "WideGuard",
            ConditionId.LaserFocus => "LaserFocus",
            ConditionId.ShedTail => "ShedTail",
            ConditionId.DragonCheer => "DragonCheer",
            ConditionId.Rest => "Rest",
            ConditionId.Wish => "Wish",
            ConditionId.IceBall => "IceBall",
            ConditionId.Rollout => "Rollout",
            ConditionId.Fly => "Fly",
            ConditionId.Bounce => "Bounce",
            ConditionId.Dive => "Dive",
            ConditionId.Dig => "Dig",
            ConditionId.PhantomForce => "PhantomForce",
            ConditionId.ShadowForce => "ShadowForce",
            ConditionId.SkyDrop => "SkyDrop",
            _ => condition.ToString()
        };
    }

    /// <summary>
    /// Get color for condition display
    /// </summary>
    private string GetConditionColor(ConditionId condition)
    {
        return condition switch
        {
            ConditionId.Stall or ConditionId.Protect or ConditionId.Detect or
                ConditionId.MaxGuard or ConditionId.KingsShield or ConditionId.SpikyShield or
                ConditionId.BanefulBunker or ConditionId.Obstruct or ConditionId.SilkTrap or
                ConditionId.BurningBulwark or ConditionId.QuickGuard
                or ConditionId.WideGuard => "blue",
            ConditionId.Confusion => "yellow",
            ConditionId.Substitute or ConditionId.ShedTail => "green",
            ConditionId.LeechSeed => "darkgreen",
            ConditionId.ChoiceLock => "red",
            ConditionId.HealBlock or ConditionId.Embargo => "darkred",
            ConditionId.Ingrain or ConditionId.Wish => "cyan",
            ConditionId.FocusEnergy or ConditionId.LaserFocus or ConditionId.DragonCheer =>
                "orange",
            ConditionId.Tailwind or ConditionId.QuarkDrive => "yellow",
            ConditionId.Reflect or ConditionId.LightScreen => "lightblue",
            ConditionId.TrickRoom or ConditionId.Gravity or ConditionId.MagicRoom
                or ConditionId.WonderRoom => "purple",
            _ => "grey"
        };
    }

    /// <summary>
    /// Get stat boosts display showing all stats with their current values
    /// Shows format: Atk+2 Def+0 SpA-1 SpD+0 Spe+0 Acc+0 Eva+0
    /// </summary>
    private string GetStatBoostsDisplay(Stats.BoostsTable boosts)
    {
        var boostStrings = new List<string>
        {
            // Format: StatName+Value or StatName-Value
            FormatStatBoost("Atk", boosts.Atk),
            FormatStatBoost("Def", boosts.Def),
            FormatStatBoost("SpA", boosts.SpA),
            FormatStatBoost("SpD", boosts.SpD),
            FormatStatBoost("Spe", boosts.Spe),
            FormatStatBoost("Acc", boosts.Accuracy),
            FormatStatBoost("Eva", boosts.Evasion),
        };

        return string.Join(" ", boostStrings);
    }

    /// <summary>
    /// Format a single stat boost with appropriate color
    /// </summary>
    private string FormatStatBoost(string statName, int boostValue)
    {
        string color = boostValue switch
        {
            > 0 => "green",
            < 0 => "red",
            _ => "grey"
        };

        string sign = boostValue switch
        {
            > 0 => "+",
            < 0 => "", // Negative sign is automatic
            _ => "="
        };

        return $"[{color}]{statName}{sign}{boostValue}[/]";
    }

    /// <summary>
    /// Get field state display showing Weather, Terrain, and PseudoWeather with durations
    /// </summary>
    private string GetFieldStateDisplay(FieldClasses.FieldPerspective fieldPerspective)
    {
        var fieldParts = new List<string>();

        // Weather
        if (fieldPerspective.Weather != ConditionId.None)
        {
            string weatherName = GetFieldConditionDisplayName(fieldPerspective.Weather);
            string duration = fieldPerspective.WeatherDuration.HasValue
                ? $":{fieldPerspective.WeatherDuration}"
                : "";
            fieldParts.Add($"[yellow]{weatherName}{duration}[/]");
        }

        // Terrain
        if (fieldPerspective.Terrain != ConditionId.None)
        {
            string terrainName = GetFieldConditionDisplayName(fieldPerspective.Terrain);
            string duration = fieldPerspective.TerrainDuration.HasValue
                ? $":{fieldPerspective.TerrainDuration}"
                : "";
            fieldParts.Add($"[green]{terrainName}{duration}[/]");
        }

        // PseudoWeather
        foreach ((ConditionId conditionId, int? duration) in fieldPerspective
                     .PseudoWeatherWithDuration)
        {
            string pwName = GetFieldConditionDisplayName(conditionId);
            string durationText = duration.HasValue ? $":{duration}" : "";
            fieldParts.Add($"[cyan]{pwName}{durationText}[/]");
        }

        return fieldParts.Count > 0
            ? $"Field: {string.Join(" ", fieldParts)}"
            : "";
    }

    /// <summary>
    /// Get side conditions display showing screen moves and other side conditions with durations
    /// </summary>
    private string GetSideConditionsDisplay(IReadOnlyDictionary<ConditionId, int?> sideConditions, string sideName)
    {
        if (sideConditions.Count == 0)
        {
            return "";
        }

        var conditionParts = new List<string>();

        foreach ((ConditionId conditionId, int? duration) in sideConditions)
        {
            string conditionName = GetSideConditionDisplayName(conditionId);
            string durationText = duration.HasValue ? $":{duration}" : "";
            string color = GetSideConditionColor(conditionId);
            conditionParts.Add($"[{color}]{conditionName}{durationText}[/]");
        }

        return conditionParts.Count > 0
            ? $"{sideName}: {string.Join(" ", conditionParts)}"
            : "";
    }

    /// <summary>
    /// Get display name for field conditions (Weather, Terrain, PseudoWeather)
    /// </summary>
    private string GetFieldConditionDisplayName(ConditionId condition)
    {
        return condition switch
        {
            // Weather
            ConditionId.SunnyDay => "Sun",
            ConditionId.RainDance => "Rain",
            ConditionId.DesolateLand => "HarshSun",
            ConditionId.PrimordialSea => "HeavyRain",

            // Terrain
            ConditionId.ElectricTerrain => "Electric",

            // PseudoWeather
            ConditionId.TrickRoom => "TrickRoom",
            ConditionId.Gravity => "Gravity",
            ConditionId.MagicRoom => "MagicRoom",
            ConditionId.WonderRoom => "WonderRoom",

            _ => condition.ToString()
        };
    }

    /// <summary>
    /// Get display name for side conditions (Reflect, Light Screen, Tailwind, etc.)
    /// </summary>
    private string GetSideConditionDisplayName(ConditionId condition)
    {
        return condition switch
        {
            ConditionId.Reflect => "Reflect",
            ConditionId.LightScreen => "Light Screen",
            ConditionId.Tailwind => "Tailwind",
            _ => condition.ToString()
        };
    }

    /// <summary>
    /// Get color for side condition display
    /// </summary>
    private string GetSideConditionColor(ConditionId condition)
    {
        return condition switch
        {
            ConditionId.Reflect or ConditionId.LightScreen => "cyan",
            ConditionId.Tailwind => "yellow",
            _ => "grey"
        };
    }

    /// <summary>
    /// Get terastallization display markup for console
    /// Shows "TERA: [Type]" if terastallized, or "Tera: [Type]" if available but not active
    /// </summary>
    private string GetTeraDisplay(MoveType? terastallized, MoveType teraType)
    {
        if (terastallized.HasValue)
        {
            // Pokemon is currently terastallized - show in bold with color
            return
                $"[bold {GetTeraTypeColor(terastallized.Value)}]TERA: {terastallized.Value.ToString().ToUpper()}[/]";
        }
        else
        {
            // Pokemon has tera available but not active - show in grey
            return $"[grey]Tera: {teraType.ToString()}[/]";
        }
    }

    /// <summary>
    /// Get the console color for a tera type
    /// </summary>
    private string GetTeraTypeColor(MoveType teraType)
    {
        return teraType switch
        {
            MoveType.Normal => "white",
            MoveType.Fire => "red",
            MoveType.Water => "blue",
            MoveType.Electric => "yellow",
            MoveType.Grass => "green",
            MoveType.Ice => "cyan",
            MoveType.Fighting => "darkorange",
            MoveType.Poison => "purple",
            MoveType.Ground => "gold3",
            MoveType.Flying => "deepskyblue1",
            MoveType.Psychic => "magenta",
            MoveType.Bug => "greenyellow",
            MoveType.Rock => "orange3",
            MoveType.Ghost => "purple",
            MoveType.Dragon => "blue",
            MoveType.Dark => "grey",
            MoveType.Steel => "grey74",
            MoveType.Fairy => "hotpink",
            _ => "white",
        };
    }

    private bool IsDisabled(object? disabled)
    {
        return disabled switch
        {
            BoolMoveIdBoolUnion boolUnion => boolUnion.Value,
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