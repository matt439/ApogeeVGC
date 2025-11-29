using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using System.Collections.Concurrent;
using static System.Threading.Thread;

namespace ApogeeVGC.Gui;

/// <summary>
/// Thread-safe coordinator for choice requests and battle events between the battle thread and GUI thread.
/// This object can be safely accessed from both threads without affecting MonoGame's state.
/// </summary>
public class GuiChoiceCoordinator
{
    private readonly ConcurrentQueue<PendingChoiceRequest> _pendingRequests = new();
    private readonly ConcurrentQueue<BattlePerspective> _pendingPerspectives = new();
    private readonly ConcurrentQueue<BattleEvent> _eventQueue = new();

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
    /// Queue a perspective update from the battle thread (legacy for team preview).
    /// This method is thread-safe and can be called from any thread.
    /// </summary>
    public void QueuePerspectiveUpdate(BattlePerspective perspective)
    {
        _pendingPerspectives.Enqueue(perspective);
    }

    /// <summary>
    /// Add a battle event to the queue.
    /// This method is thread-safe and can be called from any thread.
    /// </summary>
    public void AddBattleEvent(BattleEvent evt)
    {
        _eventQueue.Enqueue(evt);
        string messageType = evt.Message?.GetType().Name ?? "PerspectiveOnly";
        Console.WriteLine($"[GuiChoiceCoordinator] Enqueued event: {messageType}");
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
    /// Try to dequeue a battle event. Called from the GUI thread.
    /// </summary>
    public bool TryDequeueEvent(out BattleEvent? evt)
    {
        return _eventQueue.TryDequeue(out evt);
    }

    /// <summary>
    /// Get the current queue size (for debugging)
    /// </summary>
    public int QueueSize => _pendingRequests.Count;
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