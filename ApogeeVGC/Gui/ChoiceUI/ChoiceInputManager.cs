using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Gui.ChoiceUI;

/// <summary>
/// Manages user input for battle choices, supporting both mouse clicks and keyboard input.
/// Uses a thread-safe queue to communicate choices back to the async battle simulation.
/// 
/// This class is split across multiple files:
/// - ChoiceInputManager.Core.cs: Core fields, properties, and request handling
/// - ChoiceInputManager.MainBattle.cs: Main battle UI and state machine logic
/// - ChoiceInputManager.TeamPreview.cs: Team preview and legacy input processing
/// </summary>
public partial class ChoiceInputManager
{
}