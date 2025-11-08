using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils.Unions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace ApogeeVGC.Gui.ChoiceUI;

/// <summary>
/// Helper class for setting up main battle phase UI elements
/// </summary>
public static class MainBattleUiHelper
{
    private const int ButtonWidth = 200;
    private const int ButtonHeight = 50;
    private const int ButtonSpacing = 10;
    // Move buttons to the right side of the screen
    private const int LeftMargin = 800; // Changed from 50 to 800
    private const int TopMargin = 400;

    public static List<ChoiceButton> CreateMainMenuFirstPokemon(
        MoveRequest request,
        Action showMoveSelection,
        Action showSwitchSelection,
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

   // Pokemon button
        var pokemonButton = new ChoiceButton(
          new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
          "Pokemon",
  Color.Green,
       showSwitchSelection
   );
buttons.Add(pokemonButton);
 y += ButtonHeight + ButtonSpacing;

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
        Action showSwitchSelection,
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

      // Pokemon button
        var pokemonButton = new ChoiceButton(
            new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
  "Pokemon",
    Color.Green,
            showSwitchSelection
        );
        buttons.Add(pokemonButton);
        y += ButtonHeight + ButtonSpacing;

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
        Action<int> selectMove,
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

     int index = moveIndex;
   var button = new ChoiceButton(
    new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
    moveData.Move.Name,
     disabled ? Color.Gray : Color.Blue,
                () => selectMove(index)
  );

    buttons.Add(button);
     y += ButtonHeight + ButtonSpacing;
      moveIndex++;
     }

  // Add Terastallize option if available
        if (canTerastallize)
      {
            y += ButtonSpacing;
     var teraButton = new ChoiceButton(
        new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
           "Terastallize",
     isTerastallized ? Color.Purple : Color.DarkSlateBlue,
     toggleTera
   );
    buttons.Add(teraButton);
        }

      // Add back button
  y += ButtonHeight + ButtonSpacing;
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
        IEnumerable<PokemonSwitchRequestData> availablePokemon,
   Action<int> selectSwitch,
   Action goBack,
        bool showBackButton = true)
    {
        var buttons = new List<ChoiceButton>();
     int y = TopMargin;
        int pokemonIndex = 0;

        foreach (PokemonSwitchRequestData pokemon in availablePokemon)
 {
 int index = pokemonIndex;
  var button = new ChoiceButton(
                new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
       $"Pokemon (Condition: {pokemon.Condition})",
   Color.Green,
              () => selectSwitch(index)
    );

     buttons.Add(button);
     y += ButtonHeight + ButtonSpacing;
     pokemonIndex++;
        }

        // Add back button if requested
    if (showBackButton)
        {
            y += ButtonSpacing;
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
         MainBattlePhaseState.MainMenuFirstPokemon => "Choose action for Pokemon 1: Use UP/DOWN arrows to select, ENTER to confirm",
            MainBattlePhaseState.MainMenuSecondPokemon => "Choose action for Pokemon 2: Use UP/DOWN arrows to select, ENTER to confirm, ESC to go back",
          MainBattlePhaseState.MoveSelectionFirstPokemon => "Select a move for Pokemon 1: Use UP/DOWN arrows, ENTER to confirm, ESC for back",
            MainBattlePhaseState.MoveSelectionSecondPokemon => "Select a move for Pokemon 2: Use UP/DOWN arrows, ENTER to confirm, ESC for back",
            MainBattlePhaseState.SwitchSelectionFirstPokemon => "Select a Pokemon to switch in: Use UP/DOWN arrows, ENTER to confirm, ESC for back",
  MainBattlePhaseState.SwitchSelectionSecondPokemon => "Select a Pokemon to switch in: Use UP/DOWN arrows, ENTER to confirm, ESC for back",
  MainBattlePhaseState.ForceSwitch => "Your Pokemon fainted! Select a replacement: Use UP/DOWN arrows, ENTER to confirm",
      _ => "Use UP/DOWN arrows to navigate, ENTER to select",
        };
    }
}
