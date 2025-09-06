using ApogeeVGC.Data;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Extensions;

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

    private Pokemon[] AllActivePokemon
    {
        get
        {
            List<Pokemon> activePokemons = [];
            if (Side1.Team.Slot1Pokemon is not null)
            {
                activePokemons.Add(Side1.Team.Slot1Pokemon);
            }
            if (Side1.Team.Slot2Pokemon is not null)
            {
                activePokemons.Add(Side1.Team.Slot2Pokemon);
            }
            if (Side2.Team.Slot1Pokemon is not null)
            {
                activePokemons.Add(Side2.Team.Slot1Pokemon);
            }
            if (Side2.Team.Slot2Pokemon is not null)
            {
                activePokemons.Add(Side2.Team.Slot2Pokemon);
            }
            return activePokemons.ToArray();
        }
    }
    
    // Player state management
    private PlayerState Player1State { get; set; } = PlayerState.TeamPreviewSelect;
    private PlayerState Player2State { get; set; } = PlayerState.TeamPreviewSelect;
    private Choice? Player1PendingChoice { get; set; }
    private Choice? Player2PendingChoice { get; set; }
    private Choice? Player1Slot1PendingChoice => Player1PendingChoice?.GetSlot1Choice() ?? null;
    private Choice? Player1Slot2PendingChoice => Player1PendingChoice?.GetSlot2Choice() ?? null;
    private Choice? Player2Slot1PendingChoice => Player2PendingChoice?.GetSlot1Choice() ?? null;
    private Choice? Player2Slot2PendingChoice => Player2PendingChoice?.GetSlot2Choice() ?? null;
    private List<SlotId> Player1FaintedSwitches { get; } = [];
    private List<SlotId> Player2FaintedSwitches { get; } = [];
    private object ChoiceLock { get; } = new();

    // Constants
    private const int TurnLimit = 1000;
    private const double Epsilon = 1e-10;

    // Random number generation
    private Random? _battleRandom;
    private Random BattleRandom => _battleRandom ??= BattleSeed.HasValue ?
        new Random(BattleSeed.Value) : new Random();
}