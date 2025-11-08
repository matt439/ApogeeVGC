namespace ApogeeVGC.Gui.ChoiceUI;

/// <summary>
/// Represents the different UI states during the main battle phase (Scarlet/Violet style)
/// </summary>
public enum MainBattlePhaseState
{
    /// <summary>
    /// Main menu for first Pokemon: Battle, Pokemon, Run options
    /// </summary>
    MainMenuFirstPokemon,

    /// <summary>
    /// Selecting a move for the first Pokemon
    /// </summary>
    MoveSelectionFirstPokemon,

    /// <summary>
    /// Selecting a target for the first Pokemon's move
    /// </summary>
    TargetSelectionFirstPokemon,

    /// <summary>
    /// Main menu for second Pokemon: Battle, Pokemon, Back options
    /// </summary>
    MainMenuSecondPokemon,

    /// <summary>
    /// Selecting a move for the second Pokemon
    /// </summary>
    MoveSelectionSecondPokemon,

    /// <summary>
    /// Selecting a target for the second Pokemon's move
    /// </summary>
    TargetSelectionSecondPokemon,

    /// <summary>
    /// Selecting a Pokemon to switch (from main menu)
    /// </summary>
    SwitchSelectionFirstPokemon,

    /// <summary>
    /// Selecting a Pokemon to switch (from second Pokemon menu)
    /// </summary>
    SwitchSelectionSecondPokemon,

    /// <summary>
    /// Force switch (after a Pokemon faints)
    /// </summary>
    ForceSwitch,
}

/// <summary>
/// Tracks the player's selections during a turn
/// </summary>
public class TurnSelectionState
{
    /// <summary>
    /// Index of the Pokemon currently being selected for (0 or 1)
    /// </summary>
    public int CurrentPokemonIndex { get; set; }

    /// <summary>
    /// Selected move for the first Pokemon (if any)
    /// </summary>
    public int? FirstPokemonMoveIndex { get; set; }

    /// <summary>
    /// Selected target for the first Pokemon (if any)
    /// </summary>
    public int? FirstPokemonTarget { get; set; }

    /// <summary>
    /// Switch index for the first Pokemon (if switching)
    /// </summary>
    public int? FirstPokemonSwitchIndex { get; set; }

    /// <summary>
    /// Should the first Pokemon Terastallize?
    /// </summary>
    public bool FirstPokemonTerastallize { get; set; }

    /// <summary>
    /// Selected move for the second Pokemon (if any)
    /// </summary>
    public int? SecondPokemonMoveIndex { get; set; }

    /// <summary>
    /// Selected target for the second Pokemon (if any)
    /// </summary>
    public int? SecondPokemonTarget { get; set; }

    /// <summary>
    /// Switch index for the second Pokemon (if switching)
    /// </summary>
    public int? SecondPokemonSwitchIndex { get; set; }

    /// <summary>
    /// Should the second Pokemon Terastallize?
    /// </summary>
    public bool SecondPokemonTerastallize { get; set; }

    /// <summary>
    /// Is the player forfeiting?
    /// </summary>
    public bool Forfeit { get; set; }

    public void Reset()
    {
        CurrentPokemonIndex = 0;
        FirstPokemonMoveIndex = null;
        FirstPokemonTarget = null;
        FirstPokemonSwitchIndex = null;
        FirstPokemonTerastallize = false;
        SecondPokemonMoveIndex = null;
        SecondPokemonTarget = null;
        SecondPokemonSwitchIndex = null;
        SecondPokemonTerastallize = false;
        Forfeit = false;
    }
}