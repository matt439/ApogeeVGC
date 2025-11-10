using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Sim.BattleClasses;

/// <summary>
/// Event handlers for Battle lifecycle and player interaction.
/// These events allow external systems (like Simulator) to respond to battle state changes.
/// </summary>
public partial class Battle
{
    /// <summary>
    /// Raised when the battle needs a player to make a choice.
    /// The battle will pause until the choice is submitted via Choose().
    /// </summary>
    public event EventHandler<BattleChoiceRequestEventArgs>? ChoiceRequested;

    /// <summary>
    /// Raised when the battle has UI updates for a player.
/// </summary>
    public event EventHandler<BattleUpdateEventArgs>? UpdateRequested;

    /// <summary>
    /// Raised when the battle has ended with a winner or tie.
    /// </summary>
    public event EventHandler<BattleEndedEventArgs>? BattleEnded;

    /// <summary>
    /// Raised when all messages should be cleared from the UI.
    /// </summary>
    public event EventHandler? ClearMessagesRequested;

    /// <summary>
    /// Internal method called by Battle when it needs a player choice.
    /// Emits the ChoiceRequested event and waits for Choose() to be called.
    /// </summary>
    internal void RequestPlayerChoice(SideId sideId, IChoiceRequest request, BattleRequestType requestType)
    {
  BattlePerspective perspective = GetPerspectiveForSide(sideId);
 
        ChoiceRequested?.Invoke(this, new BattleChoiceRequestEventArgs
 {
        SideId = sideId,
      Request = request,
  RequestType = requestType,
Perspective = perspective
        });
    }

  /// <summary>
    /// Internal method called by Battle when there are UI updates.
    /// Emits the UpdateRequested event.
 /// </summary>
    internal void EmitUpdate(SideId sideId, IEnumerable<BattleMessage> messages)
  {
      BattlePerspective perspective = GetPerspectiveForSide(sideId);
    
  UpdateRequested?.Invoke(this, new BattleUpdateEventArgs
    {
       SideId = sideId,
     Perspective = perspective,
      Messages = messages
        });
    }

    /// <summary>
    /// Internal method called by Battle when it ends.
    /// Emits the BattleEnded event.
    /// </summary>
    internal void EmitBattleEnded()
{
        BattleEnded?.Invoke(this, new BattleEndedEventArgs
        {
      Winner = Winner
        });
    }

    /// <summary>
    /// Internal method called by Battle when messages should be cleared.
    /// Emits the ClearMessagesRequested event.
    /// </summary>
    internal void EmitClearMessages()
    {
        ClearMessagesRequested?.Invoke(this, EventArgs.Empty);
}
}

/// <summary>
/// Event args for choice requests from the battle.
/// </summary>
public class BattleChoiceRequestEventArgs : EventArgs
{
    required public SideId SideId { get; init; }
    required public IChoiceRequest Request { get; init; }
    required public BattleRequestType RequestType { get; init; }
    required public BattlePerspective Perspective { get; init; }
}

/// <summary>
/// Event args for UI updates from the battle.
/// </summary>
public class BattleUpdateEventArgs : EventArgs
{
    required public SideId SideId { get; init; }
    required public BattlePerspective Perspective { get; init; }
    required public IEnumerable<BattleMessage> Messages { get; init; }
}

/// <summary>
/// Event args for battle end notification.
/// </summary>
public class BattleEndedEventArgs : EventArgs
{
  public string? Winner { get; init; }
}
