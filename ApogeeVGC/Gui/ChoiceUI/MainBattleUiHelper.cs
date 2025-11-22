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
    private const int ButtonHeight = 50;
    
    // Vertical spacing between buttons
    private const int ButtonSpacing = 8;
    
    // Position of choice menu on screen - inline with player's Pokemon, aligned with opponent horizontally
    private const int LeftMargin = 480;   // X position aligned with opponent Pokemon (midscreen)
    private const int TopMargin = 400;   // Y position inline with player Pokemon
    
    // Position of instruction text (appears above buttons)
    private const int InstructionTextYOffset = 30; // Distance above TopMargin
    
    // Position of selection status text (appears above message box)
    private const int SelectionStatusY = 635; // Fixed Y position above message box

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

    /// <summary>
    /// Determine appropriate text color (black or white) based on background brightness
    /// Uses perceived luminance formula to determine if background is light or dark
    /// </summary>
    private static Color GetTextColorForBackground(Color backgroundColor)
    {
        // Calculate perceived luminance (0.0 to 1.0)
        // Formula: Y = 0.299*R + 0.587*G + 0.114*B
        double luminance = (0.299 * backgroundColor.R + 
                           0.587 * backgroundColor.G + 
                           0.114 * backgroundColor.B) / 255.0;
        
        // If background is bright (luminance > 0.5), use black text
        // Otherwise use white text
        return luminance > 0.5 ? Color.Black : Color.White;
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
            
            // Get the move's type color
            Color moveTypeColor = GetTeraTypeColor(moveData.Move.Type);
            
            // Determine text color based on background brightness
            // For light backgrounds (Normal, Electric, Ice, etc.), use black text
            // For dark backgrounds, use white text
            Color textColor = GetTextColorForBackground(moveTypeColor);
            
            // Format move name with PP information
            string moveDisplayText = $"{moveData.Move.Name} ({moveData.Pp}/{moveData.MaxPp})";
            
            // Add move button with type-colored background
            var button = new ChoiceButton(
                new Rectangle(x, y, moveButtonWidth, moveButtonHeight),
                moveDisplayText,
                moveTypeColor,  // Use move's type color instead of generic blue
                () => selectMove(originalIndex, isTerastallized),
                textColor  // Use appropriate text color for readability
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
            
            // Split text into white "Tera " prefix and colored type name
            string teraPrefix = "Tera ";
            string teraTypeText = teraTypeName;
            
            // Visual feedback through background color:
            // - Active (ON): Use the full type color for maximum visibility
            // - Inactive (OFF): Use a light gray that's very easy to see
            Color teraButtonBg;
            if (isTerastallized)
            {
                // Active: Use the FULL BRIGHT type color - maximum saturation
                teraButtonBg = teraColor;
            }
            else
            {
                // Inactive: Use a LIGHT gray - much brighter so it's super obvious
                teraButtonBg = new Color(100, 100, 110);
            }
            
            Console.WriteLine($"[MainBattleUiHelper] Tera button: Type={teraTypeName}, Active={isTerastallized}, BgColor=({teraButtonBg.R},{teraButtonBg.G},{teraButtonBg.B})");
            
            // When active, use white text for readability against colored background
            // When inactive, use type color for the type name against gray background
            Color typeTextColor = isTerastallized ? Color.White : teraColor;
            
            var teraToggleButton = new ChoiceButton(
                new Rectangle(teraButtonX, teraButtonY, teraButtonWidth, teraButtonHeight),
                teraPrefix,           // Primary text (white)
                teraButtonBg,         // Background color shows activation state
                toggleTera,
                Color.White,          // Primary text color (always white)
                teraTypeText,         // Secondary text (type name)
                typeTextColor         // Secondary text color (white when active, type color when inactive)
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