using ApogeeVGC.Gui;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Sim.Player;

public class PlayerGui : IPlayer
{
    public SideId SideId { get; }
    public PlayerOptions Options { get; }
    public PlayerUiType UiType => PlayerUiType.Gui;
    public IBattleController BattleController { get; }
    public BattleGame GuiWindow { get; set; }
    public GuiChoiceCoordinator ChoiceCoordinator { get; set; }
    public bool PrintDebug { get; }

    public PlayerGui(SideId sideId, PlayerOptions options, IBattleController battleController)
    {
        SideId = sideId;
        Options = options;
        BattleController = battleController;
        PrintDebug = options.PrintDebug;

        // Use the GuiWindow from options if provided, otherwise create a new one
        GuiWindow = options.GuiWindow ?? new BattleGame();

        // Use the coordinator from options, or get it from the GuiWindow
        ChoiceCoordinator = options.GuiChoiceCoordinator ?? GuiWindow.GetChoiceCoordinator();

        if (PrintDebug)
        {
            Console.WriteLine($"[PlayerGui] Constructor called for {sideId}");
            Console.WriteLine($"[PlayerGui] GuiWindow from options: {options.GuiWindow?.GetHashCode() ?? -1}");
            Console.WriteLine($"[PlayerGui] GuiWindow assigned: {GuiWindow.GetHashCode()}");
            Console.WriteLine($"[PlayerGui] ChoiceCoordinator assigned: {ChoiceCoordinator.GetHashCode()}");
        }
    }

    public async Task<Choice> GetNextChoiceAsync(IChoiceRequest choiceRequest,
        BattleRequestType requestType, BattlePerspective perspective, CancellationToken cancellationToken)
    {
        if (PrintDebug)
        {
            Console.WriteLine($"[PlayerGui] GetNextChoiceAsync called for {SideId}");
            Console.WriteLine($"[PlayerGui] Using ChoiceCoordinator: {ChoiceCoordinator.GetHashCode()}");
        }

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

        // Request the choice from the coordinator (NOT from BattleGame)
        // This avoids calling any methods on the MonoGame object from the battle thread
        Choice choice = await ChoiceCoordinator.RequestChoiceAsync(choiceRequest, requestType, perspective,
            cancellationToken);

        // Fire the choice submitted event
        ChoiceSubmitted?.Invoke(this, choice);

        return choice;
    }

    public Choice GetChoiceSync(IChoiceRequest choice, BattleRequestType requestType, BattlePerspective perspective)
    {
        throw new NotSupportedException("GUI player requires async input and cannot be used in synchronous mode");
    }

    public void UpdateUi(BattlePerspective perspective)
    {
        // Legacy method for team preview compatibility
        // For team preview, we still queue perspective updates
        if (perspective.PerspectiveType == BattlePerspectiveType.TeamPreview)
        {
            ChoiceCoordinator.QueuePerspectiveUpdate(perspective);
        }
    }

    public void UpdateEvents(IEnumerable<BattleEvent> events)
    {
        // No longer building turn batches - just forward events to coordinator
        foreach (var evt in events)
        {
            ChoiceCoordinator.AddBattleEvent(evt);
        }
    }

    public event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    public event EventHandler<Choice>? ChoiceSubmitted;

    public Task NotifyTimeoutWarningAsync(TimeSpan remainingTime)
    {
        // TODO: Show a warning in the GUI that time is running out
        if (PrintDebug)
        {
            Console.WriteLine($"[PlayerGui] Warning: {remainingTime.TotalSeconds:F0} seconds remaining");
        }
        return Task.CompletedTask;
    }

    public Task NotifyChoiceTimeoutAsync()
    {
        // TODO: Show in the GUI that the choice timed out
        if (PrintDebug)
        {
            Console.WriteLine("[PlayerGui] Choice timed out - battle will auto-choose");
        }
        return Task.CompletedTask;
    }
}