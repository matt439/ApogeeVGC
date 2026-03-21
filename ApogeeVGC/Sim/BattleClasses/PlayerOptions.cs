#if !HEADLESS
using ApogeeVGC.Gui;
#endif
using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.Player;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.BattleClasses;

public record PlayerOptions
{
    public PlayerType Type { get; init; } = PlayerType.Random;
    public required string Name { get; init; }
    public required IReadOnlyList<PokemonSet> Team { get; init; }
    public PrngSeed? Seed { get; init; }
#if !HEADLESS
    public BattleGame? GuiWindow { get; init; }
    public GuiChoiceCoordinator? GuiChoiceCoordinator { get; init; }
#endif
    public bool PrintDebug { get; init; }
    public MctsConfig? MctsConfig { get; init; }
}
