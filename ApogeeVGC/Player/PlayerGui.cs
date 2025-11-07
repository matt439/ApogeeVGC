using ApogeeVGC.Gui;
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

    public async Task<Choice> GetNextChoiceAsync(IChoiceRequest choiceRequest,
        BattleRequestType requestType, BattlePerspective perspective, CancellationToken cancellationToken)
    {
        // Fire the choice requested event
        ChoiceRequested?.Invoke(this, new ChoiceRequestEventArgs
        {
            Choice = choiceRequest,
            TimeLimit = TimeSpan.FromSeconds(90), // Default 90 second timer
            RequestTime = DateTime.UtcNow,
        });

        if (choiceRequest == null)
        {
            throw new InvalidOperationException("No choice requests available");
        }

        // Request the choice from the GUI
        Choice choice = await GuiWindow.RequestChoiceAsync(choiceRequest, requestType, perspective,
            cancellationToken);

        // Fire the choice submitted event
        ChoiceSubmitted?.Invoke(this, choice);

        return choice;
    }

    public void UpdateUi(BattlePerspective perspective)
    {
        // Update the GUI window (thread-safe)
        GuiWindow.UpdateBattlePerspective(perspective);
    }

    public event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    public event EventHandler<Choice>? ChoiceSubmitted;
    public Task NotifyTimeoutWarningAsync(TimeSpan remainingTime)
    {
        // TODO: Show a warning in the GUI that time is running out
        Console.WriteLine($"[PlayerGui] Warning: {remainingTime.TotalSeconds:F0} seconds remaining");
        return Task.CompletedTask;
    }

    public Task NotifyChoiceTimeoutAsync()
    {
        // TODO: Show in the GUI that the choice timed out
        Console.WriteLine("[PlayerGui] Choice timed out - battle will auto-choose");
        return Task.CompletedTask;
    }
}