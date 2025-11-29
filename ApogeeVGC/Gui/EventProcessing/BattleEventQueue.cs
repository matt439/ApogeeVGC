using ApogeeVGC.Sim.BattleClasses;

namespace ApogeeVGC.Gui.EventProcessing;

/// <summary>
/// Represents a complete turn's worth of events that need to be processed sequentially.
/// Contains the start-of-turn state, all events that occurred during the turn,
/// and the end-of-turn state for validation.
/// </summary>
public class TurnEventBatch
{
    /// <summary>
    /// The battle state at the start of the turn (source of truth baseline)
    /// </summary>
    public BattlePerspective? StartPerspective { get; set; }
    
    /// <summary>
    /// Ordered list of events that occurred during the turn
    /// </summary>
    public List<BattleMessage> Events { get; } = [];
    
    /// <summary>
    /// The battle state at the end of the turn (for validation)
    /// </summary>
    public BattlePerspective? EndPerspective { get; set; }
    
    /// <summary>
    /// Index of the current event being processed
    /// </summary>
    public int CurrentEventIndex { get; set; }
    
    /// <summary>
    /// Whether all events in this turn have been processed
    /// </summary>
    public bool IsComplete => CurrentEventIndex >= Events.Count;
    
    /// <summary>
    /// Whether validation against end perspective has been performed
    /// </summary>
    public bool HasBeenValidated { get; set; }
}

/// <summary>
/// Queue that stores battle events in turn-scoped batches for sequential processing.
/// Events are processed one at a time, with animations completing before the next event.
/// </summary>
public class BattleEventQueue
{
    private readonly Queue<TurnEventBatch> _turnBatches = new();
    private TurnEventBatch? _currentTurnBatch;
    private readonly object _lock = new();
    
    /// <summary>
    /// Enqueue a new turn batch (start perspective, events, end perspective)
    /// </summary>
    public void EnqueueTurnBatch(TurnEventBatch batch)
    {
        lock (_lock)
        {
            _turnBatches.Enqueue(batch);
        }
    }
    
    /// <summary>
    /// Get the next event to process, or null if no events are available.
    /// Automatically advances to the next turn batch when the current one is complete.
    /// </summary>
    public BattleMessage? DequeueNextEvent()
    {
        lock (_lock)
        {
            // If no current batch, try to get one from the queue
            if (_currentTurnBatch == null && _turnBatches.Count > 0)
            {
                _currentTurnBatch = _turnBatches.Dequeue();
            }
            
            // If we have a current batch and it's not complete, return the next event
            if (_currentTurnBatch != null && !_currentTurnBatch.IsComplete)
            {
                BattleMessage nextEvent = _currentTurnBatch.Events[_currentTurnBatch.CurrentEventIndex];
                _currentTurnBatch.CurrentEventIndex++;
                return nextEvent;
            }
            
            // No events available
            return null;
        }
    }
    
    /// <summary>
    /// Get the start perspective for the current turn batch
    /// </summary>
    public BattlePerspective? GetCurrentTurnStartPerspective()
    {
        lock (_lock)
        {
            return _currentTurnBatch?.StartPerspective;
        }
    }
    
    /// <summary>
    /// Get the end perspective for the current turn batch (for validation)
    /// Returns null if the turn is not yet complete or has no end perspective
    /// </summary>
    public BattlePerspective? GetCurrentTurnEndPerspective()
    {
        lock (_lock)
        {
            if (_currentTurnBatch?.IsComplete == true)
            {
                return _currentTurnBatch.EndPerspective;
            }
            return null;
        }
    }
    
    /// <summary>
    /// Mark the current turn as validated and prepare for the next turn
    /// </summary>
    public void CompleteCurrentTurn()
    {
        lock (_lock)
        {
            if (_currentTurnBatch != null)
            {
                _currentTurnBatch.HasBeenValidated = true;
                _currentTurnBatch = null;
            }
        }
    }
    
    /// <summary>
    /// Check if the current turn batch is complete and ready for validation
    /// </summary>
    public bool IsCurrentTurnComplete()
    {
        lock (_lock)
        {
            return _currentTurnBatch?.IsComplete == true;
        }
    }
    
    /// <summary>
    /// Check if there are any pending events or turn batches
    /// </summary>
    public bool HasPendingEvents()
    {
        lock (_lock)
        {
            return _turnBatches.Count > 0 || 
                   (_currentTurnBatch != null && !_currentTurnBatch.IsComplete);
        }
    }
    
    /// <summary>
    /// Clear all queued events (useful when starting a new battle)
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _turnBatches.Clear();
            _currentTurnBatch = null;
        }
    }
    
    /// <summary>
    /// Get the count of pending turn batches (for debugging)
    /// </summary>
    public int PendingTurnCount
    {
        get
        {
            lock (_lock)
            {
                int count = _turnBatches.Count;
                if (_currentTurnBatch != null && !_currentTurnBatch.IsComplete)
                {
                    count++;
                }
                return count;
            }
        }
    }
}
