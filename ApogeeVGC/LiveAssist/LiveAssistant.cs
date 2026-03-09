using System.Text.Json;
using System.Text.RegularExpressions;
using ApogeeVGC.Data;
using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;
using Spectre.Console;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// Interactive live-assist mode for real Pokemon Scarlet/Violet battles.
/// User inputs battle state from their Switch screen, model recommends actions.
/// </summary>
public sealed class LiveAssistant
{
    private const string TeamSaveDir = "LiveAssist/teams";

    private readonly Library _library;
    private readonly Vocab _vocab;
    private readonly ModelInference _battleModel;
    private readonly TeamPreviewInference _previewModel;
    private readonly NameResolver _resolver;
    private readonly LiveBattleState _state = new();

    public LiveAssistant(
        Library library, Vocab vocab,
        ModelInference battleModel, TeamPreviewInference previewModel)
    {
        _library = library;
        _vocab = vocab;
        _battleModel = battleModel;
        _previewModel = previewModel;
        _resolver = new NameResolver(library);
    }

    public void Run()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText("Apogee Live").Centered().Color(Color.Gold1));
        AnsiConsole.MarkupLine("[grey]Live battle assistant — input state, get AI recommendations[/]");
        AnsiConsole.WriteLine();

        // Phase 1: Setup own team
        SetupOwnTeam();

        // Game loop: can play multiple games with same team
        while (true)
        {
            // Phase 2: Team preview (new opponent each game)
            RunTeamPreview();

            // Phase 3: Battle loop
            RunBattleLoop();

            if (!AnsiConsole.Confirm("Play another game with the same team?"))
                break;

            // Reset battle state for next game (keep own team)
            ResetForNewGame();
        }

        AnsiConsole.MarkupLine("[green]Thanks for using Apogee Live![/]");
    }

    // ─────────────────────────────────────────────────────────────
    //  Phase 1: Own Team Setup
    // ─────────────────────────────────────────────────────────────

    private void SetupOwnTeam()
    {
        AnsiConsole.MarkupLine("[bold cyan]== Team Setup ==[/]");

        // Try auto-loading a saved team first
        if (TryAutoLoadTeam())
        {
            DisplayOwnTeam();
            if (AnsiConsole.Confirm("Use this team?"))
                return;
        }

        // Otherwise, offer import options
        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("How do you want to set up your team?")
                    .AddChoices("Paste from Showdown", "Load saved team", "Enter manually"));

            bool success = choice switch
            {
                "Paste from Showdown" => ImportShowdownPaste(),
                "Load saved team" => TryLoadTeam(),
                "Enter manually" => EnterTeamManually(),
                _ => false,
            };

            if (!success) continue;

            DisplayOwnTeam();
            if (AnsiConsole.Confirm("Use this team?"))
                break;
        }

        // Auto-save for future use
        SaveTeam();
    }

    private bool ImportShowdownPaste()
    {
        AnsiConsole.MarkupLine("[bold]Paste your team below (Showdown export format).[/]");
        AnsiConsole.MarkupLine("[grey]Paste all 6 Pokemon, then enter a blank line to finish.[/]");

        var lines = new List<string>();
        int consecutiveBlanks = 0;

        while (true)
        {
            string? line = Console.ReadLine();
            if (line == null) break;

            if (string.IsNullOrWhiteSpace(line))
            {
                consecutiveBlanks++;
                // Two consecutive blank lines = done (one blank line separates Pokemon)
                if (consecutiveBlanks >= 2) break;
                lines.Add("");
            }
            else
            {
                consecutiveBlanks = 0;
                lines.Add(line);
            }
        }

        return ParseShowdownPaste(string.Join('\n', lines));
    }

    /// <summary>
    /// Parses a Showdown team export paste into the battle state's MyTeam.
    /// Format per Pokemon:
    ///   Nickname (Species) @ Item   OR   Species @ Item
    ///   Ability: AbilityName
    ///   Tera Type: TypeName
    ///   EVs: 252 HP / 4 Atk / 252 SpD
    ///   Careful Nature
    ///   - Move 1
    ///   - Move 2
    ///   - Move 3
    ///   - Move 4
    /// </summary>
    private bool ParseShowdownPaste(string paste)
    {
        // Split into Pokemon blocks (separated by blank lines)
        var blocks = Regex.Split(paste.Trim(), @"\n\s*\n")
            .Where(b => !string.IsNullOrWhiteSpace(b))
            .ToList();

        if (blocks.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No Pokemon found in paste.[/]");
            return false;
        }

        if (blocks.Count > 6)
        {
            AnsiConsole.MarkupLine($"[yellow]Found {blocks.Count} Pokemon, using first 6.[/]");
            blocks = blocks.Take(6).ToList();
        }

        int parsed = 0;
        for (int i = 0; i < blocks.Count; i++)
        {
            if (ParseShowdownBlock(blocks[i], _state.MyTeam[i]))
                parsed++;
            else
                AnsiConsole.MarkupLine($"[red]Failed to parse Pokemon {i + 1}.[/]");
        }

        if (parsed < blocks.Count)
        {
            AnsiConsole.MarkupLine($"[yellow]Parsed {parsed}/{blocks.Count} Pokemon successfully.[/]");
            return parsed > 0 && AnsiConsole.Confirm("Continue with partial team?");
        }

        AnsiConsole.MarkupLine($"[green]Successfully parsed {parsed} Pokemon.[/]");
        return true;
    }

    private bool ParseShowdownBlock(string block, LivePokemonState pokemon)
    {
        var lines = block.Split('\n', StringSplitOptions.TrimEntries)
            .Where(l => !string.IsNullOrEmpty(l))
            .ToList();

        if (lines.Count == 0) return false;

        // Line 1: "Nickname (Species) @ Item"  or  "Species @ Item"  or  "Species"
        string firstLine = lines[0];

        // Extract item (after @)
        string? itemName = null;
        int atIdx = firstLine.LastIndexOf('@');
        if (atIdx >= 0)
        {
            itemName = firstLine[(atIdx + 1)..].Trim();
            firstLine = firstLine[..atIdx].Trim();
        }

        // Extract species: "Nickname (Species)" or just "Species"
        // Also handle gender suffix: "Species (M)" or "Species (F)"
        string speciesName;
        var parenMatch = Regex.Match(firstLine, @"^(.+?)\s*\((.+?)\)\s*$");
        if (parenMatch.Success)
        {
            string inParen = parenMatch.Groups[2].Value.Trim();
            // Check if the parenthesized part is a gender (M/F) rather than species
            if (inParen is "M" or "F")
            {
                speciesName = parenMatch.Groups[1].Value.Trim();
            }
            else
            {
                speciesName = inParen;
                // There might be gender after: "Nick (Species) (M)"
                // Already handled since we took the last parens... but Showdown format
                // puts nickname first, species in parens
            }
        }
        else
        {
            speciesName = firstLine.Trim();
        }

        if (!_resolver.TryResolveSpecies(speciesName, out var specieId))
        {
            AnsiConsole.MarkupLine($"[red]Unknown species: {speciesName}[/]");
            return false;
        }

        pokemon.Species = specieId;
        pokemon.Name = _resolver.GetSpeciesName(specieId, _library);

        if (itemName != null && _resolver.TryResolveItem(itemName, out var itemId))
            pokemon.Item = itemId;

        // Parse remaining lines
        int moveIdx = 0;
        for (int i = 1; i < lines.Count; i++)
        {
            string line = lines[i];

            if (line.StartsWith("Ability:", StringComparison.OrdinalIgnoreCase))
            {
                string abilityName = line["Ability:".Length..].Trim();
                if (_resolver.TryResolveAbility(abilityName, out var abilityId))
                    pokemon.Ability = abilityId;
            }
            else if (line.StartsWith("Tera Type:", StringComparison.OrdinalIgnoreCase))
            {
                string teraName = line["Tera Type:".Length..].Trim();
                if (Enum.TryParse<MoveType>(teraName, true, out var teraType))
                    pokemon.TeraType = teraType;
            }
            else if (line.StartsWith('-'))
            {
                string moveName = line[1..].Trim();
                if (moveIdx < 4 && _resolver.TryResolveMove(moveName, out var moveId))
                {
                    pokemon.Moves[moveIdx] = moveId;
                    moveIdx++;
                }
            }
            // EVs, Nature, IVs, Level — ignored for model inference (not encoded)
        }

        return true;
    }

    private bool EnterTeamManually()
    {
        AnsiConsole.MarkupLine("[bold]Enter your 6 Pokemon:[/]");

        for (int i = 0; i < 6; i++)
        {
            AnsiConsole.MarkupLine($"\n[bold yellow]Pokemon {i + 1}/6[/]");
            var pokemon = _state.MyTeam[i];

            pokemon.Species = PromptSpecies("Species");
            pokemon.Name = _resolver.GetSpeciesName(pokemon.Species, _library);
            pokemon.Ability = PromptAbility($"Ability for {pokemon.Name}");
            pokemon.Item = PromptItem($"Item for {pokemon.Name}");
            pokemon.TeraType = PromptTeraType($"Tera Type for {pokemon.Name}");

            for (int m = 0; m < 4; m++)
                pokemon.Moves[m] = PromptMove($"Move {m + 1}");
        }

        return true;
    }

    private void DisplayOwnTeam()
    {
        var table = new Table().Title("[bold]Your Team[/]").Border(TableBorder.Rounded);
        table.AddColumn("#");
        table.AddColumn("Pokemon");
        table.AddColumn("Ability");
        table.AddColumn("Item");
        table.AddColumn("Tera");
        table.AddColumn("Moves");

        for (int i = 0; i < 6; i++)
        {
            var p = _state.MyTeam[i];
            if (p.Species == default) continue;

            string moves = string.Join(", ", p.Moves
                .Where(m => m != MoveId.None)
                .Select(m => _resolver.GetMoveName(m, _library)));

            table.AddRow(
                (i + 1).ToString(),
                _resolver.GetSpeciesName(p.Species, _library),
                _resolver.GetAbilityName(p.Ability, _library),
                _resolver.GetItemName(p.Item, _library),
                p.TeraType.ToString(),
                moves);
        }

        AnsiConsole.Write(table);
    }

    // ─────────────────────────────────────────────────────────────
    //  Phase 2: Team Preview
    // ─────────────────────────────────────────────────────────────

    private void RunTeamPreview()
    {
        AnsiConsole.MarkupLine("\n[bold cyan]== Team Preview ==[/]");
        AnsiConsole.MarkupLine("[bold]Enter opponent's 6 Pokemon (species only):[/]");

        for (int i = 0; i < 6; i++)
        {
            var species = PromptSpecies($"Opponent Pokemon {i + 1}");
            _state.OppTeam[i].Species = species;
            _state.OppTeam[i].Name = _resolver.GetSpeciesName(species, _library);
        }

        // Run team preview model
        var perspective = _state.BuildTeamPreviewPerspective();
        var output = _previewModel.Evaluate(perspective);

        // Display recommendation
        DisplayTeamPreviewRecommendation(output);

        // User selects their 4 (in order: first 2 are leads)
        var indices = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter 4 Pokemon to bring (indices 1-6, first 2 are leads, e.g. '3 1 5 2')")
                .Validate(input =>
                {
                    var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 4) return ValidationResult.Error("Must enter exactly 4 numbers");
                    foreach (var part in parts)
                    {
                        if (!int.TryParse(part, out int idx) || idx < 1 || idx > 6)
                            return ValidationResult.Error("Each number must be 1-6");
                    }
                    if (parts.Select(int.Parse).Distinct().Count() != 4)
                        return ValidationResult.Error("No duplicates allowed");
                    return ValidationResult.Success();
                }));

        _state.MyBrought = indices.Trim().Split(' ')
            .Select(s => int.Parse(s) - 1)
            .ToArray();

        // Default: first 2 brought are leads (active slots 0,1)
        _state.MyActiveSlots = [0, 1];

        AnsiConsole.MarkupLine("[bold]Enter opponent's 2 leads (indices 1-6):[/]");
        var oppLeads = AnsiConsole.Prompt(
            new TextPrompt<string>("Opponent leads (e.g. '2 5')")
                .Validate(input =>
                {
                    var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 2) return ValidationResult.Error("Must enter exactly 2 numbers");
                    foreach (var part in parts)
                    {
                        if (!int.TryParse(part, out int idx) || idx < 1 || idx > 6)
                            return ValidationResult.Error("Each number must be 1-6");
                    }
                    return ValidationResult.Success();
                }));

        var oppLeadIndices = oppLeads.Trim().Split(' ')
            .Select(s => int.Parse(s) - 1)
            .ToArray();
        _state.OppActiveSlots = oppLeadIndices;

        AnsiConsole.MarkupLine($"[green]You: {string.Join(", ", _state.MyBrought.Select(i => _state.MyTeam[i].Name))}[/]");
        AnsiConsole.MarkupLine($"[red]Opp leads: {string.Join(", ", oppLeadIndices.Select(i => _state.OppTeam[i].Name))}[/]");
    }

    private void DisplayTeamPreviewRecommendation(TeamPreviewOutput output)
    {
        var table = new Table().Title("[bold gold1]Team Preview Recommendation[/]").Border(TableBorder.Rounded);
        table.AddColumn("#");
        table.AddColumn("Pokemon");
        table.AddColumn("Bring Score");
        table.AddColumn("Lead Score");

        // Sort by bring score descending
        var indexed = output.BringScores
            .Select((score, i) => (Index: i, Bring: score, Lead: output.LeadScores[i]))
            .OrderByDescending(x => x.Bring)
            .ToList();

        foreach (var (idx, bring, lead) in indexed)
        {
            var style = bring > 0.5f ? "green" : "grey";
            table.AddRow(
                (idx + 1).ToString(),
                $"[{style}]{_state.MyTeam[idx].Name}[/]",
                $"[{style}]{bring:P0}[/]",
                $"[{style}]{lead:P0}[/]");
        }

        AnsiConsole.Write(table);
    }

    // ─────────────────────────────────────────────────────────────
    //  Phase 3: Battle Loop
    // ─────────────────────────────────────────────────────────────

    private void RunBattleLoop()
    {
        AnsiConsole.MarkupLine("\n[bold cyan]== Battle Start ==[/]");

        while (true)
        {
            AnsiConsole.Write(new Rule($"[bold]Turn {_state.TurnCounter}[/]"));
            DisplayBattleState();

            // Update state
            UpdateState();

            // Check if battle is over
            if (IsBattleOver())
            {
                AnsiConsole.MarkupLine("[bold yellow]Battle over![/]");
                break;
            }

            // Get recommendation
            GetRecommendation();

            _state.TurnCounter++;

            if (!AnsiConsole.Confirm("Continue to next turn?", defaultValue: true))
                break;
        }
    }

    private void DisplayBattleState()
    {
        var myBrought = _state.GetMyBrought();

        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn(new TableColumn("[bold green]Your Side[/]").Width(35));
        table.AddColumn(new TableColumn("[bold red]Opponent Side[/]").Width(35));

        // Active Pokemon
        string myActiveStr = "";
        for (int i = 0; i < _state.MyActiveSlots.Length; i++)
        {
            int slot = _state.MyActiveSlots[i];
            if (slot >= 0 && slot < myBrought.Length)
            {
                var p = myBrought[slot];
                string status = p.Status != ConditionId.None ? $" [{GetStatusColor(p.Status)}]{GetStatusAbbr(p.Status)}[/]" : "";
                string tera = p.IsTerastallized ? " [gold1]T[/]" : "";
                string boosts = FormatBoosts(p);
                myActiveStr += $"[bold]{(char)('A' + i)}[/]: {p.Name} {p.HpPercent}%{status}{tera}{boosts}\n";
            }
        }

        string oppActiveStr = "";
        for (int i = 0; i < _state.OppActiveSlots.Length; i++)
        {
            int slot = _state.OppActiveSlots[i];
            if (slot >= 0 && slot < _state.OppTeam.Length)
            {
                var p = _state.OppTeam[slot];
                string status = p.Status != ConditionId.None ? $" [{GetStatusColor(p.Status)}]{GetStatusAbbr(p.Status)}[/]" : "";
                string tera = p.IsTerastallized ? " [gold1]T[/]" : "";
                string boosts = FormatBoosts(p);
                oppActiveStr += $"[bold]{(char)('A' + i)}[/]: {p.Name} {p.HpPercent}%{status}{tera}{boosts}\n";
            }
        }

        table.AddRow(myActiveStr.TrimEnd(), oppActiveStr.TrimEnd());

        // Bench
        string myBenchStr = "Bench: ";
        for (int i = 0; i < myBrought.Length; i++)
        {
            if (!_state.MyActiveSlots.Contains(i))
            {
                var p = myBrought[i];
                string faint = p.Fainted ? "[grey strikethrough]" : "";
                string faintEnd = p.Fainted ? "[/]" : "";
                myBenchStr += $"{faint}{p.Name} {p.HpPercent}%{faintEnd}, ";
            }
        }

        string oppBenchStr = "Bench: ";
        for (int i = 0; i < _state.OppTeam.Length; i++)
        {
            if (_state.OppTeam[i].Species != default && !_state.OppActiveSlots.Contains(i))
            {
                var p = _state.OppTeam[i];
                string faint = p.Fainted ? "[grey strikethrough]" : "";
                string faintEnd = p.Fainted ? "[/]" : "";
                oppBenchStr += $"{faint}{p.Name} {p.HpPercent}%{faintEnd}, ";
            }
        }

        table.AddRow(myBenchStr.TrimEnd(' ', ','), oppBenchStr.TrimEnd(' ', ','));

        // Field
        var fieldParts = new List<string>();
        if (_state.Weather != ConditionId.None) fieldParts.Add($"Weather: {_state.Weather}");
        if (_state.Terrain != ConditionId.None) fieldParts.Add($"Terrain: {_state.Terrain}");
        if (_state.TrickRoom) fieldParts.Add("Trick Room");
        if (_state.MyTailwind) fieldParts.Add("Your Tailwind");
        if (_state.OppTailwind) fieldParts.Add("Opp Tailwind");
        if (_state.MyReflect) fieldParts.Add("Your Reflect");
        if (_state.OppReflect) fieldParts.Add("Opp Reflect");
        if (_state.MyLightScreen) fieldParts.Add("Your LightScreen");
        if (_state.OppLightScreen) fieldParts.Add("Opp LightScreen");

        string fieldStr = fieldParts.Count > 0 ? string.Join(", ", fieldParts) : "None";
        table.AddRow($"Field: {fieldStr}", $"Tera: You={(_state.MyTeraUsed ? "used" : "available")} Opp={(_state.OppTeraUsed ? "used" : "available")}");

        AnsiConsole.Write(table);
    }

    private void UpdateState()
    {
        while (true)
        {
            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold]Update state:[/]")
                    .PageSize(15)
                    .AddChoices(
                        "Done (get recommendation)",
                        "HP% update",
                        "Status condition",
                        "Stat boosts",
                        "Switch (active Pokemon changed)",
                        "Faint a Pokemon",
                        "Field conditions",
                        "Tera used",
                        "Undo all changes this turn"));

            switch (action)
            {
                case "Done (get recommendation)":
                    return;
                case "HP% update":
                    UpdateHp();
                    break;
                case "Status condition":
                    UpdateStatus();
                    break;
                case "Stat boosts":
                    UpdateBoosts();
                    break;
                case "Switch (active Pokemon changed)":
                    UpdateSwitch();
                    break;
                case "Faint a Pokemon":
                    UpdateFaint();
                    break;
                case "Field conditions":
                    UpdateField();
                    break;
                case "Tera used":
                    UpdateTera();
                    break;
                case "Undo all changes this turn":
                    AnsiConsole.MarkupLine("[yellow]Undo not yet implemented — re-enter state manually.[/]");
                    break;
            }

            DisplayBattleState();
        }
    }

    private void UpdateHp()
    {
        var target = PromptPokemonTarget("Which Pokemon's HP changed?");
        if (target == null) return;

        int hp = AnsiConsole.Prompt(
            new TextPrompt<int>($"New HP% for {target.Name} (0-100)")
                .Validate(v => v is >= 0 and <= 100
                    ? ValidationResult.Success()
                    : ValidationResult.Error("Must be 0-100")));

        target.HpPercent = hp;
        if (hp <= 0) target.Fainted = true;
    }

    private void UpdateStatus()
    {
        var target = PromptPokemonTarget("Which Pokemon?");
        if (target == null) return;

        var status = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"Status for {target.Name}")
                .AddChoices("None", "Paralysis", "Burn", "Sleep", "Poison", "Toxic", "Freeze"));

        target.Status = status switch
        {
            "Paralysis" => ConditionId.Paralysis,
            "Burn" => ConditionId.Burn,
            "Sleep" => ConditionId.Sleep,
            "Poison" => ConditionId.Poison,
            "Toxic" => ConditionId.Toxic,
            "Freeze" => ConditionId.Freeze,
            _ => ConditionId.None,
        };
    }

    private void UpdateBoosts()
    {
        var target = PromptPokemonTarget("Which Pokemon?");
        if (target == null) return;

        var stat = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"Which stat for {target.Name}?")
                .AddChoices("Atk", "Def", "SpA", "SpD", "Spe", "Reset all"));

        if (stat == "Reset all")
        {
            target.ResetBoosts();
            return;
        }

        int value = AnsiConsole.Prompt(
            new TextPrompt<int>($"New {stat} boost (-6 to +6)")
                .Validate(v => v is >= -6 and <= 6
                    ? ValidationResult.Success()
                    : ValidationResult.Error("Must be -6 to +6")));

        switch (stat)
        {
            case "Atk": target.AtkBoost = value; break;
            case "Def": target.DefBoost = value; break;
            case "SpA": target.SpABoost = value; break;
            case "SpD": target.SpDBoost = value; break;
            case "Spe": target.SpeBoost = value; break;
        }
    }

    private void UpdateSwitch()
    {
        var side = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Which side?")
                .AddChoices("Your side", "Opponent side"));

        if (side == "Your side")
        {
            var myBrought = _state.GetMyBrought();
            var slot = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Which active slot is switching?")
                    .AddChoices("Slot A", "Slot B"));
            int slotIdx = slot == "Slot A" ? 0 : 1;

            // Show available bench Pokemon
            var benchOptions = new List<string>();
            for (int i = 0; i < myBrought.Length; i++)
            {
                if (!_state.MyActiveSlots.Contains(i) && !myBrought[i].Fainted)
                    benchOptions.Add($"{i}: {myBrought[i].Name}");
            }

            if (benchOptions.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No available bench Pokemon![/]");
                return;
            }

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Switch to which Pokemon?")
                    .AddChoices(benchOptions));

            int newIdx = int.Parse(selected.Split(':')[0]);

            // Reset boosts on the switched-out Pokemon
            myBrought[_state.MyActiveSlots[slotIdx]].ResetBoosts();

            _state.MyActiveSlots[slotIdx] = newIdx;
        }
        else
        {
            var slot = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Which opponent slot?")
                    .AddChoices("Slot A", "Slot B"));
            int slotIdx = slot == "Slot A" ? 0 : 1;

            // Show opponent team
            var options = new List<string>();
            for (int i = 0; i < _state.OppTeam.Length; i++)
            {
                if (_state.OppTeam[i].Species != default && !_state.OppActiveSlots.Contains(i) && !_state.OppTeam[i].Fainted)
                    options.Add($"{i}: {_state.OppTeam[i].Name}");
            }

            if (options.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No available opponent bench Pokemon![/]");
                return;
            }

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Opponent switched to?")
                    .AddChoices(options));

            int newIdx = int.Parse(selected.Split(':')[0]);

            // Reset boosts on the switched-out Pokemon
            _state.OppTeam[_state.OppActiveSlots[slotIdx]].ResetBoosts();

            _state.OppActiveSlots[slotIdx] = newIdx;
        }
    }

    private void UpdateFaint()
    {
        var target = PromptPokemonTarget("Which Pokemon fainted?");
        if (target == null) return;

        target.Fainted = true;
        target.HpPercent = 0;
        AnsiConsole.MarkupLine($"[red]{target.Name} fainted.[/]");
    }

    private void UpdateField()
    {
        var field = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Update which field condition?")
                .AddChoices(
                    "Weather", "Terrain", "Trick Room",
                    "Your Tailwind", "Opp Tailwind",
                    "Your Reflect", "Opp Reflect",
                    "Your Light Screen", "Opp Light Screen",
                    "Your Aurora Veil", "Opp Aurora Veil",
                    "Clear all field"));

        switch (field)
        {
            case "Weather":
                var weather = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Weather")
                        .AddChoices("None", "Sun", "Rain", "Sand", "Snow"));
                _state.Weather = weather switch
                {
                    "Sun" => ConditionId.SunnyDay,
                    "Rain" => ConditionId.RainDance,
                    "Sand" => ConditionId.Sandstorm,
                    "Snow" => ConditionId.Snowscape,
                    _ => ConditionId.None,
                };
                break;
            case "Terrain":
                var terrain = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Terrain")
                        .AddChoices("None", "Electric", "Grassy", "Psychic", "Misty"));
                _state.Terrain = terrain switch
                {
                    "Electric" => ConditionId.ElectricTerrain,
                    "Grassy" => ConditionId.GrassyTerrain,
                    "Psychic" => ConditionId.PsychicTerrain,
                    "Misty" => ConditionId.MistyTerrain,
                    _ => ConditionId.None,
                };
                break;
            case "Trick Room":
                _state.TrickRoom = !_state.TrickRoom;
                AnsiConsole.MarkupLine($"Trick Room: {(_state.TrickRoom ? "[green]ON[/]" : "[red]OFF[/]")}");
                break;
            case "Your Tailwind": _state.MyTailwind = !_state.MyTailwind; break;
            case "Opp Tailwind": _state.OppTailwind = !_state.OppTailwind; break;
            case "Your Reflect": _state.MyReflect = !_state.MyReflect; break;
            case "Opp Reflect": _state.OppReflect = !_state.OppReflect; break;
            case "Your Light Screen": _state.MyLightScreen = !_state.MyLightScreen; break;
            case "Opp Light Screen": _state.OppLightScreen = !_state.OppLightScreen; break;
            case "Your Aurora Veil": _state.MyAuroraVeil = !_state.MyAuroraVeil; break;
            case "Opp Aurora Veil": _state.OppAuroraVeil = !_state.OppAuroraVeil; break;
            case "Clear all field":
                _state.Weather = ConditionId.None;
                _state.Terrain = ConditionId.None;
                _state.TrickRoom = false;
                _state.MyTailwind = false;
                _state.OppTailwind = false;
                _state.MyReflect = false;
                _state.OppReflect = false;
                _state.MyLightScreen = false;
                _state.OppLightScreen = false;
                _state.MyAuroraVeil = false;
                _state.OppAuroraVeil = false;
                break;
        }
    }

    private void UpdateTera()
    {
        var side = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Which side used Tera?")
                .AddChoices("Your side", "Opponent side"));

        if (side == "Your side")
        {
            var myBrought = _state.GetMyBrought();
            var options = new List<string>();
            for (int i = 0; i < _state.MyActiveSlots.Length; i++)
            {
                int slot = _state.MyActiveSlots[i];
                if (slot >= 0 && slot < myBrought.Length)
                    options.Add($"{slot}: {myBrought[slot].Name}");
            }

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Which Pokemon terastallized?")
                    .AddChoices(options));

            int idx = int.Parse(selected.Split(':')[0]);
            myBrought[idx].IsTerastallized = true;
            _state.MyTeraUsed = true;
        }
        else
        {
            var options = new List<string>();
            for (int i = 0; i < _state.OppActiveSlots.Length; i++)
            {
                int slot = _state.OppActiveSlots[i];
                if (slot >= 0 && slot < _state.OppTeam.Length)
                    options.Add($"{slot}: {_state.OppTeam[slot].Name}");
            }

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Which opponent Pokemon terastallized?")
                    .AddChoices(options));

            int idx = int.Parse(selected.Split(':')[0]);
            _state.OppTeam[idx].IsTerastallized = true;

            // Ask for tera type (might not be the base type if it's revealed)
            var teraType = PromptTeraType($"Tera type for opponent {_state.OppTeam[idx].Name}");
            _state.OppTeam[idx].TeraType = teraType;
            _state.OppTeraUsed = true;
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  Recommendation
    // ─────────────────────────────────────────────────────────────

    private void GetRecommendation()
    {
        var perspective = _state.BuildPerspective();
        var output = _battleModel.Evaluate(perspective);

        // Build legal action masks for each active slot
        var myBrought = _state.GetMyBrought();

        var slotAActions = BuildLegalActions(0, myBrought);
        var slotBActions = BuildLegalActions(1, myBrought);

        var maskA = BuildLegalMask(slotAActions);
        var maskB = BuildLegalMask(slotBActions);

        var probsA = ModelInference.MaskedSoftmax(output.PolicyA, maskA);
        var probsB = ModelInference.MaskedSoftmax(output.PolicyB, maskB);

        // Display recommendation
        AnsiConsole.MarkupLine($"\n[bold gold1]Value estimate: {output.Value:F3}[/] (>0 = your advantage)");

        DisplaySlotRecommendation("Slot A", slotAActions, probsA);
        if (slotBActions.Count > 0)
            DisplaySlotRecommendation("Slot B", slotBActions, probsB);
    }

    private List<(int VocabIndex, string Label)> BuildLegalActions(int activeSlotIdx, LivePokemonState[] myBrought)
    {
        var actions = new List<(int VocabIndex, string Label)>();

        if (activeSlotIdx >= _state.MyActiveSlots.Length) return actions;

        int broughtIdx = _state.MyActiveSlots[activeSlotIdx];
        if (broughtIdx < 0 || broughtIdx >= myBrought.Length) return actions;

        var pokemon = myBrought[broughtIdx];
        if (pokemon.Fainted) return actions;

        // Add move actions
        foreach (var moveId in pokemon.Moves)
        {
            if (moveId == MoveId.None) continue;
            int vocabIdx = _vocab.GetMoveActionIndex(moveId);
            string moveName = _resolver.GetMoveName(moveId, _library);
            actions.Add((vocabIdx, $"Use {moveName}"));
        }

        // Add switch actions (all alive bench Pokemon)
        for (int i = 0; i < myBrought.Length; i++)
        {
            if (_state.MyActiveSlots.Contains(i)) continue;
            if (myBrought[i].Fainted) continue;

            int vocabIdx = _vocab.GetSwitchActionIndex(myBrought[i].Species);
            string name = myBrought[i].Name;
            actions.Add((vocabIdx, $"Switch to {name}"));
        }

        return actions;
    }

    private bool[] BuildLegalMask(List<(int VocabIndex, string Label)> actions)
    {
        var mask = new bool[_vocab.NumActions];
        foreach (var (vocabIdx, _) in actions)
        {
            mask[vocabIdx] = true;
        }
        return mask;
    }

    private void DisplaySlotRecommendation(string slotName, List<(int VocabIndex, string Label)> actions, float[] probs)
    {
        var ranked = actions
            .Select(a => (a.Label, Prob: probs[a.VocabIndex]))
            .OrderByDescending(x => x.Prob)
            .ToList();

        var table = new Table()
            .Title($"[bold]{slotName}[/]")
            .Border(TableBorder.Simple)
            .AddColumn("Action")
            .AddColumn("Probability");

        foreach (var (label, prob) in ranked)
        {
            string color = prob > 0.5f ? "green" : prob > 0.2f ? "yellow" : "grey";
            table.AddRow($"[{color}]{label}[/]", $"[{color}]{prob:P1}[/]");
        }

        AnsiConsole.Write(table);
    }

    // ─────────────────────────────────────────────────────────────
    //  Input Helpers
    // ─────────────────────────────────────────────────────────────

    private SpecieId PromptSpecies(string prompt)
    {
        while (true)
        {
            string input = AnsiConsole.Prompt(new TextPrompt<string>($"[cyan]{prompt}:[/]"));
            if (_resolver.TryResolveSpecies(input, out var result))
            {
                AnsiConsole.MarkupLine($"  [grey]→ {_resolver.GetSpeciesName(result, _library)}[/]");
                return result;
            }

            var suggestions = _resolver.GetSpeciesSuggestions(input);
            if (suggestions.Count > 0)
                AnsiConsole.MarkupLine($"[yellow]Not found. Did you mean: {string.Join(", ", suggestions)}?[/]");
            else
                AnsiConsole.MarkupLine("[red]No match found. Try again.[/]");
        }
    }

    private MoveId PromptMove(string prompt)
    {
        while (true)
        {
            string input = AnsiConsole.Prompt(new TextPrompt<string>($"[cyan]{prompt}:[/]"));
            if (_resolver.TryResolveMove(input, out var result))
            {
                AnsiConsole.MarkupLine($"  [grey]→ {_resolver.GetMoveName(result, _library)}[/]");
                return result;
            }

            var suggestions = _resolver.GetMoveSuggestions(input);
            if (suggestions.Count > 0)
                AnsiConsole.MarkupLine($"[yellow]Not found. Did you mean: {string.Join(", ", suggestions)}?[/]");
            else
                AnsiConsole.MarkupLine("[red]No match found. Try again.[/]");
        }
    }

    private AbilityId PromptAbility(string prompt)
    {
        while (true)
        {
            string input = AnsiConsole.Prompt(new TextPrompt<string>($"[cyan]{prompt}:[/]"));
            if (_resolver.TryResolveAbility(input, out var result))
            {
                AnsiConsole.MarkupLine($"  [grey]→ {_resolver.GetAbilityName(result, _library)}[/]");
                return result;
            }
            AnsiConsole.MarkupLine("[red]No match found. Try again.[/]");
        }
    }

    private ItemId PromptItem(string prompt)
    {
        while (true)
        {
            string input = AnsiConsole.Prompt(new TextPrompt<string>($"[cyan]{prompt}:[/]"));
            if (_resolver.TryResolveItem(input, out var result))
            {
                AnsiConsole.MarkupLine($"  [grey]→ {_resolver.GetItemName(result, _library)}[/]");
                return result;
            }
            AnsiConsole.MarkupLine("[red]No match found. Try again.[/]");
        }
    }

    private static MoveType PromptTeraType(string prompt)
    {
        var typeNames = Enum.GetValues<MoveType>()
            .Where(t => t != MoveType.Unknown)
            .Select(t => t.ToString())
            .ToList();

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[cyan]{prompt}:[/]")
                .PageSize(10)
                .AddChoices(typeNames));

        return Enum.Parse<MoveType>(selected);
    }

    private LivePokemonState? PromptPokemonTarget(string prompt)
    {
        var options = new List<string>();
        var lookup = new Dictionary<string, LivePokemonState>();

        var myBrought = _state.GetMyBrought();
        for (int i = 0; i < _state.MyActiveSlots.Length; i++)
        {
            int slot = _state.MyActiveSlots[i];
            if (slot >= 0 && slot < myBrought.Length)
            {
                string key = $"Your {(char)('A' + i)}: {myBrought[slot].Name}";
                options.Add(key);
                lookup[key] = myBrought[slot];
            }
        }

        for (int i = 0; i < _state.OppActiveSlots.Length; i++)
        {
            int slot = _state.OppActiveSlots[i];
            if (slot >= 0 && slot < _state.OppTeam.Length)
            {
                string key = $"Opp {(char)('A' + i)}: {_state.OppTeam[slot].Name}";
                options.Add(key);
                lookup[key] = _state.OppTeam[slot];
            }
        }

        // Also include bench Pokemon
        for (int i = 0; i < myBrought.Length; i++)
        {
            if (!_state.MyActiveSlots.Contains(i) && !myBrought[i].Fainted)
            {
                string key = $"Your bench: {myBrought[i].Name}";
                options.Add(key);
                lookup[key] = myBrought[i];
            }
        }

        for (int i = 0; i < _state.OppTeam.Length; i++)
        {
            if (_state.OppTeam[i].Species != default && !_state.OppActiveSlots.Contains(i) && !_state.OppTeam[i].Fainted)
            {
                string key = $"Opp bench: {_state.OppTeam[i].Name}";
                options.Add(key);
                lookup[key] = _state.OppTeam[i];
            }
        }

        if (options.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No Pokemon available.[/]");
            return null;
        }

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(prompt)
                .AddChoices(options));

        return lookup[selected];
    }

    private bool IsBattleOver()
    {
        var myBrought = _state.GetMyBrought();
        bool allMyFainted = myBrought.All(p => p.Fainted);
        bool allOppFainted = _state.OppTeam
            .Where(p => p.Species != default)
            .All(p => p.Fainted);

        return allMyFainted || allOppFainted;
    }

    private void ResetForNewGame()
    {
        // Reset all battle state but keep team definitions
        for (int i = 0; i < 6; i++)
        {
            _state.MyTeam[i].HpPercent = 100;
            _state.MyTeam[i].Fainted = false;
            _state.MyTeam[i].Status = ConditionId.None;
            _state.MyTeam[i].ResetBoosts();
            _state.MyTeam[i].IsActive = false;
            _state.MyTeam[i].IsTerastallized = false;

            _state.OppTeam[i] = new LivePokemonState();
        }

        _state.Weather = ConditionId.None;
        _state.Terrain = ConditionId.None;
        _state.TrickRoom = false;
        _state.MyTailwind = false;
        _state.OppTailwind = false;
        _state.MyReflect = false;
        _state.OppReflect = false;
        _state.MyLightScreen = false;
        _state.OppLightScreen = false;
        _state.MyAuroraVeil = false;
        _state.OppAuroraVeil = false;
        _state.MyTeraUsed = false;
        _state.OppTeraUsed = false;
        _state.TurnCounter = 1;
    }

    // ─────────────────────────────────────────────────────────────
    //  Team Save/Load
    // ─────────────────────────────────────────────────────────────

    private record TeamFileEntry(
        string Species, string Ability, string Item, string TeraType, string[] Moves);

    /// <summary>
    /// Auto-load: if exactly one saved team exists, load it silently.
    /// If multiple exist, let user pick.
    /// </summary>
    private bool TryAutoLoadTeam()
    {
        if (!Directory.Exists(TeamSaveDir)) return false;

        var files = Directory.GetFiles(TeamSaveDir, "*.json");
        if (files.Length == 0) return false;

        if (files.Length == 1)
        {
            string name = Path.GetFileNameWithoutExtension(files[0]);
            AnsiConsole.MarkupLine($"[grey]Auto-loading saved team '{name}'...[/]");
            return LoadTeamFromFile(files[0]);
        }

        // Multiple teams — let user pick
        return TryLoadTeam();
    }

    private void SaveTeam()
    {
        Directory.CreateDirectory(TeamSaveDir);

        // Auto-name from lead species
        string defaultName = string.Join("-", _state.MyTeam
            .Where(p => p.Species != default)
            .Take(2)
            .Select(p => p.Name));
        if (string.IsNullOrEmpty(defaultName)) defaultName = "team";

        string name = AnsiConsole.Prompt(
            new TextPrompt<string>("Team name:")
                .DefaultValue(defaultName));

        string path = Path.Combine(TeamSaveDir, $"{name}.json");

        var entries = new TeamFileEntry[6];
        for (int i = 0; i < 6; i++)
        {
            var p = _state.MyTeam[i];
            entries[i] = new TeamFileEntry(
                _resolver.GetSpeciesName(p.Species, _library),
                _resolver.GetAbilityName(p.Ability, _library),
                _resolver.GetItemName(p.Item, _library),
                p.TeraType.ToString(),
                p.Moves.Where(m => m != MoveId.None)
                    .Select(m => _resolver.GetMoveName(m, _library))
                    .ToArray());
        }

        string json = JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
        AnsiConsole.MarkupLine($"[green]Team saved to {path}[/]");
    }

    private bool TryLoadTeam()
    {
        if (!Directory.Exists(TeamSaveDir))
        {
            AnsiConsole.MarkupLine("[yellow]No saved teams found.[/]");
            return false;
        }

        var files = Directory.GetFiles(TeamSaveDir, "*.json");
        if (files.Length == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No saved teams found.[/]");
            return false;
        }

        var teamNames = files.Select(f => Path.GetFileNameWithoutExtension(f)).ToList();
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a team to load:")
                .AddChoices(teamNames));

        string path = Path.Combine(TeamSaveDir, $"{selected}.json");
        return LoadTeamFromFile(path);
    }

    private bool LoadTeamFromFile(string path)
    {
        string json = File.ReadAllText(path);
        var entries = JsonSerializer.Deserialize<TeamFileEntry[]>(json);

        if (entries == null || entries.Length != 6)
        {
            AnsiConsole.MarkupLine("[red]Invalid team file.[/]");
            return false;
        }

        for (int i = 0; i < 6; i++)
        {
            var entry = entries[i];
            var pokemon = _state.MyTeam[i];

            if (!_resolver.TryResolveSpecies(entry.Species, out var specieId))
            {
                AnsiConsole.MarkupLine($"[red]Unknown species: {entry.Species}[/]");
                return false;
            }

            pokemon.Species = specieId;
            pokemon.Name = _resolver.GetSpeciesName(specieId, _library);

            if (_resolver.TryResolveAbility(entry.Ability, out var abilityId))
                pokemon.Ability = abilityId;
            if (_resolver.TryResolveItem(entry.Item, out var itemId))
                pokemon.Item = itemId;
            if (Enum.TryParse<MoveType>(entry.TeraType, true, out var teraType))
                pokemon.TeraType = teraType;

            for (int m = 0; m < entry.Moves.Length && m < 4; m++)
            {
                if (_resolver.TryResolveMove(entry.Moves[m], out var moveId))
                    pokemon.Moves[m] = moveId;
            }
        }

        AnsiConsole.MarkupLine($"[green]Loaded team from {Path.GetFileNameWithoutExtension(path)}[/]");
        return true;
    }

    // ─────────────────────────────────────────────────────────────
    //  Display Helpers
    // ─────────────────────────────────────────────────────────────

    private static string GetStatusAbbr(ConditionId status) => status switch
    {
        ConditionId.Paralysis => "PAR",
        ConditionId.Burn => "BRN",
        ConditionId.Sleep => "SLP",
        ConditionId.Poison => "PSN",
        ConditionId.Toxic => "TOX",
        ConditionId.Freeze => "FRZ",
        _ => "",
    };

    private static string GetStatusColor(ConditionId status) => status switch
    {
        ConditionId.Paralysis => "yellow",
        ConditionId.Burn => "red",
        ConditionId.Sleep => "grey",
        ConditionId.Poison => "purple",
        ConditionId.Toxic => "purple",
        ConditionId.Freeze => "aqua",
        _ => "white",
    };

    private static string FormatBoosts(LivePokemonState p)
    {
        var parts = new List<string>();
        if (p.AtkBoost != 0) parts.Add($"Atk{FormatBoostValue(p.AtkBoost)}");
        if (p.DefBoost != 0) parts.Add($"Def{FormatBoostValue(p.DefBoost)}");
        if (p.SpABoost != 0) parts.Add($"SpA{FormatBoostValue(p.SpABoost)}");
        if (p.SpDBoost != 0) parts.Add($"SpD{FormatBoostValue(p.SpDBoost)}");
        if (p.SpeBoost != 0) parts.Add($"Spe{FormatBoostValue(p.SpeBoost)}");

        if (parts.Count == 0) return "";
        return $" ({string.Join(" ", parts)})";
    }

    private static string FormatBoostValue(int boost) =>
        boost > 0 ? $"[green]+{boost}[/]" : $"[red]{boost}[/]";
}
