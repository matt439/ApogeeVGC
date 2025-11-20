using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
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
    private const int ButtonWidth = 200;
    private const int ButtonHeight = 35;
    
    // Vertical spacing between buttons
    private const int ButtonSpacing = 3;
    
    // Extra spacing before certain buttons (e.g., before "Back" button)
    private const int ButtonSpacingLarge = 8;
    
    // Position of choice menu on screen
    private const int LeftMargin = 700;  // X position from left edge
    private const int TopMargin = 300;   // Y position from top edge
    
    // Position of instruction text (appears above buttons)
    private const int InstructionTextYOffset = 80; // Distance above TopMargin
    
    // Position of selection status text (appears below buttons)
    private const int SelectionStatusY = 650; // Fixed Y position at bottom

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
        int y = TopMargin;
        int moveIndex = 0;

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
            if (disabled)
            {
                moveIndex++;
                continue;
            }

            int index = moveIndex;
            
            // Add regular move option
            var button = new ChoiceButton(
                new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
                moveData.Move.Name,
                Color.Blue,
                () => selectMove(index, false)
            );
            buttons.Add(button);
            y += ButtonHeight + ButtonSpacing;

            // Add Tera variant if terastallization is available
            if (canTerastallize)
            {
                var teraButton = new ChoiceButton(
                    new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
                    $"{moveData.Move.Name} (+ TERA)",
                    Color.Purple,
                    () => selectMove(index, true)
                );
                buttons.Add(teraButton);
                y += ButtonHeight + ButtonSpacing;
            }

            moveIndex++;
        }

        // Add back button
        y += ButtonHeight + ButtonSpacingLarge;
        var backButton = new ChoiceButton(
            new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
            "Back",
            Color.Orange,
            goBack
        );
        buttons.Add(backButton);

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

        // Add back button if requested
        if (showBackButton)
        {
            y += ButtonSpacingLarge;
            var backButton = new ChoiceButton(
                new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
                "Back",
                Color.Orange,
                goBack
            );
            buttons.Add(backButton);
        }

        return buttons;
    }

    public static string GetInstructionText(MainBattlePhaseState state)
    {
        return state switch
        {
            MainBattlePhaseState.MainMenuFirstPokemon =>
                "Choose action for Pokemon 1.",
            MainBattlePhaseState.MainMenuSecondPokemon =>
                "Choose action for Pokemon 2.",
            MainBattlePhaseState.MoveSelectionFirstPokemon =>
                "Select a move for Pokemon 1.",
            MainBattlePhaseState.MoveSelectionSecondPokemon =>
                "Select a move for Pokemon 2.",
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