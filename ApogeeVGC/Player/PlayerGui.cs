using ApogeeVGC.Gui;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Player;

public class PlayerGui : IPlayer
{
    public SideId SideId { get; }
    public PlayerOptions Options { get; }
    public PlayerUiType UiType => PlayerUiType.Gui;
    public IBattleController BattleController { get; }
    public BattleGame GuiWindow { get; set; }

    public PlayerGui(SideId sideId, PlayerOptions options, IBattleController battleController)
    {
        SideId = sideId;
        Options = options;
        BattleController = battleController;

        // Use the GuiWindow from options if provided, otherwise create a new one
        GuiWindow = options.GuiWindow ?? new BattleGame();

        Console.WriteLine($"[PlayerGui] Constructor called for {sideId}");
        Console.WriteLine($"[PlayerGui] GuiWindow from options: {options.GuiWindow?.GetHashCode() ?? -1}");
        Console.WriteLine($"[PlayerGui] GuiWindow assigned: {GuiWindow.GetHashCode()}");
    }

    public async Task<Choice> GetNextChoiceAsync(IChoiceRequest choiceRequest,
        BattleRequestType requestType, BattlePerspective perspective, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[PlayerGui] GetNextChoiceAsync called for {SideId}");
        Console.WriteLine($"[PlayerGui] GuiWindow instance: {GuiWindow.GetHashCode()}");

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

    public void UpdateMessages(IEnumerable<BattleMessage> messages)
    {
        // Forward messages to the GUI window (thread-safe)
        GuiWindow.UpdateMessages(messages);
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