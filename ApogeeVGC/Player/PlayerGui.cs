using ApogeeVGC.Gui;
using ApogeeVGC.Gui.Rendering;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Player;

public class PlayerGui(SideId sideId, PlayerOptions options, IBattleController battleController) : IPlayer
{
    public SideId SideId { get; } = sideId;
    public PlayerOptions Options { get; } = options;
    public PlayerUiType UiType => PlayerUiType.Gui;
    public IBattleController BattleController { get; } = battleController;
    public BattleGame GuiWindow { get; set; } = new();

    public Task<Choice> GetNextChoiceAsync(List<IChoiceRequest> availableChoices, BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void UpdateUi(BattlePerspective perspective)
    {
        // Convert BattlePerspective to BattleState for rendering
        BattleState battleState = ConvertPerspectiveToBattleState(perspective);

        // Update the GUI window (thread-safe)
        GuiWindow.UpdateBattleState(battleState);
    }

    private static BattleState ConvertPerspectiveToBattleState(BattlePerspective perspective)
    {
        return new BattleState
        {
            Turn = perspective.TurnCounter,
            PlayerActivePokemon = perspective.PlayerSide.Active
                .Where(p => p != null)
                .Select(p => new PokemonDisplayInfo
                {
                    Name = p!.Name,
                    Species = p.Species.ToString(),
                    CurrentHp = p.Hp,
                    MaxHp = p.MaxHp,
                    Level = p.Level
                })
                .ToList(),
            OpponentActivePokemon = perspective.OpponentSide.Active
                .Where(p => p != null)
                .Select(p => new PokemonDisplayInfo
                {
                    Name = p!.Name,
                    Species = p.Species.ToString(),
                    CurrentHp = (int)(p.HpPercentage * 100), // Estimate HP from percentage
                    MaxHp = 100, // Use normalized max HP for opponent
                    Level = p.Level
                })
                .ToList()
        };
    }

    public event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    public event EventHandler<Choice>? ChoiceSubmitted;
    public Task NotifyTimeoutWarningAsync(TimeSpan remainingTime)
    {
        throw new NotImplementedException();
    }

    public Task NotifyChoiceTimeoutAsync()
    {
        throw new NotImplementedException();
    }
}