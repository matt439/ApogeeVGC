using ApogeeVGC.Gui.EventProcessing;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using System.Collections.Concurrent;
using static System.Threading.Thread;

namespace ApogeeVGC.Gui;

/// <summary>
/// Thread-safe coordinator for choice requests between the battle thread and GUI thread.
/// This object can be safely accessed from both threads without affecting MonoGame's state.
/// </summary>
public class GuiChoiceCoordinator
{
    private readonly ConcurrentQueue<PendingChoiceRequest> _pendingRequests = new();
    private readonly ConcurrentQueue<BattlePerspective> _pendingPerspectives = new();
    private readonly ConcurrentQueue<IEnumerable<BattleMessage>> _pendingMessages = new();
    
    // New turn-batched event queue
    private readonly ConcurrentQueue<TurnEventBatch> _pendingTurnBatches = new();
    private TurnEventBatch? _currentTurnBatch;

    /// <summary>
    /// Queue a choice request from the battle thread.
    /// This method is thread-safe and can be called from any thread.
    /// </summary>
    public Task<Choice> RequestChoiceAsync(IChoiceRequest request, BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
        Console.WriteLine(
            $"[GuiChoiceCoordinator] RequestChoiceAsync called from thread {CurrentThread.ManagedThreadId}");

        var completionSource = new TaskCompletionSource<Choice>();

        var pendingRequest = new PendingChoiceRequest
        {
            Request = request,
            RequestType = requestType,
            Perspective = perspective,
            CancellationToken = cancellationToken,
            CompletionSource = completionSource,
        };

        _pendingRequests.Enqueue(pendingRequest);
        Console.WriteLine(
            $"[GuiChoiceCoordinator] Queued request. Queue size: {_pendingRequests.Count}");

        return completionSource.Task;
    }

    /// <summary>
    /// Queue a perspective update from the battle thread.
    /// This method is thread-safe and can be called from any thread.
    /// </summary>
    public void QueuePerspectiveUpdate(BattlePerspective perspective)
    {
        _pendingPerspectives.Enqueue(perspective);
    }

    /// <summary>
    /// Queue messages from the battle thread.
    /// This method is thread-safe and can be called from any thread.
    /// </summary>
    public void QueueMessages(IEnumerable<BattleMessage> messages)
    {
        _pendingMessages.Enqueue(messages);
    }

    /// <summary>
    /// Try to dequeue a pending request. Called from the GUI thread.
    /// </summary>
    public bool TryDequeueRequest(out PendingChoiceRequest? request)
    {
        return _pendingRequests.TryDequeue(out request);
    }

    /// <summary>
    /// Try to dequeue a pending perspective update. Called from the GUI thread.
    /// </summary>
    public bool TryDequeuePerspective(out BattlePerspective? perspective)
    {
        return _pendingPerspectives.TryDequeue(out perspective);
    }

    /// <summary>
    /// Try to dequeue pending messages. Called from the GUI thread.
    /// </summary>
    public bool TryDequeueMessages(out IEnumerable<BattleMessage>? messages)
    {
        return _pendingMessages.TryDequeue(out messages);
    }

    /// <summary>
    /// Get the current queue size (for debugging)
    /// </summary>
    public int QueueSize => _pendingRequests.Count;
    
    // ========== NEW TURN-BATCHED EVENT API ==========
    
    /// <summary>
    /// Start a new turn batch with the start-of-turn perspective (optional)
    /// </summary>
    public void StartTurnBatch(BattlePerspective? startPerspective)
    {
        if (_currentTurnBatch != null)
        {
            Console.WriteLine("[GuiChoiceCoordinator] WARNING: Starting new turn batch without completing previous one");
            // Auto-complete the previous batch
            CompleteTurnBatch();
        }
        
        _currentTurnBatch = new TurnEventBatch
        {
            StartPerspective = startPerspective
        };
        
        Console.WriteLine("[GuiChoiceCoordinator] Started new turn batch");
    }
    
    /// <summary>
    /// Add an event to the current turn batch
    /// </summary>
    public void AddEventToTurnBatch(BattleMessage message)
    {
        if (_currentTurnBatch == null)
        {
            Console.WriteLine("[GuiChoiceCoordinator] WARNING: Adding event without active turn batch");
            // Auto-create a batch if needed
            _currentTurnBatch = new TurnEventBatch();
        }
        
        _currentTurnBatch.Events.Add(message);
    }
    
    /// <summary>
    /// Complete the current turn batch with the end-of-turn perspective and enqueue it
    /// </summary>
    public void CompleteTurnBatch(BattlePerspective? endPerspective = null)
    {
        if (_currentTurnBatch == null)
        {
            Console.WriteLine("[GuiChoiceCoordinator] WARNING: Completing turn batch but none is active");
            return;
        }
        
        _currentTurnBatch.EndPerspective = endPerspective;
        _pendingTurnBatches.Enqueue(_currentTurnBatch);
        
        Console.WriteLine($"[GuiChoiceCoordinator] Completed turn batch with {_currentTurnBatch.Events.Count} events");
        _currentTurnBatch = null;
    }
    
    /// <summary>
    /// Try to dequeue a completed turn batch
    /// </summary>
    public bool TryDequeueTurnBatch(out TurnEventBatch? batch)
    {
        return _pendingTurnBatches.TryDequeue(out batch);
    }
    
    /// <summary>
    /// Get the perspective from the current turn batch (for completing the batch)
    /// </summary>
    public BattlePerspective? GetCurrentTurnBatchPerspective()
    {
        return _currentTurnBatch?.StartPerspective;
    }
}

/// <summary>
/// Represents a choice request that needs to be processed on the GUI thread
/// </summary>
public record PendingChoiceRequest
{
    public required IChoiceRequest Request { get; init; }
    public required BattleRequestType RequestType { get; init; }
    public required BattlePerspective Perspective { get; init; }
    public required CancellationToken CancellationToken { get; init; }
    public required TaskCompletionSource<Choice> CompletionSource { get; init; }
}