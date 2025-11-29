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
        // The perspective represents the state AFTER the messages
        // We DON'T use it as the start perspective - just store it for the end
        // Note: Battle always calls UpdateUi then UpdateMessages in the same event
        
        // Start a new turn batch WITHOUT a start perspective
        // The start perspective would just be the end of the previous turn
        ChoiceCoordinator.StartTurnBatch(null);
        
        // Store the perspective for completing the batch
        _pendingEndPerspective = perspective;
        
        // Also queue legacy perspective update for team preview compatibility
        if (perspective.PerspectiveType == BattlePerspectiveType.TeamPreview)
        {
            ChoiceCoordinator.QueuePerspectiveUpdate(perspective);
        }
    }
    
    private BattlePerspective? _pendingEndPerspective;

    public void UpdateMessages(IEnumerable<BattleMessage> messages)
    {
        // Add messages to the current turn batch
        foreach (var message in messages)
        {
            ChoiceCoordinator.AddEventToTurnBatch(message);
        }
        
        // Complete the turn batch with the perspective as the end state
        ChoiceCoordinator.CompleteTurnBatch(_pendingEndPerspective);
        _pendingEndPerspective = null;
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