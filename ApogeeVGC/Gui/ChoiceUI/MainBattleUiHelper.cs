using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils.Unions;
using Microsoft.Xna.Framework;

namespace ApogeeVGC.Gui.ChoiceUI;

/// <summary>
/// Helper class for setting up main battle phase UI elements
/// </summary>
public static class MainBattleUiHelper
{
    // ===== MAIN BATTLE CHOICE UI LAYOUT CONSTANTS =====
    
    // Button dimensions
    private const int ButtonWidth = 180;
    private const int ButtonHeight = 28;
    
    // Vertical spacing between buttons
    private const int ButtonSpacing = 2;
    
    // Extra spacing before certain buttons (e.g., before "Back" button)
    private const int ButtonSpacingLarge = 5;
    
    // Position of choice menu on screen - in left half below Pokemon
    private const int LeftMargin = 20;   // X position from left edge (aligned with opponent Pokemon)
    private const int TopMargin = 430;   // Y position from top edge (below player Pokemon)
    
    // Position of instruction text (appears above buttons)
    private const int InstructionTextYOffset = 20; // Distance above TopMargin
    
    // Position of selection status text (appears below buttons)
    private const int SelectionStatusY = 695; // Fixed Y position at bottom

    /// <summary>
    /// Get the MonoGame Color for a tera type (matching PlayerConsole colors)
    /// </summary>
    private static Color GetTeraTypeColor(MoveType teraType)
    {
        return teraType switch
        {
            MoveType.Normal => Color.White,
            MoveType.Fire => Color.Red,
            MoveType.Water => Color.Blue,
            MoveType.Electric => Color.Yellow,
            MoveType.Grass => Color.Green,
            MoveType.Ice => Color.Cyan,
            MoveType.Fighting => Color.DarkOrange,
            MoveType.Poison => Color.Purple,
            MoveType.Ground => new Color(255, 215, 0), // Gold3-like
            MoveType.Flying => Color.DeepSkyBlue,
            MoveType.Psychic => Color.Magenta,
            MoveType.Bug => Color.GreenYellow,
            MoveType.Rock => Color.Orange,
            MoveType.Ghost => Color.Purple,
            MoveType.Dragon => Color.Blue,
            MoveType.Dark => Color.Gray,
            MoveType.Steel => new Color(188, 188, 188), // Grey74-like
            MoveType.Fairy => Color.HotPink,
            _ => Color.White,
        };
    }

    public static List<ChoiceButton> CreateMainMenuFirstPokemon(
        MoveRequest request,
        Action showMoveSelection,
        Action? showSwitchSelection,
        Action selectForfeit)
    {
        var buttons = new List<ChoiceButton>();
        int y = TopMargin;

        // Battle button
        var battleButton = new ChoiceButton(
            new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
            "Battle",
            Color.Red,
            showMoveSelection
        );
        buttons.Add(battleButton);
        y += ButtonHeight + ButtonSpacing;

        // Pokemon button - only add if there are valid switch options
        if (showSwitchSelection != null)
        {
            var pokemonButton = new ChoiceButton(
                new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
                "Pokemon",
                Color.Green,
                showSwitchSelection
            );
            buttons.Add(pokemonButton);
            y += ButtonHeight + ButtonSpacing;
        }

        // Run button
        var runButton = new ChoiceButton(
            new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
            "Run",
            Color.Gray,
            selectForfeit
        );
        buttons.Add(runButton);

        return buttons;
    }

    public static List<ChoiceButton> CreateMainMenuSecondPokemon(
        MoveRequest request,
        Action showMoveSelection,
        Action? showSwitchSelection,
        Action goBack)
    {
        var buttons = new List<ChoiceButton>();
        int y = TopMargin;

        // Battle button
        var battleButton = new ChoiceButton(
            new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
            "Battle",
            Color.Red,
            showMoveSelection
        );
        buttons.Add(battleButton);
        y += ButtonHeight + ButtonSpacing;

        // Pokemon button - only add if there are valid switch options
        if (showSwitchSelection != null)
        {
            var pokemonButton = new ChoiceButton(
                new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
                "Pokemon",
                Color.Green,
                showSwitchSelection
            );
            buttons.Add(pokemonButton);
            y += ButtonHeight + ButtonSpacing;
        }

        // Back button
        var backButton = new ChoiceButton(
            new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
            "Back",
            Color.Orange,
            goBack
        );
        buttons.Add(backButton);

        return buttons;
    }

