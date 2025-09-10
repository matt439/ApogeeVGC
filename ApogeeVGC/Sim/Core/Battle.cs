using ApogeeVGC.Data;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Core;

public partial class Battle
{
    public required Library Library { get; init; }
    public required Field Field { get; init; }
    public required Side Side1 { get; init; }
    public required Side Side2 { get; init; }
    public int Turn { get; private set; } = -1; // Start at -1 for team preview turn
    public bool PrintDebug { get; set; }
    public int? BattleSeed { get; set; }
    
    // Core computed properties
    public bool IsTeamPreview => Player1State == PlayerState.TeamPreviewSelect ||
                                 Player2State == PlayerState.TeamPreviewSelect ||
                                 Player1State == PlayerState.TeamPreviewLocked ||
                                 Player2State == PlayerState.TeamPreviewLocked;
    
    private BattleContext Context => new()
    {
        Library = Library,
        Random = BattleRandom,
        PrintDebug = PrintDebug,
    };
    
    private Pokemon[] AllActivePokemon => [Side1.Slot1, Side2.Slot1];
    
    // Player state management
    private PlayerState Player1State { get; set; } = PlayerState.TeamPreviewSelect;
    private PlayerState Player2State { get; set; } = PlayerState.TeamPreviewSelect;
    private BattleChoice? Player1PendingChoice { get; set; }
    private BattleChoice? Player2PendingChoice { get; set; }
    private object ChoiceLock { get; } = new();

    // Constants
    private const int TurnLimit = 1000;
    private const double Epsilon = 1e-10;

    // Random number generation
    private Random? _battleRandom;
    private Random BattleRandom => _battleRandom ??= BattleSeed.HasValue ?
        new Random(BattleSeed.Value) : new Random();
}