    public static List<ChoiceButton> CreateMoveSelectionButtons(
        PokemonMoveRequestData pokemonRequest,
        bool canTerastallize,
        bool isTerastallized,
        Action<int, bool> selectMove,
        Action toggleTera,
        Action goBack)
    {
        var buttons = new List<ChoiceButton>();
        int moveIndex = 0;

        // Extract tera type if terastallization is available
        MoveType? teraType = null;
        if (canTerastallize && pokemonRequest.CanTerastallize != null)
        {
            teraType = pokemonRequest.CanTerastallize switch
            {
                MoveTypeMoveTypeFalseUnion mtfu => mtfu.MoveType,
                _ => null,
            };
        }

        // Collect non-disabled moves
        var availableMoves = new List<(PokemonMoveData MoveData, int Index)>();
        foreach (PokemonMoveData moveData in pokemonRequest.Moves)
        {
            // Check if move is disabled
            bool disabled = moveData.Disabled switch
            {
                BoolMoveIdBoolUnion boolUnion => boolUnion.Value,
                MoveIdMoveIdBoolUnion => false,
                null => false,
                _ => false,
            };

            // Skip disabled moves - don't show them at all
            if (!disabled)
            {
                availableMoves.Add((moveData, moveIndex));
            }

            moveIndex++;
        }

        // Layout moves in a 2x2 grid
        // Grid constants
        const int gridColumns = 2;
        const int moveButtonWidth = 180;
        const int moveButtonHeight = 28;
        const int horizontalSpacing = 10;
        const int verticalSpacing = 10;

        for (int i = 0; i < availableMoves.Count; i++)
        {
            var (moveData, originalIndex) = availableMoves[i];
            
            // Calculate grid position (column, row)
            int col = i % gridColumns;
            int row = i / gridColumns;
            
            // Calculate button position
            int x = LeftMargin + (col * (moveButtonWidth + horizontalSpacing));
            int y = TopMargin + (row * (moveButtonHeight + verticalSpacing));
            
            // Add move button - clicking it will use the current tera toggle state
            var button = new ChoiceButton(
                new Rectangle(x, y, moveButtonWidth, moveButtonHeight),
                moveData.Move.Name,
                Color.Blue,
                () => selectMove(originalIndex, isTerastallized)
            );
            buttons.Add(button);
        }

        // Add Tera toggle button below the move grid if available
        if (canTerastallize && teraType.HasValue)
        {
            Color teraColor = GetTeraTypeColor(teraType.Value);
            string teraTypeName = teraType.Value.ToString().ToUpper();
            
            // Position below the move grid
            int teraButtonX = LeftMargin;
            int teraButtonY = TopMargin + (2 * (moveButtonHeight + verticalSpacing)) + verticalSpacing;
            int teraButtonWidth = (moveButtonWidth * 2) + horizontalSpacing; // Span full width of grid
            int teraButtonHeight = 28;
            
            // Split text into parts for color coding
            string statusText = isTerastallized ? "[ON]" : "[OFF]";
            string teraButtonText = $"TERA {teraTypeName} {statusText}";
            Color teraButtonBg = isTerastallized ? teraColor : Color.DarkGray;
            
            var teraToggleButton = new ChoiceButton(
                new Rectangle(teraButtonX, teraButtonY, teraButtonWidth, teraButtonHeight),
                teraButtonText,
                teraButtonBg,
                toggleTera,
                teraColor  // Use tera type color for text
            );
            buttons.Add(teraToggleButton);
        }

        return buttons;
    }

    public static List<ChoiceButton> CreateSwitchSelectionButtons(
        IEnumerable<(PokemonSwitchRequestData Pokemon, int OriginalIndex)> availablePokemon,
        Action<int> selectSwitch,
        Action goBack,
        BattlePerspective? perspective,
        bool showBackButton = true)
    {
        var buttons = new List<ChoiceButton>();
        int y = TopMargin;

        foreach (var (pokemon, originalIndex) in availablePokemon)
        {
            int index = originalIndex; // Use original index for switch selection
            
            // Get HP information and name from perspective if available
            string displayName = pokemon.Details;
            string hpDisplay;
            
            if (perspective != null)
            {
                // Find matching Pokemon in perspective by position
                var perspectivePokemon = perspective.PlayerSide.Pokemon
                    .FirstOrDefault(pp => pp.Position == originalIndex);

                if (perspectivePokemon != null)
                {
                    // Use name from perspective
                    displayName = $"{perspectivePokemon.Name}, L{perspectivePokemon.Level}";
                    
                    hpDisplay = perspectivePokemon.Fainted
                        ? "Fainted"
                        : $"{perspectivePokemon.Hp}/{perspectivePokemon.MaxHp}";
                }
                else
                {
                    // Fallback to max HP from stats
                    int maxHp = pokemon.Stats.Hp;
                    hpDisplay = $"{maxHp}/{maxHp}";
                }
            }
            else
            {
                // Fallback to max HP from stats if no perspective
                int maxHp = pokemon.Stats.Hp;
                hpDisplay = $"{maxHp}/{maxHp}";
            }
            
            var button = new ChoiceButton(
                new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
                $"{displayName} (HP: {hpDisplay})",
                Color.Green,
                () => selectSwitch(index)
            );

            buttons.Add(button);
            y += ButtonHeight + ButtonSpacing;
        }

        return buttons;
    }

    public static string GetInstructionText(MainBattlePhaseState state, string? firstPokemonName = null, string? secondPokemonName = null)
    {
        return state switch
        {
            MainBattlePhaseState.MainMenuFirstPokemon =>
                $"Choose action for {firstPokemonName ?? "Pokemon 1"}.",
            MainBattlePhaseState.MainMenuSecondPokemon =>
                $"Choose action for {secondPokemonName ?? "Pokemon 2"}.",
            MainBattlePhaseState.MoveSelectionFirstPokemon =>
                $"Select a move for {firstPokemonName ?? "Pokemon 1"}.",
            MainBattlePhaseState.MoveSelectionSecondPokemon =>
                $"Select a move for {secondPokemonName ?? "Pokemon 2"}.",
            MainBattlePhaseState.SwitchSelectionFirstPokemon =>
                "Select a Pokemon to switch in.",
            MainBattlePhaseState.SwitchSelectionSecondPokemon =>
                "Select a Pokemon to switch in.",
            MainBattlePhaseState.ForceSwitch =>
                "Your Pokemon fainted! Select a replacement.",
            _ => "Use UP/DOWN arrows to navigate, ENTER to select",
        };
    }

    /// <summary>
    /// Get the position for instruction text (appears above buttons)
    /// </summary>
    public static Vector2 GetInstructionTextPosition()
    {
        return new Vector2(LeftMargin, TopMargin - InstructionTextYOffset);
    }

    /// <summary>
    /// Get the position for selection status text (appears at bottom)
    /// </summary>
    public static Vector2 GetSelectionStatusPosition()
    {
        return new Vector2(LeftMargin, SelectionStatusY);
    }
}